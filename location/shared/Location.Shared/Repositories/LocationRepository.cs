using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Models;
using Enterprise.Shared.Pagination;
using Location.Shared.Database;
using Location.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Customer = Location.Shared.Database.Entities.Customer;

namespace Location.Shared.Repositories;

public interface ILocationRepository : IRepository<Database.Entities.Location>
{
    Task<Database.Entities.Location?> GetByIdAsync(string id, CancellationToken cancellationToken);

    Task<ICollection<Database.Entities.Location>> GetByIdsAsync(
        ICollection<string> ids,
        CancellationToken cancellationToken);

    Task<IEnumerable<Database.Entities.Location>> GetByCustomerIdAsync(
        string customerId,
        string? locationId,
        CancellationToken cancellationToken);

    Task<ICollection<Database.Entities.Location>> GetAllAsync(CancellationToken cancellationToken);
    Database.Entities.Location Add(Database.Entities.Location location);
    Database.Entities.Location Update(Database.Entities.Location location);
    Database.Entities.Location Remove(Database.Entities.Location location);

    Task<(PaginatedInfo, ICollection<Edge<Database.Entities.Location>>, int )> GetPaginatedLocationsAsync(
        PaginationInputParam paginationInputParam,
        LocationSearchCriteria searchCriteria,
        ICollection<LocationOrder> orderByFields,
        CancellationToken cancellationToken);
}

internal static class LocationExtensions
{
    internal static IIncludableQueryable<Database.Entities.Location, Customer> AddDependentObjects(
        this IQueryable<Database.Entities.Location> originalQuery) =>
        originalQuery
            .Include(query => query.Organization)
            .ThenInclude(query =>
                query.OrganizationMembers.Where(organizationMember => !organizationMember.DeletedAt.HasValue))
            .ThenInclude(query => query.Customer)
            .Include(query => query.PhysicalAddress)
            .Include(query => query.Desks.Where(desk => !desk.DeletedAt.HasValue))
            .ThenInclude(query => query.OrganizationTags.Where(organizationTag => !organizationTag.DeletedAt.HasValue))
            .Include(query => query.LocationMembers.Where(locationMember => !locationMember.DeletedAt.HasValue))
            .ThenInclude(query => query.Customer);

    internal static IQueryable<Database.Entities.Location> AddSearchCriteria(
        this IQueryable<Database.Entities.Location> query,
        LocationSearchCriteria searchCriteria)
    {
        query = query.Where(item => !item.DeletedAt.HasValue);

        if (string.IsNullOrWhiteSpace(searchCriteria.OrganizationId))
        {
            query = query.Where(item =>
                (item.Organization == null && item.LocationMembers.Any(locationMember =>
                    !locationMember.DeletedAt.HasValue && (searchCriteria.CustomerId == null ||
                                                           locationMember.Customer.Id == searchCriteria.CustomerId))) ||
                (item.Organization != null && !item.Organization.DeletedAt.HasValue &&
                 (searchCriteria.CustomerId == null || item.Organization.OrganizationMembers.Any(organizationMember =>
                     !organizationMember.DeletedAt.HasValue &&
                     organizationMember.Customer.Id ==
                     searchCriteria.CustomerId))));
        }
        else
        {
            query = query.Where(item =>
                item.Organization != null && !item.Organization.DeletedAt.HasValue &&
                item.Organization.Id == searchCriteria.OrganizationId &&
                (searchCriteria.CustomerId == null || item.Organization.OrganizationMembers.Any(organizationMember =>
                    !organizationMember.DeletedAt.HasValue &&
                    organizationMember.Customer.Id ==
                    searchCriteria.CustomerId)));
        }

        if (!string.IsNullOrWhiteSpace(searchCriteria.NameContains))
        {
            query = query.Where(item => EF.Functions.ILike(item.Name, $"%{searchCriteria.NameContains}%"));
        }

        if (searchCriteria.ZoneIds.Length != 0)
        {
            searchCriteria.ZoneIds.ForEach(id =>
                query = query.Where(item =>
                    item.Desks.Any(desk =>
                        !desk.DeletedAt.HasValue && desk.OrganizationTags.Select(tag => tag.Id).Contains(id))));
        }

        if (searchCriteria.CustomTagIds.Length != 0)
        {
            searchCriteria.CustomTagIds.ForEach(id =>
                query = query.Where(item =>
                    item.Desks.Any(desk =>
                        !desk.DeletedAt.HasValue && desk.OrganizationTags.Select(tag => tag.Id).Contains(id))));
        }

        return query;
    }

