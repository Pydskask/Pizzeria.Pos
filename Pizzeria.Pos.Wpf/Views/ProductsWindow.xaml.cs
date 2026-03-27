using Microsoft.Extensions.DependencyInjection;
using Pizzeria.Pos.Services;
using Pizzeria.Pos.Wpf.ViewModels;
using System.Globalization;
using System.Windows;
using System.Windows.Input;

namespace Pizzeria.Pos.Wpf.Views
{
    public partial class ProductsWindow : Window
    {
        private static readonly CultureInfo PlCulture = CultureInfo.GetCultureInfo("pl-PL");

        public ProductsWindow()
        {
            InitializeComponent();

            var productRepo = App.ServiceProvider!.GetRequiredService<IProductRepository>();
            DataContext = new ProductsViewModel(productRepo);
        }

        private ProductsViewModel? Vm => DataContext as ProductsViewModel;

        private void NameTextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            if (Vm == null)
                return;

            var keyboard = new TouchKeyboardWindow(
                "Nazwa produktu",
                Vm.NewProductName,
                false)
            {
                Owner = this
            };

            if (keyboard.ShowDialog() == true)
            {
                Vm.NewProductName = (keyboard.ResultText ?? string.Empty).Trim();
            }
        }

        private void PriceTextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            if (Vm == null)
                return;

            var initialPriceText = Vm.NewProductPrice.ToString("0.##", PlCulture);

            var keyboard = new TouchKeyboardWindow(
                "Cena produktu",
                initialPriceText,
                true)
            {
                Owner = this
            };

            if (keyboard.ShowDialog() != true)
                return;

            var result = (keyboard.ResultText ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(result))
                return;

            var normalized = result.Replace('.', ',');

            if (decimal.TryParse(normalized, NumberStyles.Number, PlCulture, out var price))
            {
                Vm.NewProductPrice = price;
            }
            else
            {
                MessageBox.Show(
                    "Podaj poprawną cenę produktu.",
                    "Błąd",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }
    }
}