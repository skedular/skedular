using HotChocolate;

namespace Customer.Api.GraphQL.Settings;

[GraphQLName("RemoveCustomerPreferredOrganizationTagInput")]
public class RemoveCustomerPreferredOrganizationTagInput
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("organizationTagId")]
    public string OrganizationTagId { get; set; } = string.Empty;
}
