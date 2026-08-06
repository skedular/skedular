using HotChocolate;

namespace Organization.Api.GraphQL.Tag;

[GraphQLName("DeleteZonesInput")]
public class DeleteZonesInput
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("ids")]
    public IEnumerable<string> Ids { get; set; } = [];
}
