using HotChocolate;

// ReSharper disable ClassNeverInstantiated.Global

namespace Location.Api.GraphQL.ContactedVia;

[GraphQLName("ToggleContactedViaWhatsappInput")]
public class ToggleContactedViaWhatsappInput
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("locationId")]
    public string LocationId { get; set; } = string.Empty;
}
