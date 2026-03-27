using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;

namespace Pizzeria.Pos.Wpf.ViewModels
{
    public partial class PizzaAddonGroupViewModel : ObservableObject
    {
        public string GroupName { get; set; } = string.Empty;

        public ObservableCollection<PizzaAddonOptionViewModel> Items { get; } = new();

        [ObservableProperty]
        private bool isSelected;

        [ObservableProperty]
        private int selectedCount;

        public void RefreshSelectedCount()
        {
            SelectedCount = Items.Count(x => x.IsSelected);
        }
    }
}
