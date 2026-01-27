using System.Net.Http.Json;
using Core;

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

    private const string BaseRoute = "api/user";

    public UserService(HttpClient http)
    {
        _http = http;
    }

    public async Task<User[]> GetAll()
        => await _http.GetFromJsonAsync<User[]>(BaseRoute) ?? Array.Empty<User>();

    public async Task<User?> GetById(int id)
        => await _http.GetFromJsonAsync<User?>($"{BaseRoute}/{id}");

    public async Task AddUser(User user)
        => await _http.PostAsJsonAsync(BaseRoute, user);

    public async Task Delete(int id)
        => await _http.DeleteAsync($"{BaseRoute}/{id}");

    public async Task Update(User user)
        => await _http.PutAsJsonAsync($"{BaseRoute}/{user.Id}", user);
}