using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
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
    Task<IReadOnlyList<OrganizationBankAccount>> GetByIdsAsync(IReadOnlyList<string> ids, CancellationToken cancellationToken);
    OrganizationBankAccount Add(OrganizationBankAccount stripeConnectAccount);
    OrganizationBankAccount Update(OrganizationBankAccount stripeConnectAccount);
    OrganizationBankAccount Remove(OrganizationBankAccount stripeConnectAccount);
    void RemoveRange(IEnumerable<OrganizationBankAccount> organizationBankAccounts);

    Task<(PaginatedInfo, IReadOnlyList<Edge<OrganizationBankAccount>>, int)> GetPaginatedBankAccountsAsync(
        PaginationInputParam paginationInputParam,
        OrganizationBankAccountSearchCriteria searchCriteria,
        IEnumerable<OrganizationBankAccountOrder> orderByFields,
        CancellationToken cancellationToken);
}

public static class OrganizationBankAccountExtensions
{
    extension(IQueryable<OrganizationBankAccount> originalQuery)
    {
        public IIncludableQueryable<OrganizationBankAccount, Database.Entities.Organization> AddDependentObjects() =>
            originalQuery
                .AsSingleQuery()
                .Include(query => query.Organization);

        public IQueryable<OrganizationBankAccount> AddSearchCriteria(OrganizationBankAccountSearchCriteria searchCriteria)
        {
            if (!string.IsNullOrWhiteSpace(searchCriteria.OrganizationId))
            {
                originalQuery = originalQuery.Where(item => !item.DeletedAt.HasValue && item.Organization.Id == searchCriteria.OrganizationId);
            }

            if (!string.IsNullOrWhiteSpace(searchCriteria.OrganizationCustomDomain))
            {
                originalQuery = originalQuery.Where(item =>
                    !item.DeletedAt.HasValue && item.Organization.CustomDomain != null &&
                    item.Organization.CustomDomain == searchCriteria.OrganizationCustomDomain);
            }

            if (!string.IsNullOrWhiteSpace(searchCriteria.NameContains))
            {
                originalQuery = originalQuery.Where(item => EF.Functions.ILike(item.Name, $"%{searchCriteria.NameContains}%"));
            }

            return originalQuery;
        }

        public IQueryable<OrganizationBankAccount> AddSortingOrders(IEnumerable<OrganizationBankAccountOrder> orderByFields)
        {
            if (!orderByFields.Any())
            {
                return originalQuery.OrderBy(query => query.Name).ThenBy(query => query.Id);
            }

            var orderByField = orderByFields.First();
            return orderByFields.Skip(1).Aggregate(orderByField.Field switch
            {
                OrganizationBankAccountOrderField.Name => orderByField.Direction == OrderDirection.Ascending
                    ? originalQuery.OrderBy(x => x.Name)
                    : originalQuery.OrderByDescending(x => x.Name),
                _ => throw new ArgumentOutOfRangeException(null,
                    "Unexpected value encountered. Update enum mapping or caller input to include this case.")
            }, (query, orderField) =>
                orderField.Field switch
                {
                    OrganizationBankAccountOrderField.Name => orderField.Direction == OrderDirection.Ascending
                        ? query.ThenBy(x => x.Name)
                        : query.ThenByDescending(x => x.Name),
                    _ => throw new ArgumentOutOfRangeException(null,
                        "Unexpected value encountered. Update enum mapping or caller input to include this case.")
                }).ThenBy(query => query.Id);
        }
    }
}

public class OrganizationBankAccountRepository(OrganizationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<OrganizationDbContext, OrganizationBankAccount>(dbContext, timeProvider), IOrganizationBankAccountRepository
{
    public async Task<OrganizationBankAccount?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.OrganizationBankAccount
            .AddDependentObjects()
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<IReadOnlyList<OrganizationBankAccount>> GetByIdsAsync(IReadOnlyList<string> ids, CancellationToken cancellationToken) =>
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

    public void RemoveRange(IEnumerable<OrganizationBankAccount> organizationBankAccounts)
    {
        var now = TimeProvider.GetUtcNow();
        DbContext.OrganizationBankAccount.UpdateRange(organizationBankAccounts.Select(item =>
        {
            item.DeletedAt = now;
            return item;
        }));
    }

    public async Task<(PaginatedInfo, IReadOnlyList<Edge<OrganizationBankAccount>>, int)> GetPaginatedBankAccountsAsync(
        PaginationInputParam paginationInputParam,
        OrganizationBankAccountSearchCriteria searchCriteria,
        IEnumerable<OrganizationBankAccountOrder> orderByFields,
        CancellationToken cancellationToken) =>
        await DbContext.OrganizationBankAccount
            .AddSearchCriteria(searchCriteria)
            .AddDependentObjects()
            .ToPaginatedAsync(paginationInputParam, GetPaginationFields(orderByFields), cancellationToken);

    private static List<KeysetPaginationField<OrganizationBankAccount>> GetPaginationFields(
        IEnumerable<OrganizationBankAccountOrder> orderByFields)
    {
        if (!orderByFields.Any())
        {
            return
            [
                KeysetPaginationField<OrganizationBankAccount>.Create(
                    nameof(OrganizationBankAccount.Name),
                    query => query.Name,
                    OrderDirection.Ascending)
            ];
        }

        return orderByFields.Select(orderField => orderField.Field switch
            {
                OrganizationBankAccountOrderField.Name => KeysetPaginationField<OrganizationBankAccount>.Create(
                    nameof(OrganizationBankAccount.Name),
                    query => query.Name,
                    orderField.Direction),
                _ => throw new ArgumentOutOfRangeException(null,
                    "Unexpected value encountered. Update enum mapping or caller input to include this case.")
            })
            .ToList();
    }
}
