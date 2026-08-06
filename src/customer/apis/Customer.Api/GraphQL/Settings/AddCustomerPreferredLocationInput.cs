using HotChocolate;

// ReSharper disable ClassNeverInstantiated.Global

namespace Customer.Api.GraphQL.Settings;

[GraphQLName("AddCustomerPreferredLocationInput")]
public class AddCustomerPreferredLocationInput
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("locationId")]
    public string LocationId { get; set; } = string.Empty;
}
