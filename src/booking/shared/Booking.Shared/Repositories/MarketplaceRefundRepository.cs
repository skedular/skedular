using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Microsoft.EntityFrameworkCore;

namespace Booking.Shared.Repositories;

public interface IMarketplaceRefundRepository : IRepository<MarketplaceRefund>
{
    MarketplaceRefund Add(MarketplaceRefund marketplaceRefund);
    MarketplaceRefund Update(MarketplaceRefund marketplaceRefund);
    Task<MarketplaceRefund?> GetByIdAsync(string id, CancellationToken cancellationToken);

    Task<MarketplaceRefund?> GetByLocalEntityAsync(
        string organizationId,
        string localEntityType,
        string localEntityId,
        CancellationToken cancellationToken);

    Task<MarketplaceRefund?> GetByLocalEntityAsync(
        string localEntityType,
        string localEntityId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<MarketplaceRefund>> GetByOrganizationIdAsync(
        string organizationId,
        IReadOnlyList<string>? statuses,
        CancellationToken cancellationToken);
}

public class MarketplaceRefundRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, MarketplaceRefund>(dbContext, timeProvider), IMarketplaceRefundRepository
{
    public MarketplaceRefund Add(MarketplaceRefund marketplaceRefund)
    {
        marketplaceRefund.CreatedAt = TimeProvider.GetUtcNow();
        return DbContext.MarketplaceRefund.Add(marketplaceRefund).Entity;
    }

    public MarketplaceRefund Update(MarketplaceRefund marketplaceRefund)
    {
        marketplaceRefund.ModifiedAt = TimeProvider.GetUtcNow();
        return DbContext.MarketplaceRefund.Update(marketplaceRefund).Entity;
    }

    public async Task<MarketplaceRefund?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.MarketplaceRefund.FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<MarketplaceRefund?> GetByLocalEntityAsync(
        string organizationId,
        string localEntityType,
        string localEntityId,
        CancellationToken cancellationToken) =>
        await DbContext.MarketplaceRefund.FirstOrDefaultAsync(
            query =>
                query.OrganizationId == organizationId &&
                query.LocalEntityType == localEntityType &&
                query.LocalEntityId == localEntityId,
            cancellationToken);

    public async Task<MarketplaceRefund?> GetByLocalEntityAsync(
        string localEntityType,
        string localEntityId,
        CancellationToken cancellationToken) =>
        await DbContext.MarketplaceRefund.FirstOrDefaultAsync(
            query =>
                query.LocalEntityType == localEntityType &&
                query.LocalEntityId == localEntityId,
            cancellationToken);

    public async Task<IReadOnlyList<MarketplaceRefund>> GetByOrganizationIdAsync(
        string organizationId,
        IReadOnlyList<string>? statuses,
        CancellationToken cancellationToken)
    {
        var query = DbContext.MarketplaceRefund.Where(item => item.OrganizationId == organizationId);
        if (statuses is { Count: > 0 })
        {
            query = query.Where(item => statuses.Contains(item.Status));
        }

        return await query
            .OrderByDescending(item => item.RequestedAt)
            .ToListAsync(cancellationToken);
    }
}
