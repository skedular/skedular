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
    Task<ICollection<TeamMember>> GetByIdsAsync(ICollection<string> ids, CancellationToken cancellationToken);
    TeamMember Add(TeamMember teamMember);
    void AddRange(ICollection<TeamMember> teamMembers);
    TeamMember Update(TeamMember teamMember);
    TeamMember Remove(TeamMember teamMember);
    void RemoveRange(ICollection<TeamMember> teamMembers);

    Task<(PaginatedInfo, ICollection<Edge<TeamMember>>, int)> GetPaginatedTeamMembersAsync(
        PaginationInputParam paginationInputParam,
        TeamMemberSearchCriteria searchCriteria,
        ICollection<TeamMemberOrder> orderByFields,
        CancellationToken cancellationToken);

    Task<ICollection<TeamMember>> GetByTeamIdAsync(
        string teamId,
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
            return originalQuery.OrderBy(query => query.Customer.Name).ThenBy(query => query.Id);
        }

        var orderByField = orderByFields.First();
        return orderByFields.Skip(1).Aggregate(orderByField.Field switch
        {
            TeamMemberOrderField.MembershipType => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.MembershipType)
                : originalQuery.OrderByDescending(x => x.MembershipType),
            TeamMemberOrderField.Status => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.Status)
                : originalQuery.OrderByDescending(x => x.Status),
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
            TeamMemberOrderField.PhoneNumber => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.Customer.PhoneNumber)
                : originalQuery.OrderByDescending(x => x.Customer.PhoneNumber),
            _ => throw new ArgumentOutOfRangeException()
        }, (query, orderField) =>
            orderField.Field switch
            {
                TeamMemberOrderField.MembershipType => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.MembershipType)
                    : query.ThenByDescending(x => x.MembershipType),
                TeamMemberOrderField.Status => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.Status)
                    : query.ThenByDescending(x => x.Status),
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
                TeamMemberOrderField.PhoneNumber => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.Customer.PhoneNumber)
                    : query.ThenByDescending(x => x.Customer.PhoneNumber),
                _ => throw new ArgumentOutOfRangeException()
            }).ThenBy(query => query.Id);
    }
}

public class TeamMemberRepository(TeamDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<TeamDbContext, TeamMember>(dbContext, timeProvider), ITeamMemberRepository
{
    public async Task<TeamMember?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.TeamMember
            .Include(query => query.Team)
            .Include(query => query.Customer)
            .Include(query => query.OrganizationMember)
            .ThenInclude(query => query.Customer)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<ICollection<TeamMember>> GetByIdsAsync(
        ICollection<string> ids,
        CancellationToken cancellationToken) =>
        await DbContext.TeamMember
            .Where(query => ids.Contains(query.Id))
            .Include(query => query.Team)
            .Include(query => query.Customer)
            .Include(query => query.OrganizationMember)
            .ThenInclude(query => query.Customer)
            .ToListAsync(cancellationToken);

    public TeamMember Add(TeamMember teamMember)
    {
        var now = TimeProvider.GetUtcNow();
        teamMember.CreatedAt = now;
        return DbContext.TeamMember.Add(teamMember).Entity;
    }

    public void AddRange(ICollection<TeamMember> teamMembers)
    {
        var now = TimeProvider.GetUtcNow();
        teamMembers.ForEach(teamMember => teamMember.CreatedAt = now);
        DbContext.TeamMember.AddRange(teamMembers);
    }

    public TeamMember Remove(TeamMember teamMember)
    {
        var now = TimeProvider.GetUtcNow();
        teamMember.DeletedAt = now;
        return DbContext.TeamMember.Update(teamMember).Entity;
    }

    public void RemoveRange(ICollection<TeamMember> teamMembers)
    {
        var now = TimeProvider.GetUtcNow();
        teamMembers.ForEach(teamMember => teamMember.DeletedAt = now);
        DbContext.TeamMember.UpdateRange(teamMembers);
    }

    public TeamMember Update(TeamMember teamMember)
    {
        var now = TimeProvider.GetUtcNow();
        teamMember.ModifiedAt = now;
        return DbContext.TeamMember.Update(teamMember).Entity;
    }

    public async Task<(PaginatedInfo, ICollection<Edge<TeamMember>>, int)>
        GetPaginatedTeamMembersAsync(
            PaginationInputParam paginationInputParam,
            TeamMemberSearchCriteria searchCriteria,
            ICollection<TeamMemberOrder> orderByFields,
            CancellationToken cancellationToken) =>
        (await DbContext.TeamMember
            .AddSearchCriteria(searchCriteria)
            .AddSortingOrders(orderByFields)
            .AddDependentObjects()
            .ToListAsync(cancellationToken))
        .ToPaginated(paginationInputParam);

    public async Task<ICollection<TeamMember>> GetByTeamIdAsync(
        string teamId,
        CancellationToken cancellationToken) =>
        await DbContext.TeamMember
            .Where(query => query.Team.Id == teamId)
            .Include(query => query.Customer)
            .ToListAsync(cancellationToken);
}
