using System.Net.Http.Json;
using Core;
using WebApp.Service.AuthServices;

namespace WebApp.Service.RuleServices;

/// <summary>
/// HTTP-baseret implementering af IRuleService.
/// 
/// Bemærk:
/// - Endpoints matcher RuleController i backend.
/// - Returnerer aldrig null-lister (giver tom liste), så UI ikke skal null-checke.
/// - Vi ændrer ikke funktionalitet: samme routes, samme payloads.
/// </summary>
public class RuleService : IRuleService
{
    private readonly HttpClient _http;
    private readonly IAuthService _auth;

    // Saml routes ét sted for vedligehold.
    private const string BaseRoute = "api/rule";

    public RuleService(HttpClient http, IAuthService auth)
    {
        _http = http;
        _auth = auth;
    }

    /// <inheritdoc />
    public async Task<List<Rule>> GetAll()
        => await _http.GetFromJsonAsync<List<Rule>>(BaseRoute) ?? new();

    /// <inheritdoc />
    public async Task Add(Rule rule)
    {
        await AddDevKeyHeaderIfLoggedIn();
       var res = await _http.PostAsJsonAsync(BaseRoute, rule);
       res.EnsureSuccessStatusCode();
    }

    /// <inheritdoc />
    public async Task Update(Rule rule)
    {
        await AddDevKeyHeaderIfLoggedIn();
        var res = await _http.PutAsJsonAsync($"{BaseRoute}/{rule.Id}", rule);
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