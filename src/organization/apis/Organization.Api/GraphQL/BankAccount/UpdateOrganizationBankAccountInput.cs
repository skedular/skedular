using HotChocolate;
using Organization.Api.Models;

namespace Organization.Api.GraphQL.BankAccount;

[GraphQLName("UpdateOrganizationBankAccountInput")]
public class UpdateOrganizationBankAccountInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
    [GraphQLName("fieldsToUpdate")] public HashSet<OrganizationBankAccountPatchField> FieldsToUpdate { get; set; } = [];
    [GraphQLName("name")] public string? Name { get; set; }
    [GraphQLName("bankName")] public string? BankName { get; set; }
    [GraphQLName("accountHolderName")] public string? AccountHolderName { get; set; }
    [GraphQLName("accountNumber")] public string? AccountNumber { get; set; }
    [GraphQLName("country")] public string? Country { get; set; }
}
