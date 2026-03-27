using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pizzeria.Pos.Core.Models;
using Pizzeria.Pos.Services;
using Pizzeria.Pos.Wpf.Helpers;
using Pizzeria.Pos.Wpf.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Text.Json;

namespace Pizzeria.Pos.Wpf.ViewModels
{
    public partial class CartItemViewModel : ObservableObject
    {
        public int ProductId { get; set; }

        [ObservableProperty]
        private string name = "";

        [ObservableProperty]
        private decimal unitPrice;

        [ObservableProperty]
        private int quantity;

        public bool IsPizzaConfigurable { get; set; }
        public string BaseProductName { get; set; } = string.Empty;
        public decimal BaseProductPrice { get; set; }
        public PizzaConfigResult? PizzaConfig { get; set; }

        public decimal LineTotal => UnitPrice * Quantity;

        partial void OnQuantityChanged(int value)
        {
            OnPropertyChanged(nameof(LineTotal));
        }

        partial void OnUnitPriceChanged(decimal value)
        {
            OnPropertyChanged(nameof(LineTotal));
        }

        public void ApplyPizzaConfig(PizzaConfigResult config)
        {
            PizzaConfig = config.Clone();
            Name = config.BuildDisplayName();
            UnitPrice = config.FinalPrice;
            BaseProductName = config.ProductName;
        }
    }

    public partial class OrderViewModel : ObservableObject
    {
        private readonly IProductRepository productRepository;
        private readonly IOrderRepository orderRepository;
        private readonly IPrintService printService;

        private int? editingOrderId;
        private User? currentUser;
        private Order? loadedOrder;

        [ObservableProperty]
        private string orderType = "M";

        [ObservableProperty]
        private string titleText = "Nowe zamówienie";

        [ObservableProperty]
        private string selectedCategory = "Pizza";

        [ObservableProperty]
        private Product? selectedProduct;

        [ObservableProperty]
        private CartItemViewModel? selectedCartItem;

        [ObservableProperty]
        private DeliveryData? deliveryData;

        [ObservableProperty]
        private string orderContextText = string.Empty;

        private const int DeliveryChargeProductId = -1;
        private const string DeliveryChargeName = "Dostawa";

        public ObservableCollection<string> Categories { get; } = new();
        public ObservableCollection<Product> Products { get; } = new();
        public ObservableCollection<CartItemViewModel> CartItems { get; } = new();

        public decimal CartTotal => CartItems.Sum(x => x.LineTotal);

        public bool CanEditSelectedPizza =>
        SelectedCartItem?.IsPizzaConfigurable == true &&
        SelectedCartItem.PizzaConfig != null;

        private sealed class OrderItemDelta
        {
            public string Name { get; set; } = string.Empty;
            public decimal UnitPrice { get; set; }
            public int QuantityDelta { get; set; }
        }


        public OrderViewModel(
            IProductRepository productRepository,
            IOrderRepository orderRepository,
            IPrintService printService)
        {
            this.productRepository = productRepository;
            this.orderRepository = orderRepository;
            this.printService = printService;
        }

        public void Initialize(string orderType, DeliveryData? deliveryData = null, User? currentUser = null, int? orderId = null)
        {
            OrderType = string.IsNullOrWhiteSpace(orderType) ? "M" : orderType;
            this.currentUser = currentUser;
            editingOrderId = orderId;
            loadedOrder = null;

            DeliveryData = null;
            OrderContextText = string.Empty;

            Categories.Clear();
            Products.Clear();
            CartItems.Clear();

            LoadCategories();
            SelectCategory("Pizza");

            if (orderId.HasValue)
            {
                LoadExistingOrder(orderId.Value);

                if (OrderType == "D" && deliveryData != null)
                {
                    ApplyEditedDeliveryData(deliveryData);
                }
                else if (OrderType == "D" && DeliveryData != null)
                {
                    SyncDeliveryChargeItem();
                }
            }
            else
            {
                DeliveryData = deliveryData;
                SetupNewOrderTitleAndContext();
            }

            RefreshCartTotals();
        }

        private sealed class CartItemSnapshot
        {
            public int ProductId { get; set; }
            public string Name { get; set; } = string.Empty;
            public decimal UnitPrice { get; set; }
            public int Quantity { get; set; }
            public string BaseProductName { get; set; } = string.Empty;
            public decimal BaseProductPrice { get; set; }
            public string? ConfigurationJson { get; set; }
        }

