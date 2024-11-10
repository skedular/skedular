using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Payment.Shared.Database;
using Payment.Shared.Database.Entities;

namespace Payment.Shared.Repositories;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<Customer> UpsertNakedAsync(string id, CancellationToken cancellationToken);
    Task<Customer?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<Customer?> GetByVerifiableTokenAsync(string verifiableToken, CancellationToken cancellationToken);
    Task<ICollection<Customer>> GetAllAsync(CancellationToken cancellationToken);
    Customer Add(Customer customer);
    Customer Update(Customer customer);
    Customer Remove(Customer customer);
}

internal static class CustomerExtensions
{
    internal static IIncludableQueryable<Customer, ICollection<Identity>> AddDependentObjects(
        this IQueryable<Customer> originalQuery) =>
        originalQuery
            .Include(query => query.Identities);
}

public class CustomerRepository(PaymentDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<PaymentDbContext, Customer>(dbContext), ICustomerRepository
{
    private static readonly Func<PaymentDbContext, string, CancellationToken, Task<Customer?>>
        s_getByIdQueryAsync =
            EF.CompileAsyncQuery<PaymentDbContext, string, CancellationToken, Customer?>((
                    dbContext,
                    id,
                    cancellationToken) =>
                dbContext.Customer
                    .AddDependentObjects()
                    .Where(query => query.Id == id)
                    .OrderBy(query => query.Id)
                    .FirstOrDefault());

    private static readonly Func<PaymentDbContext, string, CancellationToken, Task<Customer?>>
        s_getByVerifiableTokenQueryAsync =
            EF.CompileAsyncQuery<PaymentDbContext, string, CancellationToken, Customer?>((
                    dbContext,
                    verifiableToken,
                    cancellationToken) =>
                dbContext.Customer
                    .AddDependentObjects()
                    .Where(query => !query.DeletedAt.HasValue &&
                                    query.Identities.Select(identity => identity.Id).Contains(verifiableToken))
                    .OrderBy(query => query.Id)
                    .FirstOrDefault());

    private static readonly Func<PaymentDbContext, CancellationToken, Task<ICollection<Customer>>>
        s_getAllQueryAsync =
            EF.CompileAsyncQuery<PaymentDbContext, CancellationToken, ICollection<Customer>>((
                    dbContext,
                    cancellationToken) =>
                dbContext.Customer
                    .AddDependentObjects()
                    .Where(query => !query.DeletedAt.HasValue)
                    .OrderBy(query => query.Id)
                    .ToList());

    public async Task<Customer> UpsertNakedAsync(string id, CancellationToken cancellationToken)
    {
        var existing = await GetByIdAsync(id, cancellationToken);
        if (existing is not null)
        {
            return existing;
        }

        var now = timeProvider.GetUtcNow();
        return DbContext.Customer.Add(new Customer { Id = id, CreatedAt = now }).Entity;
    }

    public async Task<Customer?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await s_getByIdQueryAsync(DbContext, id, cancellationToken);

    public async Task<Customer?>
        GetByVerifiableTokenAsync(string verifiableToken, CancellationToken cancellationToken) =>
        await s_getByVerifiableTokenQueryAsync(DbContext, verifiableToken, cancellationToken);

    public async Task<ICollection<Customer>> GetAllAsync(CancellationToken cancellationToken) =>
        await s_getAllQueryAsync(DbContext, cancellationToken);

    public Customer Add(Customer customer)
    {
        var now = timeProvider.GetUtcNow();
        customer.CreatedAt = now;
        return DbContext.Customer.Add(customer).Entity;
    }

    public Customer Update(Customer customer)
    {
        var now = timeProvider.GetUtcNow();
        customer.ModifiedAt = now;
        return DbContext.Customer.Update(customer).Entity;
    }

    public Customer Remove(Customer customer)
    {
        var now = timeProvider.GetUtcNow();
        customer.DeletedAt = now;
        return DbContext.Customer.Update(customer).Entity;
    }
}
