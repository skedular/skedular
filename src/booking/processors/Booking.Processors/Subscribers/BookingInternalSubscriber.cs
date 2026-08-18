using Api.Shared.Clients.Events.Skedular.BookingInternal.V1;
using Api.Shared.Services.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Services.Entitlements;
using Booking.Shared.Workflows;
using Enterprise.Shared;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.Kafka.Consume;
using Stripe;
using Stripe.Checkout;
using Constants = Booking.Shared.GraphQL.Constants;
using Event = Api.Shared.Clients.Events.Skedular.BookingInternal.V1.Event;
using MarketplaceBooking = Booking.Shared.Database.Entities.MarketplaceBooking;
using Type = Api.Shared.Clients.Events.Skedular.BookingInternal.V1.Type;

namespace Booking.Processors.Subscribers;

public class BookingInternalSubscriber(
    IRepositoryFactory repositoryFactory,
    ITemporalService temporalService,
    IXeroWebhookService xeroWebhookService,
    IStripeHostRefundService stripeHostRefundService,
    IStripePayoutReconciliationService payoutReconciliationService,
    IGraphQlTopicEventSender graphQlTopicEventSender,
    IEntitlementPurchaseService entitlementPurchaseService,
    ILogger<BookingInternalSubscriber> logger) : IEventSubscriber<Key, Event>
{
    public async Task<EventSubscriberResult> HandleAsync(EventContext eventContext, Key key, Event @event, CancellationToken cancellationToken)
    {
        logger.LogInformation("Received BookingInternal event. EventType={EventType}", @event.Metadata.Type);
        switch (@event.Metadata.Type)
        {
            case Type.StripeConnectAccountWebhookEventReceived:
                await HandleStripeConnectAccountWebhookEventReceivedAsync(@event.StripeConnectAccountWebhookEventPayload, cancellationToken);
                break;
            case Type.XeroWebhookEventReceived:
                logger.LogInformation("Dispatching Xero webhook event from BookingInternal subscriber. PayloadLength={PayloadLength}",
                    @event.XeroWebhookEventPayload.Length);
                await xeroWebhookService.ProcessAsync(@event.XeroWebhookEventPayload, cancellationToken);
                break;
        }

        return EventSubscriberResults.Success;
    }

    private async Task HandleStripeConnectAccountWebhookEventReceivedAsync(string json, CancellationToken cancellationToken)
    {
        var stripeEvent = EventUtility.ParseEvent(json, false);
        ArgumentNullException.ThrowIfNull(stripeEvent);

        switch (stripeEvent.Type)
        {
            case EventTypes.CheckoutSessionCompleted:
            case "checkout.session.async_payment_succeeded":
                await HandleCheckoutSessionCompletedAsync(stripeEvent, cancellationToken);
                break;

            case EventTypes.CheckoutSessionExpired:
            case "checkout.session.async_payment_failed":
                await HandleCheckoutSessionExpiredAsync(stripeEvent, cancellationToken);
                break;
            case "charge.succeeded" when stripeEvent.Data.Object is Charge charge:
                await HandleChargeSucceededAsync(charge, cancellationToken);
                break;
            case "charge.failed" when stripeEvent.Data.Object is Charge charge:
                await HandleChargeFailedAsync(charge, cancellationToken);
                break;
            case "payment_intent.succeeded" when stripeEvent.Data.Object is PaymentIntent paymentIntent:
                await HandlePaymentIntentSucceededAsync(paymentIntent, cancellationToken);
                break;
            case "payment_intent.payment_failed" or "payment_intent.canceled"
                when stripeEvent.Data.Object is PaymentIntent paymentIntent:
                await HandlePaymentIntentFailedAsync(paymentIntent, cancellationToken);
                break;
            case "payout.paid" or "payout.reconciliation_completed" when stripeEvent.Data.Object is Payout payout:
                await payoutReconciliationService.HandlePaidAsync(
                    payout,
                    stripeEvent.Account,
                    cancellationToken,
                    new DateTimeOffset(stripeEvent.Created, TimeSpan.Zero),
                    stripeEvent.Id);
                break;
            case "payout.failed" or "payout.canceled" or "payout.updated" when stripeEvent.Data.Object is Payout payout:
                await payoutReconciliationService.HandleStateChangedAsync(
                    payout,
                    stripeEvent.Type,
                    cancellationToken,
                    stripeEvent.Account,
                    new DateTimeOffset(stripeEvent.Created, TimeSpan.Zero),
                    stripeEvent.Id);
                break;
            case "refund.created":
            case "refund.updated":
            case "refund.failed":
                if (stripeEvent.Data.Object is Refund refund)
                {
                    var localRefund = await stripeHostRefundService.ReconcileAsync(
                        refund,
                        cancellationToken,
                        stripeEvent.Account,
                        stripeEvent.Id);
                    if (localRefund is not null)
                    {
                        if (stripeEvent.Type == "refund.failed")
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

    internal async Task HandleChargeSucceededAsync(Charge charge, CancellationToken cancellationToken)
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

    private async Task HandleCheckoutSessionCompletedAsync(Stripe.Event stripeEvent, CancellationToken cancellationToken)
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
                var purchase = await repositoryFactory.EntitlementPurchaseRepository.GetByIdAsync(
                    session.ClientReferenceId,
                    cancellationToken);
                if (purchase is null || purchase.StripeCheckoutSessionId != session.Id)
                {
                    return;
                }

                await entitlementPurchaseService.UpdateStripePaymentContextAsync(
                    session.ClientReferenceId,
                    session.Id,
                    session.PaymentIntentId,
                    cancellationToken);
                await entitlementPurchaseService.UpdatePaymentStatusAsync(
                    session.ClientReferenceId,
                    session.PaymentStatus switch
                    {
                        "no_payment_required" => PaymentStatus.NoPaymentRequired,
                        "paid" => PaymentStatus.Confirmed,
                        "unpaid" => PaymentStatus.Pending,
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

    private async Task HandleCheckoutSessionExpiredAsync(Stripe.Event stripeEvent, CancellationToken cancellationToken)
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
