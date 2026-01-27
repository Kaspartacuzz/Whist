using Core;
using Microsoft.AspNetCore.Mvc;
using ServerAPI.Repositories.Highlights;

namespace ServerAPI.Controllers;

/// <summary>
/// API for highlights.
/// Controlleren er bevidst "tynd" og videresender logik til repository.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HighlightController : ControllerBase
{
    private readonly IHighlightRepository _repository;
    private readonly IWebHostEnvironment _env;

    /// <summary>
    /// Repository + hosting environment injiceres via DI.
    /// _env bruges til at finde wwwroot path, når vi sletter billeder fra disk.
    /// </summary>
    public HighlightController(IHighlightRepository repository, IWebHostEnvironment env)
    {
        _repository = repository;
        _env = env;
    }

    // =========================
    // READ
    // =========================

    /// <summary>
    /// Henter alle highlights.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Highlight>>> GetAll()
    {
        var highlights = await _repository.GetAll();
        return Ok(highlights);
    }

    /// <summary>
    /// Henter et highlight ud fra id.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Highlight>> GetById(int id)
    {
        var highlight = await _repository.GetById(id);
        if (highlight == null)
            return NotFound();

        return Ok(highlight);
    }

    /// <summary>
    /// Henter highlights pagineret (server-side).
    /// Bruges af UI til at vise et grid uden at hente alt.
    /// </summary>
    [HttpGet("paged")]
    public async Task<ActionResult<PagedResult<Highlight>>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 6,
        [FromQuery] string? searchTerm = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] bool includePrivate = true)
    {
        var result = await _repository.GetPaged(page, pageSize, searchTerm, fromDate, toDate, includePrivate);
        return Ok(result);
    }

    // =========================
    // WRITE
    // =========================

    /// <summary>
    /// Opretter et nyt highlight.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Highlight>> Create(Highlight highlight)
    {
        var created = await _repository.Add(highlight);

        // Returnerer 201 Created + lokation til GetById
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Opdaterer et eksisterende highlight.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Highlight highlight)
    {
        // Beskytter mod mismatch mellem url og body.
        if (id != highlight.Id)
            return BadRequest("Id i URL matcher ikke id i objektet.");

        await _repository.Update(highlight);
        return NoContent();
    }

    /// <summary>
    /// Sletter et highlight.
    /// Hvis highlight har et billede (ImageUrl), forsøger vi også at slette filen fra wwwroot.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var highlight = await _repository.GetById(id);

        // Hvis der ligger et billede på disk, prøver vi at fjerne det først.
        // (Failure her må ikke stoppe selve sletningen af highlight i DB.)
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
                // Bevidst "soft fail": vi logger blot til console.
                Console.WriteLine($"⚠️ Kunne ikke slette billede: {ex.Message}");
            }
        }

        await _repository.Delete(id);
        return NoContent();
    }
}
