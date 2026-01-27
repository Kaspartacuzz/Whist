using Core;

namespace WebApp.Service.HighlightServices;

/// <summary>
/// Kontrakt for highlight-service i frontend.
/// Bruges af Razor pages/components til at kommunikere med ServerAPI's HighlightController.
/// </summary>
public interface IHighlightService
{
    /// <summary>
    /// Henter alle highlights.
    /// OBS: God til "overblik", men kan blive tungt ved meget data.
    /// </summary>
    Task<IEnumerable<Highlight>> GetAll();

    /// <summary>
    /// Henter ét highlight ud fra id.
    /// Returnerer null hvis highlight ikke findes.
    /// </summary>
    Task<Highlight?> GetById(int id);

    /// <summary>
    /// Opretter et nyt highlight og returnerer det oprettede highlight (fra backend).
    /// </summary>
    Task<Highlight> Add(Highlight highlight);

    /// <summary>
    /// Sletter et highlight ud fra id.
    /// </summary>
    Task Delete(int id);

    /// <summary>
    /// Opdaterer et eksisterende highlight.
    /// </summary>
    Task Update(Highlight highlight);

    /// <summary>
    /// Henter highlights pagineret fra backend (server-side paging).
    /// Standard pageSize = 6 for at passe til grid'et.
    /// </summary>
    Task<PagedResult<Highlight>> GetPaged(
        int page,
        int pageSize = 6,
        string? searchTerm = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        bool includePrivate = true);

}