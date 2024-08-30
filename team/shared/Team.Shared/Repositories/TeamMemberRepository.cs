using Api.Shared.Models;
using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Models;
using Enterprise.Shared.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Team.Shared.Database;
using Team.Shared.Models;
using Customer = Team.Shared.Database.Entities.Customer;
using TeamMember = Team.Shared.Database.Entities.TeamMember;

namespace Team.Shared.Repositories;

public interface ITeamMemberRepository : IRepository<TeamMember>
{
    Task<TeamMember?> GetByIdAsync(string id, CancellationToken cancellationToken);
    TeamMember Add(TeamMember teamMember);
    void AddRange(ICollection<TeamMember> teamMembers);
    TeamMember Update(TeamMember teamMember);
    void RemoveRange(ICollection<TeamMember> teamMembers);

    Task<(PaginatedInfo, ICollection<Edge<TeamMember>>, int)> GetPaginatedTeamMembersAsync(
        PaginationInputParam paginationInputParam,
        TeamMemberSearchCriteria searchCriteria,
        ICollection<TeamMemberOrder> orderByFields,
        CancellationToken cancellationToken);
}

internal static class TeamMemberExtensions
{
    internal static IIncludableQueryable<TeamMember, Customer> AddDependentObjects(
        this IQueryable<TeamMember> originalQuery) =>
        originalQuery
            .Include(query => query.Team)
            .Include(query => query.Customer)
            .Include(query => query.OrganizationMember)
            .ThenInclude(query => query.Organization)
            .Include(query => query.OrganizationMember)
            .ThenInclude(query => query.Customer);

    internal static IQueryable<TeamMember> AddSearchCriteria(
        this IQueryable<TeamMember> query,
        TeamMemberSearchCriteria searchCriteria)
    {
        query = query.Where(item => !item.DeletedAt.HasValue && item.Team.Id == searchCriteria.TeamId);

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

    internal static IQueryable<TeamMember> AddSortingOrders(
        this IQueryable<TeamMember> originalQuery,
        ICollection<TeamMemberOrder> orderByFields)
    {
        if (orderByFields.Count == 0)
        {
            return originalQuery.OrderBy(query => query.CreatedAt);
        }

        var orderByField = orderByFields.First();
        return orderByFields.Skip(1).Aggregate(orderByField.Field switch
        {
            TeamMemberOrderField.MembershipType => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.MembershipType)
                : originalQuery.OrderByDescending(x => x.MembershipType),
            TeamMemberOrderField.Name => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.Customer.Name)
                : originalQuery.OrderByDescending(x => x.Customer.Name),
            TeamMemberOrderField.GivenName => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.Customer.GivenName)
                : originalQuery.OrderByDescending(x => x.Customer.GivenName),
            TeamMemberOrderField.MiddleName => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.Customer.MiddleName)
                : originalQuery.OrderByDescending(x => x.Customer.MiddleName),
            TeamMemberOrderField.FamilyName => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.Customer.FamilyName)
                : originalQuery.OrderByDescending(x => x.Customer.FamilyName),
            _ => throw new ArgumentOutOfRangeException()
        }, (query, orderField) =>
            orderField.Field switch
            {
                TeamMemberOrderField.MembershipType => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.MembershipType)
                    : query.ThenByDescending(x => x.MembershipType),
                TeamMemberOrderField.Name => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.Customer.Name)
                    : query.ThenByDescending(x => x.Customer.Name),
                TeamMemberOrderField.GivenName => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.Customer.GivenName)
                    : query.ThenByDescending(x => x.Customer.GivenName),
                TeamMemberOrderField.MiddleName => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.Customer.MiddleName)
                    : query.ThenByDescending(x => x.Customer.MiddleName),
                TeamMemberOrderField.FamilyName => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.Customer.FamilyName)
                    : query.ThenByDescending(x => x.Customer.FamilyName),
                _ => throw new ArgumentOutOfRangeException()
            });
    }

    public static IQueryable<TeamMember> ApplyPaginationFilters(
        this IQueryable<TeamMember> query,
        PaginationInputParam paginationInputParam,
        ICollection<TeamMemberOrder> orderByFields)
    {
        var orderByField = orderByFields.FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(paginationInputParam.After))
        {
            query = orderByField?.Field switch
            {
                TeamMemberOrderField.MembershipType => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.MembershipType.CompareTo(paginationInputParam.After
                            .FromCursorToEnum<TeamMembershipType>()) > 0)
                    : query.Where(item =>
                        item.MembershipType.CompareTo(paginationInputParam.After
                            .FromCursorToEnum<TeamMembershipType>()) < 0),
                TeamMemberOrderField.Name => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.Customer.Name != null &&
                        item.Customer.Name.CompareTo(paginationInputParam.After.FromCursor()) > 0)
                    : query.Where(item =>
                        item.Customer.Name != null &&
                        item.Customer.Name.CompareTo(paginationInputParam.After.FromCursor()) < 0),
                TeamMemberOrderField.GivenName => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.Customer.GivenName != null &&
                        item.Customer.GivenName.CompareTo(paginationInputParam.After.FromCursor()) > 0)
                    : query.Where(item =>
                        item.Customer.GivenName != null &&
                        item.Customer.GivenName.CompareTo(paginationInputParam.After.FromCursor()) < 0),
                TeamMemberOrderField.MiddleName => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.Customer.MiddleName != null &&
                        item.Customer.MiddleName.CompareTo(paginationInputParam.After.FromCursor()) > 0)
                    : query.Where(item =>
                        item.Customer.MiddleName != null &&
                        item.Customer.MiddleName.CompareTo(paginationInputParam.After.FromCursor()) < 0),
                TeamMemberOrderField.FamilyName => orderByField.Direction == OrderDirection.Ascending
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
                TeamMemberOrderField.MembershipType => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.MembershipType.CompareTo(
                            paginationInputParam.Before.FromCursorToEnum<TeamMembershipType>()) < 0)
                    : query.Where(item =>
                        item.MembershipType.CompareTo(
                            paginationInputParam.Before.FromCursorToEnum<TeamMembershipType>()) > 0),
                TeamMemberOrderField.Name => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.Customer.Name != null &&
                        item.Customer.Name.CompareTo(paginationInputParam.Before.FromCursor()) < 0)
                    : query.Where(item =>
                        item.Customer.Name != null &&
                        item.Customer.Name.CompareTo(paginationInputParam.Before.FromCursor()) > 0),
                TeamMemberOrderField.GivenName => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.Customer.GivenName != null &&
                        item.Customer.GivenName.CompareTo(paginationInputParam.Before.FromCursor()) < 0)
                    : query.Where(item =>
                        item.Customer.GivenName != null &&
                        item.Customer.GivenName.CompareTo(paginationInputParam.Before.FromCursor()) > 0),
                TeamMemberOrderField.MiddleName => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.Customer.MiddleName != null &&
                        item.Customer.MiddleName.CompareTo(paginationInputParam.Before.FromCursor()) < 0)
                    : query.Where(item =>
                        item.Customer.MiddleName != null &&
                        item.Customer.MiddleName.CompareTo(paginationInputParam.Before.FromCursor()) > 0),
                TeamMemberOrderField.FamilyName => orderByField.Direction == OrderDirection.Ascending
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

    public static ICollection<Edge<TeamMember>> ToEdges(
        this ICollection<TeamMember> items,
        ICollection<TeamMemberOrder> orderByFields) =>
        items.Select(item => orderByFields.FirstOrDefault()?.Field switch
        {
            TeamMemberOrderField.MembershipType => new Edge<TeamMember>(item.MembershipType.ToCursor(), item),
            TeamMemberOrderField.Name => new Edge<TeamMember>(item.Customer.Name.ToCursor(), item),
            TeamMemberOrderField.GivenName => new Edge<TeamMember>(item.Customer.GivenName.ToCursor(), item),
            TeamMemberOrderField.MiddleName => new Edge<TeamMember>(item.Customer.MiddleName.ToCursor(), item),
            TeamMemberOrderField.FamilyName => new Edge<TeamMember>(item.Customer.FamilyName.ToCursor(), item),
            null => new Edge<TeamMember>(item.CreatedAt.ToCursor(), item),
            _ => new Edge<TeamMember>(item.CreatedAt.ToCursor(), item)
        }).ToList();
}

