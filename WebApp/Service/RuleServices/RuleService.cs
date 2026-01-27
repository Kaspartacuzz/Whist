using System.Net.Http.Json;
using Core;

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

    // Saml routes ét sted for vedligehold.
    private const string BaseRoute = "api/rule";

    public RuleService(HttpClient http)
    {
        _http = http;
    }

    /// <inheritdoc />
    public async Task<List<Rule>> GetAll()
        => await _http.GetFromJsonAsync<List<Rule>>(BaseRoute) ?? new();

    /// <inheritdoc />
    public async Task Add(Rule rule)
        => await _http.PostAsJsonAsync(BaseRoute, rule);

    /// <inheritdoc />
    public async Task Update(Rule rule)
        => await _http.PutAsJsonAsync($"{BaseRoute}/{rule.Id}", rule);

    /// <inheritdoc />
    public async Task Delete(int id)
        => await _http.DeleteAsync($"{BaseRoute}/{id}");
}