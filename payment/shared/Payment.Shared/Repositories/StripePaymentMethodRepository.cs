using Enterprise.Shared.Database;
using Payment.Shared.Database;
using Payment.Shared.Database.Entities;

namespace Payment.Shared.Repositories;

public interface IStripePaymentMethodRepository : IRepository<StripePaymentMethod>
{
    void Add(StripePaymentMethod stripePaymentMethod);
    void Update(StripePaymentMethod stripePaymentMethod);
    StripePaymentMethod Remove(StripePaymentMethod stripePaymentMethod);
    void PurgeRange(ICollection<StripePaymentMethod> organizationStripePaymentMethods);
}

public class StripePaymentMethodRepository(PaymentDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<PaymentDbContext, StripePaymentMethod>(dbContext, timeProvider), IStripePaymentMethodRepository
{
    public void Add(StripePaymentMethod stripePaymentMethod)
    {
        var now = TimeProvider.GetUtcNow();
        stripePaymentMethod.CreatedAt = now;
        DbContext.StripePaymentMethod.Add(stripePaymentMethod);
    }

    public void Update(StripePaymentMethod stripePaymentMethod)
    {
        var now = TimeProvider.GetUtcNow();
        stripePaymentMethod.ModifiedAt = now;
        DbContext.StripePaymentMethod.Update(stripePaymentMethod);
    }

    public StripePaymentMethod Remove(StripePaymentMethod stripePaymentMethod)
    {
        var now = TimeProvider.GetUtcNow();
        stripePaymentMethod.DeletedAt = now;
        return DbContext.StripePaymentMethod.Update(stripePaymentMethod).Entity;
    }

    public void PurgeRange(ICollection<StripePaymentMethod> organizationStripePaymentMethods) =>
        DbContext.StripePaymentMethod.RemoveRange(organizationStripePaymentMethods);
}
