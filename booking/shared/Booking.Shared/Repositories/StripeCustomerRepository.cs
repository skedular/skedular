using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;

namespace Booking.Shared.Repositories;

public interface IStripeCustomerRepository : IRepository<StripeCustomer>
{
    Task<StripeCustomer?> GetByStripeCustomerIdAsync(string stripeCustomerId, CancellationToken cancellationToken);
    Task<StripeCustomer?> GetByOrganizationIdAsync(string stripeAccountId, string organizationId, CancellationToken cancellationToken);
    Task<StripeCustomer?> GetByCustomerIdAsync(string stripeAccountId, string customerId, CancellationToken cancellationToken);
    StripeCustomer Add(StripeCustomer stripeCustomer);
}

public class StripeCustomerRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, StripeCustomer>(dbContext, timeProvider), IStripeCustomerRepository
{
    public async Task<StripeCustomer?> GetByStripeCustomerIdAsync(string stripeCustomerId, CancellationToken cancellationToken) =>
        await DbContext.StripeCustomer
            .Include(query => query.Organization)
            .FirstOrDefaultAsync(query => query.StripeCustomerId == stripeCustomerId, cancellationToken);

    public async Task<StripeCustomer?> GetByOrganizationIdAsync(string stripeAccountId, string organizationId, CancellationToken cancellationToken) =>
        await DbContext.StripeCustomer
            .Include(query => query.Organization)
            .FirstOrDefaultAsync(
                query => !query.DeletedAt.HasValue && query.Organization != null && query.Organization.Id == organizationId &&
                         query.StripeAccountId == stripeAccountId,
                cancellationToken);

    public async Task<StripeCustomer?> GetByCustomerIdAsync(string stripeAccountId, string customerId, CancellationToken cancellationToken) =>
        await DbContext.StripeCustomer
            .Include(query => query.Organization)
            .FirstOrDefaultAsync(
                query => !query.DeletedAt.HasValue && query.Customer != null && query.Customer.Id == customerId &&
                         query.StripeAccountId == stripeAccountId,
                cancellationToken);

    public StripeCustomer Add(StripeCustomer stripeCustomer)
    {
        var now = TimeProvider.GetUtcNow();
        stripeCustomer.CreatedAt = now;
        return DbContext.StripeCustomer.Add(stripeCustomer).Entity;
    }
}
