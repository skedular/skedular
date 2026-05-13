using System.Linq.Expressions;
using Api.Shared.Services.Models;
using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Enterprise.Shared.Pagination;
using Microsoft.EntityFrameworkCore;
using ResourceAvailabilityOrder = Booking.Shared.Models.ResourceAvailabilityOrder;
using ResourceAvailabilityOrderByField = Booking.Shared.Models.ResourceAvailabilityOrderByField;

namespace Booking.Shared.Repositories;

public interface IResourceRepository : IRepository<Resource>
{
    Task<Resource> UpsertNakedAsync(string id, Location? location, CancellationToken cancellationToken);
    Task<Resource?> GetByIdAsync(string id, bool includeAllRelatedEntities, CancellationToken cancellationToken);
    Task<IReadOnlyList<Resource>> GetByIdsAsync(IReadOnlyList<string> ids, bool includeAllRelatedEntities, CancellationToken cancellationToken);
    Resource Add(Resource resource);
    Resource Update(Resource resource);
    void RemoveRange(IEnumerable<Resource> resources);
    Task<IReadOnlyList<Resource>> GetByLocationIdAsync(string locationId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Resource>> GetForAvailabilityDayViewAsync(
        string organizationCustomDomain,
        IReadOnlyList<string> locationIds,
        string? zoneId,
        string? resourceType,
        IReadOnlyList<ResourceAvailabilityOrder> orderBy,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Resource>> GetAvailableResourcesAsync(
        string? organizationId,
        string? locationId,
        DateTimeOffset from,
        DateTimeOffset until,
        IReadOnlyList<string> resourceIds,
        IReadOnlyList<string> tagIds,
        IReadOnlyList<string> tagTypes,
        CancellationToken cancellationToken);

    Task<int> GetAvailableResourcesCountAsync(
        string? organizationId,
        string? locationId,
        DateTimeOffset from,
        DateTimeOffset until,
        IReadOnlyList<string> resourceIds,
        IReadOnlyList<string> tagIds,
        IReadOnlyList<string> tagTypes,
        CancellationToken cancellationToken);
}

public class ResourceRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, Resource>(dbContext, timeProvider), IResourceRepository
{
    public async Task<Resource> UpsertNakedAsync(string id, Location? location, CancellationToken cancellationToken)
    {
        await UpsertNakedAsync<Location>(id, location, cancellationToken);

        return (await GetByIdAsync(id, false, cancellationToken))!;
    }

    public async Task<IReadOnlyList<Resource>> GetByIdsAsync(
        IReadOnlyList<string> ids,
        bool includeAllRelatedEntities,
        CancellationToken cancellationToken) =>
        includeAllRelatedEntities
            ? await DbContext.Resource
                .Where(query => ids.Contains(query.Id))
                .Include(query => query.PreferredByCustomers)
                .Include(query => query.Location)
                .Include(query => query.OrganizationTags.Where(tag => !tag.DeletedAt.HasValue))
                .ToListAsync(cancellationToken)
            : await DbContext.Resource
                .Where(query => ids.Contains(query.Id))
                .Include(query => query.Location)
                .Include(query => query.OrganizationTags.Where(tag => !tag.DeletedAt.HasValue))
                .ToListAsync(cancellationToken);

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

    public Resource Update(Resource resource)
    {
        var now = TimeProvider.GetUtcNow();
        resource.ModifiedAt = now;
        return DbContext.Resource.Update(resource).Entity;
    }

    public async Task<Resource?> GetByIdAsync(string id, bool includeAllRelatedEntities, CancellationToken cancellationToken) =>
        includeAllRelatedEntities
            ? await DbContext.Resource
                .Include(query => query.PreferredByCustomers)
                .Include(query => query.Location)
                .Include(query => query.OrganizationTags.Where(tag => !tag.DeletedAt.HasValue))
                .FirstOrDefaultAsync(query => query.Id == id, cancellationToken)
            : await DbContext.Resource
                .Include(query => query.Location)
                .Include(query => query.OrganizationTags.Where(tag => !tag.DeletedAt.HasValue))
                .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Resource>> GetByLocationIdAsync(string locationId, CancellationToken cancellationToken) =>
        await DbContext.Resource
            .Where(query => query.Location != null && query.Location.Id == locationId)
            .Include(query => query.OrganizationTags)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Resource>> GetForAvailabilityDayViewAsync(
        string organizationCustomDomain,
        IReadOnlyList<string> locationIds,
        string? zoneId,
        string? resourceType,
        IReadOnlyList<ResourceAvailabilityOrder> orderBy,
        CancellationToken cancellationToken)
    {
        // Organization is a mandatory tenancy boundary — it is always applied as a hard
        // WHERE clause, so a query can never accidentally return resources from another
        // organization, even if the caller passes an empty string (which matches nothing).
        var query = DbContext.Resource
            .Where(item => !item.DeletedAt.HasValue &&
                           item.Location != null &&
                           item.Location.Organization != null &&
                           item.Location.Organization.CustomDomain == organizationCustomDomain)
            .Include(item => item.Location)
            .ThenInclude(item => item!.Organization)
            .Include(item => item.OrganizationTags.Where(organizationTag => !organizationTag.DeletedAt.HasValue))
            .AsQueryable();

        if (locationIds.Count > 0)
        {
            query = query.Where(item => item.Location != null && locationIds.Contains(item.Location.Id));
        }

        if (zoneId is not null)
        {
            query = query
                .Where(item => item.OrganizationTags.Any(organizationTag =>
                    !organizationTag.DeletedAt.HasValue &&
                    organizationTag.Type == OrganizationTagTypeConstants.Zone &&
                    organizationTag.Id == zoneId));
        }

        if (resourceType is not null)
        {
            query = query
                .Where(item => item.OrganizationTags.Any(organizationTag =>
                    !organizationTag.DeletedAt.HasValue && organizationTag.Type == resourceType));
        }

        return await ApplyOrderBy(query, orderBy).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Resource>> GetAvailableResourcesAsync(
        string? organizationId,
        string? locationId,
        DateTimeOffset from,
        DateTimeOffset until,
        IReadOnlyList<string> resourceIds,
        IReadOnlyList<string> tagIds,
        IReadOnlyList<string> tagTypes,
        CancellationToken cancellationToken)
    {
        var availableResourceIds =
            await GetAvailableResourceIdsAsync(organizationId, locationId, from, until, resourceIds, tagIds, tagTypes, cancellationToken);

        var resources = await DbContext.Resource
            .Where(query => availableResourceIds.Contains(query.Id))
            .Include(query => query.ResourceBookingSlots.Where(slot => slot.Start >= from && slot.Start < until).OrderBy(slot => slot.Start))
            .ThenInclude(query => query.Bookings)
            .Include(query => query.ResourceBookingSlots.Where(slot => slot.Start >= from && slot.Start < until).OrderBy(slot => slot.Start))
            .ThenInclude(query => query.Customers)
            .Include(query => query.OrganizationTags.Where(tag => !tag.DeletedAt.HasValue))
            .Include(query => query.Location)
            .OrderBy(query => query.Id)
            .ToListAsync(cancellationToken);

        return resources;
    }

    public async Task<int> GetAvailableResourcesCountAsync(
        string? organizationId,
        string? locationId,
        DateTimeOffset from,
        DateTimeOffset until,
        IReadOnlyList<string> resourceIds,
        IReadOnlyList<string> tagIds,
        IReadOnlyList<string> tagTypes,
        CancellationToken cancellationToken) =>
        (await GetAvailableResourceIdsAsync(organizationId, locationId, from, until, resourceIds, tagIds, tagTypes, cancellationToken)).Count;

    // Translates ResourceAvailabilityOrder clauses into EF IQueryable ORDER BY expressions so the
    // database engine handles sorting rather than the application layer.
    // Status and FloorName have no DB columns in v1 and fall back to ResourceName.
    private static IQueryable<Resource> ApplyOrderBy(IQueryable<Resource> query, IReadOnlyList<ResourceAvailabilityOrder> orderBy)
    {
        if (orderBy.Count == 0)
        {
            return query.OrderBy(item => item.Name);
        }

        var clauses = orderBy.Select(item => (Key: GetKeySelector(item.Field), item.Direction)).ToList();
        var ordered = clauses[0].Direction == OrderDirection.Ascending ? query.OrderBy(clauses[0].Key) : query.OrderByDescending(clauses[0].Key);

        return clauses
            .Skip(1)
            .Aggregate(ordered, (q, c) => c.Direction == OrderDirection.Ascending ? q.ThenBy(c.Key) : q.ThenByDescending(c.Key));
    }

    // Maps each ResourceAvailabilityOrderByField to its SQL-translatable key selector.
    // FloorName and Status are not DB columns in v1 — both fall back to ResourceName.
    private static Expression<Func<Resource, object?>> GetKeySelector(ResourceAvailabilityOrderByField field) =>
        field switch
        {
            ResourceAvailabilityOrderByField.ResourceName => item => item.Name,
            ResourceAvailabilityOrderByField.LocationName => item => item.Location!.Name,
            ResourceAvailabilityOrderByField.ResourceType =>
                item => item.OrganizationTags
                    .Where(organizationTag =>
                        organizationTag.Type != null && OrganizationTagTypeConstants.ResourceTagTypes.Contains(organizationTag.Type))
                    .Select(organizationTag => organizationTag.Type)
                    .FirstOrDefault(),
            ResourceAvailabilityOrderByField.ZoneName =>
                item => item.OrganizationTags
                    .Where(organizationTag => organizationTag.Type == OrganizationTagTypeConstants.Zone)
                    .Select(organizationTag => organizationTag.Name)
                    .FirstOrDefault(),
            _ => throw new ArgumentOutOfRangeException(nameof(field), field, null)
        };

    private async Task<IReadOnlyList<string>> GetAvailableResourceIdsAsync(
        string? organizationId,
        string? locationId,
        DateTimeOffset from,
        DateTimeOffset until,
        IReadOnlyList<string> resourceIds,
        IReadOnlyList<string> tagIds,
        IReadOnlyList<string> tagTypes,
        CancellationToken cancellationToken)
    {
        var slots = await DbContext.ResourceBookingSlot
            .Where(query => !query.Resource.DeletedAt.HasValue &&
                            !query.Resource.Inactive &&
                            (resourceIds.Count == 0 || resourceIds.Contains(query.Resource.Id)) &&
                            query.Start >= from && query.Start < until &&
                            (string.IsNullOrWhiteSpace(organizationId) || (query.Resource.Location != null &&
                                                                           !query.Resource.Location.DeletedAt.HasValue &&
                                                                           query.Resource.Location.Organization != null &&
                                                                           !query.Resource.Location.Organization.DeletedAt.HasValue &&
                                                                           query.Resource.Location.Organization.Id == organizationId)) &&
                            (string.IsNullOrWhiteSpace(locationId) ||
                             (query.Resource.Location != null &&
                              !query.Resource.Location.DeletedAt.HasValue &&
                              query.Resource.Location.Organization != null &&
                              !query.Resource.Location.Organization.DeletedAt.HasValue &&
                              query.Resource.Location.Id == locationId)) &&
                            (tagIds.Count == 0 || query.Resource.OrganizationTags.Any(tag => !tag.DeletedAt.HasValue && tagIds.Contains(tag.Id))) &&
                            (tagTypes.Count == 0 || query.Resource.OrganizationTags.Any(tag =>
                                !tag.DeletedAt.HasValue && !string.IsNullOrWhiteSpace(tag.Type) && tagTypes.Contains(tag.Type))))
            .Include(query => query.Bookings)
            .Include(query => query.Resource)
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync(cancellationToken);

        return slots
            .GroupBy(slot => slot.Resource.Id)
            .Select(group => new { group.First().Resource, Slots = group.ToList() })
            .Where(grouped => grouped.Slots.All(slot => slot is { Available: true, Bookings.Count: 0 }))
            .GroupBy(slot => slot.Resource.Id)
            .Select(item => item.Key)
            .ToList();
    }
}
