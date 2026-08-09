using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Enterprise.Shared.Time;

namespace Booking.Shared.Services;

/// <summary>
///     Represents a daily booking plan for marketplace bookings, including the time window and assigned resources.
/// </summary>
/// <param name="From">The start time of the booking window.</param>
/// <param name="Until">The end time of the booking window.</param>
/// <param name="Resources">The collection of resources assigned to this booking plan.</param>
public record MarketplaceBookingDailyPlan(DateTimeOffset From, DateTimeOffset Until, IReadOnlyList<Resource> Resources);

/// <summary>
///     Service for managing marketplace booking opening hours and resolving daily booking plans.
///     Handles the logic for determining available booking windows based on location and resource opening hours.
/// </summary>
public interface IMarketplaceBookingOpeningHoursService
{
    /// <summary>
    ///     Attempts to resolve a daily booking plan for marketplace products.
    ///     For pass-style marketplace products, derives the booking window from location opening hours
    ///     instead of trusting user-supplied timestamps.
    /// </summary>
    /// <param name="customer">The customer making the booking, used for preferences.</param>
    /// <param name="productVersion">The product version being booked.</param>
    /// <param name="pricing">The pricing information for the product.</param>
    /// <param name="bookingDay">The date for which to resolve the booking plan.</param>
    /// <param name="requiredResourceCount">The number of resources required for the booking.</param>
    /// <param name="requiredResourceIds">Collection of exact resource IDs that must be used.</param>
    /// <param name="preferredResourceIds">Collection of preferred resource IDs.</param>
    /// <param name="preferredLocationId">The preferred location ID, if any.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A daily booking plan if one can be resolved, null otherwise.</returns>
    Task<MarketplaceBookingDailyPlan?> TryResolveDailyPlanAsync(
        Customer? customer,
        ProductVersion productVersion,
        ProductPricing pricing,
        DateOnly bookingDay,
        int requiredResourceCount,
        IReadOnlyList<string> requiredResourceIds,
        IReadOnlyList<string> preferredResourceIds,
        string? preferredLocationId,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Resolves the location for an existing marketplace booking.
    ///     Uses the booking's involved locations, resources, or resource booking slots to determine the location.
    /// </summary>
    /// <param name="booking">The booking for which to resolve the location.</param>
    /// <returns>The resolved location, or null if none can be determined.</returns>
    Location? ResolveLocation(Database.Entities.Booking booking);

    /// <summary>
    ///     Determines whether location opening hours window should be used for the given pricing cadence.
    ///     Only day-based pass products should stretch to the full opening-hours window.
    /// </summary>
    /// <param name="cadence">The product pricing cadence to evaluate.</param>
    /// <returns>True if location opening hours window should be used, false otherwise.</returns>
    bool ShouldUseLocationOpeningHoursWindow(ProductPricingCadence cadence);

    /// <summary>
    ///     Resolves the effective daily opening-hours booking window for a specific resource.
    ///     Resource-level available-hours overrides take precedence over the parent location.
    /// </summary>
    /// <param name="resource">The resource to evaluate.</param>
    /// <param name="bookingDay">The day to book.</param>
    /// <returns>The effective opening-hours window, or null when the resource/location is closed.</returns>
    (DateTimeOffset From, DateTimeOffset Until)? ResolveDailyBookingWindow(Resource resource, DateOnly bookingDay);
}

/// <summary>
///     Implementation of the marketplace booking opening hours service.
/// </summary>
public class MarketplaceBookingOpeningHoursService(IRepositoryFactory repositoryFactory) : IMarketplaceBookingOpeningHoursService
{
    // Pass-style marketplace products are booked day-by-day by the workflow.
    // For those products we derive the actual booking window from the location's
    // opening hours for that date instead of trusting a user-supplied timestamp.
    /// <summary>
    ///     Attempts to resolve a daily booking plan for marketplace products.
    ///     For pass-style marketplace products, derives the booking window from location opening hours
    ///     instead of trusting user-supplied timestamps.
    /// </summary>
    /// <param name="customer">The customer making the booking, used for preferences.</param>
    /// <param name="productVersion">The product version being booked.</param>
    /// <param name="pricing">The pricing information for the product.</param>
    /// <param name="bookingDay">The date for which to resolve the booking plan.</param>
    /// <param name="requiredResourceCount">The number of resources required for the booking.</param>
    /// <param name="requiredResourceIds">Collection of exact resource IDs that must be used.</param>
    /// <param name="preferredResourceIds">Collection of preferred resource IDs.</param>
    /// <param name="preferredLocationId">The preferred location ID, if any.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A daily booking plan if one can be resolved, null otherwise.</returns>
    public async Task<MarketplaceBookingDailyPlan?> TryResolveDailyPlanAsync(
        Customer? customer,
        ProductVersion productVersion,
        ProductPricing pricing,
        DateOnly bookingDay,
        int requiredResourceCount,
        IReadOnlyList<string> requiredResourceIds,
        IReadOnlyList<string> preferredResourceIds,
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
        var allLocations = await repositoryFactory.LocationRepository.GetAllWithActiveOrganizationAsync(false, false, [], cancellationToken);
        var candidateLocations = allLocations
            .Where(location => !location.DeletedAt.HasValue)
            .Where(location =>
                location.Resources.Any(resource =>
                    !resource.DeletedAt.HasValue &&
                    !resource.Inactive &&
                    (requiredResourceIds.Count == 0 || requiredResourceIds.Contains(resource.Id)) &&
                    resource.OrganizationTags.Any(tag => !tag.DeletedAt.HasValue && productTagIds.Contains(tag.Id))))
            .OrderBy(location => GetLocationPriority(location.Id, preferredLocationId, preferredResourceLocationIds, preferredLocationIds))
            .ThenBy(location => location.Id)
            .ToList();

        foreach (var location in candidateLocations)
        {
            var orderedResources = OrderResources(
                    [
                        .. location.Resources
                            .Where(resource => !resource.DeletedAt.HasValue)
                            .Where(resource => !resource.Inactive)
                            .Where(resource => requiredResourceIds.Count == 0 || requiredResourceIds.Contains(resource.Id))
                            .Where(resource => resource.OrganizationTags.Any(tag => !tag.DeletedAt.HasValue && productTagIds.Contains(tag.Id))),
                    ],
                    customer,
                    preferredResourceIds)
                .ToList();
            var resourceWindows = orderedResources
                .Select(resource => new
                {
                    Resource = resource,
                    Window = ResolveBookingWindow(resource, bookingDay, pricing),
                })
                .Where(item => item.Window is not null)
                .ToList();
            if (resourceWindows.Count == 0)
            {
                // If neither the location nor any overriding resource is open on that day,
                // we should not materialize a booking for this location.
                continue;
            }

            foreach (var resourceWindowGroup in resourceWindows
                         .GroupBy(item => new
                         {
                             item.Window!.Value.From,
                             item.Window.Value.Until,
                         })
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
                    [.. resourceWindowGroup.Select(item => item.Resource.Id)],
                    productTagIds,
                    [],
                    [],
                    cancellationToken);
                if (availableResources.Count < requiredResourceCount)
                {
                    continue;
                }

                return new MarketplaceBookingDailyPlan(
                    resourceWindowGroup.Key.From,
                    resourceWindowGroup.Key.Until,
                    [.. OrderResources(availableResources, customer, preferredResourceIds).Take(requiredResourceCount)]);
            }
        }

