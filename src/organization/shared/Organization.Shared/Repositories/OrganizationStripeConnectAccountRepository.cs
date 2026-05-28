using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
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
    Task<IReadOnlyList<OrganizationStripeConnectAccount>> GetByIdsAsync(IReadOnlyList<string> ids, CancellationToken cancellationToken);
    OrganizationStripeConnectAccount Add(OrganizationStripeConnectAccount stripeConnectAccount);
    OrganizationStripeConnectAccount Update(OrganizationStripeConnectAccount stripeConnectAccount);
    OrganizationStripeConnectAccount Remove(OrganizationStripeConnectAccount stripeConnectAccount);
    void RemoveRange(IEnumerable<OrganizationStripeConnectAccount> organizationStripeConnectAccounts);

    Task<(PaginatedInfo, IReadOnlyList<Edge<OrganizationStripeConnectAccount>>, int)> GetPaginatedAccountsAsync(
        PaginationInputParam paginationInputParam,
        OrganizationStripeConnectAccountSearchCriteria searchCriteria,
        IEnumerable<OrganizationStripeConnectAccountOrder> orderByFields,
        CancellationToken cancellationToken);
}

public static class OrganizationStripeConnectAccountExtensions
{
    extension(IQueryable<OrganizationStripeConnectAccount> originalQuery)
    {
        public IIncludableQueryable<OrganizationStripeConnectAccount, Database.Entities.Organization> AddDependentObjects() =>
            originalQuery
                .AsSingleQuery()
                .Include(query => query.OrganizationStripeConnectAccountAuthorization)
                .Include(query => query.Organization);

        public IQueryable<OrganizationStripeConnectAccount> AddSearchCriteria(OrganizationStripeConnectAccountSearchCriteria searchCriteria)
        {
            if (!string.IsNullOrWhiteSpace(searchCriteria.OrganizationId))
            {
                originalQuery = originalQuery.Where(item =>
                    !item.DeletedAt.HasValue && item.Organization.Id == searchCriteria.OrganizationId);
            }
            else if (!string.IsNullOrWhiteSpace(searchCriteria.OrganizationCustomDomain))
            {
                originalQuery = originalQuery.Where(item =>
                    !item.DeletedAt.HasValue && item.Organization.CustomDomain != null &&
                    item.Organization.CustomDomain == searchCriteria.OrganizationCustomDomain);
            }

            if (!string.IsNullOrWhiteSpace(searchCriteria.NameContains))
            {
                originalQuery = originalQuery.Where(item => EF.Functions.ILike(item.Name, $"%{searchCriteria.NameContains}%"));
            }

            if (searchCriteria.OnboardingCompleted is not null)
            {
                originalQuery = searchCriteria.OnboardingCompleted.Value
                    ? originalQuery.Where(item =>
                        item.DetailsSubmitted && item.OrganizationStripeConnectAccountAuthorization != null &&
                        item.OrganizationStripeConnectAccountAuthorization.IsAuthorized &&
                        item.ChargesEnabled && item.PayoutsEnabled)
                    : originalQuery.Where(item =>
                        !item.DetailsSubmitted || item.OrganizationStripeConnectAccountAuthorization == null ||
                        !item.OrganizationStripeConnectAccountAuthorization.IsAuthorized || !item.ChargesEnabled || !item.PayoutsEnabled);
            }

            return originalQuery;
        }
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

    public async Task<IReadOnlyList<OrganizationStripeConnectAccount>>
        GetByIdsAsync(IReadOnlyList<string> ids, CancellationToken cancellationToken) =>
        await DbContext.OrganizationStripeConnectAccount.Where(query => ids.Contains(query.Id)).AddDependentObjects().ToListAsync(cancellationToken);

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

    public void RemoveRange(IEnumerable<OrganizationStripeConnectAccount> organizationStripeConnectAccounts)
    {
        var now = TimeProvider.GetUtcNow();
        DbContext.OrganizationStripeConnectAccount.UpdateRange(organizationStripeConnectAccounts.Select(item =>
        {
            item.DeletedAt = now;
            return item;
        }));
    }

    public async Task<(PaginatedInfo, IReadOnlyList<Edge<OrganizationStripeConnectAccount>>, int)> GetPaginatedAccountsAsync(
        PaginationInputParam paginationInputParam,
        OrganizationStripeConnectAccountSearchCriteria searchCriteria,
        IEnumerable<OrganizationStripeConnectAccountOrder> orderByFields,
        CancellationToken cancellationToken) =>
        await DbContext.OrganizationStripeConnectAccount
            .AddSearchCriteria(searchCriteria)
            .AddDependentObjects()
            .ToPaginatedAsync(paginationInputParam, GetPaginationFields(orderByFields), cancellationToken);

    private static List<KeysetPaginationField<OrganizationStripeConnectAccount>> GetPaginationFields(
        IEnumerable<OrganizationStripeConnectAccountOrder> orderByFields)
    {
        if (!orderByFields.Any())
        {
            return
            [
                KeysetPaginationField<OrganizationStripeConnectAccount>.Create(
                    nameof(OrganizationStripeConnectAccount.Name),
                    query => query.Name,
                    OrderDirection.Ascending)
            ];
        }

        return orderByFields.Select(orderField => orderField.Field switch
            {
                OrganizationStripeConnectAccountOrderField.Name => KeysetPaginationField<OrganizationStripeConnectAccount>.Create(
                    nameof(OrganizationStripeConnectAccount.Name),
                    query => query.Name,
                    orderField.Direction),
                _ => throw new ArgumentOutOfRangeException()
            })
            .ToList();
    }
}
