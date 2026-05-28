using HotChocolate;

namespace Organization.Api.Models;

[GraphQLName("OrganizationBankAccountPatchField")]
public enum OrganizationBankAccountPatchField
{
    Name,
    BankName,
    AccountHolderName,
    AccountNumber,
    Country
}

public record OrganizationBankAccountPatchRequest(
    string Id,
    IReadOnlySet<OrganizationBankAccountPatchField> FieldsToUpdate,
    string? Name,
    string? BankName,
    string? AccountHolderName,
    string? AccountNumber,
    string? Country);
