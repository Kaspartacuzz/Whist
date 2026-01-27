using Core;
using System.Net.Http.Json;

namespace WebApp.Service.FineServices;

/// <summary>
/// Implementation af IFineService via HttpClient.
/// OBS: Klassen hedder "Mock", men kalder et rigtigt API (det er bare navnet i projektet).
/// </summary>
public class FineServiceMock : IFineService
{
    private readonly HttpClient _http;

    /// <summary>
    /// HttpClient er typisk konfigureret i DI med BaseAddress mod din ServerAPI.
    /// </summary>
    public FineServiceMock(HttpClient http)
    {
        _http = http;
    }

    /// <inheritdoc />
    public async Task<Fine[]> GetAll()
    {
        // Henter alle bøder. Returnerer tomt array hvis API svarer null.
        return await _http.GetFromJsonAsync<Fine[]>("api/fine") ?? Array.Empty<Fine>();
    }

    /// <inheritdoc />
    public async Task<Fine[]> GetByUserId(int userId)
    {
        // Henter bøder for specifik bruger. Returnerer tomt array hvis API svarer null.
        return await _http.GetFromJsonAsync<Fine[]>($"api/fine/user/{userId}") ?? Array.Empty<Fine>();
    }

    /// <inheritdoc />
    public async Task Add(Fine fine)
    {
        // Opretter en ny bøde i backend.
        await _http.PostAsJsonAsync("api/fine", fine);
    }

    /// <inheritdoc />
    public async Task Update(Fine fine)
    {
        // Opdaterer en bøde i backend.
        await _http.PutAsJsonAsync("api/fine", fine);
    }

    /// <inheritdoc />
    public async Task Delete(int userId, int id)
    {
        // Sletter en bøde for en given bruger.
        await _http.DeleteAsync($"api/fine/user/{userId}/{id}");
    }

    /// <inheritdoc />
    public Task<PagedResult<Fine>> GetPaged(int page, int pageSize, int? userId = null)
    {
        // Henter en pagineret liste af bøder. Backend bestemmer total count.
        var url = $"api/fine/paged?page={page}&pageSize={pageSize}";
        if (userId.HasValue) url += $"&userId={userId.Value}";

        return _http.GetFromJsonAsync<PagedResult<Fine>>(url)!;
    }
}