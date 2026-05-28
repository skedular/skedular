using System.Diagnostics;
using Enterprise.Shared.Database.Interceptors;
using Enterprise.Shared.Telemetry;
using Enterprise.Shared.Telemetry.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Temporalio.Client;
using IsolationLevel = System.Data.IsolationLevel;

namespace Enterprise.Shared.Outbox.Temporal;

/// <summary>
///     Drains Temporal signal outbox rows in small batches.
///     The worker intentionally separates the flow into:
///     1. claim rows under a short transaction with SKIP LOCKED
///     2. release the transaction before any remote Temporal RPC
///     3. finalize each claimed row with a lightweight success/failure write
///     This avoids holding database locks open while waiting on Temporal.
/// </summary>
public class TemporalSignalOutboxBackgroundService<TDbContext>(
    IOutboxDbContextAccessor<TDbContext> contextAccessor,
    IActivityAccessor activityAccessor,
    ILogger<TemporalSignalOutboxBackgroundService<TDbContext>> logger,
    OpenTelemetryConfiguration openTelemetryConfiguration,
    TimeProvider timeProvider,
    IActivityPropagator<IDictionary<string, string>> dictionaryActivityPropagator,
    IServiceProvider serviceProvider)
    : BackgroundService where TDbContext : DbContext, ITemporalSignalOutboxStore
{
    private const int CriticalRetryThreshold = 5;

    // Small batches reduce head-of-line blocking without making one polling pass too heavy.
    private const int BatchSize = 10;

    // Remote Temporal calls are network-bound, so limited parallelism improves latency
    // without overwhelming the Temporal cluster or this process.
    private const int MaxDegreeOfParallelism = 4;

    // Claimed rows are leased by pushing LastRetry into the future. If the worker crashes,
    // another worker can reclaim them once this lease expires.
    private readonly TimeSpan _processingLeaseTime = TimeSpan.FromMinutes(1);

    // When no rows are eligible we pause briefly before checking again.
    private readonly TimeSpan _retryTime = TimeSpan.FromSeconds(1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var className = GetType().ToFullName();

        logger.LogInformation("Starting Temporal Outbox - {Class}", className);

        await Policy
            .Handle<Exception>()
            .WaitAndRetryForeverAsync(
                _ => TimeSpan.FromSeconds(5),
                (exception, retry, retryTime) =>
                    logger.LogError(exception, "Database issue occured! Retry {RetryCount} will start in {Time}", retry, retryTime))
            .ExecuteAsync(async token => await ProcessOutboxAsync(token), stoppingToken);

        logger.LogInformation("Stopping Temporal {Class}", className);
    }

    private async Task ProcessOutboxAsync(CancellationToken cancellationToken)
    {
        var activitySource = activityAccessor.GetActivitySource(TelemetryKeys.TemporalSignalActivitySourceName);

        while (!cancellationToken.IsCancellationRequested)
        {
            // Claim first, then do remote work outside the transaction. This is the key
            // change from the older implementation that held a transaction open across RPCs.
            List<ClaimedOutboxEvent> outboxEvents;
            if (openTelemetryConfiguration.ExcludeOutboxTelemetry)
            {
                using (activitySource.StartActivity(TelemetryKeys.TemporalSignalEventPoll))
                {
                    outboxEvents = await TryClaimOutboxEventsAsync(cancellationToken);
                }
            }
            else
            {
                outboxEvents = await TryClaimOutboxEventsAsync(cancellationToken);
            }

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
    ///     This is intentionally written with SemaphoreSlim + Task.WhenAll instead of
    ///     Parallel.ForEachAsync so the execution model stays obvious and easy to extend
    ///     with per-item metrics or timeouts later.
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
        var activitySource = activityAccessor.GetActivitySource(TelemetryKeys.TemporalSignalActivitySourceName);

        using (dictionaryActivityPropagator.StartActivityFromPropagationContext(
                   new Dictionary<string, string>(),
                   activitySource,
                   TelemetryKeys.TemporalSignalEventSend,
                   ActivityKind.Producer))
        {
            try
            {
                // Each claimed row is processed independently. A failure on one row
                // should not stop the rest of the already-claimed batch.
                logger.LogTrace("Signaling {SignalType}", outboxEvent.SignalType);

                await using var scope = serviceProvider.CreateAsyncScope();
                var temporalSignalOutboxExecutor = scope.ServiceProvider.GetRequiredService<ITemporalSignalOutboxExecutor>();

                outboxEvent.WorkflowSignalOptions.Rpc = new RpcOptions { CancellationToken = cancellationToken };
                await temporalSignalOutboxExecutor.SignalAsync(
                    outboxEvent.WorkflowId,
                    outboxEvent.SignalType,
                    outboxEvent.ExecutionArgs,
                    outboxEvent.WorkflowSignalOptions,
                    cancellationToken);

                activityAccessor.AddEvent(
                    "Publish Temporal Signal Outbox Message",
                    "publish_temporal_signal_outbox_message",
                    new Dictionary<string, string> { [nameof(outboxEvent.SignalType)] = outboxEvent.SignalType });

                logger.LogTrace("Signal {SignalType} execution started. Removing from outbox", outboxEvent.SignalType);
                // Success means the row has been durably handed off to Temporal, so
                // it can be removed from the outbox.
                await CompleteOutboxEventAsync(outboxEvent.Id, outboxEvent.LeasedUntil, cancellationToken);
            }
            catch (Exception ex)
            {
                var retryCount = outboxEvent.OriginalRetryCount + 1;
                var level = retryCount < CriticalRetryThreshold ? LogLevel.Warning : LogLevel.Critical;

                activityAccessor.AddException(ex);

                activityAccessor.AddEvent(
                    "Retry Temporal signal Outbox Message",
                    "retry_temporal_signal_outbox_message",
                    new Dictionary<string, string>
                    {
                        [nameof(TemporalSignalOutbox.LastRetry)] = timeProvider.GetUtcNow().ToString("O"),
                        [nameof(LogLevel)] = level.ToString("G")
                    });

                logger.Log(
                    level,
                    ex,
                    "Failed to signal {SignalType}. Setting retry count to {RetryCount} and last retry to {LastRetry}",
                    outboxEvent.SignalType,
                    retryCount,
                    timeProvider.GetUtcNow());

                // Failure keeps the row in the outbox and restores a normal retryable
                // state instead of losing the signal.
                await FailOutboxEventAsync(outboxEvent.Id, outboxEvent.LeasedUntil, outboxEvent.OriginalRetryCount, ex, cancellationToken);
            }
        }
    }

    /// <summary>
    ///     Claims a batch of rows atomically.
    ///     ReadCommitted + FOR UPDATE SKIP LOCKED matter here because multiple worker
    ///     instances may be polling at once. The transaction exists only for this claim step.
    /// </summary>
    private async Task<List<ClaimedOutboxEvent>> TryClaimOutboxEventsAsync(CancellationToken cancellationToken)
    {
        var dbContext = await contextAccessor.GetContextAsync(cancellationToken);
        try
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
            var thresholdTime = timeProvider.GetUtcNow() - _retryTime;
            var outboxEvents = await dbContext.TemporalSignalOutbox
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

            // Copy the data needed for processing into an immutable in-memory shape so the
            // claimed DbContext can be disposed before the remote call starts.
            var now = timeProvider.GetUtcNow();
            var leasedUntil = now.Add(_processingLeaseTime);
            var claimed = outboxEvents
                .Select(outboxEvent => new ClaimedOutboxEvent(
                    outboxEvent.Id,
                    outboxEvent.WorkflowId,
                    outboxEvent.SignalType,
                    outboxEvent.ExecutionArgs,
                    outboxEvent.WorkflowSignalOptions,
                    outboxEvent.RetryCount,
                    leasedUntil))
                .ToList();

            foreach (var outboxEvent in outboxEvents)
            {
                // A claimed row is not "processed". It is only leased. We use LastRetry as the
                // lease timestamp so another worker can reclaim it later if this worker dies.
                outboxEvent.RetryCount = Math.Max(1, outboxEvent.RetryCount);
                outboxEvent.LastRetry = leasedUntil;
                outboxEvent.ProcessingErrors = null;
            }

            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return claimed;
        }
        finally
        {
            await contextAccessor.ReleaseContextAsync(dbContext, cancellationToken);
        }
    }

    /// <summary>
    ///     Finalizes a successful send.
    ///     No explicit transaction is needed here because this is just a single-row delete
    ///     and SaveChanges already wraps the write safely at the provider level.
    /// </summary>
    private async Task CompleteOutboxEventAsync(string id, DateTimeOffset leasedUntil, CancellationToken cancellationToken)
    {
        var dbContext = await contextAccessor.GetContextAsync(cancellationToken);
        try
        {
            var outboxEvent = await dbContext.TemporalSignalOutbox.FirstOrDefaultAsync(
                item => item.Id == id && item.LastRetry == leasedUntil,
                cancellationToken);
            if (outboxEvent is null)
            {
                return;
            }

            dbContext.Remove(outboxEvent);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        finally
        {
            await contextAccessor.ReleaseContextAsync(dbContext, cancellationToken);
        }
    }

    /// <summary>
    ///     Finalizes a failed send by restoring retry metadata.
    ///     We intentionally set LastRetry back to "now" so the normal retry threshold logic
    ///     controls when the row becomes eligible again.
    /// </summary>
    private async Task FailOutboxEventAsync(
        string id,
        DateTimeOffset leasedUntil,
        int originalRetryCount,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var dbContext = await contextAccessor.GetContextAsync(cancellationToken);
        try
        {
            var outboxEvent = await dbContext.TemporalSignalOutbox.FirstOrDefaultAsync(
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
        finally
        {
            await contextAccessor.ReleaseContextAsync(dbContext, cancellationToken);
        }
    }

    // This detached processing shape prevents us from carrying a live tracked entity and
    // DbContext across the remote Temporal call.
    private sealed record ClaimedOutboxEvent(
        string Id,
        string WorkflowId,
        string SignalType,
        string? ExecutionArgs,
        WorkflowSignalOptions WorkflowSignalOptions,
        int OriginalRetryCount,
        DateTimeOffset LeasedUntil);
}
