using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types.Relay;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Organization.Api.GraphQL;

[GraphQLName("AcceptInvitationToJoinOrganizationInput")]
public class AcceptInvitationToJoinOrganizationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("id")] public required string Id { get; set; }
}

[GraphQLName("AcceptInvitationToJoinOrganizationPayload")]
public class AcceptInvitationToJoinOrganizationPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}

[GraphQLName("AddOrganizationInput")]
public class AddOrganizationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("id")] public string? Id { get; set; }

    [GraphQLName("name")] public string Name { get; set; } = string.Empty;

    [GraphQLName("about")] public string? About { get; set; }

    [GraphQLName("website")] public string? Website { get; set; }

    [GraphQLName("agreedToTermsOfUse")] public bool AgreedToTermsOfUse { get; set; }

    [GraphQLName("termsOfUseId")] public string TermsOfUseId { get; set; } = string.Empty;

    [GraphQLName("industrySubCategoryIds")]
    public string[] IndustrySubCategoryIds { get; set; } = [];
}

[GraphQLName("CancelInvitationToJoinOrganizationInput")]
public class CancelInvitationToJoinOrganizationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("id")] public required string Id { get; set; }
}

[GraphQLName("CancelInvitationToJoinOrganizationPayload")]
public class CancelInvitationToJoinOrganizationPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}

[GraphQLName("CancelOrganizationOfferingInput")]
public class CancelOrganizationOfferingInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("id")] public required string Id { get; set; }
}

[GraphQLName("CancelOrganizationOfferingPayload")]
public class CancelOrganizationOfferingPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}

[GraphQLName("ChangeOrganizationMemberOwnershipTypeInput")]
public class ChangeOrganizationMemberOwnershipTypeInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("id")] public required string Id { get; set; }

    [GraphQLName("membershipType")] public OrganizationMemberMembershipType MembershipType { get; set; }
}

[GraphQLName("DeleteOrganizationInput")]
public class DeleteOrganizationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("id")] public required string Id { get; set; }
}

[GraphQLName("InviteCustomersToJoinOrganizationInput")]
public class InviteCustomersToJoinOrganizationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("organizationId")] public string OrganizationId { get; set; } = string.Empty;

    [GraphQLName("emails")] public string[] Emails { get; set; } = [];
}

[GraphQLName("InviteCustomersToJoinOrganizationPayload")]
public class InviteCustomersToJoinOrganizationPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}

[GraphQLName("OrganizationAnalytics")]
public class OrganizationAnalytics
{
    [GraphQLName("memberAttendancePercentage")]
    public OrganizationMemberAttendancePercentage[] MemberAttendancePercentage { get; set; } = [];

    [GraphQLName("dailyBookingsTotals")] public OrganizationDailyBookingsTotal[] DailyBookingsTotals { get; set; } = [];
}

[GraphQLName("OrganizationAvailableOfferingDetails")]
public class OrganizationAvailableOfferingDetails
{
    [GraphQLName("code")] public string Code { get; set; } = string.Empty;

    [GraphQLName("name")] public string Name { get; set; } = string.Empty;

    [GraphQLName("unitPrice")] public int UnitPrice { get; set; }

    [GraphQLName("featureSet")] public OrganizationFeatureSetDetails[] FeatureSet { get; set; } = [];

    [GraphQLName("free")] public bool Free { get; set; }
}

[GraphQLName("OrganizationConnection")]
public class OrganizationConnection : Connection<OrganizationEdge>;

[GraphQLName("OrganizationCustomerDetails")]
public class OrganizationCustomerDetails
{
    [GraphQLName("uniqueId")] [ID] public required string UniqueId { get; set; }

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
}

[GraphQLName("OrganizationDailyBookingsTotal")]
public class OrganizationDailyBookingsTotal
{
    [GraphQLName("date")] public DateTimeOffset Date { get; set; }

    [GraphQLName("total")] public int Total { get; set; }
}

[GraphQLName("OrganizationDetails")]
public class OrganizationDetails : Node
{
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;

    [GraphQLName("about")] public string? About { get; set; }

    [GraphQLName("website")] public string? Website { get; set; }

    [GraphQLName("logoUrl")] public string? LogoUrl { get; set; }

    [GraphQLName("agreedToTermsOfUse")] public bool AgreedToTermsOfUse { get; set; }

    [GraphQLName("termsOfUse")] public OrganizationTermsOfUse? TermsOfUse { get; set; }

    [GraphQLName("industrySubCategories")]
    public OrganizationIndustrySubCategoryReferenceDetails[] IndustrySubCategories { get; set; } = [];

    [GraphQLName("availableOfferings")]
    public OrganizationAvailableOfferingDetails[] AvailableOfferings { get; set; } = [];

    [GraphQLName("offering")] public OrganizationOfferingDetails Offering { get; set; }

    [GraphQLName("hasAttachedPaymentMethod")]
    public bool HasAttachedPaymentMethod { get; set; }

