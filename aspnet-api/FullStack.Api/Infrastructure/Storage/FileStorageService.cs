using FullStack.Api.Application.Interfaces;

namespace FullStack.Api.Infrastructure.Storage;

public class FileStorageService : IFileStorageService
{
    private static readonly string[] AllowedExtensions = [".png", ".jpg", ".jpeg"];
    private static readonly string[] AllowedContentTypes = ["image/png", "image/jpg", "image/jpeg"];

    private readonly string _root;
    private readonly long _maxBytes;
    private readonly ILogger<FileStorageService> _logger;

    public FileStorageService(IConfiguration config, IWebHostEnvironment env, ILogger<FileStorageService> logger)
    {
        var configured = config["UploadPath"];
        if (string.IsNullOrWhiteSpace(configured)) configured = "wwwroot/uploads";

        _root = Path.IsPathRooted(configured)
            ? configured
            : Path.GetFullPath(Path.Combine(env.ContentRootPath, configured));
        _maxBytes = config.GetValue("UploadMaxBytes", 3L * 1024 * 1024);
        _logger = logger;
    }

    public async Task<string> SaveImageAsync(IFormFile file, CancellationToken ct = default)
    {
        if (file.Length == 0) throw new InvalidUploadException("Image file is empty.");
        if (file.Length > _maxBytes)
            throw new InvalidUploadException($"Image is too large (max {_maxBytes / 1024 / 1024} MB).");

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(ext)
            || !AllowedContentTypes.Contains(file.ContentType, StringComparer.OrdinalIgnoreCase))
            throw new InvalidUploadException("Only png, jpg and jpeg images are allowed.");

        Directory.CreateDirectory(_root);

        var name = $"image-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}-{Random.Shared.Next(100_000_000, 999_999_999)}{ext}";
        var path = Path.Combine(_root, name);

        await using var stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write);
        await file.CopyToAsync(stream, ct);
        return name;
    }

    public void Delete(string? fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName)) return;

        try
        {
            // GetFileName strips any directory part => no path traversal outside the upload folder.
            var path = Path.Combine(_root, Path.GetFileName(fileName));
            if (File.Exists(path)) File.Delete(path);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not delete uploaded file {File}", fileName);
        }
    }
}
