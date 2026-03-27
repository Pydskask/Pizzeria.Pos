using System.Collections.Generic;
using System.Linq;

namespace Pizzeria.Pos.Wpf.Helpers
{
    public class PizzaConfigResult
    {
        public string ProductName { get; set; } = string.Empty;
        public string Size { get; set; } = "30 cm";
        public string Dough { get; set; } = "Tradycyjne";

        // Nowy model
        public List<PizzaAddonSelection> SelectedAddons { get; set; } = new();

        // Zgodność wsteczna dla starych zapisów JSON
        public bool ExtraCheese { get; set; }
        public bool Ham { get; set; }
        public bool ExtraSauce { get; set; }

        public decimal FinalPrice { get; set; }

        public List<PizzaAddonSelection> GetEffectiveSelectedAddons()
        {
            var result = SelectedAddons?
                .Select(x => new PizzaAddonSelection
                {
                    Id = x.Id,
                    Name = x.Name,
                    GroupName = x.GroupName,
                    Price = x.Price
                })
                .ToList() ?? new List<PizzaAddonSelection>();

            AddLegacyIfMissing(result, ExtraCheese, "extra ser", "Serowe", 6m);
            AddLegacyIfMissing(result, Ham, "szynka", "Mięsne", 7m);
            AddLegacyIfMissing(result, ExtraSauce, "sos", "Sosy", 4m);

            return result
                .OrderBy(x => x.GroupName)
                .ThenBy(x => x.Name)
                .ThenBy(x => x.Price)
                .ToList();
        }

        public string BuildDisplayName()
        {
            var addons = GetEffectiveSelectedAddons();
            var addonsText = addons.Count == 0
                ? string.Empty
                : ", " + string.Join(", ", addons.Select(x => x.Name));

            return $"{ProductName}, {Size}, {Dough}{addonsText}";
        }

        public PizzaConfigResult Clone()
        {
            return new PizzaConfigResult
            {
                ProductName = ProductName,
                Size = Size,
                Dough = Dough,
                SelectedAddons = GetEffectiveSelectedAddons()
                    .Select(x => new PizzaAddonSelection
                    {
                        Id = x.Id,
                        Name = x.Name,
                        GroupName = x.GroupName,
                        Price = x.Price
                    })
                    .ToList(),
                ExtraCheese = ExtraCheese,
                Ham = Ham,
                ExtraSauce = ExtraSauce,
                FinalPrice = FinalPrice
            };
        }

        private static void AddLegacyIfMissing(
            List<PizzaAddonSelection> target,
            bool enabled,
            string name,
            string groupName,
            decimal price)
        {
            if (!enabled)
                return;

            var exists = target.Any(x =>
                Normalize(x.Name) == Normalize(name) &&
                Normalize(x.GroupName) == Normalize(groupName));

            if (!exists)
            {
                target.Add(new PizzaAddonSelection
                {
                    Name = name,
                    GroupName = groupName,
                    Price = price
                });
            }
        }

        private static string Normalize(string? value)
        {
            return (value ?? string.Empty).Trim().ToUpperInvariant();
        }
    }
}