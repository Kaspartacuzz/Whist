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