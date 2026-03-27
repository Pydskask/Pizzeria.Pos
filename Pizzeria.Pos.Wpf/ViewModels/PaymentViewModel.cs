using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace Pizzeria.Pos.Wpf.ViewModels;

public class PaymentCartItem
{
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal => UnitPrice * Quantity;
}

public enum PaymentInputTarget
{
    AmountPaid,
    CustomDiscount
}

public partial class PaymentViewModel : ObservableObject
{
    private static readonly CultureInfo PlCulture = CultureInfo.GetCultureInfo("pl-PL");

    public ObservableCollection<PaymentCartItem> CartItems { get; } = new();

    [ObservableProperty]
    private decimal orderTotal;

    [ObservableProperty]
    private string amountPaidText = "0";

    [ObservableProperty]
    private string selectedMethod = "Gotówka";

    [ObservableProperty]
    private string customDiscountText = "0";

    [ObservableProperty]
    private PaymentInputTarget activeInput = PaymentInputTarget.AmountPaid;

    public PaymentResult? Result { get; private set; }

    public bool CanEditAmountPaid => SelectedMethod == "Gotówka";

    public decimal DiscountAmount
    {
        get
        {
            var customDiscount = ParseDecimal(CustomDiscountText) ?? 0m;
            return customDiscount > OrderTotal ? OrderTotal : customDiscount;
        }
    }

    public decimal FinalTotal
    {
        get
        {
            var calculated = OrderTotal - DiscountAmount;
            return calculated < 0 ? 0 : calculated;
        }
    }

    public decimal AmountPaid
    {
        get
        {
            var value = ParseDecimal(AmountPaidText);
            return value ?? 0m;
        }
    }

    public decimal RemainingAmount => AmountPaid >= FinalTotal ? 0m : FinalTotal - AmountPaid;
    public decimal ChangeAmount => AmountPaid > FinalTotal ? AmountPaid - FinalTotal : 0m;

    public PaymentViewModel(IEnumerable<PaymentCartItem> items, decimal total, string? defaultMethod = null)
    {
        foreach (var item in items)
            CartItems.Add(item);

        OrderTotal = total;

        if (!string.IsNullOrWhiteSpace(defaultMethod))
            SelectedMethod = defaultMethod;

        if (SelectedMethod != "Gotówka")
            SyncNonCashAmount();
    }

    partial void OnAmountPaidTextChanged(string value)
    {
        OnPropertyChanged(nameof(AmountPaid));
        OnPropertyChanged(nameof(RemainingAmount));
        OnPropertyChanged(nameof(ChangeAmount));
    }

    partial void OnSelectedMethodChanged(string value)
    {
        OnPropertyChanged(nameof(CanEditAmountPaid));

        SyncNonCashAmount();

        if (!CanEditAmountPaid && ActiveInput == PaymentInputTarget.AmountPaid)
            ActiveInput = PaymentInputTarget.CustomDiscount;
    }

    partial void OnCustomDiscountTextChanged(string value)
    {
        RefreshTotals();
    }

    private decimal? ParseDecimal(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return null;

        var normalized = text.Replace(".", ",");
        return decimal.TryParse(normalized, NumberStyles.Number, PlCulture, out var value)
            ? value
            : null;
    }

    private void RefreshTotals()
    {
        OnPropertyChanged(nameof(DiscountAmount));
        OnPropertyChanged(nameof(FinalTotal));
        OnPropertyChanged(nameof(RemainingAmount));
        OnPropertyChanged(nameof(ChangeAmount));
        SyncNonCashAmount();
    }

    private void SyncNonCashAmount()
    {
        if (SelectedMethod != "Gotówka")
            AmountPaidText = FinalTotal.ToString("0.00", PlCulture);
    }

    private string GetActiveText()
    {
        return ActiveInput switch
        {
            PaymentInputTarget.AmountPaid => AmountPaidText,
            PaymentInputTarget.CustomDiscount => CustomDiscountText,
            _ => AmountPaidText
        };
    }

