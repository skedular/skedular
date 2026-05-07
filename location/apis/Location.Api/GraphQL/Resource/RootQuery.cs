using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Types.Composite;
using HotChocolate.Types.Relay;
using Location.Api.Mappers;
using Location.Api.Services;

namespace Location.Api.GraphQL.Resource;

[QueryType]
public class RootQuery(IGraphQlMapper graphQlMapper)
{
    [UseResolverScope]
    public async Task<ResourceDetails?> ResourceAsync(string id, [Service] IResourceService resourceService, CancellationToken cancellationToken) =>
        graphQlMapper.MapTo(await resourceService.GetByIdAsync(id, false, cancellationToken));

    [UseResolverScope]
    [Lookup]
    [Internal]
    public async Task<ResourceDetails?> ResourceByIdAsync(
        [ID] string id,
        [Service] IResourceService resourceService,
        CancellationToken cancellationToken) =>
        graphQlMapper.MapTo(await resourceService.GetByIdAsync(id, true, cancellationToken));
}
