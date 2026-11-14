namespace FullStack.Application.Interfaces;

public interface IFileStorageService
{
    /// <summary>Validates and saves an image under the configured UploadPath; returns the stored file name.</summary>
    Task<string> SaveImageAsync(IFormFile file, CancellationToken ct = default);

    /// <summary>Deletes a previously stored file by name. Never throws for missing files.</summary>
    void Delete(string? fileName);
}

public class InvalidUploadException : Exception
{
    public InvalidUploadException(string message) : base(message) { }
}
