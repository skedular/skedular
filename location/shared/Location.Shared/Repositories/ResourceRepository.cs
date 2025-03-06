using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Models;
using Enterprise.Shared.Pagination;
using Location.Shared.Database;
using Location.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Resource = Location.Shared.Database.Entities.Resource;
using OrganizationTag = Location.Shared.Database.Entities.OrganizationTag;

namespace Location.Shared.Repositories;

public interface IResourceRepository : IRepository<Resource>
{
    Task<Resource?> GetByIdAsync(string id, bool includeAllRelatedEntities, CancellationToken cancellationToken);
    Task<ICollection<Resource>> GetByIdsAsync(ICollection<string> ids, bool includeAllRelatedEntities, CancellationToken cancellationToken);
    Resource Add(Resource resource);
    Resource Update(Resource resource);
    void RemoveRange(ICollection<Resource> resources);
    Resource Remove(Resource resource);

    Task<(PaginatedInfo, ICollection<Edge<Resource>>, int )> GetPaginatedResourcesAsync(
        PaginationInputParam paginationInputParam,
        ResourceSearchCriteria searchCriteria,
        ICollection<ResourceOrder> orderByFields,
        CancellationToken cancellationToken);
}

internal static class ResourceExtensions
{
    internal static IIncludableQueryable<Resource, IEnumerable<OrganizationTag>> AddDependentObjects(
        this IQueryable<Resource> originalQuery,
        bool includeAllRelatedEntities) =>
        includeAllRelatedEntities
            ? originalQuery
                .Include(query => query.Bookings)
                .Include(query => query.OrganizationResourceType)
                .Include(query => query.Location)
                .Include(query => query.OrganizationTags.Where(organizationTag => !organizationTag.DeletedAt.HasValue))
            : originalQuery
                .Include(query => query.OrganizationResourceType)
                .Include(query => query.Location)
                .Include(query => query.OrganizationTags.Where(organizationTag => !organizationTag.DeletedAt.HasValue));

    internal static IQueryable<Resource> AddSearchCriteria(this IQueryable<Resource> query, ResourceSearchCriteria searchCriteria)
    {
        query = query.Where(item => !item.DeletedAt.HasValue && item.Location.Id == searchCriteria.LocationId);

        if (!string.IsNullOrWhiteSpace(searchCriteria.NameContains))
        {
            query = query.Where(item => EF.Functions.ILike(item.Name, $"%{searchCriteria.NameContains}%"));
        }

        if (searchCriteria.ZoneIds.Count != 0)
        {
            query = query.Where(item => searchCriteria.ZoneIds.All(zoneId => item.OrganizationTags.Any(tag => tag.Id == zoneId)));
        }

        if (searchCriteria.CustomTagIds.Count != 0)
        {
            query = query.Where(item => searchCriteria.CustomTagIds.All(customTagId => item.OrganizationTags.Any(tag => tag.Id == customTagId)));
        }

        return query;
    }

    internal static IQueryable<Resource> AddSortingOrders(this IQueryable<Resource> originalQuery, ICollection<ResourceOrder> orderByFields)
    {
        if (orderByFields.Count == 0)
        {
            return originalQuery.OrderBy(query => query.Name).ThenBy(query => query.Id);
        }

        var orderByField = orderByFields.First();
        return orderByFields.Skip(1).Aggregate(orderByField.Field switch
        {
            ResourceOrderField.Name => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.Name)
                : originalQuery.OrderByDescending(x => x.Name),
            _ => throw new ArgumentOutOfRangeException()
        }, (query, orderField) =>
            orderField.Field switch
            {
                ResourceOrderField.Name => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.Name)
                    : query.ThenByDescending(x => x.Name),
                _ => throw new ArgumentOutOfRangeException()
            }).ThenBy(query => query.Id);
    }
}

public class ResourceRepository(LocationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<LocationDbContext, Resource>(dbContext, timeProvider), IResourceRepository
{
    public async Task<Resource?> GetByIdAsync(string id, bool includeAllRelatedEntities, CancellationToken cancellationToken) =>
        await DbContext.Resource
            .AddDependentObjects(includeAllRelatedEntities)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<ICollection<Resource>> GetByIdsAsync(ICollection<string> ids, bool includeAllRelatedEntities,
        CancellationToken cancellationToken) =>
        await DbContext.Resource
            .Where(query => ids.Contains(query.Id))
            .AddDependentObjects(includeAllRelatedEntities)
            .ToListAsync(cancellationToken);

    public Resource Add(Resource resource)
    {
        var now = TimeProvider.GetUtcNow();
        resource.CreatedAt = now;
        return DbContext.Resource.Add(resource).Entity;
    }

    public void RemoveRange(ICollection<Resource> resources)
    {
        var now = TimeProvider.GetUtcNow();
        resources.ForEach(resource => resource.DeletedAt = now);
        DbContext.Resource.UpdateRange(resources);
    }

    public Resource Remove(Resource resource)
    {
        var now = TimeProvider.GetUtcNow();
        resource.DeletedAt = now;
        return DbContext.Resource.Update(resource).Entity;
    }

    public Resource Update(Resource resource)
    {
        var now = TimeProvider.GetUtcNow();
        resource.ModifiedAt = now;
        return DbContext.Resource.Update(resource).Entity;
    }

    public async Task<(PaginatedInfo, ICollection<Edge<Resource>>, int)> GetPaginatedResourcesAsync(
        PaginationInputParam paginationInputParam,
        ResourceSearchCriteria searchCriteria,
        ICollection<ResourceOrder> orderByFields,
        CancellationToken cancellationToken) =>
        (await DbContext.Resource
            .AddSearchCriteria(searchCriteria)
            .AddSortingOrders(orderByFields)
            .AddDependentObjects(false)
            .ToListAsync(cancellationToken))
        .ToPaginated(paginationInputParam);
}
