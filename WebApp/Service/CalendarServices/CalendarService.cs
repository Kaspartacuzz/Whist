using System.Net.Http.Json;
using Core;

namespace WebApp.Service.CalendarServices;

/// <summary>
/// HTTP-baseret implementering af ICalendarService.
/// 
/// Bemærk:
/// - Endpoints matcher CalendarController i backend.
/// - Returnerer aldrig null-lister (giver tom liste i stedet), så UI ikke skal null-checke.
/// - Vi ændrer ikke funktionalitet: samme routes, samme payloads.
/// </summary>
public class CalendarService : ICalendarService
{
    private readonly HttpClient _http;

    // Saml routes ét sted for bedre vedligehold.
    private const string BaseRoute = "api/calendar";

    public CalendarService(HttpClient http)
    {
        _http = http;
    }

    /// <inheritdoc />
    public async Task<List<Calendar>> GetAll()
    {
        return await _http.GetFromJsonAsync<List<Calendar>>(BaseRoute) ?? new();
    }

    /// <inheritdoc />
    public async Task Save(Calendar calendar)
    {
        // Backend håndterer add/update (opret/ret) på samme endpoint.
        await _http.PostAsJsonAsync(BaseRoute, calendar);
    }

    /// <inheritdoc />
    public async Task Delete(int id)
    {
        await _http.DeleteAsync($"{BaseRoute}/{id}");
    }
}