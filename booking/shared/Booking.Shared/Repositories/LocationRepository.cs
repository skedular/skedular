using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Booking.Shared.Repositories;

public interface ILocationRepository : IRepository<Location>
{
    Task<Location> UpsertNakedAsync(string id, Organization? organization, CancellationToken cancellationToken);
    Task<Location?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<Location?> GetByIdAndExcludeDeactivatedDesksAsync(string id, CancellationToken cancellationToken);
    Location Add(Location location);
    Location Update(Location location);
    Location Remove(Location location);
    Task<ICollection<Location>> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken);
}

internal static class LocationExtensions
{
    internal static IIncludableQueryable<Location, ICollection<Customer>> AddDependentObjects(
        this IQueryable<Location> originalQuery,
        bool includeDeactivated) =>
        originalQuery
            .Include(query => query.LocationMembers.Where(locationMember => !locationMember.DeletedAt.HasValue))
            .ThenInclude(query => query.Customer)
            .ThenInclude(query => query.Identities)
            .Include(query => query.Tags.Where(desk => !desk.DeletedAt.HasValue))
            .Include(query =>
                query.Desks.Where(desk => !desk.DeletedAt.HasValue && (includeDeactivated || !desk.Deactivated)))
            .ThenInclude(query => query.Tags.Where(tag => !tag.DeletedAt.HasValue))
            .Include(query =>
                query.Desks.Where(desk => !desk.DeletedAt.HasValue && (includeDeactivated || !desk.Deactivated)))
            .ThenInclude(query => query.OrganizationTags.Where(tag => !tag.DeletedAt.HasValue))
            .Include(query => query.Organization)
            .ThenInclude(query =>
                query.OrganizationMembers.Where(organizationMember => !organizationMember.DeletedAt.HasValue))
            .ThenInclude(query => query.Customer)
            .Include(query => query.DefaultedByCustomers);
}

public class LocationRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, Location>(dbContext), ILocationRepository
{
    public async Task<Location>
        UpsertNakedAsync(string id, Organization? organization, CancellationToken cancellationToken)
    {
        var existing = await GetByIdAsync(id, cancellationToken);
        if (existing is not null)
        {
            return existing;
        }

        var now = timeProvider.GetUtcNow();
        return DbContext.Location.Add(new Location { Id = id, CreatedAt = now, Organization = organization }).Entity;
    }

    public async Task<Location?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Location
            .Where(query => query.Id == id)
            .AddDependentObjects(true)
            .OrderBy(query => query.Id)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<Location?>
        GetByIdAndExcludeDeactivatedDesksAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Location
            .Where(query => query.Id == id)
            .AddDependentObjects(false)
            .OrderBy(query => query.Id)
            .FirstOrDefaultAsync(cancellationToken);

    public Location Add(Location location)
    {
        var now = timeProvider.GetUtcNow();
        location.CreatedAt = now;
        return DbContext.Location.Add(location).Entity;
    }

    public Location Remove(Location location)
    {
        var now = timeProvider.GetUtcNow();
        location.DeletedAt = now;
        return DbContext.Location.Update(location).Entity;
    }

    public Location Update(Location location)
    {
        var now = timeProvider.GetUtcNow();
        location.ModifiedAt = now;
        return DbContext.Location.Update(location).Entity;
    }

    public async Task<ICollection<Location>> GetByCustomerIdAsync(
        string customerId,
        CancellationToken cancellationToken) =>
        await DbContext.Location
            .Where(query => !query.DeletedAt.HasValue &&
                            ((query.Organization == null && query.LocationMembers.Any(locationMember =>
                                 !locationMember.DeletedAt.HasValue && locationMember.Customer.Id == customerId)) ||
                             (query.Organization != null && !query.Organization.DeletedAt.HasValue &&
                              query.Organization.OrganizationMembers.Any(
                                  organizationMember =>
                                      !organizationMember.DeletedAt.HasValue &&
                                      organizationMember.Customer.Id == customerId))))
            .ToListAsync(cancellationToken);
}
