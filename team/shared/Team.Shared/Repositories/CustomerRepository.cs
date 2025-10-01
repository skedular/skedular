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
    Task<Customer?> GetByIdUntrackedAsync(string id, CancellationToken cancellationToken);
    Task<Customer?> GetByVerifiableTokenAsync(string verifiableToken, CancellationToken cancellationToken);
    Task<Customer?> GetByVerifiableTokenUntrackedAsync(string verifiableToken, CancellationToken cancellationToken);
    Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<ICollection<Customer>> GetByIdsAsync(ICollection<string> ids, CancellationToken cancellationToken);
    Customer Update(Customer customer);
    Customer Remove(Customer customer);
}

internal static class CustomerExtensions
{
    internal static IIncludableQueryable<Customer, Database.Entities.Team?> AddDependentObjects(
        this IQueryable<Customer> originalQuery,
        bool isTracked) =>
        (isTracked ? originalQuery.AsTracking() : originalQuery.AsNoTrackingWithIdentityResolution())
        .Include(query => query.Identities)
        .Include(query => query.OrganizationMembers)
        .ThenInclude(query => query.Organization)
        .Include(query => query.JoinInvitationsCreatedBy)
        .ThenInclude(query => query.Team)
        .Include(query => query.JoinInvitationsInvitee)
        .ThenInclude(query => query.Team);
}

public class CustomerRepository(TeamDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<TeamDbContext, Customer>(dbContext, timeProvider), ICustomerRepository
{
    private static readonly Func<TeamDbContext, bool, string, CancellationToken, Task<Customer?>>
        s_getByIdQueryAsync =
            EF.CompileAsyncQuery<TeamDbContext, bool, string, CancellationToken, Customer?>((dbContext, isTracked, id, cancellationToken) =>
                dbContext.Customer
                    .AddDependentObjects(isTracked)
                    .FirstOrDefault(query => query.Id == id));

    private static readonly Func<TeamDbContext, bool, string, CancellationToken, Task<Customer?>>
        s_getByVerifiableTokenQueryAsync =
            EF.CompileAsyncQuery<TeamDbContext, bool, string, CancellationToken, Customer?>((dbContext, isTracked, verifiableToken,
                    cancellationToken) =>
                dbContext.Customer
                    .AddDependentObjects(isTracked)
                    .FirstOrDefault(query =>
                        !query.DeletedAt.HasValue &&
                        query.Identities.Select(identity => identity.Id).Contains(verifiableToken)));

    private static readonly Func<TeamDbContext, bool, string, CancellationToken, Task<Customer?>>
        s_getByEmailQueryAsync =
            EF.CompileAsyncQuery<TeamDbContext, bool, string, CancellationToken, Customer?>((dbContext, isTracked, email, cancellationToken) =>
                dbContext.Customer
                    .AddDependentObjects(isTracked)
                    .FirstOrDefault(query =>
                        !query.DeletedAt.HasValue &&
                        query.Identities.Any(identity =>
                            identity.Email != null &&
                            EF.Functions.ILike(identity.Email, email))));

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

    public async Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken) =>
        await s_getByEmailQueryAsync(DbContext, true, email, cancellationToken);

    public async Task<ICollection<Customer>> GetByIdsAsync(ICollection<string> ids, CancellationToken cancellationToken) =>
        await DbContext.Customer
            .Where(query => ids.Contains(query.Id))
            .AddDependentObjects(true)
            .OrderBy(query => query.Id)
            .ToListAsync(cancellationToken);

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
