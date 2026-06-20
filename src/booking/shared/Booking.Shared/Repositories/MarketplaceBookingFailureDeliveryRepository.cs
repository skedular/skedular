using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Microsoft.EntityFrameworkCore;

namespace Booking.Shared.Repositories;

public interface IMarketplaceBookingFailureDeliveryRepository : IRepository<MarketplaceBookingFailureDelivery>
{
    MarketplaceBookingFailureDelivery Add(MarketplaceBookingFailureDelivery delivery);
    MarketplaceBookingFailureDelivery Update(MarketplaceBookingFailureDelivery delivery);

    Task<MarketplaceBookingFailureDelivery?> GetByFailureRecipientAndChannelAsync(
        string failureId,
        string recipientKey,
        string channel,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<MarketplaceBookingFailureDelivery>> GetPendingAsync(int take, CancellationToken cancellationToken);
}

public class MarketplaceBookingFailureDeliveryRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, MarketplaceBookingFailureDelivery>(dbContext, timeProvider), IMarketplaceBookingFailureDeliveryRepository
{
    public MarketplaceBookingFailureDelivery Add(MarketplaceBookingFailureDelivery delivery)
    {
        delivery.CreatedAt = TimeProvider.GetUtcNow();
        return DbContext.MarketplaceBookingFailureDelivery.Add(delivery).Entity;
    }

    public MarketplaceBookingFailureDelivery Update(MarketplaceBookingFailureDelivery delivery)
    {
        delivery.ModifiedAt = TimeProvider.GetUtcNow();
        return DbContext.MarketplaceBookingFailureDelivery.Update(delivery).Entity;
    }

    public async Task<MarketplaceBookingFailureDelivery?> GetByFailureRecipientAndChannelAsync(
        string failureId,
        string recipientKey,
        string channel,
        CancellationToken cancellationToken) =>
        await DbContext.MarketplaceBookingFailureDelivery.FirstOrDefaultAsync(
            item => item.MarketplaceBookingFailureId == failureId && item.RecipientKey == recipientKey && item.Channel == channel,
            cancellationToken);

    public async Task<IReadOnlyList<MarketplaceBookingFailureDelivery>> GetPendingAsync(int take, CancellationToken cancellationToken) =>
        await DbContext.MarketplaceBookingFailureDelivery
            .Where(item => item.Status == MarketplaceBookingFailureDeliveryStatusConstants.Pending ||
                           item.Status == MarketplaceBookingFailureDeliveryStatusConstants.Failed)
            .OrderBy(item => item.CreatedAt)
            .Take(take)
            .ToListAsync(cancellationToken);
}
