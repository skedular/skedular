using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.Postgres;
using Microsoft.EntityFrameworkCore;

namespace Booking.Shared.Repositories;

public interface IOrganizationArrearsInvoiceRepository : IRepository<OrganizationArrearsInvoice>
{
    OrganizationArrearsInvoice Add(OrganizationArrearsInvoice organizationArrearsInvoice);
    Task<ICollection<OrganizationArrearsInvoice>> GetByBookingIdUntrackedAsync(string bookingId, CancellationToken cancellationToken);
    Task<ICollection<OrganizationArrearsInvoice>> GetByOrganizationIdUntrackedAsync(string organizationId, CancellationToken cancellationToken);

    Task<ICollection<OrganizationArrearsInvoice>> GetByMarketplaceBookingSubscriptionIdUntrackedAsync(
        string marketplaceBookingSubscriptionId,
        CancellationToken cancellationToken);

    Task<ICollection<string>> GetProcessedSegmentKeysAsync(
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

    public async Task<ICollection<OrganizationArrearsInvoice>> GetByBookingIdUntrackedAsync(string bookingId, CancellationToken cancellationToken) =>
        await DbContext.OrganizationArrearsInvoice
            .AsNoTracking()
            .Include(query => query.Organization)
            .Include(query => query.Customer)
            .Where(query => query.Lines.Any(line => line.Booking.Id == bookingId))
            .OrderByDescending(query => query.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<ICollection<OrganizationArrearsInvoice>> GetByOrganizationIdUntrackedAsync(
        string organizationId,
        CancellationToken cancellationToken) =>
        await DbContext.OrganizationArrearsInvoice
            .AsNoTracking()
            .Include(query => query.Organization)
            .Include(query => query.Customer)
            .Include(query => query.Lines)
            .ThenInclude(query => query.Booking)
            .Where(query => query.OrganizationId == organizationId)
            .OrderBy(query => query.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<ICollection<OrganizationArrearsInvoice>> GetByMarketplaceBookingSubscriptionIdUntrackedAsync(
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

    public async Task<ICollection<string>> GetProcessedSegmentKeysAsync(
        string organizationId,
        DateTimeOffset startInclusive,
        DateTimeOffset endExclusive,
        CancellationToken cancellationToken) =>
        await s_getProcessedSegmentKeysQueryAsync(DbContext, organizationId, startInclusive, endExclusive)
            .Distinct()
            .ToListAsync(cancellationToken);
}
