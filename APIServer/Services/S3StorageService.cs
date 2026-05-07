using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using System.IO;
using Utility;

namespace Services;

public class S3StorageService : IStorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;
    private readonly string _baseUrl;
    private readonly ILogger<S3StorageService> _logger;
    
    private static string EnvPrefix => GlobalVar.IsDev ? "dev" : "prod";

    public S3StorageService(ILogger<S3StorageService> logger)
    {
        _logger = logger;
        
        var accessKey = GlobalVar.S3AccessKey 
            ?? throw new InvalidOperationException("S3 AccessKey not configured");
        var secretKey = GlobalVar.S3SecretKey 
            ?? throw new InvalidOperationException("S3 SecretKey not configured");
        _bucketName = GlobalVar.S3BucketName 
            ?? throw new InvalidOperationException("S3 BucketName not configured");
        
        var endpoint = GlobalVar.S3EndpointString; // For S3-compatible services like MinIO
        
        // Use endpoint as base URL for S3-compatible services
        _baseUrl = !string.IsNullOrEmpty(endpoint) 
            ? endpoint.TrimEnd('/') 
            : $"https://{_bucketName}.s3.amazonaws.com";
        
        var s3Config = new AmazonS3Config
        {
            RegionEndpoint = Amazon.RegionEndpoint.USEast1,
            ForcePathStyle = !string.IsNullOrEmpty(endpoint)
        };
        
        if (!string.IsNullOrEmpty(endpoint))
        {
            s3Config.ServiceURL = endpoint;
        }
        
        if (GlobalVar.S3Accelerate)
        {
            s3Config.UseAccelerateEndpoint = true;
        }
        
        _s3Client = new AmazonS3Client(accessKey, secretKey, s3Config);
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
            // Generate unique filename
            var extension = Path.GetExtension(fileName);
            var storedName = customName ?? $"{Guid.NewGuid()}{extension}";
            var key = $"{EnvPrefix}/{folder}/{storedName}";

            var request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = key,
                InputStream = stream,
                ContentType = contentType,
                CannedACL = S3CannedACL.PublicRead
            };

            await _s3Client.PutObjectAsync(request);

            _logger.LogInformation("File uploaded to S3: {Key}", key);

            return new StorageResult
            {
                Success = true,
                StoredName = storedName,
                Url = GetPublicUrl(storedName, folder),
                StorageType = "s3",
                StoragePath = key
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload file to S3: {FileName}", fileName);
            return new StorageResult
            {
                Success = false,
                Error = ex.Message
            };
        }
    }

    public async Task<bool> DeleteAsync(string storedName, string folder)
    {
        try
        {
            var key = $"{EnvPrefix}/{folder}/{storedName}";
            await _s3Client.DeleteObjectAsync(_bucketName, key);
            _logger.LogInformation("File deleted from S3: {Key}", key);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete file from S3: {StoredName}", storedName);
            return false;
        }
    }

    public async Task<string?> GetUrlAsync(string storedName, string folder)
    {
        try
        {
            var key = $"{EnvPrefix}/{folder}/{storedName}";
            var metadata = await _s3Client.GetObjectMetadataAsync(_bucketName, key);
            return GetPublicUrl(storedName, folder);
        }
        catch
        {
            return null;
        }
    }

    public async Task<Stream?> GetAsync(string storedName, string folder)
    {
        try
        {
            var key = $"{EnvPrefix}/{folder}/{storedName}";
            var response = await _s3Client.GetObjectAsync(_bucketName, key);
            return response.ResponseStream;
        }
        catch
        {
            return null;
        }
    }

    public string GetPublicUrl(string storedName, string folder)
    {
        // For S3-compatible services, use path-style URL with bucket name
        if (!string.IsNullOrEmpty(GlobalVar.S3EndpointString))
        {
            return $"{_baseUrl}/{_bucketName}/{EnvPrefix}/{folder}/{storedName}";
        }
        return $"{_baseUrl}/{EnvPrefix}/{folder}/{storedName}";
    }
}
