using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic;
using Pizzeria.Pos.Core.Models;
using Pizzeria.Pos.Services;
using Pizzeria.Pos.Wpf.Helpers;
using Pizzeria.Pos.Wpf.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace Pizzeria.Pos.Wpf.ViewModels;

public class OrderSummary
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public string ItemsSummary { get; set; } = string.Empty;
}

public partial class MainViewModel : ObservableObject
{
    private readonly IOrderRepository _orderRepo;
    private readonly IUserRepository _userRepo;
    private readonly IPrintService _printService;
    private readonly IBackupService _backupService;
    private readonly DispatcherTimer _timer;

    [ObservableProperty]
    private User currentUser;

    [ObservableProperty]
    private DateTime currentDateTime = DateTime.Now;

    [ObservableProperty]
    private ObservableCollection<OrderSummary> openOrders = new();

    [ObservableProperty]
    private OrderSummary? selectedOpenOrder;

    [ObservableProperty]
    private AppSection currentSection = AppSection.Sales;

    [ObservableProperty]
    private bool isUsersPanelVisible;

    [ObservableProperty]
    private bool isSettingsPanelVisible;

    public bool IsSalesSection => CurrentSection == AppSection.Sales;
    public bool IsOptionsGeneralSection => CurrentSection == AppSection.OptionsGeneral;
    public bool IsOptionsRegistersSection => CurrentSection == AppSection.OptionsRegisters;
    public bool IsOptionsReportsSection => CurrentSection == AppSection.OptionsReports;
    public bool IsOptionsTilesVisible => !IsUsersPanelVisible && !IsSettingsPanelVisible;

    public string SectionTitle => CurrentSection switch
    {
        AppSection.Sales => "Sprzedaż",
        AppSection.OptionsGeneral => "Opcje - Ogólne",
        AppSection.OptionsRegisters => "Opcje - Rejestry",
        AppSection.OptionsReports => "Opcje - Raporty",
        _ => "POS"
    };

    public UserManagementViewModel UserManagement { get; }
    public SettingsPanelViewModel SettingsPanel { get; }

    public MainViewModel(
        IOrderRepository orderRepo,
        IUserRepository userRepo,
        IPrintService printService,
        IBackupService backupService,
        User user)
    {
        _orderRepo = orderRepo;
        _userRepo = userRepo;
        _printService = printService;
        _backupService = backupService;
        CurrentUser = user;

        UserManagement = new UserManagementViewModel(_userRepo, CurrentUser);
        SettingsPanel = new SettingsPanelViewModel(_backupService);

        LoadOpenOrders();

        _timer = new DispatcherTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += (_, _) => CurrentDateTime = DateTime.Now;
        _timer.Start();
    }

    partial void OnCurrentSectionChanged(AppSection value)
    {
        OnPropertyChanged(nameof(IsSalesSection));
        OnPropertyChanged(nameof(IsOptionsGeneralSection));
        OnPropertyChanged(nameof(IsOptionsRegistersSection));
        OnPropertyChanged(nameof(IsOptionsReportsSection));
        OnPropertyChanged(nameof(SectionTitle));
    }

    partial void OnIsUsersPanelVisibleChanged(bool value)
    {
        OnPropertyChanged(nameof(IsOptionsTilesVisible));
    }

    partial void OnIsSettingsPanelVisibleChanged(bool value)
    {
        OnPropertyChanged(nameof(IsOptionsTilesVisible));
    }


    private Window? GetOwnerWindow()
    {
        return Application.Current.Windows
            .OfType<Window>()
            .FirstOrDefault(w => w.DataContext == this);
    }


    private void LoadOpenOrders()
    {
        var selectedId = SelectedOpenOrder?.Id;

        OpenOrders.Clear();

        var orders = _orderRepo.GetActiveOrders()
            .Where(o => !o.IsCanceled)
            .OrderByDescending(o => o.Id)
            .ToList();

        foreach (var order in orders)
        {
            var itemsSummary = order.Items.Any()
                ? string.Join(", ", order.Items.Take(3).Select(i => $"{i.Quantity}x {i.Name}"))
                : "-";

            OpenOrders.Add(new OrderSummary
            {
                Id = order.Id,
                Type = order.Type,
                Total = order.Total,
                ItemsSummary = itemsSummary
            });
        }

        SelectedOpenOrder = selectedId.HasValue
            ? OpenOrders.FirstOrDefault(x => x.Id == selectedId.Value)
            : null;
    }

