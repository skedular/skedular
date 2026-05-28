using HotChocolate;

namespace Marketplace.Api.GraphQL.Product;

[GraphQLName("Marketplace_OrganizationDetails")]
public class OrganizationDetails
{
    [GraphQLName("uniqueId")] public string Id { get; set; } = string.Empty;
    [GraphQLName("customDomain")] public string? CustomDomain { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("website")] public string? Website { get; set; }

    [GraphQLName("customerFacingTermsAndConditionsUrl")]
    public string? CustomerFacingTermsAndConditionsUrl { get; set; }

    [GraphQLName("logoUrl")] public string? LogoUrl { get; set; }
}
