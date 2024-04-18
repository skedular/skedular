using Api.Shared.Models;
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
            return originalQuery.OrderBy(query => query.CreatedAt);
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
            });
    }

    public static IQueryable<LocationMember> ApplyPaginationFilters(
        this IQueryable<LocationMember> query,
        PaginationInputParam paginationInputParam,
        ICollection<LocationMemberOrder> orderByFields)
    {
        var orderByField = orderByFields.FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(paginationInputParam.After))
        {
            query = orderByField?.Field switch
            {
                LocationMemberOrderField.MembershipType => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.MembershipType.CompareTo(paginationInputParam.After
                            .FromCursorToEnum<LocationMembershipType>()) > 0)
                    : query.Where(item =>
                        item.MembershipType.CompareTo(paginationInputParam.After
                            .FromCursorToEnum<LocationMembershipType>()) < 0),
                LocationMemberOrderField.Name => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.Customer.Name != null &&
                        item.Customer.Name.CompareTo(paginationInputParam.After.FromCursor()) > 0)
                    : query.Where(item =>
                        item.Customer.Name != null &&
                        item.Customer.Name.CompareTo(paginationInputParam.After.FromCursor()) < 0),
                LocationMemberOrderField.GivenName => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.Customer.GivenName != null &&
                        item.Customer.GivenName.CompareTo(paginationInputParam.After.FromCursor()) > 0)
                    : query.Where(item =>
                        item.Customer.GivenName != null &&
                        item.Customer.GivenName.CompareTo(paginationInputParam.After.FromCursor()) < 0),
                LocationMemberOrderField.MiddleName => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.Customer.MiddleName != null &&
                        item.Customer.MiddleName.CompareTo(paginationInputParam.After.FromCursor()) > 0)
                    : query.Where(item =>
                        item.Customer.MiddleName != null &&
                        item.Customer.MiddleName.CompareTo(paginationInputParam.After.FromCursor()) < 0),
                LocationMemberOrderField.FamilyName => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.Customer.FamilyName != null &&
                        item.Customer.FamilyName.CompareTo(paginationInputParam.After.FromCursor()) > 0)
                    : query.Where(item =>
                        item.Customer.FamilyName != null &&
                        item.Customer.FamilyName.CompareTo(paginationInputParam.After.FromCursor()) < 0),
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
                LocationMemberOrderField.MembershipType => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.MembershipType.CompareTo(
                            paginationInputParam.Before.FromCursorToEnum<LocationMembershipType>()) < 0)
                    : query.Where(item =>
                        item.MembershipType.CompareTo(
                            paginationInputParam.Before.FromCursorToEnum<LocationMembershipType>()) > 0),
                LocationMemberOrderField.Name => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.Customer.Name != null &&
                        item.Customer.Name.CompareTo(paginationInputParam.Before.FromCursor()) < 0)
                    : query.Where(item =>
                        item.Customer.Name != null &&
                        item.Customer.Name.CompareTo(paginationInputParam.Before.FromCursor()) > 0),
                LocationMemberOrderField.GivenName => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.Customer.GivenName != null &&
                        item.Customer.GivenName.CompareTo(paginationInputParam.Before.FromCursor()) < 0)
                    : query.Where(item =>
                        item.Customer.GivenName != null &&
                        item.Customer.GivenName.CompareTo(paginationInputParam.Before.FromCursor()) > 0),
                LocationMemberOrderField.MiddleName => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.Customer.MiddleName != null &&
                        item.Customer.MiddleName.CompareTo(paginationInputParam.Before.FromCursor()) < 0)
                    : query.Where(item =>
                        item.Customer.MiddleName != null &&
                        item.Customer.MiddleName.CompareTo(paginationInputParam.Before.FromCursor()) > 0),
                LocationMemberOrderField.FamilyName => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.Customer.FamilyName != null &&
                        item.Customer.FamilyName.CompareTo(paginationInputParam.Before.FromCursor()) < 0)
                    : query.Where(item =>
                        item.Customer.FamilyName != null &&
                        item.Customer.FamilyName.CompareTo(paginationInputParam.Before.FromCursor()) > 0),
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

    public static ICollection<Edge<LocationMember>> ToEdges(
        this ICollection<LocationMember> items,
        ICollection<LocationMemberOrder> orderByFields) =>
        items.Select(item => orderByFields.FirstOrDefault()?.Field switch
        {
            LocationMemberOrderField.MembershipType => new Edge<LocationMember>(item.MembershipType.ToCursor(), item),
            LocationMemberOrderField.Name => new Edge<LocationMember>(item.Customer.Name.ToCursor(), item),
            LocationMemberOrderField.GivenName => new Edge<LocationMember>(item.Customer.GivenName.ToCursor(), item),
            LocationMemberOrderField.MiddleName => new Edge<LocationMember>(item.Customer.MiddleName.ToCursor(), item),
            LocationMemberOrderField.FamilyName => new Edge<LocationMember>(item.Customer.FamilyName.ToCursor(), item),
            null => new Edge<LocationMember>(item.CreatedAt.ToCursor(), item),
            _ => new Edge<LocationMember>(item.CreatedAt.ToCursor(), item)
        }).ToList();
}

public class LocationMemberRepository(LocationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<LocationDbContext, LocationMember>(dbContext), ILocationMemberRepository
{
    public async Task<LocationMember?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.LocationMember
            .Where(query => query.Id == id)
            .Include(query => query.Location)
            .OrderBy(query => query.Id)
            .FirstOrDefaultAsync(cancellationToken);

    public LocationMember Add(LocationMember locationMember)
    {
        var now = timeProvider.GetUtcNow();
        locationMember.CreatedAt = now;
        return DbContext.LocationMember.Add(locationMember).Entity;
    }

    public void AddRange(ICollection<LocationMember> locationMembers)
    {
        var now = timeProvider.GetUtcNow();
        locationMembers.ForEach(locationMember => locationMember.CreatedAt = now);
        DbContext.LocationMember.AddRange(locationMembers);
    }

    public void RemoveRange(ICollection<LocationMember> locationMembers)
    {
        var now = timeProvider.GetUtcNow();
        locationMembers.ForEach(locationMember => locationMember.DeletedAt = now);
        DbContext.LocationMember.RemoveRange(locationMembers);
    }

    public LocationMember Update(LocationMember locationMember)
    {
        var now = timeProvider.GetUtcNow();
        locationMember.ModifiedAt = now;
        return DbContext.LocationMember.Update(locationMember).Entity;
    }

    public async Task<(PaginatedInfo, ICollection<Edge<LocationMember>>, int)> GetPaginatedLocationMembersAsync(
        PaginationInputParam paginationInputParam,
        LocationMemberSearchCriteria searchCriteria,
        ICollection<LocationMemberOrder> orderByFields,
        CancellationToken cancellationToken)
    {
        var totalCount = await DbContext.LocationMember.AsQueryable().AddSearchCriteria(searchCriteria)
            .CountAsync(cancellationToken);
        if (totalCount == 0)
        {
            return (new PaginatedInfo(false, false, null, null), [], totalCount);
        }

        var (paginatedInfo, edges) = (await DbContext.LocationMember
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
