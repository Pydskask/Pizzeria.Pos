using Pizzeria.Pos.Services;
using Pizzeria.Pos.Wpf.ViewModels;
using System.Windows;

namespace Pizzeria.Pos.Wpf.Views;

public partial class ReceiptsRegisterWindow : Window
{
    public ReceiptsRegisterWindow(IOrderRepository orderRepo)
    {
        InitializeComponent();
        DataContext = new ReceiptsRegisterViewModel(orderRepo);
    }
}