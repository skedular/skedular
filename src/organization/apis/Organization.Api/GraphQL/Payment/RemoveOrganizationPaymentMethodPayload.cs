using HotChocolate;

namespace Organization.Api.GraphQL.Payment;

[GraphQLName("RemoveOrganizationPaymentMethodPayload")]
public class RemoveOrganizationPaymentMethodPayload
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }
}
