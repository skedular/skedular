using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Kafka.Configurations;
using Enterprise.Shared.Kafka.Logger;
using Enterprise.Shared.Kafka.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace Enterprise.Shared.Kafka.Consume;

public interface IConsumerFactory
{
    IConsumer<TKey, TValue> Build<TKey, TValue>(KafkaConfiguration kafkaConfiguration);
}

public class ConsumerFactory(
    ApplicationConfiguration applicationConfiguration,
    IServiceProvider serviceProvider,
    IKafkaClientNaming clientNaming,
    IKafkaLogger kafkaLogger)
    : IConsumerFactory
{
    public IConsumer<TKey, TValue> Build<TKey, TValue>(KafkaConfiguration kafkaConfiguration)
    {
        ArgumentNullException.ThrowIfNull(kafkaConfiguration);

        var config = new ConsumerConfig
        {
            GroupId = applicationConfiguration.GetSource(),
            BootstrapServers = kafkaConfiguration.BootstrapServers,
            AutoOffsetReset = kafkaConfiguration.AutoOffsetReset ?? AutoOffsetReset.Earliest,
            EnableAutoOffsetStore = false,
            SecurityProtocol = kafkaConfiguration.SecurityProtocol,
            ClientId = clientNaming.GetClientId(),
            SaslMechanism = kafkaConfiguration.SaslMechanism,
            SaslUsername = kafkaConfiguration.SaslUsername,
            SaslPassword = kafkaConfiguration.SaslPassword,
            HeartbeatIntervalMs = kafkaConfiguration.HeartbeatIntervalMs,
            SessionTimeoutMs = kafkaConfiguration.SessionTimeoutMs,
            MaxPollIntervalMs = kafkaConfiguration.MaxPollIntervalMs
        };

        var builder = new ConsumerBuilder<TKey, TValue>(config);

        if (!KafkaSerialization.CanSerializeNatively<TKey>())
        {
            var serializer = serviceProvider.GetRequiredService<IAsyncDeserializer<TKey>>()
                .AsSyncOverAsync();
            builder.SetKeyDeserializer(serializer);
        }

        if (!KafkaSerialization.CanSerializeNatively<TValue>())
        {
            var serializer = serviceProvider.GetRequiredService<IAsyncDeserializer<TValue>>()
                .AsSyncOverAsync();
            builder.SetValueDeserializer(serializer);
        }

        kafkaLogger.SetLogHandler(builder);

        builder.SetPartitionChangeLogging();

        return builder.Build();
    }
}
