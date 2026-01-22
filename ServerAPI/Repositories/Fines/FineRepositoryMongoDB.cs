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

    public void Delete(int userId, int id)
    {
        var user = _users.Find(u => u.Id == userId).FirstOrDefault();
        if (user == null) return;

        user.Fines = user.Fines.Where(f => f.Id != id).ToList();
        _users.ReplaceOne(u => u.Id == user.Id, user);
    }

    public class FineWrapper
    {
        public Fine Fines { get; set; } = default!;
    }

    public PagedResult<Fine> GetPaged(int page, int pageSize, int? userId = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 5;

        var skip = (page - 1) * pageSize;

        var userFilter = userId.HasValue
            ? Builders<User>.Filter.Eq(u => u.Id, userId.Value)
            : Builders<User>.Filter.Empty;

        // totalCount
        int totalCount;
        if (userId.HasValue)
        {
            totalCount = _users.Find(userFilter)
                .Project(u => u.Fines.Count)
                .FirstOrDefault();
        }
        else
        {
            totalCount = _users.Aggregate()
                .Project(u => new { Count = u.Fines.Count })
                .ToList()
                .Sum(x => x.Count);
        }

        // Items (paged) via wrapper
        var items = _users.Aggregate()
            .Match(userFilter)
            .Unwind<User, FineWrapper>(u => u.Fines)          // output er FineWrapper
            .SortByDescending(fw => fw.Fines.Date)            // sorter på Fine.Date
            .Skip(skip)
            .Limit(pageSize)
            .Project(fw => fw.Fines)                          // projecter kun Fine ud
            .ToList();

        return new PagedResult<Fine>(items, totalCount, page, pageSize);
    }
}