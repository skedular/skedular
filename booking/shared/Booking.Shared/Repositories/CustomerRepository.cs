using Api.Shared.Services.Cache;
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
    ValueTask UpdateAsync(Customer customer, CancellationToken cancellationToken);
    ValueTask<Customer> RemoveAsync(Customer customer, CancellationToken cancellationToken);
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
            .ThenInclude(query => query!.Organization)
            .Include(query => query.PreferredTeams.Where(team => !includeActiveItemsOnly || !team.DeletedAt.HasValue))
            .ThenInclude(query => query.Organization);
}

public class CustomerRepository(BookingDbContext dbContext, TimeProvider timeProvider, IGenericCustomerCacheService genericCustomerCacheService)
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
