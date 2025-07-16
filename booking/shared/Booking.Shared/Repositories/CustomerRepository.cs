using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Organization = Booking.Shared.Database.Entities.Organization;

namespace Booking.Shared.Repositories;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<Customer> UpsertNakedAsync(string id, bool includeActiveItemsOnly, CancellationToken cancellationToken);
    Task<Customer?> GetByIdAsync(string id, bool includeActiveItemsOnly, CancellationToken cancellationToken);
    Task<Customer?> GetByVerifiableTokenAsync(string verifiableToken, bool includeActiveItemsOnly, CancellationToken cancellationToken);
    Task<ICollection<Customer>> GetByIdsAsync(ICollection<string> ids, bool includeActiveItemsOnly, CancellationToken cancellationToken);
    void Update(Customer customer);
    Customer Remove(Customer customer);
}

internal static class CustomerExtensions
{
    internal static IIncludableQueryable<Customer, Organization?> AddDependentObjects(
        this IQueryable<Customer> originalQuery,
        bool includeActiveItemsOnly) =>
        originalQuery
            .Include(query => query.Identities)
            .Include(query => query.DefaultOrganization)
            .Include(query => query.PreferredLocations.Where(location => !includeActiveItemsOnly || !location.DeletedAt.HasValue))
            .ThenInclude(query => query.Organization)
            .Include(query => query.PreferredOrganizationTags.Where(tag => !includeActiveItemsOnly || !tag.DeletedAt.HasValue))
            .ThenInclude(query => query.Organization)
            .Include(query => query.PreferredResources.Where(desk => !includeActiveItemsOnly || (!desk.DeletedAt.HasValue && !desk.Inactive)))
            .ThenInclude(query => query.Location)
            .ThenInclude(query => query.Organization)
            .Include(query => query.PreferredTeams.Where(team => !includeActiveItemsOnly || !team.DeletedAt.HasValue))
            .ThenInclude(query => query.Organization);
}

public class CustomerRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, Customer>(dbContext, timeProvider), ICustomerRepository
{
    public async Task<Customer> UpsertNakedAsync(string id, bool includeActiveItemsOnly, CancellationToken cancellationToken)
    {
        await base.UpsertNakedAsync(id, cancellationToken);

        return (await GetByIdAsync(id, includeActiveItemsOnly, cancellationToken))!;
    }

    public async Task<Customer?> GetByIdAsync(string id, bool includeActiveItemsOnly, CancellationToken cancellationToken) =>
        await DbContext.Customer.AddDependentObjects(includeActiveItemsOnly).FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<Customer?> GetByVerifiableTokenAsync(
        string verifiableToken,
        bool includeActiveItemsOnly,
        CancellationToken cancellationToken) =>
        await DbContext.Customer
            .AddDependentObjects(includeActiveItemsOnly)
            .FirstOrDefaultAsync(query =>
                    !query.DeletedAt.HasValue &&
                    query.Identities.Select(identity => identity.Id).Contains(verifiableToken),
                cancellationToken);

    public async Task<ICollection<Customer>>
        GetByIdsAsync(ICollection<string> ids, bool includeActiveItemsOnly, CancellationToken cancellationToken) =>
        await DbContext.Customer
            .Where(query => !query.DeletedAt.HasValue && ids.Contains(query.Id))
            .AddDependentObjects(includeActiveItemsOnly)
            .ToListAsync(cancellationToken);

    public void Update(Customer customer)
    {
        var now = TimeProvider.GetUtcNow();
        customer.ModifiedAt = now;
        DbContext.Customer.Update(customer);
    }

    public Customer Remove(Customer customer)
    {
        var now = TimeProvider.GetUtcNow();
        customer.DeletedAt = now;
        return DbContext.Customer.Update(customer).Entity;
    }
}
