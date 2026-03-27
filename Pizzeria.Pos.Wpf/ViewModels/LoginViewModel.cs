using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Pizzeria.Pos.Core.Models;
using Pizzeria.Pos.Services;
using Pizzeria.Pos.Wpf.Views;
using Pizzeria.Pos.Wpf.ViewModels;
using Pizzeria.Pos.Wpf;
using System;
using System.Linq;
using System.Text;  // Dla string('*')
using System.Threading.Tasks;
using System.Windows;

namespace Pizzeria.Pos.Wpf.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IUserRepository _userRepo;
    [ObservableProperty] private string enteredPin = string.Empty;
    [ObservableProperty] private string status = "Baza gotowa. Wpisz PIN.";
    [ObservableProperty] private string displayedPin = "PIN: ____";  // Observable!

    [RelayCommand]
    private void AppendDigit(object param)
    {
        if (param is string digit && digit.Length == 1 && EnteredPin.Length < 4)
        {
            EnteredPin += digit;
            DisplayedPin = $"PIN: {new string('*', EnteredPin.Length)}{new string('_', 4 - EnteredPin.Length)}";
        }
    }

    [RelayCommand]
    private void ClearPin()
    {
        EnteredPin = "";
        DisplayedPin = "PIN: ____";
        Status = "Wpisz PIN";
    }

    public LoginViewModel(IUserRepository userRepo)
    {
        _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
        Status = "Wpisz PIN (baza OK)"; // Bez GetAll() w ctor!
    }

    [RelayCommand]
    private void Login()
    {
        var user = _userRepo.GetByPin(EnteredPin);

        if (user != null)
        {
            Status = $"Zalogowano: {user.Name}";

            var loginWindow = Application.Current.Windows
                .OfType<LoginWindow>()
                .FirstOrDefault();

            var mainWindow = new MainWindow(user);
            Application.Current.MainWindow = mainWindow;
            mainWindow.Show();

            loginWindow?.Close();
        }
        else
        {
            Status = "BŁĄD: Zły PIN!";
            Task.Delay(1500).ContinueWith(t =>
                Application.Current.Dispatcher.Invoke((Action)ClearPin));
        }
    }
    [RelayCommand]
    private void TestRepoCommand()
    {
        try
        {
            Status = $"Repo OK, users: {_userRepo.GetAll().Count}";
        }
        catch (Exception ex)
        {
            Status = $"Baza error: {ex.Message}";
        }
    }


}
