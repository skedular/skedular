using Api.Shared.Services.Models;
using HotChocolate;

namespace Organization.Api.GraphQL.Organization;

[GraphQLName("UpdateOrganizationBillingCycleInput")]
public class UpdateOrganizationBillingCycleInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string? Id { get; set; }
    [GraphQLName("customDomain")] public string? CustomDomain { get; set; }
    [GraphQLName("billingCycle")] public OrganizationBillingCycle BillingCycle { get; set; }
}
