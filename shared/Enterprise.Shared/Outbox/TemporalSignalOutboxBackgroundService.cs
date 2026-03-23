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

public class TemporalSignalOutboxBackgroundService<TDbContext>(
    IDbContextFactory<TDbContext> contextFactory,
    IActivityAccessor activityAccessor,
    ILogger<TemporalSignalOutboxBackgroundService<TDbContext>> logger,
    TimeProvider timeProvider,
    IActivityPropagator<IDictionary<string, string>> dictionaryActivityPropagator,
    IServiceProvider serviceProvider)
    : BackgroundService where TDbContext : DbContext, ITemporalSignalOutboxStore
{
    private const int CriticalRetryThreshold = 5;

    private static readonly Func<TDbContext, DateTimeOffset, CancellationToken, Task<TemporalSignalOutbox?>>
        s_getOutboxItemQueryAsync =
            EF.CompileAsyncQuery<TDbContext, DateTimeOffset, TemporalSignalOutbox?>((dbContext, thresholdRetryTime, cancellationToken) =>
                dbContext.TemporalSignalOutbox
                    .TagWith(EntityFrameworkInterceptorTags.ForUpdateSkipLocked)
                    .OrderBy(query => query.RetryCount)
                    .FirstOrDefault(query => query.RetryCount == 0 || query.LastRetry < thresholdRetryTime));

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

            var activitySource = activityAccessor.GetActivitySource(TelemetryKeys.TemporalSignalActivitySourceName);

            using (dictionaryActivityPropagator.StartActivityFromPropagationContext(
                       new Dictionary<string, string>(),
                       activitySource,
                       TelemetryKeys.TemporalSignalEventSend,
                       ActivityKind.Producer))
            {
                try
                {
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
                    dbContext.Remove(outboxEvent);
                }
                catch (Exception ex)
                {
                    outboxEvent.RetryCount += 1;
                    outboxEvent.LastRetry = timeProvider.GetUtcNow();
                    outboxEvent.ProcessingErrors = ex.ToString().Truncate(Constants.MaxOutboxProcessingErrorsLength);

                    var level = outboxEvent.RetryCount < CriticalRetryThreshold ? LogLevel.Warning : LogLevel.Critical;

                    activityAccessor.AddException(ex);

                    activityAccessor.AddEvent(
                        "Retry Temporal signal Outbox Message",
                        "retry_temporal_signal_outbox_message",
                        new Dictionary<string, string>
                        {
                            [nameof(outboxEvent.LastRetry)] = outboxEvent.LastRetry?.ToString("O")!, [nameof(LogLevel)] = level.ToString("G")
                        });

                    logger.Log(
                        level,
                        ex,
                        "Failed to signal {SignalType}. Setting retry count to {RetryCount} and last retry to {LastRetry}",
                        outboxEvent.SignalType,
                        outboxEvent.RetryCount,
                        outboxEvent.LastRetry);
                }

                await dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
        }
    }
}
