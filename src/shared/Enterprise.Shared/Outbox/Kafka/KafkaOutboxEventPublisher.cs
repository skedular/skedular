using Api.Shared.Events;
using Confluent.Kafka;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka.Configurations;
using Enterprise.Shared.Random;
using Enterprise.Shared.Telemetry;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.Outbox.Kafka;

public interface IKafkaOutboxEventPublisher<in TKey, in TEvent> where TEvent : IEvent
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
    TimeProvider timeProvider,
    ILogger<KafkaOutboxEventPublisher<TKey, TEvent>> logger)
    : IKafkaOutboxEventPublisher<TKey, TEvent> where TEvent : class, IEvent
{
    public void Publish(TKey key, TEvent @event, IUnitOfWork unitOfWork)
    {
        logger.LogDebug("Queueing Kafka event in outbox. EventType={EventType}", typeof(TEvent).FullName);
        using (activityAccessor.GetActivitySource(TelemetryKeys.KafkaActivitySourceName).StartActivity(TelemetryKeys.KafkaEventSave))
        {
            var headers = new Dictionary<string, string>();
            dictionaryActivityPropagator.PropagateActivity(headers);

            ArgumentNullException.ThrowIfNull(@event);
            var topic = @event.GetTopicName(kafkaConfiguration.OutgoingTopicPrefix);
            var dbContext = unitOfWork as IKafkaOutboxStore;
            ArgumentNullException.ThrowIfNull(dbContext);

            dbContext.KafkaOutbox.Add(new KafkaOutbox
            {
                Id = randomHelper.Generate(),
                Headers = headers,
                Key = keySerializer.Serialize(key, new SerializationContext(MessageComponentType.Key, topic)),
                Topic = topic,
                Payload = payloadSerializer.Serialize(@event, new SerializationContext(MessageComponentType.Value, topic)),
                Timestamp = timeProvider.GetUtcNow(),
            });

            logger.LogInformation("Kafka event queued in outbox successfully. EventType={EventType}, Topic={Topic}", typeof(TEvent).FullName, topic);
        }
    }
}
