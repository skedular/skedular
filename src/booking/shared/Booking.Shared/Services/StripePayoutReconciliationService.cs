using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Microsoft.Extensions.Logging;
using Stripe;
using StripeCheckoutSession = Booking.Shared.Database.Entities.StripeCheckoutSession;

namespace Booking.Shared.Services;

public interface IStripePayoutReconciliationService
{
    Task HandlePaidAsync(
        Payout payout,
        string? stripeAccountId,
        CancellationToken cancellationToken,
        DateTimeOffset? eventCreatedAt = null,
        string? correlationId = null);

    Task HandleStateChangedAsync(
        Payout payout,
        string eventType,
        CancellationToken cancellationToken,
        string? stripeAccountId = null,
        DateTimeOffset? eventCreatedAt = null,
        string? correlationId = null);

    Task RetryUnmatchedAsync(CancellationToken cancellationToken);
}

public sealed class StripePayoutReconciliationService(
    IRepositoryFactory repositoryFactory,
    IStripeHostRefundClient stripeClient,
    IMarketplaceRefundTransitionService refundTransitionService,
    TimeProvider timeProvider,
    ILogger<StripePayoutReconciliationService> logger) : IStripePayoutReconciliationService
{
    private const int AutomaticRetryBatchSize = 100;
    private const int MaximumAutomaticRetries = 3;

    public async Task HandlePaidAsync(
        Payout payout,
        string? stripeAccountId,
        CancellationToken cancellationToken,
        DateTimeOffset? eventCreatedAt = null,
        string? correlationId = null)
    {
        var resolution = await ResolveCheckoutAsync(payout, stripeAccountId, cancellationToken);
        if (resolution.Checkouts.Count == 0)
        {
            await RecordUnmatchedAsync(payout, resolution.Reason, stripeAccountId, eventCreatedAt, cancellationToken);
            return;
        }

        foreach (var checkout in resolution.Checkouts)
        {
            checkout.PayoutId = payout.Id;
            checkout.PayoutStatus = payout.Status;
            checkout.PayoutDisbursedAt = eventCreatedAt ?? timeProvider.GetUtcNow();
            repositoryFactory.StripeCheckoutSessionRepository.Update(checkout);
            await SynchronizeRefundPayoutContextAsync(checkout, true, cancellationToken);
        }

        await MarkReconciliationResolvedAsync(payout.Id, cancellationToken);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RetryUnmatchedAsync(CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        var reconciliations = await repositoryFactory.MarketplaceRefundRepository
            .GetOpenStripePayoutReconciliationsAsync(now, AutomaticRetryBatchSize, cancellationToken);
        foreach (var reconciliation in reconciliations)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var attempt = reconciliation.RetryCount + 1;
            try
            {
                var payout = await stripeClient.GetPayoutAsync(
                    reconciliation.ExternalRefundId, reconciliation.StripeAccountId!, cancellationToken);
                if (string.Equals(payout.Status, "paid", StringComparison.OrdinalIgnoreCase))
                {
                    await HandlePaidAsync(
                        payout,
                        reconciliation.StripeAccountId,
                        cancellationToken,
                        correlationId: payout.Id);
                }
                else
                {
                    await HandleStateChangedAsync(
                        payout,
                        GetPayoutStateEventType(payout.Status),
                        cancellationToken,
                        reconciliation.StripeAccountId,
                        correlationId: payout.Id);
                }

                await RecordRetryOutcomeAsync(reconciliation.ExternalRefundId, attempt, now, null, cancellationToken);
            }
            catch (Exception exception)
            {
                await RecordRetryOutcomeAsync(reconciliation.ExternalRefundId, attempt, now, exception.Message, cancellationToken);
                logger.LogWarning(
                    exception,
                    "Stripe payout reconciliation retry {Attempt}/{MaximumAttempts} failed for payout {PayoutId}",
                    attempt,
                    MaximumAutomaticRetries,
                    reconciliation.ExternalRefundId);
            }
        }
    }

    public async Task HandleStateChangedAsync(
        Payout payout,
        string eventType,
        CancellationToken cancellationToken,
        string? stripeAccountId = null,
        DateTimeOffset? eventCreatedAt = null,
        string? correlationId = null)
    {
        correlationId ??= payout.Id;
        var checkout = await repositoryFactory.StripeCheckoutSessionRepository.GetByPayoutIdAsync(payout.Id, cancellationToken);
        if (checkout is null)
        {
            var resolution = await ResolveCheckoutAsync(payout, stripeAccountId, cancellationToken);
            checkout = resolution.Checkouts.SingleOrDefault();
            if (checkout is null)
            {
                await RecordUnmatchedAsync(
                    payout,
                    $"Unmatched Stripe payout state event: {eventType}; status={payout.Status}. {resolution.Reason}",
                    stripeAccountId,
                    eventCreatedAt,
                    cancellationToken);
                return;
            }
        }

        checkout.PayoutId = payout.Id;
        checkout.PayoutStatus = payout.Status ?? eventType;
        checkout.PayoutFailureMessage = payout.FailureMessage;
        if (eventType == "payout.updated" && payout.Status == "paid")
        {
            checkout.PayoutDisbursedAt ??= eventCreatedAt ?? timeProvider.GetUtcNow();
            repositoryFactory.StripeCheckoutSessionRepository.Update(checkout);
            await SynchronizeRefundPayoutContextAsync(checkout, true, cancellationToken);
            await MarkReconciliationResolvedAsync(payout.Id, cancellationToken);
            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            return;
        }

        var terminalFailure = payout.Status is "failed" or "canceled" || eventType is "payout.failed" or "payout.canceled";
        if (terminalFailure)
        {
            checkout.PayoutDisbursedAt = null;
        }

        repositoryFactory.StripeCheckoutSessionRepository.Update(checkout);
        if (terminalFailure)
        {
            var refunds = await repositoryFactory.MarketplaceRefundRepository.GetByStripePaymentContextAsync(
                checkout.TransferId, checkout.ChargeId, checkout.PaymentIntentId, cancellationToken);
            foreach (var refund in refunds.Where(item => item.Status is MarketplaceRefundStatusConstants.Processing
                         or MarketplaceRefundStatusConstants.ProviderPending
                         or MarketplaceRefundStatusConstants.Failed
                         or MarketplaceRefundStatusConstants.Completed))
            {
                var error = "The Stripe payout failed or was canceled after refund processing context was recorded.";
                refund.PaymentRefundLastError = error;
                await refundTransitionService.TransitionAsync(
                    refund,
                    MarketplaceRefundStatusConstants.ReconciliationRequired,
                    error,
                    null,
                    correlationId,
                    cancellationToken);
            }

            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            return;
        }

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task RecordRetryOutcomeAsync(
        string payoutId,
        int retryCount,
        DateTimeOffset now,
        string? error,
        CancellationToken cancellationToken)
    {
        var reconciliation = await repositoryFactory.MarketplaceRefundRepository
            .GetExternalReconciliationAsync(MarketplaceExternalRefundReconciliationProviderConstants.StripePayout, payoutId, null, cancellationToken);
        if (reconciliation is null || reconciliation.Status != MarketplaceExternalRefundReconciliationStatusConstants.Open)
        {
            return;
        }

        reconciliation.RetryCount = retryCount;
        reconciliation.NextRetryAt = retryCount >= MaximumAutomaticRetries
            ? null
            : now + GetRetryDelay(retryCount);
        reconciliation.ResolutionReason = retryCount >= MaximumAutomaticRetries
            ? $"Automatic Stripe payout reconciliation stopped after {retryCount} attempts. Manual review is required."
            : error is null
                ? $"Stripe payout remains unmatched after retry {retryCount}; it will be retried after {reconciliation.NextRetryAt:O}."
                : $"Stripe payout retry {retryCount} failed: {error}";
        repositoryFactory.MarketplaceRefundRepository.UpdateExternalReconciliation(reconciliation);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static TimeSpan GetRetryDelay(int retryCount) => retryCount switch
    {
        1 => TimeSpan.FromHours(1),
        2 => TimeSpan.FromHours(6),
        _ => TimeSpan.FromHours(24)
    };

    private static string GetPayoutStateEventType(string? payoutStatus)
    {
        if (string.Equals(payoutStatus, "failed", StringComparison.OrdinalIgnoreCase))
        {
            return "payout.failed";
        }

        if (string.Equals(payoutStatus, "canceled", StringComparison.OrdinalIgnoreCase))
        {
            return "payout.canceled";
        }

        return "payout.updated";
    }

    private async Task RecordUnmatchedAsync(
        Payout payout,
        string reason,
        string? stripeAccountId,
        DateTimeOffset? eventCreatedAt,
        CancellationToken cancellationToken)
    {
        var existing = await repositoryFactory.MarketplaceRefundRepository.GetExternalReconciliationAsync(
            MarketplaceExternalRefundReconciliationProviderConstants.StripePayout, payout.Id, null,
            cancellationToken);
        if (existing is null)
        {
            var organizationId = await ResolveOrganizationIdAsync(stripeAccountId, cancellationToken);
            repositoryFactory.MarketplaceRefundRepository.AddExternalReconciliation(new MarketplaceExternalRefundReconciliation
            {
                OrganizationId = organizationId,
                StripeAccountId = stripeAccountId,
                Provider = MarketplaceExternalRefundReconciliationProviderConstants.StripePayout,
                ExternalRefundId = payout.Id,
                Currency = payout.Currency,
                Status = MarketplaceExternalRefundReconciliationStatusConstants.Open,
                ResolutionReason = reason
            });
        }
        else
        {
            existing.Status = MarketplaceExternalRefundReconciliationStatusConstants.Open;
            existing.LastSeenAt = eventCreatedAt ?? timeProvider.GetUtcNow();
            existing.ResolutionReason = reason;
            if (existing.OrganizationId is null && !string.IsNullOrWhiteSpace(stripeAccountId))
            {
                existing.OrganizationId = await ResolveOrganizationIdAsync(stripeAccountId, cancellationToken);
            }

            existing.StripeAccountId ??= stripeAccountId;
            repositoryFactory.MarketplaceRefundRepository.UpdateExternalReconciliation(existing);
        }

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task SynchronizeRefundPayoutContextAsync(
        StripeCheckoutSession checkout,
        bool payoutDisbursed,
        CancellationToken cancellationToken)
    {
        var refunds = await repositoryFactory.MarketplaceRefundRepository.GetByStripePaymentContextAsync(
            checkout.TransferId,
            checkout.ChargeId,
            checkout.PaymentIntentId,
            cancellationToken);
        foreach (var refund in refunds)
        {
            refund.StripeAccountId = string.Equals(checkout.ChargeType, "Direct", StringComparison.OrdinalIgnoreCase)
                ? checkout.StripeAccountId
                : null;
            refund.StripeChargeType = checkout.ChargeType;
            refund.StripeTransferId = checkout.TransferId;
            refund.StripeChargeId = checkout.ChargeId;
            refund.StripePaymentIntentId = checkout.PaymentIntentId;
            refund.PostPayoutRefund = payoutDisbursed &&
                                      string.Equals(checkout.ChargeType, "Destination", StringComparison.OrdinalIgnoreCase);
            repositoryFactory.MarketplaceRefundRepository.Update(refund);
        }
    }

    private async Task<PayoutCheckoutResolution> ResolveCheckoutAsync(
        Payout payout,
        string? stripeAccountId,
        CancellationToken cancellationToken)
    {
        var paymentIntentId = payout.Metadata?.GetValueOrDefault("payment_intent_id");
        var transferId = payout.Metadata?.GetValueOrDefault("transfer_id");
        var hasPaymentIntentId = !string.IsNullOrWhiteSpace(paymentIntentId);
        var hasTransferId = !string.IsNullOrWhiteSpace(transferId);

        if (hasPaymentIntentId && hasTransferId)
        {
            var paymentIntentCheckout = await repositoryFactory.StripeCheckoutSessionRepository
                .GetByPaymentIntentIdAsync(paymentIntentId!, cancellationToken);
            var transferCheckout = await repositoryFactory.StripeCheckoutSessionRepository
                .GetByTransferIdAsync(transferId!, cancellationToken);
            return paymentIntentCheckout is not null && transferCheckout is not null && paymentIntentCheckout.Id == transferCheckout.Id
                ? new PayoutCheckoutResolution([paymentIntentCheckout], "")
                : new PayoutCheckoutResolution(
                    [],
                    "Stripe payout metadata contains conflicting or incomplete payment-intent and transfer correlations.");
        }

        if (hasPaymentIntentId || hasTransferId)
        {
            var checkout = hasPaymentIntentId
                ? await repositoryFactory.StripeCheckoutSessionRepository.GetByPaymentIntentIdAsync(paymentIntentId!, cancellationToken)
                : await repositoryFactory.StripeCheckoutSessionRepository.GetByTransferIdAsync(transferId!, cancellationToken);
            return checkout is null
                ? new PayoutCheckoutResolution([], "Stripe payout metadata correlation did not resolve to a checkout session.")
                : new PayoutCheckoutResolution([checkout], "");
        }

        if (string.IsNullOrWhiteSpace(stripeAccountId))
        {
            return new PayoutCheckoutResolution([], "Stripe payout has no authoritative payment-intent or transfer correlation.");
        }

        var transactions = await stripeClient.GetPayoutBalanceTransactionsAsync(payout.Id, stripeAccountId, cancellationToken);
        var transactionSourceIds = transactions
            .Select(transaction => transaction.SourceId ?? transaction.Source?.Id)
            .Where(sourceId => !string.IsNullOrWhiteSpace(sourceId))
            .Select(sourceId => sourceId!)
            .Distinct(StringComparer.Ordinal)
            .ToArray();
        var candidates = await repositoryFactory.StripeCheckoutSessionRepository
            .GetDestinationChargeCandidatesAsync(stripeAccountId, transactionSourceIds, cancellationToken);
        var matches = candidates.Where(checkout => transactionSourceIds.Any(sourceId =>
            sourceId == checkout.ChargeId || sourceId == checkout.TransferId)).ToArray();
        return matches.Length > 0
            ? new PayoutCheckoutResolution(matches, "")
            : new PayoutCheckoutResolution([], "Stripe payout balance-transaction correlation did not resolve to a checkout session.");
    }

    private async Task MarkReconciliationResolvedAsync(string payoutId, CancellationToken cancellationToken)
    {
        var reconciliation = await repositoryFactory.MarketplaceRefundRepository
            .GetExternalReconciliationAsync(MarketplaceExternalRefundReconciliationProviderConstants.StripePayout, payoutId, null, cancellationToken);
        if (reconciliation is null)
        {
            return;
        }

        reconciliation.Status = "Resolved";
        reconciliation.LastSeenAt = timeProvider.GetUtcNow();
        reconciliation.ResolutionReason = "Stripe payout matched after automatic retry.";
        repositoryFactory.MarketplaceRefundRepository.UpdateExternalReconciliation(reconciliation);
    }

    private async Task<string?> ResolveOrganizationIdAsync(string? stripeAccountId, CancellationToken cancellationToken) =>
        string.IsNullOrWhiteSpace(stripeAccountId)
            ? null
            : await repositoryFactory.StripeCustomerRepository.GetOrganizationIdByStripeAccountIdAsync(stripeAccountId, cancellationToken);

    private sealed record PayoutCheckoutResolution(
        IReadOnlyList<StripeCheckoutSession> Checkouts,
        string Reason);
}
