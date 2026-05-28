using Enterprise.Shared.Kafka;
using Enterprise.Shared.Kafka.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Testing.Shared.Fixtures;

namespace Enterprise.Shared.UnitTests.Kafka.HealthCheckExtensionsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class HealthCheckExtensionsTests
{
    [Theory]
    [AutoFakeItEasyData([typeof(ServiceCollectionFixtureCustomizer)])]
    public void AddKafkaBrokerHealthCheck_Should_Register_HealthCheck_with_services_tag(ServiceCollection serviceCollection)
    {
        const string BootstrapServers = "fakebootstrapservers";
        var kafkaConfiguration = new KafkaConfiguration { BootstrapServers = BootstrapServers };

        serviceCollection.AddKafkaBrokerHealthCheck(kafkaConfiguration);

        // act
        var services = serviceCollection.BuildServiceProvider();

        var registration = services
            .GetRequiredService<IOptions<HealthCheckServiceOptions>>()
            .Value
            .Registrations
            .First();

        registration.Tags.ShouldContain(HealthCheck.Constants.ReadinessTag);
    }
}
