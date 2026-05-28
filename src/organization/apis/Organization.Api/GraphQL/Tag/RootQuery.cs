using Api.Shared.Services.Models;
using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Types.Composite;
using HotChocolate.Types.Relay;
using Organization.Api.Mappers;
using Organization.Api.Services;

namespace Organization.Api.GraphQL.Tag;

[QueryType]
public class RootQuery(IGraphQlMapper graphQlMapper)
{
    [UseResolverScope]
    public OrganizationTagType DeskResourceType() => OrganizationTagType.ResourceDesk;

    [UseResolverScope]
    public OrganizationTagType RoomResourceType() => OrganizationTagType.ResourceRoom;

    [UseResolverScope]
    public OrganizationTagType ParkingResourceType() => OrganizationTagType.ResourceParking;

    [UseResolverScope]
    public OrganizationTagType OtherResourceType() => OrganizationTagType.ResourceOthers;

    [UseResolverScope]
    public async Task<OrganizationTagDetails?> CustomTagAsync(string id, [Service] ITagService tagService, CancellationToken cancellationToken) =>
        graphQlMapper.MapTo(await tagService.GetByIdAsync(id, false, cancellationToken));

    [UseResolverScope]
    [Lookup]
    [Internal]
    public async Task<OrganizationTagDetails?> CustomTagByIdAsync(
        [ID] string id,
        [Service] ITagService tagService,
        CancellationToken cancellationToken) =>
        await CustomTagAsync(id, tagService, cancellationToken);

    [UseResolverScope]
    public async Task<OrganizationTagDetails?> ZoneAsync(string id, [Service] ITagService tagService, CancellationToken cancellationToken) =>
        graphQlMapper.MapTo(await tagService.GetByIdAsync(id, false, cancellationToken));

    [UseResolverScope]
    [Lookup]
    [Internal]
    public async Task<OrganizationTagDetails?> ZoneByIdAsync(
        [ID] string id,
        [Service] ITagService tagService,
        CancellationToken cancellationToken) =>
        await ZoneAsync(id, tagService, cancellationToken);

    [UseResolverScope]
    public async Task<OrganizationTagDetails?> ProductTagAsync(string id, [Service] ITagService tagService, CancellationToken cancellationToken) =>
        graphQlMapper.MapTo(await tagService.GetByIdAsync(id, false, cancellationToken));

    [UseResolverScope]
    [Lookup]
    [Internal]
    public async Task<OrganizationTagDetails?> ProductTagByIdAsync(
        [ID] string id,
        [Service] ITagService tagService,
        CancellationToken cancellationToken) =>
        await ProductTagAsync(id, tagService, cancellationToken);
}