    [GraphQLName("hasFutureBooking")] public bool HasFutureBooking { get; set; }

    [GraphQLName("hasLocation")] public bool HasLocation { get; set; }

    [GraphQLName("hasTeam")] public bool HasTeam { get; set; }

    [GraphQLName("canModify")] public bool CanModify { get; set; }

    [GraphQLName("canDelete")] public bool CanDelete { get; set; }

    [GraphQLName("canInvitePeople")] public bool CanInvitePeople { get; set; }

    [GraphQLName("canViewAnalytics")] public bool CanViewAnalytics { get; set; }

    [GraphQLName("isMyOnboardingDone")] public bool IsMyOnboardingDone { get; set; }

    [GraphQLName("id")] [ID] public required string Id { get; set; }
}

[GraphQLName("OrganizationEdge")]
public class OrganizationEdge : Edge<OrganizationDetails>;

[GraphQLName("OrganizationFeatureSetDetails")]
public class OrganizationFeatureSetDetails
{
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;

    [GraphQLName("description")] public string Description { get; set; } = string.Empty;
}

[GraphQLName("OrganizationIndustryMainCategoryReferenceDetails")]
public class OrganizationIndustryMainCategoryReferenceDetails : Node
{
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;

    [GraphQLName("subCategories")]
    public OrganizationIndustrySubCategoryReferenceDetails[] SubCategories { get; set; } = [];

    [GraphQLName("id")] [ID] public required string Id { get; set; }
}

[GraphQLName("OrganizationIndustrySubCategoryReferenceDetails")]
public class OrganizationIndustrySubCategoryReferenceDetails : Node
{
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;

    [GraphQLName("mainCategoryName")] public string MainCategoryName { get; set; } = string.Empty;

    [GraphQLName("id")] [ID] public required string Id { get; set; }
}

[GraphQLName("OrganizationMemberAttendancePercentage")]
public class OrganizationMemberAttendancePercentage
{
    [GraphQLName("date")] public DateTimeOffset Date { get; set; }

    [GraphQLName("percentage")] public float Percentage { get; set; }
}

[GraphQLName("OrganizationMemberConnection")]
public class OrganizationMemberConnection : Connection<OrganizationMemberEdge>;

[GraphQLName("OrganizationMemberDetails")]
public class OrganizationMemberDetails : Node
{
    [GraphQLName("membershipType")] public OrganizationMemberMembershipType? MembershipType { get; set; }

    [GraphQLName("isOrganizationOnboardingDone")]
    public bool IsOrganizationOnboardingDone { get; set; }

    [GraphQLName("customer")] public OrganizationCustomerDetails Customer { get; set; }

    [GraphQLName("id")] [ID] public required string Id { get; set; }
}

[GraphQLName("OrganizationMemberDetailsPayload")]
public class OrganizationMemberDetailsPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("member")] public OrganizationMemberDetails? Member { get; set; }
}

[GraphQLName("OrganizationMemberEdge")]
public class OrganizationMemberEdge : Edge<OrganizationMemberDetails>;

public enum OrganizationMemberMembershipType
{
    Owner,
    Administrator,
    Member
}

public enum OrganizationMemberOrderField
{
    MembershipType,
    Name,
    GivenName,
    MiddleName,
    FamilyName
}

[GraphQLName("OrganizationMemberOrderInput")]
public class OrganizationMemberOrderInput
{
    [GraphQLName("direction")] public OrderDirection Direction { get; set; }

    [GraphQLName("field")] public OrganizationMemberOrderField Field { get; set; }
}

[GraphQLName("OrganizationMemberWhereInput")]
public class OrganizationMemberWhereInput
{
    [GraphQLName("organizationId")] public string OrganizationId { get; set; } = string.Empty;

    [GraphQLName("nameContains")] public string? NameContains { get; set; }
}

[GraphQLName("OrganizationOfferingDetails")]
public class OrganizationOfferingDetails : Node
{
    [GraphQLName("code")] public string Code { get; set; } = string.Empty;

    [GraphQLName("name")] public string Name { get; set; } = string.Empty;

    [GraphQLName("start")] public DateTimeOffset Start { get; set; }

    [GraphQLName("end")] public DateTimeOffset End { get; set; }

    [GraphQLName("unitPrice")] public int UnitPrice { get; set; }

    [GraphQLName("featureSet")] public OrganizationFeatureSetDetails[] FeatureSet { get; set; } = [];

    [GraphQLName("free")] public bool Free { get; set; }

    [GraphQLName("id")] [ID] public string Id { get; set; } = string.Empty;
}

public enum OrganizationOrderField
{
    Name
}

[GraphQLName("OrganizationOrderInput")]
public class OrganizationOrderInput
{
    [GraphQLName("direction")] public OrderDirection Direction { get; set; }

    [GraphQLName("field")] public OrganizationOrderField Field { get; set; }
}

