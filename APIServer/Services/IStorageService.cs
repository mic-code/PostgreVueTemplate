namespace Services;

public interface IStorageService
{
    Task<StorageResult> UploadAsync(IFormFile file, string folder, string? customName = null);
    Task<StorageResult> UploadAsync(Stream stream, string fileName, string contentType, string folder, string? customName = null);
    Task<bool> DeleteAsync(string storedName, string folder);
    Task<string?> GetUrlAsync(string storedName, string folder);
    Task<Stream?> GetAsync(string storedName, string folder);
    string GetPublicUrl(string storedName, string folder);
}

public class StorageResult
{
    public bool Success { get; set; }
    public string? StoredName { get; set; }
    public string? Url { get; set; }
    public string? Error { get; set; }
    public string? StorageType { get; set; }
    public string? StoragePath { get; set; }
}