    internal static IQueryable<Database.Entities.Location> AddSortingOrders(
        this IQueryable<Database.Entities.Location> originalQuery,
        ICollection<LocationOrder> orderByFields)
    {
        if (orderByFields.Count == 0)
        {
            return originalQuery.OrderBy(query => query.Name).ThenBy(query => query.Id);
        }

        var orderByField = orderByFields.First();
        return orderByFields.Skip(1).Aggregate(orderByField.Field switch
        {
            LocationOrderField.Name => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.Name)
                : originalQuery.OrderByDescending(x => x.Name),
            LocationOrderField.About => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.About)
                : originalQuery.OrderByDescending(x => x.About),
            LocationOrderField.Timezone => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.Timezone)
                : originalQuery.OrderByDescending(x => x.Timezone),
            _ => throw new ArgumentOutOfRangeException()
        }, (query, orderField) =>
            orderField.Field switch
            {
                LocationOrderField.Name => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.Name)
                    : query.ThenByDescending(x => x.Name),
                _ => throw new ArgumentOutOfRangeException()
            }).ThenBy(query => query.Id);
    }
}

public class LocationRepository(LocationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<LocationDbContext, Database.Entities.Location>(dbContext, timeProvider), ILocationRepository
{
    public async Task<Database.Entities.Location?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Location
            .AddDependentObjects()
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<ICollection<Database.Entities.Location>> GetByIdsAsync(
        ICollection<string> ids,
        CancellationToken cancellationToken) =>
        await DbContext.Location
            .Where(query => ids.Contains(query.Id))
            .AddDependentObjects()
            .ToListAsync(cancellationToken);

    public Database.Entities.Location Add(Database.Entities.Location location)
    {
        var now = TimeProvider.GetUtcNow();
        location.CreatedAt = now;
        return DbContext.Location.Add(location).Entity;
    }

    public Database.Entities.Location Update(Database.Entities.Location location)
    {
        var now = TimeProvider.GetUtcNow();
        location.ModifiedAt = now;
        return DbContext.Location.Update(location).Entity;
    }

    public async Task<IEnumerable<Database.Entities.Location>> GetByCustomerIdAsync(
        string customerId,
        string? locationId,
        CancellationToken cancellationToken)
    {
        var query = DbContext.Location
            .Where(location =>
                !location.DeletedAt.HasValue && ((location.Organization == null &&
                                                  location.LocationMembers.Any(item =>
                                                      item.Customer.Id == customerId)) ||
                                                 (location.Organization != null &&
                                                  !location.Organization.DeletedAt.HasValue &&
                                                  location.Organization.OrganizationMembers.Any(organizationMember =>
                                                      organizationMember.Customer.Id == customerId))));

        query = string.IsNullOrWhiteSpace(locationId)
            ? query.Where(team =>
                team.Organization == null || (team.Organization != null && !team.Organization.DeletedAt.HasValue))
            : query.Where(team =>
                team.Organization != null &&
                !team.Organization.DeletedAt.HasValue &&
                team.Organization.Id == locationId);

        return await query
            .AddDependentObjects()
            .ToListAsync(cancellationToken);
    }

    public async Task<ICollection<Database.Entities.Location>> GetAllAsync(CancellationToken cancellationToken) =>
        await DbContext.Location
            .Where(query => !query.DeletedAt.HasValue)
            .AddDependentObjects()
            .ToListAsync(cancellationToken);

    public Database.Entities.Location Remove(Database.Entities.Location location)
    {
        var now = TimeProvider.GetUtcNow();
        location.DeletedAt = now;
        return DbContext.Location.Update(location).Entity;
    }

    public async Task<(PaginatedInfo, ICollection<Edge<Database.Entities.Location>>, int)> GetPaginatedLocationsAsync(
        PaginationInputParam paginationInputParam,
        LocationSearchCriteria searchCriteria,
        ICollection<LocationOrder> orderByFields,
        CancellationToken cancellationToken) =>
        (await DbContext.Location
            .AddSearchCriteria(searchCriteria)
            .AddSortingOrders(orderByFields)
            .AddDependentObjects()
            .ToListAsync(cancellationToken))
        .ToPaginated(paginationInputParam);
}