    private void SetActiveText(string value)
    {
        switch (ActiveInput)
        {
            case PaymentInputTarget.AmountPaid:
                AmountPaidText = value;
                break;
            case PaymentInputTarget.CustomDiscount:
                CustomDiscountText = value;
                break;
        }
    }

    [RelayCommand]
    private void FocusAmountPaid()
    {
        if (!CanEditAmountPaid)
            return;

        ActiveInput = PaymentInputTarget.AmountPaid;
    }

    [RelayCommand]
    private void FocusCustomDiscount()
    {
        ActiveInput = PaymentInputTarget.CustomDiscount;
    }

    [RelayCommand]
    private void AppendDigit(string? digit)
    {
        if (string.IsNullOrWhiteSpace(digit))
            return;

        if (ActiveInput == PaymentInputTarget.AmountPaid && !CanEditAmountPaid)
            return;

        var current = GetActiveText();

        if (string.IsNullOrWhiteSpace(current))
            current = "0";

        if (digit == "," || digit == ".")
        {
            if (current.Contains(",") || current.Contains("."))
                return;

            SetActiveText(current + ",");
            return;
        }

        if (current == "0")
            SetActiveText(digit);
        else
            SetActiveText(current + digit);
    }

    [RelayCommand]
    private void Backspace()
    {
        if (ActiveInput == PaymentInputTarget.AmountPaid && !CanEditAmountPaid)
            return;

        var current = GetActiveText();

        if (string.IsNullOrEmpty(current) || current.Length == 1)
        {
            SetActiveText("0");
            return;
        }

        SetActiveText(current[..^1]);
    }

    [RelayCommand]
    private void ClearActiveInput()
    {
        if (ActiveInput == PaymentInputTarget.AmountPaid && !CanEditAmountPaid)
            return;

        SetActiveText("0");
    }

    [RelayCommand]
    private void SelectCash()
    {
        var wasNonCash = SelectedMethod != "Gotówka";

        SelectedMethod = "Gotówka";
        ActiveInput = PaymentInputTarget.AmountPaid;

        if (wasNonCash)
            AmountPaidText = "0";
    }

    [RelayCommand]
    private void SelectCard()
    {
        SelectedMethod = "Karta";
    }

    [RelayCommand]
    private void SelectTransfer()
    {
        SelectedMethod = "Przelew";
    }

    [RelayCommand]
    private void SelectOnline()
    {
        SelectedMethod = "Online";
    }

    [RelayCommand]
    private void SelectVoucher()
    {
        SelectedMethod = "Voucher";
    }

    [RelayCommand]
    private void ApplyDiscountPercent(string? percentText)
    {
        if (!int.TryParse(percentText, out var percent))
            return;

        var amount = decimal.Round(OrderTotal * percent / 100m, 2);
        CustomDiscountText = amount.ToString("0.##", PlCulture);
        ActiveInput = PaymentInputTarget.AmountPaid;
        RefreshTotals();
    }

    [RelayCommand]
    private void ClearDiscount()
    {
        CustomDiscountText = "0";
        RefreshTotals();
        ActiveInput = PaymentInputTarget.AmountPaid;
    }

    [RelayCommand]
    private void FinishPayment()
    {
        if (RemainingAmount > 0)
        {
            MessageBox.Show("Kwota wpłacona jest za mała.");
            return;
        }

        Result = new PaymentResult
        {
            IsPaid = true,
            PaymentMethod = SelectedMethod,
            FinalTotal = FinalTotal,
            DiscountAmount = DiscountAmount,
            AmountPaid = AmountPaid
        };

        MessageBox.Show(
            $"Płatność zatwierdzona.\nForma płatności: {SelectedMethod}\nDo zapłaty: {FinalTotal:F2} zł");

        var window = Application.Current.Windows
            .OfType<Window>()
            .FirstOrDefault(w => w.DataContext == this);

        if (window != null)
        {
            window.DialogResult = true;
            window.Close();
        }
    }
}
