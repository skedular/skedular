using Microsoft.EntityFrameworkCore;
using Slack.Shared.Database;

namespace Slack.Shared.Repositories;

public interface IRepositoryFactory
{
    ICustomerRepository CustomerRepository { get; }
    IIdentityRepository IdentityRepository { get; }
    ILocationRepository LocationRepository { get; }
    IOrganizationRepository OrganizationRepository { get; }
    IOrganizationMemberRepository OrganizationMemberRepository { get; }
    ITeamRepository TeamRepository { get; }
    IWorkspaceChannelRepository WorkspaceChannelRepository { get; }
    IWorkspaceMemberRepository WorkspaceMemberRepository { get; }
    IWorkspaceRepository WorkspaceRepository { get; }
}

public class RepositoryFactory : IRepositoryFactory, IAsyncDisposable
{
    private readonly SlackDbContext _dbContext;

    public RepositoryFactory(IDbContextFactory<SlackDbContext> dbContextFactory, TimeProvider timeProvider)
    {
        _dbContext = dbContextFactory.CreateDbContext();

        CustomerRepository = new CustomerRepository(_dbContext, timeProvider);
        IdentityRepository = new IdentityRepository(_dbContext, timeProvider);
        LocationRepository = new LocationRepository(_dbContext, timeProvider);
        OrganizationRepository = new OrganizationRepository(_dbContext, timeProvider);
        OrganizationMemberRepository = new OrganizationMemberRepository(_dbContext, timeProvider);
        TeamRepository = new TeamRepository(_dbContext, timeProvider);
        WorkspaceChannelRepository = new WorkspaceChannelRepository(_dbContext, timeProvider);
        WorkspaceMemberRepository = new WorkspaceMemberRepository(_dbContext, timeProvider);
        WorkspaceRepository = new WorkspaceRepository(_dbContext, timeProvider);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    public ICustomerRepository CustomerRepository { get; }
    public IIdentityRepository IdentityRepository { get; }
    public ILocationRepository LocationRepository { get; }
    public IOrganizationRepository OrganizationRepository { get; }
    public IOrganizationMemberRepository OrganizationMemberRepository { get; }
    public ITeamRepository TeamRepository { get; }
    public IWorkspaceChannelRepository WorkspaceChannelRepository { get; }
    public IWorkspaceMemberRepository WorkspaceMemberRepository { get; }
    public IWorkspaceRepository WorkspaceRepository { get; }

    protected virtual async ValueTask DisposeAsyncCore() => await _dbContext.DisposeAsync();
}