        private sealed class DeltaKey : IEquatable<DeltaKey>
        {
            public int ProductId { get; init; }
            public string Name { get; init; } = string.Empty;
            public decimal UnitPrice { get; init; }
            public string BaseProductName { get; init; } = string.Empty;
            public decimal BaseProductPrice { get; init; }
            public string? ConfigurationJson { get; init; }

            public bool Equals(DeltaKey? other)
            {
                if (other is null) return false;

                return ProductId == other.ProductId
                    && string.Equals(Name, other.Name, StringComparison.Ordinal)
                    && UnitPrice == other.UnitPrice
                    && string.Equals(BaseProductName, other.BaseProductName, StringComparison.Ordinal)
                    && BaseProductPrice == other.BaseProductPrice
                    && string.Equals(ConfigurationJson ?? string.Empty, other.ConfigurationJson ?? string.Empty, StringComparison.Ordinal);
            }

            public override bool Equals(object? obj) => Equals(obj as DeltaKey);

            public override int GetHashCode()
            {
                return HashCode.Combine(
                    ProductId,
                    Name,
                    UnitPrice,
                    BaseProductName,
                    BaseProductPrice,
                    ConfigurationJson ?? string.Empty);
            }
        }

        private static bool IsKitchenIgnoredSnapshot(CartItemSnapshot item)
        {
            if (item.ProductId == -1)
                return true;

            return string.Equals(item.Name, "Dostawa", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsKitchenIgnoredCartItem(CartItemViewModel item)
        {
            if (item.ProductId == -1)
                return true;

            return string.Equals(item.Name, "Dostawa", StringComparison.OrdinalIgnoreCase);
        }

        private static DeltaKey BuildDeltaKey(CartItemSnapshot item)
        {
            return new DeltaKey
            {
                ProductId = item.ProductId,
                Name = item.Name,
                UnitPrice = item.UnitPrice,
                BaseProductName = item.BaseProductName,
                BaseProductPrice = item.BaseProductPrice,
                ConfigurationJson = item.ConfigurationJson
            };
        }

        private static DeltaKey BuildDeltaKey(CartItemViewModel item)
        {
            return new DeltaKey
            {
                ProductId = item.ProductId,
                Name = item.Name,
                UnitPrice = item.UnitPrice,
                BaseProductName = item.BaseProductName ?? string.Empty,
                BaseProductPrice = item.BaseProductPrice,
                ConfigurationJson = SerializePizzaConfig(item)
            };
        }

        private void CaptureOriginalItemsSnapshot()
        {
            originalItemsSnapshot = CartItems
                .Select(x => new CartItemSnapshot
                {
                    ProductId = x.ProductId,
                    Name = x.Name,
                    UnitPrice = x.UnitPrice,
                    Quantity = x.Quantity,
                    BaseProductName = x.BaseProductName ?? string.Empty,
                    BaseProductPrice = x.BaseProductPrice,
                    ConfigurationJson = SerializePizzaConfig(x)
                })
                .ToList();
        }


        private List<CartItemSnapshot> originalItemsSnapshot = new();

        private void SetupNewOrderTitleAndContext()
        {
            TitleText = OrderType switch
            {
                "M" => "Nowe zamówienie - Na miejscu",
                "W" => "Nowe zamówienie - Wynos",
                "D" => "Nowe zamówienie - Dostawa",
                _ => "Nowe zamówienie"
            };

            if (OrderType == "D")
                SyncDeliveryChargeItem();
            else
                OrderContextText = "";
        }

        private void LoadExistingOrder(int orderId)
        {
            var order = orderRepository.GetById(orderId);

            if (order == null || order.Items == null || order.Items.Count == 0)
            {
                var fallback = orderRepository
                    .GetActiveOrders()
                    .FirstOrDefault(o => o.Id == orderId);

                if (fallback != null)
                    order = fallback;
            }

            if (order == null)
            {
                MessageBox.Show("Nie znaleziono zamówienia do edycji.");
                SetupNewOrderTitleAndContext();
                return;
            }

            loadedOrder = order;
            OrderType = order.Type;
            TitleText = $"Edycja zamówienia #{order.Id}";

            CartItems.Clear();

            foreach (var item in order.Items)
            {
                var restoredConfig = DeserializePizzaConfig(item.ConfigurationJson);

                var cartItem = new CartItemViewModel
                {
                    ProductId = item.ProductId,
                    Name = item.Name,
                    UnitPrice = item.Price,
                    Quantity = item.Quantity,
                    BaseProductName = string.IsNullOrWhiteSpace(item.BaseProductName) ? item.Name : item.BaseProductName,
                    BaseProductPrice = item.BaseProductPrice > 0 ? item.BaseProductPrice : item.Price,
                    IsPizzaConfigurable = restoredConfig != null,
                    PizzaConfig = restoredConfig
                };

                if (restoredConfig != null)
                {
                    cartItem.Name = restoredConfig.BuildDisplayName();
                    cartItem.UnitPrice = restoredConfig.FinalPrice;
                }

                CartItems.Add(cartItem);
            }

            if (order.Type == "D")
            {
                DeliveryData = BuildDeliveryDataFromOrder(order);
                OrderContextText = DeliveryData.ShortSummary;
                SyncDeliveryChargeItem();
            }
            else
            {
                DeliveryData = null;
                OrderContextText = string.Empty;
            }

            RefreshCartTotals();
            CaptureOriginalItemsSnapshot();
        }

        private DeliveryData BuildDeliveryDataFromOrder(Order order)
        {
            return new DeliveryData
            {
                Phone = order.CustomerPhone ?? string.Empty,
                Symbol = order.DeliverySymbol ?? string.Empty,
                CustomerName = order.CustomerName ?? string.Empty,
                City = FirstNonEmpty(order.DeliveryCity, order.City, "Gdańsk"),
                Street = FirstNonEmpty(order.DeliveryStreet, order.Street),
                HouseNumber = FirstNonEmpty(order.DeliveryHouseNumber, order.HouseNumber),
                ApartmentNumber = FirstNonEmpty(order.DeliveryApartmentNumber, order.ApartmentNumber),
                PostalCode = FirstNonEmpty(order.DeliveryPostalCode, order.PostalCode),
                Notes = order.Notes ?? string.Empty,
                DeliveryPrice = CartItems
                    .FirstOrDefault(x => x.ProductId == -1 &&
                                         string.Equals(x.Name, "Dostawa", StringComparison.OrdinalIgnoreCase))
                    ?.UnitPrice ?? 0m,
                DeliveryTime = order.CreatedAt == default ? DateTime.Now.AddMinutes(45) : order.CreatedAt,
                SelectedPaymentMethod = string.IsNullOrWhiteSpace(order.PaymentMethod)
                    ? "Gotówka"
                    : order.PaymentMethod
            };
        }

        private static string FirstNonEmpty(params string?[] values)
        {
            return values.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x)) ?? string.Empty;
        }


