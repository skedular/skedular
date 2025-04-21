using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types.Pagination;
using HotChocolate.Types.Relay;
using Payment.Shared.Models;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
// ReSharper disable ClassNeverInstantiated.Global

namespace Payment.Api.GraphQL;

[GraphQLName("AddOrganizationPaymentMethodIntentInput")]
public class AddOrganizationPaymentMethodIntentInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("organizationId")] public required string OrganizationId { get; set; }
}

[GraphQLName("AddOrganizationPaymentMethodIntentPayload")]
public class AddOrganizationPaymentMethodIntentPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("publishedKeys")] public string PublishedKeys { get; set; } = string.Empty;
    [GraphQLName("clientSecret")] public string ClientSecret { get; set; } = string.Empty;
}

[GraphQLName("OrganizationPaymentMethod")]
public class OrganizationPaymentMethod : Node
{
    [GraphQLName("cardBrand")] public string? CardBrand { get; set; }
    [GraphQLName("cardCountry")] public string? CardCountry { get; set; }
    [GraphQLName("cardDescription")] public string? CardDescription { get; set; }
    [GraphQLName("cardExpiryMonth")] public int? CardExpiryMonth { get; set; }
    [GraphQLName("cardExpiryYear")] public int? CardExpiryYear { get; set; }
    [GraphQLName("cardFingerprint")] public string? CardFingerprint { get; set; }
    [GraphQLName("cardFunding")] public string? CardFunding { get; set; }
    [GraphQLName("cardIssuer")] public string? CardIssuer { get; set; }
    [GraphQLName("cardLastFourDigit")] public string? CardLastFourDigit { get; set; }
    [GraphQLName("id")] [ID] public required string Id { get; set; }
}

[GraphQLName("RemoveOrganizationPaymentMethodInput")]
public class RemoveOrganizationPaymentMethodInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public required string Id { get; set; }
}

[GraphQLName("RemoveOrganizationPaymentMethodPayload")]
public class RemoveOrganizationPaymentMethodPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}

[GraphQLName("AddOrganizationStripeConnectAccountInput")]
public class AddOrganizationStripeConnectAccountInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("organizationId")] public required string OrganizationId { get; set; }
    [GraphQLName("name")] public required string Name { get; set; }
}

[GraphQLName("UpdateOrganizationStripeConnectAccountInput")]
public class UpdateOrganizationStripeConnectAccountInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public required string Id { get; set; }
    [GraphQLName("name")] public required string Name { get; set; }
}

[GraphQLName("DeleteOrganizationStripeConnectAccountInput")]
public class DeleteOrganizationStripeConnectAccountInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public required string Id { get; set; }
}

[GraphQLName("OrganizationStripeConnectAccountPayload")]
public class OrganizationStripeConnectAccountPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("account")] public OrganizationStripeConnectAccountDetails Account { get; set; }
}

[GraphQLName("OrganizationStripeConnectAccountDetails")]
public class OrganizationStripeConnectAccountDetails : Node
{
    [GraphQLName("name")] public string Name { get; set; }
    [GraphQLName("chargesEnabled")] public bool ChargesEnabled { get; set; }
    [GraphQLName("payoutsEnabled")] public bool PayoutsEnabled { get; set; }
    [GraphQLName("type")] public string Type { get; set; }
    [GraphQLName("id")] [ID] public required string Id { get; set; }
}

[GraphQLName("OrganizationStripeConnectAccountWhereInput")]
public class OrganizationStripeConnectAccountWhereInput
{
    [GraphQLName("organizationId")] public string OrganizationId { get; set; }
    [GraphQLName("nameContains")] public string? NameContains { get; set; }
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
