using Core;
using Microsoft.AspNetCore.Mvc;
using ServerAPI.Repositories.Fines;

namespace ServerAPI.Controllers;

/// <summary>
/// API for bøder.
/// Controlleren er "tynd" og videresender logik til repository.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class FineController : ControllerBase
{
    private readonly IFineRepository _fineRepository;

    /// <summary>
    /// Repository injiceres via DI.
    /// </summary>
    public FineController(IFineRepository fineRepository)
    {
        _fineRepository = fineRepository;
    }

    // =========================
    // READ
    // =========================

    /// <summary>
    /// Henter alle bøder (på tværs af brugere).
    /// Bruges primært til overblik/summary i UI.
    /// </summary>
    [HttpGet]
    public ActionResult<Fine[]> GetAll()
    {
        return _fineRepository.GetAll();
    }

    /// <summary>
    /// Henter alle bøder for en bestemt bruger.
    /// </summary>
    [HttpGet("user/{userId}")]
    public ActionResult<Fine[]> GetByUserId(int userId)
    {
        return _fineRepository.GetByUserId(userId);
    }

    /// <summary>
    /// Henter bøder pagineret (server-side paging).
    /// Fordel: UI kan vise tabel uden at hente alt.
    /// </summary>
    [HttpGet("paged")]
    public ActionResult<PagedResult<Fine>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? userId = null)
    {
        // NOTE: Dette er allerede i din kode – jeg bevarer det uændret.
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var result = _fineRepository.GetPaged(page, pageSize, userId);
        return Ok(result);
    }

    // =========================
    // WRITE
    // =========================

    /// <summary>
    /// Opretter en ny bøde.
    /// </summary>
    [HttpPost]
    public IActionResult Add(Fine fine)
    {
        _fineRepository.AddFine(fine);
        return Ok();
    }

    /// <summary>
    /// Opdaterer en eksisterende bøde (fx IsPaid/kommentar/beløb).
    /// </summary>
    [HttpPut]
    public IActionResult Update(Fine fine)
    {
        _fineRepository.Update(fine);
        return Ok();
    }

    /// <summary>
    /// Sletter en bøde for en given bruger.
    /// </summary>
    [HttpDelete("user/{userId}/{id}")]
    public IActionResult Delete(int userId, int id)
    {
        _fineRepository.Delete(userId, id);
        return Ok();
    }
}
