using Customer.Shared.Database;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.Postgres;
using Microsoft.EntityFrameworkCore;

namespace Customer.Shared.Repositories;

public interface IRepositoryFactory
{
    CustomerDbContext DbContext { get; }
    IUnitOfWork UnitOfWork { get; }
    ICustomerFeedbackRepository CustomerFeedbackRepository { get; }
    ICustomerRepository CustomerRepository { get; }
    IIdentityRepository IdentityRepository { get; }
    IOrganizationRepository OrganizationRepository { get; }
    IOrganizationMemberRepository OrganizationMemberRepository { get; }
    ILocationRepository LocationRepository { get; }
    IResourceRepository ResourceRepository { get; }
    IOrganizationTagRepository OrganizationTagRepository { get; }
    IOrganizationSsoSettingRepository OrganizationSsoSettingRepository { get; }
    IStripeCustomerRepository StripeCustomerRepository { get; }
    IStripePaymentIntentRepository StripePaymentIntentRepository { get; }
    IStripePaymentMethodRepository StripePaymentMethodRepository { get; }
    ICustomerBillingDetailsRepository CustomerBillingDetailsRepository { get; }
}

public class RepositoryFactory : RepositoryFactoryBase<CustomerDbContext>, IRepositoryFactory
{
    public RepositoryFactory(IDbContextFactory<CustomerDbContext> dbContextFactory, TimeProvider timeProvider)
    {
        _dbContext = dbContextFactory.CreateDbContext();

        CustomerFeedbackRepository = new CustomerFeedbackRepository(_dbContext, timeProvider);
        CustomerRepository = new CustomerRepository(_dbContext, timeProvider);
        IdentityRepository = new IdentityRepository(_dbContext, timeProvider);
        OrganizationRepository = new OrganizationRepository(_dbContext, timeProvider);
        OrganizationMemberRepository = new OrganizationMemberRepository(_dbContext, timeProvider);
        LocationRepository = new LocationRepository(_dbContext, timeProvider);
        ResourceRepository = new ResourceRepository(_dbContext, timeProvider);
        OrganizationTagRepository = new OrganizationTagRepository(_dbContext, timeProvider);
        OrganizationSsoSettingRepository = new OrganizationSsoSettingRepository(_dbContext, timeProvider);
        StripeCustomerRepository = new StripeCustomerRepository(_dbContext, timeProvider);
        StripePaymentIntentRepository = new StripePaymentIntentRepository(_dbContext, timeProvider);
        StripePaymentMethodRepository = new StripePaymentMethodRepository(_dbContext, timeProvider);
        CustomerBillingDetailsRepository = new CustomerBillingDetailsRepository(_dbContext, timeProvider);
    }

    public ICustomerFeedbackRepository CustomerFeedbackRepository { get; }
    public ICustomerRepository CustomerRepository { get; }
    public IIdentityRepository IdentityRepository { get; }
    public IOrganizationRepository OrganizationRepository { get; }
    public IOrganizationMemberRepository OrganizationMemberRepository { get; }
    public ILocationRepository LocationRepository { get; }
    public IResourceRepository ResourceRepository { get; }
    public IOrganizationTagRepository OrganizationTagRepository { get; }
    public IOrganizationSsoSettingRepository OrganizationSsoSettingRepository { get; }
    public IStripeCustomerRepository StripeCustomerRepository { get; }
    public IStripePaymentIntentRepository StripePaymentIntentRepository { get; }
    public IStripePaymentMethodRepository StripePaymentMethodRepository { get; }
    public ICustomerBillingDetailsRepository CustomerBillingDetailsRepository { get; }
}
