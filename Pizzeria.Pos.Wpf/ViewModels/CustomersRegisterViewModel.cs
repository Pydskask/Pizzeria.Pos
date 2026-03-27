using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pizzeria.Pos.Core.Models;
using Pizzeria.Pos.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace Pizzeria.Pos.Wpf.ViewModels;

public partial class CustomerRow : ObservableObject
{
    public string Phone { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int OrderCount { get; set; }
    public decimal TotalSpent { get; set; }
    public DateTime LastOrderDate { get; set; }
}

public partial class CustomersRegisterViewModel : ObservableObject
{
    private readonly IOrderRepository orderRepo;

    [ObservableProperty] private ObservableCollection<CustomerRow> customers = new();
    [ObservableProperty] private CustomerRow? selectedCustomer;
    [ObservableProperty] private string detailsText = "Wybierz klienta z listy.";
    [ObservableProperty] private string statusMessage = "Gotowe.";

    public CustomersRegisterViewModel(IOrderRepository orderRepo)
    {
        this.orderRepo = orderRepo;
        Load();
    }

    [RelayCommand]
    private void Load()
    {
        var orders = orderRepo.GetAll()
            .Where(o => !o.IsCanceled && !string.IsNullOrWhiteSpace(o.CustomerPhone))
            .ToList();

        var grouped = orders
            .GroupBy(o => o.CustomerPhone.Trim())
            .Select(g =>
            {
                var paid = g.Where(o => o.IsPaid).ToList();
                var last = g.OrderByDescending(o => o.CreatedAt).First();
                return new CustomerRow
                {
                    Phone = g.Key,
                    Name = last.CustomerName ?? "-",
                    Address = BuildAddress(last),
                    OrderCount = g.Count(),
                    TotalSpent = paid.Sum(o => o.Total),
                    LastOrderDate = g.Max(o => o.CreatedAt)
                };
            })
            .OrderByDescending(c => c.LastOrderDate)
            .ToList();

        Customers.Clear();
        foreach (var c in grouped) Customers.Add(c);

        StatusMessage = $"Załadowano {Customers.Count} klientów.";
    }

    partial void OnSelectedCustomerChanged(CustomerRow? value)
    {
        if (value == null) { DetailsText = "Wybierz klienta z listy."; return; }

        var orders = orderRepo.GetAll()
            .Where(o => o.CustomerPhone?.Trim() == value.Phone && !o.IsCanceled)
            .OrderByDescending(o => o.CreatedAt)
            .ToList();

        var sb = new StringBuilder();
        sb.AppendLine($"KLIENT: {value.Name}");
        sb.AppendLine($"Telefon: {value.Phone}");
        sb.AppendLine($"Adres: {value.Address}");
        sb.AppendLine($"Zamówień: {value.OrderCount}");
        sb.AppendLine($"Łącznie wydane: {value.TotalSpent:F2} zł");
        sb.AppendLine($"Ostatnie zamówienie: {value.LastOrderDate:dd.MM.yyyy HH:mm}");
        sb.AppendLine();
        sb.AppendLine("HISTORIA ZAMÓWIEŃ:");
        sb.AppendLine(new string('-', 40));

        foreach (var o in orders.Take(20))
        {
            var status = o.IsPaid ? "Opłacone" : "Nieopłacone";
            sb.AppendLine($"#{o.Id} | {o.CreatedAt:dd.MM.yyyy HH:mm} | {o.Total:F2} zł | {status}");
        }

        DetailsText = sb.ToString();
    }

    [RelayCommand]
    private void Close()
    {
        Application.Current.Windows
            .OfType<System.Windows.Window>()
            .FirstOrDefault(w => w.DataContext == this)
            ?.Close();
    }

    private static string BuildAddress(Order o)
    {
        var parts = new[] { o.DeliveryStreet, o.DeliveryHouseNumber, o.DeliveryCity }
            .Where(s => !string.IsNullOrWhiteSpace(s));
        return string.Join(" ", parts).Trim();
    }
}