using Confluent.Kafka;
using Enterprise.Shared.Kafka.Configurations;
using Enterprise.Shared.Kafka.Logger;
using Enterprise.Shared.Kafka.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.Kafka.Produce;

public interface IProducerFactory
{
    IProducer<TKey, TValue> Build<TKey, TValue>(KafkaConfiguration kafkaConfiguration);
}

public class ProducerFactory(
    IKafkaClientNaming clientNaming,
    IServiceProvider serviceProvider,
    ILogger<ProducerFactory> logger,
    IKafkaLogger kafkaLogger)
    : IProducerFactory
{
    public IProducer<TKey, TValue> Build<TKey, TValue>(KafkaConfiguration kafkaConfiguration)
    {
        ArgumentNullException.ThrowIfNull(kafkaConfiguration);

        using var logScope = logger.BeginScope("[{Build}<{Key}, {Value}>]", nameof(Build), typeof(TKey), typeof(TValue));

        var config = BuildProducerConfig(kafkaConfiguration);
        var producer = BuildProducer<TKey, TValue>(config);

        return producer;
    }

    private IProducer<TKey, TValue> BuildProducer<TKey, TValue>(ProducerConfig config)
    {
        var builder = new ProducerBuilder<TKey, TValue>(config);

        if (!KafkaSerialization.CanSerializeNatively<TKey>())
        {
            var serializer = serviceProvider.GetRequiredService<ISerializer<TKey>>();
            var className = serializer.GetType().ToFullName();

            logger.LogTrace("Setting serializer for {KeyType}: {SerializerType}", typeof(TKey), className);
            builder.SetKeySerializer(serializer);
        }

        if (!KafkaSerialization.CanSerializeNatively<TValue>())
        {
            var serializer = serviceProvider.GetRequiredService<ISerializer<TValue>>();
            var className = serializer.GetType().ToFullName();

            logger.LogTrace("Setting serializer for {KeyType}: {SerializerType}", typeof(TKey), className);
            builder.SetValueSerializer(serializer);
        }

        logger.LogTrace("Building producer of type <{TKey},{TValue}>", typeof(TKey), typeof(TValue));

        kafkaLogger.SetLogHandler(builder);
        var producer = builder.Build();

        return producer;
    }

    private ProducerConfig BuildProducerConfig(KafkaConfiguration kafkaConfiguration)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(kafkaConfiguration.BootstrapServers);

        var config = new ProducerConfig(kafkaConfiguration.ProducerSettings)
        {
            ClientId = clientNaming.GetClientId(),
            EnableIdempotence = false,
            BootstrapServers = kafkaConfiguration.BootstrapServers,
        };

        if (kafkaConfiguration.SecurityProtocol is not null)
        {
            config.SecurityProtocol = kafkaConfiguration.SecurityProtocol;
        }

        if (kafkaConfiguration.SaslMechanism is not null)
        {
            config.SaslMechanism = kafkaConfiguration.SaslMechanism;
        }

        if (kafkaConfiguration.SaslUsername is not null)
        {
            config.SaslUsername = kafkaConfiguration.SaslUsername;
        }

        if (kafkaConfiguration.SaslPassword is not null)
        {
            config.SaslPassword = kafkaConfiguration.SaslPassword;
        }

        if (kafkaConfiguration.CancellationDelayMaxMs is not null)
        {
            config.CancellationDelayMaxMs = kafkaConfiguration.CancellationDelayMaxMs.Value;
        }

        return config;
    }
}
