using HotChocolate;

namespace Customer.Api.GraphQL.Payment;

[GraphQLName("AddMyPaymentMethodIntentInput")]
public class AddMyPaymentMethodIntentInput
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }
}
