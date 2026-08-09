using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Enterprise.Shared.Database.PostgreSql;
using Microsoft.EntityFrameworkCore;

namespace Booking.Shared.Repositories;

public class MarketplaceBookingModificationRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, MarketplaceBookingModification>(dbContext, timeProvider), IMarketplaceBookingModificationRepository
{
    public MarketplaceBookingModification Add(MarketplaceBookingModification modification)
    {
        modification.CreatedAt = TimeProvider.GetUtcNow();
        return DbContext.MarketplaceBookingModification.Add(modification).Entity;
    }

    public MarketplaceBookingModificationNotificationDelivery AddDelivery(MarketplaceBookingModificationNotificationDelivery delivery)
    {
        delivery.CreatedAt = TimeProvider.GetUtcNow();
        return DbContext.MarketplaceBookingModificationNotificationDelivery.Add(delivery).Entity;
    }

    public MarketplaceBookingModificationNotificationDelivery UpdateDelivery(MarketplaceBookingModificationNotificationDelivery delivery)
    {
        delivery.ModifiedAt = TimeProvider.GetUtcNow();
        return DbContext.MarketplaceBookingModificationNotificationDelivery.Update(delivery).Entity;
    }

    public async Task<MarketplaceBookingModification?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.MarketplaceBookingModification
            .Include(item => item.NotificationDeliveries)
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

    public async Task<IReadOnlyList<MarketplaceBookingModification>> GetByBookingIdAsync(string bookingId, CancellationToken cancellationToken) =>
        await DbContext.MarketplaceBookingModification
            .Include(item => item.NotificationDeliveries)
            .Where(item => item.BookingId == bookingId)
            .OrderByDescending(item => item.OccurredAt)
            .ToListAsync(cancellationToken);

    public async Task<MarketplaceBookingModificationNotificationDelivery?> GetDeliveryByKeyAsync(
        string modificationId,
        string deliveryKey,
        CancellationToken cancellationToken) =>
        await DbContext.MarketplaceBookingModificationNotificationDelivery.FirstOrDefaultAsync(
            item => item.MarketplaceBookingModificationId == modificationId && item.DeliveryKey == deliveryKey,
            cancellationToken);

    public async Task<IReadOnlyList<MarketplaceBookingModificationNotificationDelivery>> GetPendingDeliveriesAsync(
        int take,
        CancellationToken cancellationToken) =>
        await DbContext.MarketplaceBookingModificationNotificationDelivery
            .Where(item => item.Status == MarketplaceBookingModificationNotificationDeliveryStatusConstants.Pending ||
                           item.Status == MarketplaceBookingModificationNotificationDeliveryStatusConstants.RecoveryRequired)
            .OrderBy(item => item.CreatedAt)
            .Take(take)
            .ToListAsync(cancellationToken);
}
