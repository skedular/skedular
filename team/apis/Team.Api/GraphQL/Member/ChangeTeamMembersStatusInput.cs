using Api.Shared.Services.Models;
using HotChocolate;

namespace Team.Api.GraphQL.Member;

[GraphQLName("ChangeTeamMembersStatusInput")]
public class ChangeTeamMembersStatusInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("ids")] public IEnumerable<string> Ids { get; set; } = [];
    [GraphQLName("status")] public TeamMemberStatus Status { get; set; }
}
