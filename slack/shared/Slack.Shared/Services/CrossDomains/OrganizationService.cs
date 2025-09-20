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
}

public class OrganizationService(
    ApplicationConfiguration applicationConfiguration,
    OrganizationConfiguration organizationConfiguration,
    Api.Shared.Services.Grpc.Skedular.Organization.V1.OrganizationService.OrganizationServiceClient organizationServiceClient,
    IMapper mapper,
    HybridCache hybridCache)
    : IOrganizationService
{
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
                    Type = OrganizationType.Private,
                    IsListable = true
                },
                organizationConfiguration.ApiKey.CreateMetadata(),
                cancellationToken: cancellationToken));

        await CacheAsync([mappedOrganization], cancellationToken);

        return mappedOrganization;
    }

    private async Task CacheAsync(ICollection<Organization> organizations, CancellationToken cancellationToken)
    {
        foreach (var organization in organizations)
        {
            var key = CreateKeyById(organization.Id);

            await hybridCache.RemoveAsync(key, cancellationToken);
            await hybridCache.SetAsync(
                key,
                organization,
                new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(5), LocalCacheExpiration = TimeSpan.FromMinutes(5) },
                cancellationToken: cancellationToken);
        }
    }

    private string CreateKeyById(string id) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:crossdomain:organization-id:{id}";
}
