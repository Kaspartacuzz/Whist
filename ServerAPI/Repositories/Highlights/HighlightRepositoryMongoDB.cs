using Core;
using MongoDB.Driver;
using ServerAPI.Repositories.Highlights;

namespace ServerAPI.Repositories.Highlights;

public class HighlightRepositoryMongoDB : IHighlightRepository
{
    private readonly IMongoCollection<Highlight> _highlights;

    public HighlightRepositoryMongoDB(IConfiguration config)
    {
        var client = new MongoClient(config["MongoDbSettings:ConnectionString"]);
        var db = client.GetDatabase(config["MongoDbSettings:DatabaseName"]);
        _highlights = db.GetCollection<Highlight>("highlights");
    }

    public async Task<IEnumerable<Highlight>> GetAll()
    {
        return await _highlights.Find(_ => true).ToListAsync();
    }

    public async Task<Highlight?> GetById(int id)
    {
        return await _highlights.Find(h => h.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Highlight> Add(Highlight highlight)
    {
        highlight.Date = DateTime.Today;
        highlight.Id = await GetNextIdAsync();
        await _highlights.InsertOneAsync(highlight);
        return highlight;
    }

    public async Task Delete(int id)
    {
        await _highlights.DeleteOneAsync(h => h.Id == id);
    }

    public async Task Update(Highlight highlight)
    {
        await _highlights.ReplaceOneAsync(h => h.Id == highlight.Id, highlight);
    }

    // HighlightRepositoryMongoDB.cs
    public async Task<PagedResult<Highlight>> GetPaged(int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 6;

        var skip = (page - 1) * pageSize;

        var totalCount = (int)await _highlights.CountDocumentsAsync(_ => true);

        var items = await _highlights.Find(_ => true)
            .SortByDescending(h => h.Date)
            .Skip(skip)
            .Limit(pageSize)
            .ToListAsync();

        return new PagedResult<Highlight>(items, totalCount, page, pageSize);
    }

    private async Task<int> GetNextIdAsync()
    {
        var list = await _highlights.Find(_ => true).ToListAsync();
        return list.Any() ? list.Max(h => h.Id) + 1 : 1;
    }
}