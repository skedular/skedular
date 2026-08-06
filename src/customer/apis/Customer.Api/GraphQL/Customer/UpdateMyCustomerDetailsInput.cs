using Api.Shared.Services.Models;
using Customer.Api.Models;
using HotChocolate;

namespace Customer.Api.GraphQL.Customer;

[GraphQLName("UpdateMyCustomerDetailsInput")]
public class UpdateMyCustomerDetailsInput
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("fieldsToUpdate")]
    public HashSet<CustomerDetailsPatchField> FieldsToUpdate { get; set; } = [];

    [GraphQLName("timezone")]
    public string? Timezone { get; set; }

    [GraphQLName("designation")]
    public string? Designation { get; set; }

    [GraphQLName("title")]
    public string? Title { get; set; }

    [GraphQLName("name")]
    public string? Name { get; set; }

    [GraphQLName("givenName")]
    public string? GivenName { get; set; }

    [GraphQLName("middleName")]
    public string? MiddleName { get; set; }

    [GraphQLName("familyName")]
    public string? FamilyName { get; set; }

    [GraphQLName("phoneNumber")]
    public string? PhoneNumber { get; set; }

    [GraphQLName("personalInformationVisibility")]
    public PersonalInformationVisibility PersonalInformationVisibility { get; set; }
}
