using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Microsoft.EntityFrameworkCore;

namespace Booking.Shared.Repositories;

public interface IResourceBookingSlotRepository : IRepository<ResourceBookingSlot>
{
    void AddRange(ICollection<ResourceBookingSlot> resourceBookingSlots);
    void Update(ResourceBookingSlot resourceBookingSlot);
    void UpdateRange(ICollection<ResourceBookingSlot> resourceBookingSlots);
    Task<ICollection<ResourceBookingSlot>> GetByResourceIdAsync(string resourceId, DateTimeOffset from, CancellationToken cancellationToken);
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

    public void Update(ResourceBookingSlot resourceBookingSlot)
    {
        var now = TimeProvider.GetUtcNow();
        resourceBookingSlot.ModifiedAt = now;
        DbContext.ResourceBookingSlot.Update(resourceBookingSlot);
    }

    public void UpdateRange(ICollection<ResourceBookingSlot> resourceBookingSlots)
    {
        var now = TimeProvider.GetUtcNow();
        resourceBookingSlots.ForEach(item => item.ModifiedAt = now);
        DbContext.ResourceBookingSlot.UpdateRange(resourceBookingSlots);
    }

    public async Task<ICollection<ResourceBookingSlot>> GetByResourceIdAsync(
        string resourceId,
        DateTimeOffset from,
        CancellationToken cancellationToken) =>
        await DbContext.ResourceBookingSlot
            .Where(query => query.Resource.Id == resourceId && query.Start >= from)
            .AsSingleQuery()
            .Include(query => query.Resource)
            .ThenInclude(query => query.Location)
            .ToListAsync(cancellationToken);
}
