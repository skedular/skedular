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
    : RepositoryBase<PaymentDbContext, OrganizationOfferingStripePaymentIntent>(dbContext),
        IOrganizationOfferingStripePaymentIntentRepository
{
    public OrganizationOfferingStripePaymentIntent Add(
        OrganizationOfferingStripePaymentIntent organizationOfferingStripePaymentIntent)
    {
        var now = timeProvider.GetUtcNow();
        organizationOfferingStripePaymentIntent.CreatedAt = now;
        return DbContext.OrganizationOfferingStripePaymentIntent.Add(organizationOfferingStripePaymentIntent).Entity;
    }
}
