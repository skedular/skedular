using Api.Shared.Services.Models;
using Enterprise.Shared;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types;
using Organization.Api.Mappers;
using Organization.Api.Services;
using Organization.Shared.Models;

namespace Organization.Api.GraphQL.Tag;

[QueryType]
public class Query(IMapper mapper)
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
    public async Task<OrganizationTagConnection> CustomTagsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        CustomTagOrganizationTagWhereInput where,
        IEnumerable<OrganizationTagOrderInput>? orderBy,
        [Service] ITagService tagService,
        CancellationToken cancellationToken) =>
        await OrganizationTagsAsync(
            after,
            first,
            before,
            last,
            new TagSearchCriteria(where.OrganizationId, OrganizationTagTypeConstants.Custom, where.NameContains),
            orderBy,
            tagService,
            cancellationToken);

    [UseResolverScope]
    public async Task<OrganizationTagDetails?> CustomTagAsync(string id, [Service] ITagService tagService, CancellationToken cancellationToken) =>
        mapper.MapTo(await tagService.GetByIdAsync(id, cancellationToken));

    [UseResolverScope]
    public async Task<OrganizationTagConnection> ZonesAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        ZoneOrganizationTagWhereInput where,
        IEnumerable<OrganizationTagOrderInput>? orderBy,
        [Service] ITagService tagService,
        CancellationToken cancellationToken) =>
        await OrganizationTagsAsync(
            after,
            first,
            before,
            last,
            new TagSearchCriteria(where.OrganizationId, OrganizationTagTypeConstants.Zone, where.NameContains),
            orderBy,
            tagService,
            cancellationToken);

    [UseResolverScope]
    public async Task<OrganizationTagDetails?> ZoneAsync(string id, [Service] ITagService tagService, CancellationToken cancellationToken) =>
        mapper.MapTo(await tagService.GetByIdAsync(id, cancellationToken));

    [UseResolverScope]
    public async Task<OrganizationTagConnection> ProductTagsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        ProductTagOrganizationTagWhereInput where,
        IEnumerable<OrganizationTagOrderInput>? orderBy,
        [Service] ITagService tagService,
        CancellationToken cancellationToken) =>
        await OrganizationTagsAsync(
            after,
            first,
            before,
            last,
            new TagSearchCriteria(where.OrganizationId, OrganizationTagTypeConstants.Product, where.NameContains),
            orderBy,
            tagService,
            cancellationToken);

    [UseResolverScope]
    public async Task<OrganizationTagDetails?> ProductTagAsync(string id, [Service] ITagService tagService, CancellationToken cancellationToken) =>
        mapper.MapTo(await tagService.GetByIdAsync(id, cancellationToken));

    [UseResolverScope]
    public async Task<OrganizationTagConnection> LocationTagsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        LocationTagOrganizationTagWhereInput where,
        IEnumerable<OrganizationTagOrderInput>? orderBy,
        [Service] ITagService tagService,
        CancellationToken cancellationToken) =>
        await OrganizationTagsAsync(
            after,
            first,
            before,
            last,
            new TagSearchCriteria(where.OrganizationId, OrganizationTagTypeConstants.Location, where.NameContains),
            orderBy,
            tagService,
            cancellationToken);

    [UseResolverScope]
    public async Task<OrganizationTagDetails?> LocationTagAsync(string id, [Service] ITagService tagService, CancellationToken cancellationToken) =>
        mapper.MapTo(await tagService.GetByIdAsync(id, cancellationToken));

    private async Task<OrganizationTagConnection> OrganizationTagsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        TagSearchCriteria tagSearchCriteria,
        IEnumerable<OrganizationTagOrderInput>? orderBy,
        ITagService tagService,
        CancellationToken cancellationToken)
    {
        var (paginatedInfo, edges, totalCount) = await tagService.GetPaginatedTagsAsync(
            new PaginationInputParam(after, first, before, last),
            tagSearchCriteria,
            orderBy.ToSafeCollection().Select(item => new TagOrder(item.Direction, item.Field)).ToList(),
            cancellationToken);

        return new OrganizationTagConnection
        {
            PageInfo = new PageInfo
            {
                HasNextPage = paginatedInfo.HasNextPage,
                HasPreviousPage = paginatedInfo.HasPreviousPage,
                StartCursor = paginatedInfo.StartCursor,
                EndCursor = paginatedInfo.EndCursor
            },
            Edges = edges.Select(mapper.MapTo),
            TotalCount = totalCount
        };
    }
}
