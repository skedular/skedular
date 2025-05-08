using Enterprise.Shared.Time;

namespace Booking.Shared.Services;

public interface IBookingCheckoutSessionHelperService
{
    DateTimeOffset GetExpiryDateTimeOffset();
    DateTimeOffset GetBookingCheckoutSessionExpiry(Database.Entities.Booking booking);
}

public class BookingCheckoutSessionHelperService(TimeProvider timeProvider) : IBookingCheckoutSessionHelperService
{
    public DateTimeOffset GetExpiryDateTimeOffset() => timeProvider.GetUtcNow().AddMinutes(-5);

    public DateTimeOffset GetBookingCheckoutSessionExpiry(Database.Entities.Booking booking) =>
        booking.CreatedAt.TrimAllAfterSeconds().AddMinutes(5);
}
