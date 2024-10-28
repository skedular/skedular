using Enterprise.Shared.Database;
using Enterprise.Shared.Models;
using Enterprise.Shared.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Organization.Shared.Database;
using Organization.Shared.Models;
using Team = Organization.Shared.Database.Entities.Team;

namespace Organization.Shared.Repositories;

public interface IOrganizationRepository : IRepository<Database.Entities.Organization>
{
    Task<Database.Entities.Organization> UpsertNakedAsync(string id, CancellationToken cancellationToken);

    Task<Database.Entities.Organization?> GetByIdAsync(
        string id,
        CancellationToken cancellationToken);

    Task<Database.Entities.Organization?> GetByIdAsync(
        string id,
        bool includeAllOfferings,
        CancellationToken cancellationToken);

    Task<IEnumerable<Database.Entities.Organization>> GetByCustomerIdAsync(
        string customerId,
        CancellationToken cancellationToken);

    Task<Database.Entities.Organization?> GetByAzureTenantIdAsync(
        string azureTenantId,
        CancellationToken cancellationToken);

    Task<ICollection<Database.Entities.Organization>> GetAllAsync(CancellationToken cancellationToken);
    Database.Entities.Organization Add(Database.Entities.Organization organization);
    Database.Entities.Organization Update(Database.Entities.Organization organization);
    Database.Entities.Organization Remove(Database.Entities.Organization organization);

    Task<(PaginatedInfo, ICollection<Edge<Database.Entities.Organization>>, int )> GetPaginatedOrganizationsAsync(
        PaginationInputParam paginationInputParam,
        OrganizationSearchCriteria searchCriteria,
        ICollection<OrganizationOrder> orderByFields,
        CancellationToken cancellationToken);
}

internal static class OrganizationExtensions
{
    internal static IIncludableQueryable<Database.Entities.Organization, ICollection<Team>> AddDependentObjects(
        this IQueryable<Database.Entities.Organization> originalQuery,
        bool includeAllOfferings)
    {
        var updatedQuery = originalQuery
            .Include(query => query.AzureTenants.Where(azureTenant => !azureTenant.DeletedAt.HasValue))
            .ThenInclude(query =>
                query.AzureTenantMembers.Where(azureTenantMember => !azureTenantMember.DeletedAt.HasValue))
            .Include(query =>
                query.OrganizationMembers.Where(organizationMember => !organizationMember.DeletedAt.HasValue))
            .ThenInclude(query => query.Customer)
            .Include(query => query.TermsOfUse);

        return includeAllOfferings
            ? updatedQuery.Include(query => query.OrganizationOfferings
                    .OrderByDescending(organizationOffering => organizationOffering.End))
                .ThenInclude(query => query.OrganizationOfferingActiveMembers)
                .ThenInclude(query => query.OrganizationMember)
                .ThenInclude(query => query.Customer)
                .Include(query => query.IndustrySubCategories)
                .ThenInclude(query => query.IndustryMainCategory)
                .Include(query => query.Locations)
                .Include(query => query.Teams)
            : updatedQuery.Include(query => query.OrganizationOfferings
                    .Where(organizationOffering => !organizationOffering.DeletedAt.HasValue)
                    .OrderByDescending(organizationOffering => organizationOffering.End)
                    .Take(1))
                .ThenInclude(query => query.OrganizationOfferingActiveMembers)
                .ThenInclude(query => query.OrganizationMember)
                .ThenInclude(query => query.Customer)
                .Include(query => query.IndustrySubCategories)
                .ThenInclude(query => query.IndustryMainCategory)
                .Include(query => query.Locations)
                .Include(query => query.Teams);
    }

    internal static IQueryable<Database.Entities.Organization> AddSearchCriteria(
        this IQueryable<Database.Entities.Organization> query,
        OrganizationSearchCriteria searchCriteria)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(searchCriteria.CustomerId);

        query = query.Where(item => !item.DeletedAt.HasValue);
        query = query.Where(item => item.OrganizationMembers.Any(organizationMember =>
            !organizationMember.DeletedAt.HasValue &&
            organizationMember.Customer.Id ==
            searchCriteria.CustomerId));

        if (!string.IsNullOrWhiteSpace(searchCriteria.NameContains))
        {
            query = query.Where(item => EF.Functions.ILike(item.Name, $"%{searchCriteria.NameContains}%"));
        }

