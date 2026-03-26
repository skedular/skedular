using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.Postgres;
using Microsoft.EntityFrameworkCore;

namespace Booking.Shared.Repositories;

public interface IResourceRepository : IRepository<Resource>
{
    Task<Resource> UpsertNakedAsync(string id, Location? location, CancellationToken cancellationToken);
    Task<Resource?> GetByIdAsync(string id, bool includeAllRelatedEntities, CancellationToken cancellationToken);
    Task<ICollection<Resource>> GetByIdsAsync(ICollection<string> ids, bool includeAllRelatedEntities, CancellationToken cancellationToken);
    Resource Add(Resource resource);
    Resource Update(Resource resource);
    void RemoveRange(ICollection<Resource> resources);
    Task<ICollection<Resource>> GetByLocationIdAsync(string locationId, CancellationToken cancellationToken);

    Task<ICollection<Resource>> GetAvailableResourcesAsync(
        string? organizationId,
        string? locationId,
        DateTimeOffset from,
        DateTimeOffset until,
        ICollection<string> resourceIds,
        ICollection<string> tagIds,
        ICollection<string> tagTypes,
        CancellationToken cancellationToken);

    Task<int> GetAvailableResourcesCountAsync(
        string? organizationId,
        string? locationId,
        DateTimeOffset from,
        DateTimeOffset until,
        ICollection<string> resourceIds,
        ICollection<string> tagIds,
        ICollection<string> tagTypes,
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

    public async Task<ICollection<Resource>> GetByIdsAsync(
        ICollection<string> ids,
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

    public void RemoveRange(ICollection<Resource> resources)
    {
        var now = TimeProvider.GetUtcNow();
        resources.ForEach(resource => resource.DeletedAt = now);
        DbContext.Resource.UpdateRange(resources);
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

    public async Task<ICollection<Resource>> GetByLocationIdAsync(string locationId, CancellationToken cancellationToken) =>
        await DbContext.Resource
            .Where(query => query.Location != null && query.Location.Id == locationId)
            .Include(query => query.OrganizationTags)
            .ToListAsync(cancellationToken);

    public async Task<ICollection<Resource>> GetAvailableResourcesAsync(
        string? organizationId,
        string? locationId,
        DateTimeOffset from,
        DateTimeOffset until,
        ICollection<string> resourceIds,
        ICollection<string> tagIds,
        ICollection<string> tagTypes,
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
        ICollection<string> resourceIds,
        ICollection<string> tagIds,
        ICollection<string> tagTypes,
        CancellationToken cancellationToken) =>
        (await GetAvailableResourceIdsAsync(organizationId, locationId, from, until, resourceIds, tagIds, tagTypes, cancellationToken)).Count;

    private async Task<ICollection<string>> GetAvailableResourceIdsAsync(
        string? organizationId,
        string? locationId,
        DateTimeOffset from,
        DateTimeOffset until,
        ICollection<string> resourceIds,
        ICollection<string> tagIds,
        ICollection<string> tagTypes,
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
