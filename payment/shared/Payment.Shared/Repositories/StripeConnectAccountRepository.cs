using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Pagination;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Payment.Shared.Database;
using Payment.Shared.Models;
using Customer = Payment.Shared.Database.Entities.Customer;
using StripeConnectAccount = Payment.Shared.Database.Entities.StripeConnectAccount;

namespace Payment.Shared.Repositories;

public interface IStripeConnectAccountRepository : IRepository<StripeConnectAccount>
{
    Task<StripeConnectAccount?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<StripeConnectAccount?> GetByStripeAccountIdAsync(string id, CancellationToken cancellationToken);
    Task<ICollection<StripeConnectAccount>> GetByIdsAsync(ICollection<string> ids, CancellationToken cancellationToken);
    Task<ICollection<StripeConnectAccount>> GetAllAsync(CancellationToken cancellationToken);
    StripeConnectAccount Add(StripeConnectAccount stripeConnectAccount);
    StripeConnectAccount Update(StripeConnectAccount stripeConnectAccount);
    StripeConnectAccount Remove(StripeConnectAccount stripeConnectAccount);
    void RemoveRange(ICollection<StripeConnectAccount> organizationStripeConnectAccounts);

    Task<(PaginatedInfo, ICollection<Edge<StripeConnectAccount>>, int)> GetPaginatedAccountsAsync(
        PaginationInputParam paginationInputParam,
        OrganizationStripeConnectAccountSearchCriteria searchCriteria,
        ICollection<OrganizationStripeConnectAccountOrder> orderByFields,
        CancellationToken cancellationToken);
}

internal static class OrganizationStripeConnectAccountExtensions
{
    internal static IIncludableQueryable<StripeConnectAccount, Customer> AddDependentObjects(this IQueryable<StripeConnectAccount> originalQuery) =>
        originalQuery
            .Include(query => query.StripeConnectAccountAuthorization)
            .Include(query => query.Organization)
            .ThenInclude(query => query.OrganizationMembers.Where(organizationMember => !organizationMember.DeletedAt.HasValue))
            .ThenInclude(query => query.Customer);

    internal static IQueryable<StripeConnectAccount> AddSearchCriteria(
        this IQueryable<StripeConnectAccount> query,
        OrganizationStripeConnectAccountSearchCriteria searchCriteria)
    {
        query = query.Where(item =>
            !item.DeletedAt.HasValue && item.Organization != null && item.Organization.Id == searchCriteria.OrganizationId);

        if (!string.IsNullOrWhiteSpace(searchCriteria.NameContains))
        {
            query = query.Where(item => EF.Functions.ILike(item.Name, $"%{searchCriteria.NameContains}%"));
        }

        if (searchCriteria.OnboardingCompleted is not null)
        {
            query = searchCriteria.OnboardingCompleted.Value
                ? query.Where(item =>
                    item.DetailsSubmitted && item.StripeConnectAccountAuthorization != null && item.StripeConnectAccountAuthorization.IsAuthorized &&
                    item.ChargesEnabled && item.PayoutsEnabled)
                : query.Where(item =>
                    !item.DetailsSubmitted || item.StripeConnectAccountAuthorization == null ||
                    !item.StripeConnectAccountAuthorization.IsAuthorized || !item.ChargesEnabled || !item.PayoutsEnabled);
        }

        return query;
    }

    internal static IQueryable<StripeConnectAccount> AddSortingOrders(
        this IQueryable<StripeConnectAccount> originalQuery,
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

public class StripeConnectAccountRepository(PaymentDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<PaymentDbContext, StripeConnectAccount>(dbContext, timeProvider), IStripeConnectAccountRepository
{
    public async Task<StripeConnectAccount?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.StripeConnectAccount
            .AddDependentObjects()
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<StripeConnectAccount?> GetByStripeAccountIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.StripeConnectAccount
            .AddDependentObjects()
            .FirstOrDefaultAsync(query => query.StripeAccountId == id, cancellationToken);

    public async Task<ICollection<StripeConnectAccount>> GetByIdsAsync(ICollection<string> ids, CancellationToken cancellationToken) =>
        await DbContext.StripeConnectAccount.Where(query => ids.Contains(query.Id)).AddDependentObjects().ToListAsync(cancellationToken);

    public async Task<ICollection<StripeConnectAccount>> GetAllAsync(CancellationToken cancellationToken) =>
        await DbContext.StripeConnectAccount
            .Where(query => !query.DeletedAt.HasValue)
            .AddDependentObjects()
            .ToListAsync(cancellationToken);

    public StripeConnectAccount Add(StripeConnectAccount stripeConnectAccount)
    {
        var now = TimeProvider.GetUtcNow();
        stripeConnectAccount.CreatedAt = now;
        return DbContext.StripeConnectAccount.Add(stripeConnectAccount).Entity;
    }

    public StripeConnectAccount Update(StripeConnectAccount stripeConnectAccount)
    {
        var now = TimeProvider.GetUtcNow();
        stripeConnectAccount.ModifiedAt = now;
        return DbContext.StripeConnectAccount.Update(stripeConnectAccount).Entity;
    }

    public StripeConnectAccount Remove(StripeConnectAccount stripeConnectAccount)
    {
        var now = TimeProvider.GetUtcNow();
        stripeConnectAccount.DeletedAt = now;
        return DbContext.StripeConnectAccount.Update(stripeConnectAccount).Entity;
    }

    public void RemoveRange(ICollection<StripeConnectAccount> organizationStripeConnectAccounts)
    {
        var now = TimeProvider.GetUtcNow();
        organizationStripeConnectAccounts.ForEach(organizationStripeConnectAccount => organizationStripeConnectAccount.DeletedAt = now);
        DbContext.StripeConnectAccount.UpdateRange(organizationStripeConnectAccounts);
    }

    public async Task<(PaginatedInfo, ICollection<Edge<StripeConnectAccount>>, int)> GetPaginatedAccountsAsync(
        PaginationInputParam paginationInputParam,
        OrganizationStripeConnectAccountSearchCriteria searchCriteria,
        ICollection<OrganizationStripeConnectAccountOrder> orderByFields,
        CancellationToken cancellationToken) =>
        (await DbContext.StripeConnectAccount
            .AddSearchCriteria(searchCriteria)
            .AddSortingOrders(orderByFields)
            .AddDependentObjects()
            .ToListAsync(cancellationToken))
        .ToPaginated(paginationInputParam);
}
