using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace Booking.Shared.Repositories;

public interface IOrganizationInvoiceCounterRepository : IRepository<OrganizationInvoiceCounter>
{
    Task<OrganizationInvoiceCounter?> GetByOrganizationIdAsync(string organizationId, CancellationToken cancellationToken);
    OrganizationInvoiceCounter Add(OrganizationInvoiceCounter organizationInvoiceCounter);
    OrganizationInvoiceCounter Update(OrganizationInvoiceCounter organizationInvoiceCounter);
}

public class OrganizationInvoiceCounterRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, OrganizationInvoiceCounter>(dbContext, timeProvider), IOrganizationInvoiceCounterRepository
{
    private static readonly Func<BookingDbContext, string, CancellationToken, Task<OrganizationInvoiceCounter?>>
        s_getByOrganizationIdQueryAsync =
            EF.CompileAsyncQuery<BookingDbContext, string, CancellationToken, OrganizationInvoiceCounter?>((
                    dbContext,
                    organizationId,
                    cancellationToken) =>
                dbContext.OrganizationInvoiceCounter
                    .TagWith(EntityFrameworkInterceptorTags.ForUpdate)
                    .FirstOrDefault(query => query.Organization.Id == organizationId));

    public async Task<OrganizationInvoiceCounter?> GetByOrganizationIdAsync(string organizationId, CancellationToken cancellationToken) =>
        await s_getByOrganizationIdQueryAsync(DbContext, organizationId, cancellationToken);

    public OrganizationInvoiceCounter Add(OrganizationInvoiceCounter organizationInvoiceCounter)
    {
        var now = TimeProvider.GetUtcNow();
        organizationInvoiceCounter.CreatedAt = now;
        return DbContext.OrganizationInvoiceCounter.Add(organizationInvoiceCounter).Entity;
    }

    public OrganizationInvoiceCounter Update(OrganizationInvoiceCounter organizationInvoiceCounter)
    {
        var now = TimeProvider.GetUtcNow();
        organizationInvoiceCounter.ModifiedAt = now;
        return DbContext.OrganizationInvoiceCounter.Update(organizationInvoiceCounter).Entity;
    }
}
