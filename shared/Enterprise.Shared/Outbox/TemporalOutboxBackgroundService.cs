using System.Diagnostics;
using Enterprise.Shared.Database.Interceptors;
using Enterprise.Shared.Outbox.Database;
using Enterprise.Shared.Outbox.Database.Entities;
using Enterprise.Shared.Outbox.Telemetry;
using Enterprise.Shared.Telemetry;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Temporalio.Client;
using IsolationLevel = System.Data.IsolationLevel;

namespace Enterprise.Shared.Outbox;

/// <summary>
///     Drains workflow-start outbox rows in small batches.
///     The worker claims rows under a short transaction, releases that transaction,
///     starts workflows outside the transaction, and only then finalizes each row.
///     This keeps database locking separate from slow Temporal RPCs.
/// </summary>
public class TemporalOutboxBackgroundService<TDbContext>(
    IDbContextFactory<TDbContext> contextFactory,
    IActivityAccessor activityAccessor,
    ILogger<TemporalOutboxBackgroundService<TDbContext>> logger,
    TimeProvider timeProvider,
    IActivityPropagator<IDictionary<string, string>> dictionaryActivityPropagator,
    IServiceProvider serviceProvider)
    : BackgroundService where TDbContext : DbContext, ITemporalOutboxStore
{
    private const int CriticalRetryThreshold = 5;

    // A small batch size improves throughput without making each polling cycle too large.
    private const int BatchSize = 10;

    // Starting workflows is network-bound work, so we process claimed rows in parallel
    // with a small cap to reduce latency without overwhelming Temporal.
    private const int MaxDegreeOfParallelism = 4;

    // We reuse LastRetry as a lightweight lease marker while a claimed row is in flight.
    private readonly TimeSpan _processingLeaseTime = TimeSpan.FromMinutes(1);

    // Idle poll interval when no rows are currently eligible.
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
        while (!cancellationToken.IsCancellationRequested)
        {
            // Claim rows first so the expensive remote workflow-start call happens after
            // database locks are released.
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
    ///     Using SemaphoreSlim + Task.WhenAll keeps the concurrency model easy to reason
    ///     about and easier to extend than Parallel.ForEachAsync.
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
        var activitySource = activityAccessor.GetActivitySource(TelemetryKeys.TemporalActivitySourceName);

        using (dictionaryActivityPropagator.StartActivityFromPropagationContext(
                   new Dictionary<string, string>(),
                   activitySource,
                   TelemetryKeys.TemporalEventSend,
                   ActivityKind.Producer))
        {
            try
            {
                // Each claimed row is handled independently. One failed workflow-start
                // should not prevent other rows in the batch from being attempted.
                logger.LogTrace("Started executing workflow {WorkflowType}", outboxEvent.WorkflowType);

                await using var scope = serviceProvider.CreateAsyncScope();
                var temporalOutboxExecutor = scope.ServiceProvider.GetRequiredService<ITemporalOutboxExecutor>();

                outboxEvent.WorkflowOptions.Rpc = new RpcOptions { CancellationToken = cancellationToken };
                await temporalOutboxExecutor.StartWorkflowAsync(
                    outboxEvent.WorkflowType,
                    outboxEvent.ExecutionArgs,
                    outboxEvent.WorkflowOptions,
                    cancellationToken);

                activityAccessor.AddEvent(
                    "Publish Temporal Outbox Message",
                    "publish_temporal_outbox_message",
                    new Dictionary<string, string> { [nameof(outboxEvent.WorkflowType)] = outboxEvent.WorkflowType });

                logger.LogTrace("Workflow {WorkflowType} execution started. Removing from outbox", outboxEvent.WorkflowType);
                // Successful workflow start means the row has completed its handoff.
                await CompleteOutboxEventAsync(outboxEvent.Id, outboxEvent.LeasedUntil, cancellationToken);
            }
            catch (Exception ex)
            {
                var retryCount = outboxEvent.OriginalRetryCount + 1;
                var level = retryCount < CriticalRetryThreshold ? LogLevel.Warning : LogLevel.Critical;

                activityAccessor.AddException(ex);

                activityAccessor.AddEvent(
                    "Retry Temporal Outbox Message",
                    "retry_temporal_outbox_message",
                    new Dictionary<string, string>
                    {
                        [nameof(TemporalOutbox.LastRetry)] = timeProvider.GetUtcNow().ToString("O"), [nameof(LogLevel)] = level.ToString("G")
                    });

                logger.Log(
                    level,
                    ex,
                    "Failed to execute workflow {WorkflowType}. Setting retry count to {RetryCount} and last retry to {LastRetry}",
                    outboxEvent.WorkflowType,
                    retryCount,
                    timeProvider.GetUtcNow());

                // Failed starts are returned to the retry flow rather than being lost.
                await FailOutboxEventAsync(outboxEvent.Id, outboxEvent.LeasedUntil, outboxEvent.OriginalRetryCount, ex, cancellationToken);
            }
        }
    }

    /// <summary>
    ///     Atomically claims a batch of eligible rows.
    ///     This is the only place where the explicit transaction is required because
    ///     SKIP LOCKED needs a transaction boundary to make the claim safe across workers.
    /// </summary>
    private async Task<List<ClaimedOutboxEvent>> TryClaimOutboxEventsAsync(CancellationToken cancellationToken)
    {
        await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);
        await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
        var thresholdTime = timeProvider.GetUtcNow() - _retryTime;
        var outboxEvents = await dbContext.TemporalOutbox
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

        // Materialize the minimum detached payload needed for the remote call. The claimed
        // DbContext is disposed immediately after the claim transaction commits.
        var now = timeProvider.GetUtcNow();
        var leasedUntil = now.Add(_processingLeaseTime);
        var claimed = outboxEvents
            .Select(outboxEvent => new ClaimedOutboxEvent(
                outboxEvent.Id,
                outboxEvent.WorkflowType,
                outboxEvent.ExecutionArgs,
                outboxEvent.WorkflowOptions,
                outboxEvent.RetryCount,
                leasedUntil))
            .ToList();

        foreach (var outboxEvent in outboxEvents)
        {
            // Claimed rows are leased, not completed. If the worker dies mid-flight,
            // another worker can reclaim them once the lease window expires.
            outboxEvent.RetryCount = Math.Max(1, outboxEvent.RetryCount);
            outboxEvent.LastRetry = leasedUntil;
            outboxEvent.ProcessingErrors = null;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return claimed;
    }

    /// <summary>
    ///     Removes a successfully processed row.
    ///     No explicit transaction is needed for this single-row finalize write.
    /// </summary>
    private async Task CompleteOutboxEventAsync(string id, DateTimeOffset leasedUntil, CancellationToken cancellationToken)
    {
        await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);
        var outboxEvent = await dbContext.TemporalOutbox.FirstOrDefaultAsync(
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
    ///     Restores retry metadata after a failed workflow-start attempt.
    ///     LastRetry is reset to "now" so the usual retry eligibility rule controls the next attempt.
    /// </summary>
    private async Task FailOutboxEventAsync(
        string id,
        DateTimeOffset leasedUntil,
        int originalRetryCount,
        Exception exception,
        CancellationToken cancellationToken)
    {
        await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);
        var outboxEvent = await dbContext.TemporalOutbox.FirstOrDefaultAsync(
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

    // Detached processing payload used outside the claim transaction.
    private sealed record ClaimedOutboxEvent(
        string Id,
        string WorkflowType,
        string? ExecutionArgs,
        WorkflowOptions WorkflowOptions,
        int OriginalRetryCount,
        DateTimeOffset LeasedUntil);
}
