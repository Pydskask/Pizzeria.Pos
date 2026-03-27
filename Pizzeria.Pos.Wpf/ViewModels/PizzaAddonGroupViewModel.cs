using System.Collections.ObjectModel;

namespace Pizzeria.Pos.Wpf.ViewModels
{
    public class PizzaAddonGroupViewModel
    {
        public string GroupName { get; set; } = string.Empty;
        public ObservableCollection<PizzaAddonOptionViewModel> Items { get; } = new();
    }
}