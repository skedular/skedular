using System.Diagnostics;
using System.Net;
using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Microsoft.Extensions.Logging;
using Stripe;
using Stripe.Checkout;
using StripeInvoice = Stripe.Invoice;
using StripeCheckoutSessionEntity = Booking.Shared.Database.Entities.StripeCheckoutSession;
using MarketplaceRefundEntityTypeConstants = Booking.Shared.Models.MarketplaceRefundEntityTypeConstants;
using MarketplaceRefundStatusConstants = Booking.Shared.Models.MarketplaceRefundStatusConstants;
using MarketplaceExternalRefundReconciliationStatusConstants = Booking.Shared.Models.MarketplaceExternalRefundReconciliationStatusConstants;

namespace Booking.Shared.Services;

public interface IStripeHostRefundService
{
    Task<bool> IsHostRefundAsync(MarketplaceRefund refund, CancellationToken cancellationToken);
    Task<bool> CanProcessAsync(MarketplaceRefund refund, CancellationToken cancellationToken);
    Task<MarketplaceRefund> ProcessAsync(MarketplaceRefund refund, CancellationToken cancellationToken);

    Task<MarketplaceRefund?> ReconcileAsync(
        Refund stripeRefund,
        CancellationToken cancellationToken,
        string? stripeAccountId = null,
        string? correlationId = null);

    Task<Refund> GetProviderRefundAsync(string refundId, string? stripeAccountId, CancellationToken cancellationToken);
}

