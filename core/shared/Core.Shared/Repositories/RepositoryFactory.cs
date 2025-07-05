using Core.Shared.Database;
using Enterprise.Shared.Database;
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
    }

    public ICustomerRepository CustomerRepository { get; }
    public IIdentityRepository IdentityRepository { get; }
    public ICdnFileRepository CdnFileRepository { get; }
    public IPrivateFileRepository PrivateFileRepository { get; }
}
