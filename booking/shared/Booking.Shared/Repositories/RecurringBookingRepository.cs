using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Booking.Shared.Repositories;

public interface IRecurringBookingRepository : IRepository<RecurringBooking>
{
    Task<RecurringBooking?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<RecurringBooking?> GetByIdUntrackedAsync(string id, CancellationToken cancellationToken);
    RecurringBooking Add(RecurringBooking recurringBooking);
    RecurringBooking Update(RecurringBooking recurringBooking);
    RecurringBooking Remove(RecurringBooking recurringBooking);
}

internal static class RecurringBookingExtensions
{
    extension(IQueryable<RecurringBooking> originalQuery)
    {
        internal IIncludableQueryable<RecurringBooking, Customer?> AddDependentObjects(bool isTracked) =>
            (isTracked ? originalQuery.AsTracking() : originalQuery.AsNoTrackingWithIdentityResolution())
            .Include(query => query.InvolvedCustomers)
            .ThenInclude(query => query.Identities)
            .Include(query => query.InvolvedOrganizations)
            .ThenInclude(query => query.OrganizationMembers.Where(organizationMember => !organizationMember.DeletedAt.HasValue))
            .Include(query => query.InvolvedTeams)
            .Include(query => query.CreatedByCustomer)
            .Include(query => query.LastModifiedByCustomer)
            .Include(query => query.DeletedByCustomer);
    }
}

public class RecurringBookingRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, RecurringBooking>(dbContext, timeProvider), IRecurringBookingRepository
{
    public async Task<RecurringBooking?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.RecurringBooking
            .AddDependentObjects(true)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<RecurringBooking?> GetByIdUntrackedAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.RecurringBooking
            .AddDependentObjects(false)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public RecurringBooking Add(RecurringBooking recurringBooking)
    {
        var now = TimeProvider.GetUtcNow();
        recurringBooking.CreatedAt = now;
        return DbContext.RecurringBooking.Add(recurringBooking).Entity;
    }

    public RecurringBooking Update(RecurringBooking recurringBooking)
    {
        var now = TimeProvider.GetUtcNow();
        recurringBooking.ModifiedAt = now;
        return DbContext.RecurringBooking.Update(recurringBooking).Entity;
    }

    public RecurringBooking Remove(RecurringBooking recurringBooking)
    {
        var now = TimeProvider.GetUtcNow();
        recurringBooking.DeletedAt = now;
        return DbContext.RecurringBooking.Update(recurringBooking).Entity;
    }
}
