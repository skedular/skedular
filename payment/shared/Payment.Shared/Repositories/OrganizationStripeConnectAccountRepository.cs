using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Pagination;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Payment.Shared.Database;
using Payment.Shared.Models;
using Customer = Payment.Shared.Database.Entities.Customer;
using OrganizationStripeConnectAccount = Payment.Shared.Database.Entities.OrganizationStripeConnectAccount;

namespace Payment.Shared.Repositories;

public interface IOrganizationStripeConnectAccountRepository : IRepository<OrganizationStripeConnectAccount>
{
    Task<OrganizationStripeConnectAccount?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<OrganizationStripeConnectAccount?> GetByStripeAccountIdAsync(string id, CancellationToken cancellationToken);
    Task<ICollection<OrganizationStripeConnectAccount>> GetByIdsAsync(ICollection<string> ids, CancellationToken cancellationToken);
    Task<ICollection<OrganizationStripeConnectAccount>> GetAllAsync(CancellationToken cancellationToken);
    OrganizationStripeConnectAccount Add(OrganizationStripeConnectAccount organizationStripeConnectAccount);
    OrganizationStripeConnectAccount Update(OrganizationStripeConnectAccount organizationStripeConnectAccount);
    OrganizationStripeConnectAccount Remove(OrganizationStripeConnectAccount organizationStripeConnectAccount);
    void RemoveRange(ICollection<OrganizationStripeConnectAccount> organizationStripeConnectAccounts);

    Task<(PaginatedInfo, ICollection<Edge<OrganizationStripeConnectAccount>>, int)> GetPaginatedAccountsAsync(
        PaginationInputParam paginationInputParam,
        OrganizationStripeConnectAccountSearchCriteria searchCriteria,
        ICollection<OrganizationStripeConnectAccountOrder> orderByFields,
        CancellationToken cancellationToken);
}

internal static class OrganizationStripeConnectAccountExtensions
{
    internal static IIncludableQueryable<OrganizationStripeConnectAccount, Customer> AddDependentObjects(
        this IQueryable<OrganizationStripeConnectAccount> originalQuery) =>
        originalQuery
            .Include(query => query.Organization)
            .ThenInclude(query => query.OrganizationMembers.Where(organizationMember => !organizationMember.DeletedAt.HasValue))
            .ThenInclude(query => query.Customer);

    internal static IQueryable<OrganizationStripeConnectAccount> AddSearchCriteria(
        this IQueryable<OrganizationStripeConnectAccount> query,
        OrganizationStripeConnectAccountSearchCriteria searchCriteria)
    {
        query = query.Where(item => !item.DeletedAt.HasValue && item.Organization.Id == searchCriteria.OrganizationId);

        if (!string.IsNullOrWhiteSpace(searchCriteria.NameContains))
        {
            query = query.Where(item => EF.Functions.ILike(item.Name, $"%{searchCriteria.NameContains}%"));
        }

        return query;
    }

    internal static IQueryable<OrganizationStripeConnectAccount> AddSortingOrders(
        this IQueryable<OrganizationStripeConnectAccount> originalQuery,
        ICollection<OrganizationStripeConnectAccountOrder> orderByFields)
    {
        if (orderByFields.Count == 0)
        {
            return originalQuery.OrderBy(query => query.Name).ThenBy(query => query.Id);
        }

        var orderByField = orderByFields.First();
        return orderByFields.Skip(1).Aggregate(orderByField.Field switch
        {
            OrganizationStripeConnectAccountOrderField.Name => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.Name)
                : originalQuery.OrderByDescending(x => x.Name),
            _ => throw new ArgumentOutOfRangeException()
        }, (query, orderField) =>
            orderField.Field switch
            {
                OrganizationStripeConnectAccountOrderField.Name => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.Name)
                    : query.ThenByDescending(x => x.Name),
                _ => throw new ArgumentOutOfRangeException()
            }).ThenBy(query => query.Id);
    }
}

public class OrganizationStripeConnectAccountRepository(PaymentDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<PaymentDbContext, OrganizationStripeConnectAccount>(dbContext, timeProvider), IOrganizationStripeConnectAccountRepository
{
    public async Task<OrganizationStripeConnectAccount?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.OrganizationStripeConnectAccount
            .AddDependentObjects()
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<OrganizationStripeConnectAccount?> GetByStripeAccountIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.OrganizationStripeConnectAccount
            .AddDependentObjects()
            .FirstOrDefaultAsync(query => query.StripeAccountId == id, cancellationToken);

    public async Task<ICollection<OrganizationStripeConnectAccount>> GetByIdsAsync(ICollection<string> ids, CancellationToken cancellationToken) =>
        await DbContext.OrganizationStripeConnectAccount.Where(query => ids.Contains(query.Id)).AddDependentObjects().ToListAsync(cancellationToken);

    public async Task<ICollection<OrganizationStripeConnectAccount>> GetAllAsync(CancellationToken cancellationToken) =>
        await DbContext.OrganizationStripeConnectAccount
            .Where(query => !query.DeletedAt.HasValue)
            .AddDependentObjects()
            .ToListAsync(cancellationToken);

    public OrganizationStripeConnectAccount Add(OrganizationStripeConnectAccount organizationStripeConnectAccount)
    {
        var now = TimeProvider.GetUtcNow();
        organizationStripeConnectAccount.CreatedAt = now;
        return DbContext.OrganizationStripeConnectAccount.Add(organizationStripeConnectAccount).Entity;
    }

    public OrganizationStripeConnectAccount Update(OrganizationStripeConnectAccount organizationStripeConnectAccount)
    {
        var now = TimeProvider.GetUtcNow();
        organizationStripeConnectAccount.ModifiedAt = now;
        return DbContext.OrganizationStripeConnectAccount.Update(organizationStripeConnectAccount).Entity;
    }

    public OrganizationStripeConnectAccount Remove(OrganizationStripeConnectAccount organizationStripeConnectAccount)
    {
        var now = TimeProvider.GetUtcNow();
        organizationStripeConnectAccount.DeletedAt = now;
        return DbContext.OrganizationStripeConnectAccount.Update(organizationStripeConnectAccount).Entity;
    }

    public void RemoveRange(ICollection<OrganizationStripeConnectAccount> organizationStripeConnectAccounts)
    {
        var now = TimeProvider.GetUtcNow();
        organizationStripeConnectAccounts.ForEach(organizationStripeConnectAccount => organizationStripeConnectAccount.DeletedAt = now);
        DbContext.OrganizationStripeConnectAccount.UpdateRange(organizationStripeConnectAccounts);
    }

    public async Task<(PaginatedInfo, ICollection<Edge<OrganizationStripeConnectAccount>>, int)> GetPaginatedAccountsAsync(
        PaginationInputParam paginationInputParam,
        OrganizationStripeConnectAccountSearchCriteria searchCriteria,
        ICollection<OrganizationStripeConnectAccountOrder> orderByFields,
        CancellationToken cancellationToken) =>
        (await DbContext.OrganizationStripeConnectAccount
            .AddSearchCriteria(searchCriteria)
            .AddSortingOrders(orderByFields)
            .AddDependentObjects()
            .ToListAsync(cancellationToken))
        .ToPaginated(paginationInputParam);
}
