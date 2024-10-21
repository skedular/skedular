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
    Task<Database.Entities.Location> UpsertNakedAsync(string id, CancellationToken cancellationToken);
    Task<Database.Entities.Location?> GetByIdAsync(string id, CancellationToken cancellationToken);

    Task<IEnumerable<Database.Entities.Location>> GetByCustomerIdAsync(
        string customerId,
        string? organizationId,
        CancellationToken cancellationToken);

    Task<ICollection<Database.Entities.Location>> GetAllAsync(CancellationToken cancellationToken);
    Database.Entities.Location Add(Database.Entities.Location organization);
    Database.Entities.Location Update(Database.Entities.Location organization);
    Database.Entities.Location Remove(Database.Entities.Location organization);

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
            .Include(query => query.Tags.Where(tag => !tag.DeletedAt.HasValue))
            .Include(query => query.Desks.Where(desk => !desk.DeletedAt.HasValue))
            .ThenInclude(query => query.Tags.Where(tag => !tag.DeletedAt.HasValue))
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
                (item.Organization != null &&
                 (searchCriteria.CustomerId == null || item.Organization.OrganizationMembers.Any(organizationMember =>
                     !organizationMember.DeletedAt.HasValue &&
                     organizationMember.Customer.Id ==
                     searchCriteria.CustomerId))));
        }
        else
        {
            query = query.Where(item =>
                item.Organization != null && item.Organization.Id == searchCriteria.OrganizationId &&
                (searchCriteria.CustomerId == null || item.Organization.OrganizationMembers.Any(organizationMember =>
                    !organizationMember.DeletedAt.HasValue &&
                    organizationMember.Customer.Id ==
                    searchCriteria.CustomerId)));
        }

        if (!string.IsNullOrWhiteSpace(searchCriteria.NameContains))
        {
            query = query.Where(item => EF.Functions.ILike(item.Name, $"%{searchCriteria.NameContains}%"));
        }

        return query;
    }

    internal static IQueryable<Database.Entities.Location> AddSortingOrders(
        this IQueryable<Database.Entities.Location> originalQuery,
        ICollection<LocationOrder> orderByFields)
    {
        if (orderByFields.Count == 0)
        {
            return originalQuery.OrderBy(query => query.CreatedAt);
        }

        var orderByField = orderByFields.First();
        return orderByFields.Skip(1).Aggregate(orderByField.Field switch
        {
            LocationOrderField.Name => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.Name)
                : originalQuery.OrderByDescending(x => x.Name),
            _ => throw new ArgumentOutOfRangeException()
        }, (query, orderField) =>
            orderField.Field switch
            {
                LocationOrderField.Name => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.Name)
                    : query.ThenByDescending(x => x.Name),
                _ => throw new ArgumentOutOfRangeException()
            });
    }

    public static IQueryable<Database.Entities.Location> ApplyPaginationFilters(
        this IQueryable<Database.Entities.Location> query,
        PaginationInputParam paginationInputParam,
        ICollection<LocationOrder> orderByFields)
    {
        var orderByField = orderByFields.FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(paginationInputParam.After))
        {
            query = orderByField?.Field switch
            {
                LocationOrderField.Name => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.Name.CompareTo(paginationInputParam.After.FromCursor()) > 0)
                    : query.Where(item =>
                        item.Name.CompareTo(paginationInputParam.After.FromCursor()) < 0),
                null => query.Where(item =>
                    item.CreatedAt.CompareTo(paginationInputParam.After.FromCursorToDateTimeOffset()) > 0),
                _ => query.Where(item =>
                    item.CreatedAt.CompareTo(paginationInputParam.After.FromCursorToDateTimeOffset()) > 0)
            };
        }
        else if (!string.IsNullOrWhiteSpace(paginationInputParam.Before))
        {
            query = orderByField?.Field switch
            {
                LocationOrderField.Name => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.Name.CompareTo(paginationInputParam.Before.FromCursor()) < 0)
                    : query.Where(item =>
                        item.Name.CompareTo(paginationInputParam.Before.FromCursor()) > 0),
                null => query.Where(item =>
                    item.CreatedAt.CompareTo(paginationInputParam.Before.FromCursorToDateTimeOffset()) < 0),
                _ => query.Where(item =>
                    item.CreatedAt.CompareTo(paginationInputParam.Before.FromCursorToDateTimeOffset()) < 0)
            };
        }

        if (paginationInputParam.First is not null)
        {
            query = query.Take(paginationInputParam.First.Value + 1);
        }
        else if (paginationInputParam.Last is not null)
        {
            query = query.Take(paginationInputParam.Last.Value + 1);
        }

        return query;
    }

    public static ICollection<Edge<Database.Entities.Location>> ToEdges(
        this ICollection<Database.Entities.Location> items,
        ICollection<LocationOrder> orderByFields) =>
        items.Select(item => orderByFields.FirstOrDefault()?.Field switch
        {
            LocationOrderField.Name => new Edge<Database.Entities.Location>(item.Name.ToCursor(), item),
            null => new Edge<Database.Entities.Location>(item.CreatedAt.ToCursor(), item),
            _ => new Edge<Database.Entities.Location>(item.CreatedAt.ToCursor(), item)
        }).ToList();
}

