using Api.Shared.Services;
using Api.Shared.Services.Models;

namespace Booking.Shared.Services;

public interface IMarketplaceBookingWeeklyDaySelectionService
{
    IReadOnlyList<DayOfWeek> Validate(ProductPricing pricing, IEnumerable<DayOfWeek> selectedDays);
}

public class MarketplaceBookingWeeklyDaySelectionService : IMarketplaceBookingWeeklyDaySelectionService
{
    public IReadOnlyList<DayOfWeek> Validate(ProductPricing pricing, IEnumerable<DayOfWeek> selectedDays)
    {
        var requestedDays = selectedDays.ToList();
        var selectedDaysDistinct = requestedDays.Distinct().ToList();
        if (selectedDaysDistinct.Count != requestedDays.Count ||
            (pricing.PurchaseCadence == ProductPricingCadence.Daily && selectedDaysDistinct.Count != 0))
        {
            throw new MarketplaceBookingWeeklyDaySelectionInvalid();
        }

        if (pricing.RequiredDaysPerWeek is null)
        {
            return [];
        }

        if (selectedDaysDistinct.Count != pricing.RequiredDaysPerWeek)
        {
            throw new MarketplaceBookingWeeklyDaySelectionInvalid();
        }

        var availableDays = pricing.AvailableDays is { Count: > 0 }
            ? pricing.AvailableDays.ToHashSet()
            : Enum.GetValues<DayOfWeek>().ToHashSet();
        return selectedDaysDistinct.Any(day => !availableDays.Contains(day))
            ? throw new MarketplaceBookingWeeklyDaySelectionInvalid()
            : selectedDaysDistinct;
    }

    internal static bool UsesFixedWeeklySchedule(ProductPricing pricing, IReadOnlyCollection<DayOfWeek> selectedDays) =>
        pricing.PurchaseCadence != ProductPricingCadence.Daily &&
        pricing.PurchaseCadence != ProductPricingCadence.NotSet &&
        pricing.RequiredDaysPerWeek is not null &&
        selectedDays.Count > 0;
}
