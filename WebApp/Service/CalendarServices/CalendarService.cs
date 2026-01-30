using System.Net.Http.Json;
using Core;
using WebApp.Service.AuthServices;

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
    private readonly IAuthService _auth;

    // Saml routes ét sted for bedre vedligehold.
    private const string BaseRoute = "api/calendar";

    public CalendarService(HttpClient http, IAuthService auth)
    {
        _http = http;
        _auth = auth;
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
        await AddDevKeyHeaderIfLoggedIn();
        var res = await _http.PostAsJsonAsync(BaseRoute, calendar);
        res.EnsureSuccessStatusCode();
    }

    /// <inheritdoc />
    public async Task Delete(int id)
    {
        await AddDevKeyHeaderIfLoggedIn();
        var res = await _http.DeleteAsync($"{BaseRoute}/{id}");
        res.EnsureSuccessStatusCode();
    }
    
    // Helper til authentication.
    private async Task AddDevKeyHeaderIfLoggedIn()
    {
        var user = await _auth.GetCurrentUser();
        _http.DefaultRequestHeaders.Remove("X-Whist-Key");

        if (user is not null)
            _http.DefaultRequestHeaders.Add("X-Whist-Key", "whist-dev-key");
    }

}