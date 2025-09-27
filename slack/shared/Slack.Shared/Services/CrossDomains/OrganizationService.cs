using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Services.Grpc.Skedular.Organization.V1;
using Enterprise.Shared;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Grpc;
using Microsoft.Extensions.Caching.Hybrid;
using Slack.Shared.Mappers;
using Organization = Slack.Shared.Models.Organization;

namespace Slack.Shared.Services.CrossDomains;

public interface IOrganizationService
{
    Task<Organization> AdminAddAsync(Organization organization, CancellationToken cancellationToken);
    Task<Organization> AdminGetAsync(string organizationId, CancellationToken cancellationToken);
    Task<Organization> GetAsync(string workspaceMemberId, string organizationId, CancellationToken cancellationToken);
}

public class OrganizationService(
    ApplicationConfiguration applicationConfiguration,
    OrganizationConfiguration organizationConfiguration,
    Api.Shared.Services.Grpc.Skedular.Organization.V1.OrganizationService.OrganizationServiceClient organizationServiceClient,
    IMapper mapper,
    HybridCache hybridCache)
    : IOrganizationService
{
    public async Task<Organization> AdminGetAsync(string organizationId, CancellationToken cancellationToken) =>
        await hybridCache.GetOrCreateAsync(
            CreateKeyById(organizationId),
            async ct => mapper.MapTo(
                await organizationServiceClient.Admin_GetAsync(
                    new Admin_GetInput { Id = organizationId },
                    organizationConfiguration.ApiKey.CreateMetadata(),
                    cancellationToken: ct)),
            new HybridCacheEntryOptions { Expiration = TimeSpan.FromSeconds(30), LocalCacheExpiration = TimeSpan.FromSeconds(30) },
            cancellationToken: cancellationToken);

    public async Task<Organization> AdminAddAsync(Organization organization, CancellationToken cancellationToken)
    {
        var activeTermsOfUse = await organizationServiceClient.GetActiveOrganizationTermsOfUseAsync(
            new GetActiveOrganizationTermsOfUseInput(),
            organizationConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);

        var mappedOrganization = mapper.MapTo(
            await organizationServiceClient.Admin_AddAsync(
                new Admin_AddInput
                {
                    Id = organization.Id,
                    Name = organization.Name.ToSafeString(),
                    AgreedToTermsOfUse = true,
                    TermsOfUseId = activeTermsOfUse.Id,
                    Type = OrganizationType.Private
                },
                organizationConfiguration.ApiKey.CreateMetadata(),
                cancellationToken: cancellationToken));

        await CacheAsync([mappedOrganization], cancellationToken);

        return mappedOrganization;
    }

    public async Task<Organization> GetAsync(string workspaceMemberId, string organizationId, CancellationToken cancellationToken) =>
        await hybridCache.GetOrCreateAsync(
            CreateKeyById(organizationId),
            async ct => mapper.MapTo(
                await organizationServiceClient.GetAsync(
                    new GetInput { Id = organizationId },
                    organizationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                    cancellationToken: ct)),
            new HybridCacheEntryOptions { Expiration = TimeSpan.FromSeconds(30), LocalCacheExpiration = TimeSpan.FromSeconds(30) },
            cancellationToken: cancellationToken);

    private async Task CacheAsync(ICollection<Organization> organizations, CancellationToken cancellationToken)
    {
        foreach (var organization in organizations)
        {
            var key = CreateKeyById(organization.Id);

            await hybridCache.RemoveAsync(key, cancellationToken);
            await hybridCache.SetAsync(
                key,
                organization,
                new HybridCacheEntryOptions { Expiration = TimeSpan.FromSeconds(30), LocalCacheExpiration = TimeSpan.FromSeconds(30) },
                cancellationToken: cancellationToken);
        }
    }

    private string CreateKeyById(string id) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:crossdomain:organization-id:{id}";
}
