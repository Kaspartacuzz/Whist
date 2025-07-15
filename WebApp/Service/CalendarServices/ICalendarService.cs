using Core;

namespace WebApp.Service.CalendarServices;

public interface ICalendarService
{
    Task<List<Calendar>> GetAll();
    Task Save(Calendar calendar);
    Task Delete(DateTime date);
}