using System.Diagnostics;
using System.Text;
using Api.Shared;
using Confluent.Kafka;
using Enterprise.Shared.Database.Interceptors;
using Enterprise.Shared.Kafka.Configurations;
using Enterprise.Shared.Kafka.Produce;
using Enterprise.Shared.Outbox.Database;
using Enterprise.Shared.Outbox.Telemetry;
using Enterprise.Shared.Telemetry;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using IsolationLevel = System.Data.IsolationLevel;

namespace Enterprise.Shared.Outbox;

public class OutboxBackgroundService<TDbContext>(
    IDbContextFactory<TDbContext> contextFactory,
    IProducerFactory producerFactory,
    IActivityAccessor activityAccessor,
    KafkaConfiguration kafkaConfiguration,
    ILogger<OutboxBackgroundService<TDbContext>> logger,
    IActivityPropagator<IDictionary<string, string>> dictionaryActivityPropagator)
    : BackgroundService
    where TDbContext : DbContext, IOutboxStore
{
    private static readonly Func<TDbContext, DateTimeOffset, CancellationToken, Task<Database.Entities.Outbox?>>
        s_getOutboxItemQueryAsync =
            EF.CompileAsyncQuery<TDbContext, DateTimeOffset, Database.Entities.Outbox?>((
                    dbContext,
                    thresholdRetryTime,
                    cancellationToken) =>
                dbContext.Outbox
                    .TagWith(EntityFrameworkInterceptorTags.ForUpdateSkipLocked)
                    .Where<Database.Entities.Outbox>(query =>
                        query.RetryCount == 0 || query.LastRetry < thresholdRetryTime)
                    .OrderBy(query => query.RetryCount)
                    .FirstOrDefault());

    /// <summary>
    ///     Signals to poll the database. Triggered either by the <see cref="OutboxEvents.ItemAdded" /> or the
    ///     <see cref="OutboxParameters.RetryTime" /> timeout
    /// </summary>
    private readonly ManualResetEvent _poll = new(true);

    private readonly IProducer<byte[]?, byte[]> _producer = producerFactory.Build<byte[]?, byte[]>(kafkaConfiguration);
    private void TriggerPoll(object sender, EventArgs eventArgs) => _poll.Set();

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        OutboxEvents.ItemAdded += TriggerPoll!;

        await base.StartAsync(cancellationToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        OutboxEvents.ItemAdded -= TriggerPoll!;

        await base.StopAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var className = GetType().ToFullName();

        logger.LogInformation("Starting Outbox - {Class}", className);

        await OutboxParameters.DatabasePolicy.ExecuteAsync(
            async token => await ProcessOutboxAsync(token),
            stoppingToken);

        logger.LogInformation("Stopping {Class}", className);
    }

    private async Task ProcessOutboxAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            _poll.Reset();

            await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);
            await using var transaction =
                await dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
            var retryTime = TimeSpan.FromSeconds(OutboxParameters.RetryTime.TotalSeconds);
            var thresholdTime = DateTimeOffset.UtcNow - retryTime;
            var outboxEvent = await s_getOutboxItemQueryAsync(dbContext, thresholdTime, cancellationToken);
            if (outboxEvent == null)
            {
                // if there are no events, then wait till there is one, or poll the database after a certain amount of time.
                _poll.WaitOne(OutboxParameters.RetryTime);

                continue;
            }

            var kafkaHeaders = ConvertToKafkaHeaders(outboxEvent.Headers);
            var activitySource = activityAccessor.GetActivitySource(TelemetryKeys.ActivitySourceName);

            using (dictionaryActivityPropagator.StartActivityFromPropagationContext(
                       outboxEvent.Headers,
                       activitySource,
                       TelemetryKeys.EventSend,
                       ActivityKind.Producer))
            {
                var message = new Message<byte[]?, byte[]>
                {
                    Headers = kafkaHeaders,
                    Key = outboxEvent.Key,
                    Value = outboxEvent.Payload,
                    Timestamp = new Timestamp(outboxEvent.Timestamp)
                };

                try
                {
                    // Kafka will wait here if the Kafka Servers are offline
                    logger.LogTrace("Producing message {MessageKey}", message.Key);

                    await _producer.ProduceAsync(outboxEvent.Topic, message, cancellationToken);

                    activityAccessor.AddEvent("Publish", "publish",
                        new Dictionary<string, string> { [nameof(outboxEvent.Topic)] = outboxEvent.Topic });

                    logger.LogTrace("Message {MessageKey} posted. Removing from outbox", message.Key);
                    dbContext.Remove(outboxEvent);
                }
                catch (Exception ex)
                {
                    outboxEvent.RetryCount += 1;
                    outboxEvent.LastRetry = DateTimeOffset.UtcNow;
                    outboxEvent.ProcessingErrors = ex.ToString().Truncate(Constants.MaxOutboxProcessingErrorsLength);

                    var level = outboxEvent.RetryCount <
                                OutboxParameters.CriticalRetryThreshold
                        ? LogLevel.Warning
                        : LogLevel.Critical;

                    activityAccessor.AddException(ex);

                    activityAccessor.AddEvent(
                        "Retry",
                        "retry",
                        new Dictionary<string, string>
                        {
                            [nameof(outboxEvent.LastRetry)] = outboxEvent.LastRetry?.ToString("O")!,
                            [nameof(LogLevel)] = level.ToString("G")
                        });

                    logger.Log(
                        level,
                        ex,
                        "Failed to push message {MessageKey}. Setting retry count to {RetryCount} and last retry to {LastRetry}",
                        message.Key, outboxEvent.RetryCount, outboxEvent.LastRetry);
                }

                logger.LogTrace("Saving changes to Message {MessageKey}", message.Key);
                await dbContext.SaveChangesAsync(cancellationToken);
                logger.LogTrace("Commiting transaction for Message {MessageKey}", message.Key);
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
