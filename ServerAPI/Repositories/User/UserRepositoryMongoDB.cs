using Core;
using MongoDB.Driver;
using ServerAPI.Repositories;

namespace ServerAPI.Repositories;

public class UserRepositoryMongoDB : IUserRepository
{
    private readonly IMongoCollection<User> _users;

    public UserRepositoryMongoDB(IConfiguration config)
    {
        var client = new MongoClient(config["MongoDbSettings:ConnectionString"]);
        var database = client.GetDatabase(config["MongoDbSettings:DatabaseName"]);
        _users = database.GetCollection<User>("users");
    }

    public User[] GetAll() => _users.Find(_ => true).ToList().ToArray();

    public User? GetById(int id) => _users.Find(u => u.Id == id).FirstOrDefault();

    public void AddUser(User user)
    {
        user.Id = _users.AsQueryable().Any() ? _users.AsQueryable().Max(u => u.Id) + 1 : 1;
        _users.InsertOne(user);
    }

    public void Delete(int id)
    {
        _users.DeleteOne(u => u.Id == id);
    }
}