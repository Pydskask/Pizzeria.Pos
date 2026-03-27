namespace Pizzeria.Pos.Wpf.Helpers
{
    public class PizzaAddonSelection
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}