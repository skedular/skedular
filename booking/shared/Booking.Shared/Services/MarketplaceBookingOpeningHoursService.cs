using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Enterprise.Shared.Time;

namespace Booking.Shared.Services;

public record MarketplaceBookingDailyPlan(DateTimeOffset From, DateTimeOffset Until, ICollection<Resource> Resources);

public interface IMarketplaceBookingOpeningHoursService
{
    Task<MarketplaceBookingDailyPlan?> TryResolveDailyPlanAsync(
        Customer? customer,
        ProductVersion productVersion,
        ProductPricing pricing,
        DateOnly bookingDay,
        int requiredResourceCount,
        string? preferredLocationId,
        CancellationToken cancellationToken);

    Location? ResolveLocation(Database.Entities.Booking booking);
    bool ShouldUseLocationOpeningHoursWindow(ProductPricingCadence cadence);
}

public class MarketplaceBookingOpeningHoursService(IRepositoryFactory repositoryFactory) : IMarketplaceBookingOpeningHoursService
{
    // Pass-style marketplace products are booked day-by-day by the workflow.
    // For those products we derive the actual booking window from the location's
    // opening hours for that date instead of trusting a user-supplied timestamp.
    public async Task<MarketplaceBookingDailyPlan?> TryResolveDailyPlanAsync(
        Customer? customer,
        ProductVersion productVersion,
        ProductPricing pricing,
        DateOnly bookingDay,
        int requiredResourceCount,
        string? preferredLocationId,
        CancellationToken cancellationToken)
    {
        var productTagIds = productVersion.OrganizationTags
            .Where(item => item.Type == OrganizationTagTypeConstants.Product)
            .Select(item => item.Id)
            .ToList();
        if (productTagIds.Count == 0)
        {
            return null;
        }

        var preferredResourceLocationIds = customer?.PreferredResources
            .Where(item => item.Location is not null)
            .Select(item => item.Location!.Id)
            .Distinct()
            .ToList() ?? [];
        var preferredLocationIds = customer?.PreferredLocations.Select(item => item.Id).Distinct().ToList() ?? [];

        // Opening hours can be overridden at the resource level. That means we cannot pick
        // a single location window up front anymore: we first narrow to candidate locations,
        // then evaluate the effective booking window of each resource in that location.
        var allLocations = await repositoryFactory.LocationRepository.GetAllWithActiveOrganizationAsync(false, cancellationToken);
        var candidateLocations = allLocations
            .Where(location => !location.DeletedAt.HasValue)
            .Where(location =>
                location.Resources.Any(resource =>
                    !resource.DeletedAt.HasValue &&
                    !resource.Inactive &&
                    resource.OrganizationTags.Any(tag => !tag.DeletedAt.HasValue && productTagIds.Contains(tag.Id))))
            .OrderBy(location => GetLocationPriority(location.Id, preferredLocationId, preferredResourceLocationIds, preferredLocationIds))
            .ThenBy(location => location.Id)
            .ToList();

        foreach (var location in candidateLocations)
        {
            var orderedResources = OrderResources(
                    location.Resources
                        .Where(resource => !resource.DeletedAt.HasValue)
                        .Where(resource => !resource.Inactive)
                        .Where(resource => resource.OrganizationTags.Any(tag => !tag.DeletedAt.HasValue && productTagIds.Contains(tag.Id)))
                        .ToList(),
                    customer)
                .ToList();
            var resourceWindows = orderedResources
                .Select(resource => new { Resource = resource, Window = ResolveBookingWindow(resource, bookingDay, pricing) })
                .Where(item => item.Window is not null)
                .ToList();
            if (resourceWindows.Count == 0)
            {
                // If neither the location nor any overriding resource is open on that day,
                // we should not materialize a booking for this location.
                continue;
            }

            foreach (var resourceWindowGroup in resourceWindows
                         .GroupBy(item => new { item.Window!.Value.From, item.Window.Value.Until })
                         .OrderBy(group => group.Min(item => orderedResources.IndexOf(item.Resource))))
            {
                // A marketplace booking has one From/Until window for all selected resources.
                // So when resources have different overridden opening hours, we group them by
                // effective window and only book a set of resources that share the same window.
                var availableResources = await repositoryFactory.ResourceRepository.GetAvailableResourcesAsync(
                    null,
                    location.Id,
                    resourceWindowGroup.Key.From,
                    resourceWindowGroup.Key.Until,
                    resourceWindowGroup.Select(item => item.Resource.Id).ToList(),
                    productTagIds,
                    [],
                    cancellationToken);
                if (availableResources.Count < requiredResourceCount)
                {
                    continue;
                }

                return new MarketplaceBookingDailyPlan(
                    resourceWindowGroup.Key.From,
                    resourceWindowGroup.Key.Until,
                    OrderResources(availableResources, customer).Take(requiredResourceCount).ToList());
            }
        }

        return null;
    }

