using System.Diagnostics.Metrics;
using System.Net;
using Api.Shared.Services;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Microsoft.Extensions.Logging;
using Stripe;

namespace Booking.Shared.Services;

public interface IMarketplaceRefundReconciliationService
{
    Task ReconcileAsync(CancellationToken cancellationToken);
}

public class MarketplaceRefundReconciliationService(
    IRepositoryFactory repositoryFactory,
    IStripeHostRefundService stripeHostRefundService,
    IXeroRefundService xeroRefundService,
    IMarketplaceRefundTransitionService refundTransitionService,
    TimeProvider timeProvider,
    ILogger<MarketplaceRefundReconciliationService> logger) : IMarketplaceRefundReconciliationService
{
    private const int ReconciliationBatchSize = 100;
    private static readonly TimeSpan LeaseDuration = TimeSpan.FromMinutes(10);
    private static readonly Meter Meter = new("Skedular.Booking.Refunds", "1.0");

    private static readonly Counter<long> ReconciliationResults = Meter.CreateCounter<long>("refund.reconciliation.result");

    // Refunds pending for longer than 4 hours without a webhook update are eligible for reconciliation
    private static readonly TimeSpan ReconciliationThreshold = TimeSpan.FromHours(4);

    public async Task ReconcileAsync(CancellationToken cancellationToken)
    {
        var threshold = timeProvider.GetUtcNow() - ReconciliationThreshold;
        var refundsForReconciliation = await repositoryFactory.MarketplaceRefundRepository
            .GetRefundsForReconciliationAsync(threshold, ReconciliationBatchSize, cancellationToken);

        logger.LogInformation(
            "Refund reconciliation started. Found {Count} refunds eligible for reconciliation",
            refundsForReconciliation.Count);

        var matched = 0;
        var mismatched = 0;
        var lookupFailed = 0;

        foreach (var refund in refundsForReconciliation)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var workerId = $"refund-reconciliation:{Environment.MachineName}:{Environment.ProcessId}";
            if (!await repositoryFactory.MarketplaceRefundRepository.TryClaimReconciliationAsync(
                    refund.Id, workerId, timeProvider.GetUtcNow(), LeaseDuration, cancellationToken))
            {
                logger.LogDebug("Skipping refund {RefundId}; another reconciliation worker owns the lease", refund.Id);
                continue;
            }

            try
            {
                await repositoryFactory.MarketplaceRefundRepository.RenewReconciliationLeaseAsync(
                    refund.Id, workerId, timeProvider.GetUtcNow(), LeaseDuration, cancellationToken);
                if (refund.AccountingProvider == AccountingProviderConstants.Xero && !string.IsNullOrWhiteSpace(refund.ExternalRefundId))
                {
                    var found = false;
                    await ExecuteWithLeaseRenewalAsync(
                        refund.Id,
                        workerId,
                        () => xeroRefundService.ReconcileAsync(refund, refund.LastReconciledAt ?? refund.RequestedAt, cancellationToken),
                        value => found = value,
                        cancellationToken);
                    if (!found && refund.Status != MarketplaceRefundStatusConstants.Completed)
                    {
                        await refundTransitionService.TransitionAsync(
                            refund,
                            MarketplaceRefundStatusConstants.ReconciliationRequired,
                            "The Xero refund could not be verified.",
                            null,
                            refund.ExternalRefundId ?? refund.Id,
                            cancellationToken);
                    }
                }
                else if (refund.PaymentProvider == "STRIPE" && !string.IsNullOrWhiteSpace(refund.ExternalPaymentRefundId))
                {
                    (int matched, int mismatched, int lookupFailed) result = default;
                    await ExecuteWithLeaseRenewalAsync(
                        refund.Id,
                        workerId,
                        () => ReconcileStripeRefundAsync(refund, cancellationToken),
                        value => result = value,
                        cancellationToken);
                    matched += result.matched;
                    mismatched += result.mismatched;
                    lookupFailed += result.lookupFailed;
                }

                refund.ReconciledAt = timeProvider.GetUtcNow();
                ReconciliationResults.Add(1, new KeyValuePair<string, object?>("provider", refund.PaymentProvider ?? "unknown"),
                    new KeyValuePair<string, object?>("status", refund.ReconciliationStatus ?? "unknown"),
                    new KeyValuePair<string, object?>("organization.id", refund.OrganizationId ?? "unknown"));
                repositoryFactory.MarketplaceRefundRepository.Update(refund);
                await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
                await repositoryFactory.MarketplaceRefundRepository.ReleaseReconciliationLeaseAsync(refund.Id, workerId, cancellationToken);
            }
            catch
            {
                await repositoryFactory.MarketplaceRefundRepository.ReleaseReconciliationLeaseAsync(refund.Id, workerId, cancellationToken);
                throw;
            }
        }

        logger.LogInformation(
            "Refund reconciliation complete. Matched={Matched} Mismatched={Mismatched} LookupFailed={LookupFailed}",
            matched, mismatched, lookupFailed);
    }

    private async Task ExecuteWithLeaseRenewalAsync<T>(
        string refundId,
        string workerId,
        Func<Task<T>> operation,
        Action<T> assignResult,
        CancellationToken cancellationToken)
    {
        using var renewalCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var renewalTask = RenewLeaseUntilCompleteAsync(refundId, workerId, renewalCancellation.Token);
        try
        {
            assignResult(await operation());
        }
        finally
        {
            await renewalCancellation.CancelAsync();
            await renewalTask;
        }
    }

    private async Task RenewLeaseUntilCompleteAsync(string refundId, string workerId, CancellationToken cancellationToken)
    {
        try
        {
            while (true)
            {
                await Task.Delay(LeaseDuration / 2, cancellationToken);
                await repositoryFactory.MarketplaceRefundRepository.RenewReconciliationLeaseAsync(
                    refundId, workerId, timeProvider.GetUtcNow(), LeaseDuration, cancellationToken);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
    }

    private async Task<(int matched, int mismatched, int lookupFailed)> ReconcileStripeRefundAsync(
        MarketplaceRefund refund,
        CancellationToken cancellationToken)
    {
        var matched = 0;
        var mismatched = 0;
        var lookupFailed = 0;
        try
        {
            var stripeRefund = await stripeHostRefundService.GetProviderRefundAsync(
                refund.ExternalPaymentRefundId!,
                refund.StripeAccountId,
                cancellationToken);
            var previousStatus = refund.Status;
            var reconciled = await stripeHostRefundService.ReconcileAsync(
                stripeRefund,
                cancellationToken,
                refund.StripeAccountId);

            if (reconciled?.ReconciliationStatus == "UnknownStripeRefundPath")
            {
                mismatched++;
                refund.ReconciliationStatus = "UnknownStripeRefundPath";
                logger.LogWarning(
                    "Refund {RefundId} remains unresolved because Stripe refund path context is unknown",
                    refund.Id);
            }
            else if (reconciled is not null && reconciled.Status != previousStatus)
            {
                mismatched++;
                refund.ReconciliationStatus = "Resolved";
                logger.LogInformation(
                    "Refund {RefundId} reconciled: {OldStatus} → {NewStatus}",
                    refund.Id, previousStatus, reconciled.Status);
            }
            else
            {
                matched++;
                refund.ReconciliationStatus = "Matched";
            }
        }
        catch (StripeException exception) when (exception.HttpStatusCode == HttpStatusCode.NotFound)
        {
            refund.ReconciliationStatus = "LookupFailed";
            await refundTransitionService.TransitionAsync(
                refund,
                MarketplaceRefundStatusConstants.ReconciliationRequired,
                "Stripe refund could not be retrieved during reconciliation.",
                null,
                refund.ExternalPaymentRefundId ?? refund.Id,
                cancellationToken);
            lookupFailed++;
            logger.LogError(exception, "Failed to reconcile Stripe refund {RefundId}", refund.Id);
        }
        catch (Exception exception)
        {
            refund.ReconciliationStatus = "RetryPending";
            refund.LastError = "Stripe refund reconciliation will be retried.";
            repositoryFactory.MarketplaceRefundRepository.Update(refund);
            logger.LogWarning(exception, "Transient failure reconciling Stripe refund {RefundId}", refund.Id);
        }

        return (matched, mismatched, lookupFailed);
    }
}
