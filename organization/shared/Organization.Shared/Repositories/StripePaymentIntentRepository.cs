using Enterprise.Shared.Database;
using Organization.Shared.Database;
using Organization.Shared.Database.Entities;

namespace Organization.Shared.Repositories;

public interface IStripePaymentIntentRepository : IRepository<StripePaymentIntent>
{
    void Add(StripePaymentIntent stripePaymentIntent);
}

public class StripePaymentIntentRepository(OrganizationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<OrganizationDbContext, StripePaymentIntent>(dbContext, timeProvider), IStripePaymentIntentRepository
{
    public void Add(StripePaymentIntent stripePaymentIntent)
    {
        var now = TimeProvider.GetUtcNow();
        stripePaymentIntent.CreatedAt = now;
        DbContext.StripePaymentIntent.Add(stripePaymentIntent);
    }
}