        return query;
    }

    internal static IQueryable<Database.Entities.Organization> AddSortingOrders(
        this IQueryable<Database.Entities.Organization> originalQuery,
        ICollection<OrganizationOrder> orderByFields)
    {
        if (orderByFields.Count == 0)
        {
            return originalQuery.OrderBy(query => query.CreatedAt);
        }

        var orderByField = orderByFields.First();
        return orderByFields.Skip(1).Aggregate(orderByField.Field switch
        {
            OrganizationOrderField.Name => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.Name)
                : originalQuery.OrderByDescending(x => x.Name),
            _ => throw new ArgumentOutOfRangeException()
        }, (query, orderField) =>
            orderField.Field switch
            {
                OrganizationOrderField.Name => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.Name)
                    : query.ThenByDescending(x => x.Name),
                _ => throw new ArgumentOutOfRangeException()
            });
    }

    public static IQueryable<Database.Entities.Organization> ApplyPaginationFilters(
        this IQueryable<Database.Entities.Organization> query,
        PaginationInputParam paginationInputParam,
        ICollection<OrganizationOrder> orderByFields)
    {
        var orderByField = orderByFields.FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(paginationInputParam.After))
        {
            query = orderByField?.Field switch
            {
                OrganizationOrderField.Name => orderByField.Direction == OrderDirection.Ascending
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
                OrganizationOrderField.Name => orderByField.Direction == OrderDirection.Ascending
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

    public static ICollection<Edge<Database.Entities.Organization>> ToEdges(
        this ICollection<Database.Entities.Organization> items,
        ICollection<OrganizationOrder> orderByFields) =>
        items.Select(item => orderByFields.FirstOrDefault()?.Field switch
        {
            OrganizationOrderField.Name => new Edge<Database.Entities.Organization>(item.Name.ToCursor(), item),
            null => new Edge<Database.Entities.Organization>(item.CreatedAt.ToCursor(), item),
            _ => new Edge<Database.Entities.Organization>(item.CreatedAt.ToCursor(), item)
        }).ToList();
}

public class OrganizationRepository(OrganizationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<OrganizationDbContext, Database.Entities.Organization>(dbContext), IOrganizationRepository
{
    public async Task<Database.Entities.Organization> UpsertNakedAsync(string id, CancellationToken cancellationToken)
    {
        var existing = await GetByIdAsync(id, cancellationToken);
        if (existing is not null)
        {
            return existing;
        }

        var now = timeProvider.GetUtcNow();
        return DbContext.Organization.Add(new Database.Entities.Organization { Id = id, CreatedAt = now }).Entity;
    }

    public Database.Entities.Organization Add(Database.Entities.Organization organization)
    {
        var now = timeProvider.GetUtcNow();
        organization.CreatedAt = now;
        return DbContext.Organization.Add(organization).Entity;
    }

    public Database.Entities.Organization Update(Database.Entities.Organization organization)
    {
        var now = timeProvider.GetUtcNow();
        organization.ModifiedAt = now;
        return DbContext.Organization.Update(organization).Entity;
    }

    public async Task<Database.Entities.Organization?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await GetByIdAsync(id, false, cancellationToken);

    public async Task<Database.Entities.Organization?> GetByIdAsync(
        string id,
        bool includeAllOfferings,
        CancellationToken cancellationToken) =>
        await DbContext.Organization
            .Where(query => query.Id == id)
            .AddDependentObjects(includeAllOfferings)
            .OrderBy(query => query.Id)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<IEnumerable<Database.Entities.Organization>> GetByCustomerIdAsync(
        string customerId,
        CancellationToken cancellationToken) =>
        await DbContext.Organization
            .Where(query => !query.DeletedAt.HasValue &&
                            query.OrganizationMembers.Select(item => item.Customer.Id).Contains(customerId))
            .AddDependentObjects(false)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public async Task<Database.Entities.Organization?> GetByAzureTenantIdAsync(
        string azureTenantId,
        CancellationToken cancellationToken) =>
        await DbContext.Organization
            .Where(query => !query.DeletedAt.HasValue &&
                            query.AzureTenants.Any(azureTenant => azureTenant.Id == azureTenantId))
            .AddDependentObjects(false)
            .OrderBy(query => query.Id)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<ICollection<Database.Entities.Organization>> GetAllAsync(CancellationToken cancellationToken) =>
        await DbContext.Organization
            .Where(query => !query.DeletedAt.HasValue)
            .AddDependentObjects(false)
            .ToListAsync(cancellationToken);

    public Database.Entities.Organization Remove(Database.Entities.Organization organization)
    {
        var now = timeProvider.GetUtcNow();
        organization.DeletedAt = now;
        return DbContext.Organization.Update(organization).Entity;
    }

    public async Task<(PaginatedInfo, ICollection<Edge<Database.Entities.Organization>>, int)>
        GetPaginatedOrganizationsAsync(
            PaginationInputParam paginationInputParam,
            OrganizationSearchCriteria searchCriteria,
            ICollection<OrganizationOrder> orderByFields,
            CancellationToken cancellationToken)
    {
        var totalCount = await DbContext.Organization.AsQueryable().AddSearchCriteria(searchCriteria)
            .CountAsync(cancellationToken);
        if (totalCount == 0)
        {
            return (new PaginatedInfo(false, false, null, null), [], totalCount);
        }

        var (paginatedInfo, edges) = (await DbContext.Organization
                .AsQueryable()
                .AddSearchCriteria(searchCriteria)
                .AddSortingOrders(orderByFields)
                .ApplyPaginationFilters(paginationInputParam, orderByFields)
                .AddDependentObjects(false)
                .ToListAsync(cancellationToken))
            .ToEdges(orderByFields)
            .GetPaginatedInfo(paginationInputParam);
        return (paginatedInfo, edges, totalCount);
    }
}
