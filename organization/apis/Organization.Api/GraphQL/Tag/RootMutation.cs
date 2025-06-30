using Enterprise.Shared.Sanitization;
using HotChocolate;
using HotChocolate.Types;
using Organization.Api.Mappers;
using Organization.Api.Services;

namespace Organization.Api.GraphQL.Tag;

[MutationType]
public class RootMutation(IMapper mapper)
{
    [UseResolverScope]
    public async Task<OrganizationTagPayload> AddCustomTagAsync(
        AddCustomTagInput input,
        [Service] ITagService tagService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            OrganizationTag = mapper.MapTo(await tagService.AddAsync(mapper.MapTo(input), false, cancellationToken))!
        };

    [UseResolverScope]
    public async Task<OrganizationTagPayload> UpdateCustomTagAsync(
        UpdateCustomTagInput input,
        [Service] ITagService tagService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            OrganizationTag = mapper.MapTo(await tagService.UpdateAsync(mapper.MapTo(input), cancellationToken))!
        };

    [UseResolverScope]
    public async Task<OrganizationTagPayload> DeleteCustomTagAsync(
        DeleteCustomTagInput input,
        [Service] ITagService tagService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId, OrganizationTag = mapper.MapTo(await tagService.DeleteAsync(input.Id, cancellationToken))!
        };

    [UseResolverScope]
    public async Task<OrganizationTagsPayload> DeleteCustomTagsAsync(
        DeleteCustomTagsInput input,
        [Service] ITagService tagService,
        CancellationToken cancellationToken)
    {
        var tags = await tagService.DeleteAsync(input.Ids.RemoveInvalidIds()!.ToList(), cancellationToken);
        return new OrganizationTagsPayload { ClientMutationId = input.ClientMutationId, OrganizationTags = tags.Select(mapper.MapTo).ToArray()! };
    }

    [UseResolverScope]
    public async Task<OrganizationTagPayload> AddZoneAsync(
        AddZoneInput input,
        [Service] ITagService tagService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            OrganizationTag = mapper.MapTo(await tagService.AddAsync(mapper.MapTo(input), false, cancellationToken))!
        };

    [UseResolverScope]
    public async Task<OrganizationTagPayload> UpdateZoneAsync(
        UpdateZoneInput input,
        [Service] ITagService tagService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            OrganizationTag = mapper.MapTo(await tagService.UpdateAsync(mapper.MapTo(input), cancellationToken))!
        };

    [UseResolverScope]
    public async Task<OrganizationTagPayload> DeleteZoneAsync(
        DeleteZoneInput input,
        [Service] ITagService tagService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId, OrganizationTag = mapper.MapTo(await tagService.DeleteAsync(input.Id, cancellationToken))!
        };

    [UseResolverScope]
    public async Task<OrganizationTagsPayload> DeleteZonesAsync(
        DeleteZonesInput input,
        [Service] ITagService tagService,
        CancellationToken cancellationToken)
    {
        var tags = await tagService.DeleteAsync(input.Ids.RemoveInvalidIds()!.ToList(), cancellationToken);
        return new OrganizationTagsPayload { ClientMutationId = input.ClientMutationId, OrganizationTags = tags.Select(item => mapper.MapTo(item)!) };
    }

    [UseResolverScope]
    public async Task<OrganizationTagPayload> AddProductTagAsync(
        AddProductTagInput input,
        [Service] ITagService tagService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            OrganizationTag = mapper.MapTo(await tagService.AddAsync(mapper.MapTo(input), false, cancellationToken))!
        };

    [UseResolverScope]
    public async Task<OrganizationTagPayload> UpdateProductTagAsync(
        UpdateProductTagInput input,
        [Service] ITagService tagService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            OrganizationTag = mapper.MapTo(await tagService.UpdateAsync(mapper.MapTo(input), cancellationToken))!
        };

    [UseResolverScope]
    public async Task<OrganizationTagPayload> DeleteProductTagAsync(
        DeleteProductTagInput input,
        [Service] ITagService tagService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId, OrganizationTag = mapper.MapTo(await tagService.DeleteAsync(input.Id, cancellationToken))!
        };

    [UseResolverScope]
    public async Task<OrganizationTagsPayload> DeleteProductTagsAsync(
        DeleteProductTagsInput input,
        [Service] ITagService tagService,
        CancellationToken cancellationToken)
    {
        var tags = await tagService.DeleteAsync(input.Ids.RemoveInvalidIds()!.ToList(), cancellationToken);
        return new OrganizationTagsPayload { ClientMutationId = input.ClientMutationId, OrganizationTags = tags.Select(item => mapper.MapTo(item)!) };
    }

    [UseResolverScope]
    public async Task<OrganizationTagPayload> AddLocationTagAsync(
        AddLocationTagInput input,
        [Service] ITagService tagService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            OrganizationTag = mapper.MapTo(await tagService.AddAsync(mapper.MapTo(input), false, cancellationToken))!
        };

    [UseResolverScope]
    public async Task<OrganizationTagPayload> UpdateLocationTagAsync(
        UpdateLocationTagInput input,
        [Service] ITagService tagService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            OrganizationTag = mapper.MapTo(await tagService.UpdateAsync(mapper.MapTo(input), cancellationToken))!
        };

    [UseResolverScope]
    public async Task<OrganizationTagPayload> DeleteLocationTagAsync(
        DeleteLocationTagInput input,
        [Service] ITagService tagService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId, OrganizationTag = mapper.MapTo(await tagService.DeleteAsync(input.Id, cancellationToken))!
        };

    [UseResolverScope]
    public async Task<OrganizationTagsPayload> DeleteLocationTagsAsync(
        DeleteLocationTagsInput input,
        [Service] ITagService tagService,
        CancellationToken cancellationToken)
    {
        var tags = await tagService.DeleteAsync(input.Ids.RemoveInvalidIds()!.ToList(), cancellationToken);
        return new OrganizationTagsPayload { ClientMutationId = input.ClientMutationId, OrganizationTags = tags.Select(item => mapper.MapTo(item)!) };
    }
}
