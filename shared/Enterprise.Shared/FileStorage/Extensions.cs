using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Path = System.IO.Path;

namespace Enterprise.Shared.FileStorage;

public static class Extensions
{
    private static readonly string s_homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

    private static FileStorageConfiguration GetFileStorageConfiguration(IServiceCollection services, IConfiguration configuration)
    {
        var fileStorageConfiguration = configuration.GetSection(FileStorageConfiguration.Key).Get<FileStorageConfiguration>();
        ArgumentNullException.ThrowIfNull(fileStorageConfiguration);

        services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = fileStorageConfiguration.MaxFileSize;
            options.ValueLengthLimit = int.MaxValue;
            options.MultipartHeadersLengthLimit = int.MaxValue;
        });

        return fileStorageConfiguration;
    }

    private static string EnsureLocalDirectory(string configuredPath, string folderName)
    {
        if (string.IsNullOrWhiteSpace(configuredPath))
        {
            configuredPath = Path.Combine(s_homeDirectory, "wwwroot", folderName);
        }

        Directory.CreateDirectory(configuredPath);
        return configuredPath;
    }

    /// <param name="services">The service collection to configure.</param>
    extension(IServiceCollection services)
    {
        /// <summary>
        ///     Registers file-storage service (<see cref="IFileService" />) only.
        ///     Uses filesystem backend when <c>FileStorage:UseFileServer</c> is <c>true</c>;
        ///     otherwise uses the Cloudflare backend.
        /// </summary>
        /// <param name="configuration">Application configuration (reads the <c>FileStorage</c> and <c>Cloudflare</c> sections).</param>
        /// <param name="fileEndpoint">Base URL used to build file download URLs.</param>
        public IServiceCollection AddFileUploadStorage(IConfiguration configuration, string fileEndpoint)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(fileEndpoint);

            var fileStorageConfiguration = GetFileStorageConfiguration(services, configuration);
            fileStorageConfiguration.FileEndpoint = fileEndpoint;

            if (fileStorageConfiguration.UseFileServer)
            {
                fileStorageConfiguration.FileServerFilePath = EnsureLocalDirectory(fileStorageConfiguration.FileServerFilePath, "private");

                return services
                    .AddSingleton(fileStorageConfiguration)
                    .AddSingleton<IFileService, LocalFileService>();
            }

            var cloudflare = configuration.GetSection(CloudflareConfiguration.Key).Get<CloudflareConfiguration>();
            ArgumentNullException.ThrowIfNull(cloudflare);

            return services
                .AddSingleton(fileStorageConfiguration)
                .AddSingleton(cloudflare)
                .AddSingleton<IFileService, CloudflareFileService>();
        }

        /// <summary>
        ///     Registers file-storage services (<see cref="ICdnService" /> and <see cref="IFileService" />).
        ///     Uses filesystem backend when <c>FileStorage:UseFileServer</c> is <c>true</c>;
        ///     otherwise uses the Cloudflare backend.
        /// </summary>
        /// <param name="configuration">Application configuration (reads the <c>FileStorage</c> and <c>Cloudflare</c> sections).</param>
        /// <param name="publicCdnFileEndpoint">Public base URL used to build CDN file URLs returned to clients.</param>
        /// <param name="fileEndpoint">Base URL used to build file download URLs.</param>
        public IServiceCollection AddFileStorage(IConfiguration configuration, string publicCdnFileEndpoint, string fileEndpoint)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(publicCdnFileEndpoint);
            ArgumentException.ThrowIfNullOrWhiteSpace(fileEndpoint);

            var fileStorageConfiguration = GetFileStorageConfiguration(services, configuration);

            fileStorageConfiguration.PublicCdnFileEndpoint = publicCdnFileEndpoint;
            fileStorageConfiguration.FileEndpoint = fileEndpoint;

            if (fileStorageConfiguration.UseFileServer)
            {
                fileStorageConfiguration.FileServerPublicFilePath = EnsureLocalDirectory(fileStorageConfiguration.FileServerPublicFilePath, "cdn");
                fileStorageConfiguration.FileServerFilePath = EnsureLocalDirectory(fileStorageConfiguration.FileServerFilePath, "private");

                return services
                    .AddSingleton(fileStorageConfiguration)
                    .AddSingleton<ICdnService, LocalCdnService>()
                    .AddSingleton<IFileService, LocalFileService>();
            }

            var cloudflare = configuration.GetSection(CloudflareConfiguration.Key).Get<CloudflareConfiguration>();
            ArgumentNullException.ThrowIfNull(cloudflare);

            return services
                .AddSingleton(fileStorageConfiguration)
                .AddSingleton(cloudflare)
                .AddSingleton<ICdnService, CloudflareCdnService>()
                .AddSingleton<IFileService, CloudflareFileService>();
        }
    }
}
