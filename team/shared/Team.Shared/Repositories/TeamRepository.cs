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
    Task<Database.Entities.Team> UpsertNakedAsync(string id, CancellationToken cancellationToken);
    Task<Database.Entities.Team?> GetByIdAsync(string id, CancellationToken cancellationToken);

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
            .Include(query => query.Organization)
            .ThenInclude(query =>
                query.OrganizationMembers.Where(organizationMember => !organizationMember.DeletedAt.HasValue))
            .ThenInclude(query => query.Customer)
            .Include(query => query.TeamMembers.Where(teamMember => !teamMember.DeletedAt.HasValue))
            .ThenInclude(query => query.Customer)
            .Include(query => query.TeamMembers.Where(teamMember => !teamMember.DeletedAt.HasValue))
            .ThenInclude(query => query.OrganizationMember)
            .ThenInclude(query => query.Organization)
            .ThenInclude(query =>
                query.OrganizationMembers.Where(organizationMember => !organizationMember.DeletedAt.HasValue))
            .Include(query => query.TeamMembers.Where(teamMember => !teamMember.DeletedAt.HasValue))
            .ThenInclude(query => query.OrganizationMember)
            .ThenInclude(query => query.Customer);

    internal static IQueryable<Database.Entities.Team> AddSearchCriteria(
        this IQueryable<Database.Entities.Team> query,
        TeamSearchCriteria searchCriteria)
    {
        query = query.Where(item => !item.DeletedAt.HasValue);

        if (string.IsNullOrWhiteSpace(searchCriteria.OrganizationId))
        {
            if (!string.IsNullOrWhiteSpace(searchCriteria.CustomerId))
            {
                query = query.Where(item =>
                    item.TeamMembers.Any(teamMember =>
                        !teamMember.DeletedAt.HasValue && teamMember.Customer.Id == searchCriteria.CustomerId));
            }
        }
        else
        {
            query = string.IsNullOrWhiteSpace(searchCriteria.CustomerId)
                ? query.Where(item =>
                    item.Organization != null && item.Organization.Id == searchCriteria.OrganizationId &&
                    item.Organization.OrganizationMembers.Any(organizationMember =>
                        !organizationMember.DeletedAt.HasValue))
                : query.Where(item =>
                    item.Organization != null && item.Organization.Id == searchCriteria.OrganizationId &&
                    item.Organization.OrganizationMembers.Any(organizationMember =>
                        !organizationMember.DeletedAt.HasValue &&
                        organizationMember.Customer.Id == searchCriteria.CustomerId));
        }

        if (!string.IsNullOrWhiteSpace(searchCriteria.NameContains))
        {
            query = query.Where(item => EF.Functions.ILike(item.Name, $"%{searchCriteria.NameContains}%"));
        }

        return query;
    }

    internal static IQueryable<Database.Entities.Team> AddSortingOrders(
        this IQueryable<Database.Entities.Team> originalQuery,
        ICollection<TeamOrder> orderByFields)
    {
        if (orderByFields.Count == 0)
        {
            return originalQuery.OrderBy(query => query.CreatedAt);
        }

        var orderByField = orderByFields.First();
        return orderByFields.Skip(1).Aggregate(orderByField.Field switch
        {
            TeamOrderField.Name => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.Name)
                : originalQuery.OrderByDescending(x => x.Name),
            _ => throw new ArgumentOutOfRangeException()
        }, (query, orderField) =>
            orderField.Field switch
            {
                TeamOrderField.Name => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.Name)
                    : query.ThenByDescending(x => x.Name),
                _ => throw new ArgumentOutOfRangeException()
            });
    }

    public static IQueryable<Database.Entities.Team> ApplyPaginationFilters(
        this IQueryable<Database.Entities.Team> query,
        PaginationInputParam paginationInputParam,
        ICollection<TeamOrder> orderByFields)
    {
        var orderByField = orderByFields.FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(paginationInputParam.After))
        {
            query = orderByField?.Field switch
            {
                TeamOrderField.Name => orderByField.Direction == OrderDirection.Ascending
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
                TeamOrderField.Name => orderByField.Direction == OrderDirection.Ascending
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

    public static ICollection<Edge<Database.Entities.Team>> ToEdges(
        this ICollection<Database.Entities.Team> items,
        ICollection<TeamOrder> orderByFields) =>
        items.Select(item => orderByFields.FirstOrDefault()?.Field switch
        {
            TeamOrderField.Name => new Edge<Database.Entities.Team>(item.Name.ToCursor(), item),
            null => new Edge<Database.Entities.Team>(item.CreatedAt.ToCursor(), item),
            _ => new Edge<Database.Entities.Team>(item.CreatedAt.ToCursor(), item)
        }).ToList();
}

public class TeamRepository(TeamDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<TeamDbContext, Database.Entities.Team>(dbContext), ITeamRepository
{
    public async Task<Database.Entities.Team> UpsertNakedAsync(string id, CancellationToken cancellationToken)
    {
        var existing = await GetByIdAsync(id, cancellationToken);
        if (existing is not null)
        {
            return existing;
        }

        var now = timeProvider.GetUtcNow();
        return DbContext.Team.Add(new Database.Entities.Team { Id = id, CreatedAt = now }).Entity;
    }

    public Database.Entities.Team Add(Database.Entities.Team organization)
    {
        var now = timeProvider.GetUtcNow();
        organization.CreatedAt = now;
        return DbContext.Team.Add(organization).Entity;
    }

    public Database.Entities.Team Update(Database.Entities.Team organization)
    {
        var now = timeProvider.GetUtcNow();
        organization.ModifiedAt = now;
        return DbContext.Team.Update(organization).Entity;
    }

    public async Task<Database.Entities.Team?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Team
            .Where(query => query.Id == id)
            .AddDependentObjects()
            .OrderBy(query => query.Id)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<IEnumerable<Database.Entities.Team>> GetByCustomerIdAsync(
        string customerId,
        string? organizationId,
        CancellationToken cancellationToken)
    {
        var query = DbContext.Team
            .Where(location =>
                !location.DeletedAt.HasValue && ((location.Organization == null &&
                                                  location.TeamMembers.Any(item =>
                                                      item.Customer.Id == customerId)) ||
                                                 (location.Organization != null &&
                                                  location.Organization.OrganizationMembers.Any(organizationMember =>
                                                      organizationMember.Customer.Id == customerId))));

        if (!string.IsNullOrWhiteSpace(organizationId))
        {
            query = query.Where(team =>
                team.Organization != null && team.Organization.Id == organizationId);
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
        var now = timeProvider.GetUtcNow();
        organization.DeletedAt = now;
        return DbContext.Team.Update(organization).Entity;
    }

    public async Task<(PaginatedInfo, ICollection<Edge<Database.Entities.Team>>, int)> GetPaginatedTeamsAsync(
        PaginationInputParam paginationInputParam,
        TeamSearchCriteria searchCriteria,
        ICollection<TeamOrder> orderByFields,
        CancellationToken cancellationToken)
    {
        var totalCount = await DbContext.Team.AsQueryable().AddSearchCriteria(searchCriteria)
            .CountAsync(cancellationToken);
        if (totalCount == 0)
        {
            return (new PaginatedInfo(false, false, null, null), [], totalCount);
        }

        var (paginatedInfo, edges) = (await DbContext.Team
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
