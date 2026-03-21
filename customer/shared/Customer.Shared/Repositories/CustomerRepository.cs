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
    Task<Database.Entities.Customer?> GetByIdUntrackedAsync(string id, CancellationToken cancellationToken);
    Task<Database.Entities.Customer?> GetByVerifiableTokenAsync(string verifiableToken, CancellationToken cancellationToken);
    Task<Database.Entities.Customer?> GetByVerifiableTokenUntrackedAsync(string verifiableToken, CancellationToken cancellationToken);
    Task<bool> AnyByVerifiableTokenUntrackedAsync(string verifiableToken, CancellationToken cancellationToken);
    Task<Database.Entities.Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<Database.Entities.Customer?> GetByEmailUntrackedAsync(string email, CancellationToken cancellationToken);
    Task<ICollection<Database.Entities.Customer>> GetAllUntrackedAsync(CancellationToken cancellationToken);
    Database.Entities.Customer Add(Database.Entities.Customer customer);
    Database.Entities.Customer Update(Database.Entities.Customer customer);

    Task<(PaginatedInfo, ICollection<Edge<Database.Entities.Customer>>, int)> GetPaginatedCustomersUntrackedAsync(
        PaginationInputParam paginationInputParam,
        CustomerSearchCriteria searchCriteria,
        ICollection<CustomerOrder> orderByFields,
        CancellationToken cancellationToken);
}

internal static class CustomerExtensions
{
    extension(IQueryable<Database.Entities.Customer> originalQuery)
    {
        internal IIncludableQueryable<Database.Entities.Customer, Organization?> AddDependentObjects(bool isTracked) =>
            (isTracked ? originalQuery.AsTracking() : originalQuery.AsNoTracking())
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
            .Include(query => query.FavouriteLocations)
            .ThenInclude(query => query.Organization);

