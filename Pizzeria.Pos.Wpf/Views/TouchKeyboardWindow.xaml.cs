using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Pizzeria.Pos.Wpf.Views
{
    public partial class TouchKeyboardWindow : Window
    {
        private static readonly CultureInfo PlCulture = CultureInfo.GetCultureInfo("pl-PL");
        private readonly string _fieldTitle;

        public string ResultText { get; private set; } = string.Empty;
        public bool IsNumeric { get; }
        private bool _isCapsLockEnabled;

        public TouchKeyboardWindow(string title, string initialValue, bool isNumeric)
        {
            InitializeComponent();

            IsNumeric = isNumeric;
            _fieldTitle = title ?? string.Empty;

            Title = string.IsNullOrWhiteSpace(title) ? "Klawiatura" : $"Klawiatura - {title}";

            AlphaPanel.Visibility = isNumeric ? Visibility.Collapsed : Visibility.Visible;
            NumericPanel.Visibility = isNumeric ? Visibility.Visible : Visibility.Collapsed;

            InputTextBox.Text = initialValue ?? string.Empty;
            InputTextBox.AcceptsReturn = !isNumeric;
            InputTextBox.TextWrapping = isNumeric ? TextWrapping.NoWrap : TextWrapping.Wrap;
            InputTextBox.CaretIndex = InputTextBox.Text.Length;

            UpdateLetterKeysVisualState();
            UpdateCapsButtonVisualState();

            Loaded += (_, _) =>
            {
                InputTextBox.Focus();
                InputTextBox.Select(InputTextBox.Text.Length, 0);
            };
        }

        private void Key_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Content is string value)
            {
                InsertText(TransformKeyValue(value));
            }
        }

        private void Space_Click(object sender, RoutedEventArgs e)
        {
            InsertText(" ");
        }

        private void NewLine_Click(object sender, RoutedEventArgs e)
        {
            if (!IsNumeric)
            {
                InsertText(Environment.NewLine);
            }
        }


        private void Backspace_Click(object sender, RoutedEventArgs e)
        {
            var text = InputTextBox.Text ?? string.Empty;
            var selectionStart = InputTextBox.SelectionStart;
            var selectionLength = InputTextBox.SelectionLength;

            if (selectionLength > 0)
            {
                InputTextBox.Text = text.Remove(selectionStart, selectionLength);
                InputTextBox.CaretIndex = selectionStart;
                InputTextBox.Focus();
                return;
            }

            if (selectionStart <= 0 || text.Length == 0)
                return;

            InputTextBox.Text = text.Remove(selectionStart - 1, 1);
            InputTextBox.CaretIndex = selectionStart - 1;
            InputTextBox.Focus();
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            InputTextBox.Clear();
            InputTextBox.Focus();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            var value = InputTextBox.Text ?? string.Empty;

            if (IsNumeric)
            {
                value = value.Trim();

                if (IsPriceField() && !string.IsNullOrWhiteSpace(value))
                {
                    var normalized = value.Replace('.', ',');

                    if (!decimal.TryParse(normalized, NumberStyles.Number, PlCulture, out _))
                    {
                        MessageBox.Show("Podaj poprawną wartość liczbową.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    value = normalized;
                }
            }

            ResultText = value;
            DialogResult = true;
            Close();
        }

        private bool IsPriceField()
        {
            return _fieldTitle.Contains("Cena", StringComparison.OrdinalIgnoreCase);
        }

        private void InsertText(string value)
        {
            var text = InputTextBox.Text ?? string.Empty;
            var selectionStart = InputTextBox.SelectionStart;
            var selectionLength = InputTextBox.SelectionLength;

            if (selectionLength > 0)
            {
                text = text.Remove(selectionStart, selectionLength);
            }

            text = text.Insert(selectionStart, value);
            InputTextBox.Text = text;
            InputTextBox.CaretIndex = selectionStart + value.Length;
            InputTextBox.Focus();
        }

        private string TransformKeyValue(string value)
        {
            if (IsNumeric || string.IsNullOrEmpty(value))
                return value;

            if (!char.IsLetter(value[0]))
                return value;

            if (_isCapsLockEnabled)
                return value.ToUpper(PlCulture);

            return ShouldUppercaseNextLetter()
                ? value.ToUpper(PlCulture)
                : value.ToLower(PlCulture);
        }

        private bool ShouldUppercaseNextLetter()
        {
            var text = InputTextBox.Text ?? string.Empty;
            var caret = InputTextBox.SelectionStart;

            if (caret <= 0)
                return true;

            var previousChar = text[caret - 1];
            return char.IsWhiteSpace(previousChar) || previousChar == '\n' || previousChar == '\r';
        }


        private void Caps_Click(object sender, RoutedEventArgs e)
        {
            if (IsNumeric)
                return;

            _isCapsLockEnabled = !_isCapsLockEnabled;
            UpdateLetterKeysVisualState();
            UpdateCapsButtonVisualState();
        }

        private void UpdateLetterKeysVisualState()
        {
            foreach (var child in FindVisualChildren<Button>(AlphaPanel))
            {
                if (child.Content is not string text || string.IsNullOrWhiteSpace(text))
                    continue;

                if (text.Length != 1 || !char.IsLetter(text[0]))
                    continue;

                child.Content = _isCapsLockEnabled
                    ? text.ToUpper(PlCulture)
                    : text.ToLower(PlCulture);
            }
        }

        private void UpdateCapsButtonVisualState()
        {
            if (CapsButton == null)
                return;

            CapsButton.Background = _isCapsLockEnabled
                ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2563EB"))
                : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#475569"));

            CapsButton.Content = _isCapsLockEnabled ? "CAPS ON" : "Caps";
        }

        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null)
                yield break;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T typedChild)
                    yield return typedChild;

                foreach (var descendant in FindVisualChildren<T>(child))
                    yield return descendant;
            }
        }


    }
}
