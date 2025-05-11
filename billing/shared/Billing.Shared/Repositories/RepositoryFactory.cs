using Billing.Shared.Database;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;

namespace Billing.Shared.Repositories;

public interface IRepositoryFactory
{
    BillingDbContext DbContext { get; }
    IUnitOfWork UnitOfWork { get; }
    ICustomerRepository CustomerRepository { get; }
    IIdentityRepository IdentityRepository { get; }
    IOrganizationRepository OrganizationRepository { get; }
    IOrganizationMemberRepository OrganizationMemberRepository { get; }
    IOrganizationOfferingRepository OrganizationOfferingRepository { get; }
    IOrganizationSsoSettingRepository OrganizationSsoSettingRepository { get; }
}

public class RepositoryFactory : IRepositoryFactory, IDisposable
{
    private bool _disposed;

    public RepositoryFactory(IDbContextFactory<BillingDbContext> dbContextFactory, TimeProvider timeProvider)
    {
        DbContext = dbContextFactory.CreateDbContext();

        CustomerRepository = new CustomerRepository(DbContext, timeProvider);
        IdentityRepository = new IdentityRepository(DbContext, timeProvider);
        OrganizationRepository = new OrganizationRepository(DbContext, timeProvider);
        OrganizationMemberRepository = new OrganizationMemberRepository(DbContext, timeProvider);
        OrganizationOfferingRepository = new OrganizationOfferingRepository(DbContext, timeProvider);
        OrganizationSsoSettingRepository = new OrganizationSsoSettingRepository(DbContext, timeProvider);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public BillingDbContext DbContext { get; }

    public IUnitOfWork UnitOfWork => DbContext;
    public ICustomerRepository CustomerRepository { get; }
    public IIdentityRepository IdentityRepository { get; }
    public IOrganizationRepository OrganizationRepository { get; }
    public IOrganizationMemberRepository OrganizationMemberRepository { get; }
    public IOrganizationOfferingRepository OrganizationOfferingRepository { get; }
    public IOrganizationSsoSettingRepository OrganizationSsoSettingRepository { get; }

    ~RepositoryFactory() => Dispose(false);

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            DbContext.Dispose();
        }

        _disposed = true;
    }
}
