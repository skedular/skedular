using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Organization.Shared.Database;
using Organization.Shared.Database.Entities;

namespace Organization.Shared.Repositories;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<Customer> UpsertNakedAsync(string id, CancellationToken cancellationToken);
    Task<Customer?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<Customer?> GetByVerifiableTokenAsync(string verifiableToken, CancellationToken cancellationToken);
    Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<ICollection<Customer>> GetAllAsync(CancellationToken cancellationToken);
    Customer Add(Customer customer);
    Customer Update(Customer customer);
    Customer Remove(Customer customer);
}

internal static class CustomerExtensions
{
    internal static IIncludableQueryable<Customer, Database.Entities.Organization?> AddDependentObjects(
        this IQueryable<Customer> originalQuery) =>
        originalQuery
            .Include(query => query.Identities)
            .Include(query => query.OrganizationMembers)
            .ThenInclude(query => query.Organization)
            .Include(query => query.JoinInvitationsCreatedBy)
            .ThenInclude(query => query.Organization)
            .Include(query => query.JoinInvitationsInvitee)
            .ThenInclude(query => query.Organization);
}

public class CustomerRepository(OrganizationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<OrganizationDbContext, Customer>(dbContext, timeProvider), ICustomerRepository
{
    private static readonly Func<OrganizationDbContext, string, CancellationToken, Task<Customer?>>
        s_getByIdQueryAsync =
            EF.CompileAsyncQuery<OrganizationDbContext, string, CancellationToken, Customer?>((
                    dbContext,
                    id,
                    cancellationToken) =>
                Queryable
                    .Where<Customer>(dbContext.Customer
                        .AddDependentObjects(), query => query.Id == id)
                    .OrderBy(query => query.Id)
                    .FirstOrDefault());

    private static readonly Func<OrganizationDbContext, string, CancellationToken, Task<Customer?>>
        s_getByVerifiableTokenQueryAsync =
            EF.CompileAsyncQuery<OrganizationDbContext, string, CancellationToken, Customer?>((
                    dbContext,
                    verifiableToken,
                    cancellationToken) =>
                Queryable
                    .Where<Customer>(dbContext.Customer
                        .AddDependentObjects(), query => !query.DeletedAt.HasValue &&
                                                         query.Identities.Select(identity => identity.Id)
                                                             .Contains(verifiableToken))
                    .OrderBy(query => query.Id)
                    .FirstOrDefault());

    private static readonly Func<OrganizationDbContext, string, CancellationToken, Task<Customer?>>
        s_getByEmailQueryAsync =
            EF.CompileAsyncQuery<OrganizationDbContext, string, CancellationToken, Customer?>((
                    dbContext,
                    email,
                    cancellationToken) =>
                Queryable
                    .Where<Customer>(dbContext.Customer
                        .AddDependentObjects(), query => !query.DeletedAt.HasValue &&
                                                         query.Identities.Any(identity =>
                                                             identity.Email != null &&
                                                             EF.Functions.ILike(identity.Email, email)))
                    .OrderBy(query => query.Id)
                    .FirstOrDefault());

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

    public async Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken) =>
        await s_getByEmailQueryAsync(DbContext, email, cancellationToken);

    public async Task<ICollection<Customer>> GetAllAsync(CancellationToken cancellationToken) =>
        await DbContext.Customer
            .AddDependentObjects()
            .Where(query => !query.DeletedAt.HasValue)
            .OrderBy(query => query.Id)
            .ToListAsync(cancellationToken);

    public Customer Add(Customer customer)
    {
        var now = TimeProvider.GetUtcNow();
        customer.CreatedAt = now;
        return DbContext.Customer.Add(customer).Entity;
    }

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
