using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Enterprise.Shared.Random;
using Enterprise.Shared.Time;

namespace Booking.Shared.Services;

/// <summary>
///     Service for managing location resource booking slots.
///     Provides functionality to get the start period and create available slots for resources.
/// </summary>
public interface ILocationResourceBookingSlotsHelperService
{
    /// <summary>
    ///     Gets the start period for booking slots (14 days ago from the current day).
    /// </summary>
    /// <returns>The start period as a DateTimeOffset.</returns>
    DateTimeOffset GetStartPeriod();

    /// <summary>
    ///     Creates all available booking slots for the specified resource.
    ///     Generates slots from 14 days ago to 14 months and 2 days in the future,
    ///     with each slot being the size defined in OpeningHoursDetails.BookingSlotSizeInMinutes.
    /// </summary>
    /// <param name="resource">The resource for which to create booking slots.</param>
    /// <returns>A collection of available resource booking slots.</returns>
    IReadOnlyList<ResourceBookingSlot> CreateAllAvailableSlots(Resource resource);
}

/// <summary>
///     Implementation of the location resource booking slots helper service.
/// </summary>
public class LocationResourceBookingSlotsHelperService(IRandomHelper randomHelper, TimeProvider timeProvider)
    : ILocationResourceBookingSlotsHelperService
{
    /// <summary>
    ///     Gets the start period for booking slots (14 days ago from the current day).
    /// </summary>
    /// <returns>The start period as a DateTimeOffset.</returns>
    public DateTimeOffset GetStartPeriod() => timeProvider.GetUtcNow().StartOfDay().AddDays(-14);

    /// <summary>
    ///     Creates all available booking slots for the specified resource.
    ///     Generates slots from 14 days ago to 14 months and 2 days in the future,
    ///     with each slot being the size defined in OpeningHoursDetails.BookingSlotSizeInMinutes.
    /// </summary>
    /// <param name="resource">The resource for which to create booking slots.</param>
    /// <returns>A collection of available resource booking slots.</returns>
    public IReadOnlyList<ResourceBookingSlot> CreateAllAvailableSlots(Resource resource)
    {
        var startPeriod = GetStartPeriod();
        var endPeriod = startPeriod.AddDays(14).AddYears(1).AddMonths(2);
        var count = (endPeriod - startPeriod).TotalMinutes / OpeningHoursDetails.BookingSlotSizeInMinutes;

        return Enumerable
            .Range(0, (int)count)
            .Select(idx => startPeriod.AddMinutes(idx * OpeningHoursDetails.BookingSlotSizeInMinutes))
            .Select(start => new ResourceBookingSlot
            {
                Id = randomHelper.Generate(),
                Start = start,
                Available = true,
                Resource = resource,
            })
            .ToList();
    }
}
