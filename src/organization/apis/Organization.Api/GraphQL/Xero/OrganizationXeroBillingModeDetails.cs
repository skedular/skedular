using Api.Shared.Services.Models;
using HotChocolate;

namespace Organization.Api.GraphQL.Xero;

[GraphQLName("OrganizationXeroBillingModeDetails")]
public class OrganizationXeroBillingModeDetails
{
    [GraphQLName("type")]
    public OrganizationXeroBillingMode Type { get; set; }

    [GraphQLName("name")]
    public string Name { get; set; } = string.Empty;
}
