using Core;
using MongoDB.Driver;

namespace ServerAPI.Repositories.Fines;

/// <summary>
/// MongoDB implementation af IFineRepository.
/// Her ligger bøder som en "embedded list" på User-dokumentet:
///   User { Id, ..., List&lt;Fine&gt; Fines }
///
/// Fordel: simpelt for et lille projekt.
/// Ulempe: ved store datamængder kan updates blive tungere (read-modify-write på hele user-dokumentet).
/// </summary>
public class FineRepositoryMongoDB : IFineRepository
{
    private readonly IMongoCollection<User> _users;

    /// <summary>
    /// Opretter forbindelse til MongoDB baseret på appsettings.
/// </summary>
    public FineRepositoryMongoDB(IConfiguration config)
    {
        var client = new MongoClient(config["MongoDbSettings:ConnectionString"]);
        var db = client.GetDatabase(config["MongoDbSettings:DatabaseName"]);
        _users = db.GetCollection<User>("users");
    }

    // =========================
    // READ
    // =========================

    /// <inheritdoc />
    public Fine[] GetAll()
    {
        // Henter alle users og flader deres fines ud til én liste.
        // Sorterer nyeste først for en bedre UI-oplevelse.
        var allUsers = _users.Find(_ => true).ToList();

        return allUsers
            .SelectMany(u => u.Fines)
            .OrderByDescending(f => f.Date)
            .ToArray();
    }

    /// <inheritdoc />
    public Fine[] GetByUserId(int userId)
    {
        // Find én bruger og returnér deres fines (sorteret nyeste først).
        var user = _users.Find(u => u.Id == userId).FirstOrDefault();

        return user?.Fines
                   .OrderByDescending(f => f.Date)
                   .ToArray()
               ?? Array.Empty<Fine>();
    }

    // =========================
    // CREATE
    // =========================

    /// <inheritdoc />
    public void AddFine(Fine fine)
    {
        // Find brugeren som bøden tilhører.
        var user = _users.Find(u => u.Id == fine.UserId).FirstOrDefault();
        if (user == null) return;

        // ID genereres som "max + 1" indenfor brugerens fines.
        // (Fungerer fint for jeres use case med få brugere)
        fine.Id = user.Fines.Any() ? user.Fines.Max(f => f.Id) + 1 : 1;

        // Bøder får "nu" dato ved oprettelse (uanset hvad klienten sender).
        fine.Date = DateTime.Now;

        user.Fines.Add(fine);

        // Når fines ligger embedded i user, erstatter vi hele user dokumentet.
        _users.ReplaceOne(u => u.Id == user.Id, user);
    }

    // =========================
    // UPDATE
    // =========================

    /// <inheritdoc />
    public void Update(Fine fine)
    {
        // Find brugeren og opdater bøden i deres fines-liste.
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

    // =========================
    // DELETE
    // =========================

    /// <inheritdoc />
    public void Delete(int userId, int id)
    {
        // Find brugeren og fjern bøden fra deres liste.
        var user = _users.Find(u => u.Id == userId).FirstOrDefault();
        if (user == null) return;

        user.Fines = user.Fines.Where(f => f.Id != id).ToList();
        _users.ReplaceOne(u => u.Id == user.Id, user);
    }

    // =========================
    // PAGING
    // =========================

    /// <summary>
    /// Wrapper-type som MongoDB bruger ved Unwind på embedded array.
    /// Når vi Unwind'er User.Fines, kommer hvert element ud som en wrapper med property "Fines".
    /// </summary>
    public class FineWrapper
    {
        public Fine Fines { get; set; } = default!;
    }

    /// <inheritdoc />
    public PagedResult<Fine> GetPaged(int page, int pageSize, int? userId = null)
    {
        // Bemærk: disse guards var allerede i din kode.
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 5;

        var skip = (page - 1) * pageSize;

        var userFilter = userId.HasValue
            ? Builders<User>.Filter.Eq(u => u.Id, userId.Value)
            : Builders<User>.Filter.Empty;

        // ---- totalCount ----
        // totalCount beregnes forskelligt afhængigt af om vi filtrerer på userId.
        int totalCount;

        if (userId.HasValue)
        {
            // For én user: hent count af fines direkte fra user-dokumentet
            totalCount = _users.Find(userFilter)
                .Project(u => u.Fines.Count)
                .FirstOrDefault();
        }
        else
        {
            // For alle users: summer count af fines pr. user
            totalCount = _users.Aggregate()
                .Project(u => new { Count = u.Fines.Count })
                .ToList()
                .Sum(x => x.Count);
        }

        // ---- items (paged) ----
        // Her bruger vi aggregation + Unwind for at kunne sortere og page på Fine-niveau.
        var items = _users.Aggregate()
            .Match(userFilter)
            .Unwind<User, FineWrapper>(u => u.Fines)
            .SortByDescending(fw => fw.Fines.Date)
            .Skip(skip)
            .Limit(pageSize)
            .Project(fw => fw.Fines)
            .ToList();

        return new PagedResult<Fine>(items, totalCount, page, pageSize);
    }
}
