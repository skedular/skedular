using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Pagination;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Organization.Shared.Database;
using Organization.Shared.Models;
using OrganizationStripeConnectAccount = Organization.Shared.Database.Entities.OrganizationStripeConnectAccount;

namespace Organization.Shared.Repositories;

public interface IOrganizationStripeConnectAccountRepository : IRepository<OrganizationStripeConnectAccount>
{
    Task<OrganizationStripeConnectAccount?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<OrganizationStripeConnectAccount?> GetByStripeAccountIdAsync(string id, CancellationToken cancellationToken);
    Task<ICollection<OrganizationStripeConnectAccount>> GetByIdsAsync(ICollection<string> ids, CancellationToken cancellationToken);
    Task<ICollection<OrganizationStripeConnectAccount>> GetAllAsync(CancellationToken cancellationToken);
    OrganizationStripeConnectAccount Add(OrganizationStripeConnectAccount stripeConnectAccount);
    OrganizationStripeConnectAccount Update(OrganizationStripeConnectAccount stripeConnectAccount);
    OrganizationStripeConnectAccount Remove(OrganizationStripeConnectAccount stripeConnectAccount);
    void RemoveRange(ICollection<OrganizationStripeConnectAccount> organizationStripeConnectAccounts);

    Task<(PaginatedInfo, ICollection<Edge<OrganizationStripeConnectAccount>>, int)> GetPaginatedAccountsAsync(
        PaginationInputParam paginationInputParam,
        OrganizationStripeConnectAccountSearchCriteria searchCriteria,
        ICollection<OrganizationStripeConnectAccountOrder> orderByFields,
        CancellationToken cancellationToken);
}

internal static class OrganizationStripeConnectAccountExtensions
{
    internal static IIncludableQueryable<OrganizationStripeConnectAccount, Database.Entities.Organization> AddDependentObjects(
        this IQueryable<OrganizationStripeConnectAccount> originalQuery) =>
        originalQuery
            .Include(query => query.OrganizationStripeConnectAccountAuthorization)
            .Include(query => query.Organization);

    internal static IQueryable<OrganizationStripeConnectAccount> AddSearchCriteria(
        this IQueryable<OrganizationStripeConnectAccount> query,
        OrganizationStripeConnectAccountSearchCriteria searchCriteria)
    {
        query = query.Where(item => !item.DeletedAt.HasValue && item.Organization.Id == searchCriteria.OrganizationId);

        if (!string.IsNullOrWhiteSpace(searchCriteria.NameContains))
        {
            query = query.Where(item => EF.Functions.ILike(item.Name, $"%{searchCriteria.NameContains}%"));
        }

        if (searchCriteria.OnboardingCompleted is not null)
        {
            query = searchCriteria.OnboardingCompleted.Value
                ? query.Where(item =>
                    item.DetailsSubmitted && item.OrganizationStripeConnectAccountAuthorization != null &&
                    item.OrganizationStripeConnectAccountAuthorization.IsAuthorized &&
                    item.ChargesEnabled && item.PayoutsEnabled)
                : query.Where(item =>
                    !item.DetailsSubmitted || item.OrganizationStripeConnectAccountAuthorization == null ||
                    !item.OrganizationStripeConnectAccountAuthorization.IsAuthorized || !item.ChargesEnabled || !item.PayoutsEnabled);
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

public class OrganizationStripeConnectAccountRepository(OrganizationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<OrganizationDbContext, OrganizationStripeConnectAccount>(dbContext, timeProvider), IOrganizationStripeConnectAccountRepository
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

    public OrganizationStripeConnectAccount Add(OrganizationStripeConnectAccount stripeConnectAccount)
    {
        var now = TimeProvider.GetUtcNow();
        stripeConnectAccount.CreatedAt = now;
        return DbContext.OrganizationStripeConnectAccount.Add(stripeConnectAccount).Entity;
    }

    public OrganizationStripeConnectAccount Update(OrganizationStripeConnectAccount stripeConnectAccount)
    {
        var now = TimeProvider.GetUtcNow();
        stripeConnectAccount.ModifiedAt = now;
        return DbContext.OrganizationStripeConnectAccount.Update(stripeConnectAccount).Entity;
    }

    public OrganizationStripeConnectAccount Remove(OrganizationStripeConnectAccount stripeConnectAccount)
    {
        var now = TimeProvider.GetUtcNow();
        stripeConnectAccount.DeletedAt = now;
        return DbContext.OrganizationStripeConnectAccount.Update(stripeConnectAccount).Entity;
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
