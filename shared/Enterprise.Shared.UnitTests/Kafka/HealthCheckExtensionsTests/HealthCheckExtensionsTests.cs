using Enterprise.Shared.HealthCheck;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Kafka.Configurations;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Xunit;

namespace Enterprise.Shared.UnitTests.Kafka.HealthCheckExtensionsTests;

public class HealthCheckExtensionsTests
{
    [Fact]
    public void AddKafkaBrokerHealthCheck_Should_Register_HealthCheck_with_services_tag()
    {
        const string BootstrapServers = "fakebootstrapservers";
        var kafkaConfiguration = new KafkaConfiguration { BootstrapServers = BootstrapServers };
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddKafkaBrokerHealthCheck(kafkaConfiguration);

        // act
        var services = serviceCollection.BuildServiceProvider();

        var registration = services
            .GetRequiredService<IOptions<HealthCheckServiceOptions>>()
            .Value
            .Registrations
            .First();

        registration.Tags.Should().Contain(Constants.ReadinessTag);
    }
}
