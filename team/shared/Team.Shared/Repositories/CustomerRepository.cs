using Api.Shared.Services.Cache;
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
    ValueTask<Customer> AddAsync(Customer customer, CancellationToken cancellationToken);
    ValueTask<Customer> UpdateAsync(Customer customer, CancellationToken cancellationToken);
    ValueTask<Customer> RemoveAsync(Customer customer, CancellationToken cancellationToken);
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

public class CustomerRepository(TeamDbContext dbContext, TimeProvider timeProvider, IGenericCustomerCacheService genericCustomerCacheService)
    : RepositoryBase<TeamDbContext, Customer>(dbContext, timeProvider), ICustomerRepository
{
    private static readonly Func<TeamDbContext, string, CancellationToken, Task<Customer?>>
        s_getByIdQueryAsync =
            EF.CompileAsyncQuery<TeamDbContext, string, CancellationToken, Customer?>((
                    dbContext,
                    id,
                    cancellationToken) =>
                dbContext.Customer
                    .AddDependentObjects()
                    .FirstOrDefault(query => query.Id == id));

    private static readonly Func<TeamDbContext, string, CancellationToken, Task<Customer?>>
        s_getByVerifiableTokenQueryAsync =
            EF.CompileAsyncQuery<TeamDbContext, string, CancellationToken, Customer?>((
                    dbContext,
                    verifiableToken,
                    cancellationToken) =>
                dbContext.Customer
                    .AddDependentObjects()
                    .FirstOrDefault(query =>
                        !query.DeletedAt.HasValue &&
                        query.Identities.Select(identity => identity.Id).Contains(verifiableToken)));

    private static readonly Func<TeamDbContext, string, CancellationToken, Task<Customer?>>
        s_getByEmailQueryAsync =
            EF.CompileAsyncQuery<TeamDbContext, string, CancellationToken, Customer?>((
                    dbContext,
                    email,
                    cancellationToken) =>
                dbContext.Customer
                    .AddDependentObjects()
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

    public async Task<ICollection<Customer>>
        GetByIdsAsync(ICollection<string> ids, CancellationToken cancellationToken) =>
        await DbContext.Customer
            .Where(query => ids.Contains(query.Id))
            .AddDependentObjects()
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
