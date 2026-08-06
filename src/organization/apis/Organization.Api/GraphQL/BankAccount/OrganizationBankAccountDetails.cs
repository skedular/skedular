using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using Organization.Api.GraphQL.Organization;

namespace Organization.Api.GraphQL.BankAccount;

[GraphQLName("OrganizationBankAccountDetails")]
public class OrganizationBankAccountDetails : Node
{
    [GraphQLName("isDefault")]
    public bool IsDefault { get; set; }

    [GraphQLName("name")]
    public string Name { get; set; } = string.Empty;

    [GraphQLName("bankName")]
    public string BankName { get; set; } = string.Empty;

    [GraphQLName("accountHolderName")]
    public string AccountHolderName { get; set; } = string.Empty;

    [GraphQLName("accountNumber")]
    public string AccountNumber { get; set; } = string.Empty;

    [GraphQLName("country")]
    public string Country { get; set; } = string.Empty;

    [GraphQLName("countryCode")]
    public string? CountryCode { get; set; }

    [GraphQLName("organization")]
    public OrganizationDetails Organization { get; set; } = new();
}
