using Api.Shared.Models;
using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Models;
using Enterprise.Shared.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Organization.Shared.Database;
using Organization.Shared.Models;
using Customer = Organization.Shared.Database.Entities.Customer;
using OrganizationMember = Organization.Shared.Database.Entities.OrganizationMember;

namespace Organization.Shared.Repositories;

public interface IOrganizationMemberRepository : IRepository<OrganizationMember>
{
    Task<OrganizationMember?> GetByIdAsync(string id, CancellationToken cancellationToken);
    OrganizationMember Add(OrganizationMember organizationMember);
    void AddRange(ICollection<OrganizationMember> organizationMembers);
    OrganizationMember Update(OrganizationMember organizationMember);
    void RemoveRange(ICollection<OrganizationMember> organizationMembers);

    Task<(PaginatedInfo, ICollection<Edge<OrganizationMember>>, int)> GetPaginatedOrganizationMembersAsync(
        PaginationInputParam paginationInputParam,
        OrganizationMemberSearchCriteria searchCriteria,
        ICollection<OrganizationMemberOrder> orderByFields,
        CancellationToken cancellationToken);
}

internal static class OrganizationMemberExtensions
{
    internal static IIncludableQueryable<OrganizationMember, Customer> AddDependentObjects(
        this IQueryable<OrganizationMember> originalQuery) =>
        originalQuery
            .Include(query => query.Organization)
            .Include(query => query.Customer);

    internal static IQueryable<OrganizationMember> AddSearchCriteria(
        this IQueryable<OrganizationMember> query,
        OrganizationMemberSearchCriteria searchCriteria)
    {
        query = query.Where(item => !item.DeletedAt.HasValue && item.Organization.Id == searchCriteria.OrganizationId);

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

    internal static IQueryable<OrganizationMember> AddSortingOrders(
        this IQueryable<OrganizationMember> originalQuery,
        ICollection<OrganizationMemberOrder> orderByFields)
    {
        if (orderByFields.Count == 0)
        {
            return originalQuery.OrderBy(query => query.CreatedAt);
        }

        var orderByField = orderByFields.First();
        return orderByFields.Skip(1).Aggregate(orderByField.Field switch
        {
            OrganizationMemberOrderField.MembershipType => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.MembershipType)
                : originalQuery.OrderByDescending(x => x.MembershipType),
            OrganizationMemberOrderField.Name => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.Customer.Name)
                : originalQuery.OrderByDescending(x => x.Customer.Name),
            OrganizationMemberOrderField.GivenName => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.Customer.GivenName)
                : originalQuery.OrderByDescending(x => x.Customer.GivenName),
            OrganizationMemberOrderField.MiddleName => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.Customer.MiddleName)
                : originalQuery.OrderByDescending(x => x.Customer.MiddleName),
            OrganizationMemberOrderField.FamilyName => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.Customer.FamilyName)
                : originalQuery.OrderByDescending(x => x.Customer.FamilyName),
            _ => throw new ArgumentOutOfRangeException()
        }, (query, orderField) =>
            orderField.Field switch
            {
                OrganizationMemberOrderField.MembershipType => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.MembershipType)
                    : query.ThenByDescending(x => x.MembershipType),
                OrganizationMemberOrderField.Name => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.Customer.Name)
                    : query.ThenByDescending(x => x.Customer.Name),
                OrganizationMemberOrderField.GivenName => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.Customer.GivenName)
                    : query.ThenByDescending(x => x.Customer.GivenName),
                OrganizationMemberOrderField.MiddleName => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.Customer.MiddleName)
                    : query.ThenByDescending(x => x.Customer.MiddleName),
                OrganizationMemberOrderField.FamilyName => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.Customer.FamilyName)
                    : query.ThenByDescending(x => x.Customer.FamilyName),
                _ => throw new ArgumentOutOfRangeException()
            });
    }

    public static IQueryable<OrganizationMember> ApplyPaginationFilters(
        this IQueryable<OrganizationMember> query,
        PaginationInputParam paginationInputParam,
        ICollection<OrganizationMemberOrder> orderByFields)
    {
        var orderByField = orderByFields.FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(paginationInputParam.After))
        {
            query = orderByField?.Field switch
            {
                OrganizationMemberOrderField.MembershipType => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.MembershipType.CompareTo(paginationInputParam.After
                            .FromCursorToEnum<OrganizationMembershipType>()) > 0)
                    : query.Where(item =>
                        item.MembershipType.CompareTo(paginationInputParam.After
                            .FromCursorToEnum<OrganizationMembershipType>()) < 0),
                OrganizationMemberOrderField.Name => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.Customer.Name != null &&
                        item.Customer.Name.CompareTo(paginationInputParam.After.FromCursor()) > 0)
                    : query.Where(item =>
                        item.Customer.Name != null &&
                        item.Customer.Name.CompareTo(paginationInputParam.After.FromCursor()) < 0),
                OrganizationMemberOrderField.GivenName => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.Customer.GivenName != null &&
                        item.Customer.GivenName.CompareTo(paginationInputParam.After.FromCursor()) > 0)
                    : query.Where(item =>
                        item.Customer.GivenName != null &&
                        item.Customer.GivenName.CompareTo(paginationInputParam.After.FromCursor()) < 0),
                OrganizationMemberOrderField.MiddleName => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.Customer.MiddleName != null &&
                        item.Customer.MiddleName.CompareTo(paginationInputParam.After.FromCursor()) > 0)
                    : query.Where(item =>
                        item.Customer.MiddleName != null &&
                        item.Customer.MiddleName.CompareTo(paginationInputParam.After.FromCursor()) < 0),
                OrganizationMemberOrderField.FamilyName => orderByField.Direction == OrderDirection.Ascending
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
                OrganizationMemberOrderField.MembershipType => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.MembershipType.CompareTo(
                            paginationInputParam.Before.FromCursorToEnum<OrganizationMembershipType>()) < 0)
                    : query.Where(item =>
                        item.MembershipType.CompareTo(
                            paginationInputParam.Before.FromCursorToEnum<OrganizationMembershipType>()) > 0),
                OrganizationMemberOrderField.Name => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.Customer.Name != null &&
                        item.Customer.Name.CompareTo(paginationInputParam.Before.FromCursor()) < 0)
                    : query.Where(item =>
                        item.Customer.Name != null &&
                        item.Customer.Name.CompareTo(paginationInputParam.Before.FromCursor()) > 0),
                OrganizationMemberOrderField.GivenName => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.Customer.GivenName != null &&
                        item.Customer.GivenName.CompareTo(paginationInputParam.Before.FromCursor()) < 0)
                    : query.Where(item =>
                        item.Customer.GivenName != null &&
                        item.Customer.GivenName.CompareTo(paginationInputParam.Before.FromCursor()) > 0),
                OrganizationMemberOrderField.MiddleName => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.Customer.MiddleName != null &&
                        item.Customer.MiddleName.CompareTo(paginationInputParam.Before.FromCursor()) < 0)
                    : query.Where(item =>
                        item.Customer.MiddleName != null &&
                        item.Customer.MiddleName.CompareTo(paginationInputParam.Before.FromCursor()) > 0),
                OrganizationMemberOrderField.FamilyName => orderByField.Direction == OrderDirection.Ascending
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

    public static ICollection<Edge<OrganizationMember>> ToEdges(
        this ICollection<OrganizationMember> items,
        ICollection<OrganizationMemberOrder> orderByFields) =>
        items.Select(item => orderByFields.FirstOrDefault()?.Field switch
        {
            OrganizationMemberOrderField.MembershipType => new Edge<OrganizationMember>(item.MembershipType.ToCursor(),
                item),
            OrganizationMemberOrderField.Name => new Edge<OrganizationMember>(item.Customer.Name.ToCursor(), item),
            OrganizationMemberOrderField.GivenName => new Edge<OrganizationMember>(item.Customer.GivenName.ToCursor(),
                item),
            OrganizationMemberOrderField.MiddleName => new Edge<OrganizationMember>(item.Customer.MiddleName.ToCursor(),
                item),
            OrganizationMemberOrderField.FamilyName => new Edge<OrganizationMember>(item.Customer.FamilyName.ToCursor(),
                item),
            null => new Edge<OrganizationMember>(item.CreatedAt.ToCursor(), item),
            _ => new Edge<OrganizationMember>(item.CreatedAt.ToCursor(), item)
        }).ToList();
}