public class StripeHostRefundService(
    IRepositoryFactory repositoryFactory,
    IStripeHostRefundClient stripeClient,
    IMarketplaceRefundTransitionService refundTransitionService,
    TimeProvider timeProvider,
    IRetrievable<Session, SessionGetOptions> checkoutSessionRetriever,
    SubscriptionService subscriptionService,
    IRetrievable<StripeInvoice, InvoiceGetOptions> invoiceRetriever,
    ILogger<StripeHostRefundService> logger) : IStripeHostRefundService
{
    /// <summary>
    ///     Returns true for any Stripe-backed refund — Host bookings and Spaces bookings
    ///     paid via Stripe Connect (have a StripeCheckoutSession).
    /// </summary>
    public async Task<bool> IsHostRefundAsync(MarketplaceRefund refund, CancellationToken cancellationToken) =>
        HasPersistedStripeContext(refund) ||
        await ResolveStripeCheckoutSessionAsync(refund, cancellationToken) is not null;

    public async Task<bool> CanProcessAsync(MarketplaceRefund refund, CancellationToken cancellationToken) =>
        HasPersistedStripeContext(refund) ||
        await ResolveStripeCheckoutSessionAsync(refund, cancellationToken) is not null;

    public async Task<MarketplaceRefund> ProcessAsync(MarketplaceRefund refund, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        logger.LogInformation(
            "Starting Stripe refund processing for refund {RefundId}, amount {RefundAmount}, currency {Currency}, retry count {RetryCount}",
            refund.Id, refund.RefundAmount, refund.Currency, refund.RetryCount);
        var context = GetPersistedStripeContext(refund);
        logger.LogInformation(
            "Resolved Stripe refund context for refund {RefundId}: paymentIntentIdPresent={PaymentIntentIdPresent}, stripeAccountIdPresent={StripeAccountIdPresent}, chargeType={ChargeType}, refundPath={RefundPath}",
            refund.Id,
            !string.IsNullOrWhiteSpace(refund.StripePaymentIntentId),
            !string.IsNullOrWhiteSpace(refund.StripeAccountId),
            refund.StripeChargeType,
            refund.StripeRefundPath);
        if (context is null)
        {
            var checkout = await ResolveStripeCheckoutSessionAsync(refund, cancellationToken) ??
                           throw new InvalidOperationException("The Stripe Checkout session for this Host refund could not be found.");
            if (string.IsNullOrWhiteSpace(checkout.PaymentIntentId))
            {
                var recoveredPaymentIntentId = await RecoverPaymentIntentIdAsync(checkout, cancellationToken);
                if (!string.IsNullOrWhiteSpace(recoveredPaymentIntentId))
                {
                    checkout.PaymentIntentId = recoveredPaymentIntentId;
                    repositoryFactory.StripeCheckoutSessionRepository.Update(checkout);
                    logger.LogInformation(
                        "Recovered Stripe PaymentIntent for refund {RefundId} from the Checkout Session subscription invoice: paymentIntentId={PaymentIntentId}",
                        refund.Id, recoveredPaymentIntentId);
                }
                else
                {
                    logger.LogWarning("Stripe refund {RefundId} cannot start because the Checkout Session has no PaymentIntentId", refund.Id);
                    refund.Status = MarketplaceRefundStatusConstants.ReconciliationRequired;
                    refund.LastError = "The original Stripe charge context is incomplete and requires reconciliation.";
                    return repositoryFactory.MarketplaceRefundRepository.Update(refund);
                }
            }

            PersistStripeContext(refund, checkout);
            if (!IsSupportedChargeType(refund.StripeChargeType))
            {
                logger.LogWarning("Stripe refund {RefundId} cannot start because charge type {ChargeType} is unsupported", refund.Id,
                    refund.StripeChargeType);
                refund.Status = MarketplaceRefundStatusConstants.ReconciliationRequired;
                refund.LastError = "The original Stripe charge type is unknown and requires reconciliation.";
                return repositoryFactory.MarketplaceRefundRepository.Update(refund);
            }

            refund.PostPayoutRefund = string.Equals(refund.StripeChargeType, "Destination", StringComparison.OrdinalIgnoreCase) &&
                                      checkout.PayoutDisbursedAt is not null;
            context = GetPersistedStripeContext(refund);
        }

        if (context is null || string.IsNullOrWhiteSpace(refund.StripePaymentIntentId))
        {
            logger.LogWarning("Stripe refund {RefundId} cannot start because Stripe payment context is incomplete", refund.Id);
            refund.Status = MarketplaceRefundStatusConstants.ReconciliationRequired;
            refund.LastError = "The original Stripe charge context is incomplete and requires reconciliation.";
            return repositoryFactory.MarketplaceRefundRepository.Update(refund);
        }

        if (!IsSupportedChargeType(refund.StripeChargeType))
        {
            logger.LogWarning("Stripe refund {RefundId} cannot start because charge type {ChargeType} is unsupported", refund.Id,
                refund.StripeChargeType);
            refund.Status = MarketplaceRefundStatusConstants.ReconciliationRequired;
            refund.LastError = "The original Stripe charge type is unknown and requires reconciliation.";
            return repositoryFactory.MarketplaceRefundRepository.Update(refund);
        }

        // Direct charges are created on the connected account and must be refunded
        // with that account in RequestOptions. Destination charges are created on
        // the platform; DestinationAccountId is transfer context, not the account
        // on which the Refund API request should be made.
        var providerSubmissionStarted = false;
        try
        {
            var reverseTransfer = refund.PostPayoutRefund;
            var refundApplicationFee = true;
            var isInitialProviderRequest = true;
            Refund stripeRefund;
            while (true)
            {
                try
                {
                    providerSubmissionStarted = true;
                    logger.LogInformation(
                        "Calling Stripe refund API for refund {RefundId}: paymentIntentId={PaymentIntentId}, stripeAccountId={StripeAccountId}, reverseTransfer={ReverseTransfer}, refundApplicationFee={RefundApplicationFee}, amount={Amount}, currency={Currency}",
                        refund.Id,
                        refund.StripePaymentIntentId,
                        refund.StripeAccountId,
                        reverseTransfer,
                        refundApplicationFee,
                        refund.RefundAmount,
                        refund.Currency);
                    stripeRefund = await stripeClient.CreateRefundAsync(
                        new RefundCreateOptions
                        {
                            PaymentIntent = refund.StripePaymentIntentId,
                            Amount = ToMinorUnits(refund.RefundAmount ?? 0m, refund.Currency),
                            RefundApplicationFee = refundApplicationFee,
                            ReverseTransfer = reverseTransfer,
                            Metadata = new Dictionary<string, string>
                            {
                                ["marketplace_refund_id"] = refund.Id,
                            },
                        },
                        GetStripeRefundIdempotencyKey(refund, reverseTransfer, refundApplicationFee, isInitialProviderRequest),
                        cancellationToken,
                        refund.StripeAccountId);
                    logger.LogInformation(
                        "Stripe refund API succeeded for refund {RefundId}: providerRefundId={ProviderRefundId}, providerStatus={ProviderStatus}",
                        refund.Id, stripeRefund.Id, stripeRefund.Status);
                    refund.StripeRefundPath = reverseTransfer
                        ? MarketplaceStripeRefundPathConstants.TransferReversal
                        : MarketplaceStripeRefundPathConstants.PlatformFunded;
                    break;
                }
                catch (StripeException exception) when (refundApplicationFee && IsApplicationFeeUnavailable(exception))
                {
                    logger.LogInformation(
                        "Stripe charge has no application fee for refund {RefundId}; retrying without application-fee reversal",
                        refund.Id);
                    refundApplicationFee = false;
                    isInitialProviderRequest = false;
                }
                catch (StripeException exception) when (reverseTransfer && IsTransferReversalUnavailable(exception))
                {
                    logger.LogWarning(exception,
                        "Stripe transfer reversal unavailable for refund {RefundId}; retrying with platform funds",
                        refund.Id);
                    reverseTransfer = false;
                    isInitialProviderRequest = false;
                }
            }

            refund.PaymentProvider = MarketplaceExternalRefundReconciliationProviderConstants.Stripe;
            refund.ExternalPaymentRefundId = stripeRefund.Id;
            refund.StripeRefundPathSelectedAt = timeProvider.GetUtcNow();
            refund.PaymentRefundStatus = MapStripeStatus(stripeRefund.Status);
            refund.PaymentRefundLastError = null;
            refund.Status = refund.PaymentRefundStatus;
        }
        catch (Exception exception) when (exception is StripeException or InvalidOperationException or ArgumentException)
        {
            refund.PaymentProvider = MarketplaceExternalRefundReconciliationProviderConstants.Stripe;
            refund.PaymentRefundStatus = providerSubmissionStarted && IsAmbiguousProviderOutcome(exception)
                ? MarketplaceRefundStatusConstants.ProviderPending
                : MarketplaceRefundStatusConstants.Failed;
            refund.PaymentRefundLastError = exception.Message;
            refund.Status = refund.PaymentRefundStatus;
            refund.LastError = exception.Message;
            logger.LogError(exception, "Stripe refund processing failed for refund {RefundId} after {DurationMs} ms, retry count {RetryCount}",
                refund.Id, stopwatch.ElapsedMilliseconds, refund.RetryCount);
        }

        refund.PaymentRefundLastProcessedAt = timeProvider.GetUtcNow();
        refund.LastProcessedAt = refund.PaymentRefundLastProcessedAt;
        logger.LogInformation(
            "Completed Stripe refund processing for refund {RefundId} with status {Status}, provider refund {ExternalPaymentRefundId}, duration {DurationMs} ms, retry count {RetryCount}",
            refund.Id, refund.Status, refund.ExternalPaymentRefundId, stopwatch.ElapsedMilliseconds, refund.RetryCount);
        return repositoryFactory.MarketplaceRefundRepository.Update(refund);
    }

    public async Task<MarketplaceRefund?> ReconcileAsync(
        Refund stripeRefund,
        CancellationToken cancellationToken,
        string? stripeAccountId = null,
        string? correlationId = null)
    {
        var refund = await repositoryFactory.MarketplaceRefundRepository.GetByExternalPaymentRefundIdAsync(stripeRefund.Id, cancellationToken);
        if (refund is null && stripeRefund.Metadata.TryGetValue("marketplace_refund_id", out var localRefundId))
        {
            refund = await repositoryFactory.MarketplaceRefundRepository.GetByIdAsync(localRefundId, cancellationToken);
        }

        if (refund is null)
        {
            var externalReconciliation = await repositoryFactory.MarketplaceRefundRepository
                .GetExternalReconciliationAsync(MarketplaceExternalRefundReconciliationProviderConstants.Stripe, stripeRefund.Id, null,
                    cancellationToken);
            if (externalReconciliation is null)
            {
                var organizationId = await ResolveOrganizationIdAsync(stripeAccountId, cancellationToken);
                repositoryFactory.MarketplaceRefundRepository.AddExternalReconciliation(
                    new MarketplaceExternalRefundReconciliation
                    {
                        OrganizationId = organizationId,
                        StripeAccountId = stripeAccountId,
                        Provider = MarketplaceExternalRefundReconciliationProviderConstants.Stripe,
                        ExternalRefundId = stripeRefund.Id,
                        Amount = stripeRefund.Amount / 100m,
                        Currency = stripeRefund.Currency,
                        Status = MarketplaceExternalRefundReconciliationStatusConstants.Open,
                        ResolutionReason = "No matching local refund was found.",
                    });
            }
            else
            {
                externalReconciliation.LastSeenAt = timeProvider.GetUtcNow();
                externalReconciliation.ModifiedAt = externalReconciliation.LastSeenAt;
            }

            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            return null;
        }

        var mappedStatus = MapStripeStatus(stripeRefund.Status);
        var pathIsUnknown = string.IsNullOrWhiteSpace(refund.StripeRefundPath);
        if (pathIsUnknown && string.Equals(refund.StripeChargeType, "Direct", StringComparison.OrdinalIgnoreCase))
        {
            // Direct charges are refunded from the connected account and never use
            // transfer reversal. A webhook can race the local refund save, so derive
            // this deterministic path from the persisted charge context.
            refund.StripeRefundPath = MarketplaceStripeRefundPathConstants.PlatformFunded;
            refund.StripeRefundPathSelectedAt ??= timeProvider.GetUtcNow();
            pathIsUnknown = false;
        }

        var nextStatus = pathIsUnknown && mappedStatus == MarketplaceRefundStatusConstants.Completed
            ? MarketplaceRefundStatusConstants.ReconciliationRequired
            : mappedStatus;
        logger.LogInformation(
            "Received Stripe refund webhook reconciliation: providerRefundId={ExternalPaymentRefundId}, eventCorrelationId={CorrelationId}, localRefundId={RefundId}, mappedStatus={Status}, failureReason={FailureReason}, stripeAccountId={StripeAccountId}",
            stripeRefund.Id, correlationId, refund.Id, nextStatus, stripeRefund.FailureReason, stripeAccountId);
        if (refund.PaymentRefundStatus == nextStatus)
        {
            if (pathIsUnknown)
            {
                refund.ReconciliationStatus = "UnknownStripeRefundPath";
                refund.PaymentRefundLastError = "Stripe refund path context was not persisted and requires reconciliation.";
                refund.LastError = refund.PaymentRefundLastError;
                repositoryFactory.MarketplaceRefundRepository.Update(refund);
                await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
                return refund;
            }

            return null;
        }

        refund.PaymentProvider = MarketplaceExternalRefundReconciliationProviderConstants.Stripe;
        refund.ExternalPaymentRefundId ??= stripeRefund.Id;
        if (pathIsUnknown)
        {
            refund.ReconciliationStatus = "UnknownStripeRefundPath";
            refund.PaymentRefundLastError = "Stripe refund path context was not persisted and requires reconciliation.";
            refund.LastError = refund.PaymentRefundLastError;
        }
        else
        {
            refund.StripeRefundPathSelectedAt ??= timeProvider.GetUtcNow();
        }

        refund.PaymentRefundStatus = nextStatus;
        refund.PaymentRefundLastProcessedAt = timeProvider.GetUtcNow();
        if (!pathIsUnknown)
        {
            refund.PaymentRefundLastError = stripeRefund.FailureReason;
            refund.LastError = stripeRefund.FailureReason;
        }

        return await refundTransitionService.TransitionAsync(
            refund,
            nextStatus,
            refund.LastError,
            null,
            correlationId ?? stripeRefund.Id,
            cancellationToken);
    }

    public Task<Refund> GetProviderRefundAsync(string refundId, string? stripeAccountId, CancellationToken cancellationToken) =>
        stripeClient.GetRefundAsync(refundId, cancellationToken, stripeAccountId);

    private static bool HasPersistedStripeContext(MarketplaceRefund refund) =>
        GetPersistedStripeContext(refund) is not null;

    private static StripeRefundContext? GetPersistedStripeContext(MarketplaceRefund refund)
    {
        if (string.IsNullOrWhiteSpace(refund.StripePaymentIntentId) ||
            string.IsNullOrWhiteSpace(refund.StripeChargeType))
        {
            return null;
        }

        if (string.Equals(refund.StripeChargeType, "Direct", StringComparison.OrdinalIgnoreCase) &&
            string.IsNullOrWhiteSpace(refund.StripeAccountId))
        {
            return null;
        }

        return new StripeRefundContext(
            refund.StripePaymentIntentId,
            refund.StripeAccountId,
            refund.StripeChargeType,
            refund.StripeTransferId,
            refund.PostPayoutRefund);
    }

    private static void PersistStripeContext(MarketplaceRefund refund, StripeCheckoutSessionEntity checkout)
    {
        refund.StripeAccountId = string.Equals(checkout.ChargeType, "Direct", StringComparison.OrdinalIgnoreCase)
            ? checkout.StripeAccountId
            : null;
        refund.StripeChargeType = checkout.ChargeType;
        refund.StripeTransferId = checkout.TransferId;
        refund.StripeChargeId = checkout.ChargeId;
        refund.StripePaymentIntentId = checkout.PaymentIntentId;
    }

    private async Task<string?> ResolveOrganizationIdAsync(
        string? stripeAccountId,
        CancellationToken cancellationToken) =>
        string.IsNullOrWhiteSpace(stripeAccountId)
            ? null
            : await repositoryFactory.StripeCustomerRepository.GetOrganizationIdByStripeAccountIdAsync(
                stripeAccountId,
                cancellationToken);

    private static bool IsTransferReversalUnavailable(StripeException exception)
    {
        var code = exception.StripeError?.Code;
        return code is "balance_insufficient"
            or "transfer_already_reversed"
            or "transfer_reversal_not_allowed";
    }

    private static bool IsApplicationFeeUnavailable(StripeException exception) =>
        exception.Message.Contains("has no application fee", StringComparison.OrdinalIgnoreCase);

    private static string GetStripeRefundIdempotencyKey(
        MarketplaceRefund refund,
        bool reverseTransfer,
        bool refundApplicationFee,
        bool isInitialProviderRequest)
    {
        var initialKey = string.IsNullOrWhiteSpace(refund.IdempotencyKey) ? refund.Id : refund.IdempotencyKey;

        // Preserve the existing key for the initial request. A fallback changes the
        // Stripe request body, so it needs its own stable key to remain retry-safe.
        return isInitialProviderRequest
            ? initialKey
            : $"{refund.Id}:stripe-refund:{(reverseTransfer ? "reverse" : "platform")}:{(refundApplicationFee ? "fee" : "no-fee")}";
    }

    private static bool IsSupportedChargeType(string? chargeType) =>
        string.Equals(chargeType, "Direct", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(chargeType, "Destination", StringComparison.OrdinalIgnoreCase);

    private static bool IsAmbiguousProviderOutcome(Exception exception) =>
        exception is StripeException stripeException &&
        (stripeException.HttpStatusCode == 0 ||
         (int)stripeException.HttpStatusCode >= 500 ||
         stripeException.HttpStatusCode is HttpStatusCode.RequestTimeout or HttpStatusCode.TooManyRequests);

    private async Task<StripeCheckoutSessionEntity?> ResolveStripeCheckoutSessionAsync(
        MarketplaceRefund refund,
        CancellationToken cancellationToken)
    {
        // The payment allocation is a durable snapshot of the exact payment that funded
        // this refund. Subscription roots are pricing templates; their current recurring
        // booking can change or be deleted during cancellation, so do not infer the Stripe
        // Checkout session from subscription dates when a Stripe source allocation exists.
        var stripeSource = refund.PaymentAllocations.FirstOrDefault(item =>
            item.IsSourcePayment &&
            string.Equals(item.SourcePaymentProvider, MarketplaceExternalRefundReconciliationProviderConstants.Stripe,
                StringComparison.OrdinalIgnoreCase) &&
            !string.IsNullOrWhiteSpace(item.SourcePaymentReference));
        if (stripeSource is not null)
        {
            logger.LogInformation(
                "Resolving Stripe refund source allocation for refund {RefundId}: sourceReference={SourceReference}",
                refund.Id, stripeSource.SourcePaymentReference);
            var checkout = await repositoryFactory.StripeCheckoutSessionRepository.GetByStripeCheckoutSessionIdAsync(
                stripeSource.SourcePaymentReference,
                cancellationToken);
            if (checkout is not null)
            {
                logger.LogInformation(
                    "Resolved Stripe refund Checkout Session from source allocation for refund {RefundId}: checkoutSessionId={CheckoutSessionId}, paymentIntentId={PaymentIntentId}, chargeType={ChargeType}, stripeAccountId={StripeAccountId}",
                    refund.Id, checkout.Id, checkout.PaymentIntentId, checkout.ChargeType, checkout.StripeAccountId);
                return checkout;
            }

            logger.LogWarning(
                "Stripe source allocation for refund {RefundId} referenced Checkout Session {CheckoutSessionId}, but no local Checkout Session was found",
                refund.Id, stripeSource.SourcePaymentReference);
        }

        if (refund.LocalEntityType == MarketplaceRefundEntityTypeConstants.MarketplaceBooking)
        {
            var checkout = (await repositoryFactory.MarketplaceBookingRepository.GetByIdAsync(refund.LocalEntityId, cancellationToken))
                ?.StripeCheckoutSession;
            logger.LogInformation(
                "Resolved Stripe refund Checkout Session from booking for refund {RefundId}: found={Found}, paymentIntentId={PaymentIntentId}, chargeType={ChargeType}",
                refund.Id, checkout is not null, checkout?.PaymentIntentId, checkout?.ChargeType);
            return checkout;
        }

        if (refund.LocalEntityType != MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription)
        {
            return null;
        }

        var subscription = await repositoryFactory.MarketplaceBookingSubscriptionRepository.GetByIdAsync(
            refund.LocalEntityId,
            cancellationToken);
        var checkoutSession = subscription?.RecurringBookings
            .Where(item =>
                item.MarketplaceBooking is { PaymentStatus: PaymentStatusConstants.Confirmed } &&
                item.StartDate <= refund.RequestedAt &&
                (!item.EndDate.HasValue || item.EndDate.Value >= refund.RequestedAt))
            .OrderByDescending(item => item.StartDate)
            .Select(item => item.MarketplaceBooking!.StripeCheckoutSession)
            .FirstOrDefault();
        logger.LogInformation(
            "Resolved Stripe refund Checkout Session from subscription cycle for refund {RefundId}: subscriptionFound={SubscriptionFound}, recurringBookingCount={RecurringBookingCount}, checkoutFound={CheckoutFound}, paymentIntentId={PaymentIntentId}, chargeType={ChargeType}",
            refund.Id, subscription is not null, subscription?.RecurringBookings.Count ?? 0, checkoutSession is not null,
            checkoutSession?.PaymentIntentId, checkoutSession?.ChargeType);
        return checkoutSession;
    }

    private async Task<string?> RecoverPaymentIntentIdAsync(
        StripeCheckoutSessionEntity checkout,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(checkout.StripeCheckoutSessionId) ||
            string.IsNullOrWhiteSpace(checkout.StripeAccountId))
        {
            return null;
        }

        var stripeSession = await checkoutSessionRetriever.GetAsync(
            checkout.StripeCheckoutSessionId,
            new SessionGetOptions(),
            new RequestOptions
            {
                StripeAccount = checkout.StripeAccountId,
            },
            cancellationToken);
        if (string.IsNullOrWhiteSpace(stripeSession.SubscriptionId))
        {
            return null;
        }

        var stripeSubscription = await subscriptionService.GetAsync(
            stripeSession.SubscriptionId,
            null,
            new RequestOptions
            {
                StripeAccount = checkout.StripeAccountId,
            },
            cancellationToken);
        var invoiceId = stripeSubscription.LatestInvoiceId;
        if (string.IsNullOrWhiteSpace(invoiceId))
        {
            return null;
        }

        var invoice = await invoiceRetriever.GetAsync(
            invoiceId,
            new InvoiceGetOptions
            {
                Expand = ["payments.data.payment"],
            },
            new RequestOptions
            {
                StripeAccount = checkout.StripeAccountId,
            },
            cancellationToken);
        return invoice.Payments?.Data.FirstOrDefault()?.Payment?.PaymentIntentId;
    }

    private static long ToMinorUnits(decimal amount, string? currency)
    {
        var multiplier = currency?.ToUpperInvariant() switch
        {
            "BHD" or "JOD" or "KWD" or "OMR" or "TND" => 1000m,
            "BIF" or "CLP" or "DJF" or "GNF" or "JPY" or "KMF" or "KRW" or "MGA" or "PYG" or "RWF" or "UGX" or "VND" or "VUV" or "XAF" or "XOF"
                or "XPF" => 1m,
            _ => 100m,
        };
        return decimal.ToInt64(decimal.Round(amount * multiplier, 0, MidpointRounding.AwayFromZero));
    }

    private static string MapStripeStatus(string? status) =>
        status switch
        {
            "succeeded" => MarketplaceRefundStatusConstants.Completed,
            "pending" or "requires_action" => MarketplaceRefundStatusConstants.ProviderPending,
            "failed" => MarketplaceRefundStatusConstants.Failed,
            "canceled" => MarketplaceRefundStatusConstants.Cancelled,
            _ => MarketplaceRefundStatusConstants.ProviderPending,
        };

    private sealed record StripeRefundContext(
        string PaymentIntentId,
        string? StripeAccountId,
        string ChargeType,
        string? TransferId,
        bool PostPayoutRefund);
}

