using HotChocolate;
using HotChocolate.Types;
using Location.Api.GraphQL.Location;
using Location.Api.Mappers;
using Location.Api.Services;

namespace Location.Api.GraphQL.Ownership;

[MutationType]
public class RootMutation(IMapper mapper)
{
    [UseResolverScope]
    public async Task<LocationPayload> ClaimLocationOwnershipAsync(
        ClaimLocationOwnershipInput input,
        [Service] ILocationOwnershipService locationOwnershipService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Location = mapper.MapTo(await locationOwnershipService.ClaimOwnershipAsync(
                input.UniqueClaimCode,
                input.OrganizationId,
                input.OrganizationUniqueAlphanumericName,
                cancellationToken))!
        };
}