public class TeamMemberRepository(TeamDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<TeamDbContext, TeamMember>(dbContext), ITeamMemberRepository
{
    public async Task<TeamMember?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.TeamMember
            .Where(query => query.Id == id)
            .Include(query => query.Team)
            .OrderBy(query => query.Id)
            .FirstOrDefaultAsync(cancellationToken);

    public TeamMember Add(TeamMember teamMember)
    {
        var now = timeProvider.GetUtcNow();
        teamMember.CreatedAt = now;
        return DbContext.TeamMember.Add(teamMember).Entity;
    }

    public void AddRange(ICollection<TeamMember> teamMembers)
    {
        var now = timeProvider.GetUtcNow();
        teamMembers.ForEach(teamMember => teamMember.CreatedAt = now);
        DbContext.TeamMember.AddRange(teamMembers);
    }

    public void RemoveRange(ICollection<TeamMember> teamMembers)
    {
        var now = timeProvider.GetUtcNow();
        teamMembers.ForEach(teamMember => teamMember.DeletedAt = now);
        DbContext.TeamMember.UpdateRange(teamMembers);
    }

    public TeamMember Update(TeamMember teamMember)
    {
        var now = timeProvider.GetUtcNow();
        teamMember.ModifiedAt = now;
        return DbContext.TeamMember.Update(teamMember).Entity;
    }

    public async Task<(PaginatedInfo, ICollection<Edge<TeamMember>>, int)>
        GetPaginatedTeamMembersAsync(
            PaginationInputParam paginationInputParam,
            TeamMemberSearchCriteria searchCriteria,
            ICollection<TeamMemberOrder> orderByFields,
            CancellationToken cancellationToken)
    {
        var totalCount = await DbContext.TeamMember.AsQueryable().AddSearchCriteria(searchCriteria)
            .CountAsync(cancellationToken);
        if (totalCount == 0)
        {
            return (new PaginatedInfo(false, false, null, null), [], totalCount);
        }

        var (paginatedInfo, edges) = (await DbContext.TeamMember
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
