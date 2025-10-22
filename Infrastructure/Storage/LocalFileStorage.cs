using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Storage;

public class LocalFileStorage : IFileStorage
{
    private readonly string _storagePath;

    public LocalFileStorage(IConfiguration configuration)
    {
        _storagePath = configuration["FileStorage:Path"] ?? "uploads";
        
        if (!Directory.Exists(_storagePath))
        {
            Directory.CreateDirectory(_storagePath);
        }
    }

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default)
    {
        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
        var filePath = Path.Combine(_storagePath, uniqueFileName);

        using var fileStreamOutput = File.Create(filePath);
        await fileStream.CopyToAsync(fileStreamOutput, cancellationToken);

        return filePath;
    }

    public Task DeleteFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return Task.CompletedTask;
    }
}
