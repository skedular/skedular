using Api.Shared.Services.Grpc.Skedular.Team.V1;
using Enterprise.Shared.Grpc;
using Slack.Api.Mappers;
using Slack.Shared.Models;
using Team = Slack.Shared.Models.Team;
using TeamConfiguration = Slack.Shared.Configurations.TeamConfiguration;

namespace Slack.Api.Services;

public interface ITeamService
{
    ValueTask<Team> GetTeamAsync(
        string teamId,
        WorkspaceMember workspaceMember,
        CancellationToken cancellationToken);

    ValueTask<ICollection<Team>> GetTeamsAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        CancellationToken cancellationToken);

    ValueTask<TeamPermissions> GetPermissionsAsync(
        string teamId,
        WorkspaceMember workspaceMember,
        CancellationToken cancellationToken);
}

public class TeamService(
    TeamConfiguration teamConfiguration,
    IMapper mapper,
    global::Api.Shared.Services.Grpc.Skedular.Team.V1.TeamService.TeamServiceClient
        teamServiceClient) : ITeamService, IDisposable
{
    private readonly SemaphoreSlim _cachedPermissionsLock = new(1, 1);
    private readonly SemaphoreSlim _cachedTeamLock = new(1, 1);
    private readonly SemaphoreSlim _cachedTeamsLock = new(1, 1);
    private TeamPermissions? _cachedPermissions;
    private Team? _cachedTeam;
    private ICollection<Team>? _cachedTeams;
    private bool _disposed;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask<Team> GetTeamAsync(
        string teamId,
        WorkspaceMember workspaceMember,
        CancellationToken cancellationToken)
    {
        if (_cachedTeam is not null)
        {
            return _cachedTeam;
        }

        try
        {
            await _cachedTeamLock.WaitAsync(cancellationToken);
            _cachedTeam = mapper.MapTo(await teamServiceClient.GetAsync(
                new GetInput { Id = teamId },
                teamConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
                cancellationToken: cancellationToken));

            return _cachedTeam;
        }
        finally
        {
            _cachedTeamLock.Release();
        }
    }

    public async ValueTask<ICollection<Team>> GetTeamsAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        CancellationToken cancellationToken)
    {
        if (_cachedTeams is not null)
        {
            return _cachedTeams;
        }

        try
        {
            await _cachedTeamsLock.WaitAsync(cancellationToken);
            var teamConnection = await teamServiceClient.GetPaginatedTeamsAsync(
                new GetPaginatedTeamsInput { First = -1, Last = -1, Where = new TeamWhereInput { OrganizationId = workspace.Organization.Id } },
                teamConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
                cancellationToken: cancellationToken);
            _cachedTeams = teamConnection.Edges.Select(item => mapper.MapTo(item.Node)).ToList();

            return _cachedTeams;
        }
        finally
        {
            _cachedTeamsLock.Release();
        }
    }

    public async ValueTask<TeamPermissions> GetPermissionsAsync(
        string teamId,
        WorkspaceMember workspaceMember,
        CancellationToken cancellationToken)
    {
        if (_cachedPermissions is not null)
        {
            return _cachedPermissions;
        }

        try
        {
            await _cachedPermissionsLock.WaitAsync(cancellationToken);
            _cachedPermissions = mapper.MapTo(
                await teamServiceClient.GetPermissionsAsync(
                    new GetPermissionsInput { Id = teamId },
                    teamConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
                    cancellationToken: cancellationToken));

            return _cachedPermissions;
        }
        finally
        {
            _cachedPermissionsLock.Release();
        }
    }

    ~TeamService() => Dispose(false);

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _cachedTeamLock.Dispose();
            _cachedTeamsLock.Dispose();
            _cachedPermissionsLock.Dispose();
        }

        _disposed = true;
    }
}
