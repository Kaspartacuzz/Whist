using Core;
using Microsoft.AspNetCore.Mvc;
using ServerAPI.Repositories;

namespace ServerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    // Dependency Injection: vi får et IUserRepository-objekt udefra
    // og gemmer det i en privat variabel, så vi kan bruge det i controllerens metoder
    private readonly IUserRepository _userRepository;
    private readonly IWebHostEnvironment _env;

    public UserController(IUserRepository userRepository, IWebHostEnvironment env)
    {
        _userRepository = userRepository;
        _env = env;
    }

    
    [HttpGet]
    // Returnerer en liste med brugere hentet fra Userrepository
    public User[] GetAll()
    {
        return _userRepository.GetAll(); // Kalder Userrepositoryets GetAll-metode
    }
    
    [HttpGet("{id}")]
    // Returnerer én bruger baseret på det angivne ID
    public User? GetById(int id)
    {
        return _userRepository.GetById(id); // Kalder Userrepositoryets GetById-metode
    }
    
    [HttpPost]
    // Tilføjer en ny bruger til listen i UserRepository
    public void Add(User user)
    {
        _userRepository.AddUser(user); // Kalder Userrepositoryets Add-metode
    }
    
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
        var user = _userRepository.GetById(id);
    
        if (user is not null && !string.IsNullOrEmpty(user.ImageUrl))
        {
            try
            {
                var relativePath = user.ImageUrl.Replace($"{Request.Scheme}://{Request.Host}", "");
                var fullPath = Path.Combine(_env.WebRootPath, relativePath.TrimStart('/'));

                if (System.IO.File.Exists(fullPath))
                    System.IO.File.Delete(fullPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Kunne ikke slette profilbillede: {ex.Message}");
            }
        }

        _userRepository.Delete(id);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] User updatedUser)
    {
        if (id != updatedUser.Id)
            return BadRequest("ID i URL og body matcher ikke");

        var existingUser = _userRepository.GetById(id);
        if (existingUser == null)
            return NotFound("Bruger ikke fundet");

        await _userRepository.UpdateUser(updatedUser);
        return NoContent();
    }
}
