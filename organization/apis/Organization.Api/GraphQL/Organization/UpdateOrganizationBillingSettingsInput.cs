using Api.Shared.Services.Models;
using HotChocolate;

namespace Organization.Api.GraphQL.Organization;

[GraphQLName("UpdateOrganizationBillingSettingsInput")]
public class UpdateOrganizationBillingSettingsInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string? Id { get; set; }
    [GraphQLName("customDomain")] public string? CustomDomain { get; set; }
    [GraphQLName("billingCycle")] public OrganizationBillingCycle BillingCycle { get; set; }
    [GraphQLName("invoiceDueInDays")] public int InvoiceDueInDays { get; set; }
}
