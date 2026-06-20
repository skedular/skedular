using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;

namespace Booking.Shared.Repositories;

public interface IMarketplaceBookingFailureEventRepository : IRepository<MarketplaceBookingFailureEvent>
{
    MarketplaceBookingFailureEvent Add(MarketplaceBookingFailureEvent failureEvent);
}

public class MarketplaceBookingFailureEventRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, MarketplaceBookingFailureEvent>(dbContext, timeProvider), IMarketplaceBookingFailureEventRepository
{
    public MarketplaceBookingFailureEvent Add(MarketplaceBookingFailureEvent failureEvent)
    {
        failureEvent.CreatedAt = TimeProvider.GetUtcNow();
        return DbContext.MarketplaceBookingFailureEvent.Add(failureEvent).Entity;
    }
}
