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

public partial class ReceiptRow : ObservableObject
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string TypeLabel { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string OperatorName { get; set; } = string.Empty;
    public string ItemsSummary { get; set; } = string.Empty;
}

public partial class ReceiptsRegisterViewModel : ObservableObject
{
    private readonly IOrderRepository orderRepo;

    [ObservableProperty] private ObservableCollection<ReceiptRow> receipts = new();
    [ObservableProperty] private ReceiptRow? selectedReceipt;
    [ObservableProperty] private DateTime dateFrom = DateTime.Today.AddDays(-7);
    [ObservableProperty] private DateTime dateTo = DateTime.Today;
    [ObservableProperty] private string selectedPayment = "Wszystkie";
    [ObservableProperty] private string detailsText = "Wybierz rachunek z listy.";
    [ObservableProperty] private string statusMessage = "Gotowe.";

    public ObservableCollection<string> PaymentOptions { get; } = new()
        { "Wszystkie", "Gotówka", "Karta", "Przelew", "Online", "Voucher" };

    public ReceiptsRegisterViewModel(IOrderRepository orderRepo)
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
            .Where(o => o.IsPaid && !o.IsCanceled && o.CreatedAt >= from && o.CreatedAt < to);

        if (SelectedPayment != "Wszystkie")
            filtered = filtered.Where(o =>
                string.Equals(o.PaymentMethod, SelectedPayment, StringComparison.OrdinalIgnoreCase));

        var rows = filtered.OrderByDescending(o => o.CreatedAt)
            .Select(o => new ReceiptRow
            {
                Id = o.Id,
                CreatedAt = o.CreatedAt,
                TypeLabel = o.Type switch { "M" => "Na miejscu", "W" => "Wynos", "D" => "Dostawa", _ => o.Type },
                Total = o.Total,
                PaymentMethod = string.IsNullOrWhiteSpace(o.PaymentMethod) ? "-" : o.PaymentMethod,
                OperatorName = o.User?.Name ?? "-",
                ItemsSummary = o.Items.Any()
                    ? string.Join(", ", o.Items.Take(3).Select(i => $"{i.Quantity}x {i.Name}"))
                    : "-"
            }).ToList();

        Receipts.Clear();
        foreach (var r in rows) Receipts.Add(r);
        StatusMessage = $"Rachunków: {Receipts.Count} | Suma: {rows.Sum(r => r.Total):F2} zł";
    }

    partial void OnSelectedReceiptChanged(ReceiptRow? value)
    {
        if (value == null) { DetailsText = "Wybierz rachunek z listy."; return; }

        var o = orderRepo.GetById(value.Id);
        if (o == null) { DetailsText = "Nie znaleziono."; return; }

        var items = o.Items.Any()
            ? string.Join("\n", o.Items.Select(i => $"  {i.Quantity}x {i.Name} — {i.Price:F2} zł"))
            : "  -";

        DetailsText =
$@"RACHUNEK #{o.Id}
Data: {o.CreatedAt:dd.MM.yyyy HH:mm}
Typ: {value.TypeLabel}
Operator: {value.OperatorName}
Płatność: {value.PaymentMethod}
Kwota: {o.Total:F2} zł

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
}