        private void SyncDeliveryChargeItem()
        {
            DeliveryOrderHelper.UpsertDeliveryCharge(CartItems, DeliveryData?.DeliveryPrice ?? 0m);
            RefreshCartTotals();
        }

        private void LoadCategories()
        {
            Categories.Clear();

            var categories = productRepository.GetAll()
                .Select(p => p.Category)
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Distinct()
                .OrderBy(c => c)
                .ToList();

            foreach (var category in categories)
                Categories.Add(category);

            if (Categories.Count == 0)
                Categories.Add("Pizza");
        }

        [RelayCommand]
        private void SelectCategory(string? category)
        {
            if (string.IsNullOrWhiteSpace(category))
                return;

            SelectedCategory = category;
            LoadProducts();
        }

        private void LoadProducts()
        {
            Products.Clear();

            var items = productRepository.GetByCategory(SelectedCategory);
            foreach (var item in items)
                Products.Add(item);
        }

        [RelayCommand]
        private void AddSelectedProduct()
        {
            if (SelectedProduct == null)
            {
                MessageBox.Show("Najpierw wybierz produkt.");
                return;
            }

            AddProductToCart(SelectedProduct);
        }

        [RelayCommand]
        private void AddProduct(Product? product)
        {
            if (product == null)
                return;

            AddProductToCart(product);
        }

