using Enterprise.Shared.Database;
using Enterprise.Shared.Database.Postgres;
using Marketplace.Shared.Database;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Shared.Repositories;

public interface IRepositoryFactory
{
    MarketplaceDbContext DbContext { get; }
    IUnitOfWork UnitOfWork { get; }
    ICustomerRepository CustomerRepository { get; }
    IIdentityRepository IdentityRepository { get; }
    IOrganizationRepository OrganizationRepository { get; }
    IOrganizationMemberRepository OrganizationMemberRepository { get; }
    IOrganizationTagRepository OrganizationTagRepository { get; }
    IProductRepository ProductRepository { get; }
    IProductVersionRepository ProductVersionRepository { get; }
    IOrganizationSsoSettingRepository OrganizationSsoSettingRepository { get; }
}

public class RepositoryFactory : RepositoryFactoryBase<MarketplaceDbContext>, IRepositoryFactory
{
    public RepositoryFactory(IDbContextFactory<MarketplaceDbContext> dbContextFactory, TimeProvider timeProvider)
    {
        _dbContext = dbContextFactory.CreateDbContext();

        CustomerRepository = new CustomerRepository(_dbContext, timeProvider);
        IdentityRepository = new IdentityRepository(_dbContext, timeProvider);
        OrganizationRepository = new OrganizationRepository(_dbContext, timeProvider);
        OrganizationMemberRepository = new OrganizationMemberRepository(_dbContext, timeProvider);
        OrganizationTagRepository = new OrganizationTagRepository(_dbContext, timeProvider);
        ProductRepository = new ProductRepository(_dbContext, timeProvider);
        ProductVersionRepository = new ProductVersionRepository(_dbContext, timeProvider);
        OrganizationSsoSettingRepository = new OrganizationSsoSettingRepository(_dbContext, timeProvider);
    }

    public ICustomerRepository CustomerRepository { get; }
    public IIdentityRepository IdentityRepository { get; }
    public IOrganizationRepository OrganizationRepository { get; }
    public IOrganizationMemberRepository OrganizationMemberRepository { get; }
    public IOrganizationTagRepository OrganizationTagRepository { get; }
    public IProductRepository ProductRepository { get; }
    public IProductVersionRepository ProductVersionRepository { get; }
    public IOrganizationSsoSettingRepository OrganizationSsoSettingRepository { get; }
}
