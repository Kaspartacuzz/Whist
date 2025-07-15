using Core;
using Microsoft.AspNetCore.Mvc;
using ServerAPI.Repositories.Calendars;

[ApiController]
[Route("api/[controller]")]
public class CalendarController : ControllerBase
{
    private readonly ICalendarRepository _repo;

    public CalendarController(ICalendarRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public async Task<ActionResult<List<Calendar>>> GetAll()
    {
        var items = await _repo.GetAll();
        return Ok(items);
    }

    [HttpPost]
    public async Task<IActionResult> Save(Calendar calendar)
    {
        await _repo.AddOrUpdate(calendar);
        return Ok();
    }

    [HttpDelete("{date}")]
    public async Task<IActionResult> Delete(string date)
    {
        if (DateTime.TryParse(date, out var parsedDate))
        {
            await _repo.DeleteByDate(parsedDate);
            return Ok();
        }
        return BadRequest("Ugyldigt datoformat.");
    }
}