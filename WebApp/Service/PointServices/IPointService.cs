using Core;

namespace WebApp.Service.PointServices;

/// <summary>
/// Frontend service-kontrakt for Points.
/// 
/// Ansvar:
/// - Give UI et simpelt API til at hente/tilføje/slette point
/// - Skjule HTTP-detaljer (endpoints, serialization) fra UI-laget
/// </summary>
public interface IPointService
{
    /// <summary>
    /// Henter alle point entries.
    /// Bruges til at beregne totaler i UI (WhistSchemePage).
    /// </summary>
    Task<List<PointEntry>> GetAll();

    /// <summary>
    /// Tilføjer en ny point entry.
    /// </summary>
    Task Add(PointEntry point);

    /// <summary>
    /// Sletter en specifik point entry på id.
    /// </summary>
    Task Delete(int id);

    /// <summary>
    /// Sletter alle point entries.
    /// Bruges når point konverteres til bøder.
    /// </summary>
    Task DeleteAll();
}