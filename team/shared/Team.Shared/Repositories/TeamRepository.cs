using Enterprise.Shared.Database;
using Enterprise.Shared.Pagination;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Team.Shared.Database;
using Team.Shared.Models;
using Customer = Team.Shared.Database.Entities.Customer;

namespace Team.Shared.Repositories;

public interface ITeamRepository : IRepository<Database.Entities.Team>
{
    Task<Database.Entities.Team?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<Database.Entities.Team?> GetByIdUntrackedAsync(string id, CancellationToken cancellationToken);
    Task<ICollection<Database.Entities.Team>> GetByIdsAsync(ICollection<string> ids, CancellationToken cancellationToken);

    Task<ICollection<Database.Entities.Team>> GetByCustomerIdUntrackedAsync(
        string customerId,
        string? organizationId,
        CancellationToken cancellationToken);

    Task<ICollection<Database.Entities.Team>> GetAllUntrackedAsync(CancellationToken cancellationToken);
    Database.Entities.Team Add(Database.Entities.Team team);
    Database.Entities.Team Update(Database.Entities.Team team);
    Database.Entities.Team Remove(Database.Entities.Team team);

    Task<(PaginatedInfo, ICollection<Edge<Database.Entities.Team>>, int)> GetPaginatedTeamsUntrackedAsync(
        PaginationInputParam paginationInputParam,
        TeamSearchCriteria searchCriteria,
        ICollection<TeamOrder> orderByFields,
        CancellationToken cancellationToken);
}

internal static class TeamExtensions
{
    extension(IQueryable<Database.Entities.Team> originalQuery)
    {
        internal IIncludableQueryable<Database.Entities.Team, Customer> AddDependentObjects(bool isTracked) =>
            (isTracked ? originalQuery.AsTracking() : originalQuery.AsNoTracking())
            .Include(query => query.PrimaryLocation)
            .Include(query => query.Organization)
            .ThenInclude(query => query.OrganizationMembers.Where(organizationMember => !organizationMember.DeletedAt.HasValue))
            .ThenInclude(query => query.Customer)
            .Include(query => query.TeamMembers.Where(teamMember => !teamMember.DeletedAt.HasValue))
            .ThenInclude(query => query.Customer)
            .Include(query => query.TeamMembers.Where(teamMember => !teamMember.DeletedAt.HasValue))
            .ThenInclude(query => query.OrganizationMember)
            .ThenInclude(query => query!.Organization)
            .ThenInclude(query => query.OrganizationMembers.Where(organizationMember => !organizationMember.DeletedAt.HasValue))
            .Include(query => query.TeamMembers.Where(teamMember => !teamMember.DeletedAt.HasValue))
            .ThenInclude(query => query.OrganizationMember)
            .ThenInclude(query => query!.Customer);

        internal IQueryable<Database.Entities.Team> AddSearchCriteria(TeamSearchCriteria searchCriteria)
        {
            originalQuery = originalQuery.Where(item => !item.DeletedAt.HasValue);

            if (!string.IsNullOrWhiteSpace(searchCriteria.OrganizationId))
            {
                originalQuery = originalQuery.Where(item => item.Organization.Id == searchCriteria.OrganizationId);
            }

            if (!string.IsNullOrWhiteSpace(searchCriteria.OrganizationCustomDomain))
            {
                originalQuery = originalQuery.Where(item =>
                    item.Organization.CustomDomain != null &&
                    item.Organization.CustomDomain == searchCriteria.OrganizationCustomDomain);
            }

            if (!string.IsNullOrWhiteSpace(searchCriteria.CustomerId))
            {
                originalQuery = originalQuery.Where(item =>
                    item.TeamMembers.Any(teamMember => !teamMember.DeletedAt.HasValue && teamMember.Customer.Id == searchCriteria.CustomerId));
            }

            if (!string.IsNullOrWhiteSpace(searchCriteria.NameContains))
            {
                originalQuery = originalQuery.Where(item => EF.Functions.ILike(item.Name, $"%{searchCriteria.NameContains}%"));
            }

            if (searchCriteria.PrimaryLocationIds.Count != 0)
            {
                originalQuery = originalQuery.Where(item =>
                    item.PrimaryLocation != null && searchCriteria.PrimaryLocationIds.Contains(item.PrimaryLocation.Id));
            }

            return originalQuery;
        }
    }
}

