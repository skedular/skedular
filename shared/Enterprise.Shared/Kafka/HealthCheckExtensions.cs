using Confluent.Kafka;
using Enterprise.Shared.HealthCheck;
using Enterprise.Shared.Kafka.Configurations;
using Microsoft.Extensions.DependencyInjection;

namespace Enterprise.Shared.Kafka;

public static class HealthCheckExtensions
{
    /// <summary>
    ///     Checks for Kafka connectivity.
    ///     Pass in your bootstrapServers list.
    ///     This binds to the "services" tag that outputs to /health/readiness
    /// </summary>
    public static IHealthChecksBuilder AddKafkaBrokerHealthCheck(
        this IServiceCollection services,
        KafkaConfiguration kafkaConfiguration,
        int healthCheckTimeOutInSeconds = 5
    )
    {
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = kafkaConfiguration.BootstrapServers,
            SecurityProtocol = kafkaConfiguration.SecurityProtocol,
            SaslMechanism = kafkaConfiguration.SaslMechanism,
            SaslUsername = kafkaConfiguration.SaslUsername,
            SaslPassword = kafkaConfiguration.SaslPassword
        };

        return services
            .AddHealthChecks()
            .AddKafka(
                producerConfig,
                tags: [HealthCheckTags.Readiness],
                timeout: TimeSpan.FromSeconds(healthCheckTimeOutInSeconds)
            );
    }
}
