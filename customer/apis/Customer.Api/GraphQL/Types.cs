using Customer.Shared.Models;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types.Relay;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Customer.Api.GraphQL;

[GraphQLName("AddCustomerPreferredDeskInput")]
public class AddCustomerPreferredDeskInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("deskId")] public required string DeskId { get; set; }
}

[GraphQLName("AddCustomerPreferredRoomInput")]
public class AddCustomerPreferredRoomInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("roomId")] public required string RoomId { get; set; }
}

[GraphQLName("AddCustomerPreferredLocationInput")]
public class AddCustomerPreferredLocationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("locationId")] public required string LocationId { get; set; }
}

[GraphQLName("AddCustomerPreferredOrganizationTagInput")]
public class AddCustomerPreferredOrganizationTagInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("organizationTagId")] public required string OrganizationTagId { get; set; }
}

[GraphQLName("AddCustomerPreferredTeamInput")]
public class AddCustomerPreferredTeamInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("teamId")] public required string TeamId { get; set; }
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

[GraphQLName("CompletePreferredDeskOnboardingInput")]
public class CompletePreferredDeskOnboardingInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}

[GraphQLName("CompletePreferredRoomOnboardingInput")]
public class CompletePreferredRoomOnboardingInput
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
public class CustomerConnection : Connection<CustomerEdge>;

[GraphQLName("CustomerDeskDetails")]
public class CustomerDeskDetails
{
    [GraphQLName("uniqueId")] [ID] public required string UniqueId { get; set; }
    [GraphQLName("name")] public string? Name { get; set; }
}

[GraphQLName("CustomerRoomDetails")]
public class CustomerRoomDetails
{
    [GraphQLName("uniqueId")] [ID] public required string UniqueId { get; set; }
    [GraphQLName("name")] public string? Name { get; set; }
}

[GraphQLName("CustomerDetails")]
public class CustomerDetails : Node
{
    [GraphQLName("createdAt")] public DateTimeOffset CreatedAt { get; set; }
    [GraphQLName("email")] public string? Email { get; set; }
    [GraphQLName("identities")] public CustomerIdentity[] Identities { get; set; } = [];
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

    [GraphQLName("isPreferredDeskOnboardingDone")]
    public bool IsPreferredDeskOnboardingDone { get; set; }

    [GraphQLName("isPreferredRoomOnboardingDone")]
    public bool IsPreferredRoomOnboardingDone { get; set; }

    [GraphQLName("defaultOrganization")] public CustomerOrganizationDetails? DefaultOrganization { get; set; }
    [GraphQLName("preferredLocations")] public CustomerLocationDetails[] PreferredLocations { get; set; } = [];
    [GraphQLName("preferredTeams")] public CustomerTeamDetails[] PreferredTeams { get; set; } = [];
    [GraphQLName("preferredZones")] public CustomerOrganizationTagDetails[] PreferredZones { get; set; } = [];
    [GraphQLName("preferredCustomTags")] public CustomerOrganizationTagDetails[] PreferredCustomTags { get; set; } = [];
    [GraphQLName("preferredDesks")] public CustomerDeskDetails[] PreferredDesks { get; set; } = [];
    [GraphQLName("preferredRooms")] public CustomerRoomDetails[] PreferredRooms { get; set; } = [];
    [GraphQLName("id")] [ID] public required string Id { get; set; }
}

[GraphQLName("CustomerEdge")]
public class CustomerEdge : Edge<CustomerDetails>;

[GraphQLName("CustomerEmail")]
public class CustomerIdentity : Node
{
    [GraphQLName("email")] public string? Email { get; set; }
    [GraphQLName("verified")] public bool Verified { get; set; }
    [GraphQLName("id")] [ID] public required string Id { get; set; }
}

[GraphQLName("CustomerLocationDetails")]
public class CustomerLocationDetails
{
    [GraphQLName("uniqueId")] [ID] public required string UniqueId { get; set; }
    [GraphQLName("name")] public string? Name { get; set; }
    [GraphQLName("organization")] public CustomerOrganizationDetails? Organization { get; set; }
}

[GraphQLName("CustomerOrganizationTagDetails")]
public class CustomerOrganizationTagDetails
{
    [GraphQLName("uniqueId")] [ID] public required string UniqueId { get; set; }
    [GraphQLName("name")] public string? Name { get; set; }
    [GraphQLName("color")] public string? Color { get; set; }
}

[GraphQLName("CustomerOrderInput")]
public class CustomerOrderInput
{
    [GraphQLName("direction")] public OrderDirection Direction { get; set; }
    [GraphQLName("field")] public CustomerOrderField Field { get; set; }
}

[GraphQLName("CustomerOrganizationDetails")]
public class CustomerOrganizationDetails
{
    [GraphQLName("uniqueId")] [ID] public required string UniqueId { get; set; }
    [GraphQLName("name")] public string? Name { get; set; }
    [GraphQLName("logoUrl")] public string? LogoUrl { get; set; }
}

[GraphQLName("CustomerPayload")]
public class CustomerPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("customer")] public CustomerDetails Customer { get; set; }
}

[GraphQLName("CustomerTeamDetails")]
public class CustomerTeamDetails
{
    [GraphQLName("uniqueId")] [ID] public required string UniqueId { get; set; }
    [GraphQLName("name")] public string? Name { get; set; }
    [GraphQLName("organization")] public CustomerOrganizationDetails? Organization { get; set; }
}

[GraphQLName("CustomersByPreferredLocationWhereInput")]
public class CustomersByPreferredLocationWhereInput
{
    [GraphQLName("locationId")] public required string LocationId { get; set; }
    [GraphQLName("nameContains")] public string? NameContains { get; set; }
}

[GraphQLName("RemoveCustomerPreferredDeskInput")]
public class RemoveCustomerPreferredDeskInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("deskId")] public required string DeskId { get; set; }
}

[GraphQLName("RemoveCustomerPreferredRoomInput")]
public class RemoveCustomerPreferredRoomInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("roomId")] public required string RoomId { get; set; }
}

[GraphQLName("RemoveCustomerPreferredLocationInput")]
public class RemoveCustomerPreferredLocationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("locationId")] public required string LocationId { get; set; }
}

[GraphQLName("RemoveCustomerPreferredOrganizationTagInput")]
public class RemoveCustomerPreferredOrganizationTagInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("organizationTagId")] public required string OrganizationTagId { get; set; }
}

[GraphQLName("RemoveCustomerPreferredTeamInput")]
public class RemoveCustomerPreferredTeamInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("teamId")] public required string TeamId { get; set; }
}

[GraphQLName("SetCustomerDefaultOrganizationInput")]
public class SetCustomerDefaultOrganizationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("organizationId")] public required string OrganizationId { get; set; }
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
    [GraphQLName("id")] [ID] public required string Id { get; set; }
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
    [GraphQLName("id")] public required string Id { get; set; }
    [GraphQLName("timezone")] public string? Timezone { get; set; }
    [GraphQLName("designation")] public string? Designation { get; set; }
    [GraphQLName("title")] public string? Title { get; set; }
    [GraphQLName("name")] public string? Name { get; set; }
    [GraphQLName("givenName")] public string? GivenName { get; set; }
    [GraphQLName("middleName")] public string? MiddleName { get; set; }
    [GraphQLName("familyName")] public string? FamilyName { get; set; }
    [GraphQLName("phoneNumber")] public string? PhoneNumber { get; set; }
}
