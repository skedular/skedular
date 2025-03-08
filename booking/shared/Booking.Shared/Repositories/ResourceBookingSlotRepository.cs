using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;

namespace Booking.Shared.Repositories;

public interface IResourceBookingSlotRepository : IRepository<ResourceBookingSlot>
{
    void AddRange(ICollection<ResourceBookingSlot> resourceBookingSlots);
    void UpdateRange(ICollection<ResourceBookingSlot> resourceBookingSlots);
    void RemoveRange(ICollection<ResourceBookingSlot> resourceBookingSlots);
    Task<ICollection<ResourceBookingSlot>> GetByResourceIdAsync(string resourceId, CancellationToken cancellationToken);
}

public class ResourceBookingSlotRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, ResourceBookingSlot>(dbContext, timeProvider), IResourceBookingSlotRepository
{
    public void AddRange(ICollection<ResourceBookingSlot> resourceBookingSlots)
    {
        var now = TimeProvider.GetUtcNow();
        resourceBookingSlots.ForEach(identity => identity.CreatedAt = now);
        DbContext.ResourceBookingSlot.AddRange(resourceBookingSlots);
    }

    public void RemoveRange(ICollection<ResourceBookingSlot> resourceBookingSlots) => DbContext.ResourceBookingSlot.RemoveRange(resourceBookingSlots);

    public void UpdateRange(ICollection<ResourceBookingSlot> resourceBookingSlots)
    {
        var now = TimeProvider.GetUtcNow();
        resourceBookingSlots.ForEach(identity => identity.ModifiedAt = now);
        DbContext.ResourceBookingSlot.UpdateRange(resourceBookingSlots);
    }

    public async Task<ICollection<ResourceBookingSlot>> GetByResourceIdAsync(string resourceId, CancellationToken cancellationToken) =>
        await DbContext.ResourceBookingSlot
            .Where(query => query.Resource.Id == resourceId)
            .Include(query => query.Resource)
            .ThenInclude(query => query.Location)
            .ToListAsync(cancellationToken);
}
