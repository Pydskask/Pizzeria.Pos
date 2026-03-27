using System;
using System.Windows;
using System.Windows.Controls;

namespace Pizzeria.Pos.Wpf.Views
{
    public partial class ChangeOrderTypeWindow : Window
    {
        public string SelectedType { get; private set; } = string.Empty;

        public string CurrentTypeText { get; }

        public ChangeOrderTypeWindow(string currentType)
        {
            InitializeComponent();

            var normalizedType = string.IsNullOrWhiteSpace(currentType)
                ? "M"
                : currentType.Trim().ToUpperInvariant();

            CurrentTypeText = $"Aktualny typ: {GetTypeLabel(normalizedType)} ({normalizedType})";
            DataContext = this;
        }

        private void TypeButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button)
                return;

            var selected = button.Tag?.ToString()?.Trim().ToUpperInvariant();

            if (selected is not ("M" or "W" or "D"))
                return;

            SelectedType = selected;
            DialogResult = true;
            Close();
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private static string GetTypeLabel(string type)
        {
            return type switch
            {
                "M" => "Na miejscu",
                "W" => "Wynos",
                "D" => "Dostawa",
                _ => "Nieznany"
            };
        }
    }
}