    [RelayCommand]
    private void Refresh()
    {
        LoadOpenOrders();
    }

    [RelayCommand]
    private void NewOrderOnSite()
    {
        var orderWindow = new OrderWindow("M", null, CurrentUser)
        {
            Owner = GetOwnerWindow()
        };

        orderWindow.ShowDialog();
        LoadOpenOrders();
    }

    [RelayCommand]
    private void NewOrderTakeaway()
    {
        var orderWindow = new OrderWindow("W", null, CurrentUser)
        {
            Owner = GetOwnerWindow()
        };

        orderWindow.ShowDialog();
        LoadOpenOrders();
    }

    [RelayCommand]
    private void NewOrderDelivery()
    {
        var deliveryWindow = new DeliveryWindow
        {
            Owner = GetOwnerWindow()
        };

        var deliveryResult = deliveryWindow.ShowDialog();
        if (deliveryResult != true)
            return;

        var deliveryData = deliveryWindow.GetDeliveryData();
        if (deliveryData == null)
            return;

        var orderWindow = new OrderWindow("D", deliveryData, CurrentUser)
        {
            Owner = GetOwnerWindow()
        };

        orderWindow.ShowDialog();
        LoadOpenOrders();
    }

    [RelayCommand]
    private void EditSelected()
    {
        if (SelectedOpenOrder == null)
        {
            MessageBox.Show("Najpierw zaznacz zamówienie z listy.");
            return;
        }

        var order = _orderRepo.GetById(SelectedOpenOrder.Id);
        if (order == null)
        {
            MessageBox.Show("Nie znaleziono zamówienia.");
            LoadOpenOrders();
            return;
        }

        if (order.Type == "D")
        {
            var initialDeliveryData = new DeliveryData
            {
                Phone = order.CustomerPhone ?? string.Empty,
                Symbol = order.DeliverySymbol ?? string.Empty,
                CustomerName = order.CustomerName ?? string.Empty,
                City = order.DeliveryCity ?? "Gdańsk",
                Street = order.DeliveryStreet ?? string.Empty,
                HouseNumber = order.DeliveryHouseNumber ?? string.Empty,
                ApartmentNumber = order.DeliveryApartmentNumber ?? string.Empty,
                PostalCode = order.DeliveryPostalCode ?? string.Empty,
                Notes = order.Notes ?? string.Empty,
                DeliveryPrice = order.Items.FirstOrDefault(x => x.Name == "Dostawa")?.Price ?? 0m,
                DeliveryTime = order.CreatedAt,
                SelectedPaymentMethod = string.IsNullOrWhiteSpace(order.PaymentMethod)
                    ? "Gotówka"
                    : order.PaymentMethod
            };

            var deliveryWindow = new DeliveryWindow(initialDeliveryData)
            {
                Owner = GetOwnerWindow()
            };

            var deliveryResult = deliveryWindow.ShowDialog();
            if (deliveryResult != true)
                return;

            var editedDeliveryData = deliveryWindow.GetDeliveryData();
            if (editedDeliveryData == null)
                return;

            var orderWindow = new OrderWindow("D", editedDeliveryData, CurrentUser, order.Id)
            {
                Owner = GetOwnerWindow()
            };

            orderWindow.ShowDialog();
            LoadOpenOrders();
            return;
        }

        if (order.IsCanceled)
        {
            MessageBox.Show("Nie można edytować anulowanego zamówienia.");
            return;
        }

        if (order.IsPaid)
        {
            MessageBox.Show("Nie można edytować opłaconego zamówienia.");
            return;
        }

        var normalOrderWindow = new OrderWindow(order.Type, null, CurrentUser, order.Id)
        {
            Owner = GetOwnerWindow()
        };

        normalOrderWindow.ShowDialog();
        LoadOpenOrders();
    }

