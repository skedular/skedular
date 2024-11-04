using Api.Shared.Events;
using Confluent.Kafka;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka.Configurations;
using Enterprise.Shared.Outbox.Database;
using Enterprise.Shared.Outbox.Telemetry;
using Enterprise.Shared.Random;
using Enterprise.Shared.Telemetry;

namespace Enterprise.Shared.Outbox.Publishers;

/// <summary>
///     Reliable delivery event publisher to kafka
/// </summary>
public interface IOutboxEventPublisher<in TKey, in TEvent> where TEvent : IMetadataEvent
{
    Task PublishAsync(
        TKey key,
        TEvent @event,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken);
}

public class OutboxEventPublisher<TKey, TEvent>(
    ISerializer<TKey> keySerializer,
    ISerializer<TEvent> payloadSerializer,
    IActivityAccessor activityAccessor,
    IActivityPropagator<IDictionary<string, string>> dictionaryActivityPropagator,
    KafkaConfiguration kafkaConfiguration,
    IRandomHelper randomHelper,
    TimeProvider timeProvider)
    : IOutboxEventPublisher<TKey, TEvent>
    where TEvent : class, IMetadataEvent
{
    public async Task PublishAsync(
        TKey key,
        TEvent @event,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(@event);

        var topic = @event.GetTopicName(kafkaConfiguration.OutgoingTopicPrefix);
        var serializedKey = keySerializer.Serialize(key, new SerializationContext(MessageComponentType.Key, topic));
        var serializedEvent =
            payloadSerializer.Serialize(@event, new SerializationContext(MessageComponentType.Value, topic));
        var dbContext = (IOutboxStore)unitOfWork;
        var activitySource = activityAccessor.GetActivitySource(TelemetryKeys.ActivitySourceName);

        using (activitySource.StartActivity(TelemetryKeys.EventSave))
        {
            var headers = new Dictionary<string, string>();
            dictionaryActivityPropagator.PropagateActivity(headers);

            dbContext.Outbox.Add(new Database.Entities.Outbox
            {
                Id = randomHelper.Generate(),
                Headers = headers,
                Key = serializedKey,
                Topic = topic,
                Payload = serializedEvent,
                Timestamp = timeProvider.GetUtcNow()
            });

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
