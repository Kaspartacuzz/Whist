using Microsoft.AspNetCore.Mvc;

namespace ServerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UploadController : ControllerBase
{
    private readonly IWebHostEnvironment _env;

    public UploadController(IWebHostEnvironment env)
    {
        _env = env;
    }

    [HttpPost("image")]
    public async Task<IActionResult> UploadImage([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Intet billede modtaget.");

        // ✅ Begræns filtyper
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(ext))
            return BadRequest("Filtypen er ikke tilladt. Kun jpg, jpeg, png og webp er tilladt.");

        // ✅ Begræns størrelse til f.eks. 5 MB
        const long maxFileSize = 5 * 1024 * 1024;
        if (file.Length > maxFileSize)
            return BadRequest("Filen er for stor. Maks 5 MB tilladt.");
        
        var fileName = $"{Guid.NewGuid()}{ext}";
        var folderName = DateTime.UtcNow.ToString("yyyy.MM.dd");
        var uploadPath = Path.Combine(_env.WebRootPath, "uploads", folderName);

        if (!Directory.Exists(uploadPath))
            Directory.CreateDirectory(uploadPath);

        var filePath = Path.Combine(uploadPath, fileName);
        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        var fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{folderName}/{fileName}";
        return Ok(new { Url = fileUrl });
    }
}