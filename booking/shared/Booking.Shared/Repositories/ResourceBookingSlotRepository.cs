using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Microsoft.EntityFrameworkCore;
using ResourceBookingWindowRow = Booking.Shared.Models.ResourceBookingWindowRow;

namespace Booking.Shared.Repositories;

public interface IResourceBookingSlotRepository : IRepository<ResourceBookingSlot>
{
    void AddRange(IEnumerable<ResourceBookingSlot> resourceBookingSlots);
    void Update(ResourceBookingSlot resourceBookingSlot);
    void UpdateRange(IEnumerable<ResourceBookingSlot> resourceBookingSlots);
    Task<IReadOnlyList<ResourceBookingSlot>> GetByResourceIdAsync(string resourceId, DateTimeOffset from, CancellationToken cancellationToken);

    Task<IReadOnlyList<ResourceBookingSlot>> GetByResourceIdsAndDayAsync(
        IReadOnlyList<string> resourceIds,
        DateTimeOffset dayStart,
        DateTimeOffset dayEnd, CancellationToken cancellationToken);

    Task<IReadOnlyList<ResourceBookingWindowRow>> GetBookingWindowsByResourceIdsAndDayAsync(
        IReadOnlyList<string> resourceIds,
        DateTimeOffset dayStart,
        DateTimeOffset dayEnd,
        CancellationToken cancellationToken);
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

    public async Task<IReadOnlyList<ResourceBookingWindowRow>> GetBookingWindowsByResourceIdsAndDayAsync(
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
            .AsNoTracking()
            .Where(slot => resourceIds.Contains(slot.ResourceId) &&
                           slot.Start >= dayStart &&
                           slot.Start < dayEnd &&
                           slot.Bookings.Any(booking => booking.DeletedByCustomer == null))
            .SelectMany(slot => slot.Bookings
                .Where(booking => booking.DeletedByCustomer == null)
                .Select(booking => new ResourceBookingWindowRow
                {
                    ResourceId = slot.ResourceId,
                    BookingId = booking.Id,
                    From = booking.From,
                    Until = booking.Until,
                    IsRecurring = booking.RecurringBooking != null,
                    CustomerId = booking.CreatedByCustomer != null ? booking.CreatedByCustomer.Id : null,
                    CustomerName = booking.CreatedByCustomer != null ? booking.CreatedByCustomer.Name : null,
                    CustomerGivenName = booking.CreatedByCustomer != null ? booking.CreatedByCustomer.GivenName : null,
                    CustomerFamilyName = booking.CreatedByCustomer != null ? booking.CreatedByCustomer.FamilyName : null,
                    Notes = booking.Notes
                }))
            .Distinct()
            .ToListAsync(cancellationToken);
    }
}
