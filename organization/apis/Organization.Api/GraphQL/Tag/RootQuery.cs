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
    public async Task<Connection<OrganizationTagEdge>> CustomTagsAsync(
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
            new TagSearchCriteria(
                where.OrganizationId,
                where.OrganizationUniqueAlphanumericName,
                OrganizationTagTypeConstants.Custom,
                where.NameContains),
            orderBy,
            tagService,
            cancellationToken);

    [UseResolverScope]
    public async Task<OrganizationTagDetails?> CustomTagAsync(string id, [Service] ITagService tagService, CancellationToken cancellationToken) =>
        mapper.MapTo(await tagService.GetByIdAsync(id, cancellationToken));

    [UseResolverScope]
    public async Task<Connection<OrganizationTagEdge>> ZonesAsync(
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
            new TagSearchCriteria(
                where.OrganizationId,
                where.OrganizationUniqueAlphanumericName,
                OrganizationTagTypeConstants.Zone,
                where.NameContains),
            orderBy,
            tagService,
            cancellationToken);

    [UseResolverScope]
    public async Task<OrganizationTagDetails?> ZoneAsync(string id, [Service] ITagService tagService, CancellationToken cancellationToken) =>
        mapper.MapTo(await tagService.GetByIdAsync(id, cancellationToken));

    [UseResolverScope]
    public async Task<Connection<OrganizationTagEdge>> ProductTagsAsync(
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
            new TagSearchCriteria(
                where.OrganizationId,
                where.OrganizationUniqueAlphanumericName,
                OrganizationTagTypeConstants.Product,
                where.NameContains),
            orderBy,
            tagService,
            cancellationToken);

    [UseResolverScope]
    public async Task<OrganizationTagDetails?> ProductTagAsync(string id, [Service] ITagService tagService, CancellationToken cancellationToken) =>
        mapper.MapTo(await tagService.GetByIdAsync(id, cancellationToken));

    [UseResolverScope]
    public async Task<Connection<OrganizationTagEdge>> LocationTagsAsync(
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
            new TagSearchCriteria(
                where.OrganizationId,
                where.OrganizationUniqueAlphanumericName,
                OrganizationTagTypeConstants.Location, where.NameContains),
            orderBy,
            tagService,
            cancellationToken);

    [UseResolverScope]
    public async Task<OrganizationTagDetails?> LocationTagAsync(string id, [Service] ITagService tagService, CancellationToken cancellationToken) =>
        mapper.MapTo(await tagService.GetByIdAsync(id, cancellationToken));

    private async Task<Connection<OrganizationTagEdge>> OrganizationTagsAsync(
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

        return new Connection<OrganizationTagEdge>
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
