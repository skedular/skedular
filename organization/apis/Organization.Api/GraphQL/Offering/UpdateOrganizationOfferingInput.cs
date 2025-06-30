using HotChocolate;

namespace Organization.Api.GraphQL.Offering;

[GraphQLName("UpdateOrganizationOfferingInput")]
public class UpdateOrganizationOfferingInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
    [GraphQLName("offeringCode")] public string OfferingCode { get; set; } = string.Empty;
}
