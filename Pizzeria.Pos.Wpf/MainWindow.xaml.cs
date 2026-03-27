using Microsoft.Extensions.DependencyInjection;
using Pizzeria.Pos.Core.Models;
using Pizzeria.Pos.Wpf.ViewModels;
using System.Windows;

namespace Pizzeria.Pos.Wpf;

public partial class MainWindow : Window
{
    public MainWindow(User user)
    {
        InitializeComponent();

        var vm = ActivatorUtilities.CreateInstance<MainViewModel>(
            App.ServiceProvider!,
            user);

        DataContext = vm;
    }
}