        private void AddProductToCart(Product product)
        {
            if (product.Category == "Pizza")
            {
                var configWindow = new PizzaConfigWindow(product.Name, product.Price);
                var result = configWindow.ShowDialog();

                if (result == true)
                {
                    var config = configWindow.GetResult();
                    if (config == null)
                        return;

                    var existingConfigured = CartItems.FirstOrDefault(x =>
                        x.IsPizzaConfigurable &&
                        AreSamePizzaConfig(x.PizzaConfig, config));

                    if (existingConfigured == null)
                    {
                        var cartItem = new CartItemViewModel
                        {
                            ProductId = product.Id,
                            Quantity = 1,
                            IsPizzaConfigurable = true,
                            BaseProductName = product.Name,
                            BaseProductPrice = product.Price
                        };

                        cartItem.ApplyPizzaConfig(config);
                        CartItems.Add(cartItem);
                    }
                    else
                    {
                        existingConfigured.Quantity++;
                    }

                    RefreshCartTotals();
                }

                return;
            }

            var existing = CartItems.FirstOrDefault(x => x.ProductId == product.Id && x.Name == product.Name);

            if (existing == null)
            {
                CartItems.Add(new CartItemViewModel
                {
                    ProductId = product.Id,
                    Name = product.Name,
                    UnitPrice = product.Price,
                    Quantity = 1,
                    IsPizzaConfigurable = false
                });
            }
            else
            {
                existing.Quantity++;
            }

            RefreshCartTotals();
        }

        [RelayCommand]
        private void EditSelectedPizza()
        {
            if (SelectedCartItem == null)
                return;

            if (!SelectedCartItem.IsPizzaConfigurable || SelectedCartItem.PizzaConfig == null)
            {
                MessageBox.Show("Ta pozycja nie ma zapisanej konfiguracji pizzy do edycji.");
                return;
            }

            var configWindow = new PizzaConfigWindow(
                SelectedCartItem.BaseProductName,
                SelectedCartItem.BaseProductPrice,
                SelectedCartItem.PizzaConfig.Clone());

            var result = configWindow.ShowDialog();
            if (result != true)
                return;

            var updatedConfig = configWindow.GetResult();
            if (updatedConfig == null)
                return;

            var duplicate = CartItems.FirstOrDefault(x =>
                x != SelectedCartItem &&
                x.IsPizzaConfigurable &&
                AreSamePizzaConfig(x.PizzaConfig, updatedConfig));

            if (duplicate != null)
            {
                duplicate.Quantity += SelectedCartItem.Quantity;
                CartItems.Remove(SelectedCartItem);
                SelectedCartItem = duplicate;
            }
            else
            {
                SelectedCartItem.ApplyPizzaConfig(updatedConfig);
            }




            RefreshCartTotals();
            OnPropertyChanged(nameof(CanEditSelectedPizza));

        }

        private static bool AreSamePizzaConfig(PizzaConfigResult? a, PizzaConfigResult? b)
        {
            if (a is null || b is null)
                return false;

            if (!string.Equals(a.ProductName, b.ProductName, StringComparison.Ordinal))
                return false;

            if (!string.Equals(a.Size, b.Size, StringComparison.Ordinal))
                return false;

            if (!string.Equals(a.Dough, b.Dough, StringComparison.Ordinal))
                return false;

            if (a.FinalPrice != b.FinalPrice)
                return false;

            var aAddons = a.GetEffectiveSelectedAddons()
                .OrderBy(x => x.GroupName)
                .ThenBy(x => x.Name)
                .ThenBy(x => x.Price)
                .ToList();

            var bAddons = b.GetEffectiveSelectedAddons()
                .OrderBy(x => x.GroupName)
                .ThenBy(x => x.Name)
                .ThenBy(x => x.Price)
                .ToList();

            if (aAddons.Count != bAddons.Count)
                return false;

            for (var i = 0; i < aAddons.Count; i++)
            {
                if (!string.Equals(aAddons[i].GroupName, bAddons[i].GroupName, StringComparison.Ordinal))
                    return false;

                if (!string.Equals(aAddons[i].Name, bAddons[i].Name, StringComparison.Ordinal))
                    return false;

                if (aAddons[i].Price != bAddons[i].Price)
                    return false;
            }

            return true;
        }

