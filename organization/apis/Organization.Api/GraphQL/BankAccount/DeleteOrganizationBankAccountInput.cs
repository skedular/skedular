using HotChocolate;

namespace Organization.Api.GraphQL.BankAccount;

[GraphQLName("DeleteOrganizationBankAccountInput")]
public class DeleteOrganizationBankAccountInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
}
