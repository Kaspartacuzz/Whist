using Core;
using MongoDB.Driver;
using ServerAPI.Repositories.Fines;

namespace ServerAPI.Repositories.Fines;

public class FineRepositoryMongoDB : IFineRepository
{
    private readonly IMongoCollection<User> _users;

    public FineRepositoryMongoDB(IConfiguration config)
    {
        var client = new MongoClient(config["MongoDbSettings:ConnectionString"]);
        var db = client.GetDatabase(config["MongoDbSettings:DatabaseName"]);
        _users = db.GetCollection<User>("users");
    }

    public Fine[] GetAll()
    {
        var allUsers = _users.Find(_ => true).ToList();
        return allUsers.SelectMany(u => u.Fines).OrderByDescending(f => f.Date).ToArray();
    }

    public Fine[] GetByUserId(int userId)
    {
        var user = _users.Find(u => u.Id == userId).FirstOrDefault();
        return user?.Fines.OrderByDescending(f => f.Date).ToArray() ?? Array.Empty<Fine>();
    }

    public void AddFine(Fine fine)
    {
        var user = _users.Find(u => u.Id == fine.UserId).FirstOrDefault();
        if (user == null) return;

        fine.Id = user.Fines.Any() ? user.Fines.Max(f => f.Id) + 1 : 1;
        fine.Date = DateTime.Now;

        user.Fines.Add(fine);
        _users.ReplaceOne(u => u.Id == user.Id, user);
    }
    
    public void Update(Fine fine)
    {
        var user = _users.Find(u => u.Id == fine.UserId).FirstOrDefault();
        if (user == null) return;

        var finesList = user.Fines.ToList();
        var index = finesList.FindIndex(f => f.Id == fine.Id);
        if (index >= 0)
        {
            finesList[index] = fine;
            user.Fines = finesList;
            _users.ReplaceOne(u => u.Id == user.Id, user);
        }
    }

    public void Delete(int id)
    {
        var user = _users.Find(u => u.Fines.Any(f => f.Id == id)).FirstOrDefault();
        if (user == null) return;

        user.Fines = user.Fines.Where(f => f.Id != id).ToList();
        _users.ReplaceOne(u => u.Id == user.Id, user);
    }
}