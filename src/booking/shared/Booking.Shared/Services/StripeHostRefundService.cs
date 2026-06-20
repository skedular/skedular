using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Stripe;
using Stripe.Checkout;
using MarketplaceRefundEntityTypeConstants = Booking.Shared.Models.MarketplaceRefundEntityTypeConstants;
using MarketplaceRefundEventTypeConstants = Booking.Shared.Models.MarketplaceRefundEventTypeConstants;
using MarketplaceRefundStatusConstants = Booking.Shared.Models.MarketplaceRefundStatusConstants;

namespace Booking.Shared.Services;

public interface IStripeHostRefundService
{
    Task<bool> IsHostRefundAsync(MarketplaceRefund refund, CancellationToken cancellationToken);
    Task<bool> CanProcessAsync(MarketplaceRefund refund, CancellationToken cancellationToken);
    Task<MarketplaceRefund> ProcessAsync(MarketplaceRefund refund, CancellationToken cancellationToken);
    Task<MarketplaceRefund?> ReconcileAsync(Refund stripeRefund, CancellationToken cancellationToken);
}

public class StripeHostRefundService(
    IRepositoryFactory repositoryFactory,
    IStripeHostRefundClient stripeClient,
    IMarketplaceRefundEventService marketplaceRefundEventService,
    TimeProvider timeProvider) : IStripeHostRefundService
{
    public async Task<bool> IsHostRefundAsync(MarketplaceRefund refund, CancellationToken cancellationToken) =>
        await ResolveMarketplaceBookingAsync(refund, cancellationToken) is
        {
            ProductVersion.Product.Organization.Type: OrganizationTypeConstants.Host
        };

    public async Task<bool> CanProcessAsync(MarketplaceRefund refund, CancellationToken cancellationToken) =>
        await ResolveMarketplaceBookingAsync(refund, cancellationToken) is
        {
            ProductVersion.Product.Organization.Type: OrganizationTypeConstants.Host,
            StripeCheckoutSession: not null
        };

    public async Task<MarketplaceRefund> ProcessAsync(MarketplaceRefund refund, CancellationToken cancellationToken)
    {
        var booking = await ResolveMarketplaceBookingAsync(refund, cancellationToken) ??
                      throw new InvalidOperationException("The paid Host booking for this refund could not be found.");
        var checkout = booking.StripeCheckoutSession ??
                       throw new InvalidOperationException("The Stripe Checkout session for this Host refund could not be found.");
        try
        {
            var session = await stripeClient.GetSessionAsync(
                checkout.StripeCheckoutSessionId,
                cancellationToken);
            ArgumentException.ThrowIfNullOrWhiteSpace(session.PaymentIntentId);

            var stripeRefund = await stripeClient.CreateRefundAsync(
                new RefundCreateOptions
                {
                    PaymentIntent = session.PaymentIntentId,
                    Amount = ToMinorUnits(refund.RefundAmount ?? 0m, refund.Currency),
                    RefundApplicationFee = true,
                    ReverseTransfer = true,
                    Metadata = new Dictionary<string, string> { ["marketplace_refund_id"] = refund.Id }
                },
                refund.Id,
                cancellationToken);

            refund.PaymentProvider = "STRIPE";
            refund.ExternalPaymentRefundId = stripeRefund.Id;
            refund.PaymentRefundStatus = MapStripeStatus(stripeRefund.Status);
            refund.PaymentRefundLastError = null;
            refund.Status = refund.PaymentRefundStatus;
        }
        catch (Exception exception) when (exception is StripeException or InvalidOperationException or ArgumentException)
        {
            refund.PaymentProvider = "STRIPE";
            refund.PaymentRefundStatus = MarketplaceRefundStatusConstants.Failed;
            refund.PaymentRefundLastError = exception.Message;
            refund.Status = MarketplaceRefundStatusConstants.Failed;
            refund.LastError = exception.Message;
        }

        refund.PaymentRefundLastProcessedAt = timeProvider.GetUtcNow();
        refund.LastProcessedAt = refund.PaymentRefundLastProcessedAt;
        return repositoryFactory.MarketplaceRefundRepository.Update(refund);
    }

    public async Task<MarketplaceRefund?> ReconcileAsync(Refund stripeRefund, CancellationToken cancellationToken)
    {
        var refund = await repositoryFactory.MarketplaceRefundRepository.GetByExternalPaymentRefundIdAsync(stripeRefund.Id, cancellationToken);
        if (refund is null && stripeRefund.Metadata.TryGetValue("marketplace_refund_id", out var localRefundId))
        {
            refund = await repositoryFactory.MarketplaceRefundRepository.GetByIdAsync(localRefundId, cancellationToken);
        }

        if (refund is null)
        {
            return null;
        }

        var nextStatus = MapStripeStatus(stripeRefund.Status);
        if (refund.PaymentRefundStatus == nextStatus)
        {
            return null;
        }

        refund.PaymentProvider = "STRIPE";
        refund.ExternalPaymentRefundId ??= stripeRefund.Id;
        refund.PaymentRefundStatus = nextStatus;
        refund.PaymentRefundLastProcessedAt = timeProvider.GetUtcNow();
        refund.PaymentRefundLastError = stripeRefund.FailureReason;
        refund.Status = nextStatus;
        refund.LastProcessedAt = refund.PaymentRefundLastProcessedAt;
        refund.LastError = stripeRefund.FailureReason;
        repositoryFactory.MarketplaceRefundRepository.Update(refund);
        marketplaceRefundEventService.Add(refund, MapRefundEvent(refund.Status), null, refund.LastProcessedAt);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        return refund;
    }

    private async Task<MarketplaceBooking?> ResolveMarketplaceBookingAsync(
        MarketplaceRefund refund,
        CancellationToken cancellationToken)
    {
        if (refund.LocalEntityType == MarketplaceRefundEntityTypeConstants.MarketplaceBooking)
        {
            return await repositoryFactory.MarketplaceBookingRepository.GetByIdAsync(refund.LocalEntityId, cancellationToken);
        }

        if (refund.LocalEntityType != MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription)
        {
            return null;
        }

        var subscription = await repositoryFactory.MarketplaceBookingSubscriptionRepository.GetByIdAsync(
            refund.LocalEntityId,
            cancellationToken);
        return subscription?.RecurringBookings
            .Where(item =>
                item.MarketplaceBooking is { PaymentStatus: PaymentStatusConstants.Confirmed } &&
                item.StartDate <= refund.RequestedAt &&
                (!item.EndDate.HasValue || item.EndDate.Value >= refund.RequestedAt))
            .OrderByDescending(item => item.StartDate)
            .Select(item => item.MarketplaceBooking)
            .FirstOrDefault();
    }

    private static long ToMinorUnits(decimal amount, string? currency)
    {
        var multiplier = currency?.ToUpperInvariant() switch
        {
            "BHD" or "JOD" or "KWD" or "OMR" or "TND" => 1000m,
            "BIF" or "CLP" or "DJF" or "GNF" or "JPY" or "KMF" or "KRW" or "MGA" or "PYG" or "RWF" or "UGX" or "VND" or "VUV" or "XAF" or "XOF"
                or "XPF" => 1m,
            _ => 100m
        };
        return decimal.ToInt64(decimal.Round(amount * multiplier, 0, MidpointRounding.AwayFromZero));
    }

    private static string MapStripeStatus(string? status) =>
        status switch
        {
            "succeeded" => MarketplaceRefundStatusConstants.Completed,
            "pending" or "requires_action" => MarketplaceRefundStatusConstants.PendingAccounting,
            "failed" or "canceled" => MarketplaceRefundStatusConstants.Failed,
            _ => MarketplaceRefundStatusConstants.PendingAccounting
        };

    private static string MapRefundEvent(string status) =>
        status switch
        {
            MarketplaceRefundStatusConstants.Completed => MarketplaceRefundEventTypeConstants.Completed,
            MarketplaceRefundStatusConstants.Failed => MarketplaceRefundEventTypeConstants.Failed,
            _ => MarketplaceRefundEventTypeConstants.PendingAccounting
        };
}

public interface IStripeHostRefundClient
{
    Task<Session> GetSessionAsync(string sessionId, CancellationToken cancellationToken);

    Task<Refund> CreateRefundAsync(
        RefundCreateOptions options,
        string idempotencyKey,
        CancellationToken cancellationToken);
}

public class StripeHostRefundClient(
    IRetrievable<Session, SessionGetOptions> sessionService,
    ICreatable<Refund, RefundCreateOptions> refundService) : IStripeHostRefundClient
{
    public Task<Session> GetSessionAsync(string sessionId, CancellationToken cancellationToken) =>
        sessionService.GetAsync(
            sessionId,
            new SessionGetOptions { Expand = ["payment_intent"] },
            null,
            cancellationToken);

    public Task<Refund> CreateRefundAsync(
        RefundCreateOptions options,
        string idempotencyKey,
        CancellationToken cancellationToken) =>
        refundService.CreateAsync(
            options,
            new RequestOptions { IdempotencyKey = idempotencyKey },
            cancellationToken);
}
