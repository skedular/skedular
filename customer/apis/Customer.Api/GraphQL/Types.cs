using Customer.Shared.Models;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types.Pagination;
using HotChocolate.Types.Relay;
using CustomerBillingDetails = Customer.Api.GraphQL.Billing.CustomerBillingDetails;

// ReSharper disable ClassNeverInstantiated.Global

namespace Customer.Api.GraphQL;

[GraphQLName("AddCustomerPreferredLocationInput")]
public class AddCustomerPreferredLocationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("locationId")] public string LocationId { get; set; } = string.Empty;
}

[GraphQLName("AddCustomerPreferredOrganizationTagInput")]
public class AddCustomerPreferredOrganizationTagInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("organizationTagId")] public string OrganizationTagId { get; set; } = string.Empty;
}

[GraphQLName("AddCustomerPreferredTeamInput")]
public class AddCustomerPreferredTeamInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("teamId")] public string TeamId { get; set; } = string.Empty;
}

[GraphQLName("ClearCustomerDefaultOrganizationInput")]
public class ClearCustomerDefaultOrganizationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}

[GraphQLName("CompletePreferredLocationOnboardingInput")]
public class CompletePreferredLocationOnboardingInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}

[GraphQLName("CompleteDefaultOrganizationOnboardingInput")]
public class CompleteDefaultOrganizationOnboardingInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}

[GraphQLName("CompleteLocationOnboardingInput")]
public class CompleteLocationOnboardingInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}

[GraphQLName("CompleteOrganizationOnboardingInput")]
public class CompleteOrganizationOnboardingInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}

[GraphQLName("CompletePreferredZoneOnboardingInput")]
public class CompletePreferredZoneOnboardingInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}

[GraphQLName("CompleteTeamOnboardingInput")]
public class CompleteTeamOnboardingInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}

[GraphQLName("CustomerConnection")]
public class CustomerConnection : Enterprise.Shared.GraphQL.Types.Connection<CustomerEdge>;

[GraphQLName("CustomerDetails")]
public class CustomerDetails : Node
{
    [GraphQLName("createdAt")] public DateTimeOffset CreatedAt { get; set; }
    [GraphQLName("email")] public string? Email { get; set; }
    [GraphQLName("identities")] public IEnumerable<CustomerIdentity> Identities { get; set; } = [];
    [GraphQLName("designation")] public string? Designation { get; set; }
    [GraphQLName("title")] public string? Title { get; set; }
    [GraphQLName("name")] public string? Name { get; set; }
    [GraphQLName("givenName")] public string? GivenName { get; set; }
    [GraphQLName("middleName")] public string? MiddleName { get; set; }
    [GraphQLName("familyName")] public string? FamilyName { get; set; }
    [GraphQLName("photoUrl")] public string? PhotoUrl { get; set; }
    [GraphQLName("photoUrl24")] public string? PhotoUrl24 { get; set; }
    [GraphQLName("photoUrl32")] public string? PhotoUrl32 { get; set; }
    [GraphQLName("photoUrl48")] public string? PhotoUrl48 { get; set; }
    [GraphQLName("photoUrl72")] public string? PhotoUrl72 { get; set; }
    [GraphQLName("photoUrl192")] public string? PhotoUrl192 { get; set; }
    [GraphQLName("photoUrl512")] public string? PhotoUrl512 { get; set; }
    [GraphQLName("timezone")] public string? Timezone { get; set; }
    [GraphQLName("locale")] public string? Locale { get; set; }
    [GraphQLName("phoneNumber")] public string? PhoneNumber { get; set; }

    [GraphQLName("isOrganizationOnboardingDone")]
    public bool IsOrganizationOnboardingDone { get; set; }

    [GraphQLName("isLocationOnboardingDone")]
    public bool IsLocationOnboardingDone { get; set; }

    [GraphQLName("isTeamOnboardingDone")] public bool IsTeamOnboardingDone { get; set; }

