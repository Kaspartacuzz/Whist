using Core;

namespace ServerAPI.Repositories;

public class UserRepository : IUserRepository
{
    private List<User> _users = new();

    public User[] GetAll()
    {
        return _users.ToArray();
    }

    public User? GetById(int id)
    {
        return _users.FirstOrDefault(u => u.Id == id);
    }

    public void Add(User user)
    {
        _users.Add(user);
    }

    public void Delete(int id)
    {
        var user = GetById(id);
        if (user != null)
        {
            _users.Remove(user);
        }
    }
}