    [RelayCommand]
    private void PaySelected()
    {
        if (SelectedOpenOrder == null)
        {
            MessageBox.Show("Najpierw zaznacz zamówienie z listy.");
            return;
        }

        var order = _orderRepo.GetById(SelectedOpenOrder.Id);
        if (order == null)
        {
            MessageBox.Show("Nie znaleziono zamówienia.");
            LoadOpenOrders();
            return;
        }

        if (order.IsCanceled)
        {
            MessageBox.Show("To zamówienie jest anulowane.");
            return;
        }

        if (order.IsPaid)
        {
            MessageBox.Show("To zamówienie jest już opłacone.");
            return;
        }

        var paymentItems = order.Items
            .Select(x => new PaymentCartItem
            {
                Name = x.Name,
                Quantity = x.Quantity,
                UnitPrice = x.Price
            })
            .ToList();

        var paymentWindow = new PaymentWindow(paymentItems, order.Total, order.PaymentMethod)
        {
            Owner = GetOwnerWindow()
        };

        var result = paymentWindow.ShowDialog();
        if (result != true)
            return;

        var paymentResult = paymentWindow.GetResult();
        if (paymentResult == null)
        {
            MessageBox.Show("Nie udało się odczytać wyniku płatności.");
            return;
        }

        order.IsPaid = paymentResult.IsPaid;
        order.PaymentMethod = paymentResult.PaymentMethod;
        order.Total = paymentResult.FinalTotal;


        _orderRepo.UpdateOrder(order);
        LoadOpenOrders();

        MessageBox.Show($"Zamówienie #{order.Id} opłacone.");

        if (order.IsPaid)
        {
            _printService.PrintOrderReceipt(order, CurrentUser);
        }

    }

