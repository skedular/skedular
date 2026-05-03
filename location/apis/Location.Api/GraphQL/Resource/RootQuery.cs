using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Types.Composite;
using Location.Api.Mappers;
using Location.Api.Services;

namespace Location.Api.GraphQL.Resource;

[QueryType]
public class RootQuery(IMapper mapper)
{
    [UseResolverScope]
    public async Task<ResourceDetails?> ResourceAsync(string id, [Service] IResourceService resourceService, CancellationToken cancellationToken) =>
        mapper.MapTo(await resourceService.GetByIdAsync(id, false, cancellationToken));

    [UseResolverScope]
    [Lookup]
    [Internal]
    public async Task<ResourceDetails?> ResourceByIdAsync(
        string id,
        [Service] IResourceService resourceService,
        CancellationToken cancellationToken) =>
        mapper.MapTo(await resourceService.GetByIdAsync(id, true, cancellationToken));
}
