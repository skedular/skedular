using HotChocolate;
using HotChocolate.Types.Relay;
using Team.Api.GraphQL.Team;

namespace Team.Api.GraphQL.Member;

[GraphQLName("TeamOrganizationMemberDetails")]
public class TeamOrganizationMemberDetails
{
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; } = string.Empty;
    [GraphQLName("customer")] public CustomerDetails Customer { get; set; } = new();
}
