using Pizzeria.Pos.Services;
using Pizzeria.Pos.Wpf.ViewModels;
using System.Windows;

namespace Pizzeria.Pos.Wpf.Views;

public partial class DeliveriesRegisterWindow : Window
{
    public DeliveriesRegisterWindow(IOrderRepository orderRepo)
    {
        InitializeComponent();
        DataContext = new DeliveriesRegisterViewModel(orderRepo);
    }
}