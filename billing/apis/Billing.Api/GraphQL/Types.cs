using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types.Relay;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Billing.Api.GraphQL;

[GraphQLName("OrganizationBillingInfo")]
public class OrganizationBillingInfo : Node
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

[GraphQLName("OrganizationBillingInfoPayload")]
public class OrganizationBillingInfoPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("organizationBillingInfo")]
    public OrganizationBillingInfo OrganizationBillingInfo { get; set; }
}

[GraphQLName("OrganizationCurrentOfferingChargesDetails")]
public class OrganizationCurrentOfferingChargesDetails
{
    [GraphQLName("offeringName")] public string OfferingName { get; set; } = string.Empty;
    [GraphQLName("start")] public DateTimeOffset Start { get; set; }
    [GraphQLName("end")] public DateTimeOffset End { get; set; }

    [GraphQLName("totalNumberOfActiveCustomers")]
    public int TotalNumberOfActiveCustomers { get; set; }

    [GraphQLName("unitPrice")] public int UnitPrice { get; set; }
    [GraphQLName("totalCost")] public int TotalCost { get; set; }
}

[GraphQLName("SetOrganizationBillingInfoInput")]
public class SetOrganizationBillingInfoInput
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
