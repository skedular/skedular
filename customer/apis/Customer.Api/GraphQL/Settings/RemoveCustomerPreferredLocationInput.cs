using HotChocolate;

namespace Customer.Api.GraphQL.Settings;

[GraphQLName("RemoveCustomerPreferredLocationInput")]
public class RemoveCustomerPreferredLocationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("locationId")] public string LocationId { get; set; } = string.Empty;
}
