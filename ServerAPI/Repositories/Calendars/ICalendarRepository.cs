using Core;

namespace ServerAPI.Repositories.Calendars;

/// <summary>
/// Repository-kontrakt for kalender.
/// </summary>
public interface ICalendarRepository
{
    Task<List<Calendar>> GetAll();
    Task<Calendar?> GetByDate(DateTime date);

    /// <summary>
    /// Opretter/retter event for en dato.
    /// Dato håndteres som "date-only".
    /// </summary>
    Task AddOrUpdate(Calendar evt);

    Task Delete(int id);

    /// <summary>
    /// Finder events der ligger præcis offsetDays fra i dag (i lokal timezone),
    /// og som ikke allerede har fået sendt reminder.
    /// </summary>
    Task<List<Calendar>> FindByExactOffsetDays(int offsetDays);

    /// <summary>Markerer ét event som sendt.</summary>
    Task MarkReminderSent(int calendarId);

    /// <summary>
    /// Marker flere events som sendt i ét kald (bedre performance).
    /// </summary>
    Task MarkRemindersSent(IEnumerable<int> calendarIds);
}