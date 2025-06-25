using Core;

namespace WebApp.Service;

    // Interface til UserService – definerer hvilke metoder der kan kaldes fra Razor-komponenter
public interface IUserService
{
    Task<User[]> GetAll();            // Henter alle brugere
    Task<User?> GetById(int id);      // Henter én bruger baseret på ID
    Task AddUser(User user);              // Tilføjer en ny bruger
    Task Delete(int id);              // Sletter en bruger baseret på ID
}