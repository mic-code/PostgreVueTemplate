using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Utility;

namespace Services;

public interface IStorageServiceFactory
{
    IStorageService GetStorageService();
    string GetCurrentStorageType();
}

public class StorageServiceFactory : IStorageServiceFactory
{
    private readonly IStorageService _storageService;
    private readonly string _storageType;

    public StorageServiceFactory(IConfiguration config, IHostEnvironment env, ILoggerFactory loggerFactory)
    {
        _storageType = config["Storage:Provider"]?.ToLower() ?? "local";
        
        _storageService = _storageType switch
        {
            "s3" => new S3StorageService(loggerFactory.CreateLogger<S3StorageService>()),
            _ => new LocalStorageService(config, env, loggerFactory.CreateLogger<LocalStorageService>())
        };
    }

    public IStorageService GetStorageService() => _storageService;

    public string GetCurrentStorageType() => _storageType;
}