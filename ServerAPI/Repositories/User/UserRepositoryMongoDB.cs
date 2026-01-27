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

    public UserRepositoryMongoDB(IConfiguration config)
    {
        var client = new MongoClient(config["MongoDbSettings:ConnectionString"]);
        var database = client.GetDatabase(config["MongoDbSettings:DatabaseName"]);
        _users = database.GetCollection<User>(CollectionName);
    }

    // =========================
    // READ
    // =========================

    public User[] GetAll()
        => _users.Find(_ => true).ToList().ToArray();

    public User? GetById(int id)
        => _users.Find(Builders<User>.Filter.Eq(u => u.Id, id)).FirstOrDefault();

    // =========================
    // WRITE
    // =========================

    public void AddUser(User user)
    {
        // Næste id = max + 1 (eller 1 hvis tom collection)
        var query = _users.AsQueryable();
        user.Id = query.Any() ? query.Max(u => u.Id) + 1 : 1;

        _users.InsertOne(user);
    }

    public void Delete(int id)
    {
        _users.DeleteOne(u => u.Id == id);
    }

    public async Task UpdateUser(User user)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Id, user.Id);
        await _users.ReplaceOneAsync(filter, user);
    }
}