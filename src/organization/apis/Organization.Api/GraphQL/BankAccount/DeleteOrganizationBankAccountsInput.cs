using HotChocolate;

namespace Organization.Api.GraphQL.BankAccount;

[GraphQLName("DeleteOrganizationBankAccountsInput")]
public class DeleteOrganizationBankAccountsInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("ids")] public IEnumerable<string> Ids { get; set; } = [];
}
