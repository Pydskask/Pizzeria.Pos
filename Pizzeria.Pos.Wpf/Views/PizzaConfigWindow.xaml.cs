using Microsoft.Extensions.DependencyInjection;
using Pizzeria.Pos.Services;
using Pizzeria.Pos.Wpf.Helpers;
using Pizzeria.Pos.Wpf.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Pizzeria.Pos.Wpf.Views
{
    public partial class PizzaConfigWindow : Window
    {
        public PizzaConfigWindow(string pizzaName, decimal basePrice)
            : this(pizzaName, basePrice, null)
        {
        }

        public PizzaConfigWindow(string pizzaName, decimal basePrice, PizzaConfigResult? existingConfig)
        {
            InitializeComponent();

            var addonRepository = App.ServiceProvider!.GetRequiredService<IPizzaAddonRepository>();
            DataContext = new PizzaConfigViewModel(pizzaName, basePrice, addonRepository, existingConfig);

            Loaded += PizzaConfigWindowLoaded;
        }

        public PizzaConfigResult? GetResult()
        {
            if (DataContext is PizzaConfigViewModel viewModel)
                return viewModel.Result;

            return null;
        }

        private void PizzaConfigWindowLoaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is not PizzaConfigViewModel vm)
                return;

            if (Size30Radio != null)
                Size30Radio.IsChecked = vm.SelectedSize == "30 cm";

            if (Size40Radio != null)
                Size40Radio.IsChecked = vm.SelectedSize == "40 cm";

            if (Size50Radio != null)
                Size50Radio.IsChecked = vm.SelectedSize == "50 cm";

            if (DoughTraditionalRadio != null)
                DoughTraditionalRadio.IsChecked = vm.SelectedDough == "Tradycyjne";

            if (DoughThinRadio != null)
                DoughThinRadio.IsChecked = vm.SelectedDough == "Cienkie";

            var doughFluffyRadio = FindName("DoughFluffyRadio") as RadioButton;
            if (doughFluffyRadio != null)
                doughFluffyRadio.IsChecked = vm.SelectedDough == "Puszyste";
        }

        private void Size30Checked(object sender, RoutedEventArgs e)
        {
            if (DataContext is PizzaConfigViewModel vm)
                vm.SelectedSize = "30 cm";
        }

        private void Size40Checked(object sender, RoutedEventArgs e)
        {
            if (DataContext is PizzaConfigViewModel vm)
                vm.SelectedSize = "40 cm";
        }

        private void Size50Checked(object sender, RoutedEventArgs e)
        {
            if (DataContext is PizzaConfigViewModel vm)
                vm.SelectedSize = "50 cm";
        }

        private void DoughTraditionalChecked(object sender, RoutedEventArgs e)
        {
            if (DataContext is PizzaConfigViewModel vm)
                vm.SelectedDough = "Tradycyjne";
        }

        private void DoughThinChecked(object sender, RoutedEventArgs e)
        {
            if (DataContext is PizzaConfigViewModel vm)
                vm.SelectedDough = "Cienkie";
        }

        private void DoughFluffyChecked(object sender, RoutedEventArgs e)
        {
            if (DataContext is PizzaConfigViewModel vm)
                vm.SelectedDough = "Puszyste";
        }
    }
}