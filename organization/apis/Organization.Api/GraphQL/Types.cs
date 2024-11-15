using HotChocolate;
using HotChocolate.Types.Relay;

namespace Organization.Api.GraphQL;

[GraphQLName("AcceptInvitationToJoinOrganizationInput")]
public class AcceptInvitationToJoinOrganizationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("id")] public string Id { get; set; }
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

    [GraphQLName("name")] public string Name { get; set; }

    [GraphQLName("about")] public string? About { get; set; }

    [GraphQLName("website")] public string? Website { get; set; }

    [GraphQLName("agreedToTermsOfUse")] public bool AgreedToTermsOfUse { get; set; }

    [GraphQLName("termsOfUseId")] public string TermsOfUseId { get; set; }

    [GraphQLName("industrySubCategoryIds")]
    public string[] IndustrySubCategoryIds { get; set; }
}

[GraphQLName("CancelInvitationToJoinOrganizationInput")]
public class CancelInvitationToJoinOrganizationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("id")] public string Id { get; set; }
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

    [GraphQLName("id")] public string Id { get; set; }
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

    [GraphQLName("id")] public string Id { get; set; }

    [GraphQLName("membershipType")] public OrganizationMemberMembershipType MembershipType { get; set; }
}

[GraphQLName("DeleteOrganizationInput")]
public class DeleteOrganizationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("id")] public string Id { get; set; }
}

[GraphQLName("InviteCustomersToJoinOrganizationInput")]
public class InviteCustomersToJoinOrganizationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("organizationId")] public string OrganizationId { get; set; }

    [GraphQLName("emails")] public string[] Emails { get; set; }
}

[GraphQLName("InviteCustomersToJoinOrganizationPayload")]
public class InviteCustomersToJoinOrganizationPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}

[GraphQLName("Mutation_AcceptInvitationToJoinOrganization_Arguments")]
public class Mutation_AcceptInvitationToJoinOrganization_Arguments
{
    [GraphQLName("input")] public AcceptInvitationToJoinOrganizationInput Input { get; set; }
}

[GraphQLName("Mutation_AddOrganization_Arguments")]
public class Mutation_AddOrganization_Arguments
{
    [GraphQLName("input")] public AddOrganizationInput Input { get; set; }
}

[GraphQLName("Mutation_CancelInvitationToJoinOrganization_Arguments")]
public class Mutation_CancelInvitationToJoinOrganization_Arguments
{
    [GraphQLName("input")] public CancelInvitationToJoinOrganizationInput Input { get; set; }
}

[GraphQLName("Mutation_CancelOrganizationOffering_Arguments")]
public class Mutation_CancelOrganizationOffering_Arguments
{
    [GraphQLName("input")] public CancelOrganizationOfferingInput Input { get; set; }
}

[GraphQLName("Mutation_ChangeOrganizationMemberOwnershipType_Arguments")]
public class Mutation_ChangeOrganizationMemberOwnershipType_Arguments
{
    [GraphQLName("input")] public ChangeOrganizationMemberOwnershipTypeInput Input { get; set; }
}

[GraphQLName("Mutation_DeleteOrganization_Arguments")]
public class Mutation_DeleteOrganization_Arguments
{
    [GraphQLName("input")] public DeleteOrganizationInput Input { get; set; }
}

[GraphQLName("Mutation_InviteCustomersToJoinOrganization_Arguments")]
public class Mutation_InviteCustomersToJoinOrganization_Arguments
{
    [GraphQLName("input")] public InviteCustomersToJoinOrganizationInput Input { get; set; }
}

[GraphQLName("Mutation_RejectInvitationToJoinOrganization_Arguments")]
public class Mutation_RejectInvitationToJoinOrganization_Arguments
{
    [GraphQLName("input")] public RejectInvitationToJoinOrganizationInput Input { get; set; }
}

[GraphQLName("Mutation_UpdateOrganization_Arguments")]
public class Mutation_UpdateOrganization_Arguments
{
    [GraphQLName("input")] public UpdateOrganizationInput Input { get; set; }
}

[GraphQLName("Mutation_UpdateOrganizationOffering_Arguments")]
public class Mutation_UpdateOrganizationOffering_Arguments
{
    [GraphQLName("input")] public UpdateOrganizationOfferingInput Input { get; set; }
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

[GraphQLName("OrganizationAnalytics")]
public class OrganizationAnalytics
{
    [GraphQLName("memberAttendancePercentage")]
    public OrganizationMemberAttendancePercentage[] MemberAttendancePercentage { get; set; }

    [GraphQLName("dailyBookingsTotals")] public OrganizationDailyBookingsTotal[] DailyBookingsTotals { get; set; }
}

[GraphQLName("OrganizationAvailableOfferingDetails")]
public class OrganizationAvailableOfferingDetails
{
    [GraphQLName("code")] public string Code { get; set; }

