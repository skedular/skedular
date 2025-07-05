using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Path = System.IO.Path;

namespace Enterprise.Shared.FileStorage;

public static class Extensions
{
    public static IServiceCollection AddFileStorage(
        this IServiceCollection services,
        IConfiguration configuration,
        string publicCdnFileEndpoint,
        string privateFileEndpoint)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(publicCdnFileEndpoint);
        ArgumentException.ThrowIfNullOrWhiteSpace(privateFileEndpoint);

        var fileStorageConfiguration = configuration.GetSection(FileStorageConfiguration.Key).Get<FileStorageConfiguration>();
        ArgumentNullException.ThrowIfNull(fileStorageConfiguration);

        services = services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = fileStorageConfiguration.MaxFileSize;
            options.ValueLengthLimit = int.MaxValue;
            options.MultipartHeadersLengthLimit = int.MaxValue;
        });

        fileStorageConfiguration.PublicCdnFileEndpoint = publicCdnFileEndpoint;
        fileStorageConfiguration.PrivateFileEndpoint = privateFileEndpoint;

        if (fileStorageConfiguration.UseLocal)
        {
            if (string.IsNullOrWhiteSpace(fileStorageConfiguration.LocalCdnPath))
            {
                fileStorageConfiguration.LocalCdnPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "cdn");
            }

            Directory.CreateDirectory(fileStorageConfiguration.LocalCdnPath);

            if (string.IsNullOrWhiteSpace(fileStorageConfiguration.LocalPrivateFilePath))
            {
                fileStorageConfiguration.LocalPrivateFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "private");
            }

            Directory.CreateDirectory(fileStorageConfiguration.LocalPrivateFilePath);

            return services
                .AddSingleton(fileStorageConfiguration)
                .AddSingleton<ICdnService, LocalCdnService>()
                .AddSingleton<IPrivateFileService, LocalPrivateFileService>();
        }

        var cloudflare = configuration.GetSection(CloudflareConfiguration.Key).Get<CloudflareConfiguration>();
        ArgumentNullException.ThrowIfNull(cloudflare);

        return services
            .AddSingleton(cloudflare)
            .AddSingleton<ICdnService, CloudflareCdnService>()
            .AddSingleton<IPrivateFileService, CloudflarePrivateFileService>();
    }
}
