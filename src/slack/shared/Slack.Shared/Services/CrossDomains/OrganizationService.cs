using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Grpc.Skedular.Organization.Core.V1;
using Enterprise.Shared;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Grpc;
using Microsoft.Extensions.Caching.Memory;
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
    Api.Shared.Grpc.Skedular.Organization.Core.V1.OrganizationService.OrganizationServiceClient organizationServiceClient,
    IGrpcMapper grpcMapper,
    IMemoryCache memoryCache)
    : IOrganizationService
{
    private readonly MemoryCacheEntryOptions _cacheEntryOptions = new()
    {
        SlidingExpiration = TimeSpan.FromSeconds(30),
    };

    public async Task<Organization> AdminGetAsync(string organizationId, CancellationToken cancellationToken) =>
        (await memoryCache.GetOrCreateAsync(
            CreateKeyById(organizationId),
            async _ => grpcMapper.MapTo(
                await organizationServiceClient.Admin_GetAsync(
                    new Admin_GetInput
                    {
                        Id = organizationId,
                    },
                    organizationConfiguration.ApiKey.CreateMetadata(),
                    cancellationToken: cancellationToken)),
            _cacheEntryOptions))!;

    public async Task<Organization> AdminAddAsync(Organization organization, CancellationToken cancellationToken)
    {
        var activeTermsOfUse = await organizationServiceClient.GetActiveOrganizationTermsOfUseAsync(
            new GetActiveOrganizationTermsOfUseInput(),
            organizationConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);

        var mappedOrganization = grpcMapper.MapTo(
            await organizationServiceClient.Admin_AddAsync(
                new Admin_AddInput
                {
                    Id = organization.Id,
                    Name = organization.Name.ToSafeString(),
                    AgreedToTermsOfUse = true,
                    TermsOfUseId = activeTermsOfUse.Id,
                    Type = OrganizationType.Private,
                    BillingCycle = OrganizationBillingCycle.Monthly,
                    InvoiceDueInDays = 7,
                },
                organizationConfiguration.ApiKey.CreateMetadata(),
                cancellationToken: cancellationToken));

        Cache([mappedOrganization]);

        return mappedOrganization;
    }

    public async Task<Organization> GetAsync(string workspaceMemberId, string organizationId, CancellationToken cancellationToken) =>
        (await memoryCache.GetOrCreateAsync(
            CreateKeyById(organizationId),
            async _ => grpcMapper.MapTo(
                await organizationServiceClient.GetAsync(
                    new GetInput
                    {
                        Id = organizationId,
                    },
                    organizationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                    cancellationToken: cancellationToken)),
            _cacheEntryOptions))!;

    private void Cache(IReadOnlyList<Organization> organizations)
    {
        foreach (var organization in organizations)
        {
            var key = CreateKeyById(organization.Id);

            memoryCache.Remove(key);
            memoryCache.Set(key, organization, _cacheEntryOptions);
        }
    }

    private string CreateKeyById(string id) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:crossdomain:organization-id:{id}";
}
