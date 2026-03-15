using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Enterprise.Shared.Random;
using Enterprise.Shared.Time;

namespace Booking.Shared.Services;

public interface ILocationResourceBookingSlotsHelperService
{
    DateTimeOffset GetStartPeriod();
    ICollection<ResourceBookingSlot> CreateAllAvailableSlots(Resource resource);
}

public class LocationResourceBookingSlotsHelperService(IRandomHelper randomHelper, TimeProvider timeProvider)
    : ILocationResourceBookingSlotsHelperService
{
    public DateTimeOffset GetStartPeriod() => timeProvider.GetUtcNow().StartOfDay().AddDays(-14);

    public ICollection<ResourceBookingSlot> CreateAllAvailableSlots(Resource resource)
    {
        var startPeriod = GetStartPeriod();
        var endPeriod = startPeriod.AddDays(14).AddYears(1).AddMonths(2);
        var count = (endPeriod - startPeriod).TotalMinutes / OpeningHoursDetails.BookingSlotSizeInMinutes;

        return Enumerable
            .Range(0, (int)count)
            .Select(idx => startPeriod.AddMinutes(idx * OpeningHoursDetails.BookingSlotSizeInMinutes))
            .Select(start => new ResourceBookingSlot { Id = randomHelper.Generate(), Start = start, Available = true, Resource = resource })
            .ToList();
    }
}
