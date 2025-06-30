using HotChocolate;

namespace Organization.Api.GraphQL.Payment;

[GraphQLName("AddMyPaymentMethodIntentInput")]
public class AddMyPaymentMethodIntentInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}
