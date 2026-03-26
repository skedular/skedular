using Enterprise.Shared.Database;
using Enterprise.Shared.Database.Postgres;
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
    Task<Customer?> GetByVerifiableTokenUntrackedAsync(string verifiableToken, CancellationToken cancellationToken);
    Task<Customer?> GetMinimalByVerifiableTokenUntrackedAsync(string verifiableToken, CancellationToken cancellationToken);
    Task<bool> AnyByVerifiableTokenUntrackedAsync(string verifiableToken, CancellationToken cancellationToken);
    Customer Update(Customer customer);
    Customer Remove(Customer customer);
}

internal static class CustomerExtensions
{
    extension(IQueryable<Customer> originalQuery)
    {
        internal IIncludableQueryable<Customer, Organization> AddDependentObjects(bool isTracked) =>
            (isTracked ? originalQuery.AsTracking() : originalQuery.AsNoTrackingWithIdentityResolution())
            .Include(query => query.Identities)
            .Include(query => query.OrganizationMembers)
            .ThenInclude(query => query.Organization);
    }
}

public class CustomerRepository(LocationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<LocationDbContext, Customer>(dbContext, timeProvider), ICustomerRepository
{
    public override async Task<Customer> UpsertNakedAsync(string id, CancellationToken cancellationToken)
    {
        await base.UpsertNakedAsync(id, cancellationToken);

        return (await GetByIdAsync(id, cancellationToken))!;
    }

    public async Task<Customer?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Customer
            .AddDependentObjects(true)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<Customer?> GetByIdUntrackedAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Customer
            .AddDependentObjects(false)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<Customer?> GetByVerifiableTokenUntrackedAsync(string verifiableToken, CancellationToken cancellationToken) =>
        await DbContext.Customer
            .AddDependentObjects(false)
            .FirstOrDefaultAsync(
                query => !query.DeletedAt.HasValue && query.Identities.Select(identity => identity.Id).Contains(verifiableToken),
                cancellationToken);

    public async Task<Customer?> GetMinimalByVerifiableTokenUntrackedAsync(string verifiableToken, CancellationToken cancellationToken) =>
        await DbContext.Customer
            .AsNoTracking()
            .FirstOrDefaultAsync(
                query => !query.DeletedAt.HasValue && query.Identities.Select(identity => identity.Id).Contains(verifiableToken),
                cancellationToken);

    public async Task<bool> AnyByVerifiableTokenUntrackedAsync(string verifiableToken, CancellationToken cancellationToken) =>
        await DbContext.Customer
            .AsNoTrackingWithIdentityResolution()
            .AnyAsync(
                query => !query.DeletedAt.HasValue && query.Identities.Select(identity => identity.Id).Contains(verifiableToken),
                cancellationToken);

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
