using Core;

namespace WebApp.Service.RuleServices;

/// <summary>
/// Frontend service-kontrakt for regler.
/// Formål:
/// - Give UI et simpelt API til CRUD på rules
/// - Skjule HTTP-detaljer fra pages/components
/// </summary>
public interface IRuleService
{
    /// <summary>Henter alle regler.</summary>
    Task<List<Rule>> GetAll();

    /// <summary>Tilføjer en ny regel.</summary>
    Task Add(Rule rule);

    /// <summary>Opdaterer en eksisterende regel (samme Id).</summary>
    Task Update(Rule rule);

    /// <summary>Sletter en regel ud fra id.</summary>
    Task Delete(int id);
}