    [GraphQLName("isDefaultOrganizationOnboardingDone")]
    public bool IsDefaultOrganizationOnboardingDone { get; set; }

    [GraphQLName("isPreferredLocationOnboardingDone")]
    public bool IsPreferredLocationOnboardingDone { get; set; }

    [GraphQLName("isPreferredZoneOnboardingDone")]
    public bool IsPreferredZoneOnboardingDone { get; set; }

    [GraphQLName("defaultOrganization")] public OrganizationDetails? DefaultOrganization { get; set; }
    [GraphQLName("preferredLocations")] public IEnumerable<LocationDetails> PreferredLocations { get; set; } = [];
    [GraphQLName("preferredTeams")] public IEnumerable<CustomerTeamDetails> PreferredTeams { get; set; } = [];
    [GraphQLName("preferredZones")] public IEnumerable<OrganizationTagDetails> PreferredZones { get; set; } = [];
    [GraphQLName("preferredCustomTags")] public IEnumerable<OrganizationTagDetails> PreferredCustomTags { get; set; } = [];
    [GraphQLName("preferredResources")] public IEnumerable<CustomerResourceDetails> PreferredResources { get; set; } = [];
    [GraphQLName("paymentMethods")] public IEnumerable<CustomerPaymentMethod> PaymentMethods { get; set; } = [];
    [GraphQLName("billingDetails")] public CustomerBillingDetails? BillingDetails { get; set; }

    [GraphQLName("hasAttachedPaymentMethod")]
    public bool HasAttachedPaymentMethod { get; set; }

    [GraphQLName("id")] [ID] public string Id { get; set; } = string.Empty;
}

[GraphQLName("CustomerEdge")]
public class CustomerEdge(CustomerDetails node, string cursor) : Edge<CustomerDetails>(node, cursor);

[GraphQLName("CustomerIdentity")]
public class CustomerIdentity : Node
{
    [GraphQLName("email")] public string? Email { get; set; }
    [GraphQLName("verified")] public bool Verified { get; set; }
    [GraphQLName("id")] [ID] public string Id { get; set; } = string.Empty;
}

[GraphQLName("Customer_LocationDetails")]
public class LocationDetails
{
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; } = string.Empty;
    [GraphQLName("name")] public string? Name { get; set; }
    [GraphQLName("organization")] public OrganizationDetails? Organization { get; set; }
}

[GraphQLName("Customer_OrganizationTagDetails")]
public class OrganizationTagDetails
{
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; } = string.Empty;
    [GraphQLName("name")] public string? Name { get; set; }
    [GraphQLName("color")] public string? Color { get; set; }
}

[GraphQLName("CustomerOrderInput")]
public class CustomerOrderInput
{
    [GraphQLName("direction")] public OrderDirection Direction { get; set; }
    [GraphQLName("field")] public CustomerOrderField Field { get; set; }
}

[GraphQLName("Customer_OrganizationDetails")]
public class OrganizationDetails
{
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; } = string.Empty;
    [GraphQLName("name")] public string? Name { get; set; }
    [GraphQLName("logoUrl")] public string? LogoUrl { get; set; }
}

[GraphQLName("CustomerPayload")]
public class CustomerPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("customer")] public CustomerDetails Customer { get; set; } = new();
}

[GraphQLName("CustomerTeamDetails")]
public class CustomerTeamDetails
{
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; } = string.Empty;
    [GraphQLName("name")] public string? Name { get; set; }
    [GraphQLName("organization")] public OrganizationDetails? Organization { get; set; }
}

[GraphQLName("CustomersByPreferredLocationWhereInput")]
public class CustomersByPreferredLocationWhereInput
{
    [GraphQLName("locationId")] public string LocationId { get; set; } = string.Empty;
    [GraphQLName("nameContains")] public string? NameContains { get; set; }
}

[GraphQLName("RemoveCustomerPreferredLocationInput")]
public class RemoveCustomerPreferredLocationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("locationId")] public string LocationId { get; set; } = string.Empty;
}

