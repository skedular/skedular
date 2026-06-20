using Api.Shared.Services.Models;

namespace Booking.Shared.Services;

/// <summary>
///     Resolves the booking calendar day that governs a marketplace price. An empty
///     available-days collection deliberately means that the price is available every day.
/// </summary>
public interface IMarketplaceBookingAvailableDaysService
{
    bool IsAvailable(ProductPricing pricing, DateTimeOffset bookingStart, out DateOnly bookingDate);
    bool IsAvailableOnBookingDate(ProductPricing pricing, DateOnly bookingDate);
}

public class MarketplaceBookingAvailableDaysService : IMarketplaceBookingAvailableDaysService
{
    public bool IsAvailable(ProductPricing pricing, DateTimeOffset bookingStart, out DateOnly bookingDate)
    {
        bookingDate = DateOnly.FromDateTime(bookingStart.Date);
        return IsAvailableOnBookingDate(pricing, bookingDate);
    }

    public bool IsAvailableOnBookingDate(ProductPricing pricing, DateOnly bookingDate)
    {
        // Existing prices deserialize without this property. They must retain their
        // unrestricted behavior rather than suddenly becoming unavailable.
        if (pricing.AvailableDays is not { Count: > 0 })
        {
            return true;
        }

        return pricing.AvailableDays.Contains(bookingDate.DayOfWeek);
    }
}
