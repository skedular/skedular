namespace Booking.Shared.Activities;

public record CreateCheckoutSessionAsyncInput(string BookingId, string StripeConnectAccountId, string StripeCustomerId);
