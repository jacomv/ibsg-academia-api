namespace Academia.Application.Common.Interfaces;

public interface IFileStorageService
{
    /// <summary>Saves an uploaded file and returns its public URL path.</summary>
    Task<string> SaveAsync(Stream fileStream, string fileName, string contentType, CancellationToken ct = default);
    Task DeleteAsync(string filePath, CancellationToken ct = default);
}
