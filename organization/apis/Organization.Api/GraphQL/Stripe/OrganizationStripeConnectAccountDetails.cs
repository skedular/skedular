using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types.Relay;

namespace Organization.Api.GraphQL.Stripe;

[GraphQLName("OrganizationStripeConnectAccountDetails")]
public class OrganizationStripeConnectAccountDetails : Node
{
    [GraphQLName("isDefault")] public bool IsDefault { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("chargesEnabled")] public bool ChargesEnabled { get; set; }
    [GraphQLName("payoutsEnabled")] public bool PayoutsEnabled { get; set; }
    [GraphQLName("type")] public string Type { get; set; } = string.Empty;
    [GraphQLName("country")] public string? Country { get; set; } = string.Empty;
    [GraphQLName("defaultCurrency")] public string? DefaultCurrency { get; set; } = string.Empty;
    [GraphQLName("businessType")] public string? BusinessType { get; set; } = string.Empty;
    [GraphQLName("companyName")] public string? CompanyName { get; set; }
    [GraphQLName("url")] public string? Url { get; set; }
    [GraphQLName("supportUrl")] public string? SupportUrl { get; set; }
    [GraphQLName("contactEmail")] public string? ContactEmail { get; set; }
    [GraphQLName("contactPhone")] public string? ContactPhone { get; set; }
    [GraphQLName("capabilitiesTransfers")] public string CapabilitiesTransfers { get; set; } = string.Empty;

    [GraphQLName("capabilitiesCardPayments")]
    public string CapabilitiesCardPayments { get; set; } = string.Empty;

    [GraphQLName("onboardingUrl")] public string OnboardingUrl { get; set; } = string.Empty;
    [GraphQLName("onboardingCompleted")] public bool OnboardingCompleted { get; set; }
    [GraphQLName("organization")] public OrganizationDetails Organization { get; set; } = new();
    [GraphQLName("id")] [ID] public string Id { get; set; } = string.Empty;
}
