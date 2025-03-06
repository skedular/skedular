using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Time;
using Microsoft.EntityFrameworkCore;

namespace Booking.Shared.Repositories;

public interface ILocationResourceRepository : IRepository<LocationResource>
{
    Task<LocationResource> UpsertNakedAsync(
        string id,
        Location? location,
        OrganizationResourceType organizationResourceType,
        CancellationToken cancellationToken);

    Task<LocationResource?> GetByIdAsync(string id, bool includeAllRelatedEntities, CancellationToken cancellationToken);
    LocationResource Add(LocationResource locationResource);
    LocationResource Update(LocationResource locationResource);
    void RemoveRange(ICollection<LocationResource> locationResources);

    Task<ICollection<LocationResource>> GetAvailableLocationResourcesAsync(
        string? organizationId,
        string? locationId,
        DateTimeOffset date,
        ICollection<string> locationResourceIdsToInclude,
        ICollection<string> customTagIds,
        ICollection<string> zoneIds,
        bool combineCustomTagsZones,
        CancellationToken cancellationToken);

    Task<ICollection<LocationResource>> GetByLocationIdAsync(string locationId, CancellationToken cancellationToken);
}

public class LocationResourceRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, LocationResource>(dbContext, timeProvider), ILocationResourceRepository
{
    public async Task<LocationResource> UpsertNakedAsync(
        string id,
        Location? location,
        OrganizationResourceType organizationResourceType,
        CancellationToken cancellationToken)
    {
        await UpsertNakedAsync<Location, OrganizationResourceType>(id, location, organizationResourceType, cancellationToken);

        return (await GetByIdAsync(id, false, cancellationToken))!;
    }

    public LocationResource Add(LocationResource locationResource)
    {
        var now = TimeProvider.GetUtcNow();
        locationResource.CreatedAt = now;
        return DbContext.LocationResource.Add(locationResource).Entity;
    }

    public void RemoveRange(ICollection<LocationResource> locationResources)
    {
        var now = TimeProvider.GetUtcNow();
        locationResources.ForEach(locationResource => locationResource.DeletedAt = now);
        DbContext.LocationResource.UpdateRange(locationResources);
    }

    public LocationResource Update(LocationResource locationResource)
    {
        var now = TimeProvider.GetUtcNow();
        locationResource.ModifiedAt = now;
        return DbContext.LocationResource.Update(locationResource).Entity;
    }

    public async Task<LocationResource?> GetByIdAsync(string id, bool includeAllRelatedEntities, CancellationToken cancellationToken) =>
        includeAllRelatedEntities
            ? await DbContext.LocationResource
                .Include(query => query.Bookings)
                .Include(query => query.PreferredByCustomers)
                .Include(query => query.Location)
                .Include(query => query.OrganizationTags.Where(tag => !tag.DeletedAt.HasValue))
                .FirstOrDefaultAsync(query => query.Id == id, cancellationToken)
            : await DbContext.LocationResource
                .Include(query => query.Location)
                .Include(query => query.OrganizationTags.Where(tag => !tag.DeletedAt.HasValue))
                .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<ICollection<LocationResource>> GetAvailableLocationResourcesAsync(
        string? organizationId,
        string? locationId,
        DateTimeOffset date,
        ICollection<string> locationResourceIdsToInclude,
        ICollection<string> customTagIds,
        ICollection<string> zoneIds,
        bool combineCustomTagsZones,
        CancellationToken cancellationToken)
    {
        var locationResourceQuery = locationResourceIdsToInclude.Count == 0
            ? DbContext.LocationResource
                .Where(query => !query.DeletedAt.HasValue &&
                                !query.Inactive &&
                                (string.IsNullOrWhiteSpace(organizationId) || (query.Location.Organization != null &&
                                                                               query.Location.Organization.Id ==
                                                                               organizationId)) &&
                                (string.IsNullOrWhiteSpace(locationId) || query.Location.Id == locationId) &&
                                (
                                    string.IsNullOrWhiteSpace(organizationId) ||
                                    !query.Bookings.Any(booking =>
                                        !booking.DeletedAt.HasValue && booking.From >= date &&
                                        booking.To < date.Tomorrow() && booking.Location != null &&
                                        booking.Location.Organization != null &&
                                        booking.Location.Organization.Id == organizationId)
                                ) &&
                                (
                                    string.IsNullOrWhiteSpace(locationId) ||
                                    !query.Bookings.Any(booking =>
                                        !booking.DeletedAt.HasValue && booking.From >= date &&
                                        booking.To < date.Tomorrow() && booking.Location != null &&
                                        booking.Location.Id == locationId)
                                )
                )
            : DbContext.LocationResource
                .Where(query => (!query.DeletedAt.HasValue &&
                                 !query.Inactive &&
                                 (string.IsNullOrWhiteSpace(organizationId) || (query.Location.Organization != null &&
                                                                                query.Location.Organization.Id ==
                                                                                organizationId)) &&
                                 (string.IsNullOrWhiteSpace(locationId) || query.Location.Id == locationId) &&
                                 (
                                     string.IsNullOrWhiteSpace(organizationId) ||
                                     !query.Bookings.Any(booking =>
                                         !booking.DeletedAt.HasValue && booking.From >= date &&
                                         booking.To < date.Tomorrow() && booking.Location != null &&
                                         booking.Location.Organization != null &&
                                         booking.Location.Organization.Id == organizationId)
                                 ) &&
                                 (
                                     string.IsNullOrWhiteSpace(locationId) ||
                                     !query.Bookings.Any(booking =>
                                         !booking.DeletedAt.HasValue && booking.From >= date &&
                                         booking.To < date.Tomorrow() && booking.Location != null &&
                                         booking.Location.Id == locationId)
                                 )) || locationResourceIdsToInclude.Contains(query.Id)
                );

        var locationResources = await locationResourceQuery
            .Include(query => query.Location)
            .Include(query => query.OrganizationTags)
            .OrderBy(query => query.Name)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return locationResources.Where(item =>
        {
            if (locationResourceIdsToInclude.Count != 0 && locationResourceIdsToInclude.Contains(item.Id))
            {
                return true;
            }

            if (customTagIds.Count == 0 && zoneIds.Count == 0)
            {
                return true;
            }

            var organizationTagIds = item.OrganizationTags.Select(tag => tag.Id).ToList();
            var customTagMatchResult = customTagIds.All(customTagId => organizationTagIds.Any(id => id == customTagId));
            var zoneMatchResult = zoneIds.All(zoneId => organizationTagIds.Any(id => id == zoneId));

            return combineCustomTagsZones
                ? customTagMatchResult && zoneMatchResult
                : customTagMatchResult || zoneMatchResult;
        }).ToList();
    }

    public async Task<ICollection<LocationResource>> GetByLocationIdAsync(string locationId, CancellationToken cancellationToken) =>
        await DbContext.LocationResource
            .Where(query => query.Location.Id == locationId)
            .Include(query => query.OrganizationTags)
            .ToListAsync(cancellationToken);
}
