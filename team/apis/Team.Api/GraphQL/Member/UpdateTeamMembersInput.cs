using HotChocolate;

namespace Team.Api.GraphQL.Member;

[GraphQLName("UpdateTeamMembersInput")]
public class UpdateTeamMembersInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
    [GraphQLName("customerIds")] public IEnumerable<string> CustomerIds { get; set; } = [];
    [GraphQLName("organizationMemberIds")] public IEnumerable<string> OrganizationMemberIds { get; set; } = [];
}
