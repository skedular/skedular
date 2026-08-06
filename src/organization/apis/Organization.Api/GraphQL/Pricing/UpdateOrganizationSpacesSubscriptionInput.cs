using HotChocolate;
using Organization.Shared.Models.PricingCatalog;

namespace Organization.Api.GraphQL.Pricing;

[GraphQLName("UpdateOrganizationSpacesSubscriptionInput")]
public class UpdateOrganizationSpacesSubscriptionInput
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("organizationId")]
    public string OrganizationId { get; set; } = string.Empty;

    [GraphQLName("planCode")]
    public PricingCatalogSubscriptionPlanCode PlanCode { get; set; }

    [GraphQLName("customCapacity")]
    public int? CustomCapacity { get; set; }
}
