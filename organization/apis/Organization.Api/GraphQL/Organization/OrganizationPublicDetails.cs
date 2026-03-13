using Api.Shared.Services.Models;
using Enterprise.Shared;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types;
using Organization.Api.GraphQL.PhysicalAddress;
using Organization.Api.GraphQL.Tag;
using Organization.Api.Mappers;
using Organization.Api.Services;
using Organization.Shared.Models;

namespace Organization.Api.GraphQL.Organization;

[GraphQLName("OrganizationPublicDetails")]
public class OrganizationPublicDetails : Node
{
    [GraphQLName("uniqueId")] public string Id { get; set; } = string.Empty;

    [GraphQLName("uniqueAlphanumericName")]
    public string? UniqueAlphanumericName { get; set; }

    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("website")] public string? Website { get; set; }
    [GraphQLName("logoUrl")] public string? LogoUrl { get; set; }

    [GraphQLName("industrySubCategories")]
    public IEnumerable<OrganizationIndustrySubCategoryReferenceDetails> IndustrySubCategories { get; set; } = [];

    [GraphQLName("contactEmail")] public string? ContactEmail { get; set; }
    [GraphQLName("contactPhone")] public string? ContactPhone { get; set; }
    [GraphQLName("physicalAddress")] public OrganizationPhysicalAddressDetails? PhysicalAddress { get; set; }
    [GraphQLName("resourceTypes")] public IEnumerable<OrganizationTagDetails> ResourceTypes { get; set; } = [];
    [GraphQLName("locationSpaceTypes")] public IEnumerable<OrganizationTagDetails> LocationSpaceTypes { get; set; } = [];
    [GraphQLName("amenities")] public IEnumerable<OrganizationTagDetails> Amenities { get; set; } = [];
    [GraphQLName("featureImages")] public IEnumerable<CdnImageFile> FeatureImages { get; set; } = [];
    [GraphQLName("listingMetadata")] public ListingMetadata ListingMetadata { get; set; } = ListingMetadata.Empty;

    [GraphQLName("marketplaceListingMetadata")]
    public ListingMetadata MarketplaceListingMetadata { get; set; } = ListingMetadata.Empty;

    [UseResolverScope]
    public async Task<Connection<OrganizationTagEdge>> CustomTagsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        CustomTagOrganizationTagWhereInput? where,
        IEnumerable<OrganizationTagOrderInput>? orderBy,
        [Parent] OrganizationPublicDetails organization,
        [Service] ITagService tagService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken) =>
        await OrganizationTagsAsync(
            after,
            first,
            before,
            last,
            new TagSearchCriteria(
                organization.Id,
                organization.UniqueAlphanumericName,
                [OrganizationTagTypeConstants.Custom],
                where?.NameContains),
            orderBy,
            tagService,
            mapper,
            cancellationToken);

    [UseResolverScope]
    public async Task<Connection<OrganizationTagEdge>> ZonesAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        ZoneOrganizationTagWhereInput? where,
        IEnumerable<OrganizationTagOrderInput>? orderBy,
        [Parent] OrganizationPublicDetails organization,
        [Service] ITagService tagService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken) =>
        await OrganizationTagsAsync(
            after,
            first,
            before,
            last,
            new TagSearchCriteria(
                organization.Id,
                organization.UniqueAlphanumericName,
                [OrganizationTagTypeConstants.Zone],
                where?.NameContains),
            orderBy,
            tagService,
            mapper,
            cancellationToken);

    private async Task<Connection<OrganizationTagEdge>> OrganizationTagsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        TagSearchCriteria tagSearchCriteria,
        IEnumerable<OrganizationTagOrderInput>? orderBy,
        ITagService tagService,
        IMapper mapper,
        CancellationToken cancellationToken)
    {
        var (paginatedInfo, edges, totalCount) = await tagService.GetPaginatedTagsAsync(
            new PaginationInputParam(after, first, before, last),
            tagSearchCriteria,
            orderBy.ToSafeCollection().Select(item => new TagOrder(item.Direction, item.Field)).ToList(),
            true,
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