    // Existing marketplace bookings already know which location/resource they were assigned
    // to, so reconciliation should continue using that location when opening hours change.
    public Location? ResolveLocation(Database.Entities.Booking booking) =>
        booking.InvolvedLocations.FirstOrDefault() ??
        booking.InvolvedResources.FirstOrDefault(item => item.Location is not null)?.Location ??
        booking.ResourceBookingSlots.FirstOrDefault(item => item.Resource.Location is not null)?.Resource.Location;

    // Only day-based pass products should stretch to the full opening-hours window.
    // Half-day and shorter metered products stay out of this path to avoid pricing drift
    // and to keep their explicit time selection model intact.
    public bool ShouldUseLocationOpeningHoursWindow(ProductPricingCadence cadence) =>
        cadence is ProductPricingCadence.Daily or
            ProductPricingCadence.Weekly or
            ProductPricingCadence.Fortnightly or
            ProductPricingCadence.Monthly or
            ProductPricingCadence.TwoMonths or
            ProductPricingCadence.Quarterly or
            ProductPricingCadence.FourMonths or
            ProductPricingCadence.FiveMonths or
            ProductPricingCadence.SixMonths or
            ProductPricingCadence.Yearly;

    private (DateTimeOffset From, DateTimeOffset Until)? ResolveBookingWindow(Location location, DateOnly bookingDay, ProductPricing pricing)
    {
        var dayStart = new DateTimeOffset(bookingDay.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc), TimeSpan.Zero);
        var dayEndExclusive = dayStart.AddDays(1);
        if (!ShouldUseLocationOpeningHoursWindow(pricing.PurchaseCadence))
        {
            // Sub-daily metered pricing keeps its original duration model so reconciliation
            // does not accidentally change the paid duration later.
            return (dayStart, ResolveFallbackUntil(dayStart, pricing.BookingCadence));
        }

        var openingHours = location.OpeningHours ?? OpeningHours.Default;
        if (openingHours.ClosedDates.Any(item => item.StartOfDay() == dayStart))
        {
            return null;
        }

        var openingHoursDetails = GetOpeningHoursDetails(openingHours, dayStart);
        if (openingHoursDetails.Closed)
        {
            return null;
        }

        if (openingHoursDetails.OpenAllDay)
        {
            return (dayStart, dayEndExclusive);
        }

        if (!openingHoursDetails.From.HasValue)
        {
            throw new ArgumentNullException(nameof(openingHoursDetails.From));
        }

        if (!openingHoursDetails.Until.HasValue)
        {
            throw new ArgumentNullException(nameof(openingHoursDetails.Until));
        }

