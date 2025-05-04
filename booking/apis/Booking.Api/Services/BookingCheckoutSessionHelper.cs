using Enterprise.Shared.Time;

namespace Booking.Api.Services;

public interface IBookingCheckoutSessionHelper
{
    DateTimeOffset GetBookingCheckoutSessionExpiry(Shared.Database.Entities.Booking booking);
}

public class BookingCheckoutSessionHelper : IBookingCheckoutSessionHelper
{
    public DateTimeOffset GetBookingCheckoutSessionExpiry(Shared.Database.Entities.Booking booking) =>
        booking.CreatedAt.TrimAllAfterSeconds().AddMinutes(5);
}
