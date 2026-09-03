using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models.Entitlements;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Enterprise.Shared.Time;
using Microsoft.EntityFrameworkCore;
using BookingEntity = Booking.Shared.Database.Entities.Booking;

namespace Booking.Shared.Repositories;

public interface IEntitlementRepository : IRepository<Entitlement>
{
    Entitlement Add(Entitlement entitlement);
    Task<Entitlement?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<Entitlement?> GetByPurchaseReferenceAsync(string purchaseReference, CancellationToken cancellationToken);
    Task<IReadOnlyList<Entitlement>> GetActiveForCustomerAsync(string customerId, DateTimeOffset usageAt, CancellationToken cancellationToken);
    Task<IReadOnlyList<Entitlement>> GetForCustomerAsync(string customerId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Entitlement>> GetForOrganizationAsync(string organizationId, CancellationToken cancellationToken);
    Task<CreditLedgerEntry?> GetConsumedByBookingIdAsync(string bookingId, CancellationToken cancellationToken);
    Task<bool> HasActiveMarketplaceBookingsAsync(string entitlementId, DateTimeOffset now, CancellationToken cancellationToken);

    Task<int> CountSuccessfulRedemptionsAsync(string entitlementId, DateTimeOffset weekStart, DateTimeOffset weekEnd,
        CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<string, int>> CountSuccessfulRedemptionsAsync(IReadOnlyCollection<string> entitlementIds,
        DateTimeOffset weekStart, DateTimeOffset weekEnd, CancellationToken cancellationToken);

    Task<IReadOnlyList<Entitlement>> GetExpiredActiveAsync(DateTimeOffset now, CancellationToken cancellationToken);
    CreditLedgerEntry AddLedgerEntry(CreditLedgerEntry entry);
}

public sealed class EntitlementRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, Entitlement>(dbContext, timeProvider), IEntitlementRepository
{
    public Entitlement Add(Entitlement entitlement) => DbContext.Entitlement.Add(entitlement).Entity;

    public Task<Entitlement?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        DbContext.Entitlement
            .Include(item => item.Organization)
            .Include(item => item.LedgerEntries)
            .Include(item => item.MarketplaceBookings)
            .Include(item => item.RefundLinks)
            .ThenInclude(item => item.MarketplaceRefund)
            .Include(item => item.EntitlementPurchase)
            .ThenInclude(item => item!.ProductVersion)
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);

    public Task<Entitlement?> GetByPurchaseReferenceAsync(string purchaseReference, CancellationToken cancellationToken) =>
        DbContext.Entitlement
            .Include(item => item.Organization)
            .Include(item => item.LedgerEntries)
            .Include(item => item.MarketplaceBookings)
            .Include(item => item.RefundLinks)
            .ThenInclude(item => item.MarketplaceRefund)
            .Include(item => item.EntitlementPurchase)
            .ThenInclude(item => item!.ProductVersion)
            .SingleOrDefaultAsync(item => item.PurchaseReference == purchaseReference, cancellationToken);

    public async Task<IReadOnlyList<Entitlement>> GetActiveForCustomerAsync(
        string customerId,
        DateTimeOffset usageAt,
        CancellationToken cancellationToken) =>
        await DbContext.Entitlement
            .Include(item => item.Organization)
            .Include(item => item.LedgerEntries)
            .Include(item => item.MarketplaceBookings)
            .Include(item => item.EntitlementPurchase)
            .ThenInclude(item => item!.ProductVersion)
            .Where(item => item.CustomerId == customerId && item.Status == EntitlementStatus.Active)
            .Where(item => item.ActivatesAt <= usageAt && item.ExpiresAt > usageAt)
            .OrderBy(item => item.ExpiresAt)
            .ThenBy(item => item.Id)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Entitlement>> GetForCustomerAsync(string customerId, CancellationToken cancellationToken) =>
        await DbContext.Entitlement
            .Include(item => item.Organization)
            .Include(item => item.LedgerEntries)
            .Include(item => item.MarketplaceBookings)
            .Include(item => item.RefundLinks)
            .ThenInclude(item => item.MarketplaceRefund)
            .Include(item => item.EntitlementPurchase)
            .ThenInclude(item => item!.ProductVersion)
            .Where(item => item.CustomerId == customerId)
            .OrderByDescending(item => item.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Entitlement>> GetForOrganizationAsync(string organizationId, CancellationToken cancellationToken) =>
        await DbContext.Entitlement
            .Include(item => item.LedgerEntries)
            .Include(item => item.MarketplaceBookings)
            .Include(item => item.RefundLinks)
            .ThenInclude(item => item.MarketplaceRefund)
            .Include(item => item.EntitlementPurchase)
            .ThenInclude(item => item!.ProductVersion)
            .Where(item => item.OrganizationId == organizationId)
            .OrderByDescending(item => item.CreatedAt)
            .ToListAsync(cancellationToken);

    public CreditLedgerEntry AddLedgerEntry(CreditLedgerEntry entry) => DbContext.CreditLedgerEntry.Add(entry).Entity;

    public Task<int> CountSuccessfulRedemptionsAsync(
        string entitlementId,
        DateTimeOffset weekStart,
        DateTimeOffset weekEnd,
        CancellationToken cancellationToken) =>
        DbContext.CreditLedgerEntry
            .Where(item => item.EntitlementId == entitlementId &&
                           item.TransactionType == CreditLedgerTransactionType.Consumed.ToPersistedValue() &&
                           item.Booking != null && item.Booking.From >= weekStart && item.Booking.From < weekEnd &&
                           !DbContext.CreditLedgerEntry.Any(release =>
                               release.BookingId == item.BookingId &&
                               (release.TransactionType == CreditLedgerTransactionType.Released.ToPersistedValue() ||
                                release.TransactionType == CreditLedgerTransactionType.Forfeited.ToPersistedValue())))
            .Select(item => item.Quantity)
            .SumAsync(cancellationToken);

    public async Task<IReadOnlyDictionary<string, int>> CountSuccessfulRedemptionsAsync(
        IReadOnlyCollection<string> entitlementIds,
        DateTimeOffset weekStart,
        DateTimeOffset weekEnd,
        CancellationToken cancellationToken) =>
        await DbContext.CreditLedgerEntry
            .Where(item => entitlementIds.Contains(item.EntitlementId) &&
                           item.TransactionType == CreditLedgerTransactionType.Consumed.ToPersistedValue() &&
                           item.Booking != null && item.Booking.From >= weekStart && item.Booking.From < weekEnd &&
                           !DbContext.CreditLedgerEntry.Any(release =>
                               release.BookingId == item.BookingId &&
                               (release.TransactionType == CreditLedgerTransactionType.Released.ToPersistedValue() ||
                                release.TransactionType == CreditLedgerTransactionType.Forfeited.ToPersistedValue())))
            .GroupBy(item => item.EntitlementId)
            .Select(group => new
            {
                EntitlementId = group.Key,
                Quantity = group.Sum(item => item.Quantity),
            })
            .ToDictionaryAsync(item => item.EntitlementId, item => item.Quantity, cancellationToken);

    public Task<CreditLedgerEntry?> GetConsumedByBookingIdAsync(string bookingId, CancellationToken cancellationToken) =>
        DbContext.CreditLedgerEntry
            .Include(item => item.Entitlement)
            .ThenInclude(item => item.LedgerEntries)
            .SingleOrDefaultAsync(
                item => item.BookingId == bookingId && item.TransactionType == CreditLedgerTransactionType.Consumed.ToPersistedValue(),
                cancellationToken);

    public async Task<bool> HasActiveMarketplaceBookingsAsync(string entitlementId, DateTimeOffset now, CancellationToken cancellationToken)
    {
        var bookings = await DbContext.Booking
            .Where(item => !item.DeletedAt.HasValue && item.MarketplaceBooking != null && item.MarketplaceBooking.EntitlementId == entitlementId)
            .Include(item => item.InvolvedLocations)
            .ToListAsync(cancellationToken);

        return bookings.Any(item => IsActiveAtBookingLocation(item, now));
    }

    public async Task<IReadOnlyList<Entitlement>> GetExpiredActiveAsync(DateTimeOffset now, CancellationToken cancellationToken) =>
        await DbContext.Entitlement.Where(item => item.Status == EntitlementStatus.Active && item.ExpiresAt <= now).ToListAsync(cancellationToken);

    private static bool IsActiveAtBookingLocation(BookingEntity booking, DateTimeOffset now)
    {
        var timezone = booking.InvolvedLocations
            .Select(item => item.Timezone)
            .FirstOrDefault(item => !string.IsNullOrWhiteSpace(item))
            .ToTimezoneInfo();
        var localUntil = new DateTime(
            booking.Until.Year,
            booking.Until.Month,
            booking.Until.Day,
            booking.Until.Hour,
            booking.Until.Minute,
            booking.Until.Second,
            booking.Until.Millisecond,
            DateTimeKind.Unspecified);

        return TimeZoneInfo.ConvertTimeToUtc(localUntil, timezone) >= now.UtcDateTime;
    }
}
