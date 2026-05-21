using HotChocolate;
using Organization.Api.Models;

namespace Organization.Api.GraphQL.Offering;

[GraphQLName("UpdateOrganizationOfferingInput")]
public class UpdateOrganizationOfferingInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("organizationId")] public string? OrganizationId { get; set; }

    [GraphQLName("organizationCustomDomain")]
    public string? OrganizationCustomDomain { get; set; }

    [GraphQLName("fieldsToUpdate")] public HashSet<OrganizationOfferingPatchField> FieldsToUpdate { get; set; } = [];
    [GraphQLName("offeringCode")] public string? OfferingCode { get; set; }
}
