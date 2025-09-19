using Core;
using Microsoft.AspNetCore.Mvc;
using ServerAPI.Repositories.Highlights;

namespace ServerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HighlightController : ControllerBase
{
    private readonly IHighlightRepository _repository;
    private readonly IWebHostEnvironment _env;

    public HighlightController(IHighlightRepository repository, IWebHostEnvironment env)
    {
        _repository = repository;
        _env = env;
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
        var highlight = await _repository.GetById(id);

        if (highlight is not null && !string.IsNullOrEmpty(highlight.ImageUrl))
        {
            try
            {
                // Fjern domæne fra URL, fx "http://localhost:5176/uploads/2025.07.14/abc.jpg"
                var relativePath = highlight.ImageUrl.Replace($"{Request.Scheme}://{Request.Host}", "");
                var fullPath = Path.Combine(_env.WebRootPath, relativePath.TrimStart('/'));

                if (System.IO.File.Exists(fullPath))
                    System.IO.File.Delete(fullPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Kunne ikke slette billede: {ex.Message}");
            }
        }

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
    
    // HighlightController.cs
    [HttpGet("paged")]
    public async Task<ActionResult<PagedResult<Highlight>>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 6)
    {
        var result = await _repository.GetPaged(page, pageSize);
        return Ok(result);
    }
}