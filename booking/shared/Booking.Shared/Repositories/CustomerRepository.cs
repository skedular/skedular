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
    Task<Customer?> GetByIdUntrackedAsync(string id, bool includeActiveItemsOnly, CancellationToken cancellationToken);
    Task<Customer?> GetByVerifiableTokenAsync(string verifiableToken, bool includeActiveItemsOnly, CancellationToken cancellationToken);
    Task<Customer?> GetMinimalByVerifiableTokenUntrackedAsync(string verifiableToken, CancellationToken cancellationToken);
    Task<bool> AnyByVerifiableTokenUntrackedAsync(string verifiableToken, CancellationToken cancellationToken);
    Task<ICollection<Customer>> GetByIdsAsync(ICollection<string> ids, bool includeActiveItemsOnly, CancellationToken cancellationToken);
    Customer Update(Customer customer);
    Customer Remove(Customer customer);
}

internal static class CustomerExtensions
{
    extension(IQueryable<Customer> originalQuery)
    {
        internal IIncludableQueryable<Customer, Organization?> AddDependentObjects(bool isTracked, bool includeActiveItemsOnly) =>
            (isTracked ? originalQuery.AsTracking() : originalQuery.AsNoTrackingWithIdentityResolution())
            .Include(query => query.Identities)
            .Include(query => query.DefaultOrganization)
            .Include(query => query.PreferredLocations.Where(location => !includeActiveItemsOnly || !location.DeletedAt.HasValue))
            .ThenInclude(query => query.Organization)
            .Include(query => query.PreferredOrganizationTags.Where(tag => !includeActiveItemsOnly || !tag.DeletedAt.HasValue))
            .ThenInclude(query => query.Organization)
            .Include(query => query.PreferredResources.Where(desk => !includeActiveItemsOnly || (!desk.DeletedAt.HasValue && !desk.Inactive)))
            .ThenInclude(query => query.Location)
            .ThenInclude(query => query!.Organization);
    }
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
        await DbContext.Customer
            .AddDependentObjects(true, includeActiveItemsOnly)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<Customer?> GetByIdUntrackedAsync(string id, bool includeActiveItemsOnly, CancellationToken cancellationToken) =>
        await DbContext.Customer
            .AddDependentObjects(false, includeActiveItemsOnly)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<Customer?> GetByVerifiableTokenAsync(
        string verifiableToken,
        bool includeActiveItemsOnly,
        CancellationToken cancellationToken) =>
        await DbContext.Customer
            .AddDependentObjects(true, includeActiveItemsOnly)
            .FirstOrDefaultAsync(query =>
                    !query.DeletedAt.HasValue &&
                    query.Identities.Select(identity => identity.Id).Contains(verifiableToken),
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

    public async Task<ICollection<Customer>> GetByIdsAsync(
        ICollection<string> ids,
        bool includeActiveItemsOnly,
        CancellationToken cancellationToken) =>
        await DbContext.Customer
            .Where(query => !query.DeletedAt.HasValue && ids.Contains(query.Id))
            .AddDependentObjects(true, includeActiveItemsOnly)
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
