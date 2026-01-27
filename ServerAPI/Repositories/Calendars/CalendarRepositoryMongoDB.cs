using Core;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace ServerAPI.Repositories.Calendars;

/// <summary>
/// MongoDB repository for kalender.
/// Fokus: robusthed + lav vedligeholdelse.
/// </summary>
public class CalendarRepositoryMongoDB : ICalendarRepository
{
    private const string CalendarCollectionName = "calendar";
    private const string CounterCollectionName = "counters";
    private const string CalendarCounterKey = "calendar";

    private readonly IMongoCollection<Calendar> _calendar;
    private readonly IMongoCollection<CounterDoc> _counters;

    public CalendarRepositoryMongoDB(IConfiguration config)
    {
        var client = new MongoClient(config["MongoDbSettings:ConnectionString"]);
        var db = client.GetDatabase(config["MongoDbSettings:DatabaseName"]);

        _calendar = db.GetCollection<Calendar>(CalendarCollectionName);
        _counters = db.GetCollection<CounterDoc>(CounterCollectionName);

        EnsureIndexes();
    }

    private void EnsureIndexes()
    {
        // 1) Én event pr. dato (unik)
        var dateIndex = new CreateIndexModel<Calendar>(
            Builders<Calendar>.IndexKeys.Ascending(x => x.Date),
            new CreateIndexOptions { Unique = true });

        // 2) Worker-kald: ReminderSent + Date
        var reminderIndex = new CreateIndexModel<Calendar>(
            Builders<Calendar>.IndexKeys.Ascending(x => x.ReminderSent).Ascending(x => x.Date));

        _calendar.Indexes.CreateMany(new[] { dateIndex, reminderIndex });
    }

    public async Task<List<Calendar>> GetAll()
    {
        return await _calendar.Find(_ => true).ToListAsync();
    }

    public async Task<Calendar?> GetByDate(DateTime date)
    {
        // Dato gemmes som UTC "date-only"
        var utcDateOnly = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);
        return await _calendar.Find(x => x.Date == utcDateOnly).FirstOrDefaultAsync();
    }

    public async Task AddOrUpdate(Calendar calendarEvent)
    {
        // Ensure date stored as UTC date-only
        calendarEvent.Date = DateTime.SpecifyKind(calendarEvent.Date.Date, DateTimeKind.Utc);

        // Hvis event allerede findes på datoen, så skal vi bevare eksisterende Id og ReminderSent
        // (så vi ikke "nulstiller" ReminderSent ved en edit).
        var existing = await GetByDate(calendarEvent.Date);

        if (existing is null)
        {
            // Ny dato → nyt id (O(1) counter)
            calendarEvent.Id = await GetNextId();

            // ReminderSent skal normalt være false for et nyt event.
            // (Modelens default er false, men vi sætter den eksplicit for tydelighed.)
            calendarEvent.ReminderSent = false;
        }
        else
        {
            // Eksisterende dato → bevar id + reminder state
            calendarEvent.Id = existing.Id;
            calendarEvent.ReminderSent = existing.ReminderSent;
        }

        // Upsert på dato (unik index sørger for at der ikke kan komme dubletter).
        var filter = Builders<Calendar>.Filter.Eq(x => x.Date, calendarEvent.Date);
        await _calendar.ReplaceOneAsync(filter, calendarEvent, new ReplaceOptions { IsUpsert = true });
    }

    public async Task Delete(int id)
    {
        await _calendar.DeleteOneAsync(c => c.Id == id);
    }

    public async Task<List<Calendar>> FindByExactOffsetDays(int offsetDays)
    {
        // Europe/Copenhagen (Windows-id = "Central Europe Standard Time")
        var tz = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");
        var todayLocal = TimeZoneInfo.ConvertTime(DateTime.UtcNow, tz).Date;
        var targetLocalDate = todayLocal.AddDays(offsetDays);

        // Vi gemmer datoer som "hele dage" (utc-dato uden tid)
        var targetUtcDate = DateTime.SpecifyKind(targetLocalDate, DateTimeKind.Utc);

        var filter = Builders<Calendar>.Filter.And(
            Builders<Calendar>.Filter.Eq(x => x.Date, targetUtcDate),
            Builders<Calendar>.Filter.Eq(x => x.ReminderSent, false)
        );

        return await _calendar.Find(filter).ToListAsync();
    }

    public async Task MarkReminderSent(int calendarId)
    {
        var update = Builders<Calendar>.Update.Set(x => x.ReminderSent, true);
        await _calendar.UpdateOneAsync(c => c.Id == calendarId, update);
    }

    public async Task MarkRemindersSent(IEnumerable<int> calendarIds)
    {
        var ids = calendarIds?.Distinct().ToArray() ?? Array.Empty<int>();
        if (ids.Length == 0) return;

        var filter = Builders<Calendar>.Filter.In(x => x.Id, ids);
        var update = Builders<Calendar>.Update.Set(x => x.ReminderSent, true);

        await _calendar.UpdateManyAsync(filter, update);
    }

    // --------------------------
    // Counter (O(1) id)
    // --------------------------

    private async Task<int> GetNextId()
    {
        var filter = Builders<CounterDoc>.Filter.Eq(x => x.Id, CalendarCounterKey);
        var update = Builders<CounterDoc>.Update.Inc(x => x.Seq, 1);

        var options = new FindOneAndUpdateOptions<CounterDoc>
        {
            IsUpsert = true,
            ReturnDocument = ReturnDocument.After
        };

        var doc = await _counters.FindOneAndUpdateAsync(filter, update, options);
        return doc.Seq;
    }

    private class CounterDoc
    {
        [BsonId]
        public string Id { get; set; } = default!;

        public int Seq { get; set; }
    }
}
