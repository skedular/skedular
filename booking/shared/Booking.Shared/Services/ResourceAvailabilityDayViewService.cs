using System.Diagnostics;
using Api.Shared.Services.Models;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using DbResource = Booking.Shared.Database.Entities.Resource;
using DbResourceBookingSlot = Booking.Shared.Database.Entities.ResourceBookingSlot;

namespace Booking.Shared.Services;

/// <summary>
///     Computes resource availability day views for a given date and filter.
///     Reads live booking data directly from the Booking domain DB — no cross-domain gRPC,
///     no analytics snapshots.
/// </summary>
public interface IResourceAvailabilityDayViewService
{
    /// <summary>
    ///     Returns the full sorted list of resource day views matching the given filter,
    ///     along with an opaque subscription key the client can use to subscribe to real-time updates.
    /// </summary>
    /// <param name="filter">Date, location, floor, zone, resource type, and status constraints.</param>
    /// <param name="orderBy">
    ///     Ordered list of sort clauses. The first clause is the primary sort; subsequent clauses
    ///     act as tie-breakers. When <c>null</c> or empty, defaults to
    ///     <see cref="ResourceAvailabilityOrderByField.ResourceName" /> ascending.
    /// </param>
    /// <param name="requestingUserRoles">The authenticated user's roles, used for booking detail visibility.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    ///     A <see cref="ResourceDayViewResult" /> containing the sorted items and the opaque
    ///     <see cref="ResourceDayViewResult.SubscriptionKey" />.
    /// </returns>
    Task<ResourceDayViewResult> GetAsync(
        ResourceAvailabilityDayFilter filter,
        IReadOnlyList<ResourceAvailabilityOrder> orderBy,
        IReadOnlyList<string> requestingUserRoles,
        CancellationToken cancellationToken);
}