        private Order BuildOrder(bool isPaid, string paymentMethod, decimal finalTotal)
        {
            if (currentUser == null)
                throw new InvalidOperationException("Brak zalogowanego użytkownika.");

            var phone = DeliveryData?.Phone ?? loadedOrder?.CustomerPhone ?? string.Empty;
            var customerName = DeliveryData?.CustomerName ?? loadedOrder?.CustomerName ?? string.Empty;
            var city = DeliveryData?.City ?? loadedOrder?.City ?? string.Empty;
            var street = DeliveryData?.Street ?? loadedOrder?.Street ?? string.Empty;
            var houseNumber = DeliveryData?.HouseNumber ?? loadedOrder?.HouseNumber ?? string.Empty;
            var apartmentNumber = DeliveryData?.ApartmentNumber ?? loadedOrder?.ApartmentNumber ?? string.Empty;
            var postalCode = DeliveryData?.PostalCode ?? loadedOrder?.PostalCode ?? string.Empty;
            var fullAddress = DeliveryData != null ? DeliveryData.FullAddress : loadedOrder?.Address ?? string.Empty;
            var notes = DeliveryData?.Notes ?? loadedOrder?.Notes ?? string.Empty;

            var effectivePaymentMethod =
                !string.IsNullOrWhiteSpace(paymentMethod)
                    ? paymentMethod
                    : DeliveryData?.SelectedPaymentMethod
                      ?? loadedOrder?.PaymentMethod
                      ?? string.Empty;

            return new Order
            {
                Id = editingOrderId ?? 0,
                Type = OrderType,
                Total = finalTotal,
                IsPaid = isPaid,
                IsCanceled = loadedOrder?.IsCanceled ?? false,
                PaymentMethod = effectivePaymentMethod,
                CreatedAt = loadedOrder?.CreatedAt ?? DateTime.Now,
                CustomerPhone = phone,
                CustomerName = customerName,
                Address = fullAddress,
                City = city,
                Street = street,
                HouseNumber = houseNumber,
                ApartmentNumber = apartmentNumber,
                PostalCode = postalCode,
                DeliverySymbol = DeliveryData?.Symbol ?? loadedOrder?.DeliverySymbol ?? string.Empty,
                DeliveryCity = city,
                DeliveryStreet = street,
                DeliveryHouseNumber = houseNumber,
                DeliveryApartmentNumber = apartmentNumber,
                DeliveryPostalCode = postalCode,
                Notes = notes,
                UserId = currentUser.Id,
                Items = CartItems.Select(x => new OrderItem
                {
                    Name = x.Name,
                    Price = x.UnitPrice,
                    Quantity = x.Quantity,
                    ProductId = x.ProductId,
                    BaseProductName = string.IsNullOrWhiteSpace(x.BaseProductName) ? x.Name : x.BaseProductName,
                    BaseProductPrice = x.BaseProductPrice > 0 ? x.BaseProductPrice : x.UnitPrice,
                    ConfigurationJson = SerializePizzaConfig(x)
                }).ToList()
            };
        }

        private Order SaveOrder(bool isPaid, string paymentMethod, decimal finalTotal)
        {
            var order = BuildOrder(isPaid, paymentMethod, finalTotal);

            if (editingOrderId.HasValue)
            {
                orderRepository.UpdateOrder(order);
                var updated = orderRepository.GetById(order.Id)
                    ?? throw new InvalidOperationException("Nie udało się odczytać zapisanego zamówienia.");

                loadedOrder = updated;
                return updated;
            }

            var savedOrder = orderRepository.AddOrder(order);
            editingOrderId = savedOrder.Id;
            loadedOrder = savedOrder;
            return savedOrder;
        }

        private void PrintDocumentsForOrder(Order savedOrder, bool printReceipt, bool printKitchenBon, bool printDeliveryBon)
        {
            try
            {
                if (printKitchenBon)
                    printService.PrintKitchenBon(savedOrder, currentUser);

                if (printDeliveryBon && savedOrder.Type == "D")
                    printService.PrintDeliveryBon(savedOrder, currentUser);

                if (printReceipt)
                    printService.PrintOrderReceipt(savedOrder, currentUser);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Zamówienie zapisane, ale druk się nie udał: {ex.Message}");
            }
        }

        [RelayCommand]
        private void IncreaseQuantity()
        {
            if (!CanModifySelectedItem)
                return;

            SelectedCartItem!.Quantity++;
            RefreshCartTotals();
        }

        [RelayCommand]
        private void DecreaseQuantity()
        {
            if (!CanModifySelectedItem)
                return;

            SelectedCartItem!.Quantity--;
            if (SelectedCartItem.Quantity <= 0)
                CartItems.Remove(SelectedCartItem);

            RefreshCartTotals();
        }

        [RelayCommand]
        private void RemoveSelectedItem()
        {
            if (!CanModifySelectedItem)
                return;

            CartItems.Remove(SelectedCartItem!);
            RefreshCartTotals();
        }

