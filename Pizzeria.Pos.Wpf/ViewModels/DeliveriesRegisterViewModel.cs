using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pizzeria.Pos.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Pizzeria.Pos.Wpf.ViewModels;

public partial class DeliveryRow : ObservableObject
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string StatusText { get; set; } = string.Empty;
}

public partial class DeliveriesRegisterViewModel : ObservableObject
{
    private readonly IOrderRepository orderRepo;

    [ObservableProperty] private ObservableCollection<DeliveryRow> deliveries = new();
    [ObservableProperty] private DeliveryRow? selectedDelivery;
    [ObservableProperty] private DateTime dateFrom = DateTime.Today.AddDays(-7);
    [ObservableProperty] private DateTime dateTo = DateTime.Today;
    [ObservableProperty] private string selectedStatus = "Wszystkie";
    [ObservableProperty] private string detailsText = "Wybierz dostawę z listy.";
    [ObservableProperty] private string statusMessage = "Gotowe.";

    public ObservableCollection<string> StatusOptions { get; } = new()
        { "Wszystkie", "Opłacone", "Nieopłacone", "Anulowane" };

    public DeliveriesRegisterViewModel(IOrderRepository orderRepo)
    {
        this.orderRepo = orderRepo;
        ApplyFilters();
    }

    [RelayCommand]
    private void ApplyFilters()
    {
        var from = DateFrom.Date;
        var to = DateTo.Date.AddDays(1);

        var filtered = orderRepo.GetAll()
            .Where(o => o.Type == "D" && o.CreatedAt >= from && o.CreatedAt < to);

        filtered = SelectedStatus switch
        {
            "Opłacone" => filtered.Where(o => o.IsPaid && !o.IsCanceled),
            "Nieopłacone" => filtered.Where(o => !o.IsPaid && !o.IsCanceled),
            "Anulowane" => filtered.Where(o => o.IsCanceled),
            _ => filtered
        };

        var rows = filtered.OrderByDescending(o => o.CreatedAt)
            .Select(o => new DeliveryRow
            {
                Id = o.Id,
                CreatedAt = o.CreatedAt,
                CustomerName = string.IsNullOrWhiteSpace(o.CustomerName) ? "-" : o.CustomerName,
                Phone = string.IsNullOrWhiteSpace(o.CustomerPhone) ? "-" : o.CustomerPhone,
                Address = BuildAddress(o),
                Total = o.Total,
                PaymentMethod = string.IsNullOrWhiteSpace(o.PaymentMethod) ? "-" : o.PaymentMethod,
                StatusText = o.IsCanceled ? "Anulowana" : o.IsPaid ? "Opłacona" : "Nieopłacona"
            }).ToList();

        Deliveries.Clear();
        foreach (var r in rows) Deliveries.Add(r);
        StatusMessage = $"Dostaw: {Deliveries.Count}";
    }

    partial void OnSelectedDeliveryChanged(DeliveryRow? value)
    {
        if (value == null) { DetailsText = "Wybierz dostawę z listy."; return; }

        var o = orderRepo.GetById(value.Id);
        if (o == null) { DetailsText = "Nie znaleziono."; return; }

        var items = o.Items.Any()
            ? string.Join("\n", o.Items.Select(i => $"  {i.Quantity}x {i.Name} — {i.Price:F2} zł"))
            : "  -";

        DetailsText =
$@"DOSTAWA #{o.Id}
Data: {o.CreatedAt:dd.MM.yyyy HH:mm}
Status: {value.StatusText}
Klient: {value.CustomerName}
Telefon: {value.Phone}
Adres: {value.Address}
Płatność: {value.PaymentMethod}
Kwota: {o.Total:F2} zł
Uwagi: {(string.IsNullOrWhiteSpace(o.Notes) ? "-" : o.Notes)}

POZYCJE:
{items}";
    }

    [RelayCommand]
    private void Close()
    {
        Application.Current.Windows
            .OfType<System.Windows.Window>()
            .FirstOrDefault(w => w.DataContext == this)
            ?.Close();
    }

    private static string BuildAddress(Pizzeria.Pos.Core.Models.Order o)
    {
        var parts = new[] { o.DeliveryStreet, o.DeliveryHouseNumber, o.DeliveryCity }
            .Where(s => !string.IsNullOrWhiteSpace(s));
        return string.Join(" ", parts).Trim() is { Length: > 0 } a ? a : o.Address;
    }
}