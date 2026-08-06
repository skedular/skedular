using HotChocolate;

namespace Organization.Api.GraphQL.BankAccount;

[GraphQLName("SetOrganizationBankAccountAsDefaultInput")]
public class SetOrganizationBankAccountAsDefaultInput
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("id")]
    public string Id { get; set; } = string.Empty;
}
