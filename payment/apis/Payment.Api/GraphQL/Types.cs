using HotChocolate;
using HotChocolate.Types.Relay;

namespace Payment.Api.GraphQL;

[GraphQLName("AddOrganizationPaymentMethodIntentInput")]
public class AddOrganizationPaymentMethodIntentInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("organizationId")] public string OrganizationId { get; set; }
}

[GraphQLName("AddOrganizationPaymentMethodIntentResponse")]
public class AddOrganizationPaymentMethodIntentResponse
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("publishedKeys")] public string PublishedKeys { get; set; }

    [GraphQLName("clientSecret")] public string ClientSecret { get; set; }
}

[GraphQLName("Mutation_AddOrganizationPaymentMethodIntent_Arguments")]
public class Mutation_AddOrganizationPaymentMethodIntent_Arguments
{
    [GraphQLName("input")] public AddOrganizationPaymentMethodIntentInput Input { get; set; }
}

[GraphQLName("Mutation_RemoveOrganizationPaymentMethod_Arguments")]
public class Mutation_RemoveOrganizationPaymentMethod_Arguments
{
    [GraphQLName("input")] public RemoveOrganizationPaymentMethodInput Input { get; set; }
}

[GraphQLName("Node")]
public interface Node
{
    [GraphQLName("id")] [ID] public string Id { get; set; }
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

    [GraphQLName("id")] [ID] public string Id { get; set; }
}

public enum PaymentOrganizationStripePaymentMethodStatus
{
    PENDING,
    CANCELLED,
    CONFIRMED
}

[GraphQLName("Query_OrganizationPaymentMethodsDetails_Arguments")]
public class Query_OrganizationPaymentMethodsDetails_Arguments
{
    [GraphQLName("organizationId")] public string OrganizationId { get; set; }
}

[GraphQLName("RemoveOrganizationPaymentMethodInput")]
public class RemoveOrganizationPaymentMethodInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("id")] public string Id { get; set; }
}

[GraphQLName("RemoveOrganizationPaymentMethodResponse")]
public class RemoveOrganizationPaymentMethodResponse
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}

[GraphQLName("Version")]
public class Version
{
    [GraphQLName("major")] public int Major { get; set; }

    [GraphQLName("minor")] public int Minor { get; set; }

    [GraphQLName("build")] public int Build { get; set; }

    [GraphQLName("revision")] public int Revision { get; set; }
}
