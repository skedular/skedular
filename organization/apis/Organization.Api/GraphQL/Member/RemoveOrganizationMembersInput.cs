using HotChocolate;

namespace Organization.Api.GraphQL.Member;

[GraphQLName("RemoveOrganizationMembersInput")]
public class RemoveOrganizationMembersInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("ids")] public IEnumerable<string> Ids { get; set; } = [];
}
