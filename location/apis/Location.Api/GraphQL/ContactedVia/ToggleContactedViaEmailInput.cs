using HotChocolate;

// ReSharper disable ClassNeverInstantiated.Global

namespace Location.Api.GraphQL.ContactedVia;

[GraphQLName("ToggleContactedViaEmailInput")]
public class ToggleContactedViaEmailInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("locationId")] public string LocationId { get; set; } = string.Empty;
}
