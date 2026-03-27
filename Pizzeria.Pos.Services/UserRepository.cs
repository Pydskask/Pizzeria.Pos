using Pizzeria.Pos.Core.Models;
using Pizzeria.Pos.Data;

namespace Pizzeria.Pos.Services;

public class UserRepository : IUserRepository
{
    private readonly PosDataContext _context;

    public UserRepository(PosDataContext context)
    {
        _context = context;
    }

    public User? GetByPin(string pin)
    {
        return _context.Users.FirstOrDefault(u => u.Pin == pin && u.IsActive);
    }

    public User? GetById(int id)
    {
        return _context.Users.FirstOrDefault(u => u.Id == id);
    }

    public List<User> GetAll()
    {
        return _context.Users
            .OrderBy(u => u.Name)
            .ToList();
    }

    public User Add(User user)
    {
        _context.Users.Add(user);
        _context.SaveChanges();
        return user;
    }

    public void Update(User user)
    {
        _context.Users.Update(user);
        _context.SaveChanges();
    }
}