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

    Task<IReadOnlyList<OrganizationArrearsInvoice>> GetByMarketplaceBookingSubscriptionIdUntrackedAsync(
        string marketplaceBookingSubscriptionId,
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

    public async Task<IReadOnlyList<string>> GetProcessedSegmentKeysAsync(
        string organizationId,
        DateTimeOffset startInclusive,
        DateTimeOffset endExclusive,
        CancellationToken cancellationToken) =>
        await s_getProcessedSegmentKeysQueryAsync(DbContext, organizationId, startInclusive, endExclusive)
            .Distinct()
            .ToListAsync(cancellationToken);
}
