using HotChocolate;
using HotChocolate.Types;
using Organization.Api.GraphQL.Organization;
using Organization.Api.Mappers;
using Organization.Api.Services;

namespace Organization.Api.GraphQL.PhysicalAddress;

[MutationType]
public class RootMutation(IGraphQlMapper graphQlMapper)
{
    [UseResolverScope]
    public async Task<OrganizationPayload> AddOrganizationPhysicalAddressAsync(
        AddOrganizationPhysicalAddressInput input,
        [Service] IOrganizationPhysicalAddressService organizationPhysicalAddressService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Organization =
                graphQlMapper.MapTo(await organizationPhysicalAddressService.AddAsync(graphQlMapper.MapTo(input), cancellationToken))!
        };

    [UseResolverScope]
    public async Task<OrganizationPayload> UpdateOrganizationPhysicalAddressAsync(
        UpdateOrganizationPhysicalAddressInput input,
        [Service] IOrganizationPhysicalAddressService organizationPhysicalAddressService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Organization =
                graphQlMapper.MapTo(await organizationPhysicalAddressService.UpdateAsync(graphQlMapper.MapTo(input), cancellationToken))!
        };
}
