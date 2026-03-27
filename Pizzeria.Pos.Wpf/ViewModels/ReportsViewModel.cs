using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pizzeria.Pos.Core.Models;
using Pizzeria.Pos.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Pizzeria.Pos.Wpf.ViewModels;

public partial class ReportsViewModel : ObservableObject
{
    private readonly IOrderRepository orderRepo;
    private readonly IPrintService printService;
    private readonly User currentUser;

    [ObservableProperty]
    private DateTime? selectedDate;

    [ObservableProperty]
    private int totalOrders;

    [ObservableProperty]
    private int paidOrders;

    [ObservableProperty]
    private int canceledOrders;

    [ObservableProperty]
    private decimal grossTotal;

    [ObservableProperty]
    private decimal cashTotal;

    [ObservableProperty]
    private decimal cardTotal;

    [ObservableProperty]
    private decimal transferTotal;

    [ObservableProperty]
    private decimal onlineTotal;

    [ObservableProperty]
    private decimal voucherTotal;

    [ObservableProperty]
    private string reportText = string.Empty;

    [ObservableProperty]
    private string statusMessage = "Wybierz datę raportu i kliknij 'Załaduj'.";

    public ReportsViewModel(IOrderRepository orderRepo, User currentUser, IPrintService printService)
    {
        this.orderRepo = orderRepo ?? throw new ArgumentNullException(nameof(orderRepo));
        this.currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
        this.printService = printService ?? throw new ArgumentNullException(nameof(printService));

        SelectedDate = DateTime.Today;
    }

    [RelayCommand]
    private void LoadDailyReport()
    {
        if (SelectedDate == null)
        {
            StatusMessage = "Wybierz datę raportu.";
            return;
        }

        var date = SelectedDate.Value.Date;
        var nextDate = date.AddDays(1);

        var allOrders = orderRepo.GetAll()
            .Where(o => o.CreatedAt >= date && o.CreatedAt < nextDate)
            .ToList();

        TotalOrders = allOrders.Count;
        PaidOrders = allOrders.Count(o => o.IsPaid && !o.IsCanceled);
        CanceledOrders = allOrders.Count(o => o.IsCanceled);

        GrossTotal = allOrders
            .Where(o => o.IsPaid && !o.IsCanceled)
            .Sum(o => o.Total);

        CashTotal = SumByMethod(allOrders, "Gotówka");
        CardTotal = SumByMethod(allOrders, "Karta");
        TransferTotal = SumByMethod(allOrders, "Przelew");
        OnlineTotal = SumByMethod(allOrders, "Online");
        VoucherTotal = SumByMethod(allOrders, "Voucher");

        ReportText = BuildReportText(allOrders, date);
        StatusMessage = $"Raport za {date:yyyy-MM-dd} załadowany. {TotalOrders} zamówień.";
    }

    [RelayCommand]
    private void PrintDailyReport()
    {
        if (SelectedDate == null)
        {
            StatusMessage = "Najpierw wybierz datę raportu.";
            return;
        }

        if (string.IsNullOrWhiteSpace(ReportText))
        {
            StatusMessage = "Najpierw załaduj raport (przycisk 'Załaduj').";
            return;
        }

        var date = SelectedDate.Value.Date;
        var title = $"Raport_dzienny_{date:yyyy-MM-dd}";

        var success = printService.PrintTextDocument(
            title: title,
            content: ReportText,
            orderId: null,
            printedBy: currentUser,
            documentType: "Raport dzienny");

        StatusMessage = success
            ? $"Raport za {date:yyyy-MM-dd} zapisany do pliku TXT."
            : "Nie udało się wygenerować wydruku raportu.";
    }

    [RelayCommand]
    private void CloseDay()
    {
        MessageBoxResult result = MessageBox.Show(
            "Czy na pewno chcesz zamknąć dzień? Ta operacja oznacza koniec zmiany i archiwizację danych.",
            "Zamknięcie dnia",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            StatusMessage = "Dzień zamknięty.";
            // Tutaj ewentualnie logika archiwizacji / resetu dnia
        }
    }

    [RelayCommand]
    private void Close()
    {
        var window = System.Windows.Application.Current.Windows
            .OfType<System.Windows.Window>()
            .FirstOrDefault(w => w.DataContext == this);

        window?.Close();
    }

    private string BuildReportText(List<Order> allOrders, DateTime date)
    {
        var sb = new StringBuilder();
        sb.AppendLine("PIZZERIA POS");
        sb.AppendLine("====================================");
        sb.AppendLine($"RAPORT DNIA {date:yyyy-MM-dd}");
        sb.AppendLine("====================================");
        sb.AppendLine();
        sb.AppendLine($"Liczba zamówień: {TotalOrders}");
        sb.AppendLine($"  ├─ Opłacone: {PaidOrders}");
        sb.AppendLine($"  └─ Anulowane: {CanceledOrders}");
        sb.AppendLine();
        sb.AppendLine($"Obrót brutto: {GrossTotal:F2} zł");
        sb.AppendLine();
        sb.AppendLine("Rozliczenie płatności:");
        sb.AppendLine($"  Gotówka:     {CashTotal:F2} zł");
        sb.AppendLine($"  Karta:       {CardTotal:F2} zł");
        sb.AppendLine($"  Przelew:     {TransferTotal:F2} zł");
        sb.AppendLine($"  Online:      {OnlineTotal:F2} zł");
        sb.AppendLine($"  Voucher:     {VoucherTotal:F2} zł");
        sb.AppendLine();
        sb.AppendLine($"Suma:          {(CashTotal + CardTotal + TransferTotal + OnlineTotal + VoucherTotal):F2} zł");
        sb.AppendLine();
        sb.AppendLine("Szczegóły zamówień:");

        foreach (var o in allOrders.OrderBy(o => o.CreatedAt).ThenBy(o => o.Id))
        {
            var status = o.IsCanceled ? "ANULOWANE"
                : o.IsPaid ? "OPŁACONE"
                : "NIEOPŁACONE";

            sb.AppendLine(
                $"#{o.Id,3} | {o.CreatedAt:HH:mm} | {o.Type} | " +
                $"{status,-12} | {o.Total,6:F2} zł | {o.PaymentMethod}");
        }

        sb.AppendLine();
        sb.AppendLine("====================================");
        sb.AppendLine($"Generowano: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine($"Operator: {currentUser.Name}");

        return sb.ToString();
    }

    private decimal SumByMethod(List<Order> orders, string method)
    {
        return orders
            .Where(o => o.IsPaid && !o.IsCanceled &&
                        string.Equals(o.PaymentMethod, method, StringComparison.OrdinalIgnoreCase))
            .Sum(o => o.Total);
    }
}