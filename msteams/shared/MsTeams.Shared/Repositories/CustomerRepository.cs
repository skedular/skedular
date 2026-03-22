using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MsTeams.Shared.Database;
using MsTeams.Shared.Database.Entities;

namespace MsTeams.Shared.Repositories;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<Customer> UpsertNakedAsync(string id, CancellationToken cancellationToken);
    Task<Customer?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<Customer?> GetByVerifiableTokenAsync(string verifiableToken, CancellationToken cancellationToken);
    Task<bool> AnyByVerifiableTokenUntrackedAsync(string verifiableToken, CancellationToken cancellationToken);
    Customer Update(Customer customer);
    Customer Remove(Customer customer);
}

internal static class CustomerExtensions
{
    extension(IQueryable<Customer> originalQuery)
    {
        internal IIncludableQueryable<Customer, ICollection<Identity>> AddDependentObjects(bool isTracked) =>
            (isTracked ? originalQuery.AsTracking() : originalQuery.AsNoTrackingWithIdentityResolution())
            .Include(query => query.Identities);
    }
}

public class CustomerRepository(MsTeamsDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<MsTeamsDbContext, Customer>(dbContext, timeProvider), ICustomerRepository
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

    public async Task<Customer?> GetByVerifiableTokenAsync(string verifiableToken, CancellationToken cancellationToken) =>
        await DbContext.Customer
            .AddDependentObjects(true)
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
