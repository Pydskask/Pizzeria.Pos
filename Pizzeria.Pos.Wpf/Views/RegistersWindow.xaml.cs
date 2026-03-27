using Pizzeria.Pos.Core.Models;
using Pizzeria.Pos.Services;
using Pizzeria.Pos.Wpf.ViewModels;
using System.Windows;

namespace Pizzeria.Pos.Wpf.Views;

public partial class RegistersWindow : Window
{
    public RegistersWindow(IOrderRepository orderRepo, IPrintService printService, User currentUser)
    {
        InitializeComponent();
        DataContext = new RegistersViewModel(orderRepo, printService, currentUser);
    }
}