using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Models;
using Enterprise.Shared.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Organization.Shared.Database;
using Organization.Shared.Models;
using ResourceType = Organization.Shared.Database.Entities.ResourceType;

namespace Organization.Shared.Repositories;

public interface IResourceTypeRepository : IRepository<ResourceType>
{
    Task<ResourceType?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<ICollection<ResourceType>> GetByIdsAsync(ICollection<string> ids, CancellationToken cancellationToken);
    ResourceType Add(ResourceType resourceType);
    ResourceType Update(ResourceType resourceType);
    void RemoveRange(ICollection<ResourceType> resourceTypes);
    ResourceType Remove(ResourceType resourceType);

    Task<(PaginatedInfo, ICollection<Edge<ResourceType>>, int )> GetPaginatedResourceTypesAsync(
        PaginationInputParam paginationInputParam,
        ResourceTypeSearchCriteria searchCriteria,
        ICollection<ResourceTypeOrder> orderByFields,
        CancellationToken cancellationToken);
}

internal static class ResourceTypeExtensions
{
    internal static IIncludableQueryable<ResourceType, Database.Entities.Organization> AddDependentObjects(this IQueryable<ResourceType> originalQuery) =>
        originalQuery.Include(query => query.Organization);

    internal static IQueryable<ResourceType> AddSearchCriteria(
        this IQueryable<ResourceType> query,
        ResourceTypeSearchCriteria searchCriteria)
    {
        query = query.Where(item => !item.DeletedAt.HasValue && item.Organization.Id == searchCriteria.OrganizationId);

        if (!string.IsNullOrWhiteSpace(searchCriteria.NameContains))
        {
            query = query.Where(item => EF.Functions.ILike(item.Name, $"%{searchCriteria.NameContains}%"));
        }

        return query;
    }

    internal static IQueryable<ResourceType> AddSortingOrders(
        this IQueryable<ResourceType> originalQuery,
        ICollection<ResourceTypeOrder> orderByFields)
    {
        if (orderByFields.Count == 0)
        {
            return originalQuery.OrderBy(query => query.Name).ThenBy(query => query.Id);
        }

        var orderByField = orderByFields.First();
        return orderByFields.Skip(1).Aggregate(orderByField.Field switch
        {
            OrganizationResourceTypeOrderField.Name => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.Name)
                : originalQuery.OrderByDescending(x => x.Name),
            OrganizationResourceTypeOrderField.Description => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.Description)
                : originalQuery.OrderByDescending(x => x.Description),
            _ => throw new ArgumentOutOfRangeException()
        }, (query, orderField) =>
            orderField.Field switch
            {
                OrganizationResourceTypeOrderField.Name => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.Name)
                    : query.ThenByDescending(x => x.Name),
                _ => throw new ArgumentOutOfRangeException()
            }).ThenBy(query => query.Id);
    }
}

public class ResourceTypeRepository(OrganizationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<OrganizationDbContext, ResourceType>(dbContext, timeProvider), IResourceTypeRepository
{
    public async Task<ResourceType?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.ResourceType.AddDependentObjects().FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<ICollection<ResourceType>> GetByIdsAsync(ICollection<string> ids, CancellationToken cancellationToken) =>
        await DbContext.ResourceType.Where(query => ids.Contains(query.Id)).AddDependentObjects().ToListAsync(cancellationToken);

    public ResourceType Add(ResourceType resourceType)
    {
        var now = TimeProvider.GetUtcNow();
        resourceType.CreatedAt = now;
        return DbContext.ResourceType.Add(resourceType).Entity;
    }

    public void RemoveRange(ICollection<ResourceType> resourceTypes)
    {
        var now = TimeProvider.GetUtcNow();
        resourceTypes.ForEach(resourceType => resourceType.DeletedAt = now);
        DbContext.ResourceType.UpdateRange(resourceTypes);
    }

    public ResourceType Remove(ResourceType resourceType)
    {
        var now = TimeProvider.GetUtcNow();
        resourceType.DeletedAt = now;
        return DbContext.ResourceType.Update(resourceType).Entity;
    }

    public ResourceType Update(ResourceType resourceType)
    {
        var now = TimeProvider.GetUtcNow();
        resourceType.ModifiedAt = now;
        return DbContext.ResourceType.Update(resourceType).Entity;
    }

    public async Task<(PaginatedInfo, ICollection<Edge<ResourceType>>, int)> GetPaginatedResourceTypesAsync(
        PaginationInputParam paginationInputParam,
        ResourceTypeSearchCriteria searchCriteria,
        ICollection<ResourceTypeOrder> orderByFields,
        CancellationToken cancellationToken) =>
        (await DbContext.ResourceType
            .AddSearchCriteria(searchCriteria)
            .AddSortingOrders(orderByFields)
            .AddDependentObjects()
            .ToListAsync(cancellationToken))
        .ToPaginated(paginationInputParam);
}
