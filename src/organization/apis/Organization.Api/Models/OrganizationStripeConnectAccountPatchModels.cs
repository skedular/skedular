using HotChocolate;

namespace Organization.Api.Models;

[GraphQLName("OrganizationStripeConnectAccountPatchField")]
public enum OrganizationStripeConnectAccountPatchField
{
    Name,
}

public record OrganizationStripeConnectAccountPatchRequest(
    string Id,
    IReadOnlySet<OrganizationStripeConnectAccountPatchField> FieldsToUpdate,
    string? Name);
