using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Pagination;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Team.Shared.Database;
using Team.Shared.Models;
using Identity = Team.Shared.Database.Entities.Identity;
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

    Task<(PaginatedInfo, ICollection<Edge<TeamMember>>, int)> GetPaginatedTeamMembersUntrackedAsync(
        PaginationInputParam paginationInputParam,
        TeamMemberSearchCriteria searchCriteria,
        ICollection<TeamMemberOrder> orderByFields,
        CancellationToken cancellationToken);

    Task<ICollection<TeamMember>> GetByTeamIdAsync(string teamId, CancellationToken cancellationToken);
    Task<TeamMember?> GetByTeamIdAndOrganizationMemberIdAsync(string teamId, string organizationMemberId, CancellationToken cancellationToken);
}

internal static class TeamMemberExtensions
{
    extension(IQueryable<TeamMember> originalQuery)
    {
        internal IIncludableQueryable<TeamMember, ICollection<Identity>> AddDependentObjects(bool isTracked) =>
            (isTracked ? originalQuery.AsTracking() : originalQuery.AsNoTracking())
            .Include(query => query.Team)
            .Include(query => query.Customer)
            .ThenInclude(query => query.Identities)
            .Include(query => query.OrganizationMember)
            .ThenInclude(query => query!.Organization)
            .Include(query => query.OrganizationMember)
            .ThenInclude(query => query!.Customer)
            .ThenInclude(query => query.Identities);

        internal IQueryable<TeamMember> AddSearchCriteria(TeamMemberSearchCriteria searchCriteria)
        {
            originalQuery = originalQuery.Where(item => !item.DeletedAt.HasValue && item.Team.Id == searchCriteria.TeamId);

            if (!string.IsNullOrWhiteSpace(searchCriteria.NameContains))
            {
                originalQuery = originalQuery.Where(item =>
                    (item.Customer.Name != null &&
                     EF.Functions.ILike(item.Customer.Name, $"%{searchCriteria.NameContains}%")) ||
                    (item.Customer.GivenName != null &&
                     EF.Functions.ILike(item.Customer.GivenName, $"%{searchCriteria.NameContains}%")) ||
                    (item.Customer.MiddleName != null &&
                     EF.Functions.ILike(item.Customer.MiddleName, $"%{searchCriteria.NameContains}%")) ||
                    (item.Customer.FamilyName != null &&
                     EF.Functions.ILike(item.Customer.FamilyName, $"%{searchCriteria.NameContains}%")));
            }

            return originalQuery;
        }
    }
}

public class TeamMemberRepository(TeamDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<TeamDbContext, TeamMember>(dbContext, timeProvider), ITeamMemberRepository
{
    public async Task<TeamMember?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.TeamMember
            .AddDependentObjects(true)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<ICollection<TeamMember>> GetByIdsAsync(
        ICollection<string> ids,
        CancellationToken cancellationToken) =>
        await DbContext.TeamMember
            .Where(query => ids.Contains(query.Id))
            .AddDependentObjects(true)
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
        GetPaginatedTeamMembersUntrackedAsync(
            PaginationInputParam paginationInputParam,
            TeamMemberSearchCriteria searchCriteria,
            ICollection<TeamMemberOrder> orderByFields,
            CancellationToken cancellationToken) =>
        await DbContext.TeamMember
            .AddSearchCriteria(searchCriteria)
            .AddDependentObjects(false)
            .ToPaginatedAsync(paginationInputParam, GetPaginationFields(orderByFields), cancellationToken);

    public async Task<ICollection<TeamMember>> GetByTeamIdAsync(string teamId, CancellationToken cancellationToken) =>
        await DbContext.TeamMember
            .Where(query => query.Team.Id == teamId)
            .AddDependentObjects(true)
            .ToListAsync(cancellationToken);

    public async Task<TeamMember?> GetByTeamIdAndOrganizationMemberIdAsync(
        string teamId,
        string organizationMemberId,
        CancellationToken cancellationToken) =>
        await DbContext.TeamMember
            .Include(query => query.Customer)
            .FirstOrDefaultAsync(
                query => query.Team.Id == teamId &&
                         query.OrganizationMember != null &&
                         query.OrganizationMember.Id == organizationMemberId,
                cancellationToken);

    private static List<KeysetPaginationField<TeamMember>> GetPaginationFields(ICollection<TeamMemberOrder> orderByFields)
    {
        if (orderByFields.Count == 0)
        {
            return
            [
                KeysetPaginationField<TeamMember>.Create(
                    nameof(Customer.Name),
                    query => query.Customer.Name,
                    OrderDirection.Ascending)
            ];
        }

        return orderByFields.Select(orderField => orderField.Field switch
            {
                TeamMemberOrderField.Role => KeysetPaginationField<TeamMember>.Create(
                    nameof(TeamMember.Role),
                    query => query.Role,
                    orderField.Direction),
                TeamMemberOrderField.Status => KeysetPaginationField<TeamMember>.Create(
                    nameof(TeamMember.Status),
                    query => query.Status,
                    orderField.Direction),
                TeamMemberOrderField.Name => KeysetPaginationField<TeamMember>.Create(
                    nameof(Customer.Name),
                    query => query.Customer.Name, orderField.Direction),
                TeamMemberOrderField.GivenName => KeysetPaginationField<TeamMember>.Create(
                    nameof(Customer.GivenName),
                    query => query.Customer.GivenName,
                    orderField.Direction),
                TeamMemberOrderField.MiddleName => KeysetPaginationField<TeamMember>.Create(
                    nameof(Customer.MiddleName),
                    query => query.Customer.MiddleName,
                    orderField.Direction),
                TeamMemberOrderField.FamilyName => KeysetPaginationField<TeamMember>.Create(
                    nameof(Customer.FamilyName),
                    query => query.Customer.FamilyName,
                    orderField.Direction),
                _ => throw new ArgumentOutOfRangeException()
            })
            .ToList();
    }
}
