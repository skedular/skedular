namespace Booking.Shared.Workflows.Activities;

public record CreateCheckoutSessionAsyncInput(string BookingId, string StripeConnectAccountId, string StripeCustomerId);
