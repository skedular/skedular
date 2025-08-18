using Api.Shared.Clients.Events.Skedular.BookingInternal.V1.Key;
using Api.Shared.Services.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Workflows.Payment;
using Enterprise.Shared;
using Enterprise.Shared.Kafka.Consume;
using Stripe;
using Stripe.Checkout;
using Event = Api.Shared.Clients.Events.Skedular.BookingInternal.V1.Value.Event;
using Type = Api.Shared.Clients.Events.Skedular.BookingInternal.V1.Value.Type;

namespace Booking.Processors.Subscribers;

public class BookingInternalSubscriber(IRepositoryFactory repositoryFactory, ITemporalService temporalService) : IEventSubscriber<Key, Event>
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

        var stripeCheckoutSession =
            await repositoryFactory.StripeCheckoutSessionRepository.GetByStripeCheckoutSessionIdAsync(session.Id, cancellationToken);
        if (stripeCheckoutSession is null)
        {
            return;
        }

        stripeCheckoutSession.Booking.PaymentStatus = session.PaymentStatus switch
        {
            "no_payment_required" => PaymentStatusConstants.NoPaymentRequired,
            "paid" => PaymentStatusConstants.Confirmed,
            "unpaid" => PaymentStatusConstants.Rejected,
            _ => throw new ArgumentOutOfRangeException()
        };


        stripeCheckoutSession.Booking.TotalAmountExcludeTax = session.AmountSubtotal is null ? null : (decimal)session.AmountSubtotal / 100;
        stripeCheckoutSession.Booking.TotalAmount = session.AmountTotal is null ? null : (decimal)session.AmountTotal / 100;
        stripeCheckoutSession.Booking.TaxAmount =
            stripeCheckoutSession.Booking.TotalAmountExcludeTax is not null && stripeCheckoutSession.Booking.TotalAmount is not null
                ? stripeCheckoutSession.Booking.TotalAmount - stripeCheckoutSession.Booking.TotalAmountExcludeTax
                : null;
        stripeCheckoutSession.Booking.TaxRatePercentage =
            stripeCheckoutSession.Booking.TaxAmount is not null && stripeCheckoutSession.Booking.TotalAmountExcludeTax is not null
                ? (stripeCheckoutSession.Booking.TaxAmount.Value * 100 / stripeCheckoutSession.Booking.TotalAmountExcludeTax.Value).RoundedDecimal()
                : null;
        stripeCheckoutSession.Booking.Currency = session.Currency;
        _ = repositoryFactory.BookingRepository.Update(stripeCheckoutSession.Booking);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        await temporalService.SignalPayBookingViaCardWorkflowAsync(
            stripeCheckoutSession.Booking.Id,
            new SetPaymentStatusArgs(stripeCheckoutSession.Booking.PaymentStatus),
            cancellationToken);
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

        stripeCheckoutSession.Booking.PaymentStatus = PaymentStatusConstants.Expired;
        stripeCheckoutSession.Booking.TotalAmountExcludeTax = session.AmountSubtotal is null ? null : (decimal)session.AmountSubtotal / 100;
        stripeCheckoutSession.Booking.TotalAmount = session.AmountTotal is null ? null : (decimal)session.AmountTotal / 100;
        stripeCheckoutSession.Booking.TaxAmount =
            stripeCheckoutSession.Booking.TotalAmountExcludeTax is not null && stripeCheckoutSession.Booking.TotalAmount is not null
                ? stripeCheckoutSession.Booking.TotalAmount - stripeCheckoutSession.Booking.TotalAmountExcludeTax
                : null;
        stripeCheckoutSession.Booking.TaxRatePercentage =
            stripeCheckoutSession.Booking.TaxAmount is not null && stripeCheckoutSession.Booking.TotalAmountExcludeTax is not null
                ? (stripeCheckoutSession.Booking.TaxAmount.Value * 100 / stripeCheckoutSession.Booking.TotalAmountExcludeTax.Value).RoundedDecimal()
                : null;

        stripeCheckoutSession.Booking.Currency = session.Currency;
        _ = repositoryFactory.BookingRepository.Update(stripeCheckoutSession.Booking);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        await temporalService.SignalPayBookingViaCardWorkflowAsync(
            stripeCheckoutSession.Booking.Id,
            new SetPaymentStatusArgs(stripeCheckoutSession.Booking.PaymentStatus),
            cancellationToken);
    }
}
