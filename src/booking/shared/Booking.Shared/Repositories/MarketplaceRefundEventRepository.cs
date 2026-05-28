using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Microsoft.EntityFrameworkCore;

namespace Booking.Shared.Repositories;

public interface IMarketplaceRefundEventRepository : IRepository<MarketplaceRefundEvent>
{
    MarketplaceRefundEvent Add(MarketplaceRefundEvent marketplaceRefundEvent);
    Task<IReadOnlyList<MarketplaceRefundEvent>> GetByMarketplaceRefundIdAsync(string marketplaceRefundId, CancellationToken cancellationToken);

    Task<IReadOnlyList<MarketplaceRefundEvent>> GetByMarketplaceRefundIdsAsync(IReadOnlyList<string> marketplaceRefundIds,
        CancellationToken cancellationToken);
}

public class MarketplaceRefundEventRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, MarketplaceRefundEvent>(dbContext, timeProvider), IMarketplaceRefundEventRepository
{
    public MarketplaceRefundEvent Add(MarketplaceRefundEvent marketplaceRefundEvent)
    {
        marketplaceRefundEvent.CreatedAt = TimeProvider.GetUtcNow();
        return DbContext.MarketplaceRefundEvent.Add(marketplaceRefundEvent).Entity;
    }

    public async Task<IReadOnlyList<MarketplaceRefundEvent>> GetByMarketplaceRefundIdAsync(
        string marketplaceRefundId,
        CancellationToken cancellationToken) =>
        await DbContext.MarketplaceRefundEvent
            .AsNoTrackingWithIdentityResolution()
            .Where(item => item.MarketplaceRefundId == marketplaceRefundId)
            .OrderBy(item => item.OccurredAt)
            .ThenBy(item => item.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<MarketplaceRefundEvent>> GetByMarketplaceRefundIdsAsync(
        IReadOnlyList<string> marketplaceRefundIds,
        CancellationToken cancellationToken)
    {
        if (marketplaceRefundIds.Count == 0)
        {
            return [];
        }

        return await DbContext.MarketplaceRefundEvent
            .AsNoTrackingWithIdentityResolution()
            .Where(item => marketplaceRefundIds.Contains(item.MarketplaceRefundId))
            .OrderBy(item => item.OccurredAt)
            .ThenBy(item => item.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
