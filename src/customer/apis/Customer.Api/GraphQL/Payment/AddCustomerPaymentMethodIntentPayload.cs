using HotChocolate;

namespace Customer.Api.GraphQL.Payment;

[GraphQLName("AddCustomerPaymentMethodIntentPayload")]
public class AddCustomerPaymentMethodIntentPayload
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("publishedKeys")]
    public string PublishedKeys { get; set; } = string.Empty;

    [GraphQLName("clientSecret")]
    public string ClientSecret { get; set; } = string.Empty;
}
