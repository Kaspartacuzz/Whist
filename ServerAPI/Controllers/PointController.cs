using Core;
using Microsoft.AspNetCore.Mvc;
using ServerAPI.Repositories.Points;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PointController : ControllerBase
{
    private readonly IPointRepository _repository;

    public PointController(IPointRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<List<PointEntry>>> GetAll()
    {
        var points = await _repository.GetAll();
        return Ok(points);
    }

    [HttpPost]
    public async Task<ActionResult> Add(PointEntry point)
    {
        await _repository.Add(point);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _repository.Delete(id);
        return Ok();
    }

    [HttpDelete("all")]
    public async Task<ActionResult> DeleteAll()
    {
        await _repository.DeleteAll();
        return Ok();
    }
}