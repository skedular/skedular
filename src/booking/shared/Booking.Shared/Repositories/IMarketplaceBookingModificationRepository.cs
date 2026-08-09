using Booking.Shared.Database.Entities;
using Enterprise.Shared.Database;

namespace Booking.Shared.Repositories;

public interface IMarketplaceBookingModificationRepository : IRepository<MarketplaceBookingModification>
{
    MarketplaceBookingModification Add(MarketplaceBookingModification modification);
    MarketplaceBookingModificationNotificationDelivery AddDelivery(MarketplaceBookingModificationNotificationDelivery delivery);
    MarketplaceBookingModificationNotificationDelivery UpdateDelivery(MarketplaceBookingModificationNotificationDelivery delivery);
    Task<MarketplaceBookingModification?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<IReadOnlyList<MarketplaceBookingModification>> GetByBookingIdAsync(string bookingId, CancellationToken cancellationToken);

    Task<MarketplaceBookingModificationNotificationDelivery?> GetDeliveryByKeyAsync(
        string modificationId,
        string deliveryKey,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<MarketplaceBookingModificationNotificationDelivery>> GetPendingDeliveriesAsync(
        int take,
        CancellationToken cancellationToken);
}
