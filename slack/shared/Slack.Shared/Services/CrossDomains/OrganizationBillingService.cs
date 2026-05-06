using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Grpc.Skedular.Organization.Billing.V1;
using Enterprise.Shared;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Grpc;
using Microsoft.Extensions.Caching.Memory;
using Slack.Shared.Mappers;
using Slack.Shared.Models;

namespace Slack.Shared.Services.CrossDomains;

public interface IOrganizationBillingService
{
    Task<OrganizationBillingDetails> AddAsync(
        string workspaceMemberId,
        OrganizationBillingDetails organizationBillingDetails,
        CancellationToken cancellationToken);

    Task<OrganizationBillingDetails> GetAsync(string workspaceMemberId, string organizationId, CancellationToken cancellationToken);
}

public class OrganizationBillingService(
    ApplicationConfiguration applicationConfiguration,
    OrganizationConfiguration organizationConfiguration,
    Api.Shared.Grpc.Skedular.Organization.Billing.V1.OrganizationBillingService.OrganizationBillingServiceClient organizationBillingServiceClient,
    IMapper mapper,
    IMemoryCache memoryCache) : IOrganizationBillingService
{
    private readonly MemoryCacheEntryOptions _cacheEntryOptions = new() { SlidingExpiration = TimeSpan.FromSeconds(30) };

    public async Task<OrganizationBillingDetails> AddAsync(
        string workspaceMemberId,
        OrganizationBillingDetails organizationBillingDetails,
        CancellationToken cancellationToken)
    {
        var mappedOrganizationBillingDetails = mapper.MapTo(
            await organizationBillingServiceClient.AddBillingDetailsAsync(
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

        Cache([mappedOrganizationBillingDetails]);

        return mappedOrganizationBillingDetails;
    }

    public async Task<OrganizationBillingDetails> GetAsync(string workspaceMemberId, string organizationId, CancellationToken cancellationToken) =>
        (await memoryCache.GetOrCreateAsync(
            CreateKeyById(organizationId),
            async _ => mapper.MapTo(
                await organizationBillingServiceClient.GetBillingDetailsAsync(
                    new GetBillingDetailsInput { OrganizationId = organizationId },
                    organizationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                    cancellationToken: cancellationToken)),
            _cacheEntryOptions))!;

    private void Cache(IReadOnlyList<OrganizationBillingDetails> organizationBillingDetails)
    {
        foreach (var item in organizationBillingDetails)
        {
            var key = CreateKeyById(item.Id);

            memoryCache.Remove(key);
            memoryCache.Set(key, item, _cacheEntryOptions);
        }
    }

    private string CreateKeyById(string id) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:crossdomain:organization-billing-id:{id}";
}
