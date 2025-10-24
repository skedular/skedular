using System.Diagnostics;
using System.Text;
using Confluent.Kafka;
using Enterprise.Shared.Database.Interceptors;
using Enterprise.Shared.Kafka.Configurations;
using Enterprise.Shared.Kafka.Produce;
using Enterprise.Shared.Outbox.Database;
using Enterprise.Shared.Outbox.Database.Entities;
using Enterprise.Shared.Outbox.Telemetry;
using Enterprise.Shared.Telemetry;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using IsolationLevel = System.Data.IsolationLevel;

namespace Enterprise.Shared.Outbox;

public class KafkaOutboxBackgroundService<TDbContext>(
    IDbContextFactory<TDbContext> contextFactory,
    IProducerFactory producerFactory,
    IActivityAccessor activityAccessor,
    KafkaConfiguration kafkaConfiguration,
    ILogger<KafkaOutboxBackgroundService<TDbContext>> logger,
    TimeProvider timeProvider,
    IActivityPropagator<IDictionary<string, string>> dictionaryActivityPropagator)
    : BackgroundService where TDbContext : DbContext, IKafkaOutboxStore
{
    private const int CriticalRetryThreshold = 5;

    private static readonly Func<TDbContext, DateTimeOffset, CancellationToken, Task<KafkaOutbox?>>
        s_getOutboxItemQueryAsync =
            EF.CompileAsyncQuery<TDbContext, DateTimeOffset, KafkaOutbox?>((dbContext, thresholdRetryTime, cancellationToken) =>
                dbContext.KafkaOutbox
                    .TagWith(EntityFrameworkInterceptorTags.ForUpdateSkipLocked)
                    .OrderBy(query => query.RetryCount)
                    .FirstOrDefault(query => query.RetryCount == 0 || query.LastRetry < thresholdRetryTime));

    private readonly IProducer<byte[]?, byte[]> _producer = producerFactory.Build<byte[]?, byte[]>(kafkaConfiguration);
    private readonly TimeSpan _retryTime = TimeSpan.FromSeconds(1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var className = GetType().ToFullName();

        logger.LogInformation("Starting Kafka Outbox - {Class}", className);

        await Policy
            .Handle<Exception>()
            .WaitAndRetryForeverAsync(
                _ => TimeSpan.FromSeconds(5),
                (exception, retry, retryTime) =>
                    logger.LogError(exception, "Database issue occured! Retry {RetryCount} will start in {Time}", retry, retryTime))
            .ExecuteAsync(async token => await ProcessOutboxAsync(token), stoppingToken);

        logger.LogInformation("Stopping Kafka {Class}", className);
    }

    private async Task ProcessOutboxAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);
            await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
            var retryTime = TimeSpan.FromSeconds(_retryTime.TotalSeconds);
            var thresholdTime = timeProvider.GetUtcNow() - retryTime;
            var outboxEvent = await s_getOutboxItemQueryAsync(dbContext, thresholdTime, cancellationToken);
            if (outboxEvent is null)
            {
                await Task.Delay(_retryTime, cancellationToken);

                continue;
            }

            var kafkaHeaders = ConvertToKafkaHeaders(outboxEvent.Headers);
            var activitySource = activityAccessor.GetActivitySource(TelemetryKeys.KafkaActivitySourceName);

            using (dictionaryActivityPropagator.StartActivityFromPropagationContext(
                       outboxEvent.Headers,
                       activitySource,
                       TelemetryKeys.KafkaEventSend,
                       ActivityKind.Producer))
            {
                var message = new Message<byte[]?, byte[]>
                {
                    Headers = kafkaHeaders, Key = outboxEvent.Key, Value = outboxEvent.Payload, Timestamp = new Timestamp(outboxEvent.Timestamp)
                };

                try
                {
                    logger.LogTrace("Producing message {MessageKey}", message.Key);

                    await _producer.ProduceAsync(outboxEvent.Topic, message, cancellationToken);

                    activityAccessor.AddEvent(
                        "Publish Kafka Outbox Message",
                        "publish_kafka_outbox_message",
                        new Dictionary<string, string> { [nameof(outboxEvent.Topic)] = outboxEvent.Topic });

                    logger.LogTrace("Message {MessageKey} posted. Removing from outbox", message.Key);
                    dbContext.Remove(outboxEvent);
                }
                catch (Exception ex)
                {
                    outboxEvent.RetryCount += 1;
                    outboxEvent.LastRetry = DateTimeOffset.UtcNow;
                    outboxEvent.ProcessingErrors = ex.ToString().Truncate(Constants.MaxOutboxProcessingErrorsLength);

                    var level = outboxEvent.RetryCount < CriticalRetryThreshold ? LogLevel.Warning : LogLevel.Critical;

                    activityAccessor.AddException(ex);

                    activityAccessor.AddEvent(
                        "Retry Kafka Outbox Message",
                        "retry_kafka_outbox_message",
                        new Dictionary<string, string>
                        {
                            [nameof(outboxEvent.LastRetry)] = outboxEvent.LastRetry?.ToString("O")!, [nameof(LogLevel)] = level.ToString("G")
                        });

                    logger.Log(
                        level,
                        ex,
                        "Failed to push message {MessageKey}. Setting retry count to {RetryCount} and last retry to {LastRetry}",
                        message.Key,
                        outboxEvent.RetryCount,
                        outboxEvent.LastRetry);
                }

                await dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
        }
    }

    private static Headers ConvertToKafkaHeaders(IDictionary<string, string> dictionary)
    {
        var headers = new Headers();

        foreach (var header in dictionary)
        {
            headers.Add(header.Key, Encoding.UTF8.GetBytes(header.Value));
        }

        return headers;
    }
}