        internal IQueryable<Database.Entities.Customer> AddSearchCriteria(CustomerSearchCriteria searchCriteria)
        {
            originalQuery = originalQuery.Where(item => !item.DeletedAt.HasValue);

            if (!string.IsNullOrWhiteSpace(searchCriteria.NameContains))
            {
                originalQuery = originalQuery.Where(item =>
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
                originalQuery = originalQuery.Where(item =>
                    item.PreferredLocations.Select(location => location.Id).Contains(searchCriteria.LocationId));
            }

            return originalQuery;
        }
    }
}

public class CustomerRepository(CustomerDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<CustomerDbContext, Database.Entities.Customer>(dbContext, timeProvider), ICustomerRepository
{
    public async Task<Database.Entities.Customer?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Customer
            .AddDependentObjects(true)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<Database.Entities.Customer?> GetByIdUntrackedAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Customer
            .AddDependentObjects(false)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<Database.Entities.Customer?> GetByVerifiableTokenAsync(string verifiableToken, CancellationToken cancellationToken) =>
        await DbContext.Customer
            .AddDependentObjects(true)
            .FirstOrDefaultAsync(
                query => !query.DeletedAt.HasValue && query.Identities.Select(identity => identity.Id).Contains(verifiableToken),
                cancellationToken);

    public async Task<Database.Entities.Customer?> GetByVerifiableTokenUntrackedAsync(string verifiableToken, CancellationToken cancellationToken) =>
        await DbContext.Customer
            .AddDependentObjects(false)
            .FirstOrDefaultAsync(
                query => !query.DeletedAt.HasValue && query.Identities.Select(identity => identity.Id).Contains(verifiableToken),
                cancellationToken);

    public async Task<bool> AnyByVerifiableTokenUntrackedAsync(string verifiableToken, CancellationToken cancellationToken) =>
        await DbContext.Customer
            .AsNoTracking()
            .AnyAsync(
                query => !query.DeletedAt.HasValue && query.Identities.Select(identity => identity.Id).Contains(verifiableToken),
                cancellationToken);

    public async Task<Database.Entities.Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken) =>
        await DbContext.Customer
            .AddDependentObjects(true)
            .FirstOrDefaultAsync(
                query =>
                    !query.DeletedAt.HasValue &&
                    query.Identities.Any(identity => identity.Email != null && EF.Functions.ILike(identity.Email, email)),
                cancellationToken);

    public async Task<Database.Entities.Customer?> GetByEmailUntrackedAsync(string email, CancellationToken cancellationToken) =>
        await DbContext.Customer
            .AddDependentObjects(false)
            .FirstOrDefaultAsync(
                query =>
                    !query.DeletedAt.HasValue &&
                    query.Identities.Any(identity => identity.Email != null && EF.Functions.ILike(identity.Email, email)),
                cancellationToken);

    public async Task<ICollection<Database.Entities.Customer>> GetAllUntrackedAsync(CancellationToken cancellationToken) =>
        await DbContext.Customer
            .AddDependentObjects(false)
            .Where(query => !query.DeletedAt.HasValue)
            .OrderBy(query => query.Id)
            .ToListAsync(cancellationToken);

    public Database.Entities.Customer Add(Database.Entities.Customer customer)
    {
        var now = TimeProvider.GetUtcNow();
        customer.CreatedAt = now;
        return DbContext.Customer.Add(customer).Entity;
    }

    public Database.Entities.Customer Update(Database.Entities.Customer customer)
    {
        var now = TimeProvider.GetUtcNow();
        customer.ModifiedAt = now;
        return DbContext.Customer.Update(customer).Entity;
    }

    public async Task<(PaginatedInfo, ICollection<Edge<Database.Entities.Customer>>, int)> GetPaginatedCustomersUntrackedAsync(
        PaginationInputParam paginationInputParam,
        CustomerSearchCriteria searchCriteria,
        ICollection<CustomerOrder> orderByFields,
        CancellationToken cancellationToken) =>
        await DbContext.Customer
            .AddSearchCriteria(searchCriteria)
            .AddDependentObjects(false)
            .ToPaginatedAsync(paginationInputParam, GetPaginationFields(orderByFields), cancellationToken);

    private static List<KeysetPaginationField<Database.Entities.Customer>> GetPaginationFields(ICollection<CustomerOrder> orderByFields)
    {
        if (orderByFields.Count == 0)
        {
            return
            [
                KeysetPaginationField<Database.Entities.Customer>.Create(
                    nameof(Database.Entities.Customer.Name),
                    query => query.Name,
                    OrderDirection.Ascending)
            ];
        }

        return orderByFields.Select(orderField => orderField.Field switch
            {
                CustomerOrderField.Designation => KeysetPaginationField<Database.Entities.Customer>.Create(
                    nameof(Database.Entities.Customer.Designation),
                    query => query.Designation,
                    orderField.Direction),
                CustomerOrderField.Title => KeysetPaginationField<Database.Entities.Customer>.Create(
                    nameof(Database.Entities.Customer.Title),
                    query => query.Title,
                    orderField.Direction),
                CustomerOrderField.Name => KeysetPaginationField<Database.Entities.Customer>.Create(
                    nameof(Database.Entities.Customer.Name),
                    query => query.Name,
                    orderField.Direction),
                CustomerOrderField.GivenName => KeysetPaginationField<Database.Entities.Customer>.Create(
                    nameof(Database.Entities.Customer.GivenName),
                    query => query.GivenName,
                    orderField.Direction),
                CustomerOrderField.MiddleName => KeysetPaginationField<Database.Entities.Customer>.Create(
                    nameof(Database.Entities.Customer.MiddleName),
                    query => query.MiddleName,
                    orderField.Direction),
                CustomerOrderField.FamilyName => KeysetPaginationField<Database.Entities.Customer>.Create(
                    nameof(Database.Entities.Customer.FamilyName),
                    query => query.FamilyName,
                    orderField.Direction),
                CustomerOrderField.Timezone => KeysetPaginationField<Database.Entities.Customer>.Create(
                    nameof(Database.Entities.Customer.Timezone),
                    query => query.Timezone,
                    orderField.Direction),
                CustomerOrderField.Locale => KeysetPaginationField<Database.Entities.Customer>.Create(
                    nameof(Database.Entities.Customer.Locale),
                    query => query.Locale,
                    orderField.Direction),
                CustomerOrderField.PhoneNumber => KeysetPaginationField<Database.Entities.Customer>.Create(
                    nameof(Database.Entities.Customer.PhoneNumber),
                    query => query.PhoneNumber,
                    orderField.Direction),
                _ => throw new ArgumentOutOfRangeException()
            })
            .ToList();
    }
}
