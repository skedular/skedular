namespace Booking.Shared.Activities;

public record UpsertBookingRelatedStripeCustomerInput(string BookingId, string StripeConnectAccountId);
