using System.Text.Json;
using Booking.Shared.Publishers;
using Stripe;
using Stripe.Checkout;

namespace Booking.Api.Services;

public interface IBookingStripeWebhookService
{
    Task PublishBookingEventAsync(Event stripeEvent, string json, CancellationToken cancellationToken);
    string? GetEventObjectId(Event stripeEvent, string json);
}

public sealed class BookingStripeWebhookService(IBookingInternalPublisher bookingInternalPublisher) : IBookingStripeWebhookService
{
    public async Task PublishBookingEventAsync(Event stripeEvent, string json, CancellationToken cancellationToken)
    {
        var eventObjectId = stripeEvent.Type switch
        {
            EventTypes.CheckoutSessionCompleted or EventTypes.CheckoutSessionExpired or
                EventTypes.CheckoutSessionAsyncPaymentSucceeded or EventTypes.CheckoutSessionAsyncPaymentFailed
                when stripeEvent.Data.Object is Session session => session.Id,
            EventTypes.RefundCreated or EventTypes.RefundUpdated or EventTypes.RefundFailed
                when stripeEvent.Data.Object is Refund refund => refund.Id,
            EventTypes.ChargeSucceeded or EventTypes.ChargeFailed
                when stripeEvent.Data.Object is Charge charge => charge.Id,
            EventTypes.PaymentIntentSucceeded or EventTypes.PaymentIntentPaymentFailed or EventTypes.PaymentIntentCanceled
                when stripeEvent.Data.Object is PaymentIntent paymentIntent => paymentIntent.Id,
            EventTypes.InvoicePaid or EventTypes.InvoicePaymentSucceeded
                when stripeEvent.Data.Object is Invoice invoice => invoice.Id,
            EventTypes.InvoicePaymentPaid
                when stripeEvent.Data.Object is InvoicePayment invoicePayment => invoicePayment.Id,
            EventTypes.PayoutPaid or EventTypes.PayoutReconciliationCompleted or EventTypes.PayoutFailed or EventTypes.PayoutCanceled
                or EventTypes.PayoutUpdated
                when stripeEvent.Data.Object is Payout payout => payout.Id,
            _ => GetEventObjectId(stripeEvent, json),
        };

        if (!string.IsNullOrWhiteSpace(eventObjectId))
        {
            await bookingInternalPublisher.PublishStripeConnectAccountWebhookEventReceivedAsync(
                eventObjectId,
                json,
                cancellationToken);
        }
    }

    public string? GetEventObjectId(Event stripeEvent, string json)
    {
        var objectId = stripeEvent.Data.Object switch
        {
            Refund refund => refund.Id,
            Charge charge => charge.Id,
            Payout payout => payout.Id,
            Session session => session.Id,
            Invoice invoice => invoice.Id,
            InvoicePayment invoicePayment => invoicePayment.Id,
            _ => null,
        };

        if (!string.IsNullOrWhiteSpace(objectId))
        {
            return objectId;
        }

        try
        {
            using var document = JsonDocument.Parse(json);
            return document.RootElement.GetProperty("data").GetProperty("object").GetProperty("id").GetString();
        }
        catch (JsonException)
        {
            return null;
        }
        catch (KeyNotFoundException)
        {
            return null;
        }
    }
}
