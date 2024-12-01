using System.Reflection;
using Api.Shared.Events;
using Confluent.Kafka;
using Enterprise.Shared.Kafka.Configurations;
using Enterprise.Shared.Kafka.Telemetry;
using Enterprise.Shared.Telemetry;

namespace Enterprise.Shared.Kafka.Produce;

public interface IKafkaPublisher<in TKey, in TValue> where TValue : class, IEvent
{
    Task PublishAsync(
        TKey key,
        TValue outgoingEvent,
        CancellationToken cancellationToken);
}

/// <summary>
///     Core functionality of kafka publishing.
///     Values to be sent require the <see cref="KafkaTopicAttribute" /> on the class
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
public class KafkaPublisher<TKey, TValue>(
    IProducer<TKey, TValue> producer,
    IActivityAccessor activityAccessor,
    IActivityPropagator<Headers> activityPropagator,
    KafkaConfiguration kafkaConfiguration)
    : IKafkaPublisher<TKey, TValue>
    where TValue : class, IEvent
{
    public async Task PublishAsync(
        TKey key,
        TValue outgoingEvent,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(outgoingEvent);
        ArgumentNullException.ThrowIfNull(typeof(TValue).GetCustomAttribute<KafkaTopicAttribute>());

        var activitySource = activityAccessor.GetActivitySource(TelemetryKeys.ProducerActivitySourceName);
        using var activity = activitySource.StartActivity("publish event");
        var topic = outgoingEvent.GetTopicName(kafkaConfiguration.OutgoingTopicPrefix);

        try
        {
            var message = new Message<TKey, TValue> { Value = outgoingEvent, Key = key, Headers = [] };

            activityAccessor.AddEvent("PublishAsync", "publish", new Dictionary<string, string> { ["Topic"] = topic });
            activityPropagator.PropagateActivity(message.Headers);

            await producer.ProduceAsync(topic, message, cancellationToken);
        }
        catch (Exception ex)
        {
            activityAccessor.AddException(ex);

            throw;
        }
    }
}
