using HotChocolate;
using HotChocolate.Types;
using Location.Api.GraphQL.Location;
using Location.Api.Mappers;
using Location.Api.Services;

namespace Location.Api.GraphQL.PhysicalAddress;

[MutationType]
public class RootMutation(IGraphQlMapper graphQlMapper)
{
    [UseResolverScope]
    public async Task<LocationPayload> AddLocationPhysicalAddressAsync(
        AddLocationPhysicalAddressInput input,
        [Service] ILocationPhysicalAddressService organizationPhysicalAddressService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Location = graphQlMapper.MapTo(await organizationPhysicalAddressService.AddAsync(graphQlMapper.MapTo(input), cancellationToken))!
        };

    [UseResolverScope]
    public async Task<LocationPayload> UpdateLocationPhysicalAddressAsync(
        UpdateLocationPhysicalAddressInput input,
        [Service] ILocationPhysicalAddressService organizationPhysicalAddressService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Location = graphQlMapper.MapTo(await organizationPhysicalAddressService.UpdateAsync(graphQlMapper.MapTo(input), cancellationToken))!
        };
}
