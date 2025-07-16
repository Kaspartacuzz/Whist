namespace WebApp.Service.CalendarServices;

using System.Net.Http.Json;
using Core;

public class CalendarService : ICalendarService
{
    private readonly HttpClient _http;

    public CalendarService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<Calendar>> GetAll()
    {
        return await _http.GetFromJsonAsync<List<Calendar>>("api/calendar") ?? new();
    }

    public async Task Save(Calendar calendar)
    {
        await _http.PostAsJsonAsync("api/calendar", calendar);
    }

    public async Task Delete(int id)
    {
        await _http.DeleteAsync($"api/calendar/{id}");
    }
}
