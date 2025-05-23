using Core;

namespace ServerAPI.Repositories;

public class UserRepositoryMock : IUserRepository
{
    // Intern liste til at gemme brugere midlertidigt (mock-database)
    private List<User> _users = new();

    // Returnerer alle brugere som et array
    public User[] GetAll()
    {
        return _users.ToArray();
    }

    // Finder og returnerer en enkelt bruger baseret på ID
    public User? GetById(int id)
    {
        return _users.FirstOrDefault(u => u.Id == id);
    }

    // Tilføjer en ny bruger til listen
    public void Add(User user)
    {
        _users.Add(user);
    }

    // Sletter en bruger fra listen baseret på ID, hvis brugeren findes
    public void Delete(int id)
    {
        var user = GetById(id);
        if (user != null)
        {
            _users.Remove(user);
        }
    }
}