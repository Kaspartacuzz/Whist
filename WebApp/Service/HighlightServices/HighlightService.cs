using System.Net.Http.Json;
using Core;

namespace WebApp.Service.HighlightServices;

public class HighlightService : IHighlightService
{
    private readonly HttpClient _http;

    public HighlightService(HttpClient http)
    {
        _http = http;
    }

    public async Task<IEnumerable<Highlight>> GetAll()
    {
        return await _http.GetFromJsonAsync<IEnumerable<Highlight>>("api/highlight") 
               ?? new List<Highlight>();
    }

    public async Task<Highlight?> GetById(int id)
    {
        return await _http.GetFromJsonAsync<Highlight>($"api/highlight/{id}");
    }

    public async Task<Highlight> Add(Highlight highlight)
    {
        var response = await _http.PostAsJsonAsync("api/highlight", highlight);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Highlight>() ?? new Highlight();
    }

    public async Task Delete(int id)
    {
        await _http.DeleteAsync($"api/highlight/{id}");
    }
    
    public async Task Update(Highlight highlight)
    {
        var response = await _http.PutAsJsonAsync($"api/highlight/{highlight.Id}", highlight);
        response.EnsureSuccessStatusCode();
    }
}