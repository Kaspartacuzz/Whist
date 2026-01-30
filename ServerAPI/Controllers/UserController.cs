using Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerAPI.Repositories;

namespace ServerAPI.Controllers;

/// <summary>
/// API-controller for brugere.
/// 
/// Ansvar:
/// - Eksponere endpoints til UI'et
/// - Holde controller "tynd": input-validering + kald til repository
/// 
/// Bemærk:
/// - Billed-sletning ligger her, fordi den bruger WebRoot + Request.Host.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _repo;
    private readonly IWebHostEnvironment _env;

    public UserController(IUserRepository repo, IWebHostEnvironment env)
    {
        _repo = repo;
        _env = env;
    }

    // =========================
    // READ
    // =========================

    /// <summary>
    /// Henter alle brugere.
    /// </summary>
    [HttpGet]
    public ActionResult<User[]> GetAll()
    {
        return Ok(_repo.GetAll());
    }

    /// <summary>
    /// Henter en bruger på id.
    /// </summary>
    [HttpGet("{id:int}")]
    public ActionResult<User?> GetById(int id)
    {
        var user = _repo.GetById(id);
        return user is null ? NotFound() : Ok(user);
    }

    // =========================
    // WRITE
    // =========================

    /// <summary>
    /// Opretter en ny bruger.
    /// </summary>
    [HttpPost]
    [Authorize]
    public IActionResult Add([FromBody] User user)
    {
        _repo.AddUser(user);
        return Ok();
    }

    /// <summary>
    /// Sletter en bruger og forsøger samtidig at slette profilbilledet fra wwwroot,
    /// hvis brugeren har ImageUrl sat.
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize]
    public IActionResult Delete(int id)
    {
        var user = _repo.GetById(id);

        // Best-effort: slet profilbillede hvis det findes
        if (user is not null && !string.IsNullOrWhiteSpace(user.ImageUrl))
        {
            TryDeleteProfileImage(user.ImageUrl);
        }

        _repo.Delete(id);
        return Ok();
    }

    /// <summary>
    /// Opdaterer en bruger.
    /// Returnerer 400 hvis id i URL ikke matcher body.Id.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] User updatedUser)
    {
        if (id != updatedUser.Id)
            return BadRequest("ID i URL og body matcher ikke.");

        var existingUser = _repo.GetById(id);
        if (existingUser is null)
            return NotFound("Bruger ikke fundet.");

        await _repo.UpdateUser(updatedUser);
        return NoContent();
    }

    // =========================
    // Helpers
    // =========================

    /// <summary>
    /// Forsøger at slette billedefilen ud fra ImageUrl.
    /// Metoden er "best effort": fejl må ikke stoppe sletning af brugeren.
    /// </summary>
    private void TryDeleteProfileImage(string imageUrl)
    {
        try
        {
            // imageUrl er typisk en fuld URL: https://host/.../uploads/xyz.jpg
            // Vi konverterer til relativ path, så vi kan finde filen i wwwroot.
            var relativePath = imageUrl.Replace($"{Request.Scheme}://{Request.Host}", "");
            var fullPath = Path.Combine(_env.WebRootPath, relativePath.TrimStart('/'));

            if (System.IO.File.Exists(fullPath))
                System.IO.File.Delete(fullPath);
        }
        catch (Exception ex)
        {
            // Best-effort: log til console (kan senere skiftes til ILogger)
            Console.WriteLine($"⚠️ Kunne ikke slette profilbillede: {ex.Message}");
        }
    }
}
