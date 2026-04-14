using Enterprise.Shared.Accounting;
using Enterprise.Shared.Accounting.Configurations;
using Enterprise.Shared.Security;
using Enterprise.Shared.UnitTests.Fixtures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enterprise.Shared.UnitTests.Accounting.ExtensionsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class AddXeroServicesShould
{
    [Theory]
    [AutoFakeItEasyData([typeof(ServiceCollectionFixtureCustomizer)])]
    public void Register_xero_services(ServiceCollection services)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [$"{XeroConfiguration.Key}:ClientId"] = "clientId",
                [$"{XeroConfiguration.Key}:ClientSecret"] = "secret",
                [$"{XeroConfiguration.Key}:EncryptionKey:Key"] = "12345678901234567890123456789012",
                [$"{XeroConfiguration.Key}:EncryptionKey:Iv"] = "1234567890123456"
            })
            .Build();

        services.AddLogging();
        services.AddSingleton<IStringEncryptionAlgorithm, StringEncryptionAlgorithm>();
        services.AddXeroServices(configuration);

        var provider = services.BuildServiceProvider();
        provider.GetRequiredService<IXeroSdkClientFactory>().ShouldNotBeNull();
        provider.GetRequiredService<IXeroTokenEncryptionService>().ShouldNotBeNull();
    }

    [Theory]
    [AutoFakeItEasyData([typeof(ServiceCollectionFixtureCustomizer)])]
    public void Throw_when_xero_configuration_is_missing(ServiceCollection services)
    {
        var configuration = new ConfigurationBuilder().Build();

        Should.Throw<ArgumentNullException>(() => services.AddXeroServices(configuration));
    }
}
