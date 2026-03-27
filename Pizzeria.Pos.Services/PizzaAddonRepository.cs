using Pizzeria.Pos.Core.Models;
using Pizzeria.Pos.Data;

namespace Pizzeria.Pos.Services
{
    public class PizzaAddonRepository : IPizzaAddonRepository
    {
        private readonly PosDataContext context;

        public PizzaAddonRepository(PosDataContext context)
        {
            this.context = context;
        }

        public List<PizzaAddonDefinition> GetAll()
        {
            return context.PizzaAddonDefinitions
                .OrderBy(x => x.GroupName)
                .ThenBy(x => x.SortOrder)
                .ThenBy(x => x.Name)
                .ToList();
        }

        public List<PizzaAddonDefinition> GetActive()
        {
            return context.PizzaAddonDefinitions
                .Where(x => x.IsActive)
                .OrderBy(x => x.GroupName)
                .ThenBy(x => x.SortOrder)
                .ThenBy(x => x.Name)
                .ToList();
        }

        public void Add(PizzaAddonDefinition item)
        {
            context.PizzaAddonDefinitions.Add(item);
            context.SaveChanges();
        }

        public void Update(PizzaAddonDefinition item)
        {
            var existing = context.PizzaAddonDefinitions.FirstOrDefault(x => x.Id == item.Id);
            if (existing is null)
                return;

            existing.Name = item.Name;
            existing.GroupName = item.GroupName;
            existing.Price = item.Price;
            existing.SortOrder = item.SortOrder;
            existing.IsActive = item.IsActive;
            context.SaveChanges();
        }

        public void Delete(int id)
        {
            var entity = context.PizzaAddonDefinitions.FirstOrDefault(x => x.Id == id);
            if (entity is null)
                return;

            entity.IsActive = false;
            context.SaveChanges();
        }
    }
}