using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
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
    Task<IReadOnlyList<Resource>> GetByIdsAsync(IReadOnlyList<string> ids, CancellationToken cancellationToken);
    Task<IReadOnlyList<Resource>> GetByIdsWithOrganizationTagsUntrackedAsync(IReadOnlyList<string> ids, CancellationToken cancellationToken);
    Task<bool> ExistsActiveWithNameAsync(string locationId, string name, string? excludeId, CancellationToken cancellationToken);
    Resource Add(Resource resource);
    Resource Update(Resource resource);
    void RemoveRange(IEnumerable<Resource> resources);
    Resource Remove(Resource resource);

    Task<(PaginatedInfo, IReadOnlyList<Edge<Resource>>, int)> GetPaginatedResourcesAsync(
        PaginationInputParam paginationInputParam,
        ResourceSearchCriteria searchCriteria,
        IReadOnlyList<ResourceOrder> orderByFields,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Returns the names of all active (non-deleted) resources for the given location.
    ///     Used by the bulk import service to pre-load existing names for conflict-free name generation.
    /// </summary>
    Task<IReadOnlyList<string>> GetActiveNamesByLocationIdAsync(
        string locationId,
        CancellationToken cancellationToken);
}

public static class ResourceExtensions
{
    extension(IQueryable<Resource> originalQuery)
    {
        public IIncludableQueryable<Resource, IEnumerable<OrganizationTag>> AddDependentObjects(bool isTracked) =>
            (isTracked ? originalQuery.AsTracking() : originalQuery.AsNoTrackingWithIdentityResolution())
            .Include(query => query.Location)
            .Include(query => query.ResourcePosition)
            .ThenInclude(query => query!.FloorPlan)
            .Include(query => query.OrganizationTags.Where(tag => !tag.DeletedAt.HasValue));

        public IQueryable<Resource> AddSearchCriteria(ResourceSearchCriteria searchCriteria)
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
    private const string LikeEscapeCharacter = "\\";

    public async Task<Resource?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Resource
            .AddDependentObjects(true)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Resource>> GetByIdsAsync(IReadOnlyList<string> ids, CancellationToken cancellationToken) =>
        await DbContext.Resource
            .Where(query => ids.Contains(query.Id))
            .AddDependentObjects(true)
            .ToListAsync(cancellationToken);

    /// <summary>
    ///     Returns the requested resources with only their active organization tags loaded and without change tracking.
    /// </summary>
    /// <param name="ids">The resource identifiers to resolve.</param>
    /// <param name="cancellationToken">The cancellation token for the database query.</param>
    /// <returns>The matching resources with active organization tags populated.</returns>
    /// <remarks>
    ///     This focused read was added for derived-state rebuilding so that workflow only pays for organization tags instead of the full resource include
    ///     graph.
    /// </remarks>
    public async Task<IReadOnlyList<Resource>> GetByIdsWithOrganizationTagsUntrackedAsync(
        IReadOnlyList<string> ids,
        CancellationToken cancellationToken) =>
        await DbContext.Resource
            .Where(query => ids.Contains(query.Id))
            .AsNoTrackingWithIdentityResolution()
            .Include(query => query.OrganizationTags.Where(tag => !tag.DeletedAt.HasValue))
            .ToListAsync(cancellationToken);

    /// <summary>
    ///     Checks whether an active resource already exists with the supplied name for a location.
    /// </summary>
    /// <param name="locationId">The location identifier that owns the resource name namespace.</param>
    /// <param name="name">The candidate resource name to validate.</param>
    /// <param name="excludeId">An optional resource identifier to exclude from the duplicate check, typically the resource currently being updated.</param>
    /// <param name="cancellationToken">The cancellation token for the database query.</param>
    /// <returns><see langword="true" /> when another active resource with the same effective name exists; otherwise <see langword="false" />.</returns>
    /// <remarks>
    ///     This exact-name check stays in the repository so resource create and update flows can reuse the same duplicate rule without reviving the old
    ///     specification abstraction.
    /// </remarks>
    public async Task<bool> ExistsActiveWithNameAsync(
        string locationId,
        string name,
        string? excludeId,
        CancellationToken cancellationToken) =>
        await DbContext.Resource.AnyAsync(
            query =>
                !query.DeletedAt.HasValue &&
                query.Location.Id == locationId &&
                EF.Functions.ILike(query.Name, EscapeLikePattern(name), LikeEscapeCharacter) &&
                (excludeId == null || query.Id != excludeId),
            cancellationToken);

    public Resource Add(Resource resource)
    {
        var now = TimeProvider.GetUtcNow();
        resource.CreatedAt = now;
        return DbContext.Resource.Add(resource).Entity;
    }

    public void RemoveRange(IEnumerable<Resource> resources)
    {
        var now = TimeProvider.GetUtcNow();
        DbContext.Resource.UpdateRange(resources.Select(item =>
        {
            item.DeletedAt = now;
            return item;
        }));
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

    public async Task<(PaginatedInfo, IReadOnlyList<Edge<Resource>>, int)> GetPaginatedResourcesAsync(
        PaginationInputParam paginationInputParam,
        ResourceSearchCriteria searchCriteria,
        IReadOnlyList<ResourceOrder> orderByFields,
        CancellationToken cancellationToken) =>
        await DbContext.Resource
            .AddSearchCriteria(searchCriteria)
            .AddDependentObjects(false)
            .ToPaginatedAsync(paginationInputParam, GetPaginationFields(orderByFields), cancellationToken);

    public async Task<IReadOnlyList<string>> GetActiveNamesByLocationIdAsync(string locationId, CancellationToken cancellationToken) =>
        await DbContext.Resource
            .AsNoTracking()
            .Where(r => !r.DeletedAt.HasValue && r.Location.Id == locationId)
            .Select(r => r.Name)
            .ToListAsync(cancellationToken);

    private static List<KeysetPaginationField<Resource>> GetPaginationFields(IReadOnlyList<ResourceOrder> orderByFields)
    {
        if (!orderByFields.Any())
        {
            return
            [
                KeysetPaginationField<Resource>.Create(
                    nameof(Resource.Name),
                    query => query.Name,
                    OrderDirection.Ascending),
            ];
        }

        return orderByFields.Select(orderField => orderField.Field switch
            {
                ResourceOrderField.Name => KeysetPaginationField<Resource>.Create(
                    nameof(Resource.Name),
                    query => query.Name,
                    orderField.Direction),
                _ => throw new ArgumentOutOfRangeException(null,
                    "Unexpected value encountered. Update enum mapping or caller input to include this case."),
            })
            .ToList();
    }

    /// <summary>
    ///     Escapes SQL LIKE wildcard characters in a user-supplied name so an exact-name comparison can safely use <c>ILIKE</c>.
    /// </summary>
    /// <param name="value">The raw user-supplied value that may contain wildcard characters.</param>
    /// <returns>The escaped value safe to pass into an exact-match <c>ILIKE</c> predicate.</returns>
    private static string EscapeLikePattern(string value) =>
        value
            .Replace(LikeEscapeCharacter, LikeEscapeCharacter + LikeEscapeCharacter, StringComparison.Ordinal)
            .Replace("%", LikeEscapeCharacter + "%", StringComparison.Ordinal)
            .Replace("_", LikeEscapeCharacter + "_", StringComparison.Ordinal);
}
