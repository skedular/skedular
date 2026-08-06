using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Grpc.Skedular.Organization.Core.V1;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Grpc;
using Microsoft.Extensions.Caching.Memory;
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
    Api.Shared.Grpc.Skedular.Organization.Core.V1.OrganizationService.OrganizationServiceClient organizationServiceClient,
    IGrpcMapper grpcMapper,
    IMemoryCache memoryCache)
    : IOrganizationPermissionsService
{
    private readonly MemoryCacheEntryOptions _cacheEntryOptions = new()
    {
        SlidingExpiration = TimeSpan.FromSeconds(30),
    };

    public async Task<OrganizationPermissions> GetPermissionsAsync(
        string workspaceMemberId,
        string organizationId,
        CancellationToken cancellationToken) =>
        (await memoryCache.GetOrCreateAsync(
            CreateKeyById(workspaceMemberId, organizationId),
            async _ => grpcMapper.MapTo(
                await organizationServiceClient.GetPermissionsAsync(
                    new GetPermissionsInput
                    {
                        Id = organizationId,
                    },
                    organizationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                    cancellationToken: cancellationToken)),
            _cacheEntryOptions))!;

    private string CreateKeyById(string workspaceMemberId, string organizationId) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:crossdomain:organizationpermissions-id:{workspaceMemberId}:{organizationId}";
}