        return null;
    }

    // Existing marketplace bookings already know which location/resource they were assigned
    // to, so reconciliation should continue using that location when opening hours change.
    /// <summary>
    ///     Resolves the location for an existing marketplace booking.
    ///     Uses the booking's involved locations, resources, or resource booking slots to determine the location.
    /// </summary>
    /// <param name="booking">The booking for which to resolve the location.</param>
    /// <returns>The resolved location, or null if none can be determined.</returns>
    public Location? ResolveLocation(Database.Entities.Booking booking) =>
        booking.InvolvedLocations.FirstOrDefault() ??
        booking.InvolvedResources.FirstOrDefault(item => item.Location is not null)?.Location ??
        booking.ResourceBookingSlots.FirstOrDefault(item => item.Resource.Location is not null)?.Resource.Location;

    // Only day-based pass products should stretch to the full opening-hours window.
    // Half-day and shorter metered products stay out of this path to avoid pricing drift
    // and to keep their explicit time selection model intact.
    /// <summary>
    ///     Determines whether location opening hours window should be used for the given pricing cadence.
    ///     Only day-based pass products should stretch to the full opening-hours window.
    /// </summary>
    /// <param name="cadence">The product pricing cadence to evaluate.</param>
    /// <returns>True if location opening hours window should be used, false otherwise.</returns>
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

