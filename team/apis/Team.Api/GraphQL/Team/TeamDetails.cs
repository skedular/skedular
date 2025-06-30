using Api.Shared.Services.Models;
using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types.Relay;
using Team.Api.GraphQL.Member;

namespace Team.Api.GraphQL.Team;

[GraphQLName("TeamDetails")]
public class TeamDetails : Node
{
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("about")] public string? About { get; set; }
    [GraphQLName("members")] public IEnumerable<TeamMemberDetails> Members { get; set; } = [];
    [GraphQLName("organization")] public OrganizationDetails Organization { get; set; } = new();
    [GraphQLName("primaryLocation")] public LocationDetails? PrimaryLocation { get; set; }
    [GraphQLName("timezone")] public string? Timezone { get; set; }
    [GraphQLName("hasFutureBooking")] public bool HasFutureBooking { get; set; }
    [GraphQLName("canModify")] public bool CanModify { get; set; }
    [GraphQLName("canDelete")] public bool CanDelete { get; set; }
    [GraphQLName("canInvitePeople")] public bool CanInvitePeople { get; set; }
    [GraphQLName("primaryFeatureImage")] public CdnImageFile? PrimaryFeatureImage { get; set; }
    [GraphQLName("id")] [ID] public string Id { get; set; } = string.Empty;
}
