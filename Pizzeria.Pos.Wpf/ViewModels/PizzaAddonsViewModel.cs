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
    public partial class PizzaAddonsViewModel : ObservableObject
    {
        private readonly IPizzaAddonRepository addonRepository;

        public ObservableCollection<PizzaAddonRowViewModel> Addons { get; } = new();

        public ObservableCollection<string> Groups { get; } = new()
        {
            "Serowe",
            "Mięsne",
            "Warzywne",
            "Sosy",
            "Inne"
        };

        [ObservableProperty]
        private PizzaAddonRowViewModel? selectedAddon;

        [ObservableProperty]
        private string addonName = string.Empty;

        [ObservableProperty]
        private string addonGroupName = "Serowe";

        [ObservableProperty]
        private decimal addonPrice;

        [ObservableProperty]
        private int addonSortOrder = 1;

        [ObservableProperty]
        private bool addonIsActive = true;

        [ObservableProperty]
        private string statusMessage = "Gotowe.";

        [ObservableProperty]
        private bool isEditMode;

        [ObservableProperty]
        private int editingAddonId;

        public string SaveButtonText => IsEditMode ? "Zapisz zmiany" : "Dodaj dodatek";

        public PizzaAddonsViewModel(IPizzaAddonRepository addonRepository)
        {
            this.addonRepository = addonRepository;
            LoadAddons();
        }

        partial void OnSelectedAddonChanged(PizzaAddonRowViewModel? value)
        {
            if (value is null)
                return;

            EditingAddonId = value.Id;
            AddonName = value.Name;
            AddonGroupName = value.GroupName;
            AddonPrice = value.Price;
            AddonSortOrder = value.SortOrder;
            AddonIsActive = value.IsActive;
            IsEditMode = true;
            OnPropertyChanged(nameof(SaveButtonText));
            StatusMessage = $"Wybrano dodatek: {value.Name}";
        }

        [RelayCommand]
        private void LoadAddons()
        {
            Addons.Clear();

            var items = addonRepository.GetAll()
                .OrderBy(x => x.GroupName)
                .ThenBy(x => x.SortOrder)
                .ThenBy(x => x.Name)
                .ToList();

            foreach (var item in items)
            {
                Addons.Add(new PizzaAddonRowViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    GroupName = item.GroupName,
                    Price = item.Price,
                    SortOrder = item.SortOrder,
                    IsActive = item.IsActive
                });
            }

            StatusMessage = $"Załadowano {Addons.Count} dodatków.";
        }

        [RelayCommand]
        private void NewAddon()
        {
            ClearForm();
            StatusMessage = "Tryb dodawania nowego dodatku.";
        }

        [RelayCommand]
        private void CancelEdit()
        {
            ClearForm();
            StatusMessage = "Anulowano edycję.";
        }

        [RelayCommand]
        private void SaveAddon()
        {
            if (string.IsNullOrWhiteSpace(AddonName))
            {
                StatusMessage = "Podaj nazwę dodatku.";
                return;
            }

            if (AddonPrice < 0)
            {
                StatusMessage = "Cena nie może być ujemna.";
                return;
            }

            try
            {
                if (IsEditMode && EditingAddonId > 0)
                {
                    addonRepository.Update(new PizzaAddonDefinition
                    {
                        Id = EditingAddonId,
                        Name = AddonName.Trim(),
                        GroupName = AddonGroupName,
                        Price = AddonPrice,
                        SortOrder = AddonSortOrder,
                        IsActive = AddonIsActive
                    });

                    StatusMessage = $"Zapisano zmiany dodatku: {AddonName.Trim()}";
                }
                else
                {
                    addonRepository.Add(new PizzaAddonDefinition
                    {
                        Name = AddonName.Trim(),
                        GroupName = AddonGroupName,
                        Price = AddonPrice,
                        SortOrder = AddonSortOrder,
                        IsActive = AddonIsActive
                    });

                    StatusMessage = $"Dodano dodatek: {AddonName.Trim()}";
                }

                LoadAddons();
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
            if (SelectedAddon is null)
            {
                StatusMessage = "Zaznacz dodatek do ukrycia.";
                return;
            }

            var result = MessageBox.Show(
                $"Ukryć dodatek {SelectedAddon.Name}?",
                "Potwierdź",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                var deletedName = SelectedAddon.Name;
                addonRepository.Delete(SelectedAddon.Id);
                LoadAddons();
                ClearForm();
                StatusMessage = $"Ukryto dodatek: {deletedName}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Błąd: {ex.Message}";
            }
        }

        [RelayCommand]
        private void ToggleActive()
        {
            if (SelectedAddon is null)
            {
                StatusMessage = "Zaznacz dodatek.";
                return;
            }

            try
            {
                addonRepository.Update(new PizzaAddonDefinition
                {
                    Id = SelectedAddon.Id,
                    Name = SelectedAddon.Name,
                    GroupName = SelectedAddon.GroupName,
                    Price = SelectedAddon.Price,
                    SortOrder = SelectedAddon.SortOrder,
                    IsActive = !SelectedAddon.IsActive
                });

                LoadAddons();
                ClearForm();
                StatusMessage = "Zmieniono status dodatku.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Błąd: {ex.Message}";
            }
        }

        [RelayCommand]
        private void Close()
        {
            var window = Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.DataContext == this);

            window?.Close();
        }

        private void ClearForm()
        {
            SelectedAddon = null;
            EditingAddonId = 0;
            AddonName = string.Empty;
            AddonGroupName = Groups.FirstOrDefault() ?? "Serowe";
            AddonPrice = 0m;
            AddonSortOrder = 1;
            AddonIsActive = true;
            IsEditMode = false;
            OnPropertyChanged(nameof(SaveButtonText));
        }
    }
}