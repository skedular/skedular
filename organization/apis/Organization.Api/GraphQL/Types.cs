using Api.Shared.Services.Models;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types.Pagination;
using HotChocolate.Types.Relay;
using Organization.Shared.Models;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
// ReSharper disable ClassNeverInstantiated.Global

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
    [GraphQLName("type")] public OrganizationType Type { get; set; }
    [GraphQLName("contactEmail")] public string? ContactEmail { get; set; }
    [GraphQLName("contactPhone")] public string? ContactPhone { get; set; }

    [GraphQLName("memberVisibilityPolicy")]
    public OrganizationMemberVisibilityPolicy MemberVisibilityPolicy { get; set; }

    [GraphQLName("agreedToTermsOfUse")] public bool AgreedToTermsOfUse { get; set; }
    [GraphQLName("termsOfUseId")] public string TermsOfUseId { get; set; } = string.Empty;

    [GraphQLName("industrySubCategoryIds")]
    public IEnumerable<string> IndustrySubCategoryIds { get; set; } = [];

    [GraphQLName("physicalAddress")] public AddressDetailsInput PhysicalAddress { get; set; }
}

[GraphQLName("UpdateOrganizationInput")]
public class UpdateOrganizationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public required string Id { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("about")] public string? About { get; set; }
    [GraphQLName("website")] public string? Website { get; set; }
    [GraphQLName("type")] public OrganizationType Type { get; set; }
    [GraphQLName("contactEmail")] public string? ContactEmail { get; set; }
    [GraphQLName("contactPhone")] public string? ContactPhone { get; set; }

    [GraphQLName("memberVisibilityPolicy")]
    public OrganizationMemberVisibilityPolicy MemberVisibilityPolicy { get; set; }

    [GraphQLName("industrySubCategoryIds")]
    public IEnumerable<string> IndustrySubCategoryIds { get; set; } = [];

    [GraphQLName("physicalAddress")] public AddressDetailsInput PhysicalAddress { get; set; }
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

[GraphQLName("ChangeOrganizationMemberRoleInput")]
public class ChangeOrganizationMemberRoleInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public required string Id { get; set; }
    [GraphQLName("role")] public OrganizationMemberRole Role { get; set; }
}

[GraphQLName("ChangeOrganizationMembersStatusInput")]
public class ChangeOrganizationMembersStatusInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("ids")] public required IEnumerable<string> Ids { get; set; }
    [GraphQLName("status")] public OrganizationMemberStatus Status { get; set; }
}

[GraphQLName("RemoveOrganizationMembersInput")]
public class RemoveOrganizationMembersInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("ids")] public required IEnumerable<string> Ids { get; set; }
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
    [GraphQLName("emails")] public IEnumerable<string> Emails { get; set; } = [];
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
    public IEnumerable<OrganizationMemberAttendancePercentage> MemberAttendancePercentage { get; set; } = [];

    [GraphQLName("dailyBookingsTotals")] public IEnumerable<OrganizationDailyBookingsTotal> DailyBookingsTotals { get; set; } = [];
}

[GraphQLName("OrganizationConnection")]
public class OrganizationConnection : Enterprise.Shared.GraphQL.Types.Connection<OrganizationEdge>;

[GraphQLName("OrganizationCustomerDetails")]
public class OrganizationCustomerDetails
{
    [GraphQLName("uniqueId")] [ID] public required string UniqueId { get; set; }
    [GraphQLName("email")] public string? Email { get; set; }
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
    [GraphQLName("phoneNumber")] public string? PhoneNumber { get; set; }
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
    [GraphQLName("type")] public OrganizationTypeDetails Type { get; set; }

    [GraphQLName("memberVisibilityPolicy")]
    public OrganizationMemberVisibilityPolicyDetails MemberVisibilityPolicy { get; set; }

    [GraphQLName("agreedToTermsOfUse")] public bool AgreedToTermsOfUse { get; set; }
    [GraphQLName("termsOfUse")] public OrganizationTermsOfUse? TermsOfUse { get; set; }

    [GraphQLName("industrySubCategories")]
    public IEnumerable<OrganizationIndustrySubCategoryReferenceDetails> IndustrySubCategories { get; set; } = [];

    [GraphQLName("contactEmail")] public string? ContactEmail { get; set; }
    [GraphQLName("contactPhone")] public string? ContactPhone { get; set; }

