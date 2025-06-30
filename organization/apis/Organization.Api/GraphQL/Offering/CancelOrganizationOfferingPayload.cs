using HotChocolate;

namespace Organization.Api.GraphQL.Offering;

[GraphQLName("CancelOrganizationOfferingPayload")]
public class CancelOrganizationOfferingPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}
