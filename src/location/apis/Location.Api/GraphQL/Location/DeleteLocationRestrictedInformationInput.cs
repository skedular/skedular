using HotChocolate;

namespace Location.Api.GraphQL.Location;

[GraphQLName("DeleteLocationRestrictedInformationInput")]
public class DeleteLocationRestrictedInformationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
}
