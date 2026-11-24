using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Microsoft.EntityFrameworkCore;

namespace Booking.Shared.Repositories;

public interface IOrganizationArrearsInvoiceRepository : IRepository<OrganizationArrearsInvoice>
{
    OrganizationArrearsInvoice Add(OrganizationArrearsInvoice organizationArrearsInvoice);
    Task<OrganizationArrearsInvoice?> GetByIdWithLinesAsync(string id, CancellationToken cancellationToken);
    Task<IReadOnlyList<OrganizationArrearsInvoice>> GetByBookingIdUntrackedAsync(string bookingId, CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<string, IReadOnlyList<OrganizationArrearsInvoice>>> GetByBookingIdsUntrackedAsync(
        IReadOnlyCollection<string> bookingIds,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<OrganizationArrearsInvoice>> GetByMarketplaceBookingSubscriptionIdUntrackedAsync(
        string marketplaceBookingSubscriptionId,
        CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<string, IReadOnlyList<OrganizationArrearsInvoice>>>
        GetByMarketplaceBookingSubscriptionIdsUntrackedAsync(
            IReadOnlyCollection<string> marketplaceBookingSubscriptionIds,
            CancellationToken cancellationToken);

    Task<IReadOnlyList<string>> GetProcessedSegmentKeysAsync(
        string organizationId,
        DateTimeOffset startInclusive,
        DateTimeOffset endExclusive,
        CancellationToken cancellationToken);
}

public class OrganizationArrearsInvoiceRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, OrganizationArrearsInvoice>(dbContext, timeProvider), IOrganizationArrearsInvoiceRepository
{
    private static readonly Func<BookingDbContext, string, DateTimeOffset, DateTimeOffset, IAsyncEnumerable<string>>
        s_getProcessedSegmentKeysQueryAsync =
            EF.CompileAsyncQuery((BookingDbContext dbContext, string organizationId, DateTimeOffset startInclusive, DateTimeOffset endExclusive) =>
                dbContext.OrganizationArrearsInvoiceLine
                    .Where(query => query.OrganizationArrearsInvoice.OrganizationId == organizationId)
                    .Where(query => query.EarnedAt >= startInclusive && query.EarnedAt < endExclusive)
                    .Select(query => query.SegmentKey));

    public OrganizationArrearsInvoice Add(OrganizationArrearsInvoice organizationArrearsInvoice)
    {
        var now = TimeProvider.GetUtcNow();
        organizationArrearsInvoice.CreatedAt = now;
        foreach (var line in organizationArrearsInvoice.Lines)
        {
            line.CreatedAt = now;
        }

        return DbContext.OrganizationArrearsInvoice.Add(organizationArrearsInvoice).Entity;
    }

    /// <summary>
    ///     Loads a single organization arrears invoice together with its line items.
    /// </summary>
    /// <param name="id">The local invoice identifier to retrieve.</param>
    /// <param name="cancellationToken">The cancellation token for the database query.</param>
    /// <returns>
    ///     The matching invoice with its <c>Lines</c> collection populated, or <see langword="null" /> when no invoice exists for the supplied
    ///     identifier.
    /// </returns>
    /// <remarks>
    ///     This repository-owned lookup was introduced to replace the former shared specification path for cancellation and reconciliation flows that need
    ///     the full invoice aggregate.
    /// </remarks>
    public async Task<OrganizationArrearsInvoice?> GetByIdWithLinesAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.OrganizationArrearsInvoice
            .Include(query => query.Lines)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<IReadOnlyList<OrganizationArrearsInvoice>>
        GetByBookingIdUntrackedAsync(string bookingId, CancellationToken cancellationToken) =>
        await DbContext.OrganizationArrearsInvoice
            .AsNoTracking()
            .Include(query => query.Organization)
            .Include(query => query.Customer)
            .Where(query => query.Lines.Any(line => line.Booking.Id == bookingId))
            .OrderByDescending(query => query.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyDictionary<string, IReadOnlyList<OrganizationArrearsInvoice>>> GetByBookingIdsUntrackedAsync(
        IReadOnlyCollection<string> bookingIds,
        CancellationToken cancellationToken)
    {
        if (bookingIds.Count == 0)
        {
            return new Dictionary<string, IReadOnlyList<OrganizationArrearsInvoice>>();
        }

        var lines = await DbContext.OrganizationArrearsInvoiceLine
            .AsNoTrackingWithIdentityResolution()
            .Include(line => line.OrganizationArrearsInvoice)
            .ThenInclude(invoice => invoice.Organization)
            .Include(line => line.OrganizationArrearsInvoice)
            .ThenInclude(invoice => invoice.Customer)
            .Include(line => line.Booking)
            .Where(line => bookingIds.Contains(line.Booking.Id))
            .ToListAsync(cancellationToken);

        return lines
            .GroupBy(line => line.Booking.Id)
            .ToDictionary(
                group => group.Key,
                group => (IReadOnlyList<OrganizationArrearsInvoice>)
                [
                    .. group
                        .Select(line => line.OrganizationArrearsInvoice)
                        .DistinctBy(invoice => invoice.Id)
                        .OrderByDescending(invoice => invoice.CreatedAt),
                ]);
    }

    public async Task<IReadOnlyList<OrganizationArrearsInvoice>> GetByMarketplaceBookingSubscriptionIdUntrackedAsync(
        string marketplaceBookingSubscriptionId,
        CancellationToken cancellationToken) =>
        await DbContext.OrganizationArrearsInvoice
            .AsNoTracking()
            .Include(query => query.Organization)
            .Include(query => query.Customer)
            .Where(query =>
                query.Lines.Any(line =>
                    (line.Booking.RecurringBooking != null &&
                     line.Booking.RecurringBooking.MarketplaceBookingSubscription != null &&
                     line.Booking.RecurringBooking.MarketplaceBookingSubscription.Id == marketplaceBookingSubscriptionId) ||
                    (line.Booking.MarketplaceBooking != null &&
                     line.Booking.MarketplaceBooking.MarketplaceBookingSubscriptionId == marketplaceBookingSubscriptionId)))
            .OrderByDescending(query => query.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyDictionary<string, IReadOnlyList<OrganizationArrearsInvoice>>>
        GetByMarketplaceBookingSubscriptionIdsUntrackedAsync(
            IReadOnlyCollection<string> marketplaceBookingSubscriptionIds,
            CancellationToken cancellationToken)
    {
        if (marketplaceBookingSubscriptionIds.Count == 0)
        {
            return new Dictionary<string, IReadOnlyList<OrganizationArrearsInvoice>>();
        }

        var lines = await DbContext.OrganizationArrearsInvoiceLine
            .AsNoTrackingWithIdentityResolution()
            .Include(line => line.OrganizationArrearsInvoice)
            .ThenInclude(invoice => invoice.Organization)
            .Include(line => line.OrganizationArrearsInvoice)
            .ThenInclude(invoice => invoice.Customer)
            .Include(line => line.Booking)
            .ThenInclude(booking => booking.RecurringBooking)
            .ThenInclude(recurringBooking => recurringBooking!.MarketplaceBookingSubscription)
            .Include(line => line.Booking)
            .ThenInclude(booking => booking.MarketplaceBooking)
            .Where(line =>
                (line.Booking.RecurringBooking != null &&
                 line.Booking.RecurringBooking.MarketplaceBookingSubscription != null &&
                 marketplaceBookingSubscriptionIds.Contains(line.Booking.RecurringBooking.MarketplaceBookingSubscription.Id)) ||
                (line.Booking.MarketplaceBooking != null &&
                 line.Booking.MarketplaceBooking.MarketplaceBookingSubscriptionId != null &&
                 marketplaceBookingSubscriptionIds.Contains(line.Booking.MarketplaceBooking.MarketplaceBookingSubscriptionId)))
            .ToListAsync(cancellationToken);

        return lines
            .Select(line => new
            {
                SubscriptionId = line.Booking.RecurringBooking?.MarketplaceBookingSubscription?.Id ??
                                 line.Booking.MarketplaceBooking!.MarketplaceBookingSubscriptionId!,
                line.OrganizationArrearsInvoice,
            })
            .GroupBy(row => row.SubscriptionId)
            .ToDictionary(
                group => group.Key,
                group => (IReadOnlyList<OrganizationArrearsInvoice>)
                [
                    .. group
                        .Select(row => row.OrganizationArrearsInvoice)
                        .DistinctBy(invoice => invoice.Id)
                        .OrderByDescending(invoice => invoice.CreatedAt),
                ]);
    }

    public async Task<IReadOnlyList<string>> GetProcessedSegmentKeysAsync(
        string organizationId,
        DateTimeOffset startInclusive,
        DateTimeOffset endExclusive,
        CancellationToken cancellationToken) =>
        await s_getProcessedSegmentKeysQueryAsync(DbContext, organizationId, startInclusive, endExclusive)
            .Distinct()
            .ToListAsync(cancellationToken);
}
