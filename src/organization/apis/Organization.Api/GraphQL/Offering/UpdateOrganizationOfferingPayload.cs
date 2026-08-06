using HotChocolate;

namespace Organization.Api.GraphQL.Offering;

[GraphQLName("UpdateOrganizationOfferingPayload")]
public class UpdateOrganizationOfferingPayload
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }
}
