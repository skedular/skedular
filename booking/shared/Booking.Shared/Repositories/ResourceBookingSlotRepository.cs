using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Microsoft.EntityFrameworkCore;

namespace Booking.Shared.Repositories;

public interface IResourceBookingSlotRepository : IRepository<ResourceBookingSlot>
{
    void AddRange(IEnumerable<ResourceBookingSlot> resourceBookingSlots);
    void Update(ResourceBookingSlot resourceBookingSlot);
    void UpdateRange(IEnumerable<ResourceBookingSlot> resourceBookingSlots);
    Task<IReadOnlyList<ResourceBookingSlot>> GetByResourceIdAsync(string resourceId, DateTimeOffset from, CancellationToken cancellationToken);
}

public class ResourceBookingSlotRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, ResourceBookingSlot>(dbContext, timeProvider), IResourceBookingSlotRepository
{
    public void AddRange(IEnumerable<ResourceBookingSlot> resourceBookingSlots)
    {
        var now = TimeProvider.GetUtcNow();
        DbContext.ResourceBookingSlot.AddRange(resourceBookingSlots.Select(item =>
        {
            item.CreatedAt = now;
            return item;
        }));
    }

    public void Update(ResourceBookingSlot resourceBookingSlot)
    {
        var now = TimeProvider.GetUtcNow();
        resourceBookingSlot.ModifiedAt = now;
        DbContext.ResourceBookingSlot.Update(resourceBookingSlot);
    }

    public void UpdateRange(IEnumerable<ResourceBookingSlot> resourceBookingSlots)
    {
        var now = TimeProvider.GetUtcNow();
        DbContext.ResourceBookingSlot.UpdateRange(resourceBookingSlots.Select(item =>
        {
            item.ModifiedAt = now;
            return item;
        }));
    }

    public async Task<IReadOnlyList<ResourceBookingSlot>> GetByResourceIdAsync(
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