public class TeamRepository(TeamDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<TeamDbContext, Database.Entities.Team>(dbContext, timeProvider), ITeamRepository
{
    public async Task<Database.Entities.Team?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Team
            .AddDependentObjects(true)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<Database.Entities.Team?> GetByIdUntrackedAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Team
            .AddDependentObjects(false)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<ICollection<Database.Entities.Team>> GetByIdsAsync(ICollection<string> ids, CancellationToken cancellationToken) =>
        await DbContext.Team
            .Where(query => ids.Contains(query.Id))
            .AddDependentObjects(true)
            .ToListAsync(cancellationToken);

    public async Task<ICollection<Database.Entities.Team>> GetByCustomerIdUntrackedAsync(
        string customerId,
        string? organizationId,
        CancellationToken cancellationToken)
    {
        var query = DbContext.Team.Where(team => !team.DeletedAt.HasValue && team.TeamMembers.Any(item => item.Customer.Id == customerId));
        if (!string.IsNullOrWhiteSpace(organizationId))
        {
            query = query.Where(team => !team.Organization.DeletedAt.HasValue && team.Organization.Id == organizationId);
        }

        return await query
            .AddDependentObjects(false)
            .ToListAsync(cancellationToken);
    }

    public async Task<ICollection<Database.Entities.Team>> GetAllUntrackedAsync(CancellationToken cancellationToken) =>
        await DbContext.Team
            .Where(query => !query.DeletedAt.HasValue)
            .AddDependentObjects(false)
            .ToListAsync(cancellationToken);

    public Database.Entities.Team Add(Database.Entities.Team team)
    {
        var now = TimeProvider.GetUtcNow();
        team.CreatedAt = now;
        return DbContext.Team.Add(team).Entity;
    }

    public Database.Entities.Team Update(Database.Entities.Team team)
    {
        var now = TimeProvider.GetUtcNow();
        team.ModifiedAt = now;
        return DbContext.Team.Update(team).Entity;
    }

    public Database.Entities.Team Remove(Database.Entities.Team team)
    {
        var now = TimeProvider.GetUtcNow();
        team.DeletedAt = now;
        return DbContext.Team.Update(team).Entity;
    }

    public async Task<(PaginatedInfo, ICollection<Edge<Database.Entities.Team>>, int)> GetPaginatedTeamsUntrackedAsync(
        PaginationInputParam paginationInputParam,
        TeamSearchCriteria searchCriteria,
        ICollection<TeamOrder> orderByFields,
        CancellationToken cancellationToken) =>
        await DbContext.Team
            .AddSearchCriteria(searchCriteria)
            .AddDependentObjects(false)
            .ToPaginatedAsync(paginationInputParam, GetPaginationFields(orderByFields), cancellationToken);

    private static List<KeysetPaginationField<Database.Entities.Team>> GetPaginationFields(ICollection<TeamOrder> orderByFields)
    {
        if (orderByFields.Count == 0)
        {
            return
            [
                KeysetPaginationField<Database.Entities.Team>.Create(
                    nameof(Database.Entities.Team.Name),
                    query => query.Name,
                    OrderDirection.Ascending)
            ];
        }

        return orderByFields.Select(orderField => orderField.Field switch
            {
                TeamOrderField.Name => KeysetPaginationField<Database.Entities.Team>.Create(
                    nameof(Database.Entities.Team.Name),
                    query => query.Name,
                    orderField.Direction),
                TeamOrderField.About => KeysetPaginationField<Database.Entities.Team>.Create(
                    nameof(Database.Entities.Team.About),
                    query => query.About,
                    orderField.Direction),
                _ => throw new ArgumentOutOfRangeException()
            })
            .ToList();
    }
}
