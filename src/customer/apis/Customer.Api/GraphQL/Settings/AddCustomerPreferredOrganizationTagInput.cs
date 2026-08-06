using HotChocolate;

namespace Customer.Api.GraphQL.Settings;

[GraphQLName("AddCustomerPreferredOrganizationTagInput")]
public class AddCustomerPreferredOrganizationTagInput
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("organizationTagId")]
    public string OrganizationTagId { get; set; } = string.Empty;
}
