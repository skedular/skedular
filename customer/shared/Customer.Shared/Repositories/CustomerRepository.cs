using Api.Shared.Services.Cache;
using Customer.Shared.Database;
using Customer.Shared.Models;
using Enterprise.Shared.Database;
using Enterprise.Shared.Pagination;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Organization = Customer.Shared.Database.Entities.Organization;

namespace Customer.Shared.Repositories;

public interface ICustomerRepository : IRepository<Database.Entities.Customer>
{
    Task<Database.Entities.Customer?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<Database.Entities.Customer?> GetByVerifiableTokenAsync(string verifiableToken, CancellationToken cancellationToken);
    Task<Database.Entities.Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<ICollection<Database.Entities.Customer>> GetAllAsync(CancellationToken cancellationToken);
    ValueTask<Database.Entities.Customer> AddAsync(Database.Entities.Customer customer, CancellationToken cancellationToken);
    ValueTask<Database.Entities.Customer> UpdateAsync(Database.Entities.Customer customer, CancellationToken cancellationToken);

    Task<(PaginatedInfo, ICollection<Edge<Database.Entities.Customer>>, int)> GetPaginatedCustomersAsync(
        PaginationInputParam paginationInputParam,
        CustomerSearchCriteria searchCriteria,
        ICollection<CustomerOrder> orderByFields,
        CancellationToken cancellationToken);
}

internal static class CustomerExtensions
{
    internal static IIncludableQueryable<Database.Entities.Customer, Organization?> AddDependentObjects(
        this IQueryable<Database.Entities.Customer> originalQuery) =>
        originalQuery
            .Include(query => query.Identities)
            .Include(query => query.BillingDetails)
            .Include(query => query.StripeCustomer)
            .Include(query => query.StripePaymentMethods.Where(stripePaymentMethod => !stripePaymentMethod.DeletedAt.HasValue))
            .Include(query => query.DefaultOrganization)
            .Include(query => query.PreferredLocations)
            .ThenInclude(query => query.Organization)
            .Include(query => query.PreferredOrganizationTags)
            .ThenInclude(query => query.Organization)
            .Include(query => query.PreferredResources)
            .ThenInclude(query => query.Location)
            .ThenInclude(query => query!.Organization)
            .Include(query => query.PreferredTeams)
            .ThenInclude(query => query.Organization);

    internal static IQueryable<Database.Entities.Customer> AddSearchCriteria(
        this IQueryable<Database.Entities.Customer> query,
        CustomerSearchCriteria searchCriteria)
    {
        query = query.Where(item => !item.DeletedAt.HasValue);

        if (!string.IsNullOrWhiteSpace(searchCriteria.NameContains))
        {
            query = query.Where(item =>
                (item.Name != null && EF.Functions.ILike(item.Name, $"%{searchCriteria.NameContains}%")) ||
                (item.GivenName != null &&
                 EF.Functions.ILike(item.GivenName, $"%{searchCriteria.NameContains}%")) ||
                (item.MiddleName != null &&
                 EF.Functions.ILike(item.MiddleName, $"%{searchCriteria.NameContains}%")) ||
                (item.FamilyName != null &&
                 EF.Functions.ILike(item.FamilyName, $"%{searchCriteria.NameContains}%")));
        }

        if (!string.IsNullOrWhiteSpace(searchCriteria.LocationId))
        {
            query = query.Where(item => item.PreferredLocations.Select(location => location.Id).Contains(searchCriteria.LocationId));
        }

        return query;
    }

    internal static IQueryable<Database.Entities.Customer> AddSortingOrders(
        this IQueryable<Database.Entities.Customer> originalQuery,
        ICollection<CustomerOrder> orderByFields)
    {
        if (orderByFields.Count == 0)
        {
            return originalQuery.OrderBy(query => query.Name).ThenBy(query => query.Id);
        }

        var orderByField = orderByFields.First();
        return orderByFields.Skip(1).Aggregate(orderByField.Field switch
        {
            CustomerOrderField.Designation => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.Designation)
                : originalQuery.OrderByDescending(x => x.Designation),
            CustomerOrderField.Title => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.Title)
                : originalQuery.OrderByDescending(x => x.Title),
            CustomerOrderField.Name => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.Name)
                : originalQuery.OrderByDescending(x => x.Name),
            CustomerOrderField.GivenName => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.GivenName)
                : originalQuery.OrderByDescending(x => x.GivenName),
            CustomerOrderField.MiddleName => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.MiddleName)
                : originalQuery.OrderByDescending(x => x.MiddleName),
            CustomerOrderField.FamilyName => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.FamilyName)
                : originalQuery.OrderByDescending(x => x.FamilyName),
            CustomerOrderField.Timezone => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.Timezone)
                : originalQuery.OrderByDescending(x => x.Timezone),
            CustomerOrderField.Locale => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.Locale)
                : originalQuery.OrderByDescending(x => x.Locale),
            CustomerOrderField.PhoneNumber => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.PhoneNumber)
                : originalQuery.OrderByDescending(x => x.PhoneNumber),
            _ => throw new ArgumentOutOfRangeException()
        }, (query, orderField) =>
            orderField.Field switch
            {
                CustomerOrderField.Designation => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.Designation)
                    : query.ThenByDescending(x => x.Designation),
                CustomerOrderField.Title => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.Title)
                    : query.ThenByDescending(x => x.Title),
                CustomerOrderField.Name => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.Name)
                    : query.ThenByDescending(x => x.Name),
                CustomerOrderField.GivenName => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.GivenName)
                    : query.ThenByDescending(x => x.GivenName),
                CustomerOrderField.MiddleName => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.MiddleName)
                    : query.ThenByDescending(x => x.MiddleName),
                CustomerOrderField.FamilyName => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.FamilyName)
                    : query.ThenByDescending(x => x.FamilyName),
                CustomerOrderField.Timezone => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.Timezone)
                    : query.ThenByDescending(x => x.Timezone),
                CustomerOrderField.Locale => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.Locale)
                    : query.ThenByDescending(x => x.Locale),
                CustomerOrderField.PhoneNumber => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.PhoneNumber)
                    : query.ThenByDescending(x => x.PhoneNumber),
                _ => throw new ArgumentOutOfRangeException()
            }).ThenBy(query => query.Id);
    }
}

