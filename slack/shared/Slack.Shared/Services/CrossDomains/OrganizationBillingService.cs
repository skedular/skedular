using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Services.Grpc.Skedular.Organization.V1;
using Enterprise.Shared;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Grpc;
using Microsoft.Extensions.Caching.Hybrid;
using Slack.Shared.Mappers;
using Slack.Shared.Models;

namespace Slack.Shared.Services.CrossDomains;

public interface IOrganizationBillingService
{
    Task<OrganizationBillingDetails> AddAsync(
        string workspaceMemberId,
        OrganizationBillingDetails organizationBillingDetails,
        CancellationToken cancellationToken);

    Task<OrganizationBillingDetails> UpdateAsync(
        string workspaceMemberId,
        OrganizationBillingDetails organizationBillingDetails,
        CancellationToken cancellationToken);

    Task<OrganizationBillingDetails> GetAsync(string workspaceMemberId, string organizationId, CancellationToken cancellationToken);
}

public class OrganizationBillingService(
    ApplicationConfiguration applicationConfiguration,
    OrganizationConfiguration organizationConfiguration,
    Api.Shared.Services.Grpc.Skedular.Organization.V1.OrganizationService.OrganizationServiceClient organizationServiceClient,
    IMapper mapper,
    HybridCache hybridCache) : IOrganizationBillingService
{
    public async Task<OrganizationBillingDetails> AddAsync(
        string workspaceMemberId,
        OrganizationBillingDetails organizationBillingDetails,
        CancellationToken cancellationToken)
    {
        var mappedOrganizationBillingDetails = mapper.MapTo(
            await organizationServiceClient.AddBillingDetailsAsync(
                new AddBillingDetailsInput
                {
                    Id = organizationBillingDetails.Id.ToSafeString(),
                    CompanyName = organizationBillingDetails.CompanyName.ToSafeString(),
                    Email = organizationBillingDetails.Email.ToSafeString(),
                    AddressLine1 = organizationBillingDetails.AddressLine1.ToSafeString(),
                    AddressLine2 = organizationBillingDetails.AddressLine2.ToSafeString(),
                    Suburb = organizationBillingDetails.Suburb.ToSafeString(),
                    City = organizationBillingDetails.City.ToSafeString(),
                    Province = organizationBillingDetails.Province.ToSafeString(),
                    Zipcode = organizationBillingDetails.Zipcode.ToSafeString(),
                    CountryCode = organizationBillingDetails.CountryCode.ToSafeString(),
                    Country = organizationBillingDetails.Country.ToSafeString(),
                    OrganizationId = organizationBillingDetails.Organization.Id.ToSafeString()
                },
                organizationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                cancellationToken: cancellationToken));

        await CacheAsync([mappedOrganizationBillingDetails], cancellationToken);

        return mappedOrganizationBillingDetails;
    }

    public async Task<OrganizationBillingDetails> UpdateAsync(
        string workspaceMemberId,
        OrganizationBillingDetails organizationBillingDetails,
        CancellationToken cancellationToken)
    {
        var mappedOrganizationBillingDetails = mapper.MapTo(
            await organizationServiceClient.UpdateBillingDetailsAsync(
                new UpdateBillingDetailsInput
                {
                    Id = organizationBillingDetails.Id.ToSafeString(),
                    CompanyName = organizationBillingDetails.CompanyName.ToSafeString(),
                    Email = organizationBillingDetails.Email.ToSafeString(),
                    AddressLine1 = organizationBillingDetails.AddressLine1.ToSafeString(),
                    AddressLine2 = organizationBillingDetails.AddressLine2.ToSafeString(),
                    Suburb = organizationBillingDetails.Suburb.ToSafeString(),
                    City = organizationBillingDetails.City.ToSafeString(),
                    Province = organizationBillingDetails.Province.ToSafeString(),
                    Zipcode = organizationBillingDetails.Zipcode.ToSafeString(),
                    CountryCode = organizationBillingDetails.CountryCode.ToSafeString(),
                    Country = organizationBillingDetails.Country.ToSafeString()
                },
                organizationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                cancellationToken: cancellationToken));

        await CacheAsync([mappedOrganizationBillingDetails], cancellationToken);

        return mappedOrganizationBillingDetails;
    }

    public async Task<OrganizationBillingDetails> GetAsync(string workspaceMemberId, string organizationId, CancellationToken cancellationToken) =>
        await hybridCache.GetOrCreateAsync(
            CreateKeyById(organizationId),
            async ct => mapper.MapTo(
                await organizationServiceClient.GetBillingDetailsAsync(
                    new GetBillingDetailsInput { OrganizationId = organizationId },
                    organizationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                    cancellationToken: ct)),
            new HybridCacheEntryOptions { Expiration = TimeSpan.FromSeconds(30), LocalCacheExpiration = TimeSpan.FromSeconds(30) },
            cancellationToken: cancellationToken);

    private async Task CacheAsync(ICollection<OrganizationBillingDetails> organizationBillingDetails, CancellationToken cancellationToken)
    {
        foreach (var item in organizationBillingDetails)
        {
            var key = CreateKeyById(item.Id);

            await hybridCache.RemoveAsync(key, cancellationToken);
            await hybridCache.SetAsync(
                key,
                item,
                new HybridCacheEntryOptions { Expiration = TimeSpan.FromSeconds(30), LocalCacheExpiration = TimeSpan.FromSeconds(30) },
                cancellationToken: cancellationToken);
        }
    }

    private string CreateKeyById(string id) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:crossdomain:organization-billing-id:{id}";
}
