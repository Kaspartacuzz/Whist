using Core;

namespace ServerAPI.Repositories;

/// <summary>
/// Repository-kontrakt for brugere.
/// Formål:
/// - Skjule persistence (MongoDB/mock) fra controller-laget
/// - Give et simpelt API til CRUD på User
/// </summary>
public interface IUserRepository
{
    /// <summary>Henter alle brugere.</summary>
    User[] GetAll();

    /// <summary>Henter én bruger ud fra id (eller null hvis ikke fundet).</summary>
    User? GetById(int id);

    /// <summary>Opretter en ny bruger (sætter Id i repo).</summary>
    void AddUser(User user);

    /// <summary>Sletter en bruger ud fra id.</summary>
    void Delete(int id);

    /// <summary>Opdaterer en bruger (samme Id).</summary>
    Task UpdateUser(User user);
}