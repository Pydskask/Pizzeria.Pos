using Microsoft.Extensions.DependencyInjection;
using Pizzeria.Pos.Core.Models;
using Pizzeria.Pos.Services;
using Pizzeria.Pos.Wpf.ViewModels;
using System.Windows;

namespace Pizzeria.Pos.Wpf.Views;

public partial class ReportsWindow : Window
{
    public ReportsWindow(IOrderRepository orderRepo, User currentUser, int initialTab = 0)
    {
        InitializeComponent();

        var printService = App.ServiceProvider!.GetRequiredService<IPrintService>();
        var backupService = App.ServiceProvider!.GetRequiredService<IBackupService>();
        DataContext = new ReportsViewModel(orderRepo, currentUser, printService, backupService);

        MainTabControl.SelectedIndex = initialTab;
    }
}