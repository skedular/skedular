using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.Postgres;
using Microsoft.EntityFrameworkCore;

namespace Booking.Shared.Repositories;

public interface IMarketplaceRefundEventRepository : IRepository<MarketplaceRefundEvent>
{
    MarketplaceRefundEvent Add(MarketplaceRefundEvent marketplaceRefundEvent);
    Task<ICollection<MarketplaceRefundEvent>> GetByMarketplaceRefundIdAsync(string marketplaceRefundId, CancellationToken cancellationToken);
}

public class MarketplaceRefundEventRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, MarketplaceRefundEvent>(dbContext, timeProvider), IMarketplaceRefundEventRepository
{
    public MarketplaceRefundEvent Add(MarketplaceRefundEvent marketplaceRefundEvent)
    {
        marketplaceRefundEvent.CreatedAt = TimeProvider.GetUtcNow();
        return DbContext.MarketplaceRefundEvent.Add(marketplaceRefundEvent).Entity;
    }

    public async Task<ICollection<MarketplaceRefundEvent>> GetByMarketplaceRefundIdAsync(
        string marketplaceRefundId,
        CancellationToken cancellationToken) =>
        await DbContext.MarketplaceRefundEvent
            .Where(item => item.MarketplaceRefundId == marketplaceRefundId)
            .OrderBy(item => item.OccurredAt)
            .ThenBy(item => item.CreatedAt)
            .ToListAsync(cancellationToken);
}
