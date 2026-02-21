using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared.Database;

namespace Booking.Shared.Repositories;

public interface IMarketplaceBookingRepository : IRepository<MarketplaceBooking>
{
    MarketplaceBooking Add(MarketplaceBooking marketplaceBooking);
    MarketplaceBooking Update(MarketplaceBooking marketplaceBooking);
}

public class MarketplaceBookingRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, MarketplaceBooking>(dbContext, timeProvider), IMarketplaceBookingRepository
{
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
