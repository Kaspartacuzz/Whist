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
        // Find eksisterende event for samme dato
        var existingEvent = await _calendar.Find(c => c.Date == calendarEvent.Date).FirstOrDefaultAsync();

        if (existingEvent != null)
        {
            // Behold eksisterende Id
            calendarEvent.Id = existingEvent.Id;
        }
        else
        {
            // Find højeste eksisterende Id
            var highest = await _calendar
                .Find(_ => true)
                .SortByDescending(c => c.Id)
                .Limit(1)
                .FirstOrDefaultAsync();

            calendarEvent.Id = highest != null ? highest.Id + 1 : 1;
        }
        
        await _calendar.ReplaceOneAsync(
            filter: c => c.Date == calendarEvent.Date,
            replacement: calendarEvent,
            options: new ReplaceOptions { IsUpsert = true });
    }

    public async Task DeleteByDate(DateTime date)
    {
        await _calendar.DeleteOneAsync(c => c.Date == date.Date);
    }
}