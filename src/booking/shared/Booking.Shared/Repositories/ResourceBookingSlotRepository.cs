using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Microsoft.EntityFrameworkCore;

namespace Booking.Shared.Repositories;

public interface IResourceBookingSlotRepository : IRepository<ResourceBookingSlot>
{
    const int UnbookedSlotCleanupBatchSize = 500;

    void AddRange(IEnumerable<ResourceBookingSlot> resourceBookingSlots);
    void Update(ResourceBookingSlot resourceBookingSlot);
    void UpdateRange(IEnumerable<ResourceBookingSlot> resourceBookingSlots);
    Task<IReadOnlyList<ResourceBookingSlot>> GetByResourceIdAsync(string resourceId, DateTimeOffset from, CancellationToken cancellationToken);

    Task<IReadOnlyList<ResourceBookingSlot>> GetByResourceIdsAndDayAsync(
        IReadOnlyList<string> resourceIds,
        DateTimeOffset dayStart,
        DateTimeOffset dayEnd, CancellationToken cancellationToken);

    Task<int> DeleteUnbookedBatchBeforeAsync(DateTimeOffset cutoff, CancellationToken cancellationToken);
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

    public async Task<IReadOnlyList<ResourceBookingSlot>> GetByResourceIdsAndDayAsync(
        IReadOnlyList<string> resourceIds,
        DateTimeOffset dayStart,
        DateTimeOffset dayEnd,
        CancellationToken cancellationToken)
    {
        if (resourceIds.Count == 0)
        {
            return [];
        }

        return await DbContext.ResourceBookingSlot
            .AsNoTrackingWithIdentityResolution()
            .Where(item => resourceIds.Contains(item.ResourceId) && item.Start >= dayStart && item.Start < dayEnd)
            .Include(item => item.Bookings.Where(booking => booking.DeletedByCustomer == null))
            .ThenInclude(item => item.CreatedByCustomer)
            .Include(item => item.Bookings.Where(booking => booking.DeletedByCustomer == null))
            .ThenInclude(item => item.RecurringBooking)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> DeleteUnbookedBatchBeforeAsync(DateTimeOffset cutoff, CancellationToken cancellationToken)
    {
        var slotIds = await DbContext.ResourceBookingSlot
            .Where(slot => slot.Start < cutoff && !slot.Bookings.Any())
            .OrderBy(slot => slot.Start)
            .ThenBy(slot => slot.Id)
            .Select(slot => slot.Id)
            .Take(IResourceBookingSlotRepository.UnbookedSlotCleanupBatchSize + 1)
            .ToListAsync(cancellationToken);

        var hasMore = slotIds.Count > IResourceBookingSlotRepository.UnbookedSlotCleanupBatchSize;
        if (hasMore)
        {
            slotIds.RemoveAt(IResourceBookingSlotRepository.UnbookedSlotCleanupBatchSize);
        }

        var deletedCount = 0;
        if (slotIds.Count > 0)
        {
            deletedCount = await DbContext.ResourceBookingSlot
                .Where(slot => slotIds.Contains(slot.Id) && !slot.Bookings.Any())
                .ExecuteDeleteAsync(cancellationToken);
        }

        return hasMore ? IResourceBookingSlotRepository.UnbookedSlotCleanupBatchSize + 1 : deletedCount;
    }
}
