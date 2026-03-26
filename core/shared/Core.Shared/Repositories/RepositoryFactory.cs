using Core.Shared.Database;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.Postgres;
using Microsoft.EntityFrameworkCore;

namespace Core.Shared.Repositories;

public interface IRepositoryFactory
{
    CoreDbContext DbContext { get; }
    IUnitOfWork UnitOfWork { get; }
    ICustomerRepository CustomerRepository { get; }
    IIdentityRepository IdentityRepository { get; }
    ICdnFileRepository CdnFileRepository { get; }
    IPrivateFileRepository PrivateFileRepository { get; }
    IOrganizationRepository OrganizationRepository { get; }
    IOrganizationMemberRepository OrganizationMemberRepository { get; }
    IOrganizationSsoSettingRepository OrganizationSsoSettingRepository { get; }
}

public class RepositoryFactory : RepositoryFactoryBase<CoreDbContext>, IRepositoryFactory
{
    public RepositoryFactory(IDbContextFactory<CoreDbContext> dbContextFactory, TimeProvider timeProvider)
    {
        _dbContext = dbContextFactory.CreateDbContext();

        CustomerRepository = new CustomerRepository(_dbContext, timeProvider);
        IdentityRepository = new IdentityRepository(_dbContext, timeProvider);
        CdnFileRepository = new CdnFileRepository(_dbContext, timeProvider);
        PrivateFileRepository = new PrivateFileRepository(_dbContext, timeProvider);
        OrganizationRepository = new OrganizationRepository(_dbContext, timeProvider);
        OrganizationMemberRepository = new OrganizationMemberRepository(_dbContext, timeProvider);
        OrganizationSsoSettingRepository = new OrganizationSsoSettingRepository(_dbContext, timeProvider);
    }

    public ICustomerRepository CustomerRepository { get; }
    public IIdentityRepository IdentityRepository { get; }
    public ICdnFileRepository CdnFileRepository { get; }
    public IPrivateFileRepository PrivateFileRepository { get; }
    public IOrganizationRepository OrganizationRepository { get; }
    public IOrganizationMemberRepository OrganizationMemberRepository { get; }
    public IOrganizationSsoSettingRepository OrganizationSsoSettingRepository { get; }
}
