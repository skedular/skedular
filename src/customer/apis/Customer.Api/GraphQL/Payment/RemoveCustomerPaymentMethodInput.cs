using HotChocolate;

namespace Customer.Api.GraphQL.Payment;

[GraphQLName("RemoveCustomerPaymentMethodInput")]
public class RemoveCustomerPaymentMethodInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
}