    [RelayCommand]
    private void CancelSelected()
    {
        if (SelectedOpenOrder == null)
        {
            MessageBox.Show("Najpierw zaznacz zamówienie z listy.");
            return;
        }

        if (CurrentUser.Role != "Manager")
        {
            MessageBox.Show("Storno może wykonać tylko manager.");
            return;
        }

        var confirm = MessageBox.Show(
            $"Czy na pewno chcesz zrobić storno zamówienia #{SelectedOpenOrder.Id}?",
            "Potwierdzenie storna",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (confirm != MessageBoxResult.Yes)
            return;

        var order = _orderRepo.GetById(SelectedOpenOrder.Id);
        if (order == null)
        {
            MessageBox.Show("Nie znaleziono zamówienia.");
            LoadOpenOrders();
            return;
        }

        order.IsCanceled = true;
        _orderRepo.UpdateOrder(order);

        MessageBox.Show($"Zamówienie #{order.Id} zostało oznaczone jako storno.");
        LoadOpenOrders();
    }

    [RelayCommand]
    private void ChangeTypeSelected()
    {
        if (SelectedOpenOrder == null)
        {
            MessageBox.Show("Najpierw zaznacz zamówienie z listy.");
            return;
        }

        var sourceOrder = _orderRepo.GetById(SelectedOpenOrder.Id);
        if (sourceOrder == null)
        {
            MessageBox.Show("Nie znaleziono zamówienia.");
            LoadOpenOrders();
            return;
        }

        if (sourceOrder.IsCanceled)
        {
            MessageBox.Show("Nie można zmienić typu anulowanego zamówienia.");
            return;
        }

        if (sourceOrder.IsPaid)
        {
            MessageBox.Show("Nie można zmienić typu opłaconego zamówienia.");
            return;
        }

        var currentType = string.IsNullOrWhiteSpace(sourceOrder.Type)
    ? "M"
    : sourceOrder.Type.Trim().ToUpperInvariant();

        var changeTypeWindow = new ChangeOrderTypeWindow(currentType)
        {
            Owner = GetOwnerWindow()
        };

        var dialogResult = changeTypeWindow.ShowDialog();
        if (dialogResult != true)
            return;

        var newType = changeTypeWindow.SelectedType;

        if (newType == currentType)
        {
            MessageBox.Show("Wybrano ten sam typ zamówienia.");
            return;
        }

        var originalOrder = CloneOrderForUpdate(sourceOrder);
        var orderToUpdate = CloneOrderForUpdate(sourceOrder);

        if (originalOrder.Items == null || originalOrder.Items.Count == 0)
        {
            MessageBox.Show("To zamówienie nie ma pozycji w koszyku, więc zmiana typu została przerwana.");
            return;
        }

        if (newType == "D")
        {
            var deliveryData = OpenDeliveryEditor(orderToUpdate);
            if (deliveryData == null)
                return;

            ApplyDeliveryDataToOrder(orderToUpdate, deliveryData);
            EnsureDeliveryChargeItem(orderToUpdate, deliveryData.DeliveryPrice);
            orderToUpdate.PaymentMethod = string.IsNullOrWhiteSpace(deliveryData.SelectedPaymentMethod)
                ? orderToUpdate.PaymentMethod
                : deliveryData.SelectedPaymentMethod;
        }
        else
        {
            RemoveDeliveryChargeItem(orderToUpdate);
            ClearDeliveryFields(orderToUpdate);
        }

        orderToUpdate.Type = newType;
        orderToUpdate.Total = orderToUpdate.Items.Sum(x => x.Price * x.Quantity);

        _orderRepo.UpdateOrder(orderToUpdate);

        var reloadedOrder = _orderRepo.GetById(orderToUpdate.Id);
        if (reloadedOrder == null)
        {
            MessageBox.Show("Nie udało się odczytać zamówienia po zmianie typu.");
            LoadOpenOrders();
            return;
        }

        if (originalOrder.Items.Any() && !reloadedOrder.Items.Any())
        {
            _orderRepo.UpdateOrder(originalOrder);
            MessageBox.Show("Wykryto błąd po zapisie i przywrócono poprzedni stan zamówienia.");
            LoadOpenOrders();
            return;
        }

        MessageBox.Show($"Typ zamówienia #{orderToUpdate.Id} zmieniono z {currentType} na {newType}.");
        LoadOpenOrders();
    }

    [RelayCommand]
    private void Settings()
    {
        ShowOptionsGeneral();
    }

    [RelayCommand]
    private void Reports()
    {
        ShowOptionsReports();
    }

    [RelayCommand]
    private void ShowSales()
    {
        CurrentSection = AppSection.Sales;
        IsUsersPanelVisible = false;
        IsSettingsPanelVisible = false;
        LoadOpenOrders();
    }

    [RelayCommand]
    private void ShowOptionsGeneral()
    {
        CurrentSection = AppSection.OptionsGeneral;
        IsUsersPanelVisible = false;
        IsSettingsPanelVisible = false;
    }

    [RelayCommand]
    private void ShowOptionsRegisters()
    {
        CurrentSection = AppSection.OptionsRegisters;
        IsUsersPanelVisible = false;
        IsSettingsPanelVisible = false;
    }


    [RelayCommand]
    private void ShowOptionsReports()
    {
        CurrentSection = AppSection.OptionsReports;
        IsUsersPanelVisible = false;
        IsSettingsPanelVisible = false;
    }

    [RelayCommand]
    private void OpenUsersPanel()
    {
        if (CurrentUser.Role != "Manager")
        {
            MessageBox.Show("Tylko manager ma dostęp do zarządzania użytkownikami.");
            return;
        }
        CurrentSection = AppSection.OptionsGeneral;
        IsUsersPanelVisible = true;
        IsSettingsPanelVisible = false;
        UserManagement.LoadUsersCommand.Execute(null);
    }

    [RelayCommand]
    private void CloseUsersPanel()
    {
        IsUsersPanelVisible = false;
    }

    [RelayCommand]
    private void OpenRegisters()
    {
        var registersWindow = new RegistersWindow(_orderRepo, _printService, CurrentUser)
        {
            Owner = GetOwnerWindow()
        };

        registersWindow.ShowDialog();
    }

    [RelayCommand]
    private void OpenCustomersRegister()
    {
        var window = new CustomersRegisterWindow(_orderRepo) { Owner = GetOwnerWindow() };
        window.ShowDialog();
    }

    [RelayCommand]
    private void OpenDeliveriesRegister()
    {
        var window = new DeliveriesRegisterWindow(_orderRepo) { Owner = GetOwnerWindow() };
        window.ShowDialog();
    }

    [RelayCommand]
    private void OpenReceiptsRegister()
    {
        var window = new ReceiptsRegisterWindow(_orderRepo) { Owner = GetOwnerWindow() };
        window.ShowDialog();
    }

    [RelayCommand]
    private void OpenReports()
    {
        var reportsWindow = new ReportsWindow(_orderRepo, CurrentUser)
        {
            Owner = GetOwnerWindow()
        };

        reportsWindow.ShowDialog();
    }

    [RelayCommand]
    private void OpenMonthlyReport()
    {
        var reportsWindow = new ReportsWindow(_orderRepo, CurrentUser, initialTab: 1)
        {
            Owner = GetOwnerWindow()
        };

        reportsWindow.ShowDialog();
    }

    [RelayCommand]
    private void Logout()
    {
        var confirm = MessageBox.Show(
            "Czy na pewno chcesz się wylogować?",
            "Wylogowanie",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (confirm != MessageBoxResult.Yes) return;

        // Otwórz nowe okno logowania
        var login = new Pizzeria.Pos.Wpf.Views.LoginWindow();
        login.Show();

        // Zamknij MainWindow
        Application.Current.Windows
            .OfType<System.Windows.Window>()
            .FirstOrDefault(w => w is Pizzeria.Pos.Wpf.MainWindow)
            ?.Close();
    }

    [RelayCommand]
    private void PrintPosReport()
    {
        var orders = _orderRepo.GetAll()
            .Where(o => o.CreatedAt.Date == DateTime.Today && !o.IsCanceled && o.IsPaid)
            .ToList();

        var sb = new System.Text.StringBuilder();
        sb.AppendLine("PIZZERIA POS");
        sb.AppendLine("====================================");
        sb.AppendLine($"RAPORT DNIA {DateTime.Today:yyyy-MM-dd}");
        sb.AppendLine($"Generowano: {DateTime.Now:HH:mm:ss}");
        sb.AppendLine($"Operator: {CurrentUser.Name}");
        sb.AppendLine("====================================");
        sb.AppendLine();
        sb.AppendLine($"Liczba zamówień: {orders.Count}");
        sb.AppendLine($"Obrót brutto:    {orders.Sum(o => o.Total):F2} zł");
        sb.AppendLine();
        sb.AppendLine("Płatności:");
        sb.AppendLine($"  Gotówka:  {orders.Where(o => o.PaymentMethod == "Gotówka").Sum(o => o.Total):F2} zł");
        sb.AppendLine($"  Karta:    {orders.Where(o => o.PaymentMethod == "Karta").Sum(o => o.Total):F2} zł");
        sb.AppendLine($"  Przelew:  {orders.Where(o => o.PaymentMethod == "Przelew").Sum(o => o.Total):F2} zł");
        sb.AppendLine($"  Online:   {orders.Where(o => o.PaymentMethod == "Online").Sum(o => o.Total):F2} zł");
        sb.AppendLine($"  Voucher:  {orders.Where(o => o.PaymentMethod == "Voucher").Sum(o => o.Total):F2} zł");
        sb.AppendLine("====================================");

        _printService.PrintTextDocument(
            title: $"Raport_POS_{DateTime.Today:yyyy-MM-dd}_{DateTime.Now:HHmmss}",
            content: sb.ToString(),
            orderId: null,
            printedBy: CurrentUser,
            documentType: "Raport POS");

        MessageBox.Show($"Raport POS za {DateTime.Today:yyyy-MM-dd} został zapisany.", "Raport POS");
    }


    private DeliveryData? OpenDeliveryEditor(Order order)
    {
        var deliveryWindow = new DeliveryWindow
        {
            Owner = GetOwnerWindow()
        };

        if (deliveryWindow.DataContext is DeliveryViewModel vm)
        {
            vm.Phone = order.CustomerPhone ?? string.Empty;
            vm.Symbol = order.DeliverySymbol ?? string.Empty;
            vm.CustomerName = order.CustomerName ?? string.Empty;

            vm.City = FirstNonEmpty(order.City, order.DeliveryCity);
            vm.Street = FirstNonEmpty(order.Street, order.DeliveryStreet);
            vm.HouseNumber = FirstNonEmpty(order.HouseNumber, order.DeliveryHouseNumber);
            vm.ApartmentNumber = FirstNonEmpty(order.ApartmentNumber, order.DeliveryApartmentNumber);
            vm.PostalCode = FirstNonEmpty(order.PostalCode, order.DeliveryPostalCode);

            vm.Notes = order.Notes ?? string.Empty;
            vm.DeliveryPrice = GetDeliveryPriceFromOrder(order);
            vm.SelectedPaymentMethod = string.IsNullOrWhiteSpace(order.PaymentMethod)
                ? "Gotówka"
                : order.PaymentMethod;
        }

        var result = deliveryWindow.ShowDialog();
        if (result != true)
            return null;

        return deliveryWindow.GetDeliveryData();
    }

    [RelayCommand]
    private void OpenPizzaAddons()
    {
        if (CurrentUser.Role != "Manager")
        {
            MessageBox.Show("Tylko manager ma dostęp do kartoteki dodatków do pizzy.");
            return;
        }

        var window = new PizzaAddonsWindow
        {
            Owner = GetOwnerWindow()
        };

        window.ShowDialog();
    }

    [RelayCommand]
    private void OpenSettingsPanel()
    {
        CurrentSection = AppSection.OptionsGeneral;
        IsUsersPanelVisible = false;
        IsSettingsPanelVisible = true;
    }

    [RelayCommand]
    private void CloseSettingsPanel()
    {
        IsSettingsPanelVisible = false;
    }


    private void ApplyDeliveryDataToOrder(Order order, DeliveryData data)
    => DeliveryOrderHelper.ApplyDeliveryDataToOrder(order, data);

    private void EnsureDeliveryChargeItem(Order order, decimal deliveryPrice)
        => DeliveryOrderHelper.UpsertDeliveryCharge(order.Items, deliveryPrice);

    private void RemoveDeliveryChargeItem(Order order)
        => DeliveryOrderHelper.RemoveDeliveryCharge(order.Items);

    private void ClearDeliveryFields(Order order)
        => DeliveryOrderHelper.ClearDeliveryFields(order);

    private decimal GetDeliveryPriceFromOrder(Order order)
        => DeliveryOrderHelper.GetDeliveryPrice(order.Items, 8m);

    private static string FirstNonEmpty(params string?[] values)
    {
        return values.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x)) ?? string.Empty;
    }