public class CustomerRepository(CustomerDbContext dbContext, TimeProvider timeProvider, IGenericCustomerCacheService genericCustomerCacheService)
    : RepositoryBase<CustomerDbContext, Database.Entities.Customer>(dbContext, timeProvider), ICustomerRepository
{
    private static readonly Func<CustomerDbContext, string, CancellationToken, Task<Database.Entities.Customer?>>
        s_getByIdQueryAsync =
            EF.CompileAsyncQuery<CustomerDbContext, string, CancellationToken, Database.Entities.Customer?>((
                    dbContext,
                    id,
                    cancellationToken) =>
                dbContext.Customer
                    .AddDependentObjects()
                    .FirstOrDefault(query => query.Id == id));

    private static readonly Func<CustomerDbContext, string, CancellationToken, Task<Database.Entities.Customer?>>
        s_getByVerifiableTokenQueryAsync =
            EF.CompileAsyncQuery<CustomerDbContext, string, CancellationToken, Database.Entities.Customer?>((
                    dbContext,
                    verifiableToken,
                    cancellationToken) =>
                dbContext.Customer
                    .AddDependentObjects()
                    .FirstOrDefault(query =>
                        !query.DeletedAt.HasValue &&
                        query.Identities.Select(identity => identity.Id).Contains(verifiableToken)));

    private static readonly Func<CustomerDbContext, string, CancellationToken, Task<Database.Entities.Customer?>>
        s_getByEmailQueryAsync =
            EF.CompileAsyncQuery<CustomerDbContext, string, CancellationToken, Database.Entities.Customer?>((
                    dbContext,
                    email,
                    cancellationToken) =>
                dbContext.Customer
                    .AddDependentObjects()
                    .FirstOrDefault(query =>
                        !query.DeletedAt.HasValue &&
                        query.Identities.Any(identity =>
                            identity.Email != null &&
                            EF.Functions.ILike(identity.Email, email))));

    public async Task<Database.Entities.Customer?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await s_getByIdQueryAsync(DbContext, id, cancellationToken);

    public async Task<Database.Entities.Customer?> GetByVerifiableTokenAsync(string verifiableToken, CancellationToken cancellationToken) =>
        await s_getByVerifiableTokenQueryAsync(DbContext, verifiableToken, cancellationToken);

    public async Task<Database.Entities.Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken) =>
        await s_getByEmailQueryAsync(DbContext, email, cancellationToken);

    public async Task<ICollection<Database.Entities.Customer>> GetAllAsync(CancellationToken cancellationToken) =>
        await DbContext.Customer
            .AddDependentObjects()
            .Where(query => !query.DeletedAt.HasValue)
            .OrderBy(query => query.Id)
            .ToListAsync(cancellationToken);

    public async ValueTask<Database.Entities.Customer> AddAsync(Database.Entities.Customer customer, CancellationToken cancellationToken)
    {
        var now = TimeProvider.GetUtcNow();
        customer.CreatedAt = now;
        customer = DbContext.Customer.Add(customer).Entity;

        await genericCustomerCacheService.InvalidateByVerifiableTokensAsync(customer.Identities.Select(identity => identity.Id), cancellationToken);

        return customer;
    }

    public async ValueTask<Database.Entities.Customer> UpdateAsync(Database.Entities.Customer customer, CancellationToken cancellationToken)
    {
        var now = TimeProvider.GetUtcNow();
        customer.ModifiedAt = now;
        customer = DbContext.Customer.Update(customer).Entity;

        await genericCustomerCacheService.InvalidateByVerifiableTokensAsync(customer.Identities.Select(identity => identity.Id), cancellationToken);

        return customer;
    }

    public async Task<(PaginatedInfo, ICollection<Edge<Database.Entities.Customer>>, int)>
        GetPaginatedCustomersAsync(
            PaginationInputParam paginationInputParam,
            CustomerSearchCriteria searchCriteria,
            ICollection<CustomerOrder> orderByFields,
            CancellationToken cancellationToken) =>
        (await DbContext.Customer
            .AddSearchCriteria(searchCriteria)
            .AddSortingOrders(orderByFields)
            .AddDependentObjects()
            .ToListAsync(cancellationToken))
        .ToPaginated(paginationInputParam);
}
