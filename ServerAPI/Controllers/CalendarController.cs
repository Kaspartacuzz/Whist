using Core;
using Microsoft.AspNetCore.Mvc;
using ServerAPI.Repositories.Calendars;
using System.Net;
using System.Net.Mail;
using ServerAPI.Repositories;

[ApiController]
[Route("api/[controller]")]
public class CalendarController : ControllerBase
{
    private readonly ICalendarRepository _repo;
    private readonly IUserRepository _users;
    private readonly IConfiguration _config;

    public CalendarController(ICalendarRepository repo, IUserRepository users, IConfiguration config)
    {
        _repo = repo;
        _users = users;
        _config = config;
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

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _repo.Delete(id);
        return Ok();
    }
}