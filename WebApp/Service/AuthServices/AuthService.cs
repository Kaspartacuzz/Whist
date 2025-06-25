using Blazored.LocalStorage;
using Core;
using System.Net.Http.Json;

namespace WebApp.Service.AuthServices;

public class AuthService : IAuthService
{
    private readonly HttpClient _http;
    private readonly ILocalStorageService _localStorage;

    public AuthService(HttpClient http, ILocalStorageService localStorage)
    {
        _http = http;
        _localStorage = localStorage;
    }

    public async Task<bool> Login(string email, string password)
    {
        var users = await _http.GetFromJsonAsync<User[]>("api/user");
        var user = users?.FirstOrDefault(u => u.Email == email && u.Password == password);

        if (user != null)
        {
            await _localStorage.SetItemAsync("currentUser", user);
            return true;
        }

        return false;
    }

    public async Task Logout()
    {
        await _localStorage.RemoveItemAsync("currentUser");
    }

    public async Task<User?> GetCurrentUser()
    {
        return await _localStorage.GetItemAsync<User>("currentUser");
    }
}