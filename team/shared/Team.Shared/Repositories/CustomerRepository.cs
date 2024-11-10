using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Team.Shared.Database;
using Team.Shared.Database.Entities;

namespace Team.Shared.Repositories;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<Customer> UpsertNakedAsync(string id, CancellationToken cancellationToken);
    Task<Customer?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<Customer?> GetByVerifiableTokenAsync(string verifiableToken, CancellationToken cancellationToken);
    Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<ICollection<Customer>> GetAllAsync(CancellationToken cancellationToken);
    Task<ICollection<Customer>> GetByIdsAsync(ICollection<string> ids, CancellationToken cancellationToken);
    Customer Add(Customer customer);
    Customer Update(Customer customer);
    Customer Remove(Customer customer);
}

internal static class CustomerExtensions
{
    internal static IIncludableQueryable<Customer, Database.Entities.Team?> AddDependentObjects(
        this IQueryable<Customer> originalQuery) =>
        originalQuery
            .Include(query => query.Identities)
            .Include(query => query.OrganizationMembers)
            .ThenInclude(query => query.Organization)
            .Include(query => query.JoinInvitationsCreatedBy)
            .ThenInclude(query => query.Team)
            .Include(query => query.JoinInvitationsInvitee)
            .ThenInclude(query => query.Team);
}

public class CustomerRepository(TeamDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<TeamDbContext, Customer>(dbContext), ICustomerRepository
{
    private static readonly Func<TeamDbContext, string, CancellationToken, Task<Customer?>>
        s_getByIdQueryAsync =
            EF.CompileAsyncQuery<TeamDbContext, string, CancellationToken, Customer?>((
                    dbContext,
                    id,
                    cancellationToken) =>
                dbContext.Customer
                    .AddDependentObjects()
                    .Where(query => query.Id == id)
                    .OrderBy(query => query.Id)
                    .FirstOrDefault());

    private static readonly Func<TeamDbContext, string, CancellationToken, Task<Customer?>>
        s_getByVerifiableTokenQueryAsync =
            EF.CompileAsyncQuery<TeamDbContext, string, CancellationToken, Customer?>((
                    dbContext,
                    verifiableToken,
                    cancellationToken) =>
                dbContext.Customer
                    .AddDependentObjects()
                    .Where(query => !query.DeletedAt.HasValue &&
                                    query.Identities.Select(identity => identity.Id).Contains(verifiableToken))
                    .OrderBy(query => query.Id)
                    .FirstOrDefault());

    private static readonly Func<TeamDbContext, string, CancellationToken, Task<Customer?>>
        s_getByEmailQueryAsync =
            EF.CompileAsyncQuery<TeamDbContext, string, CancellationToken, Customer?>((
                    dbContext,
                    email,
                    cancellationToken) =>
                dbContext.Customer
                    .AddDependentObjects()
                    .Where(query => !query.DeletedAt.HasValue &&
                                    query.Identities.Any(identity =>
                                        identity.Email != null && EF.Functions.ILike(identity.Email, email)))
                    .OrderBy(query => query.Id)
                    .FirstOrDefault());

    private static readonly Func<TeamDbContext, CancellationToken, Task<ICollection<Customer>>>
        s_getAllQueryAsync =
            EF.CompileAsyncQuery<TeamDbContext, CancellationToken, ICollection<Customer>>((
                    dbContext,
                    cancellationToken) =>
                dbContext.Customer
                    .AddDependentObjects()
                    .Where(query => !query.DeletedAt.HasValue)
                    .OrderBy(query => query.Id)
                    .ToList());

    private static readonly Func<TeamDbContext, ICollection<string>, CancellationToken, Task<ICollection<Customer>>>
        s_getByIdsQueryAsync =
            EF.CompileAsyncQuery<TeamDbContext, ICollection<string>, CancellationToken, ICollection<Customer>>((
                    dbContext,
                    ids,
                    cancellationToken) =>
                dbContext.Customer
                    .AddDependentObjects()
                    .Where(query => ids.Contains(query.Id))
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

    public async Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken) =>
        await s_getByEmailQueryAsync(DbContext, email, cancellationToken);

    public async Task<ICollection<Customer>> GetAllAsync(CancellationToken cancellationToken) =>
        await s_getAllQueryAsync(DbContext, cancellationToken);

    public async Task<ICollection<Customer>>
        GetByIdsAsync(ICollection<string> ids, CancellationToken cancellationToken) =>
        await s_getByIdsQueryAsync(DbContext, ids, cancellationToken);

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
