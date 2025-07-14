using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms;

namespace WebApp.Service.UploadServices;

public class UploadService : IUploadService
{
    private readonly HttpClient _http;

    public UploadService(HttpClient http)
    {
        _http = http;
    }

    public async Task<string?> UploadImageAsync(IBrowserFile file)
    {
        var content = new MultipartFormDataContent();
        var stream = file.OpenReadStream(maxAllowedSize: 10_000_000);
        var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
        content.Add(fileContent, "file", file.Name);

        var response = await _http.PostAsync("api/upload/image", content);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<UploadResult>();
            return result?.Url;
        }

        return null;
    }

    private class UploadResult
    {
        public string Url { get; set; } = "";
    }
}