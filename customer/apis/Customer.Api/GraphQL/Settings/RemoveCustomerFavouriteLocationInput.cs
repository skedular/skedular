using HotChocolate;

namespace Customer.Api.GraphQL.Settings;

[GraphQLName("RemoveCustomerFavouriteLocationInput")]
public class RemoveCustomerFavouriteLocationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("locationId")] public string LocationId { get; set; } = string.Empty;
}
