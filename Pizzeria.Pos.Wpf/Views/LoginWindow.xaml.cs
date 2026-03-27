using Microsoft.Extensions.DependencyInjection;
using Pizzeria.Pos.Wpf.ViewModels;
using System.Windows;

namespace Pizzeria.Pos.Wpf.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider!.GetRequiredService<LoginViewModel>();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}