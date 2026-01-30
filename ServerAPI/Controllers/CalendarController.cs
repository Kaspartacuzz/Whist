using Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerAPI.Repositories.Calendars;

namespace WebAPI.Controllers;

/// <summary>
/// API-controller for kalender-events.
/// Controlleren er bevidst "tynd":
/// - Ingen mail-logik her (det ligger i MailReminderWorker)
/// - Ingen database-specifik logik her (det ligger i repository)
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CalendarController : ControllerBase
{
    private readonly ICalendarRepository _repo;

    public CalendarController(ICalendarRepository repo)
    {
        _repo = repo;
    }

    /// <summary>Henter alle kalender-events.</summary>
    [HttpGet]
    public async Task<ActionResult<List<Calendar>>> GetAll()
    {
        var items = await _repo.GetAll();
        return Ok(items);
    }

    /// <summary>
    /// Gemmer et event (opret/ret).
    /// Repository håndterer selv om det er add/update.
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Save(Calendar calendar)
    {
        await _repo.AddOrUpdate(calendar);
        return Ok();
    }

    /// <summary>Sletter et event ud fra id.</summary>
    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        await _repo.Delete(id);
        return Ok();
    }
}