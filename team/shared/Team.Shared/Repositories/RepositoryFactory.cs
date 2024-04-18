using Microsoft.EntityFrameworkCore;
using Team.Shared.Database;
using TimeProvider = System.TimeProvider;

namespace Team.Shared.Repositories;

public interface IRepositoryFactory
{
    IBookingRepository BookingRepository { get; }
    ICustomerRepository CustomerRepository { get; }
    IIdentityRepository IdentityRepository { get; }
    IJoinInvitationRepository JoinInvitationRepository { get; }
    ITeamMemberRepository TeamMemberRepository { get; }
    ITeamRepository TeamRepository { get; }
    IOrganizationMemberRepository OrganizationMemberRepository { get; }
    IOrganizationRepository OrganizationRepository { get; }
}

public class RepositoryFactory : IRepositoryFactory, IAsyncDisposable
{
    private readonly TeamDbContext _dbContext;

    public RepositoryFactory(IDbContextFactory<TeamDbContext> dbContextFactory, TimeProvider timeProvider)
    {
        _dbContext = dbContextFactory.CreateDbContext();

        BookingRepository = new BookingRepository(_dbContext, timeProvider);
        CustomerRepository = new CustomerRepository(_dbContext, timeProvider);
        IdentityRepository = new IdentityRepository(_dbContext, timeProvider);
        JoinInvitationRepository = new JoinInvitationRepository(_dbContext, timeProvider);
        TeamMemberRepository = new TeamMemberRepository(_dbContext, timeProvider);
        TeamRepository = new TeamRepository(_dbContext, timeProvider);
        OrganizationMemberRepository = new OrganizationMemberRepository(_dbContext, timeProvider);
        OrganizationRepository = new OrganizationRepository(_dbContext, timeProvider);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    public IBookingRepository BookingRepository { get; }
    public ICustomerRepository CustomerRepository { get; }
    public IIdentityRepository IdentityRepository { get; }
    public IJoinInvitationRepository JoinInvitationRepository { get; }
    public ITeamMemberRepository TeamMemberRepository { get; }
    public ITeamRepository TeamRepository { get; }
    public IOrganizationMemberRepository OrganizationMemberRepository { get; }
    public IOrganizationRepository OrganizationRepository { get; }

    protected virtual async ValueTask DisposeAsyncCore() => await _dbContext.DisposeAsync();
}
