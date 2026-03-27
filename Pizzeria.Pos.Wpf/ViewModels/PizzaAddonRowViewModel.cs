using CommunityToolkit.Mvvm.ComponentModel;

namespace Pizzeria.Pos.Wpf.ViewModels
{
    public partial class PizzaAddonRowViewModel : ObservableObject
    {
        public int Id { get; set; }

        [ObservableProperty]
        private string name = string.Empty;

        [ObservableProperty]
        private string groupName = "Serowe";

        [ObservableProperty]
        private decimal price;

        [ObservableProperty]
        private int sortOrder;

        [ObservableProperty]
        private bool isActive = true;
    }
}