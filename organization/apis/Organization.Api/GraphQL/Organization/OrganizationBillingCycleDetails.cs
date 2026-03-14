using Api.Shared.Services.Models;
using HotChocolate;

namespace Organization.Api.GraphQL.Organization;

[GraphQLName("OrganizationBillingCycleDetails")]
public class OrganizationBillingCycleDetails
{
    [GraphQLName("type")] public OrganizationBillingCycle Type { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
}