    [GraphQLName("name")] public string Name { get; set; }

    [GraphQLName("unitPrice")] public int UnitPrice { get; set; }

    [GraphQLName("featureSet")] public OrganizationFeatureSetDetails[] FeatureSet { get; set; }

    [GraphQLName("free")] public bool Free { get; set; }
}

[GraphQLName("OrganizationConnection")]
public class OrganizationConnection
{
    [GraphQLName("pageInfo")] public PageInfo PageInfo { get; set; }

    [GraphQLName("edges")] public OrganizationEdge[] Edges { get; set; }

    [GraphQLName("totalCount")] public int? TotalCount { get; set; }
}

[GraphQLName("OrganizationCustomerDetails")]
public class OrganizationCustomerDetails
{
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; }

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
    [GraphQLName("name")] public string Name { get; set; }

    [GraphQLName("about")] public string? About { get; set; }

    [GraphQLName("website")] public string? Website { get; set; }

    [GraphQLName("logoUrl")] public string? LogoUrl { get; set; }

    [GraphQLName("agreedToTermsOfUse")] public bool AgreedToTermsOfUse { get; set; }

    [GraphQLName("termsOfUse")] public OrganizationTermsOfUse? TermsOfUse { get; set; }

    [GraphQLName("industrySubCategories")]
    public OrganizationIndustrySubCategoryReferenceDetails[] IndustrySubCategories { get; set; }

    [GraphQLName("availableOfferings")] public OrganizationAvailableOfferingDetails[] AvailableOfferings { get; set; }

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

    [GraphQLName("id")] [ID] public string Id { get; set; }
}

[GraphQLName("OrganizationEdge")]
public class OrganizationEdge
{
    [GraphQLName("node")] public OrganizationDetails Node { get; set; }

    [GraphQLName("cursor")] public string Cursor { get; set; }
}

[GraphQLName("OrganizationFeatureSetDetails")]
public class OrganizationFeatureSetDetails
{
    [GraphQLName("name")] public string Name { get; set; }

    [GraphQLName("description")] public string Description { get; set; }
}

[GraphQLName("OrganizationIndustryMainCategoryReferenceDetails")]
public class OrganizationIndustryMainCategoryReferenceDetails : Node
{
    [GraphQLName("name")] public string Name { get; set; }

    [GraphQLName("subCategories")] public OrganizationIndustrySubCategoryReferenceDetails[] SubCategories { get; set; }

    [GraphQLName("id")] [ID] public string Id { get; set; }
}

[GraphQLName("OrganizationIndustrySubCategoryReferenceDetails")]
public class OrganizationIndustrySubCategoryReferenceDetails : Node
{
    [GraphQLName("name")] public string Name { get; set; }

    [GraphQLName("mainCategoryName")] public string MainCategoryName { get; set; }

    [GraphQLName("id")] [ID] public string Id { get; set; }
}

public enum OrganizationJoinInvitationStatus
{
    PENDING,
    ACCEPTED,
    REJECTED
}

[GraphQLName("OrganizationMemberAttendancePercentage")]
public class OrganizationMemberAttendancePercentage
{
    [GraphQLName("date")] public DateTimeOffset Date { get; set; }

    [GraphQLName("percentage")] public float Percentage { get; set; }
}

[GraphQLName("OrganizationMemberConnection")]
public class OrganizationMemberConnection
{
    [GraphQLName("pageInfo")] public PageInfo PageInfo { get; set; }

    [GraphQLName("edges")] public OrganizationMemberEdge[] Edges { get; set; }

    [GraphQLName("totalCount")] public int? TotalCount { get; set; }
}

[GraphQLName("OrganizationMemberDetails")]
public class OrganizationMemberDetails : Node
{
    [GraphQLName("membershipType")] public OrganizationMemberMembershipType? MembershipType { get; set; }

    [GraphQLName("customer")] public OrganizationCustomerDetails Customer { get; set; }

    [GraphQLName("id")] [ID] public string Id { get; set; }
}

[GraphQLName("OrganizationMemberDetailsPayload")]
public class OrganizationMemberDetailsPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("member")] public OrganizationMemberDetails? Member { get; set; }
}

[GraphQLName("OrganizationMemberEdge")]
public class OrganizationMemberEdge
{
    [GraphQLName("node")] public OrganizationMemberDetails Node { get; set; }

    [GraphQLName("cursor")] public string Cursor { get; set; }
}

public enum OrganizationMemberMembershipType
{
    OWNER,
    ADMINISTRATOR,
    MEMBER
}

public enum OrganizationMemberOrderField
{
    membershipType,
    name,
    givenName,
    middleName,
    familyName
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
    [GraphQLName("organizationId")] public string OrganizationId { get; set; }

