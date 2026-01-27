using System.Net.Http.Json;
using Core;

namespace WebApp.Service.HighlightServices;

/// <summary>
/// HttpClient-baseret implementation af IHighlightService.
/// Endpoints matcher ServerAPI's HighlightController (api/highlight).
/// </summary>
public class HighlightService : IHighlightService
{
    private readonly HttpClient _http;

    /// <summary>
    /// HttpClient er konfigureret via DI (typisk med BaseAddress til ServerAPI).
    /// </summary>
    public HighlightService(HttpClient http)
    {
        _http = http;
    }

    // =========================
    // READ
    // =========================

    /// <inheritdoc />
    public async Task<IEnumerable<Highlight>> GetAll()
    {
        // Returnerer tom liste hvis API svarer null.
        return await _http.GetFromJsonAsync<IEnumerable<Highlight>>("api/highlight")
               ?? new List<Highlight>();
    }

    /// <inheritdoc />
    public async Task<Highlight?> GetById(int id)
    {
        // Returnerer null hvis highlight ikke findes.
        return await _http.GetFromJsonAsync<Highlight>($"api/highlight/{id}");
    }

    // =========================
    // WRITE
    // =========================

    /// <inheritdoc />
    public async Task<Highlight> Add(Highlight highlight)
    {
        // Opretter highlight i backend og forventer at backend returnerer highlight objektet.
        var response = await _http.PostAsJsonAsync("api/highlight", highlight);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Highlight>() ?? new Highlight();
    }

    /// <inheritdoc />
    public async Task Delete(int id)
    {
        // Sletter highlight i backend (ingen response-body forventes).
        await _http.DeleteAsync($"api/highlight/{id}");
    }

    /// <inheritdoc />
    public async Task Update(Highlight highlight)
    {
        // Opdaterer highlight i backend (ingen response-body forventes).
        var response = await _http.PutAsJsonAsync($"api/highlight/{highlight.Id}", highlight);
        response.EnsureSuccessStatusCode();
    }

    // =========================
    // PAGING
    // =========================

    /// <inheritdoc />
    public async Task<PagedResult<Highlight>> GetPaged(int page, int pageSize = 6)
    {
        // Henter highlights pagineret (server-side).
        var url = $"api/highlight/paged?page={page}&pageSize={pageSize}";

        return await _http.GetFromJsonAsync<PagedResult<Highlight>>(url)
               ?? new PagedResult<Highlight>(new List<Highlight>(), 0, page, pageSize);
    }
}
