using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types.Pagination;
using HotChocolate.Types.Relay;
using Payment.Shared.Models;

// ReSharper disable ClassNeverInstantiated.Global

namespace Payment.Api.GraphQL;

[GraphQLName("AddOrganizationStripeConnectAccountInput")]
public class AddOrganizationStripeConnectAccountInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string? Id { get; set; }
    [GraphQLName("organizationId")] public string OrganizationId { get; set; } = string.Empty;
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("redirectUrl")] public string RedirectUrl { get; set; } = string.Empty;
}

[GraphQLName("UpdateOrganizationStripeConnectAccountInput")]
public class UpdateOrganizationStripeConnectAccountInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
}

[GraphQLName("DeleteOrganizationStripeConnectAccountInput")]
public class DeleteOrganizationStripeConnectAccountInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
}

[GraphQLName("DeleteOrganizationStripeConnectAccountsInput")]
public class DeleteOrganizationStripeConnectAccountsInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("ids")] public IEnumerable<string> Ids { get; set; } = [];
}

[GraphQLName("OrganizationStripeConnectAccountPayload")]
public class OrganizationStripeConnectAccountPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("account")] public OrganizationStripeConnectAccountDetails Account { get; set; } = new();
}

[GraphQLName("OrganizationStripeConnectAccountsPayload")]
public class OrganizationStripeConnectAccountsPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("accounts")] public IEnumerable<OrganizationStripeConnectAccountDetails> Accounts { get; set; } = [];
}

[GraphQLName("OrganizationStripeConnectAccountDetails")]
public class OrganizationStripeConnectAccountDetails : Node
{
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

[GraphQLName("OrganizationStripeConnectAccountWhereInput")]
public class OrganizationStripeConnectAccountWhereInput
{
    [GraphQLName("organizationId")] public string OrganizationId { get; set; } = string.Empty;
    [GraphQLName("nameContains")] public string? NameContains { get; set; }
    [GraphQLName("onboardingCompleted")] public bool? OnboardingCompleted { get; set; }
}

[GraphQLName("OrganizationStripeConnectAccountOrderInput")]
public class OrganizationStripeConnectAccountOrderInput
{
    [GraphQLName("direction")] public OrderDirection Direction { get; set; }
    [GraphQLName("field")] public OrganizationStripeConnectAccountOrderField Field { get; set; }
}

[GraphQLName("OrganizationStripeConnectAccountConnection")]
public class OrganizationStripeConnectAccountConnection : Enterprise.Shared.GraphQL.Types.Connection<OrganizationStripeConnectAccountEdge>;

[GraphQLName("OrganizationStripeConnectAccountEdge")]
public class OrganizationStripeConnectAccountEdge(OrganizationStripeConnectAccountDetails node, string cursor)
    : Edge<OrganizationStripeConnectAccountDetails>(node, cursor);

[GraphQLName("Payment_OrganizationDetails")]
public class OrganizationDetails
{
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; } = string.Empty;
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("website")] public string? Website { get; set; }
}
