using Pizzeria.Pos.Services;
using Pizzeria.Pos.Wpf.ViewModels;
using System.Windows;

namespace Pizzeria.Pos.Wpf.Views;

public partial class CustomersRegisterWindow : Window
{
    public CustomersRegisterWindow(IOrderRepository orderRepo)
    {
        InitializeComponent();
        DataContext = new CustomersRegisterViewModel(orderRepo);
    }
}