public class OrganizationMemberRepository(OrganizationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<OrganizationDbContext, OrganizationMember>(dbContext), IOrganizationMemberRepository
{
    public async Task<OrganizationMember?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.OrganizationMember
            .Where(query => query.Id == id)
            .Include(query => query.Organization)
            .OrderBy(query => query.Id)
            .FirstOrDefaultAsync(cancellationToken);

    public OrganizationMember Add(OrganizationMember organizationMember)
    {
        var now = timeProvider.GetUtcNow();
        organizationMember.CreatedAt = now;
        return DbContext.OrganizationMember.Add(organizationMember).Entity;
    }

    public void AddRange(ICollection<OrganizationMember> organizationMembers)
    {
        var now = timeProvider.GetUtcNow();
        organizationMembers.ForEach(organizationMember => organizationMember.CreatedAt = now);
        DbContext.OrganizationMember.AddRange(organizationMembers);
    }

    public void RemoveRange(ICollection<OrganizationMember> organizationMembers)
    {
        var now = timeProvider.GetUtcNow();
        organizationMembers.ForEach(organizationMember => organizationMember.DeletedAt = now);
        DbContext.OrganizationMember.RemoveRange(organizationMembers);
    }

    public OrganizationMember Update(OrganizationMember organizationMember)
    {
        var now = timeProvider.GetUtcNow();
        organizationMember.ModifiedAt = now;
        return DbContext.OrganizationMember.Update(organizationMember).Entity;
    }

    public async Task<(PaginatedInfo, ICollection<Edge<OrganizationMember>>, int)>
        GetPaginatedOrganizationMembersAsync(
            PaginationInputParam paginationInputParam,
            OrganizationMemberSearchCriteria searchCriteria,
            ICollection<OrganizationMemberOrder> orderByFields,
            CancellationToken cancellationToken)
    {
        var totalCount = await DbContext.OrganizationMember.AsQueryable().AddSearchCriteria(searchCriteria)
            .CountAsync(cancellationToken);
        if (totalCount == 0)
        {
            return (new PaginatedInfo(false, false, null, null), [], totalCount);
        }

        var (paginatedInfo, edges) = (await DbContext.OrganizationMember
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
