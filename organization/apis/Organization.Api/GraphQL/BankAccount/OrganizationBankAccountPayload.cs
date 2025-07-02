using HotChocolate;

namespace Organization.Api.GraphQL.BankAccount;

[GraphQLName("OrganizationBankAccountPayload")]
public class OrganizationBankAccountPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("organizationBankAccounts")] public OrganizationBankAccountDetails OrganizationBankAccount { get; set; } = new();
}
