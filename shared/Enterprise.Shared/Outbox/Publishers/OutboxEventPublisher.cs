using Api.Shared.Events;
using Confluent.Kafka;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka.Configurations;
using Enterprise.Shared.Outbox.Database;
using Enterprise.Shared.Outbox.Database.Entities;
using Enterprise.Shared.Outbox.Telemetry;
using Enterprise.Shared.Random;
using Enterprise.Shared.Telemetry;

namespace Enterprise.Shared.Outbox.Publishers;

/// <summary>
///     Reliable delivery event publisher to kafka
/// </summary>
public interface IKafkaOutboxEventPublisher<in TKey, in TEvent> where TEvent : IMetadataEvent
{
    void Publish(TKey key, TEvent @event, IUnitOfWork unitOfWork);
}

public class KafkaOutboxEventPublisher<TKey, TEvent>(
    ISerializer<TKey> keySerializer,
    ISerializer<TEvent> payloadSerializer,
    IActivityAccessor activityAccessor,
    IActivityPropagator<IDictionary<string, string>> dictionaryActivityPropagator,
    KafkaConfiguration kafkaConfiguration,
    IRandomHelper randomHelper,
    TimeProvider timeProvider)
    : IKafkaOutboxEventPublisher<TKey, TEvent>
    where TEvent : class, IMetadataEvent
{
    public void Publish(TKey key, TEvent @event, IUnitOfWork unitOfWork)
    {
        ArgumentNullException.ThrowIfNull(@event);

        var topic = @event.GetTopicName(kafkaConfiguration.OutgoingTopicPrefix);
        var dbContext = unitOfWork as IKafkaOutboxStore;

        ArgumentNullException.ThrowIfNull(dbContext);

        using (activityAccessor.GetActivitySource(TelemetryKeys.KafkaActivitySourceName).StartActivity(TelemetryKeys.KafkaEventSave))
        {
            var headers = new Dictionary<string, string>();
            dictionaryActivityPropagator.PropagateActivity(headers);

            dbContext.KafkaOutbox.Add(new KafkaOutbox
            {
                Id = randomHelper.Generate(),
                Headers = headers,
                Key = keySerializer.Serialize(key, new SerializationContext(MessageComponentType.Key, topic)),
                Topic = topic,
                Payload = payloadSerializer.Serialize(@event, new SerializationContext(MessageComponentType.Value, topic)),
                Timestamp = timeProvider.GetUtcNow()
            });
        }
    }
}
