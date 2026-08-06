using Confluent.Kafka;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Kafka.Configurations;
using Enterprise.Shared.Kafka.Logger;
using Enterprise.Shared.Kafka.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.Kafka.Consume;

public interface IConsumerFactory
{
    IConsumer<TKey, TValue> Build<TKey, TValue>(KafkaConfiguration kafkaConfiguration, Action<ConsumerBuilder<TKey, TValue>>? options = null);
}

public class ConsumerFactory(
    ApplicationConfiguration applicationConfiguration,
    IServiceProvider serviceProvider,
    IKafkaClientNaming clientNaming,
    IKafkaLogger kafkaLogger,
    ILogger<ConsumerFactory> logger)
    : IConsumerFactory
{
    public IConsumer<TKey, TValue> Build<TKey, TValue>(KafkaConfiguration kafkaConfiguration, Action<ConsumerBuilder<TKey, TValue>>? options = null)
    {
        ArgumentNullException.ThrowIfNull(kafkaConfiguration);
        logger.LogDebug("Building Kafka consumer. KeyType={KeyType}, ValueType={ValueType}", typeof(TKey).FullName, typeof(TValue).FullName);
        return BuildConsumer(options, BuildConsumerConfig(kafkaConfiguration));
    }

    private IConsumer<TKey, TValue> BuildConsumer<TKey, TValue>(Action<ConsumerBuilder<TKey, TValue>>? options, ConsumerConfig consumerConfig)
    {
        logger.LogDebug("Configuring Kafka consumer builder. KeyType={KeyType}, ValueType={ValueType}", typeof(TKey).FullName,
            typeof(TValue).FullName);
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
        logger.LogInformation("Kafka consumer built successfully. KeyType={KeyType}, ValueType={ValueType}", typeof(TKey).FullName,
            typeof(TValue).FullName);
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

        if (kafkaConfiguration.HeartbeatIntervalMs is not null)
        {
            config.HeartbeatIntervalMs = kafkaConfiguration.HeartbeatIntervalMs;
        }

        if (kafkaConfiguration.SessionTimeoutMs is not null)
        {
            config.SessionTimeoutMs = kafkaConfiguration.SessionTimeoutMs;
        }

        if (kafkaConfiguration.MaxPollIntervalMs is not null)
        {
            config.MaxPollIntervalMs = kafkaConfiguration.MaxPollIntervalMs;
        }

        if (kafkaConfiguration.FetchWaitMaxMs is not null)
        {
            config.FetchWaitMaxMs = kafkaConfiguration.FetchWaitMaxMs;
        }

        if (kafkaConfiguration.CancellationDelayMaxMs is not null)
        {
            config.CancellationDelayMaxMs = kafkaConfiguration.CancellationDelayMaxMs.Value;
        }

        logger.LogDebug("Kafka consumer configuration created. GroupId={GroupId}, HasSecurityProtocol={HasSecurityProtocol}",
            groupId,
            kafkaConfiguration.SecurityProtocol is not null);
        return config;
    }
}
