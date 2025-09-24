using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Services.Grpc.Skedular.Organization.V1;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Grpc;
using Microsoft.Extensions.Caching.Hybrid;
using Slack.Shared.Mappers;
using Slack.Shared.Models;

namespace Slack.Shared.Services.CrossDomains;

public interface IOrganizationPermissionsService
{
    Task<OrganizationPermissions> GetPermissionsAsync(string workspaceMemberId, string organizationId, CancellationToken cancellationToken);
}

public class OrganizationPermissionsService(
    ApplicationConfiguration applicationConfiguration,
    OrganizationConfiguration organizationConfiguration,
    Api.Shared.Services.Grpc.Skedular.Organization.V1.OrganizationService.OrganizationServiceClient organizationServiceClient,
    IMapper mapper,
    HybridCache hybridCache)
    : IOrganizationPermissionsService
{
    public async Task<OrganizationPermissions> GetPermissionsAsync(
        string workspaceMemberId,
        string organizationId,
        CancellationToken cancellationToken) =>
        await hybridCache.GetOrCreateAsync(
            CreateKeyById(workspaceMemberId, organizationId),
            async ct => mapper.MapTo(
                await organizationServiceClient.GetPermissionsAsync(
                    new GetPermissionsInput { Id = organizationId },
                    organizationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                    cancellationToken: ct)),
            new HybridCacheEntryOptions { Expiration = TimeSpan.FromSeconds(30), LocalCacheExpiration = TimeSpan.FromSeconds(30) },
            cancellationToken: cancellationToken);

    private string CreateKeyById(string workspaceMemberId, string organizationId) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:crossdomain:organizationpermissions-id:{workspaceMemberId}:{organizationId}";
}
