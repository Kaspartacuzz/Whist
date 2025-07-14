using Core;
using System.Net.Http.Json;

namespace WebApp.Service.RuleServices;

public class RuleService : IRuleService
{
    private readonly HttpClient _http;

    public RuleService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<Rule>> GetAll()
    {
        return await _http.GetFromJsonAsync<List<Rule>>("api/rule") ?? new();
    }

    public async Task Add(Rule rule)
    {
        await _http.PostAsJsonAsync("api/rule", rule);
    }

    public async Task Update(Rule rule)
    {
        await _http.PutAsJsonAsync($"api/rule/{rule.Id}", rule);
    }

    public async Task Delete(int id)
    {
        await _http.DeleteAsync($"api/rule/{id}");
    }
}