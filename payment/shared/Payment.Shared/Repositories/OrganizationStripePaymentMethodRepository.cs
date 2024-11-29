using Enterprise.Shared;
using Enterprise.Shared.Database;
using Payment.Shared.Database;
using Payment.Shared.Database.Entities;

namespace Payment.Shared.Repositories;

public interface IOrganizationStripePaymentMethodRepository : IRepository<OrganizationStripePaymentMethod>
{
    OrganizationStripePaymentMethod Add(
        OrganizationStripePaymentMethod organizationStripePaymentMethod);

    OrganizationStripePaymentMethod Update(OrganizationStripePaymentMethod organizationStripePaymentMethod);
    OrganizationStripePaymentMethod Remove(OrganizationStripePaymentMethod organizationStripePaymentMethod);
    void RemoveRange(ICollection<OrganizationStripePaymentMethod> organizationStripePaymentMethods);
}

public class OrganizationStripePaymentMethodRepository(PaymentDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<PaymentDbContext, OrganizationStripePaymentMethod>(dbContext, timeProvider),
        IOrganizationStripePaymentMethodRepository
{
    public OrganizationStripePaymentMethod Add(OrganizationStripePaymentMethod organizationStripePaymentMethod)
    {
        var now = TimeProvider.GetUtcNow();
        organizationStripePaymentMethod.CreatedAt = now;
        return DbContext.OrganizationStripePaymentMethod.Add(organizationStripePaymentMethod).Entity;
    }

    public OrganizationStripePaymentMethod Update(OrganizationStripePaymentMethod organizationStripePaymentMethod)
    {
        var now = TimeProvider.GetUtcNow();
        organizationStripePaymentMethod.ModifiedAt = now;
        return DbContext.OrganizationStripePaymentMethod.Update(organizationStripePaymentMethod).Entity;
    }

    public OrganizationStripePaymentMethod Remove(OrganizationStripePaymentMethod organizationStripePaymentMethod)
    {
        var now = TimeProvider.GetUtcNow();
        organizationStripePaymentMethod.DeletedAt = now;
        return DbContext.OrganizationStripePaymentMethod.Update(organizationStripePaymentMethod).Entity;
    }

    public void RemoveRange(ICollection<OrganizationStripePaymentMethod> organizationStripePaymentMethods)
    {
        var now = TimeProvider.GetUtcNow();
        organizationStripePaymentMethods.ForEach(organizationStripePaymentMethod =>
            organizationStripePaymentMethod.DeletedAt = now);
        DbContext.OrganizationStripePaymentMethod.UpdateRange(organizationStripePaymentMethods);
    }
}
