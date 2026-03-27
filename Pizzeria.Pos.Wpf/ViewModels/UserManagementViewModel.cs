using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pizzeria.Pos.Core.Models;
using Pizzeria.Pos.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Xml.Linq;

namespace Pizzeria.Pos.Wpf.ViewModels;

public partial class UserManagementViewModel : ObservableObject
{
    private readonly IUserRepository _userRepository;
    private readonly User _currentUser;

    public ObservableCollection<User> Users { get; } = new();

    public ObservableCollection<string> Roles { get; } = new()
    {
        "Manager",
        "Kelner"
    };

    [ObservableProperty]
    private User? selectedUser;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string pin = string.Empty;

    [ObservableProperty]
    private string selectedRole = "Kelner";

    [ObservableProperty]
    private bool isActive = true;

    [ObservableProperty]
    private string statusMessage = string.Empty;

    public bool CanEdit => _currentUser.Role == "Manager";

    public UserManagementViewModel(IUserRepository userRepository, User currentUser)
    {
        _userRepository = userRepository;
        _currentUser = currentUser;
        LoadUsers();
    }

    partial void OnSelectedUserChanged(User? value)
    {
        if (value == null)
            return;

        Name = value.Name;
        Pin = value.Pin;
        SelectedRole = value.Role;
        IsActive = value.IsActive;
        StatusMessage = $"Wybrano użytkownika: {value.Name}";
    }

    [RelayCommand]
    private void LoadUsers()
    {
        Users.Clear();

        foreach (var user in _userRepository.GetAll())
            Users.Add(user);
    }

    [RelayCommand]
    private void NewUser()
    {
        SelectedUser = null;
        Name = string.Empty;
        Pin = string.Empty;
        SelectedRole = "Kelner";
        IsActive = true;
        StatusMessage = "Nowy użytkownik.";
    }

    [RelayCommand]
    private void SaveUser()
    {
        if (!CanEdit)
        {
            MessageBox.Show("Tylko Manager może edytować użytkowników.");
            return;
        }

        if (string.IsNullOrWhiteSpace(Name))
        {
            MessageBox.Show("Podaj nazwę użytkownika.");
            return;
        }

        if (Pin.Length != 4 || !Pin.All(char.IsDigit))
        {
            MessageBox.Show("PIN musi mieć dokładnie 4 cyfry.");
            return;
        }

        if (SelectedUser == null)
        {
            var existing = _userRepository.GetAll().FirstOrDefault(u => u.Pin == Pin);
            if (existing != null)
            {
                MessageBox.Show("Istnieje już użytkownik z takim PIN-em.");
                return;
            }

            _userRepository.Add(new User
            {
                Name = Name.Trim(),
                Pin = Pin,
                Role = SelectedRole,
                IsActive = IsActive
            });

            StatusMessage = "Dodano użytkownika.";
        }
        else
        {
            var duplicatePin = _userRepository.GetAll()
                .FirstOrDefault(u => u.Pin == Pin && u.Id != SelectedUser.Id);

            if (duplicatePin != null)
            {
                MessageBox.Show("Inny użytkownik ma już taki PIN.");
                return;
            }

            SelectedUser.Name = Name.Trim();
            SelectedUser.Pin = Pin;
            SelectedUser.Role = SelectedRole;
            SelectedUser.IsActive = IsActive;

            _userRepository.Update(SelectedUser);
            StatusMessage = "Zapisano zmiany użytkownika.";
        }

        LoadUsers();
        NewUser();
    }

    [RelayCommand]
    private void DeactivateUser()
    {
        if (!CanEdit)
        {
            MessageBox.Show("Tylko Manager może dezaktywować użytkowników.");
            return;
        }

        if (SelectedUser == null)
        {
            MessageBox.Show("Najpierw wybierz użytkownika.");
            return;
        }

        if (SelectedUser.Id == _currentUser.Id)
        {
            MessageBox.Show("Nie możesz dezaktywować aktualnie zalogowanego użytkownika.");
            return;
        }

        SelectedUser.IsActive = false;
        _userRepository.Update(SelectedUser);

        StatusMessage = "Użytkownik został dezaktywowany.";
        LoadUsers();
        NewUser();
    }

    [RelayCommand]
    private void ClearForm()
    {
        NewUser();
    }
}