public interface IStripeHostRefundClient
{
    Task<Refund> CreateRefundAsync(
        RefundCreateOptions options,
        string idempotencyKey,
        CancellationToken cancellationToken,
        string? stripeAccountId = null);

    Task<Refund> GetRefundAsync(string refundId, CancellationToken cancellationToken, string? stripeAccountId = null);
    Task<Payout> GetPayoutAsync(string payoutId, string stripeAccountId, CancellationToken cancellationToken);

    Task<IReadOnlyList<BalanceTransaction>> GetPayoutBalanceTransactionsAsync(
        string payoutId, string stripeAccountId, CancellationToken cancellationToken);
}

public class StripeHostRefundClient(
    ICreatable<Refund, RefundCreateOptions> refundService,
    IRetrievable<Refund, RefundGetOptions> refundRetriever,
    IRetrievable<Payout, PayoutGetOptions> payoutRetriever,
    BalanceTransactionService balanceTransactionService) : IStripeHostRefundClient
{
    public Task<Refund> CreateRefundAsync(
        RefundCreateOptions options,
        string idempotencyKey,
        CancellationToken cancellationToken,
        string? stripeAccountId = null) =>
        refundService.CreateAsync(
            options,
            new RequestOptions
            {
                IdempotencyKey = idempotencyKey,
                StripeAccount = stripeAccountId,
            },
            cancellationToken);

    public Task<Refund> GetRefundAsync(string refundId, CancellationToken cancellationToken, string? stripeAccountId = null) =>
        refundRetriever.GetAsync(refundId, new RefundGetOptions(), new RequestOptions
        {
            StripeAccount = stripeAccountId,
        }, cancellationToken);

    public Task<Payout> GetPayoutAsync(string payoutId, string stripeAccountId, CancellationToken cancellationToken) =>
        payoutRetriever.GetAsync(payoutId, new PayoutGetOptions(), new RequestOptions
        {
            StripeAccount = stripeAccountId,
        }, cancellationToken);

    public async Task<IReadOnlyList<BalanceTransaction>> GetPayoutBalanceTransactionsAsync(
        string payoutId, string stripeAccountId, CancellationToken cancellationToken)
    {
        var transactions = new List<BalanceTransaction>();
        await foreach (var transaction in balanceTransactionService.ListAutoPagingAsync(
                           new BalanceTransactionListOptions
                           {
                               Payout = payoutId,
                           },
                           new RequestOptions
                           {
                               StripeAccount = stripeAccountId,
                           },
                           cancellationToken))
        {
            transactions.Add(transaction);
        }

        return transactions;
    }
}
