using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pizzeria.Pos.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Pizzeria.Pos.Wpf.ViewModels
{
    public partial class DeliveryViewModel : ObservableObject
    {
        private readonly IOrderRepository orderRepository;

        public ObservableCollection<DeliveryAddressViewModel> SavedAddresses { get; } = new();

        [ObservableProperty]
        private string phone = string.Empty;

        [ObservableProperty]
        private string symbol = string.Empty;

        [ObservableProperty]
        private string customerName = string.Empty;

        [ObservableProperty]
        private string city = "Gdańsk";

        [ObservableProperty]
        private string street = string.Empty;

        [ObservableProperty]
        private string houseNumber = string.Empty;

        [ObservableProperty]
        private string apartmentNumber = string.Empty;

        [ObservableProperty]
        private string postalCode = string.Empty;

        [ObservableProperty]
        private string notes = string.Empty;

        [ObservableProperty]
        private decimal deliveryPrice = 8m;

        [ObservableProperty]
        private DateTime deliveryTime = DateTime.Now.AddMinutes(45);

        [ObservableProperty]
        private string selectedPaymentMethod = "Gotówka";

        [ObservableProperty]
        private bool deliveryToAddress = true;

        [ObservableProperty]
        private bool pickupInStore = false;

        [ObservableProperty]
        private bool assignCard = false;

        [ObservableProperty]
        private DeliveryAddressViewModel? selectedSavedAddress;

        public DeliveryData? Result { get; private set; }

        public DeliveryViewModel(IOrderRepository orderRepository, DeliveryData? initialData = null)
        {
            this.orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));

            if (initialData != null)
                LoadFromInitialData(initialData);
        }

        private void LoadFromInitialData(DeliveryData data)
        {
            Phone = data.Phone ?? string.Empty;
            Symbol = data.Symbol ?? string.Empty;
            CustomerName = data.CustomerName ?? string.Empty;
            City = string.IsNullOrWhiteSpace(data.City) ? "Gdańsk" : data.City;
            Street = data.Street ?? string.Empty;
            HouseNumber = data.HouseNumber ?? string.Empty;
            ApartmentNumber = data.ApartmentNumber ?? string.Empty;
            PostalCode = data.PostalCode ?? string.Empty;
            Notes = data.Notes ?? string.Empty;
            DeliveryPrice = data.DeliveryPrice;
            DeliveryTime = data.DeliveryTime == default ? DateTime.Now.AddMinutes(45) : data.DeliveryTime;
            SelectedPaymentMethod = string.IsNullOrWhiteSpace(data.SelectedPaymentMethod)
                ? "Gotówka"
                : data.SelectedPaymentMethod;
        }

        [RelayCommand]
        private void SearchPhone()
        {
            SavedAddresses.Clear();
            SelectedSavedAddress = null;

            var normalizedPhone = NormalizePhone(Phone);

            if (normalizedPhone.Length < 9)
            {
                MessageBox.Show("Podaj poprawny numer telefonu.");
                return;
            }

            Phone = normalizedPhone;

            var addresses = orderRepository.GetDeliveryAddressesByPhone(normalizedPhone);

            foreach (var address in addresses)
            {
                SavedAddresses.Add(new DeliveryAddressViewModel
                {
                    Symbol = address.Symbol,
                    CustomerName = address.CustomerName,
                    City = string.IsNullOrWhiteSpace(address.City) ? "Gdańsk" : address.City,
                    Street = address.Street,
                    HouseNumber = address.HouseNumber,
                    ApartmentNumber = address.ApartmentNumber,
                    PostalCode = address.PostalCode,
                    Notes = address.Notes
                });
            }

            if (SavedAddresses.Count == 0)
            {
                MessageBox.Show("Nie znaleziono zapisanych adresów dla tego numeru.");
                return;
            }

            SelectedSavedAddress = SavedAddresses[0];
            ApplyAddress(SelectedSavedAddress);
        }

        [RelayCommand]
        private void SelectSavedAddress(DeliveryAddressViewModel? address)
        {
            if (address == null)
                return;

            SelectedSavedAddress = address;
            ApplyAddress(address);
        }

        private void ApplyAddress(DeliveryAddressViewModel address)
        {
            Symbol = address.Symbol ?? string.Empty;
            CustomerName = address.CustomerName ?? string.Empty;
            City = string.IsNullOrWhiteSpace(address.City) ? "Gdańsk" : address.City;
            Street = address.Street ?? string.Empty;
            HouseNumber = address.HouseNumber ?? string.Empty;
            ApartmentNumber = address.ApartmentNumber ?? string.Empty;
            PostalCode = address.PostalCode ?? string.Empty;
            Notes = address.Notes ?? string.Empty;
        }

        [RelayCommand]
        private void SelectCash() => SelectedPaymentMethod = "Gotówka";

        [RelayCommand]
        private void SelectCard() => SelectedPaymentMethod = "Karta";

        [RelayCommand]
        private void SelectTransfer() => SelectedPaymentMethod = "Przelew";

        [RelayCommand]
        private void SelectOnline() => SelectedPaymentMethod = "Online";

        [RelayCommand]
        private void SelectVoucher() => SelectedPaymentMethod = "Voucher";

        [RelayCommand]
        private void Confirm()
        {
            var normalizedPhone = NormalizePhone(Phone);

            if (normalizedPhone.Length < 9)
            {
                MessageBox.Show("Numer telefonu jest wymagany.");
                return;
            }

            if (string.IsNullOrWhiteSpace(City) ||
                string.IsNullOrWhiteSpace(Street) ||
                string.IsNullOrWhiteSpace(HouseNumber))
            {
                MessageBox.Show("Uzupełnij adres dostawy.");
                return;
            }

            Phone = normalizedPhone;

            Result = new DeliveryData
            {
                Phone = normalizedPhone,
                Symbol = Symbol,
                CustomerName = CustomerName,
                City = City,
                Street = Street,
                HouseNumber = HouseNumber,
                ApartmentNumber = ApartmentNumber,
                PostalCode = PostalCode,
                Notes = Notes,
                DeliveryPrice = DeliveryPrice,
                DeliveryTime = DeliveryTime,
                SelectedPaymentMethod = SelectedPaymentMethod
            };

            CloseWindow(true);
        }

        [RelayCommand]
        private void Cancel()
        {
            Result = null;
            CloseWindow(false);
        }

        private void CloseWindow(bool? dialogResult)
        {
            var window = Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.DataContext == this);

            if (window != null)
            {
                window.DialogResult = dialogResult;
                window.Close();
            }
        }

        private static string NormalizePhone(string? phone)
        {
            return new string((phone ?? string.Empty).Where(char.IsDigit).ToArray());
        }
    }
}