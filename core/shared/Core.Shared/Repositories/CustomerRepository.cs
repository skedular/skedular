using Api.Shared.Services.Cache;
using Core.Shared.Database;
using Core.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Core.Shared.Repositories;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<Customer> UpsertNakedAsync(string id, CancellationToken cancellationToken);
    Task<Customer?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<Customer?> GetByVerifiableTokenAsync(string verifiableToken, CancellationToken cancellationToken);
    ValueTask UpdateAsync(Customer customer, CancellationToken cancellationToken);
    ValueTask<Customer> RemoveAsync(Customer customer, CancellationToken cancellationToken);
}

internal static class CustomerExtensions
{
    internal static IIncludableQueryable<Customer, ICollection<Identity>> AddDependentObjects(
        this IQueryable<Customer> originalQuery) =>
        originalQuery
            .Include(query => query.Identities);
}

public class CustomerRepository(CoreDbContext dbContext, TimeProvider timeProvider, IGenericCustomerCacheService genericCustomerCacheService)
    : RepositoryBase<CoreDbContext, Customer>(dbContext, timeProvider), ICustomerRepository
{
    private static readonly Func<CoreDbContext, string, CancellationToken, Task<Customer?>>
        s_getByIdQueryAsync =
            EF.CompileAsyncQuery<CoreDbContext, string, CancellationToken, Customer?>((
                    dbContext,
                    id,
                    cancellationToken) =>
                dbContext.Customer
                    .AddDependentObjects()
                    .FirstOrDefault(query => query.Id == id));

    private static readonly Func<CoreDbContext, string, CancellationToken, Task<Customer?>>
        s_getByVerifiableTokenQueryAsync =
            EF.CompileAsyncQuery<CoreDbContext, string, CancellationToken, Customer?>((
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

    public async Task<Customer?> GetByVerifiableTokenAsync(string verifiableToken, CancellationToken cancellationToken) =>
        await s_getByVerifiableTokenQueryAsync(DbContext, verifiableToken, cancellationToken);

    public async ValueTask UpdateAsync(Customer customer, CancellationToken cancellationToken)
    {
        var now = TimeProvider.GetUtcNow();
        customer.ModifiedAt = now;
        DbContext.Customer.Update(customer);

        await genericCustomerCacheService.InvalidateByVerifiableTokensAsync(customer.Identities.Select(identity => identity.Id), cancellationToken);
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
