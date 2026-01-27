using Core;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;

namespace ServerAPI.Repositories.Highlights;

/// <summary>
/// MongoDB implementation af highlight repository.
/// Her ligger highlights i en separat collection: "highlights".
/// </summary>
public class HighlightRepositoryMongoDB : IHighlightRepository
{
    private readonly IMongoCollection<Highlight> _highlights;
    private readonly IMongoCollection<CounterDoc> _counters;

    /// <summary>
    /// Opretter forbindelse til MongoDB baseret på appsettings.
/// </summary>
    public HighlightRepositoryMongoDB(IConfiguration config)
    {
        var client = new MongoClient(config["MongoDbSettings:ConnectionString"]);
        var db = client.GetDatabase(config["MongoDbSettings:DatabaseName"]);
        _highlights = db.GetCollection<Highlight>("highlights");
        
        // Collection til tællere (counter pattern)
        _counters = db.GetCollection<CounterDoc>("counters");

        // Opret index på Date, så SortByDescending(Date) + paging bliver hurtigere ved mange highlights.
        // CreateOne er idempotent: Mongo genbruger eksisterende index med samme navn/definition.
        try
        {
            var dateIndex = new CreateIndexModel<Highlight>(
                Builders<Highlight>.IndexKeys.Descending(h => h.Date),
                new CreateIndexOptions { Name = "idx_highlights_date_desc" });

            _highlights.Indexes.CreateOne(dateIndex);
        }
        catch
        {
            // Soft fail: hvis index allerede findes eller DB ikke tillader create i miljøet,
            // så skal appen stadig kunne køre.
        }
    }

    // =========================
    // READ
    // =========================

    /// <inheritdoc />
    public async Task<IEnumerable<Highlight>> GetAll()
    {
        return await _highlights.Find(_ => true).ToListAsync();
    }

    /// <inheritdoc />
    public async Task<Highlight?> GetById(int id)
    {
        return await _highlights.Find(h => h.Id == id).FirstOrDefaultAsync();
    }

    /// <inheritdoc />
    public async Task<PagedResult<Highlight>> GetPaged(int page, int pageSize)
    {
        // Guard rails (samme adfærd som din nuværende kode)
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 6;

        var skip = (page - 1) * pageSize;

        // Total antal highlights (bruges af UI til at beregne TotalPages)
        var totalCount = (int)await _highlights.CountDocumentsAsync(_ => true);

        // Hent page items (nyeste først)
        var items = await _highlights.Find(_ => true)
            .SortByDescending(h => h.Date)
            .Skip(skip)
            .Limit(pageSize)
            .ToListAsync();

        return new PagedResult<Highlight>(items, totalCount, page, pageSize);
    }

    // =========================
    // WRITE
    // =========================

    /// <inheritdoc />
    public async Task<Highlight> Add(Highlight highlight)
    {
        // NOTE: Adfærd bevares: dato sættes til "i dag" (DateTime.Today) ved oprettelse.
        highlight.Date = DateTime.Today;

        // NOTE: Id genereres sekventielt via counter-pattern (1, 2, 3, ...), uden at scanne hele collectionen.
        highlight.Id = await GetNextIdAsync();

        await _highlights.InsertOneAsync(highlight);
        return highlight;
    }

    /// <inheritdoc />
    public async Task Delete(int id)
    {
        await _highlights.DeleteOneAsync(h => h.Id == id);
    }

    /// <inheritdoc />
    public async Task Update(Highlight highlight)
    {
        await _highlights.ReplaceOneAsync(h => h.Id == highlight.Id, highlight);
    }

    // =========================
    // Helpers
    // =========================

    private async Task<int> GetNextIdAsync()
    {
        // Automatisk: find counter doc for "highlights" og increment med 1.
        // Upsert gør at den oprettes automatisk første gang.
        var filter = Builders<CounterDoc>.Filter.Eq(c => c.Id, "highlights");
        var update = Builders<CounterDoc>.Update.Inc(c => c.Seq, 1);

        var options = new FindOneAndUpdateOptions<CounterDoc>
        {
            IsUpsert = true,
            ReturnDocument = ReturnDocument.After
        };

        var counter = await _counters.FindOneAndUpdateAsync(filter, update, options);
        return counter.Seq;
    }

    
    /// <summary>
    /// Bruges til at generere sekventielle ids (max+1) uden at scanne hele collectionen.
    /// </summary>
    private class CounterDoc
    {
        // MongoDB convention: _id er en string key, fx "highlights"
        [BsonId]
        public string Id { get; set; } = default!;

        // Selve tælleren
        public int Seq { get; set; }
    }
}
