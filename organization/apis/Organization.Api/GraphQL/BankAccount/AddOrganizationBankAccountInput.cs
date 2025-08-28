using HotChocolate;

// ReSharper disable ClassNeverInstantiated.Global

namespace Organization.Api.GraphQL.BankAccount;

[GraphQLName("AddOrganizationBankAccountInput")]
public class AddOrganizationBankAccountInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string? Id { get; set; }
    [GraphQLName("organizationId")] public string? OrganizationId { get; set; }

    [GraphQLName("organizationUniqueAlphanumericName")]
    public string? OrganizationUniqueAlphanumericName { get; set; }

    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("bankName")] public string BankName { get; set; } = string.Empty;
    [GraphQLName("accountHolderName")] public string AccountHolderName { get; set; } = string.Empty;
    [GraphQLName("accountNumber")] public string AccountNumber { get; set; } = string.Empty;
    [GraphQLName("country")] public string Country { get; set; } = string.Empty;
    [GraphQLName("countryCode")] public string? CountryCode { get; set; }
}
