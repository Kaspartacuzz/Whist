using Core;
using MongoDB.Driver;

namespace ServerAPI.Repositories;

/// <summary>
/// MongoDB repository for brugere.
/// Collection: "users"
/// 
/// Bemærk:
/// - Id sættes manuelt (Max(Id)+1).
/// - For jeres 4 brugere er det helt fint og kræver minimal vedligehold.
/// </summary>
public class UserRepositoryMongoDB : IUserRepository
{
    private const string CollectionName = "users";
    private readonly IMongoCollection<User> _users;

    /// <summary>
    /// Opretter forbindelse til MongoDB baseret på appsettings og initialiserer users-collection.
    /// </summary>
    public UserRepositoryMongoDB(IConfiguration config)
    {
        var client = new MongoClient(config["MongoDbSettings:ConnectionString"]);
        var database = client.GetDatabase(config["MongoDbSettings:DatabaseName"]);
        _users = database.GetCollection<User>(CollectionName);
    }

    // =========================
    // READ
    // =========================

    /// <summary>
    /// Returnerer alle brugere fra databasen.
    /// </summary>
    public User[] GetAll()
        => _users.Find(_ => true).ToList().ToArray();

    /// <summary>
    /// Finder én bruger ud fra Id (returnerer null hvis brugeren ikke findes).
    /// </summary>
    public User? GetById(int id)
        => _users.Find(Builders<User>.Filter.Eq(u => u.Id, id)).FirstOrDefault();

    // =========================
    // WRITE
    // =========================

    /// <summary>
    /// Opretter en ny bruger ved at sætte næste Id (max+1) og indsætte brugeren i databasen.
    /// </summary>
    public void AddUser(User user)
    {
        // Næste id = max + 1 (eller 1 hvis tom collection)
        var query = _users.AsQueryable();
        user.Id = query.Any() ? query.Max(u => u.Id) + 1 : 1;

        _users.InsertOne(user);
    }

    /// <summary>
    /// Sletter en bruger ud fra Id.
    /// </summary>
    public void Delete(int id)
    {
        _users.DeleteOne(u => u.Id == id);
    }

    /// <summary>
    /// Opdaterer en eksisterende bruger ved at erstatte hele dokumentet med samme Id.
    /// </summary>
    public async Task UpdateUser(User user)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Id, user.Id);
        await _users.ReplaceOneAsync(filter, user);
    }
}
