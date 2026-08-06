using HotChocolate;

namespace Location.Api.GraphQL.Ownership;

[GraphQLName("ClaimLocationOwnershipInput")]
public class ClaimLocationOwnershipInput
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("id")]
    public string? Id { get; set; }

    [GraphQLName("uniqueClaimCode")]
    public string UniqueClaimCode { get; set; } = string.Empty;

    [GraphQLName("organizationId")]
    public string? OrganizationId { get; set; }

    [GraphQLName("organizationCustomDomain")]
    public string? OrganizationCustomDomain { get; set; }
}