    [GraphQLName("nameContains")] public string? NameContains { get; set; }
}

[GraphQLName("OrganizationOfferingDetails")]
public class OrganizationOfferingDetails : Node
{
    [GraphQLName("code")] public string Code { get; set; }

    [GraphQLName("name")] public string Name { get; set; }

    [GraphQLName("start")] public DateTimeOffset Start { get; set; }

    [GraphQLName("end")] public DateTimeOffset End { get; set; }

    [GraphQLName("unitPrice")] public int UnitPrice { get; set; }

    [GraphQLName("featureSet")] public OrganizationFeatureSetDetails[] FeatureSet { get; set; }

    [GraphQLName("free")] public bool Free { get; set; }

    [GraphQLName("id")] [ID] public string Id { get; set; }
}

public enum OrganizationOrderField
{
    name
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
    [GraphQLName("terms")] public string Terms { get; set; }

    [GraphQLName("id")] [ID] public string Id { get; set; }
}

[GraphQLName("OrganizationWhereInput")]
public class OrganizationWhereInput
{
    [GraphQLName("nameContains")] public string? NameContains { get; set; }
}

[GraphQLName("PageInfo")]
public class PageInfo
{
    [GraphQLName("hasNextPage")] public bool HasNextPage { get; set; }

    [GraphQLName("hasPreviousPage")] public bool HasPreviousPage { get; set; }

    [GraphQLName("startCursor")] public string? StartCursor { get; set; }

    [GraphQLName("endCursor")] public string? EndCursor { get; set; }
}

[GraphQLName("Query_Organization_Arguments")]
public class Query_Organization_Arguments
{
    [GraphQLName("id")] public string Id { get; set; }
}

[GraphQLName("Query_OrganizationAnalytics_Arguments")]
public class Query_OrganizationAnalytics_Arguments
{
    [GraphQLName("organizationId")] public string OrganizationId { get; set; }

    [GraphQLName("from")] public DateTimeOffset From { get; set; }

    [GraphQLName("until")] public DateTimeOffset Until { get; set; }
}

[GraphQLName("Query_OrganizationMembers_Arguments")]
public class Query_OrganizationMembers_Arguments
{
    [GraphQLName("where")] public OrganizationMemberWhereInput Where { get; set; }

    [GraphQLName("orderBy")] public OrganizationMemberOrderInput[]? OrderBy { get; set; }
}

[GraphQLName("Query_Organizations_Arguments")]
public class Query_Organizations_Arguments
{
    [GraphQLName("after")] public string? After { get; set; }

    [GraphQLName("first")] public int? First { get; set; }

    [GraphQLName("before")] public string? Before { get; set; }

    [GraphQLName("last")] public int? Last { get; set; }

    [GraphQLName("where")] public OrganizationWhereInput Where { get; set; }

    [GraphQLName("orderBy")] public OrganizationOrderInput[]? OrderBy { get; set; }
}

[GraphQLName("Query_PaginatedOrganizationMembers_Arguments")]
public class Query_PaginatedOrganizationMembers_Arguments
{
    [GraphQLName("after")] public string? After { get; set; }

    [GraphQLName("first")] public int? First { get; set; }

    [GraphQLName("before")] public string? Before { get; set; }

    [GraphQLName("last")] public int? Last { get; set; }

    [GraphQLName("where")] public OrganizationMemberWhereInput Where { get; set; }

    [GraphQLName("orderBy")] public OrganizationMemberOrderInput[]? OrderBy { get; set; }
}

[GraphQLName("RejectInvitationToJoinOrganizationInput")]
public class RejectInvitationToJoinOrganizationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("id")] public string Id { get; set; }
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

    [GraphQLName("id")] public string Id { get; set; }

    [GraphQLName("name")] public string Name { get; set; }

    [GraphQLName("about")] public string? About { get; set; }

    [GraphQLName("website")] public string? Website { get; set; }

    [GraphQLName("industrySubCategoryIds")]
    public string[] IndustrySubCategoryIds { get; set; }
}

[GraphQLName("UpdateOrganizationOfferingInput")]
public class UpdateOrganizationOfferingInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("id")] public string Id { get; set; }

    [GraphQLName("offeringCode")] public string OfferingCode { get; set; }
}

[GraphQLName("UpdateOrganizationOfferingPayload")]
public class UpdateOrganizationOfferingPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}

[GraphQLName("Version")]
public class Version
{
    [GraphQLName("major")] public int Major { get; set; }

    [GraphQLName("minor")] public int Minor { get; set; }

    [GraphQLName("build")] public int Build { get; set; }

    [GraphQLName("revision")] public int Revision { get; set; }
}
