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
    Task<Customer?> GetByEmailAsync(string email, bool includeActiveItemsOnly, CancellationToken cancellationToken);
    Task<ICollection<Customer>> GetAllAsync(bool includeActiveItemsOnly, CancellationToken cancellationToken);
    Customer Add(Customer customer);
    Customer Update(Customer customer);
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
            .Include(query => query.PreferredDesks.Where(desk => !includeActiveItemsOnly || (!desk.DeletedAt.HasValue && !desk.Deactivated)))
            .Include(query => query.PreferredRooms.Where(room => !includeActiveItemsOnly || (!room.DeletedAt.HasValue && !room.Deactivated)))
            .ThenInclude(query => query.Location)
            .ThenInclude(query => query.Organization)
            .Include(query => query.PreferredTeams.Where(team => !includeActiveItemsOnly || !team.DeletedAt.HasValue))
            .ThenInclude(query => query.Organization);
}

public class CustomerRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, Customer>(dbContext, timeProvider), ICustomerRepository
{
    private static readonly Func<BookingDbContext, string, bool, CancellationToken, Task<Customer?>>
        s_getByEmailQueryAsync =
            EF.CompileAsyncQuery<BookingDbContext, string, bool, CancellationToken, Customer?>((
                    dbContext,
                    email,
                    includeActiveItemsOnly,
                    cancellationToken) =>
                dbContext.Customer
                    .AddDependentObjects(includeActiveItemsOnly)
                    .FirstOrDefault(query =>
                        !query.DeletedAt.HasValue &&
                        query.Identities.Any(identity =>
                            identity.Email != null &&
                            EF.Functions.ILike(identity.Email, email))));

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

    public async Task<Customer?> GetByEmailAsync(string email, bool includeActiveItemsOnly, CancellationToken cancellationToken) =>
        await dbContext.Customer
            .AddDependentObjects(includeActiveItemsOnly)
            .FirstOrDefaultAsync(query =>
                    !query.DeletedAt.HasValue &&
                    query.Identities.Any(identity =>
                        identity.Email != null &&
                        EF.Functions.ILike(identity.Email, email)),
                cancellationToken);

    public async Task<ICollection<Customer>> GetAllAsync(bool includeActiveItemsOnly, CancellationToken cancellationToken) =>
        await DbContext.Customer
            .AddDependentObjects(includeActiveItemsOnly)
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
