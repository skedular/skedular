using System.Diagnostics;
using Api.Shared.Services.Models;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Booking.Shared.Services;

/// <summary>
///     Computes resource availability day views for a given date and filter.
///     Reads live booking data directly from the Booking domain DB — no cross-domain gRPC,
///     no analytics snapshots.
/// </summary>
/// <remarks>
///     Query strategy: a single database round-trip fetches every matching resource together
///     with its day-scoped booking windows via the <c>Booking.InvolvedResources</c> many-to-many
///     join table.  The previous two-query approach (resources first, booking slots second) was
///     replaced to eliminate the second round-trip, remove the wide <c>IN (…resourceIds…)</c>
///     clause on <c>ResourceBookingSlot</c>, and avoid loading slot rows that were only needed
///     to navigate to their parent bookings.
/// </remarks>
public interface IResourceAvailabilityDayViewService
{
    /// <summary>
    ///     Returns the full sorted list of resource day views matching the given filter,
    ///     along with an opaque subscription key the client can use to subscribe to real-time updates.
    /// </summary>
    /// <param name="filter">Date, location, floor, zone, resource type, and status constraints.</param>
    /// <param name="orderBy">
    ///     Ordered list of sort clauses. The first clause is the primary sort; further clauses
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

        // Single query: resources arrive with their day-scoped booking windows already embedded.
        // The repository projects Resource.InvolvedBookings directly — no second round-trip to
        // ResourceBookingSlot, and no wide IN-clause fanout across resource IDs.
        var resources = await repositoryFactory.ResourceRepository.GetForAvailabilityDayViewAsync(
            filter.OrganizationCustomDomain,
            filter.LocationIds,
            filter.ZoneId,
            filter.ResourceType,
            filter.Statuses,
            dayStart,
            dayEnd,
            orderBy,
            cancellationToken);

        var views = new List<ResourceDayView>(resources.Count);
        foreach (var resource in resources)
        {
            var view = BuildDayView(resource, filter.Date, dayStart, classifier);

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
        var orgType = resources.Select(item => item.OrganizationType).FirstOrDefault() ?? OrganizationTypeConstants.Private;
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
        ResourceAvailabilityResourceRow resource,
        DateOnly date,
        DateTimeOffset dayStart,
        IResourceAvailabilityClassifier classifier)
    {
        // Opening hours resolution
        var openingHours = resource.OpeningHours;
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
                    else if (details is { From: not null, Until: not null })
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

        var bookingWindows = resource.BookingWindows
            .Select(ToBookingWindow)
            .OrderBy(w => w.From)
            .ToList();

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
            ResourceName = resource.Name,
            ResourceType = resource.ResourceType,
            LocationId = resource.LocationId,
            LocationName = resource.LocationName,
            FloorId = null, // no FLOOR tag type in v1
            FloorName = null, // no FLOOR tag type in v1
            ZoneId = resource.ZoneId,
            ZoneName = resource.ZoneName,
            Date = date,
            Status = status,
            OpeningFrom = openingFrom,
            OpeningUntil = openingUntil,
            TotalOpeningMinutes = totalOpeningMinutes,
            BookedMinutes = bookedMinutes,
            BookingWindows = bookingWindows.AsReadOnly(),
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
                _ => throw new ArgumentOutOfRangeException(nameof(dayStart), dayStart,
                    $"Unexpected value for {nameof(dayStart)}: {dayStart}. Update enum mapping or caller input."),
            };

    private static BookingWindow ToBookingWindow(ResourceBookingWindowRow row)
    {
        var bookedByName = row.CustomerName ??
                           $"{row.CustomerGivenName} {row.CustomerFamilyName}".Trim();

        return new BookingWindow
        {
            BookingId = row.BookingId,
            From = row.From,
            Until = row.Until,
            IsRecurring = row.IsRecurring,
            IsCheckedIn = false,
            BookedByName = string.IsNullOrWhiteSpace(bookedByName) ? null : bookedByName,
            BookedByUserId = row.CustomerId,
            Notes = row.Notes,
        };
    }
}
