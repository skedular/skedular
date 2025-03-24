using Enterprise.Shared.Database;
using Enterprise.Shared.Models;
using Enterprise.Shared.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Team.Shared.Database;
using Team.Shared.Models;
using Customer = Team.Shared.Database.Entities.Customer;

namespace Team.Shared.Repositories;

public interface ITeamRepository : IRepository<Database.Entities.Team>
{
    Task<Database.Entities.Team?> GetByIdAsync(string id, CancellationToken cancellationToken);

    Task<ICollection<Database.Entities.Team>> GetByIdsAsync(
        ICollection<string> ids,
        CancellationToken cancellationToken);

    Task<IEnumerable<Database.Entities.Team>> GetByCustomerIdAsync(
        string customerId,
        string? organizationId,
        CancellationToken cancellationToken);

    Task<ICollection<Database.Entities.Team>> GetAllAsync(CancellationToken cancellationToken);
    Database.Entities.Team Add(Database.Entities.Team organization);
    Database.Entities.Team Update(Database.Entities.Team organization);
    Database.Entities.Team Remove(Database.Entities.Team organization);

    Task<(PaginatedInfo, ICollection<Edge<Database.Entities.Team>>, int)> GetPaginatedTeamsAsync(
        PaginationInputParam paginationInputParam,
        TeamSearchCriteria searchCriteria,
        ICollection<TeamOrder> orderByFields,
        CancellationToken cancellationToken);
}

internal static class TeamExtensions
{
    internal static IIncludableQueryable<Database.Entities.Team, Customer> AddDependentObjects(
        this IQueryable<Database.Entities.Team> originalQuery) =>
        originalQuery
            .Include(query => query.PrimaryLocation)
            .Include(query => query.Organization)
            .ThenInclude(query => query.OrganizationMembers.Where(organizationMember => !organizationMember.DeletedAt.HasValue))
            .ThenInclude(query => query.Customer)
            .Include(query => query.TeamMembers.Where(teamMember => !teamMember.DeletedAt.HasValue))
            .ThenInclude(query => query.Customer)
            .Include(query => query.TeamMembers.Where(teamMember => !teamMember.DeletedAt.HasValue))
            .ThenInclude(query => query.OrganizationMember)
            .ThenInclude(query => query.Organization)
            .ThenInclude(query => query.OrganizationMembers.Where(organizationMember => !organizationMember.DeletedAt.HasValue))
            .Include(query => query.TeamMembers.Where(teamMember => !teamMember.DeletedAt.HasValue))
            .ThenInclude(query => query.OrganizationMember)
            .ThenInclude(query => query.Customer);

    internal static IQueryable<Database.Entities.Team> AddSearchCriteria(
        this IQueryable<Database.Entities.Team> query,
        TeamSearchCriteria searchCriteria)
    {
        query = query.Where(item => !item.DeletedAt.HasValue);

        if (!string.IsNullOrWhiteSpace(searchCriteria.OrganizationId))
        {
            query = query.Where(item => item.Organization.Id == searchCriteria.OrganizationId);
        }

        if (!string.IsNullOrWhiteSpace(searchCriteria.CustomerId))
        {
            query = query.Where(item =>
                item.TeamMembers.Any(teamMember => !teamMember.DeletedAt.HasValue && teamMember.Customer.Id == searchCriteria.CustomerId));
        }

        if (!string.IsNullOrWhiteSpace(searchCriteria.NameContains))
        {
            query = query.Where(item => EF.Functions.ILike(item.Name, $"%{searchCriteria.NameContains}%"));
        }

        if (searchCriteria.PrimaryLocationIds.Count != 0)
        {
            query = query.Where(item => item.PrimaryLocation != null && searchCriteria.PrimaryLocationIds.Contains(item.PrimaryLocation.Id));
        }

        return query;
    }

    internal static IQueryable<Database.Entities.Team> AddSortingOrders(
        this IQueryable<Database.Entities.Team> originalQuery,
        ICollection<TeamOrder> orderByFields)
    {
        if (orderByFields.Count == 0)
        {
            return originalQuery.OrderBy(query => query.Name).ThenBy(query => query.Id);
        }

        var orderByField = orderByFields.First();
        return orderByFields.Skip(1).Aggregate(orderByField.Field switch
        {
            TeamOrderField.Name => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.Name)
                : originalQuery.OrderByDescending(x => x.Name),
            TeamOrderField.About => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.About)
                : originalQuery.OrderByDescending(x => x.About),
            _ => throw new ArgumentOutOfRangeException()
        }, (query, orderField) =>
            orderField.Field switch
            {
                TeamOrderField.Name => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.Name)
                    : query.ThenByDescending(x => x.Name),
                _ => throw new ArgumentOutOfRangeException()
            }).ThenBy(query => query.Id);
    }
}

public class TeamRepository(TeamDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<TeamDbContext, Database.Entities.Team>(dbContext, timeProvider), ITeamRepository
{
    public Database.Entities.Team Add(Database.Entities.Team organization)
    {
        var now = TimeProvider.GetUtcNow();
        organization.CreatedAt = now;
        return DbContext.Team.Add(organization).Entity;
    }

    public Database.Entities.Team Update(Database.Entities.Team organization)
    {
        var now = TimeProvider.GetUtcNow();
        organization.ModifiedAt = now;
        return DbContext.Team.Update(organization).Entity;
    }

    public async Task<Database.Entities.Team?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Team
            .AddDependentObjects()
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<ICollection<Database.Entities.Team>> GetByIdsAsync(
        ICollection<string> ids,
        CancellationToken cancellationToken) =>
        await DbContext.Team
            .Where(query => ids.Contains(query.Id))
            .AddDependentObjects()
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Database.Entities.Team>> GetByCustomerIdAsync(
        string customerId,
        string? organizationId,
        CancellationToken cancellationToken)
    {
        var query = DbContext.Team
            .Where(team =>
                !team.DeletedAt.HasValue && team.TeamMembers.Any(item => item.Customer.Id == customerId));

        if (!string.IsNullOrWhiteSpace(organizationId))
        {
            query = query.Where(team => !team.Organization.DeletedAt.HasValue && team.Organization.Id == organizationId);
        }

        return await query
            .AddDependentObjects()
            .ToListAsync(cancellationToken);
    }

    public async Task<ICollection<Database.Entities.Team>> GetAllAsync(CancellationToken cancellationToken) =>
        await DbContext.Team
            .Where(query => !query.DeletedAt.HasValue)
            .AddDependentObjects()
            .ToListAsync(cancellationToken);

    public Database.Entities.Team Remove(Database.Entities.Team organization)
    {
        var now = TimeProvider.GetUtcNow();
        organization.DeletedAt = now;
        return DbContext.Team.Update(organization).Entity;
    }

    public async Task<(PaginatedInfo, ICollection<Edge<Database.Entities.Team>>, int)> GetPaginatedTeamsAsync(
        PaginationInputParam paginationInputParam,
        TeamSearchCriteria searchCriteria,
        ICollection<TeamOrder> orderByFields,
        CancellationToken cancellationToken) =>
        (await DbContext.Team
            .AddSearchCriteria(searchCriteria)
            .AddSortingOrders(orderByFields)
            .AddDependentObjects()
            .ToListAsync(cancellationToken))
        .ToPaginated(paginationInputParam);
}
