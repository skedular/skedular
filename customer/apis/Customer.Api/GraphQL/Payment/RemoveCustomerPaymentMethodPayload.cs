using HotChocolate;

namespace Customer.Api.GraphQL.Payment;

[GraphQLName("RemoveCustomerPaymentMethodPayload")]
public class RemoveCustomerPaymentMethodPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}
