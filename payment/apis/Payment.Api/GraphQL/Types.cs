using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types.Relay;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
// ReSharper disable ClassNeverInstantiated.Global

namespace Payment.Api.GraphQL;

[GraphQLName("AddOrganizationPaymentMethodIntentInput")]
public class AddOrganizationPaymentMethodIntentInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("organizationId")] public required string OrganizationId { get; set; }
}

[GraphQLName("AddOrganizationPaymentMethodIntentResponse")]
public class AddOrganizationPaymentMethodIntentResponse
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

[GraphQLName("RemoveOrganizationPaymentMethodResponse")]
public class RemoveOrganizationPaymentMethodResponse
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}

[GraphQLName("AddOrganizationStripeConnectAccountInput")]
public class AddOrganizationStripeConnectAccountInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("organizationId")] public required string OrganizationId { get; set; }
}

[GraphQLName("AddOrganizationStripeConnectAccountResponse")]
public class AddOrganizationStripeConnectAccountResponse
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}
