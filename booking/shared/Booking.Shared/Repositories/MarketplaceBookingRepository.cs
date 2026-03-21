using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Booking.Shared.Repositories;

public interface IMarketplaceBookingRepository : IRepository<MarketplaceBooking>
{
    Task<MarketplaceBooking?> GetByIdAsync(string id, CancellationToken cancellationToken);
    MarketplaceBooking Add(MarketplaceBooking marketplaceBooking);
    MarketplaceBooking Update(MarketplaceBooking marketplaceBooking);
}

internal static class MarketplaceBookingExtensions
{
    extension(IQueryable<MarketplaceBooking> originalQuery)
    {
        internal IIncludableQueryable<MarketplaceBooking, StripeCheckoutSession?> AddDependentObjects(bool isTracked) =>
            (isTracked ? originalQuery.AsTracking() : originalQuery.AsNoTracking())
            .Include(query => query.RecurringBooking)
            .Include(query => query.PaidByCustomer)
            .Include(query => query.PaidByOrganization)
            .Include(query => query.ProductVersion)
            .ThenInclude(query => query.OrganizationTags.Where(tag => !tag.DeletedAt.HasValue))
            .Include(query => query.ProductVersion)
            .ThenInclude(query => query.Product)
            .ThenInclude(query => query.Organization)
            .Include(query => query.StripeCheckoutSession);
    }
}

public class MarketplaceBookingRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, MarketplaceBooking>(dbContext, timeProvider), IMarketplaceBookingRepository
{
    public async Task<MarketplaceBooking?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.MarketplaceBooking
            .AddDependentObjects(true)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public MarketplaceBooking Add(MarketplaceBooking marketplaceBooking)
    {
        var now = TimeProvider.GetUtcNow();
        marketplaceBooking.CreatedAt = now;
        return DbContext.MarketplaceBooking.Add(marketplaceBooking).Entity;
    }

    public MarketplaceBooking Update(MarketplaceBooking marketplaceBooking)
    {
        var now = TimeProvider.GetUtcNow();
        marketplaceBooking.ModifiedAt = now;
        return DbContext.MarketplaceBooking.Update(marketplaceBooking).Entity;
    }
}
