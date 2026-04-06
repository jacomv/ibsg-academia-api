using Academia.Application.Common.Interfaces;
using Microsoft.Extensions.Hosting;

namespace Academia.Infrastructure.Storage;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _uploadsPath;
    private const string UrlPrefix = "/uploads";

    private static readonly HashSet<string> AllowedExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp", ".gif", ".pdf" };

    public LocalFileStorageService(IHostEnvironment environment)
    {
        _uploadsPath = Path.Combine(environment.ContentRootPath, "wwwroot", "uploads");
        Directory.CreateDirectory(_uploadsPath);
    }

    public async Task<string> SaveAsync(
        Stream fileStream, string fileName, string contentType, CancellationToken ct = default)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();

        if (!AllowedExtensions.Contains(extension))
            throw new InvalidOperationException($"File type '{extension}' is not allowed.");

        var uniqueName = $"{Guid.NewGuid()}{extension}";
        var fullPath = Path.Combine(_uploadsPath, uniqueName);

        await using var stream = new FileStream(fullPath, FileMode.Create);
        await fileStream.CopyToAsync(stream, ct);

        return $"{UrlPrefix}/{uniqueName}";
    }

    public Task DeleteAsync(string filePath, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(filePath)) return Task.CompletedTask;

        var fileName = Path.GetFileName(filePath);
        var fullPath = Path.Combine(_uploadsPath, fileName);

        if (File.Exists(fullPath))
            File.Delete(fullPath);

        return Task.CompletedTask;
    }
}
