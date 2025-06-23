using Enterprise.Shared.Sanitization;
using HotChocolate;
using HotChocolate.Types;
using Location.Api.Mappers;
using Location.Api.Services;

namespace Location.Api.GraphQL.Resource;

[MutationType]
public class Mutation(IMapper mapper)
{
    [UseResolverScope]
    public async Task<ResourcePayload?> AddResourceAsync(
        AddResourceInput input,
        [Service] IResourceService resourceService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Resource = mapper.MapTo(await resourceService.AddAsync(mapper.MapTo(input), false, cancellationToken))
        };

    [UseResolverScope]
    public async Task<ResourcePayload?> UpdateResourceAsync(
        UpdateResourceInput input,
        [Service] IResourceService resourceService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Resource = mapper.MapTo(await resourceService.UpdateAsync(mapper.MapTo(input), cancellationToken))
        };

    [UseResolverScope]
    public async Task<ResourcePayload?> DeleteResourceAsync(
        DeleteResourceInput input,
        [Service] IResourceService resourceService,
        CancellationToken cancellationToken) =>
        new() { ClientMutationId = input.ClientMutationId, Resource = mapper.MapTo(await resourceService.DeleteAsync(input.Id, cancellationToken)) };

    [UseResolverScope]
    public async Task<ResourcesPayload?> DeleteResourcesAsync(
        DeleteResourcesInput input,
        [Service] IResourceService resourceService,
        CancellationToken cancellationToken)
    {
        var resources = await resourceService.DeleteAsync(input.Ids.RemoveInvalidIds()!.ToList(), cancellationToken);
        return new ResourcesPayload { ClientMutationId = input.ClientMutationId, Resources = resources.Select(mapper.MapTo) };
    }

    [UseResolverScope]
    public async Task<ResourcesPayload?> ActivateResourcesAsync(
        ActivateResourcesInput input,
        [Service] IResourceService resourceService,
        CancellationToken cancellationToken)
    {
        var resources = await resourceService.ActivateAsync(input.Ids.RemoveInvalidIds()!.ToList(), cancellationToken);
        return new ResourcesPayload { ClientMutationId = input.ClientMutationId, Resources = resources.Select(mapper.MapTo) };
    }

    [UseResolverScope]
    public async Task<ResourcesPayload?> DeactivateResourcesAsync(
        DeactivateResourcesInput input,
        [Service] IResourceService resourceService,
        CancellationToken cancellationToken)
    {
        var resources = await resourceService.DeactivateAsync(input.Ids.RemoveInvalidIds()!.ToList(), cancellationToken);
        return new ResourcesPayload { ClientMutationId = input.ClientMutationId, Resources = resources.Select(mapper.MapTo) };
    }

    [UseResolverScope]
    public async Task<ResourcePayload?> UpdateLocationResourceAvailableHoursAsync(
        UpdateLocationResourceAvailableHoursInput input,
        [Service] IResourceAvailableHoursService resourceAvailableHoursService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Resource = mapper.MapTo(
                await resourceAvailableHoursService.UpdateAvailableHoursAsync(
                    input.Id,
                    input.OverrideAvailableHours,
                    mapper.MapTo(input.AvailableHours),
                    cancellationToken))
        };
}
