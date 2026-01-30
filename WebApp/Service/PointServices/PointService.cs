using System.Net.Http.Json;
using Core;
using WebApp.Service.AuthServices;

namespace WebApp.Service.PointServices;

/// <summary>
/// HTTP-baseret implementering af IPointService.
/// 
/// Bemærk:
/// - Endpoints matcher PointController i backend.
/// - Returnerer aldrig null-lister (giver tom liste i stedet) for at undgå null-checks i UI.
/// </summary>
public class PointService : IPointService
{
    private readonly HttpClient _http;
    private readonly IAuthService _auth;

    // Saml routes ét sted, så de er nemme at ændre (hvis du en dag ændrer controller routes).
    private const string BaseRoute = "api/point";
    private const string DeleteAllRoute = "api/point/all";

    public PointService(HttpClient http, IAuthService auth)
    {
        _http = http;
        _auth = auth;
    }

    /// <inheritdoc />
    public async Task<List<PointEntry>> GetAll()
    {
        // Returnér altid en liste (aldrig null), så UI er simpelt.
        return await _http.GetFromJsonAsync<List<PointEntry>>(BaseRoute) ?? new();
    }

    /// <inheritdoc />
    public async Task Add(PointEntry point)
    {
        // Vi forventer ikke noget svar-body.
        await AddDevKeyHeaderIfLoggedIn();
        var res = await _http.PostAsJsonAsync(BaseRoute, point);
        res.EnsureSuccessStatusCode();
    }

    /// <inheritdoc />
    public async Task Delete(int id)
    {
        await AddDevKeyHeaderIfLoggedIn();
        var res = await _http.DeleteAsync($"{BaseRoute}/{id}");
        res.EnsureSuccessStatusCode();
    }

    /// <inheritdoc />
    public async Task DeleteAll()
    {
        await AddDevKeyHeaderIfLoggedIn();
        var res = await _http.DeleteAsync(DeleteAllRoute);
        res.EnsureSuccessStatusCode();
    }
    
    // Helper til authentication
    private async Task AddDevKeyHeaderIfLoggedIn()
    {
        var user = await _auth.GetCurrentUser();
        _http.DefaultRequestHeaders.Remove("X-Whist-Key");

        if (user is not null)
            _http.DefaultRequestHeaders.Add("X-Whist-Key", "whist-dev-key");
    }
}