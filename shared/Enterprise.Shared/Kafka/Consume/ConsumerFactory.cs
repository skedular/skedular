using Confluent.Kafka;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Kafka.Configurations;
using Enterprise.Shared.Kafka.Logger;
using Enterprise.Shared.Kafka.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace Enterprise.Shared.Kafka.Consume;

public interface IConsumerFactory
{
    IConsumer<TKey, TValue> Build<TKey, TValue>(
        KafkaConfiguration kafkaConfiguration,
        Action<ConsumerBuilder<TKey, TValue>>? options = null);
}

public class ConsumerFactory(
    ApplicationConfiguration applicationConfiguration,
    IServiceProvider serviceProvider,
    IKafkaClientNaming clientNaming,
    IKafkaLogger kafkaLogger)
    : IConsumerFactory
{
    public IConsumer<TKey, TValue> Build<TKey, TValue>(
        KafkaConfiguration kafkaConfiguration,
        Action<ConsumerBuilder<TKey, TValue>>? options = null)
    {
        ArgumentNullException.ThrowIfNull(kafkaConfiguration);

        var consumerConfig = BuildConsumerConfig(kafkaConfiguration);

        return BuildConsumer(options, consumerConfig);
    }

    private IConsumer<TKey, TValue> BuildConsumer<TKey, TValue>(
        Action<ConsumerBuilder<TKey, TValue>>? options,
        ConsumerConfig consumerConfig)
    {
        var builder = new ConsumerBuilder<TKey, TValue>(consumerConfig);

        if (!KafkaSerialization.CanSerializeNatively<TKey>())
        {
            var serializer = serviceProvider.GetRequiredService<IDeserializer<TKey>>();
            builder.SetKeyDeserializer(serializer);
        }

        if (!KafkaSerialization.CanSerializeNatively<TValue>())
        {
            var serializer = serviceProvider.GetRequiredService<IDeserializer<TValue>>();
            builder.SetValueDeserializer(serializer);
        }

        kafkaLogger.SetLogHandler(builder);

        options?.Invoke(builder);

        return builder.Build();
    }

    private ConsumerConfig BuildConsumerConfig(KafkaConfiguration kafkaConfiguration)
    {
        var groupId = applicationConfiguration.GetSource();
        ArgumentException.ThrowIfNullOrWhiteSpace(groupId);

        ArgumentException.ThrowIfNullOrEmpty(kafkaConfiguration.BootstrapServers);
        ArgumentException.ThrowIfNullOrEmpty(groupId);

        var config = new ConsumerConfig(kafkaConfiguration.ConsumerSettings)
        {
            ClientId = clientNaming.GetClientId(),
            EnableAutoOffsetStore = false,
            AutoOffsetReset = kafkaConfiguration.AutoOffsetReset ?? AutoOffsetReset.Earliest,
            GroupId = groupId,
            BootstrapServers = kafkaConfiguration.BootstrapServers
        };

        if (kafkaConfiguration.SecurityProtocol != null)
        {
            config.SecurityProtocol = kafkaConfiguration.SecurityProtocol;
        }

        if (kafkaConfiguration.SaslMechanism != null)
        {
            config.SaslMechanism = kafkaConfiguration.SaslMechanism;
        }

        if (kafkaConfiguration.SaslUsername != null)
        {
            config.SaslUsername = kafkaConfiguration.SaslUsername;
        }

        if (kafkaConfiguration.SaslPassword != null)
        {
            config.SaslPassword = kafkaConfiguration.SaslPassword;
        }

        if (kafkaConfiguration.HeartbeatIntervalMs != null)
        {
            config.HeartbeatIntervalMs = kafkaConfiguration.HeartbeatIntervalMs;
        }

        if (kafkaConfiguration.SessionTimeoutMs != null)
        {
            config.SessionTimeoutMs = kafkaConfiguration.SessionTimeoutMs;
        }

        if (kafkaConfiguration.MaxPollIntervalMs != null)
        {
            config.MaxPollIntervalMs = kafkaConfiguration.MaxPollIntervalMs;
        }

        if (kafkaConfiguration.FetchWaitMaxMs != null)
        {
            config.FetchWaitMaxMs = kafkaConfiguration.FetchWaitMaxMs;
        }

        return config;
    }
}