[GraphQLName("OrganizationPayload")]
public class OrganizationPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("organization")] public OrganizationDetails Organization { get; set; }
}

[GraphQLName("OrganizationTermsOfUse")]
public class OrganizationTermsOfUse : Node
{
    [GraphQLName("terms")] public string Terms { get; set; } = string.Empty;

    [GraphQLName("id")] [ID] public required string Id { get; set; }
}

[GraphQLName("OrganizationWhereInput")]
public class OrganizationWhereInput
{
    [GraphQLName("nameContains")] public string? NameContains { get; set; }
}

[GraphQLName("RejectInvitationToJoinOrganizationInput")]
public class RejectInvitationToJoinOrganizationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("id")] public required string Id { get; set; }
}

[GraphQLName("RejectInvitationToJoinOrganizationPayload")]
public class RejectInvitationToJoinOrganizationPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}

[GraphQLName("UpdateOrganizationInput")]
public class UpdateOrganizationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("id")] public required string Id { get; set; }

    [GraphQLName("name")] public string Name { get; set; } = string.Empty;

    [GraphQLName("about")] public string? About { get; set; }

    [GraphQLName("website")] public string? Website { get; set; }

    [GraphQLName("industrySubCategoryIds")]
    public string[] IndustrySubCategoryIds { get; set; } = [];
}

[GraphQLName("UpdateOrganizationOfferingInput")]
public class UpdateOrganizationOfferingInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("id")] public required string Id { get; set; }

    [GraphQLName("offeringCode")] public string OfferingCode { get; set; } = string.Empty;
}

[GraphQLName("UpdateOrganizationOfferingPayload")]
public class UpdateOrganizationOfferingPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}

[GraphQLName("CompleteOrganizationMemberOnboardingInput")]
public class CompleteOrganizationMemberOnboardingInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("organizationId")] public string OrganizationId { get; set; } = string.Empty;
}

[GraphQLName("OrganizationMemberPayload")]
public class OrganizationMemberPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}

[GraphQLName("AddOrganizationTagInput")]
public class AddOrganizationTagInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("id")] public string? Id { get; set; }

    [GraphQLName("name")] public string Name { get; set; } = string.Empty;

    [GraphQLName("description")] public string? Description { get; set; }

    [GraphQLName("tagType")] public string TagType { get; set; } = string.Empty;

    [GraphQLName("organizationId")] public required string OrganizationId { get; set; }
}

[GraphQLName("UpdateOrganizationTagInput")]
public class UpdateOrganizationTagInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("id")] public required string Id { get; set; }

    [GraphQLName("name")] public string Name { get; set; }= string.Empty;

    [GraphQLName("description")] public string? Description { get; set; }

    [GraphQLName("tagType")] public string TagType { get; set; }= string.Empty;
}

[GraphQLName("DeleteOrganizationTagInput")]
public class DeleteOrganizationTagInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("id")] public required string Id { get; set; }
}

[GraphQLName("OrganizationTagConnection")]
public class OrganizationTagConnection : Connection<OrganizationTagEdge>;

[GraphQLName("OrganizationTagDetails")]
public class OrganizationTagDetails : Node
{
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;

    [GraphQLName("description")] public string? Description { get; set; }

    [GraphQLName("tagType")] public string TagType { get; set; } = string.Empty;

    [GraphQLName("id")][ID] public required string Id { get; set; }
}

[GraphQLName("OrganizationTagEdge")]
public class OrganizationTagEdge : Edge<OrganizationTagDetails>;

public enum OrganizationTagOrderField
{
    Name,
    Description,
    TagType
}

[GraphQLName("OrganizationTagOrderInput")]
public class OrganizationTagOrderInput
{
    [GraphQLName("direction")] public OrderDirection Direction { get; set; }

    [GraphQLName("field")] public OrganizationTagOrderField Field { get; set; }
}

[GraphQLName("OrganizationTagPayload")]
public class OrganizationTagPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("organizationTag")] public OrganizationTagDetails OrganizationTag { get; set; }
}

[GraphQLName("OrganizationTagWhereInput")]
public class OrganizationTagWhereInput
{
    [GraphQLName("organizationId")] public required string OrganizationId { get; set; }

    [GraphQLName("tagType")] public string? TagType { get; set; }

    [GraphQLName("nameContains")] public string? NameContains { get; set; }
}

[GraphQLName("DeskTypeOrganizationTagWhereInput")]
public class DeskTypeOrganizationTagWhereInput
{
    [GraphQLName("organizationId")] public required string OrganizationId { get; set; }

    [GraphQLName("nameContains")] public string? NameContains { get; set; }
}

[GraphQLName("ZoneOrganizationTagWhereInput")]
public class ZoneOrganizationTagWhereInput
{
    [GraphQLName("organizationId")] public required string OrganizationId { get; set; }

    [GraphQLName("nameContains")] public string? NameContains { get; set; }
}

