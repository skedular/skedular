using Api.Shared.Services.Models;
using Customer.Shared.Models;
using HotChocolate;

namespace Customer.Api.Models;

[GraphQLName("CustomerDetailsPatchField")]
public enum CustomerDetailsPatchField
{
    Timezone,
    Designation,
    Title,
    Name,
    GivenName,
    MiddleName,
    FamilyName,
    PhoneNumber,
    PersonalInformationVisibility
}

[GraphQLName("CustomerBillingDetailsPatchField")]
public enum CustomerBillingDetailsPatchField
{
    CompanyName,
    Email,
    BillingAddress
}

public record CustomerDetailsPatchRequest(
    IReadOnlySet<CustomerDetailsPatchField> FieldsToUpdate,
    string? Timezone,
    string? Designation,
    string? Title,
    string? Name,
    string? GivenName,
    string? MiddleName,
    string? FamilyName,
    string? PhoneNumber,
    PersonalInformationVisibility PersonalInformationVisibility);

public record CustomerBillingDetailsPatchRequest(
    CustomerBillingDetails BillingDetails,
    IReadOnlySet<CustomerBillingDetailsPatchField> FieldsToUpdate);

public enum CustomerIdentityPatchField
{
    Email,
    EmailVerified
}

public record CustomerIdentityPatchRequest(
    Identity Identity,
    IReadOnlySet<CustomerIdentityPatchField> FieldsToUpdate);
