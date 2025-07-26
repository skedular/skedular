using Customer.Api.GraphQL.Billing;
using Customer.Api.GraphQL.Payment;
using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types.Relay;

namespace Customer.Api.GraphQL.Customer;

[GraphQLName("CustomerDetails")]
public class CustomerDetails : Node
{
    [GraphQLName("createdAt")] public DateTimeOffset CreatedAt { get; set; }
    [GraphQLName("email")] public string? Email { get; set; }
    [GraphQLName("emails")] public ICollection<string> Emails { get; set; } = [];
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
    [GraphQLName("isOnboardingDone")] public bool IsOnboardingDone { get; set; }
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
