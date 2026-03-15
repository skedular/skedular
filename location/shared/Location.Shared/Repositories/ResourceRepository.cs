using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Pagination;
using HotChocolate.Types.Pagination;
using Location.Shared.Database;
using Location.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Resource = Location.Shared.Database.Entities.Resource;
using OrganizationTag = Location.Shared.Database.Entities.OrganizationTag;

namespace Location.Shared.Repositories;

public interface IResourceRepository : IRepository<Resource>
{
    Task<Resource?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<ICollection<Resource>> GetByIdsAsync(ICollection<string> ids, CancellationToken cancellationToken);
    Resource Add(Resource resource);
    Resource Update(Resource resource);
    void RemoveRange(ICollection<Resource> resources);
    Resource Remove(Resource resource);

    Task<(PaginatedInfo, ICollection<Edge<Resource>>, int)> GetPaginatedResourcesAsync(
        PaginationInputParam paginationInputParam,
        ResourceSearchCriteria searchCriteria,
        ICollection<ResourceOrder> orderByFields,
        CancellationToken cancellationToken);
}

internal static class ResourceExtensions
{
    extension(IQueryable<Resource> originalQuery)
    {
        internal IIncludableQueryable<Resource, IEnumerable<OrganizationTag>> AddDependentObjects() =>
            originalQuery
                .Include(query => query.Location)
                .Include(query => query.ResourcePosition)
                .ThenInclude(query => query!.FloorPlan)
                .Include(query => query.OrganizationTags.Where(tag => !tag.DeletedAt.HasValue));

        internal IQueryable<Resource> AddSearchCriteria(ResourceSearchCriteria searchCriteria)
        {
            originalQuery = originalQuery.Where(item => !item.DeletedAt.HasValue && item.Location.Id == searchCriteria.LocationId);

            if (!string.IsNullOrWhiteSpace(searchCriteria.NameContains))
            {
                originalQuery = originalQuery.Where(item => EF.Functions.ILike(item.Name, $"%{searchCriteria.NameContains}%"));
            }

            if (searchCriteria.TagIds.Count != 0)
            {
                originalQuery = originalQuery.Where(item => searchCriteria.TagIds.All(zoneId => item.OrganizationTags.Any(tag => tag.Id == zoneId)));
            }

            if (!string.IsNullOrWhiteSpace(searchCriteria.FloorPlanId))
            {
                originalQuery = originalQuery.Where(item =>
                    item.ResourcePosition == null ||
                    (item.ResourcePosition != null && item.ResourcePosition.FloorPlan.Id == searchCriteria.FloorPlanId));
            }

            return originalQuery;
        }
    }
}

public class ResourceRepository(LocationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<LocationDbContext, Resource>(dbContext, timeProvider), IResourceRepository
{
    public async Task<Resource?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Resource
            .AddDependentObjects()
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<ICollection<Resource>> GetByIdsAsync(ICollection<string> ids, CancellationToken cancellationToken) =>
        await DbContext.Resource
            .Where(query => ids.Contains(query.Id))
            .AddDependentObjects()
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
        await DbContext.Resource
            .AddSearchCriteria(searchCriteria)
            .AddDependentObjects()
            .ToPaginatedAsync(paginationInputParam, GetPaginationFields(orderByFields), cancellationToken);

    private static List<KeysetPaginationField<Resource>> GetPaginationFields(ICollection<ResourceOrder> orderByFields)
    {
        if (orderByFields.Count == 0)
        {
            return
            [
                KeysetPaginationField<Resource>.Create(
                    nameof(Resource.Name),
                    query => query.Name,
                    OrderDirection.Ascending)
            ];
        }

        return orderByFields.Select(orderField => orderField.Field switch
            {
                ResourceOrderField.Name => KeysetPaginationField<Resource>.Create(
                    nameof(Resource.Name),
                    query => query.Name,
                    orderField.Direction),
                _ => throw new ArgumentOutOfRangeException()
            })
            .ToList();
    }
}
