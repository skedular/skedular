using HotChocolate;
using HotChocolate.Types;
using Location.Api.GraphQL.Location;
using Location.Api.Mappers;
using Location.Api.Services;

namespace Location.Api.GraphQL.PhysicalAddress;

[MutationType]
public class RootMutation(IMapper mapper)
{
    [UseResolverScope]
    public async Task<LocationPayload> AddLocationPhysicalAddressAsync(
        AddLocationPhysicalAddressInput input,
        [Service] ILocationPhysicalAddressService organizationPhysicalAddressService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Location = mapper.MapTo(await organizationPhysicalAddressService.AddAsync(mapper.MapTo(input), cancellationToken))!
        };

    [UseResolverScope]
    public async Task<LocationPayload> UpdateLocationPhysicalAddressAsync(
        UpdateLocationPhysicalAddressInput input,
        [Service] ILocationPhysicalAddressService organizationPhysicalAddressService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Location = mapper.MapTo(await organizationPhysicalAddressService.UpdateAsync(mapper.MapTo(input), cancellationToken))!
        };
}
