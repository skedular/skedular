using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Booking.Shared.Repositories;

public interface ILocationRepository : IRepository<Location>
{
    Task<Location> UpsertNakedAsync(string id, Organization? organization, CancellationToken cancellationToken);

    Task<ICollection<Location>> GetAllAsync(
        bool includeDeletedLocationMembers,
        bool includeDeletedResources,
        bool includeDeletedDesks,
        bool includeDeletedRooms,
        CancellationToken cancellationToken);

    Task<Location?> GetByIdAsync(
        string id,
        bool includeDeletedLocationMembers,
        bool includeDeletedResources,
        bool includeDeletedDesks,
        bool includeDeletedRooms,
        CancellationToken cancellationToken);

    Task<Location?> GetByIdAndExcludeDeactivatedDesksAndRoomsAsync(
        string id,
        bool includeDeletedLocationMembers,
        bool includeDeletedResources,
        bool includeDeletedDesks,
        bool includeDeletedRooms,
        CancellationToken cancellationToken);

    Location Add(Location location);
    Location Update(Location location);
    Location Remove(Location location);

    Task<ICollection<Location>> GetByCustomerIdAsync(
        string customerId,
        bool includeDeletedLocationMembers,
        bool includeDeletedResources,
        bool includeDeletedDesks,
        bool includeDeletedRooms,
        CancellationToken cancellationToken);

    Task<ICollection<Location>> GetByOrganizationIdAsync(
        string organizationId,
        bool includeDeletedLocationMembers,
        bool includeDeletedResources,
        bool includeDeletedDesks,
        bool includeDeletedRooms,
        CancellationToken cancellationToken);
}

internal static class LocationExtensions
{
    internal static IIncludableQueryable<Location, ICollection<Customer>> AddDependentObjects(
        this IQueryable<Location> originalQuery,
        bool includeDeletedLocationMembers,
        bool includeDeletedResource,
        bool includeInactiveResource,
        bool includeDeletedDesks,
        bool includeDeactivatedDesk,
        bool includeDeletedRooms,
        bool includeDeactivatedRoom) =>
        originalQuery
            .Include(query => query.LocationMembers.Where(locationMember => includeDeletedLocationMembers || !locationMember.DeletedAt.HasValue))
            .ThenInclude(query => query.Customer)
            .ThenInclude(query => query.Identities)
            .Include(query => query.Resources.Where(resource =>
                includeDeletedResource || (!resource.DeletedAt.HasValue && (includeInactiveResource || !resource.Inactive))))
            .ThenInclude(query => query.OrganizationTags.Where(tag => !tag.DeletedAt.HasValue))
            .Include(query => query.Resources.Where(resource =>
                includeDeletedResource || (!resource.DeletedAt.HasValue && (includeInactiveResource || !resource.Inactive))))
            .ThenInclude(query => query.OrganizationResourceType)
            .Include(query =>
                query.Desks.Where(desk => includeDeletedDesks || (!desk.DeletedAt.HasValue && (includeDeactivatedDesk || !desk.Deactivated))))
            .ThenInclude(query => query.OrganizationTags.Where(tag => !tag.DeletedAt.HasValue))
            .Include(query =>
                query.Rooms.Where(room => includeDeletedRooms || (!room.DeletedAt.HasValue && (includeDeactivatedRoom || !room.Deactivated))))
            .ThenInclude(query => query.OrganizationTags.Where(tag => !tag.DeletedAt.HasValue))
            .Include(query => query.Organization)
            .ThenInclude(query => query.OrganizationMembers.Where(organizationMember => !organizationMember.DeletedAt.HasValue))
            .ThenInclude(query => query.Customer)
            .Include(query => query.DefaultedByCustomers);
}

public class LocationRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, Location>(dbContext, timeProvider), ILocationRepository
{
    public async Task<Location> UpsertNakedAsync(string id, Organization? organization, CancellationToken cancellationToken)
    {
        await UpsertNakedAsync<Organization>(id, organization, cancellationToken);

        return (await GetByIdAsync(id, true, true, true, true, cancellationToken))!;
    }

    public async Task<ICollection<Location>> GetAllAsync(
        bool includeDeletedLocationMembers,
        bool includeDeletedResources,
        bool includeDeletedDesks,
        bool includeDeletedRooms,
        CancellationToken cancellationToken) =>
        await DbContext.Location
            .AddDependentObjects(includeDeletedLocationMembers, includeDeletedResources, true, includeDeletedDesks, true, includeDeletedRooms, true)
            .ToListAsync(cancellationToken);

    public async Task<Location?> GetByIdAsync(
        string id,
        bool includeDeletedLocationMembers,
        bool includeDeletedResources,
        bool includeDeletedDesks,
        bool includeDeletedRooms,
        CancellationToken cancellationToken) =>
        await DbContext.Location
            .AddDependentObjects(includeDeletedLocationMembers, includeDeletedResources, true, includeDeletedDesks, true, includeDeletedRooms, true)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<Location?> GetByIdAndExcludeDeactivatedDesksAndRoomsAsync(
        string id,
        bool includeDeletedLocationMembers,
        bool includeDeletedResources,
        bool includeDeletedDesks,
        bool includeDeletedRooms,
        CancellationToken cancellationToken) =>
        await DbContext.Location
            .AddDependentObjects(includeDeletedLocationMembers, includeDeletedResources, false, includeDeletedDesks, false, includeDeletedRooms,
                false)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public Location Add(Location location)
    {
        var now = TimeProvider.GetUtcNow();
        location.CreatedAt = now;
        return DbContext.Location.Add(location).Entity;
    }

    public Location Remove(Location location)
    {
        var now = TimeProvider.GetUtcNow();
        location.DeletedAt = now;
        return DbContext.Location.Update(location).Entity;
    }

    public Location Update(Location location)
    {
        var now = TimeProvider.GetUtcNow();
        location.ModifiedAt = now;
        return DbContext.Location.Update(location).Entity;
    }

    public async Task<ICollection<Location>> GetByCustomerIdAsync(
        string customerId,
        bool includeDeletedLocationMembers,
        bool includeDeletedResources,
        bool includeDeletedDesks,
        bool includeDeletedRooms,
        CancellationToken cancellationToken) =>
        await DbContext.Location
            .Where(query => !query.DeletedAt.HasValue &&
                            ((query.Organization == null && query.LocationMembers.Any(locationMember =>
                                 !locationMember.DeletedAt.HasValue && locationMember.Customer.Id == customerId)) ||
                             (query.Organization != null && !query.Organization.DeletedAt.HasValue &&
                              query.Organization.OrganizationMembers.Any(
                                  organizationMember => !organizationMember.DeletedAt.HasValue && organizationMember.Customer.Id == customerId))))
            .AddDependentObjects(includeDeletedLocationMembers, includeDeletedResources, false, includeDeletedDesks, false, includeDeletedRooms,
                false)
            .ToListAsync(cancellationToken);

    public async Task<ICollection<Location>> GetByOrganizationIdAsync(
        string organizationId,
        bool includeDeletedLocationMembers,
        bool includeDeletedResources,
        bool includeDeletedDesks,
        bool includeDeletedRooms,
        CancellationToken cancellationToken) =>
        await DbContext.Location
            .Where(query => !query.DeletedAt.HasValue && query.Organization != null && query.Organization.Id == organizationId)
            .AddDependentObjects(includeDeletedLocationMembers, includeDeletedResources, false, includeDeletedDesks, false, includeDeletedRooms,
                false)
            .ToListAsync(cancellationToken);
}
