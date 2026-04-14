using System.Reflection;
using Confluent.Kafka;
using Enterprise.Shared.Events;
using Enterprise.Shared.Kafka.Configurations;
using Enterprise.Shared.Kafka.Telemetry;
using Enterprise.Shared.Telemetry;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.Kafka.Produce;

public interface IKafkaPublisher<in TKey, in TValue> where TValue : class, IEvent
{
    Task PublishAsync(TKey key, TValue outgoingEvent, CancellationToken cancellationToken);
}

public class KafkaPublisher<TKey, TValue>(
    IProducer<TKey, TValue> producer,
    IActivityAccessor activityAccessor,
    IActivityPropagator<Headers> activityPropagator,
    KafkaConfiguration kafkaConfiguration,
    ILogger<KafkaPublisher<TKey, TValue>> logger)
    : IKafkaPublisher<TKey, TValue>
    where TValue : class, IEvent
{
    public async Task PublishAsync(TKey key, TValue outgoingEvent, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(outgoingEvent);
        ArgumentNullException.ThrowIfNull(typeof(TValue).GetCustomAttribute<KafkaTopicAttribute>());

        var activitySource = activityAccessor.GetActivitySource(TelemetryKeys.ProducerActivitySourceName);
        using var activity = activitySource.StartActivity("publish event");
        var topic = outgoingEvent.GetTopicName(kafkaConfiguration.OutgoingTopicPrefix);
        logger.LogDebug("Publishing Kafka event. EventType={EventType}, Topic={Topic}", typeof(TValue).FullName, topic);

        try
        {
            var message = new Message<TKey, TValue> { Value = outgoingEvent, Key = key, Headers = [] };

            activityAccessor.AddEvent("PublishAsync", "publish", new Dictionary<string, string> { ["Topic"] = topic });
            activityPropagator.PropagateActivity(message.Headers);

            await producer.ProduceAsync(topic, message, cancellationToken);
            logger.LogInformation("Kafka event published successfully. EventType={EventType}, Topic={Topic}", typeof(TValue).FullName, topic);
        }
        catch (Exception ex)
        {
            activityAccessor.AddException(ex);
            logger.LogWarning("Kafka event publishing failed. EventType={EventType}, Topic={Topic}, ExceptionType={ExceptionType}",
                typeof(TValue).FullName,
                topic,
                ex.GetType().Name);

            throw;
        }
    }
}
