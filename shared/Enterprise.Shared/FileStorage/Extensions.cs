using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Path = System.IO.Path;

namespace Enterprise.Shared.FileStorage;

public static class Extensions
{
    private static readonly string s_homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

    /// <summary>
    ///     Registers file-storage services (<see cref="ICdnService" /> and <see cref="IPrivateFileService" />).
    ///     Uses the local filesystem backend when <c>FileStorage:UseLocal</c> is <c>true</c>;
    ///     otherwise uses the Cloudflare backend.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">Application configuration (reads the <c>FileStorage</c> and <c>Cloudflare</c> sections).</param>
    /// <param name="publicCdnFileEndpoint">Public base URL used to build CDN file URLs returned to clients.</param>
    /// <param name="privateFileEndpoint">Base URL used to build private-file download URLs.</param>
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
                fileStorageConfiguration.LocalCdnPath = Path.Combine(s_homeDirectory, "wwwroot", "cdn");
            }

            Directory.CreateDirectory(fileStorageConfiguration.LocalCdnPath);

            if (string.IsNullOrWhiteSpace(fileStorageConfiguration.LocalPrivateFilePath))
            {
                fileStorageConfiguration.LocalPrivateFilePath = Path.Combine(s_homeDirectory, "wwwroot", "private");
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
            .AddSingleton(fileStorageConfiguration)
            .AddSingleton(cloudflare)
            .AddSingleton<ICdnService, CloudflareCdnService>()
            .AddSingleton<IPrivateFileService, CloudflarePrivateFileService>();
    }
}
