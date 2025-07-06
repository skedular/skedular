using Api.Shared.Services;
using Enterprise.Shared.Time;

namespace Booking.Shared.Services;

public interface IBookingCheckoutSessionHelperService
{
    DateTimeOffset GetBookingCheckoutSessionExpiry(Database.Entities.Booking booking);
}

public class BookingCheckoutSessionHelperService : IBookingCheckoutSessionHelperService
{
    public DateTimeOffset GetBookingCheckoutSessionExpiry(Database.Entities.Booking booking)
    {
        var allowedTime = Constants.DefaultMaxAllowedResourcesLockTimePaidByCard;
        if (booking.ProductVersions.Count != 0)
        {
            allowedTime = booking.ProductVersions.Select(item => item.MaxAllowedResourcesLockTimePaidByCard).Min();
        }

        return booking.CreatedAt.TrimAllAfterSeconds().AddMinutes(allowedTime);
    }
}
