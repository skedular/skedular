using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Services.Grpc.Skedular.Organization.V1;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Grpc;
using Microsoft.Extensions.Caching.Memory;
using Slack.Shared.Mappers;
using Slack.Shared.Models;

namespace Slack.Shared.Services.CrossDomains;

public interface IOrganizationProductTagService
{
    Task<OrganizationProductTag> AdminGetAsync(string productTagId, CancellationToken cancellationToken);
    Task<OrganizationProductTag> GetAsync(string workspaceMemberId, string productTagId, CancellationToken cancellationToken);
}

public class OrganizationProductTagService(
    ApplicationConfiguration applicationConfiguration,
    OrganizationConfiguration organizationConfiguration,
    Api.Shared.Services.Grpc.Skedular.Organization.V1.OrganizationService.OrganizationServiceClient organizationServiceClient,
    IMapper mapper,
    IMemoryCache memoryCache) : IOrganizationProductTagService
{
    private readonly MemoryCacheEntryOptions _cacheEntryOptions = new() { SlidingExpiration = TimeSpan.FromSeconds(30) };

    public async Task<OrganizationProductTag> AdminGetAsync(string productTagId, CancellationToken cancellationToken) =>
        (await memoryCache.GetOrCreateAsync(
            CreateKeyById(productTagId),
            async _ => mapper.MapTo(
                await organizationServiceClient.Admin_GetProductTagAsync(
                    new Admin_GetProductTagInput { Id = productTagId },
                    organizationConfiguration.ApiKey.CreateMetadata(),
                    cancellationToken: cancellationToken)),
            _cacheEntryOptions))!;

    public async Task<OrganizationProductTag> GetAsync(string workspaceMemberId, string productTagId, CancellationToken cancellationToken) =>
        (await memoryCache.GetOrCreateAsync(
            CreateKeyById(productTagId),
            async _ => mapper.MapTo(
                await organizationServiceClient.GetProductTagAsync(
                    new GetProductTagInput { Id = productTagId },
                    organizationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                    cancellationToken: cancellationToken)),
            _cacheEntryOptions))!;

    private string CreateKeyById(string id) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:crossdomain:organization-producttag-id:{id}";
}
