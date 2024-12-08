using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Models;
using Enterprise.Shared.Pagination;
using Location.Shared.Database;
using Location.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Customer = Location.Shared.Database.Entities.Customer;
using LocationMember = Location.Shared.Database.Entities.LocationMember;

namespace Location.Shared.Repositories;

public interface ILocationMemberRepository : IRepository<LocationMember>
{
    Task<LocationMember?> GetByIdAsync(string id, CancellationToken cancellationToken);
    LocationMember Add(LocationMember locationMember);
    void AddRange(ICollection<LocationMember> locationMembers);
    LocationMember Update(LocationMember locationMember);
    void RemoveRange(ICollection<LocationMember> locationMembers);

    Task<(PaginatedInfo, ICollection<Edge<LocationMember>>, int )> GetPaginatedLocationMembersAsync(
        PaginationInputParam paginationInputParam,
        LocationMemberSearchCriteria searchCriteria,
        ICollection<LocationMemberOrder> orderByFields,
        CancellationToken cancellationToken);
}

internal static class LocationMemberExtensions
{
    internal static IIncludableQueryable<LocationMember, Customer> AddDependentObjects(
        this IQueryable<LocationMember> originalQuery) =>
        originalQuery
            .Include(query => query.Location)
            .Include(query => query.Customer);

    internal static IQueryable<LocationMember> AddSearchCriteria(
        this IQueryable<LocationMember> query,
        LocationMemberSearchCriteria searchCriteria)
    {
        query = query.Where(item => !item.DeletedAt.HasValue && item.Location.Id == searchCriteria.LocationId);

        if (!string.IsNullOrWhiteSpace(searchCriteria.NameContains))
        {
            query = query.Where(item =>
                (item.Customer.Name != null &&
                 EF.Functions.ILike(item.Customer.Name, $"%{searchCriteria.NameContains}%")) ||
                (item.Customer.GivenName != null &&
                 EF.Functions.ILike(item.Customer.GivenName, $"%{searchCriteria.NameContains}%")) ||
                (item.Customer.MiddleName != null &&
                 EF.Functions.ILike(item.Customer.MiddleName, $"%{searchCriteria.NameContains}%")) ||
                (item.Customer.FamilyName != null &&
                 EF.Functions.ILike(item.Customer.FamilyName, $"%{searchCriteria.NameContains}%")));
        }

        return query;
    }

    internal static IQueryable<LocationMember> AddSortingOrders(
        this IQueryable<LocationMember> originalQuery,
        ICollection<LocationMemberOrder> orderByFields)
    {
        if (orderByFields.Count == 0)
        {
            return originalQuery.OrderBy(query => query.Customer.Name).ThenBy(query => query.Id);
        }

        var orderByField = orderByFields.First();
        return orderByFields.Skip(1).Aggregate(orderByField.Field switch
        {
            LocationMemberOrderField.MembershipType => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.MembershipType)
                : originalQuery.OrderByDescending(x => x.MembershipType),
            LocationMemberOrderField.Name => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.Customer.Name)
                : originalQuery.OrderByDescending(x => x.Customer.Name),
            LocationMemberOrderField.GivenName => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.Customer.GivenName)
                : originalQuery.OrderByDescending(x => x.Customer.GivenName),
            LocationMemberOrderField.MiddleName => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.Customer.MiddleName)
                : originalQuery.OrderByDescending(x => x.Customer.MiddleName),
            LocationMemberOrderField.FamilyName => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.Customer.FamilyName)
                : originalQuery.OrderByDescending(x => x.Customer.FamilyName),
            _ => throw new ArgumentOutOfRangeException()
        }, (query, orderField) =>
            orderField.Field switch
            {
                LocationMemberOrderField.MembershipType => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.MembershipType)
                    : query.ThenByDescending(x => x.MembershipType),
                LocationMemberOrderField.Name => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.Customer.Name)
                    : query.ThenByDescending(x => x.Customer.Name),
                LocationMemberOrderField.GivenName => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.Customer.GivenName)
                    : query.ThenByDescending(x => x.Customer.GivenName),
                LocationMemberOrderField.MiddleName => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.Customer.MiddleName)
                    : query.ThenByDescending(x => x.Customer.MiddleName),
                LocationMemberOrderField.FamilyName => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.Customer.FamilyName)
                    : query.ThenByDescending(x => x.Customer.FamilyName),
                _ => throw new ArgumentOutOfRangeException()
            }).ThenBy(query => query.Id);
    }
}

public class LocationMemberRepository(LocationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<LocationDbContext, LocationMember>(dbContext, timeProvider), ILocationMemberRepository
{
    public async Task<LocationMember?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.LocationMember
            .Include(query => query.Location)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public LocationMember Add(LocationMember locationMember)
    {
        var now = TimeProvider.GetUtcNow();
        locationMember.CreatedAt = now;
        return DbContext.LocationMember.Add(locationMember).Entity;
    }

    public void AddRange(ICollection<LocationMember> locationMembers)
    {
        var now = TimeProvider.GetUtcNow();
        locationMembers.ForEach(locationMember => locationMember.CreatedAt = now);
        DbContext.LocationMember.AddRange(locationMembers);
    }

    public void RemoveRange(ICollection<LocationMember> locationMembers)
    {
        var now = TimeProvider.GetUtcNow();
        locationMembers.ForEach(locationMember => locationMember.DeletedAt = now);
        DbContext.LocationMember.UpdateRange(locationMembers);
    }

    public LocationMember Update(LocationMember locationMember)
    {
        var now = TimeProvider.GetUtcNow();
        locationMember.ModifiedAt = now;
        return DbContext.LocationMember.Update(locationMember).Entity;
    }

    public async Task<(PaginatedInfo, ICollection<Edge<LocationMember>>, int)> GetPaginatedLocationMembersAsync(
        PaginationInputParam paginationInputParam,
        LocationMemberSearchCriteria searchCriteria,
        ICollection<LocationMemberOrder> orderByFields,
        CancellationToken cancellationToken) =>
        (await DbContext.LocationMember
            .AddSearchCriteria(searchCriteria)
            .AddSortingOrders(orderByFields)
            .AddDependentObjects()
            .ToListAsync(cancellationToken))
        .ToPaginated(paginationInputParam);
}
