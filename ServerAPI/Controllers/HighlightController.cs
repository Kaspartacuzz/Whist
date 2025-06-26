using Core;
using Microsoft.AspNetCore.Mvc;
using ServerAPI.Repositories.Highlights;

namespace ServerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HighlightController : ControllerBase
{
    private readonly IHighlightRepository _repository;

    public HighlightController(IHighlightRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Highlight>>> GetAll()
    {
        var highlights = await _repository.GetAll();
        return Ok(highlights);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Highlight>> GetById(int id)
    {
        var highlight = await _repository.GetById(id);
        if (highlight == null)
            return NotFound();
        return Ok(highlight);
    }

    [HttpPost]
    public async Task<ActionResult<Highlight>> Create(Highlight highlight)
    {
        var created = await _repository.Add(highlight);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _repository.Delete(id);
        return NoContent();
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Highlight highlight)
    {
        if (id != highlight.Id)
        {
            return BadRequest("Id i URL matcher ikke id i objektet.");
        }

        await _repository.Update(highlight);
        return NoContent();
    }
}