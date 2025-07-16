using Core;
using MongoDB.Driver;

namespace ServerAPI.Repositories.Points;

public class PointRepositoryMongoDB : IPointRepository
{
    private readonly IMongoCollection<PointEntry> _points;

    public PointRepositoryMongoDB(IConfiguration config)
    {
        var client = new MongoClient(config["MongoDbSettings:ConnectionString"]);
        var db = client.GetDatabase(config["MongoDbSettings:DatabaseName"]);
        _points = db.GetCollection<PointEntry>("points");
    }

    public async Task<List<PointEntry>> GetAll()
    {
        return await _points.Find(_ => true).ToListAsync();
    }

    public async Task Add(PointEntry point)
    {
        point.Id = await GetNextId();
        await _points.InsertOneAsync(point);
    }

    public async Task Delete(int id)
    {
        await _points.DeleteOneAsync(p => p.Id == id);
    }

    public async Task<int> GetNextId()
    {
        var last = await _points.Find(_ => true)
            .SortByDescending(p => p.Id)
            .Limit(1)
            .FirstOrDefaultAsync();

        return last != null ? last.Id + 1 : 1;
    }

    public async Task DeleteAll()
    {
        await _points.DeleteManyAsync(_ => true);
    }
}