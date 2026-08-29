using Booking.Shared.Services;

namespace Booking.Jobs.Services;

/// <summary>
///     Hosted service that runs a daily reconciliation batch to compare local refund
///     records against the Stripe provider state. Refunds stuck in ProviderPending or
///     Processing beyond the configured threshold are transitioned to
///     ReconciliationRequired for human review.
/// </summary>
public class MarketplaceRefundReconciliationHostedService(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<MarketplaceRefundReconciliationHostedService> logger,
    TimeProvider timeProvider) : IHostedService, IDisposable
{
    private readonly SemaphoreSlim _runLock = new(1, 1);
    private readonly CancellationTokenSource _stopping = new();
    private Timer? _timer;

    public void Dispose()
    {
        _timer?.Dispose();
        _stopping.Cancel();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Schedule daily at 02:00 UTC. DateTimeOffset.Date loses the UTC offset and
        // causes Timer to receive a negative delay on hosts outside UTC.
        var now = timeProvider.GetUtcNow();
        var nextRun = new DateTimeOffset(now.Year, now.Month, now.Day, 2, 0, 0, TimeSpan.Zero);
        if (now >= nextRun)
        {
            nextRun = nextRun.AddDays(1);
        }

        var initialDelay = nextRun - now;

        _timer = new Timer(RunReconciliation, null, initialDelay, TimeSpan.FromDays(1));
        logger.LogInformation(
            "Refund reconciliation batch scheduled. First run at {NextRun:O}",
            nextRun);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _stopping.Cancel();
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    private async void RunReconciliation(object? state)
    {
        if (!await _runLock.WaitAsync(0))
        {
            logger.LogWarning("Skipped refund reconciliation because the previous batch is still running");
            return;
        }

        try
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();
            var service = scope.ServiceProvider.GetRequiredService<IMarketplaceRefundReconciliationService>();
            var operations = scope.ServiceProvider.GetRequiredService<IMarketplaceRefundOperationsService>();
            var payoutReconciliation = scope.ServiceProvider.GetRequiredService<IStripePayoutReconciliationService>();
            var cleanup = scope.ServiceProvider.GetRequiredService<IMarketplaceBookingCleanupReconciliationService>();
            var accountingCleanup = scope.ServiceProvider.GetRequiredService<IMarketplaceBookingAccountingCleanupService>();
            try
            {
                await payoutReconciliation.RetryUnmatchedAsync(_stopping.Token);
                await service.ReconcileAsync(_stopping.Token);
            }
            catch (OperationCanceledException) when (_stopping.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Provider refund reconciliation failed");
            }

            try
            {
                await cleanup.ReconcileAsync(_stopping.Token);
            }
            catch (OperationCanceledException) when (_stopping.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Marketplace booking cleanup reconciliation failed");
            }

            try
            {
                await accountingCleanup.ReconcileAsync(_stopping.Token);
            }
            catch (OperationCanceledException) when (_stopping.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Marketplace accounting cleanup reconciliation failed");
            }

            await operations.LogOverdueBankTransferRefundsAsync(timeProvider.GetUtcNow(), _stopping.Token);
            await operations.RecordDashboardMetricsAsync(timeProvider.GetUtcNow(), _stopping.Token);
            logger.LogInformation("Refund reconciliation batch completed");
        }
        catch (Exception exception)
        {
            if (!_stopping.IsCancellationRequested)
            {
                logger.LogError(exception, "Refund reconciliation batch failed");
            }
        }
        finally
        {
            _runLock.Release();
        }
    }
}
