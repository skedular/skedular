using Booking.Shared.Database.Entities;
using Enterprise.Shared.Random;

namespace Booking.Shared.Services;

public interface IResourceBookingSlotHelper
{
    ICollection<ResourceBookingSlot> CreateAllAvailableSlots(Resource resource);
}

public class ResourceBookingSlotHelper(IRandomHelper randomHelper) : IResourceBookingSlotHelper
{
    public ICollection<ResourceBookingSlot> CreateAllAvailableSlots(Resource resource)
    {
        var periodStart = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var periodEnd = new DateTimeOffset(2027, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var count = (periodEnd - periodStart).TotalMinutes / 15;

        return Enumerable
            .Range(0, (int)count)
            .Select(idx => periodStart.AddMinutes(idx * 15))
            .Select(start => new ResourceBookingSlot { Id = randomHelper.Generate(), Start = start, Available = true, Resource = resource })
            .ToList();
    }
}
