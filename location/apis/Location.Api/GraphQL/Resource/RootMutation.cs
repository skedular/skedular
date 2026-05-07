using Enterprise.Shared.Sanitization;
using HotChocolate;
using HotChocolate.Types;
using Location.Api.Mappers;
using Location.Api.Services;

namespace Location.Api.GraphQL.Resource;

[MutationType]
public class RootMutation(IGraphQlMapper graphQlMapper)
{
    [UseResolverScope]
    public async Task<ResourcePayload> AddResourceAsync(
        AddResourceInput input,
        [Service] IResourceService resourceService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Resource = graphQlMapper.MapTo(await resourceService.AddAsync(graphQlMapper.MapTo(input), false, cancellationToken))
        };

    [UseResolverScope]
    public async Task<ResourcePayload> UpdateResourceAsync(
        UpdateResourceInput input,
        [Service] IResourceService resourceService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Resource = graphQlMapper.MapTo(await resourceService.UpdateAsync(graphQlMapper.MapTo(input), cancellationToken))
        };

    [UseResolverScope]
    public async Task<ResourcePayload> DeleteResourceAsync(
        DeleteResourceInput input,
        [Service] IResourceService resourceService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Resource = graphQlMapper.MapTo(await resourceService.DeleteAsync(input.Id, cancellationToken))
        };

    [UseResolverScope]
    public async Task<ResourcesPayload> DeleteResourcesAsync(
        DeleteResourcesInput input,
        [Service] IResourceService resourceService,
        CancellationToken cancellationToken)
    {
        var resources = await resourceService.DeleteAsync(input.Ids.RemoveInvalidIds().ToList(), cancellationToken);
        return new ResourcesPayload { ClientMutationId = input.ClientMutationId, Resources = resources.Select(graphQlMapper.MapTo) };
    }

    [UseResolverScope]
    public async Task<ResourcesPayload> ActivateResourcesAsync(
        ActivateResourcesInput input,
        [Service] IResourceService resourceService,
        CancellationToken cancellationToken)
    {
        var resources = await resourceService.ActivateAsync(input.Ids.RemoveInvalidIds().ToList(), cancellationToken);
        return new ResourcesPayload { ClientMutationId = input.ClientMutationId, Resources = resources.Select(graphQlMapper.MapTo) };
    }

    [UseResolverScope]
    public async Task<ResourcesPayload> DeactivateResourcesAsync(
        DeactivateResourcesInput input,
        [Service] IResourceService resourceService,
        CancellationToken cancellationToken)
    {
        var resources = await resourceService.DeactivateAsync(input.Ids.RemoveInvalidIds().ToList(), cancellationToken);
        return new ResourcesPayload { ClientMutationId = input.ClientMutationId, Resources = resources.Select(graphQlMapper.MapTo) };
    }

    [UseResolverScope]
    public async Task<ResourcePayload> UpdateLocationResourceAvailableHoursAsync(
        UpdateLocationResourceAvailableHoursInput input,
        [Service] IResourceAvailableHoursService resourceAvailableHoursService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Resource = graphQlMapper.MapTo(
                await resourceAvailableHoursService.UpdateAvailableHoursAsync(
                    input.Id,
                    input.OverrideAvailableHours,
                    graphQlMapper.MapTo(input.AvailableHours),
                    cancellationToken))
        };
}
