using Microsoft.Extensions.DependencyInjection;
using Pizzeria.Pos.Services;
using Pizzeria.Pos.Wpf.ViewModels;
using System.Windows.Controls;

namespace Pizzeria.Pos.Wpf.Views;

public partial class SettingsPanel : UserControl
{
    public SettingsPanel()
    {
        InitializeComponent();
        var backupService = App.ServiceProvider!.GetRequiredService<IBackupService>();
        DataContext = new SettingsPanelViewModel(backupService);
    }
}