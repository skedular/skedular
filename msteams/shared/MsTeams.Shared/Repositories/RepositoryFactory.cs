using Microsoft.EntityFrameworkCore;
using MsTeams.Shared.Database;

namespace MsTeams.Shared.Repositories;

public interface IRepositoryFactory
{
    ICustomerRepository CustomerRepository { get; }
    IIdentityRepository IdentityRepository { get; }
    ITenantRepository TenantRepository { get; }
    ITenantMemberRepository TenantMemberRepository { get; }
}

public class RepositoryFactory : IRepositoryFactory, IAsyncDisposable
{
    private readonly MsTeamsDbContext _dbContext;

    public RepositoryFactory(IDbContextFactory<MsTeamsDbContext> dbContextFactory, TimeProvider timeProvider)
    {
        _dbContext = dbContextFactory.CreateDbContext();

        CustomerRepository = new CustomerRepository(_dbContext, timeProvider);
        IdentityRepository = new IdentityRepository(_dbContext, timeProvider);
        TenantRepository = new TenantRepository(_dbContext, timeProvider);
        TenantMemberRepository = new TenantMemberRepository(_dbContext, timeProvider);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    public ICustomerRepository CustomerRepository { get; }
    public IIdentityRepository IdentityRepository { get; }
    public ITenantRepository TenantRepository { get; }
    public ITenantMemberRepository TenantMemberRepository { get; }

    protected virtual async ValueTask DisposeAsyncCore() => await _dbContext.DisposeAsync();
}
