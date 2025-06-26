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

public class TemporalOutboxBackgroundService<TDbContext>(
    IDbContextFactory<TDbContext> contextFactory,
    IActivityAccessor activityAccessor,
    ILogger<TemporalOutboxBackgroundService<TDbContext>> logger,
    IActivityPropagator<IDictionary<string, string>> dictionaryActivityPropagator,
    IServiceProvider serviceProvider)
    : BackgroundService where TDbContext : DbContext, ITemporalOutboxStore
{
    private const int CriticalRetryThreshold = 5;

    private static readonly Func<TDbContext, DateTimeOffset, CancellationToken, Task<TemporalOutbox?>>
        s_getOutboxItemQueryAsync =
            EF.CompileAsyncQuery<TDbContext, DateTimeOffset, TemporalOutbox?>((dbContext, thresholdRetryTime, cancellationToken) =>
                dbContext.TemporalOutbox
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
            var thresholdTime = DateTimeOffset.UtcNow - retryTime;
            var outboxEvent = await s_getOutboxItemQueryAsync(dbContext, thresholdTime, cancellationToken);
            if (outboxEvent is null)
            {
                await Task.Delay(_retryTime, cancellationToken);

                continue;
            }

            var activitySource = activityAccessor.GetActivitySource(TelemetryKeys.TemporalActivitySourceName);

            using (dictionaryActivityPropagator.StartActivityFromPropagationContext(
                       new Dictionary<string, string>(),
                       activitySource,
                       TelemetryKeys.TemporalEventSend,
                       ActivityKind.Producer))
            {
                try
                {
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
                        "Retry Temporal Outbox Message",
                        "retry_temporal_outbox_message",
                        new Dictionary<string, string>
                        {
                            [nameof(outboxEvent.LastRetry)] = outboxEvent.LastRetry?.ToString("O")!, [nameof(LogLevel)] = level.ToString("G")
                        });

                    logger.Log(
                        level,
                        ex,
                        "Failed to execute workflow {WorkflowType}. Setting retry count to {RetryCount} and last retry to {LastRetry}",
                        outboxEvent.WorkflowType,
                        outboxEvent.RetryCount,
                        outboxEvent.LastRetry);
                }

                await dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
        }
    }
}
