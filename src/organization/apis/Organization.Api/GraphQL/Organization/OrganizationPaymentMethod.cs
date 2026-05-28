using Enterprise.Shared.GraphQL.Types;
using HotChocolate;

namespace Organization.Api.GraphQL.Organization;

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
}
