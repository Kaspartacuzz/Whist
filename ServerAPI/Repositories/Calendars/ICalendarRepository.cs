using Core;

namespace ServerAPI.Repositories.Calendars;

public interface ICalendarRepository
{
    Task<List<Calendar>> GetAll();
    Task<Calendar?> GetByDate(DateTime date);
    Task AddOrUpdate(Calendar evt);
    Task Delete(int id);
}