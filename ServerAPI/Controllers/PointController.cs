using Core;
using Microsoft.AspNetCore.Mvc;
using ServerAPI.Repositories.Points;

namespace WebAPI.Controllers;

/// <summary>
/// API-controller for Whist points.
/// Ansvar:
/// - Eksponere simple endpoints til UI'et (WhistSchemePage)
/// - Holde controller "tynd": ingen forretningslogik her
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PointController : ControllerBase
{
    private readonly IPointRepository _repository;

    public PointController(IPointRepository repository)
    {
        _repository = repository;
    }

    // =========================
    // READ
    // =========================

    /// <summary>
    /// Henter alle point entries.
    /// UI bruger dette til at beregne totals pr. spiller.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<PointEntry>>> GetAll()
    {
        var points = await _repository.GetAll();
        return Ok(points);
    }

    // =========================
    // WRITE
    // =========================

    /// <summary>
    /// Tilføjer en ny point entry.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult> Add(PointEntry point)
    {
        await _repository.Add(point);
        return Ok();
    }

    /// <summary>
    /// Sletter en enkelt point entry ud fra id.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _repository.Delete(id);
        return Ok();
    }

    /// <summary>
    /// Sletter alle point entries (nulstil).
    /// Bruges når points konverteres til bøder.
    /// </summary>
    [HttpDelete("all")]
    public async Task<ActionResult> DeleteAll()
    {
        await _repository.DeleteAll();
        return Ok();
    }
}