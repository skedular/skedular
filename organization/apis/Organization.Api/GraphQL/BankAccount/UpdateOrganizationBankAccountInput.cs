using HotChocolate;

namespace Organization.Api.GraphQL.BankAccount;

[GraphQLName("UpdateOrganizationBankAccountInput")]
public class UpdateOrganizationBankAccountInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("bankName")] public string BankName { get; set; } = string.Empty;
    [GraphQLName("accountHolderName")] public string AccountHolderName { get; set; } = string.Empty;
    [GraphQLName("accountNumber")] public string AccountNumber { get; set; } = string.Empty;
    [GraphQLName("country")] public string Country { get; set; } = string.Empty;
}
