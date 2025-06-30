using HotChocolate;

namespace Organization.Api.GraphQL.Payment;

[GraphQLName("AddOrganizationPaymentMethodIntentPayload")]
public class AddOrganizationPaymentMethodIntentPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("publishedKeys")] public string PublishedKeys { get; set; } = string.Empty;
    [GraphQLName("clientSecret")] public string ClientSecret { get; set; } = string.Empty;
}