[GraphQLName("RemoveCustomerPreferredOrganizationTagInput")]
public class RemoveCustomerPreferredOrganizationTagInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("organizationTagId")] public string OrganizationTagId { get; set; } = string.Empty;
}

[GraphQLName("RemoveCustomerPreferredTeamInput")]
public class RemoveCustomerPreferredTeamInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("teamId")] public string TeamId { get; set; } = string.Empty;
}

[GraphQLName("SetCustomerDefaultOrganizationInput")]
public class SetCustomerDefaultOrganizationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("organizationId")] public string OrganizationId { get; set; } = string.Empty;
}

[GraphQLName("SubmitCustomerFeedbackInput")]
public class SubmitCustomerFeedbackInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string? Id { get; set; }
    [GraphQLName("feedbackContent")] public string FeedbackContent { get; set; } = string.Empty;
    [GraphQLName("channel")] public FeedbackChannelType Channel { get; set; }
}

[GraphQLName("SubmitCustomerFeedbackPayload")]
public class SubmitCustomerFeedbackPayload
{
    [GraphQLName("id")] [ID] public string Id { get; set; } = string.Empty;
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}

[GraphQLName("UpdateMyCustomerDetailsInput")]
public class UpdateMyCustomerDetailsInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("timezone")] public string? Timezone { get; set; }
    [GraphQLName("designation")] public string? Designation { get; set; }
    [GraphQLName("title")] public string? Title { get; set; }
    [GraphQLName("name")] public string? Name { get; set; }
    [GraphQLName("givenName")] public string? GivenName { get; set; }
    [GraphQLName("middleName")] public string? MiddleName { get; set; }
    [GraphQLName("familyName")] public string? FamilyName { get; set; }
    [GraphQLName("phoneNumber")] public string? PhoneNumber { get; set; }
}

[GraphQLName("UpdateCustomerDetailsInput")]
public class UpdateCustomerDetailsInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
    [GraphQLName("timezone")] public string? Timezone { get; set; }
    [GraphQLName("designation")] public string? Designation { get; set; }
    [GraphQLName("title")] public string? Title { get; set; }
    [GraphQLName("name")] public string? Name { get; set; }
    [GraphQLName("givenName")] public string? GivenName { get; set; }
    [GraphQLName("middleName")] public string? MiddleName { get; set; }
    [GraphQLName("familyName")] public string? FamilyName { get; set; }
    [GraphQLName("phoneNumber")] public string? PhoneNumber { get; set; }
}

[GraphQLName("AddCustomerPreferredResourceInput")]
public class AddCustomerPreferredResourceInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("resourceId")] public string ResourceId { get; set; } = string.Empty;
}

[GraphQLName("RemoveCustomerPreferredResourceInput")]
public class RemoveCustomerPreferredResourceInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("resourceId")] public string ResourceId { get; set; } = string.Empty;
}

[GraphQLName("CustomerResourceDetails")]
public class CustomerResourceDetails
{
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; } = string.Empty;
    [GraphQLName("name")] public string? Name { get; set; }
}

[GraphQLName("CustomerPaymentMethod")]
public class CustomerPaymentMethod : Node
{
    [GraphQLName("cardBrand")] public string? CardBrand { get; set; }
    [GraphQLName("cardCountry")] public string? CardCountry { get; set; }
    [GraphQLName("cardDescription")] public string? CardDescription { get; set; }
    [GraphQLName("cardExpiryMonth")] public int? CardExpiryMonth { get; set; }
    [GraphQLName("cardExpiryYear")] public int? CardExpiryYear { get; set; }
    [GraphQLName("cardFingerprint")] public string? CardFingerprint { get; set; }
    [GraphQLName("cardFunding")] public string? CardFunding { get; set; }
    [GraphQLName("cardIssuer")] public string? CardIssuer { get; set; }
    [GraphQLName("cardLastFourDigit")] public string? CardLastFourDigit { get; set; }
    [GraphQLName("id")] [ID] public string Id { get; set; } = string.Empty;
}
