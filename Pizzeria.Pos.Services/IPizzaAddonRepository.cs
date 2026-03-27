using Pizzeria.Pos.Core.Models;

namespace Pizzeria.Pos.Services
{
    public interface IPizzaAddonRepository
    {
        List<PizzaAddonDefinition> GetAll();
        List<PizzaAddonDefinition> GetActive();
        void Add(PizzaAddonDefinition item);
        void Update(PizzaAddonDefinition item);
        void Delete(int id);
    }
}