public class LocationRepository(LocationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<LocationDbContext, Database.Entities.Location>(dbContext), ILocationRepository
{
    public async Task<Database.Entities.Location> UpsertNakedAsync(string id, CancellationToken cancellationToken)
    {
        var existing = await GetByIdAsync(id, cancellationToken);
        if (existing is not null)
        {
            return existing;
        }

        var now = timeProvider.GetUtcNow();
        return DbContext.Location.Add(new Database.Entities.Location { Id = id, CreatedAt = now }).Entity;
    }

    public Database.Entities.Location Add(Database.Entities.Location organization)
    {
        var now = timeProvider.GetUtcNow();
        organization.CreatedAt = now;
        return DbContext.Location.Add(organization).Entity;
    }

    public Database.Entities.Location Update(Database.Entities.Location organization)
    {
        var now = timeProvider.GetUtcNow();
        organization.ModifiedAt = now;
        return DbContext.Location.Update(organization).Entity;
    }

    public async Task<Database.Entities.Location?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Location
            .Where(query => query.Id == id)
            .AddDependentObjects()
            .OrderBy(query => query.Id)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<IEnumerable<Database.Entities.Location>> GetByCustomerIdAsync(
        string customerId,
        string? organizationId,
        CancellationToken cancellationToken)
    {
        var query = DbContext.Location
            .Where(location =>
                !location.DeletedAt.HasValue && ((location.Organization == null &&
                                                  location.LocationMembers.Any(item =>
                                                      item.Customer.Id == customerId)) ||
                                                 (location.Organization != null &&
                                                  location.Organization.OrganizationMembers.Any(organizationMember =>
                                                      organizationMember.Customer.Id == customerId))));

        if (!string.IsNullOrWhiteSpace(organizationId))
        {
            query = query.Where(location =>
                location.Organization != null && location.Organization.Id == organizationId);
        }

        return await query
            .AddDependentObjects()
            .ToListAsync(cancellationToken);
    }

    public async Task<ICollection<Database.Entities.Location>> GetAllAsync(CancellationToken cancellationToken) =>
        await DbContext.Location
            .Where(query => !query.DeletedAt.HasValue)
            .AddDependentObjects()
            .ToListAsync(cancellationToken);

    public Database.Entities.Location Remove(Database.Entities.Location organization)
    {
        var now = timeProvider.GetUtcNow();
        organization.DeletedAt = now;
        return DbContext.Location.Update(organization).Entity;
    }

    public async Task<(PaginatedInfo, ICollection<Edge<Database.Entities.Location>>, int)> GetPaginatedLocationsAsync(
        PaginationInputParam paginationInputParam,
        LocationSearchCriteria searchCriteria,
        ICollection<LocationOrder> orderByFields,
        CancellationToken cancellationToken)
    {
        var totalCount = await DbContext.Location.AsQueryable().AddSearchCriteria(searchCriteria)
            .CountAsync(cancellationToken);
        if (totalCount == 0)
        {
            return (new PaginatedInfo(false, false, null, null), [], totalCount);
        }

        var (paginatedInfo, edges) = (await DbContext.Location
                .AsQueryable()
                .AddSearchCriteria(searchCriteria)
                .AddSortingOrders(orderByFields)
                .ApplyPaginationFilters(paginationInputParam, orderByFields)
                .AddDependentObjects()
                .ToListAsync(cancellationToken))
            .ToEdges(orderByFields)
            .GetPaginatedInfo(paginationInputParam);
        return (paginatedInfo, edges, totalCount);
    }
}
