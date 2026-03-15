using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MarketplaceBookingSubscription = Booking.Shared.Database.Entities.MarketplaceBookingSubscription;

namespace Booking.Shared.Repositories;

public interface IMarketplaceBookingSubscriptionRepository : IRepository<MarketplaceBookingSubscription>
{
    Task<MarketplaceBookingSubscription?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<MarketplaceBookingSubscription?> GetByIdUntrackedAsync(string id, CancellationToken cancellationToken);
    MarketplaceBookingSubscription Add(MarketplaceBookingSubscription recurringBooking);
    MarketplaceBookingSubscription Update(MarketplaceBookingSubscription recurringBooking);
    MarketplaceBookingSubscription Remove(MarketplaceBookingSubscription recurringBooking);
}

internal static class MarketplaceBookingSubscriptionExtensions
{
    extension(IQueryable<MarketplaceBookingSubscription> originalQuery)
    {
        internal IIncludableQueryable<MarketplaceBookingSubscription, Customer?> AddDependentObjects(bool isTracked) =>
            (isTracked ? originalQuery.AsTracking() : originalQuery.AsNoTrackingWithIdentityResolution())
            .Include(query => query.RecurringBookings)
            .Include(query => query.ProductVersion)
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

public class MarketplaceBookingSubscriptionRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, MarketplaceBookingSubscription>(dbContext, timeProvider), IMarketplaceBookingSubscriptionRepository
{
    public async Task<MarketplaceBookingSubscription?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.MarketplaceBookingSubscription
            .AddDependentObjects(true)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<MarketplaceBookingSubscription?> GetByIdUntrackedAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.MarketplaceBookingSubscription
            .AddDependentObjects(false)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public MarketplaceBookingSubscription Add(MarketplaceBookingSubscription recurringBooking)
    {
        var now = TimeProvider.GetUtcNow();
        recurringBooking.CreatedAt = now;
        return DbContext.MarketplaceBookingSubscription.Add(recurringBooking).Entity;
    }

    public MarketplaceBookingSubscription Update(MarketplaceBookingSubscription recurringBooking)
    {
        var now = TimeProvider.GetUtcNow();
        recurringBooking.ModifiedAt = now;
        return DbContext.MarketplaceBookingSubscription.Update(recurringBooking).Entity;
    }

    public MarketplaceBookingSubscription Remove(MarketplaceBookingSubscription recurringBooking)
    {
        var now = TimeProvider.GetUtcNow();
        recurringBooking.DeletedAt = now;
        return DbContext.MarketplaceBookingSubscription.Update(recurringBooking).Entity;
    }
}
