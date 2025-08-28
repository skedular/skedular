using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Pagination;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Organization.Shared.Database;
using Organization.Shared.Models;
using OrganizationBankAccount = Organization.Shared.Database.Entities.OrganizationBankAccount;

namespace Organization.Shared.Repositories;

public interface IOrganizationBankAccountRepository : IRepository<OrganizationBankAccount>
{
    Task<OrganizationBankAccount?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<ICollection<OrganizationBankAccount>> GetByIdsAsync(ICollection<string> ids, CancellationToken cancellationToken);
    OrganizationBankAccount Add(OrganizationBankAccount stripeConnectAccount);
    OrganizationBankAccount Update(OrganizationBankAccount stripeConnectAccount);
    OrganizationBankAccount Remove(OrganizationBankAccount stripeConnectAccount);
    void RemoveRange(ICollection<OrganizationBankAccount> organizationBankAccounts);

    Task<(PaginatedInfo, ICollection<Edge<OrganizationBankAccount>>, int)> GetPaginatedBankAccountsAsync(
        PaginationInputParam paginationInputParam,
        OrganizationBankAccountSearchCriteria searchCriteria,
        ICollection<OrganizationBankAccountOrder> orderByFields,
        CancellationToken cancellationToken);
}

internal static class OrganizationBankAccountExtensions
{
    internal static IIncludableQueryable<OrganizationBankAccount, Database.Entities.Organization> AddDependentObjects(
        this IQueryable<OrganizationBankAccount> originalQuery) =>
        originalQuery
            .Include(query => query.Organization);

    internal static IQueryable<OrganizationBankAccount> AddSearchCriteria(
        this IQueryable<OrganizationBankAccount> query,
        OrganizationBankAccountSearchCriteria searchCriteria)
    {
        if (!string.IsNullOrWhiteSpace(searchCriteria.OrganizationId))
        {
            query = query.Where(item => !item.DeletedAt.HasValue && item.Organization.Id == searchCriteria.OrganizationId);
        }

        if (!string.IsNullOrWhiteSpace(searchCriteria.OrganizationUniqueAlphanumericName))
        {
            query = query.Where(item =>
                !item.DeletedAt.HasValue && item.Organization.UniqueAlphanumericName != null &&
                item.Organization.UniqueAlphanumericName == searchCriteria.OrganizationUniqueAlphanumericName);
        }

        if (!string.IsNullOrWhiteSpace(searchCriteria.NameContains))
        {
            query = query.Where(item => EF.Functions.ILike(item.Name, $"%{searchCriteria.NameContains}%"));
        }

        return query;
    }

    internal static IQueryable<OrganizationBankAccount> AddSortingOrders(
        this IQueryable<OrganizationBankAccount> originalQuery,
        ICollection<OrganizationBankAccountOrder> orderByFields)
    {
        if (orderByFields.Count == 0)
        {
            return originalQuery.OrderBy(query => query.Name).ThenBy(query => query.Id);
        }

        var orderByField = orderByFields.First();
        return orderByFields.Skip(1).Aggregate(orderByField.Field switch
        {
            OrganizationBankAccountOrderField.Name => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.Name)
                : originalQuery.OrderByDescending(x => x.Name),
            _ => throw new ArgumentOutOfRangeException()
        }, (query, orderField) =>
            orderField.Field switch
            {
                OrganizationBankAccountOrderField.Name => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.Name)
                    : query.ThenByDescending(x => x.Name),
                _ => throw new ArgumentOutOfRangeException()
            }).ThenBy(query => query.Id);
    }
}

public class OrganizationBankAccountRepository(OrganizationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<OrganizationDbContext, OrganizationBankAccount>(dbContext, timeProvider), IOrganizationBankAccountRepository
{
    public async Task<OrganizationBankAccount?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.OrganizationBankAccount
            .AddDependentObjects()
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<ICollection<OrganizationBankAccount>> GetByIdsAsync(ICollection<string> ids, CancellationToken cancellationToken) =>
        await DbContext.OrganizationBankAccount.Where(query => ids.Contains(query.Id)).AddDependentObjects().ToListAsync(cancellationToken);

    public OrganizationBankAccount Add(OrganizationBankAccount stripeConnectAccount)
    {
        var now = TimeProvider.GetUtcNow();
        stripeConnectAccount.CreatedAt = now;
        return DbContext.OrganizationBankAccount.Add(stripeConnectAccount).Entity;
    }

    public OrganizationBankAccount Update(OrganizationBankAccount stripeConnectAccount)
    {
        var now = TimeProvider.GetUtcNow();
        stripeConnectAccount.ModifiedAt = now;
        return DbContext.OrganizationBankAccount.Update(stripeConnectAccount).Entity;
    }

    public OrganizationBankAccount Remove(OrganizationBankAccount stripeConnectAccount)
    {
        var now = TimeProvider.GetUtcNow();
        stripeConnectAccount.DeletedAt = now;
        return DbContext.OrganizationBankAccount.Update(stripeConnectAccount).Entity;
    }

    public void RemoveRange(ICollection<OrganizationBankAccount> organizationBankAccounts)
    {
        var now = TimeProvider.GetUtcNow();
        organizationBankAccounts.ForEach(organizationBankAccount => organizationBankAccount.DeletedAt = now);
        DbContext.OrganizationBankAccount.UpdateRange(organizationBankAccounts);
    }

    public async Task<(PaginatedInfo, ICollection<Edge<OrganizationBankAccount>>, int)> GetPaginatedBankAccountsAsync(
        PaginationInputParam paginationInputParam,
        OrganizationBankAccountSearchCriteria searchCriteria,
        ICollection<OrganizationBankAccountOrder> orderByFields,
        CancellationToken cancellationToken) =>
        (await DbContext.OrganizationBankAccount
            .AddSearchCriteria(searchCriteria)
            .AddSortingOrders(orderByFields)
            .AddDependentObjects()
            .ToListAsync(cancellationToken))
        .ToPaginated(paginationInputParam);
}
