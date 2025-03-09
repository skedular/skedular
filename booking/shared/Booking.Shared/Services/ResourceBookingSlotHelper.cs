using Booking.Shared.Database.Entities;
using Enterprise.Shared.Random;
using Enterprise.Shared.Time;

namespace Booking.Shared.Services;

public interface IResourceBookingSlotHelper
{
    ICollection<ResourceBookingSlot> CreateAllAvailableSlots(Resource resource);
}

public class ResourceBookingSlotHelper(IRandomHelper randomHelper, TimeProvider timeProvider) : IResourceBookingSlotHelper
{
    public ICollection<ResourceBookingSlot> CreateAllAvailableSlots(Resource resource)
    {
        var periodStart = timeProvider.GetUtcNow().StartOfDay();
        var periodEnd = periodStart.AddMonths(3);
        var count = (periodEnd - periodStart).TotalMinutes / 15;

        return Enumerable
            .Range(0, (int)count)
            .Select(idx => periodStart.AddMinutes(idx * 15))
            .Select(start => new ResourceBookingSlot { Id = randomHelper.Generate(), Start = start, Available = true, Resource = resource })
            .ToList();
    }
}
