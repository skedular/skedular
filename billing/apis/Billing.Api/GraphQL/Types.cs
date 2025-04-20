using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types.Relay;

namespace Billing.Api.GraphQL;

[GraphQLName("OrganizationBillingContactDetailsPayload")]
public class OrganizationBillingContactDetailsPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("organizationBillingContactDetails")]
    public OrganizationBillingContactDetails OrganizationBillingContactDetails { get; set; }
}

[GraphQLName("OrganizationBillingContactDetails")]
public class OrganizationBillingContactDetails : Node
{
    [GraphQLName("email")] public string? Email { get; set; }
    [GraphQLName("addressLine1")] public string? AddressLine1 { get; set; }
    [GraphQLName("addressLine2")] public string? AddressLine2 { get; set; }
    [GraphQLName("suburb")] public string? Suburb { get; set; }
    [GraphQLName("city")] public string? City { get; set; }
    [GraphQLName("province")] public string? Province { get; set; }
    [GraphQLName("zipcode")] public string? Zipcode { get; set; }
    [GraphQLName("country")] public string? Country { get; set; }
    [GraphQLName("id")] [ID] public required string Id { get; set; }
}

[GraphQLName("UpdateOrganizationBillingContactDetailsInput")]
public class UpdateOrganizationBillingContactDetailsInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("organizationId")] public required string OrganizationId { get; set; }
    [GraphQLName("email")] public string? Email { get; set; }
    [GraphQLName("addressLine1")] public string? AddressLine1 { get; set; }
    [GraphQLName("addressLine2")] public string? AddressLine2 { get; set; }
    [GraphQLName("suburb")] public string? Suburb { get; set; }
    [GraphQLName("city")] public string? City { get; set; }
    [GraphQLName("province")] public string? Province { get; set; }
    [GraphQLName("zipcode")] public string? Zipcode { get; set; }
    [GraphQLName("country")] public string? Country { get; set; }
}

[GraphQLName("MyBillingContactDetailsPayload")]
public class MyBillingContactDetailsPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("customerBillingContactDetails")]
    public CustomerBillingContactDetails CustomerBillingContactDetails { get; set; }
}

[GraphQLName("CustomerBillingContactDetails")]
public class CustomerBillingContactDetails : Node
{
    [GraphQLName("companyName")] public string? CompanyName { get; set; }
    [GraphQLName("email")] public string? Email { get; set; }
    [GraphQLName("addressLine1")] public string? AddressLine1 { get; set; }
    [GraphQLName("addressLine2")] public string? AddressLine2 { get; set; }
    [GraphQLName("suburb")] public string? Suburb { get; set; }
    [GraphQLName("city")] public string? City { get; set; }
    [GraphQLName("province")] public string? Province { get; set; }
    [GraphQLName("zipcode")] public string? Zipcode { get; set; }
    [GraphQLName("country")] public string? Country { get; set; }
    [GraphQLName("id")] [ID] public required string Id { get; set; }
}

[GraphQLName("UpdateMyBillingContactDetailsInput")]
public class UpdateMyBillingContactDetailsInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("companyName")] public string? CompanyName { get; set; }
    [GraphQLName("email")] public string? Email { get; set; }
    [GraphQLName("addressLine1")] public string? AddressLine1 { get; set; }
    [GraphQLName("addressLine2")] public string? AddressLine2 { get; set; }
    [GraphQLName("suburb")] public string? Suburb { get; set; }
    [GraphQLName("city")] public string? City { get; set; }
    [GraphQLName("province")] public string? Province { get; set; }
    [GraphQLName("zipcode")] public string? Zipcode { get; set; }
    [GraphQLName("country")] public string? Country { get; set; }
}
