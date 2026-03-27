using Microsoft.Extensions.DependencyInjection;
using Pizzeria.Pos.Services;
using Pizzeria.Pos.Wpf.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Pizzeria.Pos.Wpf.Views
{
    public partial class DeliveryWindow : Window
    {
        private readonly bool _openPhoneOnLoad;

        public DeliveryWindow(DeliveryData? initialData = null)
        {
            InitializeComponent();

            var orderRepository = App.ServiceProvider!.GetRequiredService<IOrderRepository>();
            DataContext = new DeliveryViewModel(orderRepository, initialData);

            _openPhoneOnLoad = initialData == null;
            Loaded += DeliveryWindow_Loaded;
        }

        public DeliveryData? GetDeliveryData()
        {
            return (DataContext as DeliveryViewModel)?.Result;
        }

        private void DeliveryWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_openPhoneOnLoad)
                return;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                OpenKeyboardForTextBox(PhoneTextBox);
            }), DispatcherPriority.ApplicationIdle);
        }

        private void InputTextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not TextBox textBox)
                return;

            e.Handled = true;
            OpenKeyboardForTextBox(textBox);
        }

        private void OpenKeyboardForTextBox(TextBox textBox)
        {
            var isNumeric = string.Equals(
                textBox.Tag?.ToString(),
                "Numeric",
                StringComparison.OrdinalIgnoreCase);

            var title = GetFieldTitle(textBox);

            var keyboardWindow = new TouchKeyboardWindow(title, textBox.Text ?? string.Empty, isNumeric)
            {
                Owner = this
            };

            var result = keyboardWindow.ShowDialog();
            if (result != true)
                return;

            textBox.Text = keyboardWindow.ResultText;

            if (textBox == PhoneTextBox &&
                DataContext is DeliveryViewModel viewModel &&
                viewModel.SearchPhoneCommand.CanExecute(null))
            {
                viewModel.SearchPhoneCommand.Execute(null);
            }
        }

        private static string GetFieldTitle(TextBox textBox)
        {
            return textBox.Name switch
            {
                "PhoneTextBox" => "Telefon",
                "SymbolTextBox" => "Symbol",
                "CustomerNameTextBox" => "Nazwa klienta",
                "CityTextBox" => "Miasto",
                "StreetTextBox" => "Ulica",
                "HouseNumberTextBox" => "Numer domu",
                "ApartmentNumberTextBox" => "Numer lokalu",
                "PostalCodeTextBox" => "Kod pocztowy",
                "DeliveryPriceTextBox" => "Cena dostawy",
                "NotesTextBox" => "Uwagi",
                _ => "Wpisywanie"
            };
        }
    }
}