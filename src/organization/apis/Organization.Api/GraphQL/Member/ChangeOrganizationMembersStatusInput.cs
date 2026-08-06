using Api.Shared.Services.Models;
using HotChocolate;

namespace Organization.Api.GraphQL.Member;

[GraphQLName("ChangeOrganizationMembersStatusInput")]
public class ChangeOrganizationMembersStatusInput
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("ids")]
    public IEnumerable<string> Ids { get; set; } = [];

    [GraphQLName("status")]
    public OrganizationMemberStatus Status { get; set; }
}
