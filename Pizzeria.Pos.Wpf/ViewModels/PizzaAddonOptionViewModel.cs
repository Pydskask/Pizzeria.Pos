using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace Pizzeria.Pos.Wpf.ViewModels
{
    public partial class PizzaAddonOptionViewModel : ObservableObject
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public decimal Price { get; set; }

        public string DisplayLabel => $"{Name} (+{Price:F2} zł)";

        public event Action? SelectionChanged;

        [ObservableProperty]
        private bool isSelected;

        partial void OnIsSelectedChanged(bool value)
        {
            SelectionChanged?.Invoke();
        }
    }
}