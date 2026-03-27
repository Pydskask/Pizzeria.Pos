using Pizzeria.Pos.Core.Models;

namespace Pizzeria.Pos.Services;

public interface IUserRepository
{
    User? GetByPin(string pin);
    User? GetById(int id);
    List<User> GetAll();
    User Add(User user);
    void Update(User user);
}