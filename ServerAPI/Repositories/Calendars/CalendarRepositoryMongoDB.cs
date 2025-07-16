using Core;
using MongoDB.Driver;

namespace ServerAPI.Repositories.Calendars;

public class CalendarRepositoryMongoDB : ICalendarRepository
{
    private readonly IMongoCollection<Calendar> _calendar;

    public CalendarRepositoryMongoDB(IConfiguration config)
    {
        var client = new MongoClient(config["MongoDbSettings:ConnectionString"]);
        var db = client.GetDatabase(config["MongoDbSettings:DatabaseName"]);
        _calendar = db.GetCollection<Calendar>("calendar");
    }

    public async Task<List<Calendar>> GetAll()
    {
        return await _calendar.Find(_ => true).ToListAsync();
    }

    public async Task<Calendar?> GetByDate(DateTime date)
    {
        return await _calendar.Find(x => x.Date == date.Date).FirstOrDefaultAsync();
    }

    public async Task AddOrUpdate(Calendar calendarEvent)
    {
        // Sikrer at datoen ikke har tid og er i UTC
        calendarEvent.Date = DateTime.SpecifyKind(calendarEvent.Date.Date, DateTimeKind.Utc);

        // Find højeste eksisterende Id i kollektionen
        var highest = await _calendar
            .Find(_ => true)
            .SortByDescending(c => c.Id)
            .Limit(1)
            .FirstOrDefaultAsync();

        calendarEvent.Id = highest != null ? highest.Id + 1 : 1;

        // Indsæt ny begivenhed
        await _calendar.InsertOneAsync(calendarEvent);
    }

    public async Task Delete(int id)
    {
        await _calendar.DeleteOneAsync(c => c.Id == id);
    }
}