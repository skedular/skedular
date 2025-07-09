using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Pagination;
using HotChocolate.Types.Pagination;
using Location.Shared.Database;
using Location.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using OrganizationTag = Location.Shared.Database.Entities.OrganizationTag;

namespace Location.Shared.Repositories;

public interface ILocationRepository : IRepository<Database.Entities.Location>
{
    Task<Database.Entities.Location?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<ICollection<Database.Entities.Location>> GetByIdsAsync(ICollection<string> ids, CancellationToken cancellationToken);
    Task<IEnumerable<Database.Entities.Location>> GetByCustomerIdAsync(string customerId, string? locationId, CancellationToken cancellationToken);
    Task<ICollection<Database.Entities.Location>> GetAllAsync(bool includeDeletedResources, CancellationToken cancellationToken);
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
    internal static IIncludableQueryable<Database.Entities.Location, IEnumerable<OrganizationTag>> AddDependentObjects(
        this IQueryable<Database.Entities.Location> originalQuery,
        bool includeDeletedResources) =>
        originalQuery
            .Include(query => query.Organization)
            .ThenInclude(query => query.Tags.Where(tag => !tag.DeletedAt.HasValue))
            .Include(query => query.Organization)
            .ThenInclude(query => query.OrganizationMembers.Where(organizationMember => !organizationMember.DeletedAt.HasValue))
            .ThenInclude(query => query.Customer)
            .Include(query => query.Address)
            .Include(query => query.PhysicalAddress)
            .Include(query => query.Resources.Where(resource => includeDeletedResources || !resource.DeletedAt.HasValue))
            .ThenInclude(query => query.OrganizationTags.Where(tag => !tag.DeletedAt.HasValue))
            .Include(query => query.FloorPlans.Where(fp => !fp.DeletedAt.HasValue))
            .ThenInclude(query => query.ResourcePositions)
            .ThenInclude(query => query.Resource)
            .Include(query => query.OrganizationTags.Where(tag => !tag.DeletedAt.HasValue));

    internal static IQueryable<Database.Entities.Location> AddSearchCriteria(
        this IQueryable<Database.Entities.Location> query,
        LocationSearchCriteria searchCriteria)
    {
        query = query.Where(item => !item.DeletedAt.HasValue);

        if (string.IsNullOrWhiteSpace(searchCriteria.OrganizationId))
        {
            query = query.Where(item => !item.Organization.DeletedAt.HasValue &&
                                        (searchCriteria.CustomerId == null || item.Organization.OrganizationMembers.Any(organizationMember =>
                                            !organizationMember.DeletedAt.HasValue && organizationMember.Customer.Id == searchCriteria.CustomerId)));
        }
        else
        {
            query = query.Where(item => !item.Organization.DeletedAt.HasValue &&
                                        item.Organization.Id == searchCriteria.OrganizationId &&
                                        (searchCriteria.CustomerId == null || item.Organization.OrganizationMembers.Any(organizationMember =>
                                            !organizationMember.DeletedAt.HasValue && organizationMember.Customer.Id == searchCriteria.CustomerId)));
        }

        if (searchCriteria.LocationIds.Count > 0)
        {
            query = query.Where(item => searchCriteria.LocationIds.Contains(item.Id));
        }

        if (!string.IsNullOrWhiteSpace(searchCriteria.NameContains))
        {
            query = query.Where(item => EF.Functions.ILike(item.Name, $"%{searchCriteria.NameContains}%"));
        }

        if (searchCriteria.TagIds.Count != 0)
        {
            searchCriteria.TagIds.ForEach(id =>
                query = query.Where(item =>
                    item.Resources.Any(resource => !resource.DeletedAt.HasValue && resource.OrganizationTags.Select(tag => tag.Id).Contains(id))));
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
                LocationOrderField.About => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.About)
                    : query.ThenByDescending(x => x.About),
                LocationOrderField.Timezone => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.Timezone)
                    : query.ThenByDescending(x => x.Timezone),
                _ => throw new ArgumentOutOfRangeException()
            }).ThenBy(query => query.Id);
    }
}

public class LocationRepository(LocationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<LocationDbContext, Database.Entities.Location>(dbContext, timeProvider), ILocationRepository
{
    public async Task<Database.Entities.Location?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Location
            .AddDependentObjects(false)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<ICollection<Database.Entities.Location>> GetByIdsAsync(ICollection<string> ids, CancellationToken cancellationToken) =>
        await DbContext.Location
            .Where(query => ids.Contains(query.Id))
            .AddDependentObjects(false)
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
            .Where(location => !location.DeletedAt.HasValue && !location.Organization.DeletedAt.HasValue &&
                               location.Organization.OrganizationMembers.Any(organizationMember =>
                                   organizationMember.Customer.Id == customerId));

        if (!string.IsNullOrWhiteSpace(locationId))
        {
            query = query.Where(location => location.Id == locationId);
        }

        return await query
            .AddDependentObjects(false)
            .ToListAsync(cancellationToken);
    }

    public async Task<ICollection<Database.Entities.Location>> GetAllAsync(bool includeDeletedResources, CancellationToken cancellationToken) =>
        await DbContext.Location
            .Where(query => !query.DeletedAt.HasValue)
            .AddDependentObjects(includeDeletedResources)
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
            .AddDependentObjects(false)
            .ToListAsync(cancellationToken))
        .ToPaginated(paginationInputParam);
}
