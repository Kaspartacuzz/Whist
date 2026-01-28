using Microsoft.AspNetCore.Components.Forms;

namespace WebApp.Service.UploadServices;

/// <summary>
/// Frontend service-kontrakt til upload af billeder.
/// 
/// Formål:
/// - Skjule multipart/form-data detaljer fra components/pages
/// - Returnere en offentlig URL (som backend genererer) til det uploadede billede
/// </summary>
public interface IUploadService
{
    /// <summary>
    /// Uploader et billede til backend og returnerer en URL (eller null ved fejl).
    /// </summary>
    Task<string?> UploadImageAsync(IBrowserFile file);
}