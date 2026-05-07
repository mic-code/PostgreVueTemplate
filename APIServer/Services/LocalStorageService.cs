using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.IO;
using Utility;

namespace Services;

public class LocalStorageService : IStorageService
{
    private readonly string _basePath;
    private readonly string _baseUrl;
    private readonly ILogger<LocalStorageService> _logger;
    
    private static string EnvPrefix => GlobalVar.IsDev ? "dev" : "prod";

    public LocalStorageService(IConfiguration config, IHostEnvironment env, ILogger<LocalStorageService> logger)
    {
        _logger = logger;
        
        // Get storage path from config or use default
        var storagePath = config["Storage:Local:Path"] ?? "uploads";
        
        // If relative path, make it relative to content root
        if (!Path.IsPathRooted(storagePath))
        {
            _basePath = Path.Combine(env.ContentRootPath, "wwwroot", storagePath);
        }
        else
        {
            _basePath = storagePath;
        }
        
        _baseUrl = config["Storage:Local:BaseUrl"] ?? "/uploads";
        
        // Ensure directory exists
        if (!Directory.Exists(_basePath))
        {
            Directory.CreateDirectory(_basePath);
            _logger.LogInformation("Created storage directory: {Path}", _basePath);
        }
    }

    public async Task<StorageResult> UploadAsync(IFormFile file, string folder, string? customName = null)
    {
        using var stream = file.OpenReadStream();
        return await UploadAsync(stream, file.FileName, file.ContentType, folder, customName);
    }

    public async Task<StorageResult> UploadAsync(Stream stream, string fileName, string contentType, string folder, string? customName = null)
    {
        try
        {
            // Create folder if needed
            var folderPath = Path.Combine(_basePath, EnvPrefix, folder);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Generate unique filename
            var extension = Path.GetExtension(fileName);
            var storedName = customName ?? $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(folderPath, storedName);

            // Save file
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await stream.CopyToAsync(fileStream);
            }

            _logger.LogInformation("File uploaded: {Path}", filePath);

            return new StorageResult
            {
                Success = true,
                StoredName = storedName,
                Url = GetPublicUrl(storedName, folder),
                StorageType = "local",
                StoragePath = Path.Combine(EnvPrefix, folder, storedName)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload file: {FileName}", fileName);
            return new StorageResult
            {
                Success = false,
                Error = ex.Message
            };
        }
    }

    public Task<bool> DeleteAsync(string storedName, string folder)
    {
        try
        {
            var filePath = Path.Combine(_basePath, EnvPrefix, folder, storedName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                _logger.LogInformation("File deleted: {Path}", filePath);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete file: {StoredName}", storedName);
            return Task.FromResult(false);
        }
    }

    public Task<string?> GetUrlAsync(string storedName, string folder)
    {
        var filePath = Path.Combine(_basePath, EnvPrefix, folder, storedName);
        if (File.Exists(filePath))
        {
            return Task.FromResult<string?>(GetPublicUrl(storedName, folder));
        }
        return Task.FromResult<string?>(null);
    }

    public Task<Stream?> GetAsync(string storedName, string folder)
    {
        try
        {
            var filePath = Path.Combine(_basePath, EnvPrefix, folder, storedName);
            if (File.Exists(filePath))
            {
                var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                return Task.FromResult<Stream?>(stream);
            }
            return Task.FromResult<Stream?>(null);
        }
        catch
        {
            return Task.FromResult<Stream?>(null);
        }
    }

    public string GetPublicUrl(string storedName, string folder)
    {
        return $"{_baseUrl}/{EnvPrefix}/{folder}/{storedName}";
    }
}
