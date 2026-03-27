using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pizzeria.Pos.Core.Models;
using Pizzeria.Pos.Services;
using Pizzeria.Pos.Wpf.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Pizzeria.Pos.Wpf.ViewModels
{
    public partial class PizzaConfigViewModel : ObservableObject
    {
        private readonly decimal basePrice;
        private readonly IPizzaAddonRepository addonRepository;

        [ObservableProperty]
        private string pizzaName;

        [ObservableProperty]
        private string selectedSize = "30 cm";

        [ObservableProperty]
        private string selectedDough = "Tradycyjne";

        [ObservableProperty]
        private PizzaAddonGroupViewModel? selectedAddonGroup;

        public ObservableCollection<PizzaAddonGroupViewModel> AddonGroups { get; } = new();

        public decimal TotalPrice => CalculateTotalPrice();

        public PizzaConfigResult? Result { get; private set; }

        public PizzaConfigViewModel(
            string pizzaName,
            decimal basePrice,
            IPizzaAddonRepository addonRepository,
            PizzaConfigResult? existingConfig = null)
        {
            this.pizzaName = pizzaName;
            this.basePrice = basePrice;
            this.addonRepository = addonRepository;

            if (existingConfig is not null)
            {
                SelectedSize = string.IsNullOrWhiteSpace(existingConfig.Size)
                    ? "30 cm"
                    : existingConfig.Size;

                SelectedDough = string.IsNullOrWhiteSpace(existingConfig.Dough)
                    ? "Tradycyjne"
                    : existingConfig.Dough;
            }

            LoadAddons(existingConfig);
            OnPropertyChanged(nameof(TotalPrice));

            // Zaznacz pierwszą kategorię domyślnie
            SelectedAddonGroup = AddonGroups.FirstOrDefault();
            if (SelectedAddonGroup is not null)
                SelectedAddonGroup.IsSelected = true;
        }

        partial void OnSelectedSizeChanged(string value)
        {
            OnPropertyChanged(nameof(TotalPrice));
        }

        partial void OnSelectedDoughChanged(string value)
        {
            OnPropertyChanged(nameof(TotalPrice));
        }

        [RelayCommand]
        private void SelectAddonGroup(PizzaAddonGroupViewModel group)
        {
            // Od-zaznacz poprzednią
            if (SelectedAddonGroup is not null)
                SelectedAddonGroup.IsSelected = false;

            SelectedAddonGroup = group;
            group.IsSelected = true;
        }

        [RelayCommand]
        private void Confirm()
        {
            var selected = GetSelectedAddons();

            Result = new PizzaConfigResult
            {
                ProductName = PizzaName,
                Size = SelectedSize,
                Dough = SelectedDough,
                SelectedAddons = selected,
                ExtraCheese = selected.Any(x => NameMatches(x.Name, "extra ser")),
                Ham = selected.Any(x => NameMatches(x.Name, "szynka")),
                ExtraSauce = selected.Any(x => NameMatches(x.Name, "sos", "dodatkowy sos")),
                FinalPrice = TotalPrice
            };

            CloseWindow(true);
        }

        [RelayCommand]
        private void Cancel()
        {
            Result = null;
            CloseWindow(false);
        }

        private void LoadAddons(PizzaConfigResult? existingConfig)
        {
            AddonGroups.Clear();

            var selectedAddons = existingConfig?.GetEffectiveSelectedAddons()
                ?? new List<PizzaAddonSelection>();

            var selectedKeys = new HashSet<string>(
                selectedAddons.Select(BuildAddonKey),
                StringComparer.OrdinalIgnoreCase);

            var dbAddons = addonRepository.GetAll()
                .Where(x => x.IsActive || selectedKeys.Contains(BuildAddonKey(x.Name, x.GroupName, x.Price)))
                .OrderBy(x => x.GroupName)
                .ThenBy(x => x.SortOrder)
                .ThenBy(x => x.Name)
                .ToList();

            foreach (var group in dbAddons.GroupBy(x => x.GroupName))
            {
                var groupVm = new PizzaAddonGroupViewModel
                {
                    GroupName = group.Key
                };

                foreach (var addon in group)
                {
                    var option = new PizzaAddonOptionViewModel
                    {
                        Id = addon.Id,
                        Name = addon.Name,
                        GroupName = addon.GroupName,
                        Price = addon.Price,
                        IsSelected = selectedKeys.Contains(BuildAddonKey(addon.Name, addon.GroupName, addon.Price))
                    };

                    option.SelectionChanged += OnAddonSelectionChanged;
                    groupVm.Items.Add(option);
                }

                AddonGroups.Add(groupVm);
            }

            var dbKeys = new HashSet<string>(
                dbAddons.Select(x => BuildAddonKey(x.Name, x.GroupName, x.Price)),
                StringComparer.OrdinalIgnoreCase);

            var archived = selectedAddons
                .Where(x => !dbKeys.Contains(BuildAddonKey(x)))
                .OrderBy(x => x.GroupName)
                .ThenBy(x => x.Name)
                .ToList();

            if (archived.Any())
            {
                var archivedGroup = new PizzaAddonGroupViewModel
                {
                    GroupName = "Archiwalne"
                };

                foreach (var addon in archived)
                {
                    var option = new PizzaAddonOptionViewModel
                    {
                        Id = addon.Id,
                        Name = addon.Name,
                        GroupName = addon.GroupName,
                        Price = addon.Price,
                        IsSelected = true
                    };

                    option.SelectionChanged += OnAddonSelectionChanged;
                    archivedGroup.Items.Add(option);
                }

                AddonGroups.Add(archivedGroup);
            }
        }

        private void OnAddonSelectionChanged()
        {
            OnPropertyChanged(nameof(TotalPrice));

            // Odśwież licznik w kategorii
            foreach (var g in AddonGroups)
                g.RefreshSelectedCount();
        }

        private decimal CalculateTotalPrice()
        {
            var total = basePrice;

            total += SelectedSize switch
            {
                "40 cm" => 10m,
                "50 cm" => 18m,
                _ => 0m
            };

            total += AddonGroups
                .SelectMany(x => x.Items)
                .Where(x => x.IsSelected)
                .Sum(x => x.Price);

            return total;
        }

        private List<PizzaAddonSelection> GetSelectedAddons()
        {
            return AddonGroups
                .SelectMany(x => x.Items)
                .Where(x => x.IsSelected)
                .Select(x => new PizzaAddonSelection
                {
                    Id = x.Id,
                    Name = x.Name,
                    GroupName = x.GroupName,
                    Price = x.Price
                })
                .OrderBy(x => x.GroupName)
                .ThenBy(x => x.Name)
                .ThenBy(x => x.Price)
                .ToList();
        }

        private void CloseWindow(bool? dialogResult)
        {
            var window = Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.DataContext == this);

            if (window is not null)
            {
                window.DialogResult = dialogResult;
                window.Close();
            }
        }

        private static bool NameMatches(string? actual, params string[] expected)
        {
            var normalizedActual = Normalize(actual);
            return expected.Any(x => Normalize(x) == normalizedActual);
        }

        private static string BuildAddonKey(PizzaAddonDefinition addon)
            => BuildAddonKey(addon.Name, addon.GroupName, addon.Price);

        private static string BuildAddonKey(PizzaAddonSelection addon)
            => BuildAddonKey(addon.Name, addon.GroupName, addon.Price);

        private static string BuildAddonKey(string? name, string? groupName, decimal price)
            => $"{Normalize(groupName)}|{Normalize(name)}|{price:F2}";

        private static string Normalize(string? value)
            => (value ?? string.Empty).Trim().ToUpperInvariant();
    }
}
