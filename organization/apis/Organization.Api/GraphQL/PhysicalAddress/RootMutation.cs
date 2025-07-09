using HotChocolate;
using HotChocolate.Types;
using Organization.Api.GraphQL.Organization;
using Organization.Api.Mappers;
using Organization.Api.Services;

namespace Organization.Api.GraphQL.PhysicalAddress;

[MutationType]
public class RootMutation(IMapper mapper)
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
                mapper.MapTo(await organizationPhysicalAddressService.AddAsync(mapper.MapTo(input), cancellationToken))!
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
                mapper.MapTo(await organizationPhysicalAddressService.UpdateAsync(mapper.MapTo(input), cancellationToken))!
        };

}
