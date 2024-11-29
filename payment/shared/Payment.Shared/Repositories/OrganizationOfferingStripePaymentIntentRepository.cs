using Enterprise.Shared.Database;
using Payment.Shared.Database;
using Payment.Shared.Database.Entities;

namespace Payment.Shared.Repositories;

public interface
    IOrganizationOfferingStripePaymentIntentRepository : IRepository<OrganizationOfferingStripePaymentIntent>
{
    OrganizationOfferingStripePaymentIntent Add(
        OrganizationOfferingStripePaymentIntent organizationOfferingStripePaymentIntent);
}

public class OrganizationOfferingStripePaymentIntentRepository(PaymentDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<PaymentDbContext, OrganizationOfferingStripePaymentIntent>(dbContext, timeProvider),
        IOrganizationOfferingStripePaymentIntentRepository
{
    public OrganizationOfferingStripePaymentIntent Add(
        OrganizationOfferingStripePaymentIntent organizationOfferingStripePaymentIntent)
    {
        var now = TimeProvider.GetUtcNow();
        organizationOfferingStripePaymentIntent.CreatedAt = now;
        return DbContext.OrganizationOfferingStripePaymentIntent.Add(organizationOfferingStripePaymentIntent).Entity;
    }
}
