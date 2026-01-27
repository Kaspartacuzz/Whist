using Core;

namespace WebApp.Service.AuthServices;

/// <summary>
/// Kontrakt for simpel auth i frontend.
/// 
/// OBS:
/// - Dette er et "lightweight" login (for jeres 4-bruger setup).
/// - Rigtig auth/rolle-håndtering kan laves senere ifm. Azure deployment.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Logger ind ved at validere email + password (nu: på klienten).
    /// Returnerer true hvis login lykkes.
    /// </summary>
    Task<bool> Login(string email, string password);

    /// <summary>
    /// Logger ud (fjerner currentUser fra localStorage).
    /// </summary>
    Task Logout();

    /// <summary>
    /// Returnerer den bruger der er logget ind (fra localStorage), eller null.
    /// </summary>
    Task<User?> GetCurrentUser();
}