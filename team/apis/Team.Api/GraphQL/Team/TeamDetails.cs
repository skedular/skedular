using Api.Shared.Services.Models;
using Enterprise.Shared;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types;
using Team.Api.GraphQL.Member;
using Team.Api.Mappers;
using Team.Api.Services;
using Team.Shared.Models;
using Constants = Api.Shared.Services.Constants;

namespace Team.Api.GraphQL.Team;

[GraphQLName("TeamDetails")]
public class TeamDetails : Node
{
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("about")] public string? About { get; set; }
    [GraphQLName("organizationId")] public string OrganizationId { get; set; } = string.Empty;

    [GraphQLName("organizationUniqueAlphanumericName")]
    public string OrganizationUniqueAlphanumericName { get; set; } = string.Empty;

    [GraphQLName("primaryLocationId")] public string? PrimaryLocationId { get; set; }
    [GraphQLName("timezone")] public string? Timezone { get; set; }
    [GraphQLName("canModify")] public bool CanModify { get; set; }
    [GraphQLName("canDelete")] public bool CanDelete { get; set; }
    [GraphQLName("canInvitePeople")] public bool CanInvitePeople { get; set; }
    [GraphQLName("featureImages")] public IEnumerable<CdnImageFile> FeatureImages { get; set; } = [];

    [UseResolverScope]
    public async Task<Connection<TeamMemberEdge>> MembersAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        TeamMemberWhereInput? where,
        IEnumerable<TeamMemberOrderInput>? orderBy,
        [Parent] TeamDetails team,
        [Service] ITeamMemberService teamMemberService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var (paginatedInfo, edges, totalCount) = await teamMemberService.GetPaginatedMembersAsync(
            new PaginationInputParam(after, first, before, last),
            new TeamMemberSearchCriteria(team.Id, where?.NameContains),
            orderBy.ToSafeCollection().Select(item => new TeamMemberOrder(item.Direction, item.Field)).ToList(),
            cancellationToken);

        return new Connection<TeamMemberEdge>
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
    public async Task<bool> HasFutureBookingAsync(
        [Parent] TeamDetails team,
        [Service] ITeamService teamService,
        CancellationToken cancellationToken) =>
        team.OrganizationUniqueAlphanumericName != Constants.SkedularPublicLocationsUniqueAlphanumericName &&
        await teamService.HasFutureBookingAsync(team.Id, false, cancellationToken);
}

[ObjectType<TeamDetails>]
public static partial class TeamDetailsType
{
    static partial void Configure(IObjectTypeDescriptor<TeamDetails> descriptor)
    {
        descriptor.Ignore(item => item.OrganizationId);
        descriptor.Ignore(item => item.OrganizationUniqueAlphanumericName);
        descriptor.Ignore(item => item.PrimaryLocationId);
    }

    public static OrganizationDetails GetOrganization([Parent] TeamDetails item) => new(item.OrganizationId, item.OrganizationUniqueAlphanumericName);

    public static LocationDetails? GetPrimaryLocation([Parent] TeamDetails item) => string.IsNullOrWhiteSpace(item.PrimaryLocationId)
        ? null
        : new LocationDetails(item.PrimaryLocationId);
}
