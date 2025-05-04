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
    public UserController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    [HttpGet]
    // Returnerer en liste med brugere hentet fra Userrepository
    public User[] GetAll()
    {
        return _userRepository.GetAll();
    }
}
