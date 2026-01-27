using Core;
using MongoDB.Driver;

namespace ServerAPI.Repositories.Points;

/// <summary>
/// MongoDB implementation af points repository.
/// Points ligger i en separat collection: "points".
/// </summary>
public class PointRepositoryMongoDB : IPointRepository
{
    private const string CollectionName = "points";

    private readonly IMongoCollection<PointEntry> _points;

    /// <summary>
    /// Opretter forbindelse til MongoDB baseret på appsettings.
    /// </summary>
    public PointRepositoryMongoDB(IConfiguration config)
    {
        var client = new MongoClient(config["MongoDbSettings:ConnectionString"]);
        var db = client.GetDatabase(config["MongoDbSettings:DatabaseName"]);
        _points = db.GetCollection<PointEntry>(CollectionName);
    }

    // =========================
    // READ
    // =========================

    /// <inheritdoc />
    public async Task<List<PointEntry>> GetAll()
    {
        return await _points.Find(_ => true).ToListAsync();
    }

    // =========================
    // WRITE
    // =========================

    /// <inheritdoc />
    public async Task Add(PointEntry point)
    {
        // Bevar adfærd: Id genereres sekventielt.
        point.Id = await GetNextId();
        await _points.InsertOneAsync(point);
    }

    /// <inheritdoc />
    public async Task Delete(int id)
    {
        await _points.DeleteOneAsync(p => p.Id == id);
    }

    /// <inheritdoc />
    public async Task DeleteAll()
    {
        await _points.DeleteManyAsync(_ => true);
    }

    // =========================
    // Helpers
    // =========================

    /// <inheritdoc />
    public async Task<int> GetNextId()
    {
        // Bevar adfærd: find højeste Id ved at sortere desc og tage første.
        var last = await _points.Find(_ => true)
            .SortByDescending(p => p.Id)
            .Limit(1)
            .FirstOrDefaultAsync();

        return last != null ? last.Id + 1 : 1;
    }
}