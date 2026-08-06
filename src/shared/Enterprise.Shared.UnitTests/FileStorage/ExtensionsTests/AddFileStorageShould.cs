using Enterprise.Shared.Configurations;
using Enterprise.Shared.FileStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testing.Shared.Fixtures;

namespace Enterprise.Shared.UnitTests.FileStorage.ExtensionsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class AddFileStorageShould
{
    [Theory]
    [AutoFakeItEasyData([typeof(ServiceCollectionFixtureCustomizer)])]
    public void Register_local_services_when_use_file_server_is_true(ServiceCollection services)
    {
        var tempCdn = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        var tempPrivate = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [$"{FileStorageConfiguration.Key}:UseFileServer"] = "true",
                [$"{FileStorageConfiguration.Key}:FileServerPublicFilePath"] = tempCdn,
                [$"{FileStorageConfiguration.Key}:FileServerFilePath"] = tempPrivate,
                [$"{FileStorageConfiguration.Key}:MaxFileSize"] = "1048576",
            })
            .Build();

        services.AddLogging();
        services.AddSingleton(new ApplicationConfiguration
        {
            ApiBaseDomain = new Uri("https://example.com"),
        });
        services.AddFileStorage(configuration, "https://example.com/cdn", "https://example.com/files");

        var provider = services.BuildServiceProvider();
        provider.GetRequiredService<ICdnService>().ShouldBeOfType<LocalCdnService>();
        provider.GetRequiredService<IFileService>().ShouldBeOfType<LocalFileService>();

        if (Directory.Exists(tempCdn))
        {
            Directory.Delete(tempCdn);
        }

        if (Directory.Exists(tempPrivate))
        {
            Directory.Delete(tempPrivate);
        }
    }

    [Theory]
    [AutoFakeItEasyData([typeof(ServiceCollectionFixtureCustomizer)])]
    public void Throw_when_public_cdn_endpoint_is_empty(ServiceCollection services)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [$"{FileStorageConfiguration.Key}:UseFileServer"] = "true",
            })
            .Build();

        Should.Throw<ArgumentException>(() =>
            services.AddFileStorage(configuration, "", "https://example.com/files"));
    }

    [Theory]
    [AutoFakeItEasyData([typeof(ServiceCollectionFixtureCustomizer)])]
    public void Throw_when_file_endpoint_is_empty(ServiceCollection services)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [$"{FileStorageConfiguration.Key}:UseFileServer"] = "true",
            })
            .Build();

        Should.Throw<ArgumentException>(() =>
            services.AddFileStorage(configuration, "https://example.com/cdn", ""));
    }

    [Theory]
    [AutoFakeItEasyData([typeof(ServiceCollectionFixtureCustomizer)])]
    public void Throw_when_file_storage_configuration_is_missing(ServiceCollection services)
    {
        var configuration = new ConfigurationBuilder().Build();

        Should.Throw<ArgumentNullException>(() =>
            services.AddFileStorage(configuration, "https://example.com/cdn", "https://example.com/files"));
    }
}
