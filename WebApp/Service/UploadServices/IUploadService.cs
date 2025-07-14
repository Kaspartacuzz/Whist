using Microsoft.AspNetCore.Components.Forms;

namespace WebApp.Service.UploadServices;

public interface IUploadService
{
    Task<string?> UploadImageAsync(IBrowserFile file);
}