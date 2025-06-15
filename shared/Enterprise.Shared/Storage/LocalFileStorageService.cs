using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.Storage;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _basePath;
    private readonly ILogger<LocalFileStorageService> _logger;
    private readonly HashSet<string> _allowedImageTypes = new() { "image/png", "image/jpeg", "image/jpg" };

    public LocalFileStorageService(IConfiguration configuration, ILogger<LocalFileStorageService> logger)
    {
        _logger = logger;
        
        // default to wwwroot/uploads
        var configuredPath = configuration["FileStorage:LocalPath"];
        if (string.IsNullOrWhiteSpace(configuredPath))
        {
            _basePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        }
        else
        {
            _basePath = System.IO.Path.IsPathRooted(configuredPath)
                ? configuredPath
                : System.IO.Path.Combine(Directory.GetCurrentDirectory(), configuredPath);
        }

        Directory.CreateDirectory(_basePath);
        _logger.LogInformation("LocalFileStorageService initialized with base path: {BasePath}", _basePath);
    }

    public async Task<string> SaveFileAsync(byte[] fileContent, string fileName, string contentType, string? subDirectory = null)
    {
        ValidateFileType(contentType);
        
        var relativePath = GenerateFilePath(fileName, subDirectory);
        var fullPath = System.IO.Path.Combine(_basePath, relativePath);
        
        var directory = System.IO.Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await File.WriteAllBytesAsync(fullPath, fileContent);
        _logger.LogInformation("File saved successfully: {FilePath}", relativePath);
        
        return relativePath;
    }

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string contentType, string? subDirectory = null)
    {
        ValidateFileType(contentType);
        
        var relativePath = GenerateFilePath(fileName, subDirectory);
        var fullPath = System.IO.Path.Combine(_basePath, relativePath);
        
        var directory = System.IO.Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using var fileStreamOutput = File.Create(fullPath);
        await fileStream.CopyToAsync(fileStreamOutput);
        _logger.LogInformation("File saved successfully: {FilePath}", relativePath);
        
        return relativePath;
    }

    public Task DeleteFileAsync(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return Task.CompletedTask;
        }

        var fullPath = System.IO.Path.Combine(_basePath, filePath);
        
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
            _logger.LogInformation("File deleted successfully: {FilePath}", filePath);
        }
        else
        {
            _logger.LogWarning("Attempted to delete non-existent file: {FilePath}", filePath);
        }

        return Task.CompletedTask;
    }

    public string GetFileUrl(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return string.Empty;
        }

        if (filePath.StartsWith("floor-plans/", StringComparison.OrdinalIgnoreCase))
        {
            var relativePath = filePath.Replace('\\', '/');
            
            if (relativePath.Contains("/thumbnails/"))
            {
                var thumbnailPath = relativePath.Replace("floor-plans/thumbnails/", "");
                return $"/api/floor-plans/thumbnails/{thumbnailPath}";
            }
            else
            {
                var imagePath = relativePath.Replace("floor-plans/", "");
                return $"/api/floor-plans/images/{imagePath}";
            }
        }
        
        return $"/uploads/{filePath.Replace('\\', '/')}";
    }

    public Task<bool> FileExistsAsync(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return Task.FromResult(false);
        }

        var fullPath = System.IO.Path.Combine(_basePath, filePath);
        return Task.FromResult(File.Exists(fullPath));
    }

    public async Task<byte[]> GetFileAsync(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("File path cannot be empty", nameof(filePath));
        }

        var fullPath = System.IO.Path.Combine(_basePath, filePath);
        
        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"File not found: {filePath}");
        }

        return await File.ReadAllBytesAsync(fullPath);
    }

    public Task<Stream> GetFileStreamAsync(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("File path cannot be empty", nameof(filePath));
        }

        var fullPath = System.IO.Path.Combine(_basePath, filePath);
        
        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"File not found: {filePath}");
        }

        return Task.FromResult<Stream>(File.OpenRead(fullPath));
    }

    private void ValidateFileType(string contentType)
    {
        if (!_allowedImageTypes.Contains(contentType.ToLowerInvariant()))
        {
            throw new InvalidOperationException($"File type '{contentType}' is not allowed. Only PNG and JPG/JPEG images are supported.");
        }
    }

    private string GenerateFilePath(string fileName, string? subDirectory)
    {
        var fileExtension = System.IO.Path.GetExtension(fileName);
        var fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(fileName);
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var uniqueFileName = $"{fileNameWithoutExtension}_{timestamp}_{Guid.NewGuid():N}{fileExtension}";

        if (!string.IsNullOrWhiteSpace(subDirectory))
        {
            return System.IO.Path.Combine(subDirectory, uniqueFileName);
        }

        return uniqueFileName;
    }
}