public class ResourceAvailabilityDayViewService(
    // IRepositoryFactory is used here — rather than injecting IResourceRepository and
    // IResourceBookingSlotRepository directly — because Booking.Api uses a pooled
    // DbContext factory. Direct repository injection bypasses that pool and causes a DI
    // validation failure at startup. IRepositoryFactory creates its own DbContext from
    // the pool and exposes all repositories through a single scoped instance.
    IRepositoryFactory repositoryFactory,
    IResourceAvailabilityClassifier classifier,
    IResourceDayViewBookingVisibilityFilter visibilityFilter,
    ISubscriptionKeyService subscriptionKeyService,
    ILogger<ResourceAvailabilityDayViewService> logger,
    IOptions<ResourceAvailabilityOptions> options)
    : IResourceAvailabilityDayViewService
{
    public async Task<ResourceDayViewResult> GetAsync(
        ResourceAvailabilityDayFilter filter,
        IReadOnlyList<ResourceAvailabilityOrder> orderBy,
        IReadOnlyList<string> requestingUserRoles,
        CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();

        logger.LogDebug(
            "ResourceAvailabilityDayView query started. Date={Date} OrganizationCustomDomain={OrganizationCustomDomain} LocationIds={LocationIds} ResourceType={ResourceType} StatusCount={StatusCount}",
            filter.Date, filter.OrganizationCustomDomain, string.Join(",", filter.LocationIds), filter.ResourceType, filter.Statuses.Count);

        var dayStart = new DateTimeOffset(filter.Date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc), TimeSpan.Zero);
        var dayEnd = dayStart.AddDays(1);

        // Load resources with their location, organization tags, and booking slots for the day
        var resources = await repositoryFactory.ResourceRepository.GetForAvailabilityDayViewAsync(
            filter.OrganizationCustomDomain,
            filter.LocationIds,
            filter.ZoneId,
            filter.ResourceType,
            orderBy,
            cancellationToken);

        // Load booking slots that overlap with the day for those resource IDs
        var resourceIds = resources.Select(r => r.Id).ToList();
        var slots = await repositoryFactory.ResourceBookingSlotRepository.GetByResourceIdsAndDayAsync(resourceIds, dayStart, dayEnd,
            cancellationToken);
        var slotsByResource = slots.GroupBy(s => s.ResourceId).ToDictionary(g => g.Key, g => g.ToList());

        var views = new List<ResourceDayView>(resources.Count);
        foreach (var resource in resources)
        {
            var view = BuildDayView(resource, filter.Date, dayStart, slotsByResource, classifier);

            // LOG-002: state-transition diagnostic per resource
            logger.LogDebug(
                "ResourceAvailabilityDayView status computed. ResourceId={ResourceId} ResourceName={ResourceName} Status={Status} BookedMinutes={BookedMinutes} TotalOpeningMinutes={TotalOpeningMinutes}",
                resource.Id, resource.Name, view.Status, view.BookedMinutes, view.TotalOpeningMinutes);

            if (filter.Statuses.Count > 0 && !filter.Statuses.Contains(view.Status))
            {
                continue;
            }

            views.Add(view);
        }

        // Apply booking visibility filter based on org type and user roles
        var orgType = resources.Select(r => r.Location?.Organization?.Type).FirstOrDefault(item => item is not null) ??
                      OrganizationTypeConstants.Private;
        var visibleViews = visibilityFilter.Apply(views, orgType, requestingUserRoles);

        var subscriptionKey = subscriptionKeyService.Compute(filter);

        sw.Stop();

        // LOG-001: query lifecycle completion
        logger.LogInformation(
            "ResourceAvailabilityDayView query completed. Date={Date} OrganizationCustomDomain={OrganizationCustomDomain} ResultCount={ResultCount} ElapsedMs={ElapsedMs}",
            filter.Date,
            filter.OrganizationCustomDomain,
            visibleViews.Count,
            sw.ElapsedMilliseconds);

        // LOG-005: slow-query warning
        if (sw.ElapsedMilliseconds > options.Value.SlowQueryThresholdMs)
        {
            logger.LogWarning(
                "ResourceAvailabilityDayView slow query detected. Date={Date} OrganizationCustomDomain={OrganizationCustomDomain} ElapsedMs={ElapsedMs} ThresholdMs={ThresholdMs}",
                filter.Date,
                filter.OrganizationCustomDomain,
                sw.ElapsedMilliseconds,
                options.Value.SlowQueryThresholdMs);
        }

        return new ResourceDayViewResult(visibleViews, subscriptionKey);
    }

    // ------------------------------------------------------------------
    // Private helpers
    // ------------------------------------------------------------------

    private static ResourceDayView BuildDayView(
        DbResource resource,
        DateOnly date,
        DateTimeOffset dayStart,
        Dictionary<string, List<DbResourceBookingSlot>> slotsByResource,
        IResourceAvailabilityClassifier classifier)
    {
        var location = resource.Location;
        var locationId = location?.Id ?? string.Empty;
        var locationName = location?.Name ?? string.Empty;

        // Zone tag (first ZONE tag found)
        var zoneTag = resource.OrganizationTags.FirstOrDefault(t => t.Type == OrganizationTagTypeConstants.Zone);
        var zoneId = zoneTag?.Id;
        var zoneName = zoneTag?.Name;

        // Resource type tag
        var resourceTypeTag = resource.OrganizationTags.FirstOrDefault(t =>
            t.Type is not null && OrganizationTagTypeConstants.ResourceTagTypes.Contains(t.Type));
        var resourceType = resourceTypeTag?.Type ?? string.Empty;

        // Opening hours resolution
        var openingHours = resource.Location?.OpeningHours;
        TimeOnly? openingFrom = null;
        TimeOnly? openingUntil = null;
        var totalOpeningMinutes = 0;

        if (openingHours is not null)
        {
            var isClosedDate = openingHours.ClosedDates.Any(cd => DateOnly.FromDateTime(cd.UtcDateTime) == date);

            if (!isClosedDate)
            {
                var details = GetDayOpeningHoursDetails(openingHours, dayStart);
                if (!details.Closed)
                {
                    if (details.OpenAllDay)
                    {
                        openingFrom = TimeOnly.MinValue;
                        openingUntil = TimeOnly.MaxValue;
                        totalOpeningMinutes = 24 * 60;
                    }
                    else if (details.From.HasValue && details.Until.HasValue)
                    {
                        openingFrom = details.From;
                        openingUntil = details.Until;
                        var fromDto = dayStart.Add(details.From.Value.ToTimeSpan());
                        var untilDto = dayStart.Add(details.Until.Value.ToTimeSpan());
                        totalOpeningMinutes = (int)(untilDto - fromDto).TotalMinutes;
                    }
                }
            }
        }

        // Collect active bookings from slots
        var activeSlots = slotsByResource.TryGetValue(resource.Id, out var resourceSlots)
            ? resourceSlots
            : [];

        var bookingWindows = new List<BookingWindow>();
        var seenBookingIds = new HashSet<string>();

        foreach (var slot in activeSlots)
        {
            foreach (var booking in slot.Bookings)
            {
                if (!seenBookingIds.Add(booking.Id))
                {
                    continue;
                }

                var createdByCustomer = booking.CreatedByCustomer;
                var bookedByName = createdByCustomer?.Name
                                   ?? (createdByCustomer is not null
                                       ? $"{createdByCustomer.GivenName} {createdByCustomer.FamilyName}".Trim()
                                       : null);

                bookingWindows.Add(new BookingWindow
                {
                    BookingId = booking.Id,
                    From = booking.From,
                    Until = booking.Until,
                    IsRecurring = booking.RecurringBooking is not null,
                    IsCheckedIn = false, // stub for v1
                    BookedByName = string.IsNullOrWhiteSpace(bookedByName) ? null : bookedByName,
                    BookedByUserId = createdByCustomer?.Id,
                    Notes = booking.Notes
                });
            }
        }

        var bookedMinutes = CalculateBookedMinutes(bookingWindows, dayStart, openingFrom, openingUntil);

        var isLocationClosed = false;
        var isDayClosed = false;
        if (openingHours is not null)
        {
            isLocationClosed = openingHours.ClosedDates
                .Any(cd => DateOnly.FromDateTime(cd.UtcDateTime) == date);
            if (!isLocationClosed)
            {
                isDayClosed = GetDayOpeningHoursDetails(openingHours, dayStart).Closed;
            }
        }

        var status = classifier.Classify(
            resource.Inactive,
            isLocationClosed,
            isDayClosed,
            totalOpeningMinutes,
            bookedMinutes);

        return new ResourceDayView
        {
            ResourceId = resource.Id,
            ResourceName = resource.Name ?? string.Empty,
            ResourceType = resourceType,
            LocationId = locationId,
            LocationName = locationName,
            FloorId = null, // no FLOOR tag type in v1
            FloorName = null, // no FLOOR tag type in v1
            ZoneId = zoneId,
            ZoneName = zoneName,
            Date = date,
            Status = status,
            OpeningFrom = openingFrom,
            OpeningUntil = openingUntil,
            TotalOpeningMinutes = totalOpeningMinutes,
            BookedMinutes = bookedMinutes,
            BookingWindows = bookingWindows.AsReadOnly()
        };
    }

    private static int CalculateBookedMinutes(
        List<BookingWindow> windows,
        DateTimeOffset dayStart,
        TimeOnly? openingFrom = null,
        TimeOnly? openingUntil = null)
    {
        if (windows.Count == 0)
        {
            return 0;
        }

        // Clip boundaries: only count time that falls within the opening window.
        var clipFrom = openingFrom.HasValue ? dayStart.Add(openingFrom.Value.ToTimeSpan()) : (DateTimeOffset?)null;
        var clipUntil = openingUntil.HasValue ? dayStart.Add(openingUntil.Value.ToTimeSpan()) : (DateTimeOffset?)null;

        // Merge overlapping windows to avoid double-counting
        var sorted = windows.OrderBy(item => item.From).Select(item => (item.From, item.Until)).ToList();
        var merged = new List<(DateTimeOffset From, DateTimeOffset Until)>();
        var (currentFrom, currentUntil) = sorted[0];
        for (var i = 1; i < sorted.Count; i++)
        {
            var (from, until) = sorted[i];
            if (from <= currentUntil)
            {
                currentUntil = until > currentUntil ? until : currentUntil;
            }
            else
            {
                merged.Add((currentFrom, currentUntil));
                (currentFrom, currentUntil) = (from, until);
            }
        }

        merged.Add((currentFrom, currentUntil));

        return (int)merged.Sum(m =>
        {
            var effectiveFrom = clipFrom.HasValue && m.From < clipFrom.Value ? clipFrom.Value : m.From;
            var effectiveUntil = clipUntil.HasValue && m.Until > clipUntil.Value ? clipUntil.Value : m.Until;
            var minutes = (effectiveUntil - effectiveFrom).TotalMinutes;
            return minutes > 0 ? minutes : 0;
        });
    }

    private static OpeningHoursDetails GetDayOpeningHoursDetails(OpeningHours openingHours, DateTimeOffset dayStart) =>
        openingHours.DatesWithVariedOpeningHours.TryGetValue(dayStart, out var varied)
            ? varied
            : dayStart.DayOfWeek switch
            {
                DayOfWeek.Monday => openingHours.WeekOpeningHours.Monday,
                DayOfWeek.Tuesday => openingHours.WeekOpeningHours.Tuesday,
                DayOfWeek.Wednesday => openingHours.WeekOpeningHours.Wednesday,
                DayOfWeek.Thursday => openingHours.WeekOpeningHours.Thursday,
                DayOfWeek.Friday => openingHours.WeekOpeningHours.Friday,
                DayOfWeek.Saturday => openingHours.WeekOpeningHours.Saturday,
                DayOfWeek.Sunday => openingHours.WeekOpeningHours.Sunday,
                _ => throw new ArgumentOutOfRangeException(nameof(dayStart), dayStart, null)
            };
}
