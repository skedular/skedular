using Api.Shared.Events;
using Confluent.Kafka;
using Enterprise.Shared.Kafka.Telemetry;
using Enterprise.Shared.Telemetry;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using Polly;

namespace Enterprise.Shared.Kafka.Consume;

public interface IKafkaMessageHandler<TKey, TEvent> where TKey : IEvent, new() where TEvent : IEvent, new()
{
    Task HandleMessageAsync(ConsumeResult<byte[], byte[]> consumeResult, CancellationToken cancellationToken);
}

public class KafkaMessageHandler<TKey, TEvent>(
    ILogger<KafkaMessageHandler<TKey, TEvent>> logger,
    IActivityAccessor activityAccessor,
    IServiceProvider serviceProvider,
    IDeserializer<TKey> keyDeserializer,
    IDeserializer<TEvent> valueDeserializer)
    : IKafkaMessageHandler<TKey, TEvent>
    where TKey : IEvent, new() where TEvent : IEvent, new()
{
    public async Task HandleMessageAsync(ConsumeResult<byte[], byte[]> consumeResult,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var eventSubscriber =
            scope.ServiceProvider.GetRequiredService<IEventSubscriber<TKey, TEvent>>();
        var activitySource = activityAccessor.GetActivitySource(TelemetryKeys.IncomingActivitySourceName);

        using (activitySource.StartActivity($"handler {eventSubscriber.GetType().Name}"))
        {
            var key = keyDeserializer.Deserialize(
                consumeResult.Message.Value,
                false,
                SerializationContext.Empty);

            var @event = valueDeserializer.Deserialize(
                consumeResult.Message.Value,
                false,
                SerializationContext.Empty);

            ConsumeResultContext.Current.Value = consumeResult;

            try
            {
                // Call the subscriber and process the message
                var eventContext = new EventContext(consumeResult);

                await Policy
                    .Handle<DbUpdateConcurrencyException>()
                    .Or<DbUpdateException>(ex =>
                        ex.InnerException is PostgresException &&
                        ex.InnerException.Message.Contains("duplicate key value violates unique constraint"))
                    .Or<InvalidOperationException>(ex =>
                        ex.Message.Contains("cannot be tracked because another instance with the key value") ||
                        (ex.Message.Contains(
                             "An exception has been raised that is likely due to a transient failure") &&
                         ex.InnerException is TimeoutException or NpgsqlException && (
                             ex.InnerException.Message.Contains("The operation has timed out") ||
                             ex.InnerException.Message.Contains("Exception while reading from stream"))))
                    .WaitAndRetryAsync(10, retryAttempt =>
                    {
                        logger.LogWarning("Failed to call eventSubscriber.HandleAsync - Retry attempt: {retryAttempt}",
                            retryAttempt);

                        return TimeSpan.FromSeconds(1);
                    })
                    .ExecuteAsync(async () =>
                    {
                        _ = await eventSubscriber.HandleAsync(eventContext, key, @event, cancellationToken);
                    });
            }
            catch (Exception ex)
            {
                activityAccessor.RecordException(ex);

                throw;
            }
        }
    }
}
