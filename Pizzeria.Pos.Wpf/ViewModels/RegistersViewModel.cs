using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pizzeria.Pos.Core.Models;
using Pizzeria.Pos.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace Pizzeria.Pos.Wpf.ViewModels;

public partial class RegisterOrderRow : ObservableObject
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Type { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string OperatorName { get; set; } = string.Empty;
    public bool IsPaid { get; set; }
    public bool IsCanceled { get; set; }
    public string StatusText { get; set; } = string.Empty;
    public string ItemsSummary { get; set; } = string.Empty;
    public string TypeLabel => Type switch
    {
        "M" => "Na miejscu",
        "W" => "Wynos",
        "D" => "Dostawa",
        _ => Type
    };
}

public partial class RegistersViewModel : ObservableObject
{
    private readonly IOrderRepository orderRepo;
    private readonly IPrintService printService;
    private readonly User currentUser;

    private List<Order> _allOrders = new();

    [ObservableProperty]
    private ObservableCollection<RegisterOrderRow> orders = new();

    [ObservableProperty]
    private RegisterOrderRow? selectedOrder;

    [ObservableProperty]
    private DateTime dateFrom = DateTime.Today.AddDays(-7);

    [ObservableProperty]
    private DateTime dateTo = DateTime.Today;

    [ObservableProperty]
    private string selectedType = "Wszystkie";

    [ObservableProperty]
    private string selectedStatus = "Wszystkie";

    [ObservableProperty]
    private string detailsText = "Wybierz zamówienie z listy.";

    [ObservableProperty]
    private string statusMessage = "Gotowe.";



    public ObservableCollection<string> TypeOptions { get; } = new()
    {
    "Wszystkie", "Na miejscu", "Wynos", "Dostawa"
    };

    public ObservableCollection<string> StatusOptions { get; } = new()
    {
        "Wszystkie",
        "Aktywne",
        "Opłacone",
        "Anulowane"
    };

