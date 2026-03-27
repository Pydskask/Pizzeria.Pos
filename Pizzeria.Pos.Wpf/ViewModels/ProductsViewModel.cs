using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pizzeria.Pos.Core.Models;
using Pizzeria.Pos.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Pizzeria.Pos.Wpf.ViewModels
{
    public partial class ProductsViewModel : ObservableObject
    {
        private readonly IProductRepository productRepo;

        [ObservableProperty]
        private ObservableCollection<Product> products = new();

        [ObservableProperty]
        private Product? selectedProduct;

        [ObservableProperty]
        private string newProductName = string.Empty;

        [ObservableProperty]
        private decimal newProductPrice = 0m;

        [ObservableProperty]
        private string newProductCategory = "Pizza";

        [ObservableProperty]
        private string statusMessage = "Gotowe.";

        [ObservableProperty]
        private bool isEditMode;

        [ObservableProperty]
        private int editingProductId;

        public ObservableCollection<string> Categories { get; } = new()
        {
            "Pizza",
            "Napoje zimne",
            "Napoje gorące",
            "Dodatki",
            "Desery"
        };

        public string SaveButtonText => IsEditMode ? "Zapisz zmiany" : "Dodaj produkt";

        public ProductsViewModel(IProductRepository productRepo)
        {
            this.productRepo = productRepo ?? throw new ArgumentNullException(nameof(productRepo));
            LoadProducts();
        }

        partial void OnSelectedProductChanged(Product? value)
        {
            if (value == null)
                return;

            EditingProductId = value.Id;
            NewProductName = value.Name;
            NewProductPrice = value.Price;
            NewProductCategory = value.Category;
            IsEditMode = true;
            OnPropertyChanged(nameof(SaveButtonText));

            StatusMessage = $"Wybrano produkt: {value.Name}";
        }

        private void LoadProducts()
        {
            var items = productRepo.GetAll()
                .OrderBy(x => x.Category)
                .ThenBy(x => x.Name)
                .ToList();

            Products.Clear();
            foreach (var item in items)
                Products.Add(item);

            StatusMessage = $"Załadowano {Products.Count} produktów.";
        }

        private void ClearForm()
        {
            SelectedProduct = null;
            EditingProductId = 0;
            NewProductName = string.Empty;
            NewProductPrice = 0m;
            NewProductCategory = Categories.FirstOrDefault() ?? "Pizza";
            IsEditMode = false;
            OnPropertyChanged(nameof(SaveButtonText));
        }

        [RelayCommand]
        private void NewProduct()
        {
            ClearForm();
            StatusMessage = "Tryb dodawania nowego produktu.";
        }

        [RelayCommand]
        private void CancelEdit()
        {
            ClearForm();
            StatusMessage = "Anulowano edycję.";
        }

        [RelayCommand]
        private void SaveProduct()
        {
            if (string.IsNullOrWhiteSpace(NewProductName) || NewProductPrice <= 0)
            {
                StatusMessage = "Podaj nazwę i cenę > 0.";
                return;
            }

            try
            {
                if (IsEditMode && EditingProductId > 0)
                {
                    var updatedProduct = new Product
                    {
                        Id = EditingProductId,
                        Name = NewProductName.Trim(),
                        Price = NewProductPrice,
                        Category = NewProductCategory
                    };

                    productRepo.UpdateProduct(updatedProduct);
                    StatusMessage = $"Zapisano zmiany produktu: {updatedProduct.Name}";
                }
                else
                {
                    var newProduct = new Product
                    {
                        Name = NewProductName.Trim(),
                        Price = NewProductPrice,
                        Category = NewProductCategory
                    };

                    productRepo.AddProduct(newProduct);
                    StatusMessage = $"Dodano produkt: {newProduct.Name}";
                }

                LoadProducts();
                ClearForm();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Błąd: {ex.Message}";
            }
        }

        [RelayCommand]
        private void DeleteSelected()
        {
            if (SelectedProduct == null)
            {
                StatusMessage = "Zaznacz produkt do usunięcia.";
                return;
            }

            var result = MessageBox.Show(
                $"Usunąć produkt „{SelectedProduct.Name}”?",
                "Potwierdź usunięcie",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                productRepo.DeleteProduct(SelectedProduct.Id);
                var deletedName = SelectedProduct.Name;

                LoadProducts();
                ClearForm();

                StatusMessage = $"Produkt usunięty: {deletedName}";
            }
            catch (InvalidOperationException ex)
            {
                StatusMessage = ex.Message;
                MessageBox.Show(ex.Message, "Usuwanie zablokowane", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Błąd: {ex.Message}";
            }
        }

        [RelayCommand]
        private void Refresh()
        {
            LoadProducts();
            StatusMessage = "Lista odświeżona.";
        }

        [RelayCommand]
        private void Close()
        {
            var window = Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.DataContext == this);

            window?.Close();
        }
    }
}