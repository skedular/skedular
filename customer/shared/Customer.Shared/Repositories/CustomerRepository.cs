using Customer.Shared.Database;
using Customer.Shared.Models;
using Enterprise.Shared.Database;
using Enterprise.Shared.Models;
using Enterprise.Shared.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Organization = Customer.Shared.Database.Entities.Organization;

namespace Customer.Shared.Repositories;

public interface ICustomerRepository : IRepository<Database.Entities.Customer>
{
    Task<Database.Entities.Customer> UpsertNakedAsync(string id, CancellationToken cancellationToken);
    Task<Database.Entities.Customer?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<ICollection<Database.Entities.Customer>> GetAllAsync(CancellationToken cancellationToken);

    Task<Database.Entities.Customer?> GetByVerifiableTokenAsync(
        string verifiableToken,
        CancellationToken cancellationToken);

    Task<Database.Entities.Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Database.Entities.Customer Add(Database.Entities.Customer customer);
    Database.Entities.Customer Update(Database.Entities.Customer customer);
    Database.Entities.Customer Remove(Database.Entities.Customer customer);

    Task<(PaginatedInfo, ICollection<Edge<Database.Entities.Customer>>, int )> GetPaginatedCustomersAsync(
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
            .Include(query => query.DefaultOrganization)
            .Include(query => query.DefaultLocations)
            .ThenInclude(query => query.Organization)
            .Include(query => query.PreferredLocationTags)
            .ThenInclude(query => query.Location)
            .ThenInclude(query => query.Organization)
            .Include(query => query.PreferredDesks)
            .ThenInclude(query => query.Location)
            .ThenInclude(query => query.Organization)
            .Include(query => query.DefaultTeams)
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
            query = query.Where(item =>
                item.DefaultLocations.Select(location => location.Id).Contains(searchCriteria.LocationId));
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
                _ => throw new ArgumentOutOfRangeException()
            }).ThenBy(query => query.Id);
    }
}

public class CustomerRepository(CustomerDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<CustomerDbContext, Database.Entities.Customer>(dbContext), ICustomerRepository
{
    public async Task<Database.Entities.Customer> UpsertNakedAsync(string id, CancellationToken cancellationToken)
    {
        var existing = await GetByIdAsync(id, cancellationToken);
        if (existing is not null)
        {
            return existing;
        }

        var now = timeProvider.GetUtcNow();
        return DbContext.Customer.Add(new Database.Entities.Customer { Id = id, CreatedAt = now }).Entity;
    }

    public async Task<Database.Entities.Customer?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Customer
            .AddDependentObjects()
            .Where(query => query.Id == id)
            .OrderBy(query => query.Id)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<ICollection<Database.Entities.Customer>> GetAllAsync(CancellationToken cancellationToken) =>
        await DbContext.Customer.AddDependentObjects().Where(query => !query.DeletedAt.HasValue)
            .ToListAsync(cancellationToken);

    public async Task<Database.Entities.Customer?> GetByVerifiableTokenAsync(
        string verifiableToken,
        CancellationToken cancellationToken) =>
        await DbContext.Customer.AddDependentObjects()
            .Where(query => !query.DeletedAt.HasValue &&
                            query.Identities.Select(identity => identity.Id).Contains(verifiableToken))
            .OrderBy(query => query.Id)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<Database.Entities.Customer?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken) =>
        await DbContext.Customer.AddDependentObjects()
            .Where(query => !query.DeletedAt.HasValue &&
                            query.Identities.Any(identity =>
                                identity.Email != null && EF.Functions.ILike(identity.Email, email)))
            .OrderBy(query => query.Id)
            .FirstOrDefaultAsync(cancellationToken);


    public Database.Entities.Customer Add(Database.Entities.Customer customer)
    {
        var now = timeProvider.GetUtcNow();
        customer.CreatedAt = now;
        return DbContext.Customer.Add(customer).Entity;
    }

    public Database.Entities.Customer Update(Database.Entities.Customer customer)
    {
        var now = timeProvider.GetUtcNow();
        customer.ModifiedAt = now;
        return DbContext.Customer.Update(customer).Entity;
    }

    public Database.Entities.Customer Remove(Database.Entities.Customer customer)
    {
        var now = timeProvider.GetUtcNow();
        customer.DeletedAt = now;
        return DbContext.Customer.Update(customer).Entity;
    }

    public async Task<(PaginatedInfo, ICollection<Edge<Database.Entities.Customer>>, int )>
        GetPaginatedCustomersAsync(
            PaginationInputParam paginationInputParam,
            CustomerSearchCriteria searchCriteria,
            ICollection<CustomerOrder> orderByFields,
            CancellationToken cancellationToken) =>
        (await DbContext.Customer
            .AsQueryable()
            .AddSearchCriteria(searchCriteria)
            .AddSortingOrders(orderByFields)
            .AddDependentObjects()
            .ToListAsync(cancellationToken))
        .ToPaginated(paginationInputParam);
}
