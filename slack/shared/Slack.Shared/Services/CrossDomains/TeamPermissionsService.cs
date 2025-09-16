using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Services.Grpc.Skedular.Team.V1;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Grpc;
using Microsoft.Extensions.Caching.Hybrid;
using Slack.Shared.Mappers;
using Slack.Shared.Models;

namespace Slack.Shared.Services.CrossDomains;

public interface ITeamPermissionsService
{
    Task<TeamPermissions> GetPermissionsAsync(string workspaceMemberId, string teamId, CancellationToken cancellationToken);
}

public class TeamPermissionsService(
    ApplicationConfiguration applicationConfiguration,
    TeamConfiguration teamConfiguration,
    Api.Shared.Services.Grpc.Skedular.Team.V1.TeamService.TeamServiceClient teamServiceClient,
    IMapper mapper,
    HybridCache hybridCache)
    : ITeamPermissionsService
{
    public async Task<TeamPermissions> GetPermissionsAsync(string workspaceMemberId, string teamId,
        CancellationToken cancellationToken) =>
        await hybridCache.GetOrCreateAsync(
            CreateKeyById(workspaceMemberId, teamId),
            async ct => mapper.MapTo(
                await teamServiceClient.GetPermissionsAsync(
                    new GetPermissionsInput { Id = teamId },
                    teamConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                    cancellationToken: ct)),
            new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(5), LocalCacheExpiration = TimeSpan.FromMinutes(5) },
            cancellationToken: cancellationToken);

    private string CreateKeyById(string workspaceMemberId, string teamId) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:crossdomain:teampermissions-id:{workspaceMemberId}:{teamId}";
}
