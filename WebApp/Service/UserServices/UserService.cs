using System.Net.Http.Json;
using Core;
using WebApp.Service.AuthServices;

namespace WebApp.Service;

/// <summary>
/// HTTP-baseret UserService.
/// 
/// Bemærk:
/// - Endpoints matcher UserController i backend.
/// - Vi ændrer ikke funktionaliteten (samme routes og payloads).
/// </summary>
public class UserService : IUserService
{
    private readonly HttpClient _http;
    private readonly IAuthService _auth;

    private const string BaseRoute = "api/user";

    public UserService(HttpClient http, IAuthService auth)
    {
        _http = http;
        _auth = auth;
    }

    public async Task<User[]> GetAll() => await _http.GetFromJsonAsync<User[]>(BaseRoute) ?? Array.Empty<User>();
    

    public async Task<User?> GetById(int id) => await _http.GetFromJsonAsync<User?>($"{BaseRoute}/{id}");
    

    public async Task AddUser(User user)
    {
        await AddDevKeyHeaderIfLoggedIn();
        var res = await _http.PostAsJsonAsync(BaseRoute, user);
        res.EnsureSuccessStatusCode();
    }

    public async Task Delete(int id)
    {
        await AddDevKeyHeaderIfLoggedIn();
        var res = await _http.DeleteAsync($"{BaseRoute}/{id}");
        res.EnsureSuccessStatusCode();
    }

    public async Task Update(User user)
    {
        await AddDevKeyHeaderIfLoggedIn();
        var res = await _http.PutAsJsonAsync($"{BaseRoute}/{user.Id}", user);
        res.EnsureSuccessStatusCode();
    }
    
    // Helper til authentication.
    private async Task AddDevKeyHeaderIfLoggedIn()
    {
        var user = await _auth.GetCurrentUser();
        _http.DefaultRequestHeaders.Remove("X-Whist-Key");

        if (user is not null)
            _http.DefaultRequestHeaders.Add("X-Whist-Key", "whist-dev-key");
    }
}