using System.Net.Http.Json;
using Blazored.LocalStorage;
using Core;

namespace WebApp.Service.AuthServices;

/// <summary>
/// Simpel auth-service til frontend.
/// 
/// Nuværende adfærd (bevidst bevaret):
/// - Henter alle users fra API og matcher email+password i browseren.
/// - Gemmer den fundne bruger i localStorage som "currentUser".
/// 
/// OBS: Sikkerhed kan strammes senere (server-side auth + hashing + tokens).
/// </summary>
public class AuthService : IAuthService
{
    private const string CurrentUserStorageKey = "currentUser";

    private readonly HttpClient _http;
    private readonly ILocalStorageService _localStorage;

    public AuthService(HttpClient http, ILocalStorageService localStorage)
    {
        _http = http;
        _localStorage = localStorage;
    }

    public async Task<bool> Login(string email, string password)
    {
        // Bevar logik: hent alle brugere og find match
        var users = await _http.GetFromJsonAsync<User[]>("api/user") ?? Array.Empty<User>();

        var user = users.FirstOrDefault(u =>
            string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase) &&
            u.Password == password);

        if (user is null)
            return false;

        await _localStorage.SetItemAsync(CurrentUserStorageKey, user);
        return true;
    }

    public async Task Logout()
    {
        await _localStorage.RemoveItemAsync(CurrentUserStorageKey);
    }

    public async Task<User?> GetCurrentUser()
    {
        return await _localStorage.GetItemAsync<User>(CurrentUserStorageKey);
    }
}