using HotChocolate;
using Organization.Shared.Models.PricingCatalog;

namespace Organization.Api.GraphQL.Pricing;

[GraphQLName("UpdateOrganizationTeamsSubscriptionInput")]
public class UpdateOrganizationTeamsSubscriptionInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("organizationId")] public string OrganizationId { get; set; } = string.Empty;
    [GraphQLName("planCode")] public PricingCatalogSubscriptionPlanCode PlanCode { get; set; }
    [GraphQLName("purchasedUserCapacity")] public int? PurchasedUserCapacity { get; set; }

    [GraphQLName("purchasedLocationCapacity")]
    public int? PurchasedLocationCapacity { get; set; }

    [GraphQLName("purchasedTeamCapacity")] public int? PurchasedTeamCapacity { get; set; }
}
