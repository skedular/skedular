using Api.Shared.Services.Models;
using HotChocolate;

namespace Team.Api.GraphQL.Member;

[GraphQLName("ChangeTeamMemberRoleInput")]
public class ChangeTeamMemberRoleInput
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("id")]
    public string Id { get; set; } = string.Empty;

    [GraphQLName("role")]
    public TeamMemberRole Role { get; set; }
}
