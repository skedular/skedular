using System.Diagnostics;
using System.Text;
using Confluent.Kafka;
using Enterprise.Shared.Database.Interceptors;
using Enterprise.Shared.Kafka.Configurations;
using Enterprise.Shared.Kafka.Produce;
using Enterprise.Shared.Telemetry;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using IsolationLevel = System.Data.IsolationLevel;

namespace Enterprise.Shared.Outbox.Kafka;

/// <summary>
///     Drains Kafka outbox rows in small batches.
///     Rows are claimed under a short transaction, published outside the transaction,
///     and then finalized with a small success/failure write.
///     This avoids holding database locks while waiting on Kafka.
/// </summary>
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

    // A modest batch size reduces queue latency without overloading one polling pass.
    private const int BatchSize = 10;

    // Kafka publishes are network-bound, so bounded parallelism improves throughput
    // without turning one poll loop into an unbounded fan-out.
    private const int MaxDegreeOfParallelism = 4;

    // Claimed rows are leased by moving LastRetry into the future.
    private readonly TimeSpan _processingLeaseTime = TimeSpan.FromMinutes(1);

    private readonly IProducer<byte[]?, byte[]> _producer = producerFactory.Build<byte[]?, byte[]>(kafkaConfiguration);

    // Idle poll interval when no Kafka rows are ready to be sent.
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
            // Claim a batch up front so Kafka network calls happen after the database
            // transaction has already committed.
            var outboxEvents = await TryClaimOutboxEventsAsync(cancellationToken);
            if (outboxEvents.Count == 0)
            {
                await Task.Delay(_retryTime, cancellationToken);

                continue;
            }

            await ProcessClaimedOutboxEventsAsync(outboxEvents, cancellationToken);
        }
    }

    /// <summary>
    ///     Processes a claimed batch with explicit bounded concurrency.
    ///     This makes the concurrency behavior more explicit than Parallel.ForEachAsync
    ///     and gives us a cleaner place to add per-item timeouts or metrics later.
    /// </summary>
    private async Task ProcessClaimedOutboxEventsAsync(
        IReadOnlyCollection<ClaimedOutboxEvent> outboxEvents,
        CancellationToken cancellationToken)
    {
        using var concurrencyGate = new SemaphoreSlim(MaxDegreeOfParallelism);
        var tasks = outboxEvents.Select(async outboxEvent =>
        {
            // ReSharper disable once AccessToDisposedClosure
            await concurrencyGate.WaitAsync(cancellationToken);

            try
            {
                await ProcessClaimedOutboxEventAsync(outboxEvent, cancellationToken);
            }
            finally
            {
                // ReSharper disable once AccessToDisposedClosure
                concurrencyGate.Release();
            }
        });

        await Task.WhenAll(tasks);
    }

    private async Task ProcessClaimedOutboxEventAsync(ClaimedOutboxEvent outboxEvent, CancellationToken cancellationToken)
    {
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
                // Publish each claimed row independently so one failing message does
                // not prevent later rows in the same batch from being attempted.
                logger.LogTrace("Producing message {MessageKey}", message.Key);

                await _producer.ProduceAsync(outboxEvent.Topic, message, cancellationToken);

                activityAccessor.AddEvent(
                    "Publish Kafka Outbox Message",
                    "publish_kafka_outbox_message",
                    new Dictionary<string, string> { [nameof(outboxEvent.Topic)] = outboxEvent.Topic });

                logger.LogTrace("Message {MessageKey} posted. Removing from outbox", message.Key);
                // Kafka accepted the message, so the outbox row can be removed.
                await CompleteOutboxEventAsync(outboxEvent.Id, outboxEvent.LeasedUntil, cancellationToken);
            }
            catch (Exception ex)
            {
                var retryCount = outboxEvent.OriginalRetryCount + 1;
                var level = retryCount < CriticalRetryThreshold ? LogLevel.Warning : LogLevel.Critical;

                activityAccessor.AddException(ex);

                activityAccessor.AddEvent(
                    "Retry Kafka Outbox Message",
                    "retry_kafka_outbox_message",
                    new Dictionary<string, string>
                    {
                        [nameof(KafkaOutbox.LastRetry)] = timeProvider.GetUtcNow().ToString("O"), [nameof(LogLevel)] = level.ToString("G")
                    });

                logger.Log(
                    level,
                    ex,
                    "Failed to push message {MessageKey}. Setting retry count to {RetryCount} and last retry to {LastRetry}",
                    message.Key,
                    retryCount,
                    timeProvider.GetUtcNow());

                // A failed publish stays in the outbox with updated retry metadata.
                await FailOutboxEventAsync(outboxEvent.Id, outboxEvent.LeasedUntil, outboxEvent.OriginalRetryCount, ex, cancellationToken);
            }
        }
    }

    /// <summary>
    ///     Atomically claims a batch of Kafka rows.
    ///     The explicit transaction is required here because SKIP LOCKED is only meaningful
    ///     while the transaction is holding the claim lock.
    /// </summary>
    private async Task<List<ClaimedOutboxEvent>> TryClaimOutboxEventsAsync(CancellationToken cancellationToken)
    {
        await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);
        await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
        var thresholdTime = timeProvider.GetUtcNow() - _retryTime;
        var outboxEvents = await dbContext.KafkaOutbox
            .TagWith(EntityFrameworkInterceptorTags.ForUpdateSkipLocked)
            .Where(query => query.RetryCount == 0 || query.LastRetry < thresholdTime)
            .OrderBy(query => query.RetryCount)
            .ThenBy(query => query.Timestamp)
            .Take(BatchSize)
            .ToListAsync(cancellationToken);
        if (outboxEvents.Count == 0)
        {
            await transaction.CommitAsync(cancellationToken);
            return [];
        }

        // Detach the data needed for publishing so the DbContext from the claim step can
        // be disposed before the Kafka call happens.
        var now = timeProvider.GetUtcNow();
        var leasedUntil = now.Add(_processingLeaseTime);
        var claimed = outboxEvents
            .Select(outboxEvent => new ClaimedOutboxEvent(
                outboxEvent.Id,
                outboxEvent.Topic,
                outboxEvent.Headers,
                outboxEvent.Key,
                outboxEvent.Payload,
                outboxEvent.Timestamp,
                outboxEvent.RetryCount,
                leasedUntil))
            .ToList();

        foreach (var outboxEvent in outboxEvents)
        {
            // Lease each row rather than marking it completed. If this worker dies, another
            // worker can reclaim it after the lease window.
            outboxEvent.RetryCount = Math.Max(1, outboxEvent.RetryCount);
            outboxEvent.LastRetry = leasedUntil;
            outboxEvent.ProcessingErrors = null;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return claimed;
    }

    /// <summary>
    ///     Removes a successfully published Kafka row.
    ///     No explicit transaction is needed for this single-row finalize step.
    /// </summary>
    private async Task CompleteOutboxEventAsync(string id, DateTimeOffset leasedUntil, CancellationToken cancellationToken)
    {
        await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);
        var outboxEvent = await dbContext.KafkaOutbox.FirstOrDefaultAsync(
            item => item.Id == id && item.LastRetry == leasedUntil,
            cancellationToken);
        if (outboxEvent is null)
        {
            return;
        }

        dbContext.Remove(outboxEvent);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    ///     Records retry metadata after a failed Kafka publish.
    ///     LastRetry is reset to "now" so the normal retry eligibility rule controls the next attempt.
    /// </summary>
    private async Task FailOutboxEventAsync(
        string id,
        DateTimeOffset leasedUntil,
        int originalRetryCount,
        Exception exception,
        CancellationToken cancellationToken)
    {
        await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);
        var outboxEvent = await dbContext.KafkaOutbox.FirstOrDefaultAsync(
            item => item.Id == id && item.LastRetry == leasedUntil,
            cancellationToken);
        if (outboxEvent is null)
        {
            return;
        }

        outboxEvent.RetryCount = originalRetryCount + 1;
        outboxEvent.LastRetry = timeProvider.GetUtcNow();
        outboxEvent.ProcessingErrors = exception.ToString().Truncate(Constants.MaxOutboxProcessingErrorsLength);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static Headers ConvertToKafkaHeaders(IDictionary<string, string> dictionary)
    {
        var headers = new Headers();

        // Kafka expects byte[] header values, so the stored string metadata is converted here.
        foreach (var header in dictionary)
        {
            headers.Add(header.Key, Encoding.UTF8.GetBytes(header.Value));
        }

        return headers;
    }

    // Detached processing payload used after the claim transaction has completed.
    private sealed record ClaimedOutboxEvent(
        string Id,
        string Topic,
        Dictionary<string, string> Headers,
        byte[] Key,
        byte[] Payload,
        DateTimeOffset Timestamp,
        int OriginalRetryCount,
        DateTimeOffset LeasedUntil);
}