    [GraphQLName("physicalAddress")] public AddressDetails PhysicalAddress { get; set; }

    [GraphQLName("availableOfferings")] public IEnumerable<OrganizationOfferingDetails> AvailableOfferings { get; set; } = [];
    [GraphQLName("activeOffering")] public OrganizationActiveOfferingDetails ActiveOffering { get; set; }
    [GraphQLName("hasFutureBooking")] public bool HasFutureBooking { get; set; }
    [GraphQLName("hasLocation")] public bool HasLocation { get; set; }
    [GraphQLName("hasTeam")] public bool HasTeam { get; set; }
    [GraphQLName("canModify")] public bool CanModify { get; set; }
    [GraphQLName("canDelete")] public bool CanDelete { get; set; }
    [GraphQLName("canInvitePeople")] public bool CanInvitePeople { get; set; }
    [GraphQLName("canViewAnalytics")] public bool CanViewAnalytics { get; set; }
    [GraphQLName("isMyOnboardingDone")] public bool IsMyOnboardingDone { get; set; }
    [GraphQLName("members")] public IEnumerable<OrganizationMemberDetails> Members { get; set; } = [];
    [GraphQLName("resourceTypes")] public IEnumerable<OrganizationTagDetails> ResourceTypes { get; set; } = [];

    [GraphQLName("hasAttachedPaymentMethod")]
    public bool HasAttachedPaymentMethod { get; set; }

    [GraphQLName("ssoSettings")] public OrganizationSsoSettingsDetails? SsoSettings { get; set; }

    [GraphQLName("id")] [ID] public required string Id { get; set; }
}

[GraphQLName("OrganizationEdge")]
public class OrganizationEdge(OrganizationDetails node, string cursor) : Edge<OrganizationDetails>(node, cursor);

[GraphQLName("OrganizationIndustryMainCategoryReferenceDetails")]
public class OrganizationIndustryMainCategoryReferenceDetails : Node
{
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("subCategories")] public IEnumerable<OrganizationIndustrySubCategoryReferenceDetails> SubCategories { get; set; } = [];
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
public class OrganizationMemberConnection : Enterprise.Shared.GraphQL.Types.Connection<OrganizationMemberEdge>;

[GraphQLName("OrganizationMemberDetails")]
public class OrganizationMemberDetails : Node
{
    [GraphQLName("role")] public OrganizationMemberRole? Role { get; set; }
    [GraphQLName("status")] public OrganizationMemberStatus Status { get; set; }

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

[GraphQLName("OrganizationMembersDetailsPayload")]
public class OrganizationMembersDetailsPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("members")] public IEnumerable<OrganizationMemberDetails> Members { get; set; } = [];
}

[GraphQLName("OrganizationMemberEdge")]
public class OrganizationMemberEdge(OrganizationMemberDetails node, string cursor) : Edge<OrganizationMemberDetails>(node, cursor);

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
    [GraphQLName("customerId")] public string? CustomerId { get; set; }
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

[GraphQLName("AddCustomTagInput")]
public class AddCustomTagInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string? Id { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("description")] public string? Description { get; set; }
    [GraphQLName("color")] public string? Color { get; set; }
    [GraphQLName("organizationId")] public required string OrganizationId { get; set; }
}

[GraphQLName("UpdateCustomTagInput")]
public class UpdateCustomTagInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public required string Id { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("description")] public string? Description { get; set; }
    [GraphQLName("color")] public string? Color { get; set; }
}

[GraphQLName("DeleteCustomTagInput")]
public class DeleteCustomTagInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public required string Id { get; set; }
}

[GraphQLName("DeleteCustomTagsInput")]
public class DeleteCustomTagsInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("ids")] public required IEnumerable<string> Ids { get; set; } = [];
}

[GraphQLName("OrganizationTagConnection")]
public class OrganizationTagConnection : Enterprise.Shared.GraphQL.Types.Connection<OrganizationTagEdge>;

[GraphQLName("OrganizationTagDetails")]
public class OrganizationTagDetails : Node
{
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("description")] public string? Description { get; set; }
    [GraphQLName("tagType")] public string TagType { get; set; } = string.Empty;
    [GraphQLName("color")] public string? Color { get; set; }
    [GraphQLName("id")] [ID] public required string Id { get; set; }
}

