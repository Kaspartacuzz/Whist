using System.Net.Http.Json;
using Core;
using WebApp.Service.AuthServices;

namespace WebApp.Service.HighlightServices;

/// <summary>
/// HttpClient-baseret implementation af IHighlightService.
/// Endpoints matcher ServerAPI's HighlightController (api/highlight).
/// </summary>
public class HighlightService : IHighlightService
{
    private readonly HttpClient _http;
    private readonly IAuthService _auth;

    /// <summary>
    /// HttpClient er konfigureret via DI (typisk med BaseAddress til ServerAPI).
    /// </summary>
    public HighlightService(HttpClient http, IAuthService auth)
    {
        _http = http;
        _auth = auth;
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
        await AddDevKeyHeaderIfLoggedIn();
        var response = await _http.PostAsJsonAsync("api/highlight", highlight);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Highlight>() ?? new Highlight();
    }

    /// <inheritdoc />
    public async Task Delete(int id)
    {
        // Sletter highlight i backend (ingen response-body forventes).
        await AddDevKeyHeaderIfLoggedIn();
        var res = await _http.DeleteAsync($"api/highlight/{id}");
        res.EnsureSuccessStatusCode();
    }

    /// <inheritdoc />
    public async Task Update(Highlight highlight)
    {
        // Opdaterer highlight i backend (ingen response-body forventes).
        await AddDevKeyHeaderIfLoggedIn();
        var response = await _http.PutAsJsonAsync($"api/highlight/{highlight.Id}", highlight);
        response.EnsureSuccessStatusCode();
    }

    // =========================
    // PAGING
    // =========================

    /// <inheritdoc />
    public async Task<PagedResult<Highlight>> GetPaged(
        int page,
        int pageSize = 6,
        string? searchTerm = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        bool includePrivate = true)
    {
        var url = $"api/highlight/paged?page={page}&pageSize={pageSize}&includePrivate={includePrivate}";

        if (!string.IsNullOrWhiteSpace(searchTerm))
            url += $"&searchTerm={Uri.EscapeDataString(searchTerm)}";

        if (fromDate.HasValue)
            url += $"&fromDate={Uri.EscapeDataString(fromDate.Value.ToString("O"))}";

        if (toDate.HasValue)
            url += $"&toDate={Uri.EscapeDataString(toDate.Value.ToString("O"))}";

        return await _http.GetFromJsonAsync<PagedResult<Highlight>>(url)
               ?? new PagedResult<Highlight>(new List<Highlight>(), 0, page, pageSize);
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