    public (DateTimeOffset From, DateTimeOffset Until)? ResolveDailyBookingWindow(Resource resource, DateOnly bookingDay) =>
        ResolveBookingWindow(resource, bookingDay);

    /// <summary>
    ///     Resolves the booking window for a location on a specific booking day.
    ///     Determines the available time window based on the location's opening hours and pricing cadence.
    /// </summary>
    /// <param name="location">The location for which to resolve the booking window.</param>
    /// <param name="bookingDay">The date for the booking.</param>
    /// <param name="pricing">The pricing information that determines the window behavior.</param>
    /// <returns>A tuple with From and Until times if the location is open, null otherwise.</returns>
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

    /// <summary>
    ///     Resolves the booking window for a resource on a specific booking day.
    ///     If the resource has overridden opening hours, uses those; otherwise falls back to location hours.
    /// </summary>
    /// <param name="resource">The resource for which to resolve the booking window.</param>
    /// <param name="bookingDay">The date for the booking.</param>
    /// <param name="pricing">The pricing information that determines the window behavior.</param>
    /// <returns>A tuple with From and Until times if the resource is available, null otherwise.</returns>
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

    private (DateTimeOffset From, DateTimeOffset Until)? ResolveBookingWindow(Resource resource, DateOnly bookingDay)
    {
        if (resource.IsAvailableHoursOverridden.HasValue && resource.IsAvailableHoursOverridden.Value && resource.AvailableHours is not null)
        {
            return ResolveOpeningHoursWindow(resource.AvailableHours, bookingDay);
        }

        ArgumentNullException.ThrowIfNull(resource.Location);

        return ResolveOpeningHoursWindow(resource.Location.OpeningHours ?? OpeningHours.Default, bookingDay);
    }

    /// <summary>
    ///     Orders resources based on customer preferences and other priority criteria.
    ///     Prioritizes resources based on customer preferences, preferred locations, and tags.
    /// </summary>
    /// <param name="resources">The collection of resources to order.</param>
    /// <param name="customer">The customer whose preferences should be considered.</param>
    /// <param name="preferredResourceIds">Collection of preferred resource IDs from the booking request.</param>
    /// <returns>An ordered enumerable of resources.</returns>
    private static IEnumerable<Resource> OrderResources(
        IReadOnlyList<Resource> resources,
        Customer? customer,
        IReadOnlyList<string> preferredResourceIds)
    {
        var preferredGeneratedResourceIds = preferredResourceIds.ToHashSet();
        if (customer is null)
        {
            return resources
                .OrderBy(resource => preferredGeneratedResourceIds.Contains(resource.Id) ? 0 : 1)
                .ThenBy(item => item.Id);
        }

        var preferredCustomerResourceIds = customer.PreferredResources.Select(item => item.Id).ToHashSet();
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
            // Keep cadence-generated marketplace bookings sticky when the same resource is
            // still available for the next generated instance in the same series.
            .OrderBy(resource => preferredGeneratedResourceIds.Contains(resource.Id) ? 0 : 1)
            .ThenBy(resource => preferredCustomerResourceIds.Contains(resource.Id) ? 0 : 1)
            .ThenBy(resource => resource.Location is not null && preferredLocationIds.Contains(resource.Location.Id) ? 0 : 1)
            .ThenBy(resource => resource.OrganizationTags.Any(tag => preferredZoneTagIds.Contains(tag.Id)) ? 0 : 1)
            .ThenBy(resource => resource.OrganizationTags.Any(tag => preferredCustomTagIds.Contains(tag.Id)) ? 0 : 1)
            .ThenBy(resource => resource.Id);
    }

