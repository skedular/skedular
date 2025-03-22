using Booking.Shared.Database.Entities;
using Enterprise.Shared.Random;
using Enterprise.Shared.Time;

namespace Booking.Shared.Services;

public interface IResourceBookingSlotHelperService
{
    DateTimeOffset GetStartPeriod();
    ICollection<ResourceBookingSlot> CreateAllAvailableSlots(Resource resource);
}

public class ResourceBookingSlotHelperService(IRandomHelper randomHelper, TimeProvider timeProvider) : IResourceBookingSlotHelperService
{
    public DateTimeOffset GetStartPeriod() => timeProvider.GetUtcNow().StartOfDay().AddYears(-1);

    public ICollection<ResourceBookingSlot> CreateAllAvailableSlots(Resource resource)
    {
        var startPeriod = GetStartPeriod();
        var endPeriod = startPeriod.AddYears(1).AddYears(1);
        var count = (endPeriod - startPeriod).TotalMinutes / 15;

        return Enumerable
            .Range(0, (int)count)
            .Select(idx => startPeriod.AddMinutes(idx * 15))
            .Select(start => new ResourceBookingSlot { Id = randomHelper.Generate(), Start = start, Available = true, Resource = resource })
            .ToList();
    }
}
