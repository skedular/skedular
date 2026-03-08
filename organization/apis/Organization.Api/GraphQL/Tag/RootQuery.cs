using Api.Shared.Services.Models;
using HotChocolate;
using HotChocolate.Fusion.SourceSchema.Types;
using HotChocolate.Types;
using Organization.Api.Mappers;
using Organization.Api.Services;

namespace Organization.Api.GraphQL.Tag;

[QueryType]
public class RootQuery(IMapper mapper)
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
        mapper.MapTo(await tagService.GetByIdAsync(id, false, cancellationToken));

    [UseResolverScope]
    [Lookup]
    [Internal]
    public async Task<OrganizationTagDetails?> CustomTagByIdAsync(
        string id,
        [Service] ITagService tagService,
        CancellationToken cancellationToken) =>
        await CustomTagAsync(id, tagService, cancellationToken);

    [UseResolverScope]
    public async Task<OrganizationTagDetails?> ZoneAsync(string id, [Service] ITagService tagService, CancellationToken cancellationToken) =>
        mapper.MapTo(await tagService.GetByIdAsync(id, false, cancellationToken));

    [UseResolverScope]
    [Lookup]
    [Internal]
    public async Task<OrganizationTagDetails?> ZoneByIdAsync(
        string id,
        [Service] ITagService tagService,
        CancellationToken cancellationToken) =>
        await ZoneAsync(id, tagService, cancellationToken);

    [UseResolverScope]
    public async Task<OrganizationTagDetails?> ProductTagAsync(string id, [Service] ITagService tagService, CancellationToken cancellationToken) =>
        mapper.MapTo(await tagService.GetByIdAsync(id, false, cancellationToken));

    [UseResolverScope]
    [Lookup]
    [Internal]
    public async Task<OrganizationTagDetails?> ProductTagByIdAsync(
        string id,
        [Service] ITagService tagService,
        CancellationToken cancellationToken) =>
        await ProductTagAsync(id, tagService, cancellationToken);
}