    /// <summary>
    ///     Gets the priority score for a location based on customer preferences.
    ///     Lower scores indicate higher priority.
    /// </summary>
    /// <param name="locationId">The ID of the location to evaluate.</param>
    /// <param name="preferredLocationId">The explicitly preferred location ID.</param>
    /// <param name="preferredResourceLocationIds">Location IDs from preferred resources.</param>
    /// <param name="preferredLocationIds">All preferred location IDs for the customer.</param>
    /// <returns>A priority score (0 = highest priority).</returns>
    private static int GetLocationPriority(
        string locationId,
        string? preferredLocationId,
        List<string> preferredResourceLocationIds,
        List<string> preferredLocationIds) =>
        !string.IsNullOrWhiteSpace(preferredLocationId) && locationId == preferredLocationId ? 0 :
        preferredResourceLocationIds.Contains(locationId) ? 1 :
        preferredLocationIds.Contains(locationId) ? 2 : 3;

    /// <summary>
    ///     Gets the opening hours details for a specific day from the opening hours configuration.
    ///     Checks for varied opening hours first, then falls back to weekly schedule.
    /// </summary>
    /// <param name="openingHours">The opening hours configuration.</param>
    /// <param name="dayStart">The start of the day to check.</param>
    /// <returns>The opening hours details for the specified day.</returns>
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
                _ => throw new ArgumentOutOfRangeException(null,
                    "Unexpected value encountered. Update enum mapping or caller input to include this case."),
            };

    /// <summary>
    ///     Resolves the booking window from opening hours configuration for a specific day.
    ///     Handles closed dates, all-day openings, and specific time ranges.
    /// </summary>
    /// <param name="openingHours">The opening hours configuration.</param>
    /// <param name="bookingDay">The date for the booking.</param>
    /// <param name="pricing">The pricing information that determines the window behavior.</param>
    /// <returns>A tuple with From and Until times if available, null otherwise.</returns>
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

    private static (DateTimeOffset From, DateTimeOffset Until)? ResolveOpeningHoursWindow(OpeningHours openingHours, DateOnly bookingDay)
    {
        var dayStart = new DateTimeOffset(bookingDay.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc), TimeSpan.Zero);
        var dayEndExclusive = dayStart.AddDays(1);

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

    /// <summary>
    ///     Resolves a fallback end time for bookings that don't use location opening hours.
    ///     Uses the pricing cadence to determine the duration from the start time.
    /// </summary>
    /// <param name="from">The start time of the booking.</param>
    /// <param name="cadence">The pricing cadence that determines the duration.</param>
    /// <returns>The calculated end time.</returns>
    private static DateTimeOffset ResolveFallbackUntil(DateTimeOffset from, ProductPricingCadence cadence) =>
        cadence switch
        {
            ProductPricingCadence.PerMinute => from.AddMinutes(1),
            ProductPricingCadence.Per15Minutes => from.AddMinutes(15),
            ProductPricingCadence.Per30Minutes => from.AddMinutes(30),
            ProductPricingCadence.PerHour => from.AddHours(1),
            ProductPricingCadence.HalfDay => from.AddHours(4),
            _ => from.AddDays(1),
        };
}