[GraphQLName("OrganizationTagEdge")]
public class OrganizationTagEdge(OrganizationTagDetails node, string cursor) : Edge<OrganizationTagDetails>(node, cursor);

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

[GraphQLName("CustomTagOrganizationTagWhereInput")]
public class CustomTagOrganizationTagWhereInput
{
    [GraphQLName("organizationId")] public required string OrganizationId { get; set; }
    [GraphQLName("nameContains")] public string? NameContains { get; set; }
}

[GraphQLName("OrganizationTagsPayload")]
public class OrganizationTagsPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("organizationTags")] public IEnumerable<OrganizationTagDetails> OrganizationTags { get; set; } = [];
}

[GraphQLName("OrganizationOfferingDetails")]
public class OrganizationOfferingDetails
{
    [GraphQLName("code")] public string Code { get; set; } = string.Empty;
    [GraphQLName("isEnterprise")] public bool IsEnterprise { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("unitPrice")] public int UnitPrice { get; set; }
    [GraphQLName("underPriceLines")] public IEnumerable<string> UnderPriceLines { get; set; } = [];
    [GraphQLName("featureSet")] public IEnumerable<string> FeatureSet { get; set; } = [];
    [GraphQLName("free")] public bool Free { get; set; }
    [GraphQLName("earlyBird")] public bool EarlyBird { get; set; }
}

[GraphQLName("OrganizationActiveOfferingDetails")]
public class OrganizationActiveOfferingDetails : Node
{
    [GraphQLName("code")] public string Code { get; set; } = string.Empty;
    [GraphQLName("isEnterprise")] public bool IsEnterprise { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("start")] public DateTimeOffset Start { get; set; }
    [GraphQLName("end")] public DateTimeOffset End { get; set; }
    [GraphQLName("unitPrice")] public int UnitPrice { get; set; }
    [GraphQLName("underPriceLines")] public IEnumerable<string> UnderPriceLines { get; set; } = [];
    [GraphQLName("featureSet")] public IEnumerable<string> FeatureSet { get; set; } = [];
    [GraphQLName("free")] public bool Free { get; set; }
    [GraphQLName("earlyBird")] public bool EarlyBird { get; set; }
    [GraphQLName("id")] [ID] public string Id { get; set; } = string.Empty;
}

[GraphQLName("UpdateOrganizationSsoSettingsInput")]
public class UpdateOrganizationSsoSettingsInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("organizationId")] public required string OrganizationId { get; set; }
    [GraphQLName("entityId")] public required string EntityId { get; set; }
    [GraphQLName("loginUrl")] public string LoginUrl { get; set; }

    [GraphQLName("appFederationMetadataUrl")]
    public required string AppFederationMetadataUrl { get; set; }
}

[GraphQLName("UpdateOrganizationSsoSettingsPayload")]
public class UpdateOrganizationSsoSettingsPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("organization")] public OrganizationDetails Organization { get; set; }
}

[GraphQLName("RemoveOrganizationSsoSettingsInput")]
public class RemoveOrganizationSsoSettingsInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("organizationId")] public required string OrganizationId { get; set; }
}

[GraphQLName("RemoveOrganizationSsoSettingsPayload")]
public class RemoveOrganizationSsoSettingsPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("organization")] public OrganizationDetails Organization { get; set; }
}

[GraphQLName("OrganizationSsoSettingsDetails")]
public class OrganizationSsoSettingsDetails
{
    [GraphQLName("entityId")] public required string EntityId { get; set; }
    [GraphQLName("loginUrl")] public string LoginUrl { get; set; }

    [GraphQLName("appFederationMetadataUrl")]
    public required string AppFederationMetadataUrl { get; set; }
}

[GraphQLName("OrganizationTypeDetails")]
public class OrganizationTypeDetails
{
    [GraphQLName("type")] public OrganizationType Type { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
}

[GraphQLName("OrganizationMemberVisibilityPolicyDetails")]
public class OrganizationMemberVisibilityPolicyDetails
{
    [GraphQLName("type")] public OrganizationMemberVisibilityPolicy Type { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
}

[GraphQLName("AddZoneInput")]
public class AddZoneInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string? Id { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("description")] public string? Description { get; set; }
    [GraphQLName("organizationId")] public required string OrganizationId { get; set; }
    [GraphQLName("color")] public string? Color { get; set; }
}

[GraphQLName("UpdateZoneInput")]
public class UpdateZoneInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public required string Id { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("description")] public string? Description { get; set; }
    [GraphQLName("color")] public string? Color { get; set; }
}

