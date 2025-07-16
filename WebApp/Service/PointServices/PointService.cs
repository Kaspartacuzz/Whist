using System.Net.Http.Json;
using Core;

namespace WebApp.Service.PointServices;

public class PointService : IPointService
{
    private readonly HttpClient _http;

    public PointService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<PointEntry>> GetAll()
    {
        return await _http.GetFromJsonAsync<List<PointEntry>>("api/point") ?? new();
    }

    public async Task Add(PointEntry point)
    {
        await _http.PostAsJsonAsync("api/point", point);
    }

    public async Task Delete(int id)
    {
        await _http.DeleteAsync($"api/point/{id}");
    }

    public async Task DeleteAll()
    {
        await _http.DeleteAsync("api/point/all");
    }
}