        return (
            bookingDay.ToDateTimeOffset(openingHoursDetails.From.Value.ToTimeSpan()),
            bookingDay.ToDateTimeOffset(openingHoursDetails.Until.Value.ToTimeSpan()));
    }

    private (DateTimeOffset From, DateTimeOffset Until)? ResolveBookingWindow(Resource resource, DateOnly bookingDay, ProductPricing pricing)
    {
        if (resource.IsAvailableHoursOverridden.HasValue && resource.IsAvailableHoursOverridden.Value && resource.AvailableHours is not null)
        {
            // User requirement: if the resource overrides opening hours, day-based marketplace
            // bookings should use that resource window rather than the parent location window.
            return ResolveBookingWindow(resource.AvailableHours, bookingDay, pricing);
        }

        ArgumentNullException.ThrowIfNull(resource.Location);

        // Otherwise the booking falls back to the location opening hours.
        return ResolveBookingWindow(resource.Location, bookingDay, pricing);
    }

    private static IEnumerable<Resource> OrderResources(ICollection<Resource> resources, Customer? customer)
    {
        if (customer is null)
        {
            return resources.OrderBy(item => item.Id);
        }

        var preferredResourceIds = customer.PreferredResources.Select(item => item.Id).ToHashSet();
        var preferredLocationIds = customer.PreferredLocations.Select(item => item.Id).ToHashSet();
        var preferredZoneTagIds = customer.PreferredOrganizationTags
            .Where(item => !string.IsNullOrWhiteSpace(item.Type) && item.Type.ToOrganizationTagType() == OrganizationTagType.Zone)
            .Select(item => item.Id)
            .ToHashSet();
        var preferredCustomTagIds = customer.PreferredOrganizationTags
            .Where(item => !string.IsNullOrWhiteSpace(item.Type) && item.Type.ToOrganizationTagType() == OrganizationTagType.Custom)
            .Select(item => item.Id)
            .ToHashSet();

        return resources
            .OrderBy(resource => preferredResourceIds.Contains(resource.Id) ? 0 : 1)
            .ThenBy(resource => resource.Location is not null && preferredLocationIds.Contains(resource.Location.Id) ? 0 : 1)
            .ThenBy(resource => resource.OrganizationTags.Any(tag => preferredZoneTagIds.Contains(tag.Id)) ? 0 : 1)
            .ThenBy(resource => resource.OrganizationTags.Any(tag => preferredCustomTagIds.Contains(tag.Id)) ? 0 : 1)
            .ThenBy(resource => resource.Id);
    }

    private static int GetLocationPriority(
        string locationId,
        string? preferredLocationId,
        List<string> preferredResourceLocationIds,
        List<string> preferredLocationIds) =>
        !string.IsNullOrWhiteSpace(preferredLocationId) && locationId == preferredLocationId ? 0 :
        preferredResourceLocationIds.Contains(locationId) ? 1 :
        preferredLocationIds.Contains(locationId) ? 2 : 3;

    private static OpeningHoursDetails GetOpeningHoursDetails(OpeningHours openingHours, DateTimeOffset dayStart) =>
        openingHours.DatesWithVariedOpeningHours.TryGetValue(dayStart, out var variedOpeningHours)
            ? variedOpeningHours
            : dayStart.DayOfWeek switch
            {
                DayOfWeek.Monday => openingHours.WeekOpeningHours.Monday,
                DayOfWeek.Tuesday => openingHours.WeekOpeningHours.Tuesday,
                DayOfWeek.Wednesday => openingHours.WeekOpeningHours.Wednesday,
                DayOfWeek.Thursday => openingHours.WeekOpeningHours.Thursday,
                DayOfWeek.Friday => openingHours.WeekOpeningHours.Friday,
                DayOfWeek.Saturday => openingHours.WeekOpeningHours.Saturday,
                DayOfWeek.Sunday => openingHours.WeekOpeningHours.Sunday,
                _ => throw new ArgumentOutOfRangeException()
            };

    private (DateTimeOffset From, DateTimeOffset Until)? ResolveBookingWindow(OpeningHours openingHours, DateOnly bookingDay, ProductPricing pricing)
    {
        var dayStart = new DateTimeOffset(bookingDay.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc), TimeSpan.Zero);
        var dayEndExclusive = dayStart.AddDays(1);
        if (!ShouldUseLocationOpeningHoursWindow(pricing.PurchaseCadence))
        {
            return (dayStart, ResolveFallbackUntil(dayStart, pricing.BookingCadence));
        }

        if (openingHours.ClosedDates.Any(item => item.StartOfDay() == dayStart))
        {
            return null;
        }

        var openingHoursDetails = GetOpeningHoursDetails(openingHours, dayStart);
        if (openingHoursDetails.Closed)
        {
            return null;
        }

        if (openingHoursDetails.OpenAllDay)
        {
            return (dayStart, dayEndExclusive);
        }

        ArgumentNullException.ThrowIfNull(openingHoursDetails.From);
        ArgumentNullException.ThrowIfNull(openingHoursDetails.Until);

        return (
            bookingDay.ToDateTimeOffset(openingHoursDetails.From.Value.ToTimeSpan()),
            bookingDay.ToDateTimeOffset(openingHoursDetails.Until.Value.ToTimeSpan()));
    }

    private static DateTimeOffset ResolveFallbackUntil(DateTimeOffset from, ProductPricingCadence cadence) =>
        cadence switch
        {
            ProductPricingCadence.PerMinute => from.AddMinutes(1),
            ProductPricingCadence.Per15Minutes => from.AddMinutes(15),
            ProductPricingCadence.Per30Minutes => from.AddMinutes(30),
            ProductPricingCadence.PerHour => from.AddHours(1),
            ProductPricingCadence.HalfDay => from.AddHours(4),
            _ => from.AddDays(1)
        };
}
