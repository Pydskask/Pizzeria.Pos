using Pizzeria.Pos.Wpf.ViewModels;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Pizzeria.Pos.Wpf.Views;

public partial class UsersPanel : UserControl
{
    public UsersPanel()
    {
        InitializeComponent();
    }

    private void NameTextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;
        OpenKeyboard("Alpha", "Imię", NameTextBox);
    }

    private void PinTextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;
        OpenKeyboard("Numeric", "PIN (4 cyfry)", PinTextBox);
    }

    private void OpenKeyboard(string mode, string title, TextBox targetBox)
    {
        var ownerWindow = Window.GetWindow(this);

        var keyboard = new TouchKeyboardWindow(title, targetBox.Text ?? string.Empty, mode == "Numeric")
        {
            Owner = ownerWindow
        };

        var result = keyboard.ShowDialog();
        if (result != true)
            return;

        var text = keyboard.ResultText;

        // PIN — max 4 cyfry, tylko liczby
        if (mode == "Numeric")
        {
            text = new string(text.Where(char.IsDigit).Take(4).ToArray());
        }

        targetBox.Text = text;

        // Zaktualizuj ViewModel
        if (DataContext is UserManagementViewModel vm)
        {
            if (targetBox == NameTextBox)
                vm.Name = text;
            else if (targetBox == PinTextBox)
                vm.Pin = text;
        }
    }
}