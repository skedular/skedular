using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Organization = Booking.Shared.Database.Entities.Organization;

namespace Booking.Shared.Repositories;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<Customer> UpsertNakedAsync(string id, CancellationToken cancellationToken);
    Task<Customer?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<ICollection<Customer>> GetAllAsync(CancellationToken cancellationToken);
    Task<Customer?> GetByVerifiableTokenAsync(string verifiableToken, CancellationToken cancellationToken);
    Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Customer Add(Customer customer);
    Customer Update(Customer customer);
    Customer Remove(Customer customer);
}

internal static class CustomerExtensions
{
    internal static IIncludableQueryable<Customer, Organization?> AddDependentObjects(
        this IQueryable<Customer> originalQuery) =>
        originalQuery
            .Include(query => query.Identities)
            .Include(query => query.DefaultOrganization)
            .Include(query => query.DefaultLocations)
            .ThenInclude(query => query.Organization)
            .Include(query => query.PreferredLocationTags)
            .ThenInclude(query => query.Location)
            .ThenInclude(query => query.Organization)
            .Include(query => query.PreferredDesks)
            .ThenInclude(query => query.Location)
            .ThenInclude(query => query.Organization)
            .Include(query => query.DefaultTeams)
            .ThenInclude(query => query.Organization);
}

public class CustomerRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, Customer>(dbContext), ICustomerRepository
{
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
        await DbContext.Customer
            .AddDependentObjects()
            .Where(query => query.Id == id)
            .OrderBy(query => query.Id)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<ICollection<Customer>> GetAllAsync(CancellationToken cancellationToken) =>
        await DbContext.Customer.AddDependentObjects().Where(query => !query.DeletedAt.HasValue)
            .ToListAsync(cancellationToken);

    public async Task<Customer?> GetByVerifiableTokenAsync(
        string verifiableToken,
        CancellationToken cancellationToken) =>
        await DbContext.Customer.AddDependentObjects()
            .Where(query => !query.DeletedAt.HasValue &&
                            query.Identities.Select(identity => identity.Id).Contains(verifiableToken))
            .OrderBy(query => query.Id)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<Customer?> GetByEmailAsync(string email,
        CancellationToken cancellationToken) =>
        await DbContext.Customer.AddDependentObjects()
            .Where(query => !query.DeletedAt.HasValue &&
                            query.Identities.Any(identity =>
                                identity.Email != null && EF.Functions.ILike(identity.Email, email)))
            .OrderBy(query => query.Id)
            .FirstOrDefaultAsync(cancellationToken);

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
