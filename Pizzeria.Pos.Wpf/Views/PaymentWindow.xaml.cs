using Pizzeria.Pos.Wpf.ViewModels;
using System.Collections.Generic;
using System.Windows;

namespace Pizzeria.Pos.Wpf.Views;

public partial class PaymentWindow : Window
{
    public PaymentWindow(List<PaymentCartItem> items, decimal total, string? defaultMethod = null)
    {
        InitializeComponent();
        DataContext = new PaymentViewModel(items, total, defaultMethod);
    }

    public PaymentResult? GetResult()
    {
        return (DataContext as PaymentViewModel)?.Result;
    }
}