        [RelayCommand]
        private void SaveAndBack()
        {
            if (CartItems.Count == 0)
            {
                MessageBox.Show("Koszyk jest pusty.");
                return;
            }

            var wasEditing = editingOrderId.HasValue;

            try
            {
                var savedOrder = SaveOrder(false, "", CartTotal);

                if (wasEditing)
                {
                    PrintEditedOrderDocuments(savedOrder, printReceipt: false);
                    CaptureOriginalItemsSnapshot();
                }
                else
                {
                    PrintDocumentsForOrder(savedOrder, printReceipt: false, printKitchenBon: true, printDeliveryBon: savedOrder.Type == "D");
                }

                MessageBox.Show(wasEditing
                    ? $"Zamówienie #{savedOrder.Id} zaktualizowane."
                    : $"Zamówienie zapisane. ID: {savedOrder.Id}");

                var currentWindow = Application.Current.Windows
                    .OfType<Window>()
                    .FirstOrDefault(w => w.DataContext == this);

                currentWindow?.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd zapisu zamówienia: {ex.Message}");
            }
        }

        [RelayCommand]
        private void GoToPayment()
        {
            if (CartItems.Count == 0)
            {
                MessageBox.Show("Koszyk jest pusty.");
                return;
            }

            var paymentItems = CartItems
                .Select(x => new PaymentCartItem
                {
                    Name = x.Name,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice
                })
                .ToList();

            var paymentWindow = new PaymentWindow(paymentItems, CartTotal, DeliveryData?.SelectedPaymentMethod)
            {
                Owner = Application.Current.Windows
                    .OfType<Window>()
                    .FirstOrDefault(w => w.DataContext == this)
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

            var wasEditing = editingOrderId.HasValue;

            try
            {
                var savedOrder = SaveOrder(
                    paymentResult.IsPaid,
                    paymentResult.PaymentMethod,
                    paymentResult.FinalTotal);

                if (wasEditing)
                {
                    PrintEditedOrderDocuments(savedOrder, printReceipt: true);
                    CaptureOriginalItemsSnapshot();
                }
                else
                {
                    PrintDocumentsForOrder(savedOrder, printReceipt: true, printKitchenBon: true, printDeliveryBon: savedOrder.Type == "D");
                }

                MessageBox.Show($"Zamówienie #{savedOrder.Id} opłacone i zapisane.");

                CartItems.Clear();
                RefreshCartTotals();

                var currentWindow = Application.Current.Windows
                    .OfType<Window>()
                    .FirstOrDefault(w => w.DataContext == this);

                currentWindow?.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd zapisu zamówienia: {ex.Message}");
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            if (CartItems.Count > 0)
            {
                var result = MessageBox.Show(
                    "Koszyk zawiera pozycje. Czy na pewno chcesz zamknąć okno bez zapisu?",
                    "Potwierdzenie",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result != MessageBoxResult.Yes)
                    return;
            }

            var currentWindow = Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.DataContext == this);

            currentWindow?.Close();
        }

        private void RefreshCartTotals()
        {
            OnPropertyChanged(nameof(CartTotal));
        }

        private List<OrderItem> GetAddedItemsDelta()
        {
            var originalMap = originalItemsSnapshot
                .Where(x => !IsKitchenIgnoredSnapshot(x))
                .GroupBy(BuildDeltaKey)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Quantity));