    private static Order CloneOrderForUpdate(Order source)
    {
        return new Order
        {
            Id = source.Id,
            Type = source.Type,
            Total = source.Total,
            IsPaid = source.IsPaid,
            IsCanceled = source.IsCanceled,
            CustomerPhone = source.CustomerPhone ?? string.Empty,
            CustomerName = source.CustomerName ?? string.Empty,
            Address = source.Address ?? string.Empty,
            Notes = source.Notes ?? string.Empty,
            PaymentMethod = source.PaymentMethod ?? string.Empty,
            City = source.City ?? string.Empty,
            Street = source.Street ?? string.Empty,
            HouseNumber = source.HouseNumber ?? string.Empty,
            ApartmentNumber = source.ApartmentNumber ?? string.Empty,
            PostalCode = source.PostalCode ?? string.Empty,
            DeliverySymbol = source.DeliverySymbol ?? string.Empty,
            DeliveryCity = source.DeliveryCity ?? string.Empty,
            DeliveryStreet = source.DeliveryStreet ?? string.Empty,
            DeliveryHouseNumber = source.DeliveryHouseNumber ?? string.Empty,
            DeliveryApartmentNumber = source.DeliveryApartmentNumber ?? string.Empty,
            DeliveryPostalCode = source.DeliveryPostalCode ?? string.Empty,
            CreatedAt = source.CreatedAt,
            UserId = source.UserId,
            Items = (source.Items ?? new List<OrderItem>())
                .Select(item => new OrderItem
                {
                    Id = item.Id,
                    Name = item.Name,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    ProductId = item.ProductId,
                    BaseProductName = item.BaseProductName,
                    BaseProductPrice = item.BaseProductPrice,
                    ConfigurationJson = item.ConfigurationJson,
                    OrderId = item.OrderId
                })
                .ToList()
        };
    }

    [RelayCommand]
    private void OpenProducts()
    {
        var productRepo = App.ServiceProvider!.GetRequiredService<IProductRepository>();
        var productsWindow = new ProductsWindow();
        productsWindow.Owner = GetOwnerWindow();
        productsWindow.ShowDialog();
    }


    [RelayCommand]
    private void OpenPrinters()
    {
        MessageBox.Show("Konfiguracja drukarek w toku...", "Drukarki", MessageBoxButton.OK, MessageBoxImage.Information);
    }


}