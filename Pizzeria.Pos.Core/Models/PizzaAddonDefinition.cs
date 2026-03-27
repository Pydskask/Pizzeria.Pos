namespace Pizzeria.Pos.Core.Models
{
    public class PizzaAddonDefinition
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;   // np. Serowe, Mięsne, Warzywne, Sosy
        public decimal Price { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; } = true;
    }
}