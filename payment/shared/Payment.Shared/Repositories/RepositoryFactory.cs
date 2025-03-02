using Microsoft.EntityFrameworkCore;
using Payment.Shared.Database;

namespace Payment.Shared.Repositories;

public interface IRepositoryFactory
{
    ICustomerRepository CustomerRepository { get; }
    IIdentityRepository IdentityRepository { get; }
    IOrganizationRepository OrganizationRepository { get; }
    IOrganizationMemberRepository OrganizationMemberRepository { get; }
    IOrganizationOfferingRepository OrganizationOfferingRepository { get; }
    IOrganizationOfferingStripePaymentIntentRepository OrganizationOfferingStripePaymentIntentRepository { get; }
    IOrganizationStripePaymentMethodRepository OrganizationStripePaymentMethodRepository { get; }
}

public class RepositoryFactory : IRepositoryFactory, IDisposable
{
    private readonly PaymentDbContext _dbContext;
    private bool _disposed;

    public RepositoryFactory(IDbContextFactory<PaymentDbContext> dbContextFactory, TimeProvider timeProvider)
    {
        _dbContext = dbContextFactory.CreateDbContext();

        CustomerRepository = new CustomerRepository(_dbContext, timeProvider);
        IdentityRepository = new IdentityRepository(_dbContext, timeProvider);
        OrganizationRepository = new OrganizationRepository(_dbContext, timeProvider);
        OrganizationMemberRepository = new OrganizationMemberRepository(_dbContext, timeProvider);
        OrganizationOfferingRepository = new OrganizationOfferingRepository(_dbContext, timeProvider);
        OrganizationOfferingStripePaymentIntentRepository = new OrganizationOfferingStripePaymentIntentRepository(_dbContext, timeProvider);
        OrganizationStripePaymentMethodRepository = new OrganizationStripePaymentMethodRepository(_dbContext, timeProvider);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public ICustomerRepository CustomerRepository { get; }
    public IIdentityRepository IdentityRepository { get; }
    public IOrganizationRepository OrganizationRepository { get; }
    public IOrganizationMemberRepository OrganizationMemberRepository { get; }
    public IOrganizationOfferingRepository OrganizationOfferingRepository { get; }
    public IOrganizationOfferingStripePaymentIntentRepository OrganizationOfferingStripePaymentIntentRepository { get; }
    public IOrganizationStripePaymentMethodRepository OrganizationStripePaymentMethodRepository { get; }

    ~RepositoryFactory() => Dispose(false);

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _dbContext.Dispose();
        }

        _disposed = true;
    }
}
