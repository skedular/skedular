using Api.Shared.Services.Models;
using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Enterprise.Shared.Pagination;
using HotChocolate.Types.Pagination;
using Location.Shared.Database;
using Location.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using Product = Location.Shared.Database.Entities.Product;

namespace Location.Shared.Repositories;

public interface ILocationRepository : IRepository<Database.Entities.Location>
{
    Task<Database.Entities.Location?> GetByIdUntrackedAsync(string id, CancellationToken cancellationToken);
    Task<Database.Entities.Location?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Database.Entities.Location>> GetByIdsAsync(IReadOnlyList<string> ids, CancellationToken cancellationToken);

    Task<IReadOnlyList<Database.Entities.Location>> GetByCustomerIdUntrackedAsync(
        string customerId,
        string? organizationId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Database.Entities.Location>> GetByOrganizationIdAsync(string organizationId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Database.Entities.Location>> GetAllUntrackedAsync(bool includeDeletedResources, CancellationToken cancellationToken);
    Database.Entities.Location Add(Database.Entities.Location location);
    Database.Entities.Location Update(Database.Entities.Location location);
    Database.Entities.Location Remove(Database.Entities.Location location);
    Task<Database.Entities.Location?> GetByUniqueClaimCodeAsync(string uniqueClaimCode, CancellationToken cancellationToken);

    Task<(PaginatedInfo, IReadOnlyList<Edge<Database.Entities.Location>>, int)> GetPaginatedLocationsUntrackedAsync(
        PaginationInputParam paginationInputParam,
        LocationSearchCriteria searchCriteria,
        IReadOnlyList<LocationOrder> orderByFields,
        CancellationToken cancellationToken);
}

public static class LocationExtensions
{
    extension(IQueryable<Database.Entities.Location> originalQuery)
    {
        public IIncludableQueryable<Database.Entities.Location, Product> AddDependentObjects(bool isTracked, bool includeDeletedResources) =>
            (isTracked ? originalQuery.AsTracking() : originalQuery.AsNoTrackingWithIdentityResolution())
            .Include(query => query.Organization)
            .Include(query => query.PhysicalAddress)
            .Include(query => query.Resources.Where(resource => includeDeletedResources || !resource.DeletedAt.HasValue))
            .ThenInclude(query => query.OrganizationTags.Where(tag => !tag.DeletedAt.HasValue))
            .Include(query => query.FloorPlans.Where(fp => !fp.DeletedAt.HasValue))
            .ThenInclude(query =>
                query.ResourcePositions.Where(resourcePartition => includeDeletedResources || !resourcePartition.Resource.DeletedAt.HasValue))
            .ThenInclude(query => query.Resource)
            .Include(query => query.OrganizationTags.Where(tag => !tag.DeletedAt.HasValue))
            .Include(query => query.RestrictedInformation)
            // Host locations use an inactive system-managed draft Product while pricing is being configured.
            // Keep that draft in the Location projection so host pricing editors can load and update it;
            // publication/customer-facing queries continue to apply their own active-product rules.
            .Include(query => query.PrecomputedLocationProducts.Where(precomputedLocationProduct =>
                !precomputedLocationProduct.Product.DeletedAt.HasValue))
            .ThenInclude(query => query.Product);

        public IQueryable<Database.Entities.Location> AddSearchCriteria(LocationSearchCriteria searchCriteria)
        {
            originalQuery = originalQuery.Where(item => !item.DeletedAt.HasValue && !item.Organization.DeletedAt.HasValue);

            if (string.IsNullOrWhiteSpace(searchCriteria.OrganizationId) &&
                string.IsNullOrWhiteSpace(searchCriteria.OrganizationCustomDomain))
            {
                originalQuery = originalQuery.Where(item => !item.Organization.DeletedAt.HasValue &&
                                                            (searchCriteria.CustomerId == null ||
                                                             item.Organization.OrganizationMembers.Any(organizationMember =>
                                                                 !organizationMember.DeletedAt.HasValue &&
                                                                 organizationMember.Customer.Id == searchCriteria.CustomerId)));
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(searchCriteria.OrganizationId))
                {
                    originalQuery = originalQuery.Where(item => !item.Organization.DeletedAt.HasValue &&
                                                                item.Organization.Id == searchCriteria.OrganizationId &&
                                                                (searchCriteria.CustomerId == null ||
                                                                 item.Organization.OrganizationMembers.Any(organizationMember =>
                                                                     !organizationMember.DeletedAt.HasValue &&
                                                                     organizationMember.Customer.Id == searchCriteria.CustomerId)));
                }

                if (!string.IsNullOrWhiteSpace(searchCriteria.OrganizationCustomDomain))
                {
                    originalQuery = originalQuery.Where(item => !item.Organization.DeletedAt.HasValue &&
                                                                item.Organization.CustomDomain != null &&
                                                                item.Organization.CustomDomain ==
                                                                searchCriteria.OrganizationCustomDomain &&
                                                                (searchCriteria.CustomerId == null ||
                                                                 item.Organization.OrganizationMembers.Any(organizationMember =>
                                                                     !organizationMember.DeletedAt.HasValue &&
                                                                     organizationMember.Customer.Id == searchCriteria.CustomerId)));
                }
            }

            if (searchCriteria.LocationIds.Count > 0)
            {
                originalQuery = originalQuery.Where(item => searchCriteria.LocationIds.Contains(item.Id));
            }

            if (!string.IsNullOrWhiteSpace(searchCriteria.NameContains))
            {
                originalQuery = originalQuery.Where(item => EF.Functions.ILike(item.Name, $"%{searchCriteria.NameContains}%"));
            }

            if (searchCriteria.TagIds.Count != 0)
            {
                searchCriteria.TagIds.ForEach(id =>
                    originalQuery = originalQuery.Where(item =>
                        item.Resources.Any(resource =>
                            !resource.DeletedAt.HasValue && resource.OrganizationTags.Select(tag => tag.Id).Contains(id))));
            }

            if (searchCriteria.Types.Count > 0)
            {
                var types = searchCriteria.Types.Select(item => item.ToLocationType()).ToList();
                originalQuery = originalQuery.Where(item => types.Contains(item.Type));
            }

            if (searchCriteria.SearchBoundaries is not null)
            {
                var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(4326);
                var envelopePolygon = geometryFactory.CreatePolygon([
                    new Coordinate(
                        searchCriteria.SearchBoundaries.SouthWest.Longitude,
                        searchCriteria.SearchBoundaries.SouthWest.Latitude), // SouthWest (bottom-left)
                    new Coordinate(
                        searchCriteria.SearchBoundaries.SouthWest.Longitude,
                        searchCriteria.SearchBoundaries.NorthEast.Latitude), // NorthWest (top-left)
                    new Coordinate(
                        searchCriteria.SearchBoundaries.NorthEast.Longitude,
                        searchCriteria.SearchBoundaries.NorthEast.Latitude), // NorthEast (top-right)
                    new Coordinate(
                        searchCriteria.SearchBoundaries.NorthEast.Longitude,
                        searchCriteria.SearchBoundaries.SouthWest.Latitude), // SouthEast (bottom-right)
                    new Coordinate(
                        searchCriteria.SearchBoundaries.SouthWest.Longitude,
                        searchCriteria.SearchBoundaries.SouthWest.Latitude), // Close polygon back to SouthWest
                ]);

                originalQuery = originalQuery.Where(item =>
                    item.PhysicalAddress != null && item.PhysicalAddress.Coordinates != null &&
                    envelopePolygon.Contains(item.PhysicalAddress.Coordinates));
            }

            if (searchCriteria.NotContactedYet is not null && searchCriteria.NotContactedYet.Value)
            {
                originalQuery = originalQuery.Where(item =>
                    !item.ContactedViaEmail && !item.ContactedViaCall && !item.ContactedViaSms && !item.ContactedViaWhatsapp);
            }

            if (searchCriteria.ResourceType is not null)
            {
                var resourceType = searchCriteria.ResourceType.Value.ToOrganizationTagType();

                originalQuery = originalQuery.Where(item => item.PrecomputedLocationProducts.Any(precomputedLocationProduct =>
                    precomputedLocationProduct.OrganizationTags.Any(organizationTag => organizationTag.Type == resourceType)));
            }

            if (searchCriteria.FilterThoseWithUnverifiedOrganization is not null && searchCriteria.FilterThoseWithUnverifiedOrganization.Value)
            {
                originalQuery = originalQuery.Where(item =>
                    (item.Organization.Type == OrganizationTypeConstants.Marketplace ||
                     item.Organization.Type == OrganizationTypeConstants.Host) &&
                    item.Organization.IsOwnershipVerified.HasValue && item.Organization.IsOwnershipVerified.Value);
            }

            if (searchCriteria.ProductIds.Count > 0)
            {
                originalQuery = originalQuery.Where(item =>
                    item.PrecomputedLocationProducts.Any(precomputedLocationProduct =>
                        searchCriteria.ProductIds.Contains(precomputedLocationProduct.Product.Id)));
            }

            return originalQuery;
        }
    }
}

public class LocationRepository(LocationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<LocationDbContext, Database.Entities.Location>(dbContext, timeProvider), ILocationRepository
{
    public async Task<Database.Entities.Location?> GetByIdUntrackedAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Location
            .AddDependentObjects(false, false)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<Database.Entities.Location?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Location
            .AddDependentObjects(true, false)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Database.Entities.Location>> GetByIdsAsync(IReadOnlyList<string> ids, CancellationToken cancellationToken) =>
        await DbContext.Location
            .Where(query => ids.Contains(query.Id))
            .AddDependentObjects(true, false)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Database.Entities.Location>> GetByCustomerIdUntrackedAsync(
        string customerId,
        string? organizationId,
        CancellationToken cancellationToken)
    {
        var query = DbContext.Location
            .Where(location => !location.DeletedAt.HasValue && !location.Organization.DeletedAt.HasValue &&
                               location.Organization.OrganizationMembers.Any(organizationMember =>
                                   organizationMember.Customer.Id == customerId));

        if (!string.IsNullOrWhiteSpace(organizationId))
        {
            query = query.Where(team => !team.Organization.DeletedAt.HasValue && team.Organization.Id == organizationId);
        }

        return await query
            .AddDependentObjects(false, false)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Database.Entities.Location>> GetByOrganizationIdAsync(
        string organizationId,
        CancellationToken cancellationToken) =>
        await DbContext.Location
            .Where(query => !query.DeletedAt.HasValue && query.Organization.Id == organizationId)
            .AddDependentObjects(true, false)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Database.Entities.Location>> GetAllUntrackedAsync(
        bool includeDeletedResources,
        CancellationToken cancellationToken) =>
        await DbContext.Location
            .Where(query => !query.DeletedAt.HasValue && !query.Organization.DeletedAt.HasValue)
            .AddDependentObjects(false, includeDeletedResources)
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

    public Database.Entities.Location Remove(Database.Entities.Location location)
    {
        var now = TimeProvider.GetUtcNow();
        location.DeletedAt = now;
        return DbContext.Location.Update(location).Entity;
    }

    public async Task<Database.Entities.Location?> GetByUniqueClaimCodeAsync(string uniqueClaimCode, CancellationToken cancellationToken) =>
        await DbContext.Location
            .AddDependentObjects(true, false)
            .FirstOrDefaultAsync(query => query.UniqueClaimCode == uniqueClaimCode, cancellationToken);

    public async Task<(PaginatedInfo, IReadOnlyList<Edge<Database.Entities.Location>>, int)> GetPaginatedLocationsUntrackedAsync(
        PaginationInputParam paginationInputParam,
        LocationSearchCriteria searchCriteria,
        IReadOnlyList<LocationOrder> orderByFields,
        CancellationToken cancellationToken) =>
        await DbContext.Location
            .AddSearchCriteria(searchCriteria)
            .AddDependentObjects(false, false)
            .ToPaginatedAsync(paginationInputParam, GetPaginationFields(orderByFields), cancellationToken);

    private static List<KeysetPaginationField<Database.Entities.Location>> GetPaginationFields(IReadOnlyList<LocationOrder> orderByFields)
    {
        if (!orderByFields.Any())
        {
            return
            [
                KeysetPaginationField<Database.Entities.Location>.Create(
                    nameof(Database.Entities.Location.Name),
                    query => query.Name,
                    OrderDirection.Ascending),
            ];
        }

        return
        [
            .. orderByFields.Select(orderField => orderField.Field switch
            {
                LocationOrderField.Name => KeysetPaginationField<Database.Entities.Location>.Create(
                    nameof(Database.Entities.Location.Name),
                    query => query.Name,
                    orderField.Direction),
                LocationOrderField.Timezone => KeysetPaginationField<Database.Entities.Location>.Create(
                    nameof(Database.Entities.Location.Timezone),
                    query => query.Timezone,
                    orderField.Direction),
                LocationOrderField.Type => KeysetPaginationField<Database.Entities.Location>.Create(
                    nameof(Database.Entities.Location.Type),
                    query => query.Type,
                    orderField.Direction),
                _ => throw new ArgumentOutOfRangeException(null,
                    "Unexpected value encountered. Update enum mapping or caller input to include this case."),
            }),
        ];
    }
}
