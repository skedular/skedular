using HotChocolate;

namespace Organization.Api.GraphQL.BankAccount;

[GraphQLName("OrganizationBankAccountsPayload")]
public class OrganizationBankAccountsPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("organizationBankAccounts")]
    public IEnumerable<OrganizationBankAccountDetails> OrganizationBankAccounts { get; set; } = [];
}
