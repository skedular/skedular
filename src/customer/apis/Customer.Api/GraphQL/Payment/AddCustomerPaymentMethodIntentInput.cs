using HotChocolate;

// ReSharper disable ClassNeverInstantiated.Global

namespace Customer.Api.GraphQL.Payment;

[GraphQLName("AddCustomerPaymentMethodIntentInput")]
public class AddCustomerPaymentMethodIntentInput
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }
}
