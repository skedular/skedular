namespace Booking.Shared.Workflows.Activities;

public record UpsertBookingRelatedStripeCustomerInput(string BookingId, string StripeConnectAccountId);
