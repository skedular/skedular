using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Services.Grpc.Skedular.Organization.V1;
using Enterprise.Shared;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Grpc;
using Microsoft.Extensions.Caching.Hybrid;
using Slack.Shared.Mappers;
using Organization = Slack.Shared.Models.Organization;
using OrganizationMember = Slack.Shared.Models.OrganizationMember;

namespace Slack.Shared.Services.CrossDomains;

public interface IOrganizationService
{
    Task<Organization> AdminAddAsync(string organizationId, string? name, CancellationToken cancellationToken);
    Task<Organization> GetAsync(string workspaceMemberId, string organizationId, CancellationToken cancellationToken);

    Task<(ICollection<OrganizationMember>, MemberConnection)> GetPaginatedMembersAsync(
        string workspaceMemberId,
        string organizationId,
        string? nameContains,
        string? after,
        int? first,
        string? before,
        int? last,
        CancellationToken cancellationToken);
}

public class OrganizationService(
    ApplicationConfiguration applicationConfiguration,
    OrganizationConfiguration organizationConfiguration,
    Api.Shared.Services.Grpc.Skedular.Organization.V1.OrganizationService.OrganizationServiceClient organizationServiceClient,
    IMapper mapper,
    HybridCache hybridCache,
    ICustomerService customerService)
    : IOrganizationService
{
    public async Task<Organization> AdminAddAsync(string organizationId, string? name, CancellationToken cancellationToken)
    {
        var activeTermsOfUse = await organizationServiceClient.GetActiveOrganizationTermsOfUseAsync(
            new GetActiveOrganizationTermsOfUseInput(),
            organizationConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);

        var organization = mapper.MapTo(
            await organizationServiceClient.Admin_AddAsync(
                new Admin_AddInput
                {
                    Id = organizationId,
                    Name = name.ToSafeString(),
                    AgreedToTermsOfUse = true,
                    TermsOfUseId = activeTermsOfUse.Id,
                    Type = OrganizationType.Private,
                    IsListable = true
                },
                organizationConfiguration.ApiKey.CreateMetadata(),
                cancellationToken: cancellationToken));

        await CacheOrganizationAsync([organization], cancellationToken);

        return organization;
    }

    public async Task<Organization> GetAsync(string workspaceMemberId, string organizationId, CancellationToken cancellationToken) =>
        await hybridCache.GetOrCreateAsync(
            CreateKeyById(organizationId),
            async ct => mapper.MapTo(
                await organizationServiceClient.GetAsync(
                    new GetInput { Id = organizationId },
                    organizationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                    cancellationToken: ct)),
            new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(5), LocalCacheExpiration = TimeSpan.FromMinutes(5) },
            cancellationToken: cancellationToken);

    public async Task<(ICollection<OrganizationMember>, MemberConnection)> GetPaginatedMembersAsync(
        string workspaceMemberId,
        string organizationId,
        string? nameContains,
        string? after,
        int? first,
        string? before,
        int? last,
        CancellationToken cancellationToken)
    {
        var getPaginatedMembersInput = new GetPaginatedMembersInput
        {
            First = first.ToNullInt(),
            After = after.ToSafeString(),
            Before = before.ToSafeString(),
            Last = last.ToNullInt(),
            Where = new MemberWhereInput { OrganizationId = organizationId, NameContains = nameContains.ToSafeString() }
        };

        getPaginatedMembersInput.OrderBy.Add(new MemberOrderInput { Direction = OrderDirection.Ascending, Field = MemberOrderField.Name });

        var memberConnection = await organizationServiceClient.GetPaginatedMembersAsync(
            getPaginatedMembersInput,
            organizationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
            cancellationToken: cancellationToken);

        var customers = await Task.WhenAll(
            memberConnection.Edges.Select(item => customerService.GetByIdAsync(workspaceMemberId, item.Node.CustomerId, cancellationToken)));

        return (memberConnection.Edges
            .Select(item => mapper.MapTo(item.Node))
            .Select(item =>
            {
                var matchingCustomer = customers.FirstOrDefault(customer => customer.Id == item.Customer.Id);
                if (matchingCustomer is not null)
                {
                    item.Customer = matchingCustomer;
                }

                return item;
            }).ToList(), memberConnection);
    }

    private async Task CacheOrganizationAsync(ICollection<Organization> organizations, CancellationToken cancellationToken)
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
