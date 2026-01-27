using Core;

namespace WebApp.Service.FineServices;

/// <summary>
/// Kontrakt for bøde-service i frontend.
/// Denne service bruges af Razor pages/components til at kommunikere med ServerAPI (FineController).
/// </summary>
public interface IFineService
{
    /// <summary>
    /// Henter alle bøder (bruges primært til overblik/summary).
    /// </summary>
    Task<Fine[]> GetAll();

    /// <summary>
    /// Henter alle bøder for en bestemt bruger.
    /// </summary>
    Task<Fine[]> GetByUserId(int userId);

    /// <summary>
    /// Opretter en ny bøde.
    /// </summary>
    Task Add(Fine fine);

    /// <summary>
    /// Opdaterer en eksisterende bøde (fx IsPaid/kommentar/beløb).
    /// </summary>
    Task Update(Fine fine);

    /// <summary>
    /// Sletter en bøde for en given bruger.
    /// </summary>
    Task Delete(int userId, int id);

    /// <summary>
    /// Henter bøder pagineret fra backend.
    /// page = 1 betyder første side.
    /// </summary>
    Task<PagedResult<Fine>> GetPaged(int page, int pageSize, int? userId = null);
}