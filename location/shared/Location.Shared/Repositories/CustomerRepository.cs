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
    Task<Customer?> GetByIdUntrackedAsync(string id, CancellationToken cancellationToken);
    Task<Customer?> GetByVerifiableTokenAsync(string verifiableToken, CancellationToken cancellationToken);
    Task<Customer?> GetByVerifiableTokenUntrackedAsync(string verifiableToken, CancellationToken cancellationToken);
    Customer Update(Customer customer);
    Customer Remove(Customer customer);
}

internal static class CustomerExtensions
{
    internal static IIncludableQueryable<Customer, Organization> AddDependentObjects(this IQueryable<Customer> originalQuery, bool isTracked) =>
        (isTracked ? originalQuery.AsTracking() : originalQuery.AsNoTrackingWithIdentityResolution())
        .Include(query => query.Identities)
        .Include(query => query.OrganizationMembers)
        .ThenInclude(query => query.Organization);
}

public class CustomerRepository(LocationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<LocationDbContext, Customer>(dbContext, timeProvider), ICustomerRepository
{
    private static readonly Func<LocationDbContext, bool, string, CancellationToken, Task<Customer?>>
        s_getByIdQueryAsync =
            EF.CompileAsyncQuery<LocationDbContext, bool, string, CancellationToken, Customer?>((dbContext, isTracked, id, cancellationToken) =>
                dbContext.Customer
                    .AddDependentObjects(isTracked)
                    .FirstOrDefault(query => query.Id == id));

    private static readonly Func<LocationDbContext, bool, string, CancellationToken, Task<Customer?>>
        s_getByVerifiableTokenQueryAsync =
            EF.CompileAsyncQuery<LocationDbContext, bool, string, CancellationToken, Customer?>((dbContext, isTracked, verifiableToken,
                    cancellationToken) =>
                dbContext.Customer
                    .AddDependentObjects(isTracked)
                    .FirstOrDefault(query =>
                        !query.DeletedAt.HasValue &&
                        query.Identities.Select(identity => identity.Id).Contains(verifiableToken)));

    public override async Task<Customer> UpsertNakedAsync(string id, CancellationToken cancellationToken)
    {
        await base.UpsertNakedAsync(id, cancellationToken);

        return (await GetByIdAsync(id, cancellationToken))!;
    }

    public async Task<Customer?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await s_getByIdQueryAsync(DbContext, true, id, cancellationToken);

    public async Task<Customer?> GetByIdUntrackedAsync(string id, CancellationToken cancellationToken) =>
        await s_getByIdQueryAsync(DbContext, false, id, cancellationToken);

    public async Task<Customer?> GetByVerifiableTokenAsync(string verifiableToken, CancellationToken cancellationToken) =>
        await s_getByVerifiableTokenQueryAsync(DbContext, true, verifiableToken, cancellationToken);

    public async Task<Customer?> GetByVerifiableTokenUntrackedAsync(string verifiableToken, CancellationToken cancellationToken) =>
        await s_getByVerifiableTokenQueryAsync(DbContext, false, verifiableToken, cancellationToken);

    public Customer Update(Customer customer)
    {
        var now = TimeProvider.GetUtcNow();
        customer.ModifiedAt = now;
        return DbContext.Customer.Update(customer).Entity;
    }

    public Customer Remove(Customer customer)
    {
        var now = TimeProvider.GetUtcNow();
        customer.DeletedAt = now;
        return DbContext.Customer.Update(customer).Entity;
    }
}