            var currentMap = CartItems
                .Where(x => !IsKitchenIgnoredCartItem(x))
                .GroupBy(BuildDeltaKey)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Quantity));

            var delta = new List<OrderItem>();

            foreach (var current in currentMap)
            {
                originalMap.TryGetValue(current.Key, out var originalQty);
                var addedQty = current.Value - originalQty;

                if (addedQty <= 0)
                    continue;

                delta.Add(new OrderItem
                {
                    ProductId = current.Key.ProductId,
                    Name = current.Key.Name,
                    Price = current.Key.UnitPrice,
                    Quantity = addedQty,
                    BaseProductName = current.Key.BaseProductName,
                    BaseProductPrice = current.Key.BaseProductPrice,
                    ConfigurationJson = current.Key.ConfigurationJson
                });
            }

            return delta
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Price)
                .ToList();
        }

        private void PrintKitchenDeltaIfNeeded(Order savedOrder)
        {
            if (!editingOrderId.HasValue)
                return;

            var deltaItems = GetAddedItemsDelta();
            if (!deltaItems.Any())
                return;

            var combinedNotes = string.IsNullOrWhiteSpace(savedOrder.Notes)
                ? $"DODANE DO ZAMÓWIENIA #{savedOrder.Id}"
                : $"DODANE DO ZAMÓWIENIA #{savedOrder.Id}{Environment.NewLine}{savedOrder.Notes}";

            var deltaOrder = new Order
            {
                Id = savedOrder.Id,
                Type = savedOrder.Type,
                Total = deltaItems.Sum(x => x.Price * x.Quantity),
                IsPaid = savedOrder.IsPaid,
                IsCanceled = savedOrder.IsCanceled,
                PaymentMethod = savedOrder.PaymentMethod,
                CreatedAt = savedOrder.CreatedAt,
                CustomerPhone = savedOrder.CustomerPhone,
                CustomerName = savedOrder.CustomerName,
                Address = savedOrder.Address,
                Notes = combinedNotes,
                DeliverySymbol = savedOrder.DeliverySymbol,
                DeliveryCity = savedOrder.DeliveryCity,
                DeliveryStreet = savedOrder.DeliveryStreet,
                DeliveryHouseNumber = savedOrder.DeliveryHouseNumber,
                DeliveryApartmentNumber = savedOrder.DeliveryApartmentNumber,
                DeliveryPostalCode = savedOrder.DeliveryPostalCode,
                UserId = savedOrder.UserId,
                User = savedOrder.User,
                Items = deltaItems
            };

            try
            {
                printService.PrintKitchenBon(deltaOrder, currentUser);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Zmiany zapisane, ale druk bonu różnicowego się nie udał: {ex.Message}");
            }
        }

        private void PrintEditedOrderDocuments(Order savedOrder, bool printReceipt)
        {
            PrintKitchenDeltaIfNeeded(savedOrder);

            try
            {
                if (savedOrder.Type == "D")
                    printService.PrintDeliveryBon(savedOrder, currentUser);

                if (printReceipt)
                    printService.PrintOrderReceipt(savedOrder, currentUser);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Zamówienie zapisane, ale część wydruków się nie udała: {ex.Message}");
            }
        }

        public void ApplyEditedDeliveryData(DeliveryData deliveryResult)
        {
            DeliveryData = new DeliveryData
            {
                Phone = deliveryResult.Phone,
                Symbol = deliveryResult.Symbol,
                CustomerName = deliveryResult.CustomerName,
                City = deliveryResult.City,
                Street = deliveryResult.Street,
                HouseNumber = deliveryResult.HouseNumber,
                ApartmentNumber = deliveryResult.ApartmentNumber,
                PostalCode = deliveryResult.PostalCode,
                Notes = deliveryResult.Notes,
                DeliveryPrice = deliveryResult.DeliveryPrice,
                DeliveryTime = deliveryResult.DeliveryTime,
                SelectedPaymentMethod = deliveryResult.SelectedPaymentMethod
            };

            OrderContextText = DeliveryData.ShortSummary;
            SyncDeliveryChargeItem();
            OnPropertyChanged(nameof(DeliveryData));
        }





        private static string? SerializePizzaConfig(CartItemViewModel item)
        {
            if (!item.IsPizzaConfigurable || item.PizzaConfig == null)
                return null;

            return JsonSerializer.Serialize(item.PizzaConfig);
        }

        private static PizzaConfigResult? DeserializePizzaConfig(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            try
            {
                var result = JsonSerializer.Deserialize<PizzaConfigResult>(json);
                return result;
            }
            catch
            {
                return null;
            }
        }




        private List<OrderItemDelta> GetOrderItemsDelta()
        {
            var originalMap = originalItemsSnapshot
                .GroupBy(x => new { x.Name, x.UnitPrice })
                .ToDictionary(
                    g => (g.Key.Name, g.Key.UnitPrice),
                    g => g.Sum(x => x.Quantity));

            var currentMap = CartItems
                .GroupBy(x => new { x.Name, x.UnitPrice })
                .ToDictionary(
                    g => (g.Key.Name, g.Key.UnitPrice),
                    g => g.Sum(x => x.Quantity));

            var allKeys = originalMap.Keys
                .Union(currentMap.Keys)
                .Distinct()
                .ToList();

            var delta = new List<OrderItemDelta>();

            foreach (var key in allKeys)
            {
                originalMap.TryGetValue(key, out var originalQty);
                currentMap.TryGetValue(key, out var currentQty);

                var qtyDelta = currentQty - originalQty;
                if (qtyDelta == 0)
                    continue;

                delta.Add(new OrderItemDelta
                {
                    Name = key.Name,
                    UnitPrice = key.UnitPrice,
                    QuantityDelta = qtyDelta
                });
            }

            return delta;
        }

        private void PrintOrderDeltaIfNeeded(Order savedOrder)
        {
            if (!editingOrderId.HasValue)
                return;

            var deltaRows = GetOrderItemsDelta();
            if (!deltaRows.Any())
                return;

            var deltaItems = deltaRows
                .Select(x => new OrderItem
                {
                    Name = x.QuantityDelta > 0
                        ? $"[DODANO] {x.Name}"
                        : $"[USUNIĘTO] {x.Name}",
                    Price = x.UnitPrice,
                    Quantity = Math.Abs(x.QuantityDelta),
                    ProductId = 0,
                    BaseProductName = x.Name,
                    BaseProductPrice = x.UnitPrice,
                    ConfigurationJson = null
                })
                .ToList();

            var deltaOrder = new Order
            {
                Id = savedOrder.Id,
                Type = savedOrder.Type,
                Total = deltaItems.Sum(x => x.Price * x.Quantity),
                IsPaid = savedOrder.IsPaid,
                IsCanceled = savedOrder.IsCanceled,
                PaymentMethod = savedOrder.PaymentMethod,
                CreatedAt = savedOrder.CreatedAt,
                CustomerPhone = savedOrder.CustomerPhone,
                CustomerName = savedOrder.CustomerName,
                Address = savedOrder.Address,
                Notes = $"ZMIANA W ZAMÓWIENIU #{savedOrder.Id}" +
                        (string.IsNullOrWhiteSpace(savedOrder.Notes)
                            ? string.Empty
                            : Environment.NewLine + savedOrder.Notes),
                DeliverySymbol = savedOrder.DeliverySymbol,
                DeliveryCity = savedOrder.DeliveryCity,
                DeliveryStreet = savedOrder.DeliveryStreet,
                DeliveryHouseNumber = savedOrder.DeliveryHouseNumber,
                DeliveryApartmentNumber = savedOrder.DeliveryApartmentNumber,
                DeliveryPostalCode = savedOrder.DeliveryPostalCode,
                UserId = savedOrder.UserId,
                User = savedOrder.User,
                Items = deltaItems
            };

            try
            {
                printService.PrintKitchenBon(deltaOrder, currentUser);

                if (deltaOrder.Type == "D")
                    printService.PrintDeliveryBon(deltaOrder, currentUser);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Zmiany zapisane, ale druk bonu różnicowego się nie udał: {ex.Message}");
            }
        }



        private bool IsDeliveryChargeItem(CartItemViewModel? item)
        {
            if (item == null)
                return false;

            return item.ProductId == DeliveryChargeProductId
                || string.Equals(item.Name, DeliveryChargeName, StringComparison.OrdinalIgnoreCase);
        }

        private CartItemViewModel? GetDeliveryChargeItem()
        {
            return CartItems.FirstOrDefault(IsDeliveryChargeItem);
        }

        private void UpsertDeliveryChargeItem(decimal deliveryPrice)
        {
            var deliveryItem = GetDeliveryChargeItem();

            if (deliveryPrice <= 0m)
            {
                if (deliveryItem != null)
                    CartItems.Remove(deliveryItem);

                RefreshCartTotals();
                return;
            }

            if (deliveryItem == null)
            {
                CartItems.Add(new CartItemViewModel
                {
                    ProductId = DeliveryChargeProductId,
                    Name = DeliveryChargeName,
                    UnitPrice = deliveryPrice,
                    Quantity = 1,
                    IsPizzaConfigurable = false,
                    BaseProductName = DeliveryChargeName,
                    BaseProductPrice = deliveryPrice
                });
            }
            else
            {
                deliveryItem.ProductId = DeliveryChargeProductId;
                deliveryItem.Name = DeliveryChargeName;
                deliveryItem.UnitPrice = deliveryPrice;
                deliveryItem.Quantity = 1;
                deliveryItem.IsPizzaConfigurable = false;
                deliveryItem.BaseProductName = DeliveryChargeName;
                deliveryItem.BaseProductPrice = deliveryPrice;
                deliveryItem.PizzaConfig = null;
            }

            RefreshCartTotals();
        }

        private decimal GetDeliveryChargePrice()
        {
            return GetDeliveryChargeItem()?.UnitPrice ?? 0m;
        }

        public bool CanModifySelectedItem =>
            SelectedCartItem != null && !IsDeliveryChargeItem(SelectedCartItem);

        partial void OnSelectedCartItemChanged(CartItemViewModel? value)
        {
            OnPropertyChanged(nameof(CanEditSelectedPizza));
            OnPropertyChanged(nameof(CanModifySelectedItem));
        }


        partial void OnSelectedCategoryChanged(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            LoadProducts();
        }



    }



}