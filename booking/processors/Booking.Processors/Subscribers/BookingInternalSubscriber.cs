using Api.Shared.Clients.Events.Skedular.BookingInternal.V1.Key;
using Api.Shared.Services.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Workflows;
using Enterprise.Shared;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.Kafka.Consume;
using Stripe;
using Stripe.Checkout;
using Constants = Booking.Shared.GraphQL.Constants;
using Event = Api.Shared.Clients.Events.Skedular.BookingInternal.V1.Value.Event;
using Type = Api.Shared.Clients.Events.Skedular.BookingInternal.V1.Value.Type;

namespace Booking.Processors.Subscribers;

public class BookingInternalSubscriber(
    IRepositoryFactory repositoryFactory,
    ITemporalService temporalService,
    IGraphQlTopicEventSender graphQlTopicEventSender) : IEventSubscriber<Key, Event>
{
    public async Task<EventSubscriberResult> HandleAsync(EventContext eventContext, Key key, Event @event, CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.StripeConnectAccountWebhookEventReceived:
                await HandleStripeConnectAccountWebhookEventReceivedAsync(@event.StripeConnectAccountWebhookEventPayload, cancellationToken);
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
                await HandleCheckoutSessionCompletedAsync(stripeEvent, cancellationToken);
                break;

            case EventTypes.CheckoutSessionExpired:
                await HandleCheckoutSessionExpiredAsync(stripeEvent, cancellationToken);
                break;
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
            return;
        }

        var marketplaceBooking = stripeCheckoutSession.MarketplaceBooking;
        marketplaceBooking.PaymentStatus = session.PaymentStatus switch
        {
            "no_payment_required" => PaymentStatusConstants.NoPaymentRequired,
            "paid" => PaymentStatusConstants.Confirmed,
            "unpaid" => PaymentStatusConstants.Rejected,
            _ => throw new ArgumentOutOfRangeException()
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
