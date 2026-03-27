using Microsoft.Extensions.DependencyInjection;
using Pizzeria.Pos.Services;
using Pizzeria.Pos.Wpf.ViewModels;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Pizzeria.Pos.Wpf.Views
{
    public partial class PizzaAddonsWindow : Window
    {
        private static readonly CultureInfo PlCulture = CultureInfo.GetCultureInfo("pl-PL");

        public PizzaAddonsWindow()
        {
            InitializeComponent();

            var repo = App.ServiceProvider!.GetRequiredService<IPizzaAddonRepository>();
            DataContext = new PizzaAddonsViewModel(repo);
        }

        private PizzaAddonsViewModel? Vm => DataContext as PizzaAddonsViewModel;

        private void InputTextBoxPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not TextBox textBox)
                return;

            e.Handled = true;
            OpenKeyboardForTextBox(textBox);
        }

        private void OpenKeyboardForTextBox(TextBox textBox)
        {
            if (Vm is null)
                return;

            var isNumeric = string.Equals(
                textBox.Tag?.ToString(),
                "Numeric",
                StringComparison.OrdinalIgnoreCase);

            var initialValue = textBox.Name switch
            {
                nameof(AddonNameTextBox) => Vm.AddonName ?? string.Empty,
                nameof(AddonPriceTextBox) => Vm.AddonPrice.ToString("0.##", PlCulture),
                nameof(AddonSortOrderTextBox) => Vm.AddonSortOrder.ToString(),
                _ => textBox.Text ?? string.Empty
            };

            var keyboardWindow = new TouchKeyboardWindow(
                GetFieldTitle(textBox),
                initialValue,
                isNumeric)
            {
                Owner = this
            };

            var result = keyboardWindow.ShowDialog();
            if (result != true)
                return;

            var value = keyboardWindow.ResultText?.Trim() ?? string.Empty;

            switch (textBox.Name)
            {
                case nameof(AddonNameTextBox):
                    Vm.AddonName = value;
                    break;

                case nameof(AddonPriceTextBox):
                    if (string.IsNullOrWhiteSpace(value))
                        return;

                    var normalizedPrice = value.Replace(".", ",");
                    if (!decimal.TryParse(normalizedPrice, NumberStyles.Number, PlCulture, out var price))
                    {
                        MessageBox.Show("Podaj poprawną cenę.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    Vm.AddonPrice = price;
                    break;

                case nameof(AddonSortOrderTextBox):
                    if (string.IsNullOrWhiteSpace(value))
                        return;

                    if (!int.TryParse(value, out var sortOrder))
                    {
                        MessageBox.Show("Podaj poprawną liczbę całkowitą.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    Vm.AddonSortOrder = sortOrder;
                    break;
            }
        }

        private static string GetFieldTitle(TextBox textBox)
        {
            return textBox.Name switch
            {
                nameof(AddonNameTextBox) => "Nazwa dodatku",
                nameof(AddonPriceTextBox) => "Cena dodatku",
                nameof(AddonSortOrderTextBox) => "Kolejność sortowania",
                _ => "Wpisywanie"
            };
        }
    }
}