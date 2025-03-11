using Booking.Shared.Database.Entities;
using Enterprise.Shared.Random;
using Enterprise.Shared.Time;

namespace Booking.Shared.Services;

public interface IResourceBookingSlotHelper
{
    DateTimeOffset GetStartPeriod();
    ICollection<ResourceBookingSlot> CreateAllAvailableSlots(Resource resource);
}

public class ResourceBookingSlotHelper(IRandomHelper randomHelper, TimeProvider timeProvider) : IResourceBookingSlotHelper
{
    public DateTimeOffset GetStartPeriod() => timeProvider.GetUtcNow().StartOfDay();

    public ICollection<ResourceBookingSlot> CreateAllAvailableSlots(Resource resource)
    {
        var startPeriod = GetStartPeriod();
        var endPeriod = startPeriod.AddMonths(6);
        var count = (endPeriod - startPeriod).TotalMinutes / 15;

        return Enumerable
            .Range(0, (int)count)
            .Select(idx => startPeriod.AddMinutes(idx * 15))
            .Select(start => new ResourceBookingSlot { Id = randomHelper.Generate(), Start = start, Available = true, Resource = resource })
            .ToList();
    }
}
