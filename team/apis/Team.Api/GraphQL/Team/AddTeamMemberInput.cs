using HotChocolate;

namespace Team.Api.GraphQL.Team;

[GraphQLName("AddTeamMemberInput")]
public class AddTeamMemberInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
    [GraphQLName("customerId")] public string? CustomerId { get; set; }
    [GraphQLName("organizationMemberId")] public string? OrganizationMemberId { get; set; }
}
