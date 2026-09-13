using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Services.Entitlements;
using Booking.Shared.Workflows;
using Enterprise.Shared;
using Enterprise.Shared.GraphQL;
using Stripe;
using Stripe.Checkout;
using Constants = Booking.Shared.GraphQL.Constants;
using MarketplaceBooking = Booking.Shared.Database.Entities.MarketplaceBooking;
using MarketplaceBookingSubscriptionEntity = Booking.Shared.Database.Entities.MarketplaceBookingSubscription;
using StripeInvoice = Stripe.Invoice;

namespace Booking.Processors.Subscribers;

public interface IBookingStripeWebhookProcessor
{
    Task ProcessAsync(string json, CancellationToken cancellationToken);
}

public class BookingStripeWebhookProcessor(
    IRepositoryFactory repositoryFactory,
    ITemporalService temporalService,
    IStripeHostRefundService stripeHostRefundService,
    IStripePayoutReconciliationService payoutReconciliationService,
    IGraphQlTopicEventSender graphQlTopicEventSender,
    IEntitlementPurchaseService entitlementPurchaseService,
    IMarketplacePaidInvoiceProvisioner paidInvoiceProvisioner,
    SubscriptionService subscriptionService,
    IMarketplaceStripeSubscriptionPriceService marketplaceStripeSubscriptionPriceService,
    IRetrievable<StripeInvoice, InvoiceGetOptions> invoiceService,
    ILogger<BookingStripeWebhookProcessor> logger) : IBookingStripeWebhookProcessor
{
    public async Task ProcessAsync(string json, CancellationToken cancellationToken)
    {
        var stripeEvent = EventUtility.ParseEvent(json, false);
        ArgumentNullException.ThrowIfNull(stripeEvent);

        switch (stripeEvent.Type)
        {
            case EventTypes.CheckoutSessionCompleted:
            case EventTypes.CheckoutSessionAsyncPaymentSucceeded:
                await HandleCheckoutSessionCompletedAsync(stripeEvent, cancellationToken);
                break;

            case EventTypes.CheckoutSessionExpired:
            case EventTypes.CheckoutSessionAsyncPaymentFailed:
                await HandleCheckoutSessionExpiredAsync(stripeEvent, cancellationToken);
                break;

            case EventTypes.ChargeSucceeded when stripeEvent.Data.Object is Charge charge:
                await HandleChargeSucceededAsync(charge, cancellationToken);
                break;

            case EventTypes.ChargeFailed when stripeEvent.Data.Object is Charge charge:
                await HandleChargeFailedAsync(charge, cancellationToken);
                break;

            case EventTypes.PaymentIntentSucceeded when stripeEvent.Data.Object is PaymentIntent paymentIntent:
                await HandlePaymentIntentSucceededAsync(paymentIntent, cancellationToken);
                break;

            case EventTypes.PaymentIntentPaymentFailed or EventTypes.PaymentIntentCanceled
                when stripeEvent.Data.Object is PaymentIntent paymentIntent:
                await HandlePaymentIntentFailedAsync(paymentIntent, cancellationToken);
                break;

            case EventTypes.InvoicePaid or EventTypes.InvoicePaymentSucceeded when stripeEvent.Data.Object is StripeInvoice invoice:
                await HandleInvoicePaidAsync(invoice, stripeEvent.Account, cancellationToken);
                break;

            case EventTypes.InvoicePaymentPaid when stripeEvent.Data.Object is InvoicePayment invoicePayment:
                if (invoicePayment.Invoice is not null && !string.IsNullOrWhiteSpace(stripeEvent.Account))
                {
                    var paidInvoice = await invoiceService.GetAsync(
                        invoicePayment.Invoice.Id,
                        new InvoiceGetOptions
                        {
                            Expand = ["payments.data.payment"],
                        },
                        new RequestOptions
                        {
                            StripeAccount = stripeEvent.Account,
                        },
                        cancellationToken);
                    await HandleInvoicePaidAsync(paidInvoice, stripeEvent.Account, cancellationToken);
                }

                break;
            case EventTypes.InvoiceUpcoming when stripeEvent.Data.Object is StripeInvoice upcomingInvoice:
                var upcomingSubscriptionId = upcomingInvoice.Parent?.SubscriptionDetails?.SubscriptionId;
                if (!string.IsNullOrWhiteSpace(stripeEvent.Account) &&
                    !string.IsNullOrWhiteSpace(upcomingSubscriptionId))
                {
                    await marketplaceStripeSubscriptionPriceService.PrepareCurrentPriceAsync(
                        stripeEvent.Account,
                        upcomingSubscriptionId,
                        cancellationToken);
                }

                break;
            case EventTypes.InvoicePaymentActionRequired or EventTypes.InvoicePaymentFailed or EventTypes.InvoiceFinalizationFailed
                when stripeEvent.Data.Object is StripeInvoice invoice:
                await HandleInvoicePaymentProblemAsync(invoice, stripeEvent.Account, stripeEvent.Type, stripeEvent.Id, cancellationToken);
                break;
            case EventTypes.CustomerSubscriptionCreated or EventTypes.CustomerSubscriptionUpdated
                when stripeEvent.Data.Object is Subscription subscription:
                await HandleStripeSubscriptionUpdatedAsync(subscription, stripeEvent.Account, false, cancellationToken);
                break;
            case EventTypes.CustomerSubscriptionPaused when stripeEvent.Data.Object is Subscription subscription:
                await HandleStripeSubscriptionUpdatedAsync(subscription, stripeEvent.Account, false, cancellationToken, "paused");
                break;
            case EventTypes.CustomerSubscriptionResumed when stripeEvent.Data.Object is Subscription subscription:
                await HandleStripeSubscriptionUpdatedAsync(subscription, stripeEvent.Account, false, cancellationToken, "active");
                break;
            case EventTypes.CustomerSubscriptionDeleted when stripeEvent.Data.Object is Subscription subscription:
                await HandleStripeSubscriptionUpdatedAsync(subscription, stripeEvent.Account, true, cancellationToken);
                break;
            case EventTypes.AccountApplicationDeauthorized:
                await HandleStripeAccountDisconnectedAsync(stripeEvent.Account, cancellationToken);
                break;
            case EventTypes.PayoutPaid or EventTypes.PayoutReconciliationCompleted when stripeEvent.Data.Object is Payout payout:
                await payoutReconciliationService.HandlePaidAsync(
                    payout,
                    stripeEvent.Account,
                    cancellationToken,
                    new DateTimeOffset(stripeEvent.Created, TimeSpan.Zero),
                    stripeEvent.Id);
                break;
            case EventTypes.PayoutFailed or EventTypes.PayoutCanceled or EventTypes.PayoutUpdated when stripeEvent.Data.Object is Payout payout:
                await payoutReconciliationService.HandleStateChangedAsync(
                    payout,
                    stripeEvent.Type,
                    cancellationToken,
                    stripeEvent.Account,
                    new DateTimeOffset(stripeEvent.Created, TimeSpan.Zero),
                    stripeEvent.Id);
                break;
            case EventTypes.RefundCreated:
            case EventTypes.RefundUpdated:
            case EventTypes.RefundFailed:
                if (stripeEvent.Data.Object is Refund refund)
                {
                    var localRefund = await stripeHostRefundService.ReconcileAsync(
                        refund,
                        cancellationToken,
                        stripeEvent.Account,
                        stripeEvent.Id);
                    if (localRefund is not null)
                    {
                        if (stripeEvent.Type == EventTypes.RefundFailed)
                        {
                            localRefund.RetryCount++;
                            repositoryFactory.MarketplaceRefundRepository.Update(localRefund);
                            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
                        }
                    }
                }

                break;
        }
    }

    private async Task HandleChargeSucceededAsync(Charge charge, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(charge.PaymentIntentId))
        {
            return;
        }

        var checkout = await repositoryFactory.StripeCheckoutSessionRepository.GetByPaymentIntentIdAsync(
            charge.PaymentIntentId, cancellationToken);
        if (checkout is null)
        {
            return;
        }

        checkout.ChargeId = charge.Id;
        checkout.TransferId = charge.TransferId;
        repositoryFactory.StripeCheckoutSessionRepository.Update(checkout);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleChargeFailedAsync(Charge charge, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(charge.PaymentIntentId))
        {
            return;
        }

        var checkout = await repositoryFactory.StripeCheckoutSessionRepository.GetByPaymentIntentIdAsync(
            charge.PaymentIntentId, cancellationToken);
        if (checkout is null)
        {
            return;
        }

        checkout.MarketplaceBooking.PaymentStatus = PaymentStatusConstants.Rejected;
        repositoryFactory.StripeCheckoutSessionRepository.Update(checkout);
        repositoryFactory.MarketplaceBookingRepository.Update(checkout.MarketplaceBooking);
        await repositoryFactory.MarketplacePurchaseHistoryRepository.RefreshForMarketplaceBookingAsync(
            checkout.MarketplaceBooking.Id, cancellationToken);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await SignalPaymentStatusAsync(checkout.MarketplaceBooking, cancellationToken);
    }

    private async Task HandlePaymentIntentSucceededAsync(PaymentIntent paymentIntent, CancellationToken cancellationToken)
    {
        var checkout = await repositoryFactory.StripeCheckoutSessionRepository.GetByPaymentIntentIdAsync(
            paymentIntent.Id, cancellationToken);
        if (checkout is null)
        {
            return;
        }

        checkout.PaymentIntentId = paymentIntent.Id;
        checkout.MarketplaceBooking.PaymentStatus = PaymentStatusConstants.Confirmed;
        repositoryFactory.StripeCheckoutSessionRepository.Update(checkout);
        repositoryFactory.MarketplaceBookingRepository.Update(checkout.MarketplaceBooking);
        await repositoryFactory.MarketplacePurchaseHistoryRepository.RefreshForMarketplaceBookingAsync(
            checkout.MarketplaceBooking.Id, cancellationToken);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await SignalPaymentStatusAsync(checkout.MarketplaceBooking, cancellationToken);
    }

    private async Task HandlePaymentIntentFailedAsync(PaymentIntent paymentIntent, CancellationToken cancellationToken)
    {
        var checkout = await repositoryFactory.StripeCheckoutSessionRepository.GetByPaymentIntentIdAsync(
            paymentIntent.Id, cancellationToken);
        if (checkout is null)
        {
            return;
        }

        checkout.MarketplaceBooking.PaymentStatus = PaymentStatusConstants.Rejected;
        repositoryFactory.StripeCheckoutSessionRepository.Update(checkout);
        repositoryFactory.MarketplaceBookingRepository.Update(checkout.MarketplaceBooking);
        await repositoryFactory.MarketplacePurchaseHistoryRepository.RefreshForMarketplaceBookingAsync(
            checkout.MarketplaceBooking.Id, cancellationToken);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await SignalPaymentStatusAsync(checkout.MarketplaceBooking, cancellationToken);
    }

    private async Task HandleInvoicePaidAsync(StripeInvoice invoice, string? stripeAccountId, CancellationToken cancellationToken)
    {
        var subscriptionId = invoice.Parent?.SubscriptionDetails?.SubscriptionId;
        if (string.IsNullOrWhiteSpace(stripeAccountId) || string.IsNullOrWhiteSpace(subscriptionId))
        {
            return;
        }

        // The first subscription invoice is the payment for the initial Checkout.
        // Checkout completion owns creation/confirmation of that initial purchase;
        // only cycle invoices represent a marketplace renewal that needs renewal
        // provisioning.
        if (invoice.BillingReason == "subscription_create")
        {
            var initialEntitlement = await repositoryFactory.EntitlementPurchaseRepository
                .GetByStripeSubscriptionAsync(stripeAccountId, subscriptionId, cancellationToken);
            if (initialEntitlement is not null)
            {
                if (invoice.Payments?.Data.FirstOrDefault()?.Payment?.PaymentIntentId is null)
                {
                    invoice = await invoiceService.GetAsync(
                        invoice.Id,
                        new InvoiceGetOptions
                        {
                            Expand = ["payments.data.payment"],
                        },
                        new RequestOptions
                        {
                            StripeAccount = stripeAccountId,
                        },
                        cancellationToken);
                }

                await entitlementPurchaseService.UpdateStripePaymentContextAsync(
                    initialEntitlement.Id,
                    null,
                    invoice.Payments?.Data.FirstOrDefault()?.Payment?.PaymentIntentId,
                    cancellationToken);
            }

            if (initialEntitlement is null)
            {
                var initialSubscription = await repositoryFactory.MarketplaceBookingSubscriptionRepository
                    .GetByStripeSubscriptionAsync(stripeAccountId, subscriptionId, cancellationToken);
                if (initialSubscription is not null)
                {
                    initialSubscription.StripeSubscriptionStatus = "active";
                    initialSubscription.StripeCurrentPeriodEndsAt = new DateTimeOffset(invoice.PeriodEnd, TimeSpan.Zero);
                    repositoryFactory.MarketplaceBookingSubscriptionRepository.Update(initialSubscription);
                    await paidInvoiceProvisioner.ProvisionAsync(
                        initialSubscription.Id,
                        null,
                        stripeAccountId,
                        invoice.Id,
                        new DateTimeOffset(invoice.PeriodStart, TimeSpan.Zero),
                        invoice.Payments?.Data.FirstOrDefault()?.Payment?.PaymentIntentId,
                        cancellationToken);
                    await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
                    await graphQlTopicEventSender.RaiseGraphqlChangeAsync(
                        Constants.MarketplaceBookingSubscriptionTopicName,
                        initialSubscription.Id,
                        cancellationToken);
                }
            }

            logger.LogInformation(
                "Ignoring initial Stripe subscription invoice for renewal provisioning. AccountId={AccountId}, SubscriptionId={SubscriptionId}, InvoiceId={InvoiceId}",
                stripeAccountId,
                subscriptionId,
                invoice.Id);
            return;
        }

        var subscription = await repositoryFactory.MarketplaceBookingSubscriptionRepository.GetByStripeSubscriptionAsync(
            stripeAccountId, subscriptionId, cancellationToken);
        var entitlement = subscription is null
            ? await repositoryFactory.EntitlementPurchaseRepository.GetByStripeSubscriptionAsync(
                stripeAccountId, subscriptionId, cancellationToken)
            : null;
        if (subscription is null && entitlement is null)
        {
            var stripeSubscription = await subscriptionService.GetAsync(
                subscriptionId,
                null,
                new RequestOptions
                {
                    StripeAccount = stripeAccountId,
                },
                cancellationToken);
            (subscription, entitlement) = await CorrelateStripeSubscriptionFromMetadataAsync(
                stripeSubscription,
                stripeAccountId,
                cancellationToken);
            if (subscription is null && entitlement is null)
            {
                throw new InvalidOperationException(
                    "A paid Stripe invoice could not be correlated to a marketplace purchase. The webhook will be retried.");
            }
        }

        // Stripe's invoice webhook payload normally identifies the invoice but may omit the
        // payment collection and PaymentIntent. Retrieve it from the connected account so the
        // local checkout record can be linked to the payment used for refunds.
        if (invoice.Payments?.Data.FirstOrDefault()?.Payment?.PaymentIntentId is null)
        {
            invoice = await invoiceService.GetAsync(
                invoice.Id,
                new InvoiceGetOptions
                {
                    Expand = ["payments.data.payment"],
                },
                new RequestOptions
                {
                    StripeAccount = stripeAccountId,
                },
                cancellationToken);
        }

        if (!await paidInvoiceProvisioner.ProvisionAsync(
                subscription?.Id,
                entitlement?.Id,
                stripeAccountId,
                invoice.Id,
                new DateTimeOffset(invoice.PeriodStart, TimeSpan.Zero),
                invoice.Payments?.Data.FirstOrDefault()?.Payment?.PaymentIntentId,
                cancellationToken))
        {
            await temporalService.StartWorkflowProvisionMarketplacePaidInvoiceAsync(
                new ProvisionMarketplacePaidInvoiceInput(
                    subscription?.Id,
                    entitlement?.Id,
                    stripeAccountId,
                    invoice.Id,
                    new DateTimeOffset(invoice.PeriodStart, TimeSpan.Zero),
                    new DateTimeOffset(invoice.PeriodEnd, TimeSpan.Zero),
                    invoice.Payments?.Data.FirstOrDefault()?.Payment?.PaymentIntentId),
                cancellationToken);
            return;
        }

        var currentPeriodEndsAt = new DateTimeOffset(invoice.PeriodEnd, TimeSpan.Zero);
        if (subscription is not null)
        {
            subscription.StripeSubscriptionStatus = "active";
            subscription.StripeCurrentPeriodEndsAt = currentPeriodEndsAt;
            repositoryFactory.MarketplaceBookingSubscriptionRepository.Update(subscription);
        }
        else
        {
            entitlement!.StripeSubscriptionStatus = "active";
            entitlement.StripeCurrentPeriodEndsAt = currentPeriodEndsAt;
            repositoryFactory.EntitlementPurchaseRepository.Update(entitlement);
        }

        await AppendStripeHistoryEventAsync(
            subscription?.Id ?? entitlement!.Id,
            subscription is null,
            currentPeriodEndsAt,
            MarketplacePurchaseHistoryEventType.PaymentStateChanged,
            PaymentStatus.Confirmed,
            $"Stripe invoice {invoice.Id} was paid.",
            new DateTimeOffset(invoice.PeriodStart, TimeSpan.Zero).ToUnixTimeSeconds(),
            $"invoice:{stripeAccountId}:{invoice.Id}",
            cancellationToken);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await RaiseMarketplacePurchaseChangeAsync(subscription?.Id ?? entitlement!.Id, subscription is null, cancellationToken);
    }

    private async Task HandleInvoicePaymentProblemAsync(
        StripeInvoice invoice,
        string? stripeAccountId,
        string eventType,
        string eventId,
        CancellationToken cancellationToken)
    {
        var subscriptionId = invoice.Parent?.SubscriptionDetails?.SubscriptionId;
        if (string.IsNullOrWhiteSpace(stripeAccountId) || string.IsNullOrWhiteSpace(subscriptionId))
        {
            return;
        }

        var subscription = await repositoryFactory.MarketplaceBookingSubscriptionRepository.GetByStripeSubscriptionAsync(
            stripeAccountId, subscriptionId, cancellationToken);
        var entitlement = subscription is null
            ? await repositoryFactory.EntitlementPurchaseRepository.GetByStripeSubscriptionAsync(
                stripeAccountId, subscriptionId, cancellationToken)
            : null;
        if (subscription is null && entitlement is null)
        {
            return;
        }

        var status = eventType switch
        {
            EventTypes.InvoicePaymentActionRequired => "action_required",
            EventTypes.InvoicePaymentFailed => "past_due",
            _ => "finalization_failed",
        };
        if (subscription is not null)
        {
            subscription.StripeSubscriptionStatus = status;
            repositoryFactory.MarketplaceBookingSubscriptionRepository.Update(subscription);
        }
        else
        {
            entitlement!.StripeSubscriptionStatus = status;
            repositoryFactory.EntitlementPurchaseRepository.Update(entitlement);
        }

        var sourceId = subscription?.Id ?? entitlement!.Id;
        await AppendStripeHistoryEventAsync(
            sourceId,
            subscription is null,
            subscription?.StripeCurrentPeriodEndsAt ?? entitlement?.StripeCurrentPeriodEndsAt,
            MarketplacePurchaseHistoryEventType.PaymentStateChanged,
            eventType == EventTypes.InvoicePaymentActionRequired ? PaymentStatus.Pending : PaymentStatus.Rejected,
            $"Stripe invoice {invoice.Id} reported {eventType}.",
            new DateTimeOffset(invoice.PeriodStart, TimeSpan.Zero).ToUnixTimeSeconds(),
            eventId,
            cancellationToken);
        logger.LogWarning(
            "Stripe Billing invoice needs recovery; no marketplace grant was made. EventId={EventId}, AccountId={AccountId}, SubscriptionId={SubscriptionId}, InvoiceId={InvoiceId}, Status={Status}",
            eventId,
            stripeAccountId,
            subscriptionId,
            invoice.Id,
            status);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await RaiseMarketplacePurchaseChangeAsync(sourceId, subscription is null, cancellationToken);
    }

    private async Task HandleStripeSubscriptionUpdatedAsync(
        Subscription subscription,
        string? stripeAccountId,
        bool deleted,
        CancellationToken cancellationToken,
        string? statusOverride = null)
    {
        if (string.IsNullOrWhiteSpace(stripeAccountId))
        {
            return;
        }

        var marketplaceSubscription = await repositoryFactory.MarketplaceBookingSubscriptionRepository.GetByStripeSubscriptionAsync(
            stripeAccountId, subscription.Id, cancellationToken);
        var entitlement = marketplaceSubscription is null
            ? await repositoryFactory.EntitlementPurchaseRepository.GetByStripeSubscriptionAsync(
                stripeAccountId, subscription.Id, cancellationToken)
            : null;
        if (marketplaceSubscription is null && entitlement is null)
        {
            (marketplaceSubscription, entitlement) = await CorrelateStripeSubscriptionFromMetadataAsync(
                subscription, stripeAccountId, cancellationToken);
        }

        if (marketplaceSubscription is null && entitlement is null)
        {
            return;
        }

        var status = deleted ? "cancelled" : statusOverride ?? subscription.Status;
        var cancelAtPeriodEnd = !deleted && subscription.CancelAtPeriodEnd;
        var currentPeriodEnd = subscription.Items.Data.FirstOrDefault()?.CurrentPeriodEnd;
        var currentPeriodEndsAt = currentPeriodEnd is null
            ? (DateTimeOffset?)null
            : new DateTimeOffset(currentPeriodEnd.Value, TimeSpan.Zero);
        if (marketplaceSubscription is not null)
        {
            marketplaceSubscription.StripeSubscriptionStatus = status;
            marketplaceSubscription.StripeCancelAtPeriodEnd = cancelAtPeriodEnd;
            marketplaceSubscription.StripeCurrentPeriodEndsAt = currentPeriodEndsAt;
            repositoryFactory.MarketplaceBookingSubscriptionRepository.Update(marketplaceSubscription);
        }
        else
        {
            entitlement!.StripeSubscriptionStatus = status;
            entitlement.StripeCancelAtPeriodEnd = cancelAtPeriodEnd;
            entitlement.StripeCurrentPeriodEndsAt = currentPeriodEndsAt;
            repositoryFactory.EntitlementPurchaseRepository.Update(entitlement);
        }

        var sourceId = marketplaceSubscription?.Id ?? entitlement!.Id;
        await AppendStripeHistoryEventAsync(
            sourceId,
            marketplaceSubscription is null,
            currentPeriodEndsAt,
            MarketplacePurchaseHistoryEventType.PaymentStateChanged,
            null,
            deleted ? "Stripe subscription was deleted." : "Stripe subscription status changed.",
            DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            $"subscription:{subscription.Id}:{subscription.Status}:{subscription.CancelAtPeriodEnd}:{deleted}",
            cancellationToken,
            cancelAtPeriodEnd);
        logger.LogInformation(
            "Updated marketplace Stripe subscription projection. AccountId={AccountId}, SubscriptionId={SubscriptionId}, Status={Status}, CancelAtPeriodEnd={CancelAtPeriodEnd}",
            stripeAccountId,
            subscription.Id,
            status,
            cancelAtPeriodEnd);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await RaiseMarketplacePurchaseChangeAsync(sourceId, marketplaceSubscription is null, cancellationToken);
    }

    private async Task<(MarketplaceBookingSubscriptionEntity? Subscription, EntitlementPurchase? Entitlement)>
        CorrelateStripeSubscriptionFromMetadataAsync(Subscription stripeSubscription, string stripeAccountId, CancellationToken cancellationToken)
    {
        var purchaseType = stripeSubscription.Metadata?.GetValueOrDefault("marketplace_purchase_type");
        var purchaseId = stripeSubscription.Metadata?.GetValueOrDefault("marketplace_purchase_id");
        var stripePriceId = stripeSubscription.Metadata?.GetValueOrDefault("marketplace_stripe_price_id");
        if (string.IsNullOrWhiteSpace(purchaseType) || string.IsNullOrWhiteSpace(purchaseId) ||
            string.IsNullOrWhiteSpace(stripePriceId) || string.IsNullOrWhiteSpace(stripeSubscription.CustomerId))
        {
            return (null, null);
        }

        if (string.Equals(purchaseType, "reservation", StringComparison.Ordinal))
        {
            var marketplaceSubscription = await repositoryFactory.MarketplaceBookingSubscriptionRepository.GetByIdAsync(
                purchaseId, cancellationToken);
            if (marketplaceSubscription is null || !marketplaceSubscription.AutoRenew)
            {
                return (null, null);
            }

            EnsureStripeCorrelationMatches(marketplaceSubscription.StripeAccountId, stripeAccountId, "account");
            EnsureStripeCorrelationMatches(marketplaceSubscription.StripeSubscriptionId, stripeSubscription.Id, "subscription");
            marketplaceSubscription.StripeAccountId = stripeAccountId;
            marketplaceSubscription.StripeCustomerId = stripeSubscription.CustomerId;
            marketplaceSubscription.StripeSubscriptionId = stripeSubscription.Id;
            marketplaceSubscription.StripePriceId = stripePriceId;
            marketplaceSubscription.StripeSubscriptionStatus ??= "pending";
            repositoryFactory.MarketplaceBookingSubscriptionRepository.Update(marketplaceSubscription);
            return (marketplaceSubscription, null);
        }

        if (!string.Equals(purchaseType, "entitlement", StringComparison.Ordinal))
        {
            return (null, null);
        }

        var entitlement = await repositoryFactory.EntitlementPurchaseRepository.GetByIdAsync(purchaseId, cancellationToken);
        if (entitlement is null || !entitlement.AutoRenew)
        {
            return (null, null);
        }

        EnsureStripeCorrelationMatches(entitlement.StripeAccountId, stripeAccountId, "account");
        EnsureStripeCorrelationMatches(entitlement.StripeSubscriptionId, stripeSubscription.Id, "subscription");
        entitlement.StripeAccountId = stripeAccountId;
        entitlement.StripeCustomerId = stripeSubscription.CustomerId;
        entitlement.StripeSubscriptionId = stripeSubscription.Id;
        entitlement.StripePriceId = stripePriceId;
        entitlement.StripeSubscriptionStatus ??= "pending";
        repositoryFactory.EntitlementPurchaseRepository.Update(entitlement);
        return (null, entitlement);
    }

    private async Task HandleStripeAccountDisconnectedAsync(string? stripeAccountId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(stripeAccountId))
        {
            return;
        }

        var subscriptions = await repositoryFactory.MarketplaceBookingSubscriptionRepository.GetByStripeAccountAsync(
            stripeAccountId, cancellationToken);
        var entitlements = await repositoryFactory.EntitlementPurchaseRepository.GetByStripeAccountAsync(
            stripeAccountId, cancellationToken);
        foreach (var subscription in subscriptions)
        {
            subscription.StripeSubscriptionStatus = "disconnected";
            repositoryFactory.MarketplaceBookingSubscriptionRepository.Update(subscription);
            await AppendStripeHistoryEventAsync(
                subscription.Id,
                false,
                subscription.StripeCurrentPeriodEndsAt,
                MarketplacePurchaseHistoryEventType.PaymentStateChanged,
                PaymentStatus.Rejected,
                "Stripe Connect account was disconnected; automatic renewal is blocked.",
                DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                $"account-disconnected:{stripeAccountId}",
                cancellationToken);
        }

        foreach (var entitlement in entitlements)
        {
            entitlement.StripeSubscriptionStatus = "disconnected";
            repositoryFactory.EntitlementPurchaseRepository.Update(entitlement);
            await AppendStripeHistoryEventAsync(
                entitlement.Id,
                true,
                entitlement.StripeCurrentPeriodEndsAt,
                MarketplacePurchaseHistoryEventType.PaymentStateChanged,
                PaymentStatus.Rejected,
                "Stripe Connect account was disconnected; automatic renewal is blocked.",
                DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                $"account-disconnected:{stripeAccountId}:{entitlement.Id}",
                cancellationToken);
        }

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        foreach (var subscription in subscriptions)
        {
            await RaiseMarketplacePurchaseChangeAsync(subscription.Id, false, cancellationToken);
        }

        foreach (var entitlement in entitlements)
        {
            await RaiseMarketplacePurchaseChangeAsync(entitlement.Id, true, cancellationToken);
        }

        logger.LogWarning(
            "Stripe Connect account disconnected; marketplace renewal grants are blocked. AccountId={AccountId}, LinkCount={LinkCount}",
            stripeAccountId,
            subscriptions.Count + entitlements.Count);
    }

    private async Task AppendStripeHistoryEventAsync(
        string sourceId,
        bool isEntitlement,
        DateTimeOffset? currentPeriodEndsAt,
        MarketplacePurchaseHistoryEventType eventType,
        PaymentStatus? paymentStatus,
        string reason,
        long occurredAtUnixSeconds,
        string idempotencyKey,
        CancellationToken cancellationToken,
        bool? cancelAtPeriodEnd = null)
    {
        var source = isEntitlement
            ? MarketplacePurchaseHistoryEligibleSourceType.Entitlement
            : MarketplacePurchaseHistoryEligibleSourceType.Subscription;

        try
        {
            await repositoryFactory.MarketplacePurchaseHistoryRepository.AppendEventAsync(
                new MarketplacePurchaseHistoryEventModel(
                    $"stripe:{idempotencyKey}",
                    sourceId,
                    source,
                    eventType,
                    DateTimeOffset.FromUnixTimeSeconds(occurredAtUnixSeconds),
                    DateTimeOffset.UtcNow,
                    null,
                    paymentStatus,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    currentPeriodEndsAt,
                    reason,
                    AutoRenew: true,
                    CancelAtPeriodEnd: cancelAtPeriodEnd,
                    CorrelationId: idempotencyKey),
                idempotencyKey,
                cancellationToken);
        }
        catch (InvalidOperationException exception)
        {
            logger.LogWarning(
                exception,
                "Stripe lifecycle event could not be recorded because the marketplace history snapshot is unavailable. SourceId={SourceId}, IdempotencyKey={IdempotencyKey}",
                sourceId,
                idempotencyKey);
            throw;
        }
    }

    private Task RaiseMarketplacePurchaseChangeAsync(
        string sourceId,
        bool isEntitlement,
        CancellationToken cancellationToken)
    {
        var topicName = isEntitlement
            ? Constants.EntitlementPurchaseTopicName
            : Constants.MarketplaceBookingSubscriptionTopicName;
        return graphQlTopicEventSender.RaiseGraphqlChangeAsync(topicName, sourceId, cancellationToken);
    }

    private async Task SignalPaymentStatusAsync(
        MarketplaceBooking marketplaceBooking,
        CancellationToken cancellationToken)
    {
        if (marketplaceBooking.Booking is not null)
        {
            await temporalService.SignalPayBookingViaCardWorkflowAsync(
                marketplaceBooking.Booking.Id,
                new SetPaymentStatusArgs(marketplaceBooking.PaymentStatus),
                cancellationToken);
        }
        else if (marketplaceBooking.RecurringBooking?.MarketplaceBookingSubscription is not null)
        {
            await temporalService.SignalPayRecurringBookingViaCardWorkflowAsync(
                marketplaceBooking.RecurringBooking.Id,
                new SetPaymentStatusArgs(marketplaceBooking.PaymentStatus),
                cancellationToken);
        }
    }

    private async Task HandleCheckoutSessionCompletedAsync(Event stripeEvent, CancellationToken cancellationToken)
    {
        var session = stripeEvent.Data.Object as Session;
        ArgumentNullException.ThrowIfNull(session);

        var stripeCheckoutSession = await repositoryFactory.StripeCheckoutSessionRepository.GetByStripeCheckoutSessionIdAsync(
            session.Id,
            cancellationToken);
        if (stripeCheckoutSession is null)
        {
            if (!string.IsNullOrWhiteSpace(session.ClientReferenceId))
            {
                var purchase = await repositoryFactory.EntitlementPurchaseRepository.GetByIdAsync(session.ClientReferenceId, cancellationToken);
                if (purchase is null || purchase.StripeCheckoutSessionId != session.Id)
                {
                    return;
                }

                await entitlementPurchaseService.UpdateStripePaymentContextAsync(
                    session.ClientReferenceId,
                    session.Id,
                    session.PaymentIntentId,
                    cancellationToken);
                if (purchase.AutoRenew)
                {
                    await CorrelateEntitlementStripeSubscriptionAsync(purchase, session, stripeEvent.Account, cancellationToken);
                    if (session.PaymentStatus == "paid")
                    {
                        await entitlementPurchaseService.UpdatePaymentStatusAsync(
                            purchase.Id,
                            PaymentStatus.Confirmed,
                            new DateTimeOffset(session.Created, TimeSpan.Zero),
                            cancellationToken);
                    }

                    return;
                }

                await entitlementPurchaseService.UpdatePaymentStatusAsync(
                    session.ClientReferenceId,
                    session.PaymentStatus switch
                    {
                        "no_payment_required" => PaymentStatus.NoPaymentRequired,
                        "paid" => PaymentStatus.Confirmed,
                        _ => PaymentStatus.Pending,
                    },
                    new DateTimeOffset(session.Created, TimeSpan.Zero),
                    cancellationToken);
            }

            return;
        }

        var marketplaceBooking = stripeCheckoutSession.MarketplaceBooking;
        stripeCheckoutSession.PaymentIntentId = session.PaymentIntentId;
        repositoryFactory.StripeCheckoutSessionRepository.Update(stripeCheckoutSession);
        if (marketplaceBooking.RecurringBooking?.MarketplaceBookingSubscription is
            { AutoRenew: true, MarketplaceBooking.ProductPricing.SupportsSubscriptionAutoRenewal: true })
        {
            await CorrelateReservationStripeSubscriptionAsync(
                marketplaceBooking.RecurringBooking.MarketplaceBookingSubscription,
                session,
                stripeEvent.Account,
                cancellationToken);
            if (session.PaymentStatus == "paid")
            {
                marketplaceBooking.PaymentStatus = PaymentStatusConstants.Confirmed;
                repositoryFactory.MarketplaceBookingRepository.Update(marketplaceBooking);
                await repositoryFactory.MarketplacePurchaseHistoryRepository.RefreshForMarketplaceBookingAsync(
                    marketplaceBooking.Id,
                    cancellationToken);
            }

            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            if (session.PaymentStatus == "paid")
            {
                await SignalPaymentStatusAsync(marketplaceBooking, cancellationToken);
            }

            return;
        }

        marketplaceBooking.PaymentStatus = session.PaymentStatus switch
        {
            "no_payment_required" => PaymentStatusConstants.NoPaymentRequired,
            "paid" => PaymentStatusConstants.Confirmed,
            "unpaid" => PaymentStatusConstants.Rejected,
            _ => throw new ArgumentOutOfRangeException(nameof(session.PaymentStatus), session.PaymentStatus,
                $"Unexpected value for {nameof(session.PaymentStatus)}: {session.PaymentStatus}. Update enum mapping or caller input."),
        };

        if (marketplaceBooking.RecurringBooking?.MarketplaceBookingSubscription is null)
        {
            marketplaceBooking.TotalAmountExcludeTax = session.AmountSubtotal is null ? null : (decimal)session.AmountSubtotal / 100;
            marketplaceBooking.TotalAmount = session.AmountTotal is null ? null : (decimal)session.AmountTotal / 100;
            marketplaceBooking.TaxAmount = marketplaceBooking.TotalAmountExcludeTax is not null && marketplaceBooking.TotalAmount is not null
                ? marketplaceBooking.TotalAmount - marketplaceBooking.TotalAmountExcludeTax
                : null;
            marketplaceBooking.TaxRatePercentage = marketplaceBooking.TaxAmount is not null && marketplaceBooking.TotalAmountExcludeTax is not null
                ? (marketplaceBooking.TaxAmount.Value * 100 / marketplaceBooking.TotalAmountExcludeTax.Value).RoundedDecimal()
                : null;
        }

        marketplaceBooking.Currency = session.Currency;
        _ = repositoryFactory.MarketplaceBookingRepository.Update(marketplaceBooking);
        await repositoryFactory.MarketplacePurchaseHistoryRepository.RefreshForMarketplaceBookingAsync(
            marketplaceBooking.Id, cancellationToken);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        if (marketplaceBooking.Booking is not null)
        {
            await temporalService.SignalPayBookingViaCardWorkflowAsync(
                marketplaceBooking.Booking.Id,
                new SetPaymentStatusArgs(marketplaceBooking.PaymentStatus),
                cancellationToken);

            await graphQlTopicEventSender.RaiseGraphqlChangeAsync(Constants.BookingTopicName, marketplaceBooking.Booking.Id, cancellationToken);
        }
        else if (marketplaceBooking.RecurringBooking?.MarketplaceBookingSubscription is not null)
        {
            await temporalService.SignalPayRecurringBookingViaCardWorkflowAsync(
                marketplaceBooking.RecurringBooking.Id,
                new SetPaymentStatusArgs(marketplaceBooking.PaymentStatus),
                cancellationToken);

            await graphQlTopicEventSender.RaiseGraphqlChangeAsync(
                Constants.MarketplaceBookingSubscriptionTopicName,
                marketplaceBooking.RecurringBooking.MarketplaceBookingSubscription.Id,
                cancellationToken);
        }
    }

    private async Task CorrelateReservationStripeSubscriptionAsync(
        MarketplaceBookingSubscriptionEntity marketplaceSubscription,
        Session session,
        string? stripeAccountId,
        CancellationToken cancellationToken)
    {
        var subscriptionId = session.SubscriptionId;
        var stripePriceId = session.Metadata?.GetValueOrDefault("marketplace_stripe_price_id");
        if (string.IsNullOrWhiteSpace(stripeAccountId) || string.IsNullOrWhiteSpace(subscriptionId) ||
            string.IsNullOrWhiteSpace(session.CustomerId) || string.IsNullOrWhiteSpace(stripePriceId))
        {
            throw new InvalidOperationException("Stripe subscription Checkout completed without the required marketplace correlation values.");
        }

        EnsureStripeCorrelationMatches(marketplaceSubscription.StripeAccountId, stripeAccountId, "account");
        EnsureStripeCorrelationMatches(marketplaceSubscription.StripeSubscriptionId, subscriptionId, "subscription");
        marketplaceSubscription.StripeAccountId = stripeAccountId;
        marketplaceSubscription.StripeCustomerId = session.CustomerId;
        marketplaceSubscription.StripeSubscriptionId = subscriptionId;
        marketplaceSubscription.StripePriceId = stripePriceId;
        marketplaceSubscription.StripeSubscriptionStatus ??= "pending";
        marketplaceSubscription.StripeCancelAtPeriodEnd = false;
        repositoryFactory.MarketplaceBookingSubscriptionRepository.Update(marketplaceSubscription);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task CorrelateEntitlementStripeSubscriptionAsync(
        EntitlementPurchase purchase,
        Session session,
        string? stripeAccountId,
        CancellationToken cancellationToken)
    {
        var subscriptionId = session.SubscriptionId;
        var stripePriceId = session.Metadata?.GetValueOrDefault("marketplace_stripe_price_id");
        if (string.IsNullOrWhiteSpace(stripeAccountId) || string.IsNullOrWhiteSpace(subscriptionId) ||
            string.IsNullOrWhiteSpace(session.CustomerId) || string.IsNullOrWhiteSpace(stripePriceId))
        {
            throw new InvalidOperationException("Stripe subscription Checkout completed without the required marketplace correlation values.");
        }

        EnsureStripeCorrelationMatches(purchase.StripeAccountId, stripeAccountId, "account");
        EnsureStripeCorrelationMatches(purchase.StripeSubscriptionId, subscriptionId, "subscription");
        purchase.StripeAccountId = stripeAccountId;
        purchase.StripeCustomerId = session.CustomerId;
        purchase.StripeSubscriptionId = subscriptionId;
        purchase.StripePriceId = stripePriceId;
        purchase.StripeSubscriptionStatus ??= "pending";
        purchase.StripeCancelAtPeriodEnd = false;
        repositoryFactory.EntitlementPurchaseRepository.Update(purchase);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static void EnsureStripeCorrelationMatches(string? persistedValue, string incomingValue, string name)
    {
        if (!string.IsNullOrWhiteSpace(persistedValue) && !string.Equals(persistedValue, incomingValue, StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"Stripe {name} correlation does not match the existing marketplace purchase.");
        }
    }

    private async Task HandleCheckoutSessionExpiredAsync(Event stripeEvent, CancellationToken cancellationToken)
    {
        var session = stripeEvent.Data.Object as Session;
        ArgumentNullException.ThrowIfNull(session);

        var stripeCheckoutSession =
            await repositoryFactory.StripeCheckoutSessionRepository.GetByStripeCheckoutSessionIdAsync(session.Id, cancellationToken);
        if (stripeCheckoutSession is null)
        {
            if (!string.IsNullOrWhiteSpace(session.ClientReferenceId))
            {
                await entitlementPurchaseService.UpdatePaymentStatusAsync(
                    session.ClientReferenceId,
                    PaymentStatus.Expired,
                    new DateTimeOffset(session.Created, TimeSpan.Zero),
                    cancellationToken);
            }

            return;
        }

        var marketplaceBooking = stripeCheckoutSession.MarketplaceBooking;
        marketplaceBooking.PaymentStatus = PaymentStatusConstants.Expired;
        if (marketplaceBooking.RecurringBooking?.MarketplaceBookingSubscription is null)
        {
            marketplaceBooking.TotalAmountExcludeTax = session.AmountSubtotal is null ? null : (decimal)session.AmountSubtotal / 100;
            marketplaceBooking.TotalAmount = session.AmountTotal is null ? null : (decimal)session.AmountTotal / 100;
            marketplaceBooking.TaxAmount = marketplaceBooking.TotalAmountExcludeTax is not null && marketplaceBooking.TotalAmount is not null
                ? marketplaceBooking.TotalAmount - marketplaceBooking.TotalAmountExcludeTax
                : null;
            marketplaceBooking.TaxRatePercentage = marketplaceBooking.TaxAmount is not null && marketplaceBooking.TotalAmountExcludeTax is not null
                ? (marketplaceBooking.TaxAmount.Value * 100 / marketplaceBooking.TotalAmountExcludeTax.Value).RoundedDecimal()
                : null;
        }

        marketplaceBooking.Currency = session.Currency;
        _ = repositoryFactory.MarketplaceBookingRepository.Update(marketplaceBooking);
        await repositoryFactory.MarketplacePurchaseHistoryRepository.RefreshForMarketplaceBookingAsync(
            marketplaceBooking.Id, cancellationToken);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        if (marketplaceBooking.Booking is not null)
        {
            await temporalService.SignalPayBookingViaCardWorkflowAsync(
                marketplaceBooking.Booking.Id,
                new SetPaymentStatusArgs(marketplaceBooking.PaymentStatus),
                cancellationToken);

            await graphQlTopicEventSender.RaiseGraphqlChangeAsync(Constants.BookingTopicName, marketplaceBooking.Booking.Id, cancellationToken);
        }
        else if (marketplaceBooking.RecurringBooking?.MarketplaceBookingSubscription is not null)
        {
            await temporalService.SignalPayRecurringBookingViaCardWorkflowAsync(
                marketplaceBooking.RecurringBooking.Id,
                new SetPaymentStatusArgs(marketplaceBooking.PaymentStatus),
                cancellationToken);

            await graphQlTopicEventSender.RaiseGraphqlChangeAsync(
                Constants.MarketplaceBookingSubscriptionTopicName,
                marketplaceBooking.RecurringBooking.MarketplaceBookingSubscription.Id,
                cancellationToken);
        }
    }
}
