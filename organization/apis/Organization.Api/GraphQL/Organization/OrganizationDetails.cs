using Api.Shared.Services.Models;
using Enterprise.Shared;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types;
using Organization.Api.GraphQL.Analytics;
using Organization.Api.GraphQL.Member;
using Organization.Api.GraphQL.Offering;
using Organization.Api.GraphQL.PhysicalAddress;
using Organization.Api.GraphQL.Sso;
using Organization.Api.GraphQL.Tag;
using Organization.Api.Mappers;
using Organization.Api.Services;
using Organization.Shared.Models;
using OrganizationBillingDetails = Organization.Api.GraphQL.Billing.OrganizationBillingDetails;
using OrganizationTaxDetails = Organization.Api.GraphQL.TaxDetails.OrganizationTaxDetails;

namespace Organization.Api.GraphQL.Organization;

[GraphQLName("OrganizationDetails")]
public class OrganizationDetails : Node
{
    [GraphQLName("uniqueAlphanumericName")]
    public string? UniqueAlphanumericName { get; set; }

    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("website")] public string? Website { get; set; }
    [GraphQLName("logoUrl")] public string? LogoUrl { get; set; }
    [GraphQLName("type")] public OrganizationTypeDetails Type { get; set; } = new();

    [GraphQLName("agreedToTermsOfUse")] public bool AgreedToTermsOfUse { get; set; }
    [GraphQLName("termsOfUse")] public OrganizationTermsOfUse? TermsOfUse { get; set; }

    [GraphQLName("industrySubCategories")]
    public IEnumerable<OrganizationIndustrySubCategoryReferenceDetails> IndustrySubCategories { get; set; } = [];

    [GraphQLName("contactEmail")] public string? ContactEmail { get; set; }
    [GraphQLName("contactPhone")] public string? ContactPhone { get; set; }
    [GraphQLName("isOwnershipVerified")] public bool IsOwnershipVerified { get; set; }

    [GraphQLName("stripeAuthorizeExistingConnectAccountUrl")]
    public string StripeAuthorizeExistingConnectAccountUrl { get; set; } = string.Empty;

    [GraphQLName("physicalAddress")] public OrganizationPhysicalAddressDetails? PhysicalAddress { get; set; }
    [GraphQLName("billingDetails")] public OrganizationBillingDetails? BillingDetails { get; set; }

    [GraphQLName("availableOfferings")] public IEnumerable<OrganizationOfferingDetails> AvailableOfferings { get; set; } = [];
    [GraphQLName("activeOffering")] public OrganizationActiveOfferingDetails ActiveOffering { get; set; } = new();
    [GraphQLName("hasFutureBooking")] public bool HasFutureBooking { get; set; }
    [GraphQLName("hasLocation")] public bool HasLocation { get; set; }
    [GraphQLName("hasTeam")] public bool HasTeam { get; set; }
    [GraphQLName("canModify")] public bool CanModify { get; set; }
    [GraphQLName("canDelete")] public bool CanDelete { get; set; }
    [GraphQLName("canInvitePeople")] public bool CanInvitePeople { get; set; }
    [GraphQLName("canViewAnalytics")] public bool CanViewAnalytics { get; set; }
    [GraphQLName("isMyOnboardingDone")] public bool IsMyOnboardingDone { get; set; }
    [GraphQLName("resourceTypes")] public IEnumerable<OrganizationTagDetails> ResourceTypes { get; set; } = [];
    [GraphQLName("locationSpaceTypes")] public IEnumerable<OrganizationTagDetails> LocationSpaceTypes { get; set; } = [];
    [GraphQLName("amenities")] public IEnumerable<OrganizationTagDetails> Amenities { get; set; } = [];
    [GraphQLName("paymentMethods")] public IEnumerable<OrganizationPaymentMethod> PaymentMethods { get; set; } = [];

    [GraphQLName("hasAttachedPaymentMethod")]
    public bool HasAttachedPaymentMethod { get; set; }

    [GraphQLName("ssoSettings")] public OrganizationSsoSettingsDetails? SsoSettings { get; set; }
    [GraphQLName("taxDetails")] public OrganizationTaxDetails? TaxDetails { get; set; }
    [GraphQLName("featureImages")] public IEnumerable<CdnImageFile> FeatureImages { get; set; } = [];
    [GraphQLName("listingMetadata")] public ListingMetadata ListingMetadata { get; set; } = ListingMetadata.Empty();

    [GraphQLName("marketplaceListingMetadata")]
    public ListingMetadata MarketplaceListingMetadata { get; set; } = ListingMetadata.Empty();

    [UseResolverScope]
    public async Task<Connection<OrganizationMemberEdge>> MembersAsync(
        string? after,
        int? first,
        string? before, int? last,
        OrganizationMemberWhereInput? where,
        IEnumerable<OrganizationMemberOrderInput>? orderBy,
        [Parent] OrganizationDetails organization,
        [Service] IOrganizationMemberService organizationMemberService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var (paginatedInfo, edges, totalCount) = await organizationMemberService.GetPaginatedOrganizationMembersAsync(
            new PaginationInputParam(after, first, before, last),
            new OrganizationMemberSearchCriteria(organization.Id, organization.UniqueAlphanumericName, where?.NameContains, where?.CustomerId),
            orderBy.ToSafeCollection().Select(item => new OrganizationMemberOrder(item.Direction, item.Field)).ToList(),
            cancellationToken);

        return new Connection<OrganizationMemberEdge>
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

    [UseResolverScope]
    public async Task<Connection<OrganizationTagEdge>> CustomTagsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        CustomTagOrganizationTagWhereInput? where,
        IEnumerable<OrganizationTagOrderInput>? orderBy,
        [Parent] OrganizationDetails organization,
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
        [Parent] OrganizationDetails organization,
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

    [UseResolverScope]
    public async Task<Connection<OrganizationTagEdge>> ProductTagsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        ProductTagOrganizationTagWhereInput? where,
        IEnumerable<OrganizationTagOrderInput>? orderBy,
        [Parent] OrganizationDetails organization,
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
                [OrganizationTagTypeConstants.Product],
                where?.NameContains),
            orderBy,
            tagService,
            mapper,
            cancellationToken);

    [UseResolverScope]
    public async Task<OrganizationAnalytics> AnalyticsAsync(
        DateTimeOffset from,
        DateTimeOffset until,
        [Parent] OrganizationDetails organization,
        [Service] IOrganizationAnalyticsService organizationAnalyticsService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var organizationAnalytics = await organizationAnalyticsService.GetAnalyticsAsync(
            organization.Id,
            organization.UniqueAlphanumericName,
            from,
            until,
            cancellationToken);
        return mapper.MapTo(organizationAnalytics.MemberAttendancePercentage, organizationAnalytics.DailyBookingsTotal);
    }

    [UseResolverScope]
    public async Task<bool> IsSsoTokenValidAsync(
        [Parent] OrganizationDetails organization,
        [Service] IOrganizationSsoService organizationSsoService,
        CancellationToken cancellationToken) =>
        await organizationSsoService.IsSsoTokenValidAsync(organization.Id, organization.UniqueAlphanumericName, cancellationToken);

    [UseResolverScope]
    public async Task<string> SsoLoginUrlAsync(
        string redirectUrl,
        [Parent] OrganizationDetails organization,
        [Service] IOrganizationSsoService organizationSsoService,
        CancellationToken cancellationToken) =>
        await organizationSsoService.SsoLoginAsync(organization.Id, organization.UniqueAlphanumericName, redirectUrl, cancellationToken);

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
            false,
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
