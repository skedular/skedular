using Api.Shared.Services.Cache;
using Enterprise.Shared.Database;
using Location.Shared.Database;
using Location.Shared.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Location.Shared.Repositories;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<Customer> UpsertNakedAsync(string id, CancellationToken cancellationToken);
    Task<Customer?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<Customer?> GetByVerifiableTokenAsync(string verifiableToken, CancellationToken cancellationToken);
    Task<ICollection<Customer>> GetAllAsync(CancellationToken cancellationToken);
    ValueTask<Customer> AddAsync(Customer customer, CancellationToken cancellationToken);
    ValueTask<Customer> UpdateAsync(Customer customer, CancellationToken cancellationToken);
    ValueTask<Customer> RemoveAsync(Customer customer, CancellationToken cancellationToken);
}

internal static class CustomerExtensions
{
    internal static IIncludableQueryable<Customer, Organization> AddDependentObjects(
        this IQueryable<Customer> originalQuery) =>
        originalQuery
            .Include(query => query.Identities)
            .Include(query => query.OrganizationMembers)
            .ThenInclude(query => query.Organization);
}

public class CustomerRepository(LocationDbContext dbContext, TimeProvider timeProvider, IGenericCustomerCacheService genericCustomerCacheService)
    : RepositoryBase<LocationDbContext, Customer>(dbContext, timeProvider), ICustomerRepository
{
    private static readonly Func<LocationDbContext, string, CancellationToken, Task<Customer?>>
        s_getByIdQueryAsync =
            EF.CompileAsyncQuery<LocationDbContext, string, CancellationToken, Customer?>((
                    dbContext,
                    id,
                    cancellationToken) =>
                dbContext.Customer
                    .AddDependentObjects()
                    .FirstOrDefault(query => query.Id == id));

    private static readonly Func<LocationDbContext, string, CancellationToken, Task<Customer?>>
        s_getByVerifiableTokenQueryAsync =
            EF.CompileAsyncQuery<LocationDbContext, string, CancellationToken, Customer?>((
                    dbContext,
                    verifiableToken,
                    cancellationToken) =>
                dbContext.Customer
                    .AddDependentObjects()
                    .FirstOrDefault(query =>
                        !query.DeletedAt.HasValue &&
                        query.Identities.Select(identity => identity.Id).Contains(verifiableToken)));

    public override async Task<Customer> UpsertNakedAsync(string id, CancellationToken cancellationToken)
    {
        await base.UpsertNakedAsync(id, cancellationToken);

        return (await GetByIdAsync(id, cancellationToken))!;
    }

    public async Task<Customer?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await s_getByIdQueryAsync(DbContext, id, cancellationToken);

    public async Task<Customer?>
        GetByVerifiableTokenAsync(string verifiableToken, CancellationToken cancellationToken) =>
        await s_getByVerifiableTokenQueryAsync(DbContext, verifiableToken, cancellationToken);

    public async Task<ICollection<Customer>> GetAllAsync(CancellationToken cancellationToken) =>
        await DbContext.Customer
            .AddDependentObjects()
            .Where(query => !query.DeletedAt.HasValue)
            .OrderBy(query => query.Id)
            .ToListAsync(cancellationToken);

    public async ValueTask<Customer> AddAsync(Customer customer, CancellationToken cancellationToken)
    {
        var now = TimeProvider.GetUtcNow();
        customer.CreatedAt = now;
        customer = DbContext.Customer.Add(customer).Entity;

        await genericCustomerCacheService.InvalidateByVerifiableTokensAsync(customer.Identities.Select(identity => identity.Id), cancellationToken);

        return customer;
    }

    public async ValueTask<Customer> UpdateAsync(Customer customer, CancellationToken cancellationToken)
    {
        var now = TimeProvider.GetUtcNow();
        customer.ModifiedAt = now;
        customer = DbContext.Customer.Update(customer).Entity;

        await genericCustomerCacheService.InvalidateByVerifiableTokensAsync(customer.Identities.Select(identity => identity.Id), cancellationToken);

        return customer;
    }

    public async ValueTask<Customer> RemoveAsync(Customer customer, CancellationToken cancellationToken)
    {
        var now = TimeProvider.GetUtcNow();
        customer.DeletedAt = now;
        customer = DbContext.Customer.Update(customer).Entity;

        await genericCustomerCacheService.InvalidateByVerifiableTokensAsync(customer.Identities.Select(identity => identity.Id), cancellationToken);

        return customer;
    }
}
