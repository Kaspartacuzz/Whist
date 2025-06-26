using Core;
using System.Net.Http.Json;

namespace WebApp.Service.FineServices;

public class FineServiceMock : IFineService
{
    private readonly HttpClient _http;

    public FineServiceMock(HttpClient http)
    {
        _http = http;
    }

    public async Task<Fine[]> GetAll()
    {
        return await _http.GetFromJsonAsync<Fine[]>("api/fine") ?? Array.Empty<Fine>();
    }

    public async Task<Fine[]> GetByUserId(int userId)
    {
        return await _http.GetFromJsonAsync<Fine[]>($"api/fine/user/{userId}") ?? Array.Empty<Fine>();
    }

    public async Task Add(Fine fine)
    {
        await _http.PostAsJsonAsync("api/fine", fine);
    }

    public async Task Update(Fine fine)
    {
        await _http.PutAsJsonAsync("api/fine", fine);
    }

    public async Task Delete(int id)
    {
        await _http.DeleteAsync($"api/fine/{id}");
    }
}