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
    private readonly IBackupService backupService;

    // ─── DZIENNY ───────────────────────────────────────────
    [ObservableProperty] private DateTime? selectedDate;
    [ObservableProperty] private int totalOrders;
    [ObservableProperty] private int paidOrders;
    [ObservableProperty] private int canceledOrders;
    [ObservableProperty] private decimal grossTotal;
    [ObservableProperty] private decimal cashTotal;
    [ObservableProperty] private decimal cardTotal;
    [ObservableProperty] private decimal transferTotal;
    [ObservableProperty] private decimal onlineTotal;
    [ObservableProperty] private decimal voucherTotal;
    [ObservableProperty] private string reportText = string.Empty;

    // ─── MIESIĘCZNY ────────────────────────────────────────
    [ObservableProperty] private int selectedMonth = DateTime.Today.Month;
    [ObservableProperty] private int selectedYear = DateTime.Today.Year;
    [ObservableProperty] private int monthTotalOrders;
    [ObservableProperty] private int monthPaidOrders;
    [ObservableProperty] private int monthCanceledOrders;
    [ObservableProperty] private decimal monthGrossTotal;
    [ObservableProperty] private decimal monthCashTotal;
    [ObservableProperty] private decimal monthCardTotal;
    [ObservableProperty] private decimal monthTransferTotal;
    [ObservableProperty] private decimal monthOnlineTotal;
    [ObservableProperty] private decimal monthVoucherTotal;
    [ObservableProperty] private string monthReportText = string.Empty;
    [ObservableProperty] private string monthStatusMessage = "Wybierz miesiąc i kliknij 'Załaduj'.";

    // ─── WSPÓLNE ───────────────────────────────────────────
    [ObservableProperty] private string statusMessage = "Wybierz datę raportu i kliknij 'Załaduj'.";

    public List<int> MonthOptions { get; } = Enumerable.Range(1, 12).ToList();
    public List<int> YearOptions { get; } = Enumerable.Range(DateTime.Today.Year - 3, 6).ToList();

    public ReportsViewModel(IOrderRepository orderRepo, User currentUser, IPrintService printService)
    {
        this.orderRepo = orderRepo ?? throw new ArgumentNullException(nameof(orderRepo));
        this.currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
        this.printService = printService ?? throw new ArgumentNullException(nameof(printService));
        SelectedDate = DateTime.Today;
    }

    // ══════════════════════════════════════════════════════
    // RAPORT DZIENNY
    // ══════════════════════════════════════════════════════

    [RelayCommand]
    private void LoadDailyReport()
    {
        if (SelectedDate == null) { StatusMessage = "Wybierz datę raportu."; return; }

        var date = SelectedDate.Value.Date;
        var allOrders = orderRepo.GetAll()
            .Where(o => o.CreatedAt >= date && o.CreatedAt < date.AddDays(1))
            .ToList();

        TotalOrders = allOrders.Count;
        PaidOrders = allOrders.Count(o => o.IsPaid && !o.IsCanceled);
        CanceledOrders = allOrders.Count(o => o.IsCanceled);
        GrossTotal = allOrders.Where(o => o.IsPaid && !o.IsCanceled).Sum(o => o.Total);
        CashTotal = SumByMethod(allOrders, "Gotówka");
        CardTotal = SumByMethod(allOrders, "Karta");
        TransferTotal = SumByMethod(allOrders, "Przelew");
        OnlineTotal = SumByMethod(allOrders, "Online");
        VoucherTotal = SumByMethod(allOrders, "Voucher");
        ReportText = BuildDailyReportText(allOrders, date);
        StatusMessage = $"Raport za {date:yyyy-MM-dd} załadowany. {TotalOrders} zamówień.";
    }

    [RelayCommand]
    private void PrintDailyReport()
    {
        if (SelectedDate == null) { StatusMessage = "Najpierw wybierz datę raportu."; return; }
        if (string.IsNullOrWhiteSpace(ReportText)) { StatusMessage = "Najpierw załaduj raport."; return; }

        var date = SelectedDate.Value.Date;
        var success = printService.PrintTextDocument(
            title: $"Raport_dzienny_{date:yyyy-MM-dd}",
            content: ReportText,
            orderId: null,
            printedBy: currentUser,
            documentType: "Raport dzienny");

        StatusMessage = success
            ? $"Raport za {date:yyyy-MM-dd} zapisany do pliku TXT."
            : "Nie udało się wygenerować wydruku raportu.";
    }

    // ══════════════════════════════════════════════════════
    // RAPORT MIESIĘCZNY
    // ══════════════════════════════════════════════════════

    [RelayCommand]
    private void LoadMonthlyReport()
    {
        var dateFrom = new DateTime(SelectedYear, SelectedMonth, 1);
        var dateTo = dateFrom.AddMonths(1);

        var allOrders = orderRepo.GetAll()
            .Where(o => o.CreatedAt >= dateFrom && o.CreatedAt < dateTo)
            .ToList();

        MonthTotalOrders = allOrders.Count;
        MonthPaidOrders = allOrders.Count(o => o.IsPaid && !o.IsCanceled);
        MonthCanceledOrders = allOrders.Count(o => o.IsCanceled);
        MonthGrossTotal = allOrders.Where(o => o.IsPaid && !o.IsCanceled).Sum(o => o.Total);
        MonthCashTotal = SumByMethod(allOrders, "Gotówka");
        MonthCardTotal = SumByMethod(allOrders, "Karta");
        MonthTransferTotal = SumByMethod(allOrders, "Przelew");
        MonthOnlineTotal = SumByMethod(allOrders, "Online");
        MonthVoucherTotal = SumByMethod(allOrders, "Voucher");
        MonthReportText = BuildMonthlyReportText(allOrders, dateFrom);
        MonthStatusMessage = $"Raport za {dateFrom:yyyy-MM} załadowany. {MonthTotalOrders} zamówień.";
    }

    [RelayCommand]
    private void PrintMonthlyReport()
    {
        if (string.IsNullOrWhiteSpace(MonthReportText)) { MonthStatusMessage = "Najpierw załaduj raport."; return; }

        var success = printService.PrintTextDocument(
            title: $"Raport_miesięczny_{SelectedYear}-{SelectedMonth:D2}",
            content: MonthReportText,
            orderId: null,
            printedBy: currentUser,
            documentType: "Raport miesięczny");

        MonthStatusMessage = success
            ? $"Raport za {SelectedYear}-{SelectedMonth:D2} zapisany do pliku TXT."
            : "Nie udało się wygenerować wydruku raportu.";
    }

    // ══════════════════════════════════════════════════════
    // WSPÓLNE
    // ══════════════════════════════════════════════════════

    [RelayCommand]
    private void CloseDay()
    {
        var today = DateTime.Today;

        // Sprawdź czy jest co zamykać
        var todayOrders = orderRepo.GetAll()
            .Where(o => o.CreatedAt.Date == today)
            .ToList();

        if (!todayOrders.Any())
        {
            MessageBox.Show("Brak zamówień z dzisiaj — nie ma czego zamykać.",
                "Zamknięcie dnia", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var paid = todayOrders.Where(o => o.IsPaid && !o.IsCanceled).ToList();
        var canceled = todayOrders.Where(o => o.IsCanceled).ToList();
        var gross = paid.Sum(o => o.Total);

        var confirm = MessageBox.Show(
            $"ZAMKNIĘCIE DNIA {today:dd.MM.yyyy}\n\n" +
            $"Zamówień łącznie:  {todayOrders.Count}\n" +
            $"  Opłaconych:      {paid.Count}\n" +
            $"  Anulowanych:     {canceled.Count}\n" +
            $"  Obrót brutto:    {gross:F2} zł\n\n" +
            "Czy na pewno chcesz zamknąć dzień?\n" +
            "Zostanie wydrukowany raport i wykonany backup bazy.",
            "Zamknięcie dnia",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (confirm != MessageBoxResult.Yes)
            return;

        // 1. Załaduj i wydrukuj raport dnia
        SelectedDate = today;
        LoadDailyReport();
        var printSuccess = printService.PrintTextDocument(
            title: $"ZAMKNIECIE_DNIA_{today:yyyy-MM-dd}",
            content: ReportText,
            orderId: null,
            printedBy: currentUser,
            documentType: "Zamknięcie dnia");

        // 2. Backup bazy
        var backupPath = backupService.CreateBackup();
        var backupInfo = backupPath != null
            ? $"✔ Backup: {System.IO.Path.GetFileName(backupPath)}"
            : "⚠ Backup nie powiódł się!";

        // 3. Podsumowanie
        var printInfo = printSuccess
            ? "✔ Raport zapisany do pliku TXT."
            : "⚠ Nie udało się zapisać raportu.";

        MessageBox.Show(
            $"Dzień {today:dd.MM.yyyy} zamknięty.\n\n{printInfo}\n{backupInfo}",
            "Zamknięcie dnia — gotowe",
            MessageBoxButton.OK,
            printSuccess && backupPath != null ? MessageBoxImage.Information : MessageBoxImage.Warning);

        StatusMessage = $"Dzień {today:dd.MM.yyyy} zamknięty. {backupInfo}";
    }

    [RelayCommand]
    private void Close()
    {
        Application.Current.Windows
            .OfType<Window>()
            .FirstOrDefault(w => w.DataContext == this)
            ?.Close();
    }

    // ══════════════════════════════════════════════════════
    // POMOCNICZE
    // ══════════════════════════════════════════════════════

    private string BuildDailyReportText(List<Order> allOrders, DateTime date)
    {
        var sb = new StringBuilder();
        sb.AppendLine("PIZZERIA POS");
        sb.AppendLine("====================================");
        sb.AppendLine($"RAPORT DNIA {date:yyyy-MM-dd}");
        sb.AppendLine("====================================");
        sb.AppendLine();
        sb.AppendLine($"Liczba zamówień: {TotalOrders}");
        sb.AppendLine($"  ├─ Opłacone:   {PaidOrders}");
        sb.AppendLine($"  └─ Anulowane:  {CanceledOrders}");
        sb.AppendLine();
        sb.AppendLine($"Obrót brutto: {GrossTotal:F2} zł");
        sb.AppendLine();
        sb.AppendLine("Rozliczenie płatności:");
        sb.AppendLine($"  Gotówka:   {CashTotal:F2} zł");
        sb.AppendLine($"  Karta:     {CardTotal:F2} zł");
        sb.AppendLine($"  Przelew:   {TransferTotal:F2} zł");
        sb.AppendLine($"  Online:    {OnlineTotal:F2} zł");
        sb.AppendLine($"  Voucher:   {VoucherTotal:F2} zł");
        sb.AppendLine($"  Suma:      {(CashTotal + CardTotal + TransferTotal + OnlineTotal + VoucherTotal):F2} zł");
        sb.AppendLine();
        sb.AppendLine("Szczegóły zamówień:");

        foreach (var o in allOrders.OrderBy(o => o.CreatedAt))
        {
            var status = o.IsCanceled ? "ANULOWANE" : o.IsPaid ? "OPŁACONE" : "NIEOPŁACONE";
            sb.AppendLine($"#{o.Id,3} | {o.CreatedAt:HH:mm} | {o.Type} | {status,-12} | {o.Total,6:F2} zł | {o.PaymentMethod}");
        }

        sb.AppendLine();
        sb.AppendLine("====================================");
        sb.AppendLine($"Generowano: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine($"Operator: {currentUser.Name}");
        return sb.ToString();
    }

    private string BuildMonthlyReportText(List<Order> allOrders, DateTime dateFrom)
    {
        var sb = new StringBuilder();
        sb.AppendLine("PIZZERIA POS");
        sb.AppendLine("====================================");
        sb.AppendLine($"RAPORT MIESIĘCZNY {dateFrom:yyyy-MM}");
        sb.AppendLine("====================================");
        sb.AppendLine();
        sb.AppendLine($"Liczba zamówień: {MonthTotalOrders}");
        sb.AppendLine($"  ├─ Opłacone:   {MonthPaidOrders}");
        sb.AppendLine($"  └─ Anulowane:  {MonthCanceledOrders}");
        sb.AppendLine();
        sb.AppendLine($"Obrót brutto: {MonthGrossTotal:F2} zł");
        sb.AppendLine();
        sb.AppendLine("Rozliczenie płatności:");
        sb.AppendLine($"  Gotówka:   {MonthCashTotal:F2} zł");
        sb.AppendLine($"  Karta:     {MonthCardTotal:F2} zł");
        sb.AppendLine($"  Przelew:   {MonthTransferTotal:F2} zł");
        sb.AppendLine($"  Online:    {MonthOnlineTotal:F2} zł");
        sb.AppendLine($"  Voucher:   {MonthVoucherTotal:F2} zł");
        sb.AppendLine($"  Suma:      {(MonthCashTotal + MonthCardTotal + MonthTransferTotal + MonthOnlineTotal + MonthVoucherTotal):F2} zł");
        sb.AppendLine();

        // Zestawienie dzień po dniu
        sb.AppendLine("Zestawienie dzienne:");
        sb.AppendLine(new string('-', 52));

        int daysInMonth = DateTime.DaysInMonth(dateFrom.Year, dateFrom.Month);
        for (int d = 1; d <= daysInMonth; d++)
        {
            var day = new DateTime(dateFrom.Year, dateFrom.Month, d);
            var dayOrders = allOrders.Where(o => o.CreatedAt.Date == day).ToList();
            if (!dayOrders.Any()) continue;

            var dayPaid = dayOrders.Where(o => o.IsPaid && !o.IsCanceled).Sum(o => o.Total);
            sb.AppendLine($"{day:dd.MM.yyyy} | {dayOrders.Count,3} zam. | {dayPaid,8:F2} zł");
        }

        sb.AppendLine(new string('-', 52));
        sb.AppendLine();
        sb.AppendLine("Podsumowanie wg typu zamówienia:");
        var byType = allOrders.Where(o => o.IsPaid && !o.IsCanceled)
            .GroupBy(o => o.Type)
            .Select(g => new { Type = g.Key, Count = g.Count(), Total = g.Sum(x => x.Total) });

        foreach (var t in byType)
        {
            var label = t.Type switch { "M" => "Na miejscu", "W" => "Wynos", "D" => "Dostawa", _ => t.Type };
            sb.AppendLine($"  {label,-14}: {t.Count,3} zam. | {t.Total:F2} zł");
        }

        sb.AppendLine();
        sb.AppendLine("====================================");
        sb.AppendLine($"Generowano: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine($"Operator: {currentUser.Name}");
        return sb.ToString();
    }

    private decimal SumByMethod(List<Order> orders, string method) =>
        orders
            .Where(o => o.IsPaid && !o.IsCanceled &&
                        string.Equals(o.PaymentMethod, method, StringComparison.OrdinalIgnoreCase))
            .Sum(o => o.Total);

    public ReportsViewModel(IOrderRepository orderRepo, User currentUser,
    IPrintService printService, IBackupService backupService)
    {
        this.orderRepo = orderRepo ?? throw new ArgumentNullException(nameof(orderRepo));
        this.currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
        this.printService = printService ?? throw new ArgumentNullException(nameof(printService));
        this.backupService = backupService ?? throw new ArgumentNullException(nameof(backupService));
        SelectedDate = DateTime.Today;
    }
}