using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms;
using WebApp.Service.AuthServices;

namespace WebApp.Service.UploadServices;

/// <summary>
/// HTTP-baseret upload-service.
/// 
/// Sender multipart/form-data til: POST api/upload/image
/// Backend gemmer filen i wwwroot/uploads/... og returnerer JSON:
/// { "url": "https://host/uploads/..." }
/// </summary>
public class UploadService : IUploadService
{
    private const string UploadEndpoint = "api/upload/image";

    // Match serverens max (UploadController har 5 MB limit)
    private const long MaxAllowedSizeBytes = 5 * 1024 * 1024;

    private readonly HttpClient _http;
    private readonly IAuthService _auth;

    public UploadService(HttpClient http, IAuthService auth)
    {
        _http = http;
        _auth = auth;
    }

    public async Task<string?> UploadImageAsync(IBrowserFile file)
    {
        // Hurtig sanity-check
        if (file is null || file.Size == 0)
            return null;
        
        await AddDevKeyHeaderIfLoggedIn();

        // Multipart content skal disponeres korrekt
        await using var stream = file.OpenReadStream(maxAllowedSize: MaxAllowedSizeBytes);

        using var content = new MultipartFormDataContent();

        using var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

        // "file" skal matche backend [FromForm] IFormFile file
        content.Add(fileContent, "file", file.Name);

        var response = await _http.PostAsync(UploadEndpoint, content);

        if (!response.IsSuccessStatusCode)
            return null;
        
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<UploadResult>();
        return string.IsNullOrWhiteSpace(result?.Url) ? null : result.Url;
    }

    /// <summary>
    /// DTO til at læse serverens response.
    /// </summary>
    private sealed class UploadResult
    {
        public string Url { get; set; } = "";
    }
    
    // Helper til Authentication
    private async Task AddDevKeyHeaderIfLoggedIn()
    {
        var user = await _auth.GetCurrentUser();
        _http.DefaultRequestHeaders.Remove("X-Whist-Key");

        if (user is not null)
            _http.DefaultRequestHeaders.Add("X-Whist-Key", "whist-dev-key");
    }

}