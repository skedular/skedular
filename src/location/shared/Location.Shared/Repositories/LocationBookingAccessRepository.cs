using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Location.Shared.Database;
using Location.Shared.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Location.Shared.Repositories;

public interface ILocationBookingAccessRepository : IRepository<LocationBookingAccess>
{
    Task<LocationBookingAccess?> GetByCustomerLocationAndOrganizationAsync(
        string customerId,
        string locationId,
        string organizationId,
        CancellationToken cancellationToken);

    Task<bool> AnyActiveByCustomerAndLocationAsync(string customerId, string locationId, CancellationToken cancellationToken);
    Task<bool> AnyActiveByCustomerAndOrganizationAsync(string customerId, string organizationId, CancellationToken cancellationToken);
    LocationBookingAccess Add(LocationBookingAccess access);
    LocationBookingAccess Update(LocationBookingAccess access);
}

public class LocationBookingAccessRepository(LocationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<LocationDbContext, LocationBookingAccess>(dbContext, timeProvider), ILocationBookingAccessRepository
{
    public async Task<LocationBookingAccess?> GetByCustomerLocationAndOrganizationAsync(
        string customerId,
        string locationId,
        string organizationId,
        CancellationToken cancellationToken) =>
        await DbContext.LocationBookingAccess
            .FirstOrDefaultAsync(
                query =>
                    query.CustomerId == customerId &&
                    query.LocationId == locationId &&
                    query.OrganizationId == organizationId,
                cancellationToken);

    public async Task<bool> AnyActiveByCustomerAndLocationAsync(
        string customerId,
        string locationId,
        CancellationToken cancellationToken) =>
        await DbContext.LocationBookingAccess
            .AsNoTrackingWithIdentityResolution()
            .AnyAsync(
                query =>
                    query.CustomerId == customerId &&
                    query.LocationId == locationId &&
                    query.ActiveBookingCount > 0 &&
                    !query.DeletedAt.HasValue,
                cancellationToken);

    public async Task<bool> AnyActiveByCustomerAndOrganizationAsync(
        string customerId,
        string organizationId,
        CancellationToken cancellationToken) =>
        await DbContext.LocationBookingAccess
            .AsNoTrackingWithIdentityResolution()
            .AnyAsync(
                query =>
                    query.CustomerId == customerId &&
                    query.OrganizationId == organizationId &&
                    query.ActiveBookingCount > 0 &&
                    !query.DeletedAt.HasValue,
                cancellationToken);

    public LocationBookingAccess Add(LocationBookingAccess access)
    {
        var now = TimeProvider.GetUtcNow();
        access.CreatedAt = now;
        return DbContext.LocationBookingAccess.Add(access).Entity;
    }

    public LocationBookingAccess Update(LocationBookingAccess access)
    {
        var now = TimeProvider.GetUtcNow();
        access.ModifiedAt = now;
        return DbContext.LocationBookingAccess.Update(access).Entity;
    }
}