    public RegistersViewModel(IOrderRepository orderRepo, IPrintService printService, User currentUser)
    {
        this.orderRepo = orderRepo ?? throw new ArgumentNullException(nameof(orderRepo));
        this.printService = printService ?? throw new ArgumentNullException(nameof(printService));
        this.currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));

        LoadOrders();
    }

    [RelayCommand]
    private void LoadOrders()
    {
        _allOrders = orderRepo.GetAll()
            .OrderByDescending(o => o.CreatedAt)
            .ThenByDescending(o => o.Id)
            .ToList();

        ApplyFilters();
        StatusMessage = $"Załadowano {_allOrders.Count} zamówień.";
    }

    [RelayCommand]
    private void ApplyFilters()
    {
        var previouslySelectedId = SelectedOrder?.Id;

        var from = DateFrom.Date;
        var to = DateTo.Date.AddDays(1);

        var filtered = _allOrders
            .Where(o => o.CreatedAt >= from && o.CreatedAt < to);

        if (SelectedType != "Wszystkie")
        {
            var typeCode = SelectedType switch
            {
                "Na miejscu" => "M",
                "Wynos" => "W",
                "Dostawa" => "D",
                _ => SelectedType
            };
            filtered = filtered.Where(o => o.Type == typeCode);
        }

        filtered = SelectedStatus switch
        {
            "Aktywne" => filtered.Where(o => !o.IsPaid && !o.IsCanceled),
            "Opłacone" => filtered.Where(o => o.IsPaid && !o.IsCanceled),
            "Anulowane" => filtered.Where(o => o.IsCanceled),
            _ => filtered
        };

        var rows = filtered
            .Select(o => new RegisterOrderRow
            {
                Id = o.Id,
                CreatedAt = o.CreatedAt,
                Type = o.Type,
                Total = o.Total,
                PaymentMethod = string.IsNullOrWhiteSpace(o.PaymentMethod) ? "-" : o.PaymentMethod,
                OperatorName = o.User?.Name ?? "-",
                IsPaid = o.IsPaid,
                IsCanceled = o.IsCanceled,
                StatusText = GetStatusText(o),
                ItemsSummary = o.Items.Any()
                    ? string.Join(", ", o.Items.Take(3).Select(i => $"{i.Quantity}x {i.Name}"))
                    : "-"
            })
            .ToList();

        Orders.Clear();
        foreach (var row in rows)
            Orders.Add(row);

        SelectedOrder = previouslySelectedId.HasValue
            ? Orders.FirstOrDefault(x => x.Id == previouslySelectedId.Value)
            : null;

        if (SelectedOrder == null)
            DetailsText = "Wybierz zamówienie z listy.";

        StatusMessage = $"Po filtrach: {Orders.Count} zamówień.";
    }

    partial void OnSelectedOrderChanged(RegisterOrderRow? value)
    {
        if (value == null)
        {
            DetailsText = "Wybierz zamówienie z listy.";
            return;
        }

        var order = orderRepo.GetById(value.Id);
        if (order == null)
        {
            DetailsText = "Nie znaleziono zamówienia.";
            return;
        }

        var itemsText = order.Items.Any()
            ? string.Join(Environment.NewLine, order.Items.Select(i =>
                $"- {i.Quantity}x {i.Name} | {i.Price:F2} zł"))
            : "-";

        DetailsText =
$@"ZAMÓWIENIE #{order.Id}
Data: {order.CreatedAt:yyyy-MM-dd HH:mm}
Typ: {GetTypeLabel(order.Type)}
Status: {GetStatusText(order)}
Operator: {order.User?.Name ?? "-"}
Płatność: {(string.IsNullOrWhiteSpace(order.PaymentMethod) ? "-" : order.PaymentMethod)}
Kwota: {order.Total:F2} zł

Klient: {Safe(order.CustomerName)}
Telefon: {Safe(order.CustomerPhone)}
Adres: {Safe(order.Address)}
Uwagi: {Safe(order.Notes)}

POZYCJE:
{itemsText}";
    }

    [RelayCommand]
    private void ClearFilters()
    {
        DateFrom = DateTime.Today.AddDays(-7);
        DateTo = DateTime.Today;
        SelectedType = "Wszystkie";
        SelectedStatus = "Wszystkie";
        ApplyFilters();
    }

    [RelayCommand]
    private void ReprintKitchenBon()
    {
        if (SelectedOrder == null)
        {
            MessageBox.Show("Najpierw zaznacz zamówienie.");
            return;
        }

        var order = orderRepo.GetById(SelectedOrder.Id);
        if (order == null)
        {
            MessageBox.Show("Nie znaleziono zamówienia.");
            return;
        }

        var success = printService.PrintTextDocument(
            title: $"BON_KUCHNIA_{order.Id}_{DateTime.Now:yyyyMMdd_HHmmss}",
            content: BuildKitchenBon(order, true),
            orderId: order.Id,
            printedBy: currentUser,
            documentType: "Bon kuchenny");

        StatusMessage = success
            ? $"Wysłano BON kuchenny dla zamówienia #{order.Id}."
            : $"Błąd drukowania BON-u kuchennego dla zamówienia #{order.Id}.";
    }

    [RelayCommand]
    private void ReprintDeliveryBon()
    {
        if (SelectedOrder == null)
        {
            MessageBox.Show("Najpierw zaznacz zamówienie.");
            return;
        }

        var order = orderRepo.GetById(SelectedOrder.Id);
        if (order == null)
        {
            MessageBox.Show("Nie znaleziono zamówienia.");
            return;
        }

        if (order.Type != "D")
        {
            MessageBox.Show("BON dostawy można wydrukować tylko dla zamówienia typu Dostawa.");
            return;
        }

        var success = printService.PrintTextDocument(
            title: $"BON_DOSTAWA_{order.Id}_{DateTime.Now:yyyyMMdd_HHmmss}",
            content: BuildDeliveryBon(order, true),
            orderId: order.Id,
            printedBy: currentUser,
            documentType: "Bon dostawy");

        StatusMessage = success
            ? $"Wysłano BON dostawy dla zamówienia #{order.Id}."
            : $"Błąd drukowania BON-u dostawy dla zamówienia #{order.Id}.";
    }

    [RelayCommand]
    private void ReprintReceipt()
    {
        if (SelectedOrder == null)
        {
            MessageBox.Show("Najpierw zaznacz zamówienie.");
            return;
        }

        var order = orderRepo.GetById(SelectedOrder.Id);
        if (order == null)
        {
            MessageBox.Show("Nie znaleziono zamówienia.");
            return;
        }

        if (!order.IsPaid)
        {
            MessageBox.Show("Paragon można wydrukować ponownie tylko dla opłaconego zamówienia.");
            return;
        }

        var success = printService.PrintOrderReceipt(order, currentUser);

        StatusMessage = success
            ? $"Wysłano paragon dla zamówienia #{order.Id}."
            : $"Błąd drukowania paragonu dla zamówienia #{order.Id}.";
    }

    [RelayCommand]
    private void RefreshSelected()
    {
        if (SelectedOrder == null)
        {
            LoadOrders();
            return;
        }

        var selectedId = SelectedOrder.Id;
        LoadOrders();
        SelectedOrder = Orders.FirstOrDefault(x => x.Id == selectedId);
    }

    [RelayCommand]
    private void Close()
    {
        var window = Application.Current.Windows
            .OfType<Window>()
            .FirstOrDefault(w => w.DataContext == this);

        window?.Close();
    }

    private static string GetStatusText(Order order)
    {
        if (order.IsCanceled)
            return "Anulowane";

        if (order.IsPaid)
            return "Opłacone";

        return "Aktywne";
    }

    private static string GetTypeLabel(string type)
    {
        return type switch
        {
            "M" => "Na miejscu",
            "W" => "Wynos",
            "D" => "Dostawa",
            _ => type
        };
    }

    private static string Safe(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? "-" : value;
    }

    private static string BuildKitchenBon(Order order, bool isReprint)
    {
        var sb = new StringBuilder();

        sb.AppendLine("PIZZERIA POS");
        sb.AppendLine(isReprint ? "BON KUCHENNY - DUPLIKAT" : "BON KUCHENNY");
        sb.AppendLine($"Zamówienie #{order.Id}");
        sb.AppendLine($"Data: {order.CreatedAt:yyyy-MM-dd HH:mm}");
        sb.AppendLine($"Typ: {GetTypeLabel(order.Type)}");
        sb.AppendLine(new string('-', 32));

        foreach (var item in order.Items)
            sb.AppendLine($"{item.Quantity}x {item.Name}");

        if (!string.IsNullOrWhiteSpace(order.Notes))
        {
            sb.AppendLine(new string('-', 32));
            sb.AppendLine("UWAGI:");
            sb.AppendLine(order.Notes);
        }

        return sb.ToString();
    }

    private static string BuildDeliveryBon(Order order, bool isReprint)
    {
        var sb = new StringBuilder();

        sb.AppendLine("PIZZERIA POS");
        sb.AppendLine(isReprint ? "BON DOSTAWY - DUPLIKAT" : "BON DOSTAWY");
        sb.AppendLine($"Zamówienie #{order.Id}");
        sb.AppendLine($"Data: {order.CreatedAt:yyyy-MM-dd HH:mm}");
        sb.AppendLine($"Płatność: {(string.IsNullOrWhiteSpace(order.PaymentMethod) ? "-" : order.PaymentMethod)}");
        sb.AppendLine(new string('-', 32));

        foreach (var item in order.Items)
            sb.AppendLine($"{item.Quantity}x {item.Name}");

        sb.AppendLine(new string('-', 32));
        sb.AppendLine($"Klient: {Safe(order.CustomerName)}");
        sb.AppendLine($"Telefon: {Safe(order.CustomerPhone)}");
        sb.AppendLine($"Adres: {Safe(order.Address)}");
        sb.AppendLine($"Kwota: {order.Total:F2} zł");

        if (!string.IsNullOrWhiteSpace(order.Notes))
        {
            sb.AppendLine("Uwagi:");
            sb.AppendLine(order.Notes);
        }

        return sb.ToString();
    }
}