[GraphQLName("DeleteZoneInput")]
public class DeleteZoneInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public required string Id { get; set; }
}

[GraphQLName("DeleteZonesInput")]
public class DeleteZonesInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("ids")] public required IEnumerable<string> Ids { get; set; } = [];
}

[GraphQLName("ZoneOrganizationTagWhereInput")]
public class ZoneOrganizationTagWhereInput
{
    [GraphQLName("organizationId")] public required string OrganizationId { get; set; }
    [GraphQLName("nameContains")] public string? NameContains { get; set; }
}

[GraphQLName("AddProductTagInput")]
public class AddProductTagInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string? Id { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("description")] public string? Description { get; set; }
    [GraphQLName("organizationId")] public required string OrganizationId { get; set; }
    [GraphQLName("color")] public string? Color { get; set; }
}

[GraphQLName("UpdateProductTagInput")]
public class UpdateProductTagInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public required string Id { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("description")] public string? Description { get; set; }
    [GraphQLName("color")] public string? Color { get; set; }
}

[GraphQLName("DeleteProductTagInput")]
public class DeleteProductTagInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public required string Id { get; set; }
}

[GraphQLName("DeleteProductTagsInput")]
public class DeleteProductTagsInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("ids")] public required IEnumerable<string> Ids { get; set; } = [];
}

[GraphQLName("ProductTagOrganizationTagWhereInput")]
public class ProductTagOrganizationTagWhereInput
{
    [GraphQLName("organizationId")] public required string OrganizationId { get; set; }
    [GraphQLName("nameContains")] public string? NameContains { get; set; }
}

[GraphQLName("AddLocationTagInput")]
public class AddLocationTagInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string? Id { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("description")] public string? Description { get; set; }
    [GraphQLName("organizationId")] public required string OrganizationId { get; set; }
    [GraphQLName("color")] public string? Color { get; set; }
}

[GraphQLName("UpdateLocationTagInput")]
public class UpdateLocationTagInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public required string Id { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("description")] public string? Description { get; set; }
    [GraphQLName("color")] public string? Color { get; set; }
}

[GraphQLName("DeleteLocationTagInput")]
public class DeleteLocationTagInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public required string Id { get; set; }
}

[GraphQLName("DeleteLocationTagsInput")]
public class DeleteLocationTagsInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("ids")] public required IEnumerable<string> Ids { get; set; } = [];
}

[GraphQLName("LocationTagOrganizationTagWhereInput")]
public class LocationTagOrganizationTagWhereInput
{
    [GraphQLName("organizationId")] public required string OrganizationId { get; set; }
    [GraphQLName("nameContains")] public string? NameContains { get; set; }
}

[GraphQLName("OrganizationAddressDetails")]
public class AddressDetails
{
    [GraphQLName("formattedAddress")] public string? FormattedAddress { get; set; }
    [GraphQLName("addressLine1")] public required string AddressLine1 { get; set; }
    [GraphQLName("addressLine2")] public string? AddressLine2 { get; set; }
    [GraphQLName("suburb")] public required string Suburb { get; set; }
    [GraphQLName("city")] public required string City { get; set; }
    [GraphQLName("province")] public string? Province { get; set; }
    [GraphQLName("zipcode")] public required string Zipcode { get; set; }
    [GraphQLName("country")] public required string Country { get; set; }
}

[GraphQLName("OrganizationAddressDetailsInput")]
public class AddressDetailsInput
{
    [GraphQLName("addressLine1")] public required string AddressLine1 { get; set; }
    [GraphQLName("addressLine2")] public string? AddressLine2 { get; set; }
    [GraphQLName("suburb")] public required string Suburb { get; set; }
    [GraphQLName("city")] public required string City { get; set; }
    [GraphQLName("province")] public string? Province { get; set; }
    [GraphQLName("zipcode")] public required string Zipcode { get; set; }
    [GraphQLName("country")] public required string Country { get; set; }
}
