using HotChocolate;
using HotChocolate.Types.Relay;

namespace Customer.Api.GraphQL;

[GraphQLName("AddCustomerDefaultDeskInput")]
public class AddCustomerDefaultDeskInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("deskId")] public string DeskId { get; set; }
}

[GraphQLName("AddCustomerDefaultLocationInput")]
public class AddCustomerDefaultLocationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("locationId")] public string LocationId { get; set; }
}

[GraphQLName("AddCustomerDefaultLocationTagInput")]
public class AddCustomerDefaultLocationTagInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("locationTagId")] public string LocationTagId { get; set; }
}

[GraphQLName("AddCustomerDefaultTeamInput")]
public class AddCustomerDefaultTeamInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("teamId")] public string TeamId { get; set; }
}

[GraphQLName("ClearCustomerDefaultOrganizationInput")]
public class ClearCustomerDefaultOrganizationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}

[GraphQLName("CompleteDefaultLocationOnboardingInput")]
public class CompleteDefaultLocationOnboardingInput
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
public class CustomerConnection
{
    [GraphQLName("pageInfo")] public PageInfo PageInfo { get; set; }

    [GraphQLName("edges")] public CustomerEdge[] Edges { get; set; }

    [GraphQLName("totalCount")] public int? TotalCount { get; set; }
}

[GraphQLName("CustomerDeskDetails")]
public class CustomerDeskDetails
{
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; }

    [GraphQLName("name")] public string? Name { get; set; }
}

[GraphQLName("CustomerDetails")]
public class CustomerDetails : Node
{
    [GraphQLName("createdAt")] public DateTimeOffset CreatedAt { get; set; }

    [GraphQLName("email")] public CustomerEmail? Email { get; set; }

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

    [GraphQLName("isOrganizationOnboardingDone")]
    public bool IsOrganizationOnboardingDone { get; set; }

    [GraphQLName("isLocationOnboardingDone")]
    public bool IsLocationOnboardingDone { get; set; }

    [GraphQLName("isTeamOnboardingDone")] public bool IsTeamOnboardingDone { get; set; }

    [GraphQLName("isDefaultOrganizationOnboardingDone")]
    public bool IsDefaultOrganizationOnboardingDone { get; set; }

    [GraphQLName("isDefaultLocationOnboardingDone")]
    public bool IsDefaultLocationOnboardingDone { get; set; }

    [GraphQLName("isPreferredZoneOnboardingDone")]
    public bool IsPreferredZoneOnboardingDone { get; set; }

    [GraphQLName("isPreferredDeskOnboardingDone")]
    public bool IsPreferredDeskOnboardingDone { get; set; }

    [GraphQLName("defaultLocations")] public CustomerLocationDetails[] DefaultLocations { get; set; }

    [GraphQLName("defaultTeams")] public CustomerTeamDetails[] DefaultTeams { get; set; }

    [GraphQLName("defaultOrganization")] public CustomerOrganizationDetails? DefaultOrganization { get; set; }

    [GraphQLName("preferredZones")] public CustomerLocationTagDetails[] PreferredZones { get; set; }

    [GraphQLName("preferredDesks")] public CustomerDeskDetails[] PreferredDesks { get; set; }

    [GraphQLName("id")] [ID] public string Id { get; set; }
}

[GraphQLName("CustomerEdge")]
public class CustomerEdge
{
    [GraphQLName("node")] public CustomerDetails Node { get; set; }

    [GraphQLName("cursor")] public string Cursor { get; set; }
}

[GraphQLName("CustomerEmail")]
public class CustomerEmail : Node
{
    [GraphQLName("email")] public string Email { get; set; }

    [GraphQLName("verified")] public bool Verified { get; set; }

    [GraphQLName("id")] [ID] public string Id { get; set; }
}

[GraphQLName("CustomerLocationDetails")]
public class CustomerLocationDetails
{
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; }

    [GraphQLName("name")] public string? Name { get; set; }

    [GraphQLName("organization")] public CustomerOrganizationDetails? Organization { get; set; }
}

[GraphQLName("CustomerLocationTagDetails")]
public class CustomerLocationTagDetails
{
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; }

    [GraphQLName("name")] public string? Name { get; set; }
}

public enum CustomerOrderField
{
    Designation,
    Title,
    Name,
    GivenName,
    MiddleName,
    FamilyName,
    Timezone,
    Locale
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
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; }

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
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; }

    [GraphQLName("name")] public string? Name { get; set; }

    [GraphQLName("organization")] public CustomerOrganizationDetails? Organization { get; set; }
}

[GraphQLName("CustomerWhereInput")]
public class CustomerWhereInput
{
    [GraphQLName("locationId")] public string LocationId { get; set; }

    [GraphQLName("nameContains")] public string? NameContains { get; set; }
}

public enum FeedbackChannel
{
    Web,
    Slack,
    MsTeams
}

[GraphQLName("Node")]
public interface Node
{
    [GraphQLName("id")] [ID] public string Id { get; set; }
}

public enum OrderDirection
{
    Ascending,
    Descending
}

[GraphQLName("PageInfo")]
public class PageInfo
{
    [GraphQLName("hasNextPage")] public bool HasNextPage { get; set; }

    [GraphQLName("hasPreviousPage")] public bool HasPreviousPage { get; set; }

    [GraphQLName("startCursor")] public string? StartCursor { get; set; }

    [GraphQLName("endCursor")] public string? EndCursor { get; set; }
}

[GraphQLName("RemoveCustomerDefaultDeskInput")]
public class RemoveCustomerDefaultDeskInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("deskId")] public string DeskId { get; set; }
}

[GraphQLName("RemoveCustomerDefaultLocationInput")]
public class RemoveCustomerDefaultLocationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("locationId")] public string LocationId { get; set; }
}

[GraphQLName("RemoveCustomerDefaultLocationTagInput")]
public class RemoveCustomerDefaultLocationTagInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("locationTagId")] public string LocationTagId { get; set; }
}

[GraphQLName("RemoveCustomerDefaultTeamInput")]
public class RemoveCustomerDefaultTeamInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("teamId")] public string TeamId { get; set; }
}

[GraphQLName("SetCustomerDefaultOrganizationInput")]
public class SetCustomerDefaultOrganizationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("organizationId")] public string OrganizationId { get; set; }
}

[GraphQLName("SubmitCustomerFeedbackInput")]
public class SubmitCustomerFeedbackInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("id")] public string Id { get; set; }

    [GraphQLName("feedbackContent")] public string FeedbackContent { get; set; }

    [GraphQLName("channel")] public FeedbackChannel Channel { get; set; }
}

[GraphQLName("SubmitCustomerFeedbackPayload")]
public class SubmitCustomerFeedbackPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("id")] [ID] public string Id { get; set; }
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
}

[GraphQLName("Version")]
public class Version
{
    [GraphQLName("major")] public int Major { get; set; }

    [GraphQLName("minor")] public int Minor { get; set; }

    [GraphQLName("build")] public int Build { get; set; }

    [GraphQLName("revision")] public int Revision { get; set; }
}
