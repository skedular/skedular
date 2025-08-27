using Api.Shared.Services.Grpc.Skedular.Organization.V1;
using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Enterprise.Shared;
using Google.Protobuf.WellKnownTypes;
using HotChocolate.Types.Pagination;
using NetTopologySuite.Geometries;
using Organization.Api.GraphQL.Analytics;
using Organization.Api.GraphQL.BankAccount;
using Organization.Api.GraphQL.Invitation;
using Organization.Api.GraphQL.Member;
using Organization.Api.GraphQL.Offering;
using Organization.Api.GraphQL.Organization;
using Organization.Api.GraphQL.PhysicalAddress;
using Organization.Api.GraphQL.Sso;
using Organization.Api.GraphQL.Stripe;
using Organization.Api.GraphQL.Tag;
using Organization.Api.GraphQL.TaxDetails;
using Organization.Shared.Models;
using Stripe;
using AddCustomTagInput = Organization.Api.GraphQL.Tag.AddCustomTagInput;
using AddOrganizationBillingDetailsInput = Organization.Api.GraphQL.Billing.AddOrganizationBillingDetailsInput;
using AddZoneInput = Api.Shared.Services.Grpc.Skedular.Organization.V1.AddZoneInput;
using AzureTenant = Organization.Shared.Models.AzureTenant;
using AzureTenantMember = Organization.Shared.Models.AzureTenantMember;
using BankAccount = Api.Shared.Services.Grpc.Skedular.Organization.V1.BankAccount;
using Coordinates = Api.Shared.Services.Grpc.Skedular.Organization.V1.Coordinates;
using Customer = Organization.Shared.Models.Customer;
using DailyMemberCountRecording = Organization.Shared.Models.DailyMemberCountRecording;
using Identity = Organization.Shared.Models.Identity;
using IndustryMainCategory = Organization.Shared.Models.IndustryMainCategory;
using IndustrySubCategory = Organization.Shared.Models.IndustrySubCategory;
using JoinInvitation = Organization.Shared.Models.JoinInvitation;
using Location = Organization.Shared.Models.Location;
using Offering = Api.Shared.Services.Offering.Offering;
using OrganizationDailyBookingsTotal = Organization.Shared.Models.OrganizationDailyBookingsTotal;
using OrganizationMember = Organization.Shared.Models.OrganizationMember;
using OrganizationMemberAttendancePercentage = Organization.Shared.Models.OrganizationMemberAttendancePercentage;
using OrganizationMemberStatus = Api.Shared.Services.Models.OrganizationMemberStatus;
using OrganizationOffering = Organization.Shared.Models.OrganizationOffering;
using Tag = Organization.Shared.Models.Tag;
using Team = Organization.Shared.Models.Team;
using TermsOfUse = Organization.Shared.Database.Entities.TermsOfUse;
using UpdateCustomTagInput = Organization.Api.GraphQL.Tag.UpdateCustomTagInput;
using UpdateZoneInput = Api.Shared.Services.Grpc.Skedular.Organization.V1.UpdateZoneInput;
using Member = Api.Shared.Services.Grpc.Skedular.Organization.V1.OrganizationMember;
using OrganizationBankAccount = Organization.Shared.Database.Entities.OrganizationBankAccount;
using OrganizationBillingDetails = Organization.Shared.Database.Entities.OrganizationBillingDetails;
using OrganizationDetails = Organization.Api.GraphQL.Organization.OrganizationDetails;
using OrganizationSsoSettings = Organization.Shared.Models.OrganizationSsoSettings;
using OrganizationTaxDetails = Organization.Shared.Models.OrganizationTaxDetails;
using OrganizationStripeConnectAccount = Organization.Shared.Database.Entities.OrganizationStripeConnectAccount;
using OrganizationStripeConnectAccountAuthorization = Organization.Shared.Models.OrganizationStripeConnectAccountAuthorization;
using OrganizationStripeCustomer = Organization.Shared.Models.OrganizationStripeCustomer;
using OrganizationStripePaymentMethod = Organization.Shared.Models.OrganizationStripePaymentMethod;
using UpdateOrganizationBillingDetailsInput = Organization.Api.GraphQL.Billing.UpdateOrganizationBillingDetailsInput;
using OrganizationPhysicalAddress = Organization.Shared.Database.Entities.OrganizationPhysicalAddress;

namespace Organization.Api.Mappers;

public interface IMapper
{
    Shared.Models.Organization MapTo(Shared.Database.Entities.Organization src, Uri stripeAuthorizeExistingConnectAccountUrl);
    OrganizationMember MapTo(Shared.Database.Entities.OrganizationMember src, Shared.Models.Organization organization);
    JoinInvitation MapTo(Shared.Database.Entities.JoinInvitation src);

    Shared.Database.Entities.Organization MapTo(
        Shared.Models.Organization src,
        TermsOfUse termsOfUse,
        ICollection<Shared.Database.Entities.IndustrySubCategory> industrySubCategories);

    Shared.Database.Entities.Organization MergeTo(
        Shared.Models.Organization src,
        Shared.Database.Entities.Organization dest,
        ICollection<Shared.Database.Entities.IndustrySubCategory> industrySubCategories);

    Shared.Models.TermsOfUse? MapTo(TermsOfUse? src);
    Customer? MapTo(Shared.Database.Entities.Customer? src);
    IEnumerable<IndustryMainCategory> MapTo(IEnumerable<Shared.Database.Entities.IndustryMainCategory> src);
    OrganizationTermsOfUse? MapTo(Shared.Models.TermsOfUse? src);
    IEnumerable<OrganizationIndustryMainCategoryReferenceDetails> MapTo(IEnumerable<IndustryMainCategory> src);
    IEnumerable<OrganizationDetails> MapTo(IEnumerable<Shared.Models.Organization> src);
    OrganizationDetails? MapTo(Shared.Models.Organization? src);
    OrganizationMemberDetails MapTo(OrganizationMember src);

    OrganizationAnalytics MapTo(
        IEnumerable<OrganizationMemberAttendancePercentage> organizationMemberAttendancePercentages,
        IEnumerable<OrganizationDailyBookingsTotal> organizationDailyBookingsTotals);

    Shared.Models.Organization MapTo(AddOrganizationInput src);
    Shared.Models.Organization MapTo(UpdateOrganizationInput src);
    global::Api.Shared.Services.Grpc.Skedular.Organization.V1.TermsOfUse MapToGrpcResponse(Shared.Models.TermsOfUse src);
    Shared.Models.Organization MapTo(Admin_AddInput src);
    global::Api.Shared.Services.Grpc.Skedular.Organization.V1.Organization MapToGrpcResponse(Shared.Models.Organization src);

    Shared.Database.Entities.OrganizationMember MapToEntity(
        OrganizationMember src,
        Shared.Database.Entities.Organization organization,
        Shared.Database.Entities.Customer customer);

    Shared.Database.Entities.OrganizationMember MergeToEntity(
        OrganizationMember src,
        Shared.Database.Entities.OrganizationMember dest,
        Shared.Database.Entities.Organization organization,
        Shared.Database.Entities.Customer customer);

    OrganizationMember MapTo(Admin_AddMemberInput src);
    OrganizationEdge MapTo(Edge<Shared.Models.Organization> src);

    IEnumerable<Edge<OrganizationMember>> MapTo(
        IEnumerable<Edge<Shared.Database.Entities.OrganizationMember>> src,
        Shared.Models.Organization organization);

    OrganizationMemberEdge MapTo(Edge<OrganizationMember> src);
    MemberEdge MapToGrpcResponse(Edge<OrganizationMember> src);
    Tag MapTo(Shared.Database.Entities.Tag src);
    Shared.Database.Entities.Tag MapTo(Tag src, Shared.Database.Entities.Organization organization);
    Shared.Database.Entities.Tag MergeTo(Tag src, Shared.Database.Entities.Tag dest, Shared.Database.Entities.Organization organization);
    IEnumerable<Edge<Tag>> MapTo(IEnumerable<Edge<Shared.Database.Entities.Tag>> src, Shared.Models.Organization organization);
    Tag MapTo(AddCustomTagInput src);
    Tag MapTo(UpdateCustomTagInput src);
    Tag MapTo(GraphQL.Tag.AddZoneInput src);
    Tag MapTo(GraphQL.Tag.UpdateZoneInput src);
    OrganizationTagDetails? MapTo(Tag? src);
    OrganizationTagEdge MapTo(Edge<Tag> src);

    CustomTag MapToGrpcResponseCustomTag(Tag? src);
    CustomTagEdge MapToGrpcResponseCustomTag(Edge<Tag> src);
    Tag MapTo(global::Api.Shared.Services.Grpc.Skedular.Organization.V1.AddCustomTagInput src);
    Tag MapTo(global::Api.Shared.Services.Grpc.Skedular.Organization.V1.UpdateCustomTagInput src);

    Zone MapToGrpcResponseZone(Tag? src);
    ZoneEdge MapToGrpcResponseZone(Edge<Tag> src);
    Tag MapTo(AddZoneInput src);
    Tag MapTo(UpdateZoneInput src);

    IEnumerable<string> MapTo(Offering offering);
    OrganizationSsoSettings MapTo(UpdateOrganizationSsoSettingsInput src);
    Shared.Database.Entities.OrganizationSsoSettings MapToEntity(OrganizationSsoSettings src, Shared.Database.Entities.Organization organization);

    Shared.Database.Entities.OrganizationSsoSettings MergeToEntity(
        OrganizationSsoSettings src,
        Shared.Database.Entities.OrganizationSsoSettings dest,
        Shared.Database.Entities.Organization organization);

    Tag MapTo(AddProductTagInput src);
    Tag MapTo(UpdateProductTagInput src);
    Tag MapTo(AddLocationTagInput src);
    Tag MapTo(UpdateLocationTagInput src);
    OrganizationBillingDetails MapTo(Shared.Models.OrganizationBillingDetails src, Shared.Database.Entities.Organization organization);

    OrganizationBillingDetails MergeToEntity(
        Shared.Models.OrganizationBillingDetails src,
        OrganizationBillingDetails dest,
        Shared.Database.Entities.Organization organization);

    CustomerCreateOptions MapToStripeCustomerCreateOption(Shared.Database.Entities.Organization src);
    Shared.Models.OrganizationBillingDetails MapTo(AddOrganizationBillingDetailsInput src);
    Shared.Models.OrganizationBillingDetails MapTo(UpdateOrganizationBillingDetailsInput src);
    Shared.Models.OrganizationBillingDetails MapTo(AddBillingDetailsInput src);
    Shared.Models.OrganizationBillingDetails MapTo(UpdateBillingDetailsInput src);
    BillingDetails MapToGrpcResponse(Shared.Models.OrganizationBillingDetails? src);
    Shared.Models.OrganizationBillingDetails? MapTo(OrganizationBillingDetails? src);
    AccountCreateOptions MapToStripeAccountRequest(Shared.Database.Entities.Organization src);
    OrganizationStripeConnectAccount MapTo(Account src, string id, string name, bool isDefault, Shared.Database.Entities.Organization organization);
    Shared.Models.OrganizationStripeConnectAccount MapTo(OrganizationStripeConnectAccount src);
    OrganizationStripeConnectAccountDetails? MapTo(Shared.Models.OrganizationStripeConnectAccount? src);
    OrganizationStripeConnectAccountEdge MapTo(Edge<Shared.Models.OrganizationStripeConnectAccount> src);
    StripeConnectAccountEdge MapToGrpcResponse(Edge<Shared.Models.OrganizationStripeConnectAccount> src);
    OrganizationBankAccount MapTo(Shared.Models.OrganizationBankAccount src, Shared.Database.Entities.Organization organization);

    OrganizationBankAccount MergeTo(
        Shared.Models.OrganizationBankAccount src,
        OrganizationBankAccount dest,
        Shared.Database.Entities.Organization organization);

    Shared.Models.OrganizationBankAccount MapTo(OrganizationBankAccount src);
    Shared.Models.OrganizationBankAccount MapTo(AddOrganizationBankAccountInput src);
    Shared.Models.OrganizationBankAccount MapTo(UpdateOrganizationBankAccountInput src);
    OrganizationBankAccountDetails? MapTo(Shared.Models.OrganizationBankAccount? src);
    OrganizationBankAccountEdge MapTo(Edge<Shared.Models.OrganizationBankAccount> src);
    BankAccountEdge MapToGrpcResponse(Edge<Shared.Models.OrganizationBankAccount> src);

    OrganizationTaxDetails MapTo(UpdateOrganizationTaxDetailsInput src);
    Shared.Database.Entities.OrganizationTaxDetails MapToEntity(OrganizationTaxDetails src, Shared.Database.Entities.Organization organization);

    Shared.Database.Entities.OrganizationTaxDetails MergeToEntity(
        OrganizationTaxDetails src,
        Shared.Database.Entities.OrganizationTaxDetails dest,
        Shared.Database.Entities.Organization organization);

    OrganizationPhysicalAddress MapTo(Shared.Models.OrganizationPhysicalAddress src, Shared.Database.Entities.Organization organization);

    OrganizationPhysicalAddress MergeTo(
        Shared.Models.OrganizationPhysicalAddress src,
        OrganizationPhysicalAddress dest,
        Shared.Database.Entities.Organization organization);

    Shared.Models.OrganizationPhysicalAddress MapTo(OrganizationPhysicalAddress src);
    Shared.Models.OrganizationPhysicalAddress MapTo(AddOrganizationPhysicalAddressInput src);
    Shared.Models.OrganizationPhysicalAddress MapTo(UpdateOrganizationPhysicalAddressInput src);
    OrganizationPhysicalAddressDetails MapTo(Shared.Models.OrganizationPhysicalAddress src);
    IEnumerable<InviteCustomerToJoinOrganizationDetails> MapTo(IEnumerable<JoinInvitation> src);
    InviteCustomerToJoinOrganizationDetails MapTo(JoinInvitation src);
    Edge<JoinInvitation> MapTo(Edge<Shared.Database.Entities.JoinInvitation> src);
    OrganizationJoinInvitationEdge MapTo(Edge<JoinInvitation> src);
    OrganizationStripeConnectAccount MergeTo(Account src, OrganizationStripeConnectAccount dest);
}

public class Mapper : IMapper
{
    public Shared.Models.Organization MapTo(Shared.Database.Entities.Organization src, Uri stripeAuthorizeExistingConnectAccountUrl)
    {
        var organization = new Shared.Models.Organization
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            UniqueAlphanumericName = src.UniqueAlphanumericName,
            Name = src.Name,
            About = src.About,
            Website = src.Website,
            AgreedToTermsOfUse = src.AgreedToTermsOfUse,
            LogoUrl = src.LogoUrl,
            Type = src.Type.ToOrganizationType(),
            ContactEmail = src.ContactEmail,
            ContactPhone = src.ContactPhone,
            StripeAuthorizeExistingConnectAccountUrl = stripeAuthorizeExistingConnectAccountUrl,
            MemberVisibilityPolicy = src.MemberVisibilityPolicy.ToOrganizationMemberVisibilityPolicy(),
            PaymentMethodEventRaisedAt = src.PaymentMethodEventRaisedAt,
            TermsOfUse = MapTo(src.TermsOfUse),
            IndustrySubCategories = MapTo(src.IndustrySubCategories, null).ToList(),
            OrganizationSsoSettings = MapTo(src.OrganizationSsoSettings),
            OrganizationTaxDetails = MapTo(src.OrganizationTaxDetails)
        };

        organization.OrganizationMembers = MapTo(src.OrganizationMembers, organization).ToList();
        organization.OrganizationOfferings = MapTo(src.OrganizationOfferings, organization).ToList();
        organization.DailyMemberCountRecordings = MapTo(src.DailyMemberCountRecordings, organization).ToList();
        organization.Locations = MapTo(src.Locations, organization).ToList();
        organization.Teams = MapTo(src.Teams, organization).ToList();
        organization.JoinInvitations = MapTo(src.JoinInvitations, organization).ToList();
        organization.AzureTenants = MapTo(src.AzureTenants, organization).ToList();
        organization.Tags = MapTo(src.Tags, organization).ToList();
        organization.PhysicalAddress = MapTo(src.PhysicalAddress, organization);
        organization.BillingDetails = MapTo(src.BillingDetails, organization);
        organization.OrganizationStripePaymentMethods = MapTo(src.OrganizationStripePaymentMethods, organization).ToList();
        organization.OrganizationStripeCustomer = MapTo(src.OrganizationStripeCustomer, organization);
        organization.OrganizationStripeConnectAccounts = MapTo(src.OrganizationStripeConnectAccounts, organization).ToList();

        return organization;
    }

    public OrganizationMember MapTo(Shared.Database.Entities.OrganizationMember src, Shared.Models.Organization organization) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Role = src.Role.ToOrganizationMemberRole(),
            Status = src.Status.ToOrganizationMemberStatus(),
            IsOrganizationOnboardingDone = src.IsOrganizationOnboardingDone,
            Customer = MapTo(src.Customer)!,
            Organization = organization
        };

    public JoinInvitation MapTo(Shared.Database.Entities.JoinInvitation src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Email = src.Email,
            Status = src.Status.ToInvitationStatus(),
            Role = src.Role.ToOrganizationMemberRole(),
            Organization = MapTo(src.Organization, Constants.EmptyUri),
            CreatedBy = MapTo(src.CreatedBy)!,
            Invitee = MapTo(src.Invitee)
        };

    public Shared.Database.Entities.Organization MapTo(
        Shared.Models.Organization src,
        TermsOfUse termsOfUse,
        ICollection<Shared.Database.Entities.IndustrySubCategory> industrySubCategories) =>
        new()
        {
            Id = src.Id,
            UniqueAlphanumericName = src.UniqueAlphanumericName,
            Name = src.Name,
            About = src.About,
            Website = src.Website,
            AgreedToTermsOfUse = src.AgreedToTermsOfUse,
            LogoUrl = src.LogoUrl,
            Type = src.Type.ToOrganizationType(),
            ContactEmail = src.ContactEmail,
            ContactPhone = src.ContactPhone,
            MemberVisibilityPolicy = src.MemberVisibilityPolicy.ToOrganizationMemberVisibilityPolicy(),
            TermsOfUse = termsOfUse,
            IndustrySubCategories = industrySubCategories
        };

    public Shared.Database.Entities.Organization MergeTo(
        Shared.Models.Organization src,
        Shared.Database.Entities.Organization dest,
        ICollection<Shared.Database.Entities.IndustrySubCategory> industrySubCategories)
    {
        dest.Id = src.Id;
        dest.UniqueAlphanumericName = dest.UniqueAlphanumericName;
        dest.Name = src.Name;
        dest.About = src.About;
        dest.Website = src.Website;
        dest.AgreedToTermsOfUse = src.AgreedToTermsOfUse;
        dest.LogoUrl = src.LogoUrl;
        dest.Type = src.Type.ToOrganizationType();
        dest.ContactEmail = src.ContactEmail;
        dest.ContactPhone = src.ContactPhone;
        dest.MemberVisibilityPolicy = src.MemberVisibilityPolicy.ToOrganizationMemberVisibilityPolicy();
        dest.IndustrySubCategories = industrySubCategories;
        return dest;
    }

    public Customer? MapTo(Shared.Database.Entities.Customer? src) =>
        src is null
            ? null
            : new Customer
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                EventRaisedAt = src.EventRaisedAt,
                Name = src.Name,
                GivenName = src.GivenName,
                MiddleName = src.MiddleName,
                FamilyName = src.FamilyName,
                PhotoUrl = src.PhotoUrl,
                PhotoUrl24 = src.PhotoUrl24,
                PhotoUrl32 = src.PhotoUrl32,
                PhotoUrl48 = src.PhotoUrl48,
                PhotoUrl72 = src.PhotoUrl72,
                PhotoUrl192 = src.PhotoUrl192,
                PhotoUrl512 = src.PhotoUrl512,
                PhoneNumber = src.PhoneNumber,
                Identities = MapTo(src.Identities).ToList()
            };

    public Shared.Models.TermsOfUse? MapTo(TermsOfUse? src) =>
        src is null
            ? null
            : new Shared.Models.TermsOfUse
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                Active = src.Active,
                Terms = src.Terms
            };

    public IEnumerable<IndustryMainCategory> MapTo(IEnumerable<Shared.Database.Entities.IndustryMainCategory> src) => src.Select(MapTo);

    public OrganizationTermsOfUse? MapTo(Shared.Models.TermsOfUse? src) =>
        src is null ? null : new OrganizationTermsOfUse { Id = src.Id, Terms = src.Terms };

    public IEnumerable<OrganizationIndustryMainCategoryReferenceDetails> MapTo(IEnumerable<IndustryMainCategory> src) => src.Select(MapTo);

    public IEnumerable<OrganizationDetails> MapTo(IEnumerable<Shared.Models.Organization> src) => src.Select(MapTo)!;

    public OrganizationDetails? MapTo(Shared.Models.Organization? src)
    {
        if (src is null)
        {
            return null;
        }

        var organizationOffering = src.OrganizationOfferings.FirstOrDefault();
        var availableOfferings = organizationOffering is null || organizationOffering.Code == OfferingCode.EarlyBirdV1
            ? []
            : Offerings.AllOfferings
                .Where(item => item != organizationOffering.Code)
                .Select(item =>
                {
                    var offering = item.GetOffering();
                    return new OrganizationOfferingDetails
                    {
                        IsEnterprise = item.IsEnterpriseOffering(),
                        Code = item.ToOfferingCode(),
                        Name = offering.Name,
                        UnitPrice = offering.UnitPrice,
                        FeatureSet = MapTo(offering),
                        UnderPriceLines = offering.UnderPriceLines,
                        Free = item.IsFreeOffering(),
                        EarlyBird = item.IsEarlyBirdOffering()
                    };
                });

        return new OrganizationDetails
        {
            Id = src.Id,
            UniqueAlphanumericName = src.UniqueAlphanumericName,
            Name = src.Name,
            About = src.About,
            Website = src.Website,
            AgreedToTermsOfUse = src.AgreedToTermsOfUse,
            LogoUrl = src.LogoUrl,
            Type = new OrganizationTypeDetails { Type = src.Type, Name = src.Type.ToOrganizationTypeName() },
            ContactEmail = src.ContactEmail,
            ContactPhone = src.ContactPhone,
            StripeAuthorizeExistingConnectAccountUrl = src.StripeAuthorizeExistingConnectAccountUrl.ToString(),
            MemberVisibilityPolicy =
                new OrganizationMemberVisibilityPolicyDetails
                {
                    Type = src.MemberVisibilityPolicy, Name = src.MemberVisibilityPolicy.ToOrganizationMemberVisibilityPolicyName()
                },
            PaymentMethods = MapTo(src.OrganizationStripePaymentMethods),
            HasAttachedPaymentMethod = src.HasAttachedPaymentMethod,
            TermsOfUse = MapTo(src.TermsOfUse),
            IndustrySubCategories = src.IndustrySubCategories.Select(item => MapTo(item, null)),
            PhysicalAddress = MapToGraphQl(src.PhysicalAddress),
            BillingDetails = MapToGraphQl(src.BillingDetails),
            AvailableOfferings = availableOfferings,
            ActiveOffering = MapTo(organizationOffering),
            CanModify = src.CanModify,
            CanDelete = src.CanDelete,
            CanInvitePeople = src.CanInvitePeople,
            CanViewAnalytics = src.CanViewAnalytics,
            HasLocation = src.HasLocation,
            HasTeam = src.HasTeam,
            HasFutureBooking = src.HasFutureBooking,
            IsMyOnboardingDone = src.IsMyOnboardingDone,
            Members = MapTo(src.OrganizationMembers),
            ResourceTypes = src.Tags
                .Where(item => OrganizationTagTypeConstants.ResourceTypes.Any(resourceType => resourceType == item.Type))
                .Select(item => MapTo(item)!),
            SsoSettings = MapTo(src.OrganizationSsoSettings),
            TaxDetails = MapTo(src.OrganizationTaxDetails)
        };
    }

    public OrganizationMemberDetails MapTo(OrganizationMember src) =>
        new()
        {
            Id = src.Id,
            Role = src.Role,
            Status = src.Status,
            IsOrganizationOnboardingDone = src.IsOrganizationOnboardingDone ?? false,
            Customer = MapTo(src.Customer)!
        };

    public OrganizationAnalytics MapTo(
        IEnumerable<OrganizationMemberAttendancePercentage> organizationMemberAttendancePercentages,
        IEnumerable<OrganizationDailyBookingsTotal> organizationDailyBookingsTotals) =>
        new()
        {
            MemberAttendancePercentage = organizationMemberAttendancePercentages
                .Select(item => new GraphQL.Analytics.OrganizationMemberAttendancePercentage { Date = item.Date, Percentage = item.Percentage }),
            DailyBookingsTotals = organizationDailyBookingsTotals
                .Select(item => new GraphQL.Analytics.OrganizationDailyBookingsTotal { Date = item.Date, Total = item.Total })
        };

    public Shared.Models.Organization MapTo(AddOrganizationInput src) =>
        new()
        {
            Id = src.Id.ToSafeString(),
            UniqueAlphanumericName = src.UniqueAlphanumericName,
            Name = src.Name,
            About = src.About,
            Website = src.Website,
            Type = src.Type,
            ContactEmail = src.ContactEmail,
            ContactPhone = src.ContactPhone,
            MemberVisibilityPolicy = src.MemberVisibilityPolicy,
            AgreedToTermsOfUse = src.AgreedToTermsOfUse,
            IndustrySubCategories = src.IndustrySubCategoryIds.Select(item => new IndustrySubCategory { Id = item }).ToList(),
            TermsOfUse = new Shared.Models.TermsOfUse { Id = src.TermsOfUseId }
        };

    public Shared.Models.Organization MapTo(UpdateOrganizationInput src) =>
        new()
        {
            Id = src.Id,
            UniqueAlphanumericName = src.UniqueAlphanumericName,
            Name = src.Name,
            About = src.About,
            Website = src.Website,
            Type = src.Type,
            ContactEmail = src.ContactEmail,
            ContactPhone = src.ContactPhone,
            MemberVisibilityPolicy = src.MemberVisibilityPolicy,
            IndustrySubCategories = src.IndustrySubCategoryIds.Select(item => new IndustrySubCategory { Id = item }).ToList()
        };

    public global::Api.Shared.Services.Grpc.Skedular.Organization.V1.TermsOfUse
        MapToGrpcResponse(Shared.Models.TermsOfUse src) => new() { Id = src.Id, Terms = src.Terms };

    public Shared.Models.Organization MapTo(Admin_AddInput src) =>
        new()
        {
            Id = src.Id,
            UniqueAlphanumericName = src.UniqueAlphanumericName,
            Name = src.Name,
            About = src.About,
            Website = src.Website,
            Type = src.Type.ToOrganizationType(),
            ContactEmail = src.ContactEmail,
            ContactPhone = src.ContactPhone,
            MemberVisibilityPolicy = src.MemberVisibilityPolicy.ToOrganizationMemberVisibilityPolicy(),
            AgreedToTermsOfUse = src.AgreedToTermsOfUse,
            TermsOfUse = string.IsNullOrWhiteSpace(src.TermsOfUseId) ? null : new Shared.Models.TermsOfUse { Id = src.TermsOfUseId },
            LogoUrl = src.LogoUrl,
            IndustrySubCategories = src.IndustrySubCategoryIds.Select(item => new IndustrySubCategory { Id = item }).ToList()
        };

    public global::Api.Shared.Services.Grpc.Skedular.Organization.V1.Organization MapToGrpcResponse(Shared.Models.Organization src)
    {
        var organizationOffering = src.OrganizationOfferings.Where(item => !item.DeletedAt.HasValue)
            .OrderByDescending(item => item.End).First();
        var organization = new global::Api.Shared.Services.Grpc.Skedular.Organization.V1.Organization
        {
            Id = src.Id,
            UniqueAlphanumericName = src.UniqueAlphanumericName.ToSafeString(),
            Name = src.Name.ToSafeString(),
            About = src.About.ToSafeString(),
            Website = src.Website.ToSafeString(),
            Type = src.Type.ToOrganizationType(),
            ContactEmail = src.ContactEmail.ToSafeString(),
            ContactPhone = src.ContactPhone.ToSafeString(),
            MemberVisibilityPolicy = src.MemberVisibilityPolicy.ToOrganizationMemberVisibilityPolicy(),
            AgreedToTermsOfUse = src.AgreedToTermsOfUse,
            LogoUrl = src.LogoUrl.ToSafeString(),
            Offering = new global::Api.Shared.Services.Grpc.Skedular.Organization.V1.Offering
            {
                Id = organizationOffering.Id,
                OrganizationId = src.Id,
                Code = organizationOffering.Code.ToOfferingCode(),
                Start = organizationOffering.Start.ToTimestamp(),
                End = organizationOffering.End.ToTimestamp(),
                AutoRenew = organizationOffering.AutoRenew,
                UnitPrice = organizationOffering.UnitPrice
            },
            HasAttachedPaymentMethod = src.HasAttachedPaymentMethod,
            HasFutureBooking = src.HasFutureBooking,
            TaxDetails = MapToGrpcResponse(src.OrganizationTaxDetails),
            PhysicalAddress = MapToGrpcResponse(src.PhysicalAddress)
        };

        organization.Tags.AddRange(MapToGrpcResponse(src.Tags));
        organization.ResourceTypes.AddRange(MapToGrpcResponseResourceType(src.Tags));

        organization.Offering.ActiveCustomerIds.AddRange(
            organizationOffering.OrganizationOfferingActiveMembers.Select(item => item.OrganizationMember.Customer.Id));

        organization.IndustrySubCategories.AddRange(src.IndustrySubCategories.Select(item =>
            new global::Api.Shared.Services.Grpc.Skedular.Organization.V1.IndustrySubCategory
            {
                Id = item.Id, Name = item.Name, MainCategoryName = item.IndustryMainCategory.Name
            }));

        organization.Members.AddRange(MapToGrpcResponse(src.OrganizationMembers));

        return organization;
    }

    public Shared.Database.Entities.OrganizationMember MapToEntity(
        OrganizationMember src,
        Shared.Database.Entities.Organization organization,
        Shared.Database.Entities.Customer customer) =>
        MergeToEntity(src, new Shared.Database.Entities.OrganizationMember(), organization, customer);

    public Shared.Database.Entities.OrganizationMember MergeToEntity(
        OrganizationMember src,
        Shared.Database.Entities.OrganizationMember dest,
        Shared.Database.Entities.Organization organization,
        Shared.Database.Entities.Customer customer)
    {
        dest.Id = src.Id;
        dest.Role = src.Role.ToOrganizationMemberRole();
        dest.Status = src.Status.ToOrganizationMemberStatus();
        dest.IsOrganizationOnboardingDone = src.IsOrganizationOnboardingDone;
        dest.Organization = organization;
        dest.Customer = customer;
        return dest;
    }

    public OrganizationMember MapTo(Admin_AddMemberInput src) => MapTo(src.Member, new Shared.Models.Organization { Id = src.Id });

    public OrganizationEdge MapTo(Edge<Shared.Models.Organization> src) => new(MapTo(src.Node)!, src.Cursor);

    public IEnumerable<Edge<OrganizationMember>> MapTo(
        IEnumerable<Edge<Shared.Database.Entities.OrganizationMember>> src,
        Shared.Models.Organization organization) =>
        src.Select(item => MapTo(item, organization));

    public OrganizationMemberEdge MapTo(Edge<OrganizationMember> src) => new(MapTo(src.Node), src.Cursor);

    public MemberEdge MapToGrpcResponse(Edge<OrganizationMember> src) => new() { Cursor = src.Cursor, Node = MapToGrpcResponse(src.Node) };

    public Tag MapTo(Shared.Database.Entities.Tag src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Name = src.Name,
            Description = src.Description,
            Type = src.Type.ToOrganizationTagType(),
            Color = src.Color
        };

    public Shared.Database.Entities.Tag MapTo(Tag src, Shared.Database.Entities.Organization organization) =>
        MergeTo(src, new Shared.Database.Entities.Tag(), organization);

    public Shared.Database.Entities.Tag MergeTo(
        Tag src,
        Shared.Database.Entities.Tag dest,
        Shared.Database.Entities.Organization organization)
    {
        dest.Id = src.Id;
        dest.Name = src.Name;
        dest.Description = src.Description;
        dest.Type = src.Type.ToOrganizationTagType();
        dest.Color = src.Color;
        dest.Organization = organization;
        return dest;
    }

    public IEnumerable<Edge<Tag>> MapTo(IEnumerable<Edge<Shared.Database.Entities.Tag>> src, Shared.Models.Organization organization) =>
        src.Select(item => MapTo(item, organization));

    public Tag MapTo(AddCustomTagInput src) =>
        new()
        {
            Id = src.Id.ToSafeString(),
            Name = src.Name,
            Description = src.Description,
            Organization = new Shared.Models.Organization { Id = src.OrganizationId },
            Type = OrganizationTagType.Custom,
            Color = src.Color
        };

    public Tag MapTo(UpdateCustomTagInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name,
            Description = src.Description,
            Type = OrganizationTagType.Custom,
            Color = src.Color
        };

    public Tag MapTo(GraphQL.Tag.AddZoneInput src) =>
        new()
        {
            Id = src.Id.ToSafeString(),
            Name = src.Name,
            Description = src.Description,
            Organization = new Shared.Models.Organization { Id = src.OrganizationId },
            Type = OrganizationTagType.Zone,
            Color = src.Color
        };

    public Tag MapTo(GraphQL.Tag.UpdateZoneInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name,
            Description = src.Description,
            Type = OrganizationTagType.Zone,
            Color = src.Color
        };

    public OrganizationTagDetails? MapTo(Tag? src) =>
        src is null
            ? null
            : new OrganizationTagDetails
            {
                Id = src.Id,
                Name = src.Name,
                Description = src.Description,
                TagType = src.Type.ToOrganizationTagType(),
                Color = src.Color
            };

    public OrganizationTagEdge MapTo(Edge<Tag> src) => new(MapTo(src.Node)!, src.Cursor);

    public CustomTag MapToGrpcResponseCustomTag(Tag? src) =>
        src is null
            ? new CustomTag()
            : new CustomTag
            {
                Id = src.Id, Name = src.Name.ToSafeString(), Description = src.Description.ToSafeString(), Color = src.Color.ToSafeString()
            };

    public CustomTagEdge MapToGrpcResponseCustomTag(Edge<Tag> src) => new() { Cursor = src.Cursor, Node = MapToGrpcResponseCustomTag(src.Node) };

    public Tag MapTo(global::Api.Shared.Services.Grpc.Skedular.Organization.V1.AddCustomTagInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Description = src.Description.ToSafeString(),
            Type = OrganizationTagType.Custom,
            Color = src.Color.ToSafeString(),
            Organization = new Shared.Models.Organization { Id = src.OrganizationId }
        };

    public Tag MapTo(global::Api.Shared.Services.Grpc.Skedular.Organization.V1.UpdateCustomTagInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Description = src.Description.ToSafeString(),
            Type = OrganizationTagType.Custom,
            Color = src.Color
        };

    public Zone MapToGrpcResponseZone(Tag? src) =>
        src is null
            ? new Zone()
            : new Zone
            {
                Id = src.Id, Name = src.Name.ToSafeString(), Description = src.Description.ToSafeString(), Color = src.Color.ToSafeString()
            };

    public ZoneEdge MapToGrpcResponseZone(Edge<Tag> src) => new() { Cursor = src.Cursor, Node = MapToGrpcResponseZone(src.Node) };

    public Tag MapTo(AddZoneInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Description = src.Description.ToSafeString(),
            Type = OrganizationTagType.Zone,
            Color = src.Color.ToSafeString(),
            Organization = new Shared.Models.Organization { Id = src.OrganizationId }
        };

    public Tag MapTo(UpdateZoneInput src) =>
        new() { Id = src.Id, Name = src.Name.ToSafeString(), Description = src.Description.ToSafeString(), Type = OrganizationTagType.Zone };

    public IEnumerable<string> MapTo(Offering offering) => offering.FeatureSets.Select(MapTo);

    public OrganizationSsoSettings MapTo(UpdateOrganizationSsoSettingsInput src) =>
        new()
        {
            IsActive = src.IsActive,
            EntityId = src.EntityId,
            LoginUrl = src.LoginUrl,
            AppFederationMetadataUrl = src.AppFederationMetadataUrl,
            Organization = new Shared.Models.Organization { Id = src.OrganizationId }
        };

    public Shared.Database.Entities.OrganizationSsoSettings MapToEntity(
        OrganizationSsoSettings src,
        Shared.Database.Entities.Organization organization) =>
        MergeToEntity(src, new Shared.Database.Entities.OrganizationSsoSettings(), organization);

    public Shared.Database.Entities.OrganizationSsoSettings MergeToEntity(
        OrganizationSsoSettings src,
        Shared.Database.Entities.OrganizationSsoSettings dest,
        Shared.Database.Entities.Organization organization)
    {
        dest.Id = src.Id;
        dest.IsActive = src.IsActive;
        dest.EntityId = src.EntityId;
        dest.LoginUrl = src.LoginUrl;
        dest.AppFederationMetadataUrl = src.AppFederationMetadataUrl;
        dest.Organization = organization;
        return dest;
    }

    public Tag MapTo(AddProductTagInput src) =>
        new()
        {
            Id = src.Id.ToSafeString(),
            Name = src.Name,
            Description = src.Description,
            Organization = new Shared.Models.Organization { Id = src.OrganizationId },
            Type = OrganizationTagType.Product,
            Color = src.Color
        };

    public Tag MapTo(UpdateProductTagInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name,
            Description = src.Description,
            Type = OrganizationTagType.Product,
            Color = src.Color
        };

    public Tag MapTo(AddLocationTagInput src) =>
        new()
        {
            Id = src.Id.ToSafeString(),
            Name = src.Name,
            Description = src.Description,
            Organization = new Shared.Models.Organization { Id = src.OrganizationId },
            Type = OrganizationTagType.Location,
            Color = src.Color
        };

    public Tag MapTo(UpdateLocationTagInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name,
            Description = src.Description,
            Type = OrganizationTagType.Location,
            Color = src.Color
        };

    public OrganizationBillingDetails MapTo(Shared.Models.OrganizationBillingDetails src, Shared.Database.Entities.Organization organization) =>
        MergeToEntity(src, new OrganizationBillingDetails(), organization);

    public OrganizationBillingDetails MergeToEntity(
        Shared.Models.OrganizationBillingDetails src,
        OrganizationBillingDetails dest,
        Shared.Database.Entities.Organization organization)
    {
        dest.Id = src.Id;
        dest.CompanyName = src.CompanyName;
        dest.Email = src.Email;
        dest.OsmType = src.OsmType;
        dest.OsmId = src.OsmId;
        dest.PlaceId = src.PlaceId;
        dest.Coordinates = src.Coordinates;
        dest.FormattedAddress = src.FormattedAddress;
        dest.AddressLine1 = src.AddressLine1;
        dest.AddressLine2 = src.AddressLine2;
        dest.Suburb = src.Suburb;
        dest.City = src.City;
        dest.Province = src.Province;
        dest.Zipcode = src.Zipcode;
        dest.Country = src.Country;
        dest.Organization = organization;
        return dest;
    }

    public CustomerCreateOptions MapToStripeCustomerCreateOption(Shared.Database.Entities.Organization src) =>
        new()
        {
            Name = src.Name,
            Email = string.IsNullOrWhiteSpace(src.ContactEmail) ? null : src.ContactEmail,
            Phone = string.IsNullOrWhiteSpace(src.ContactPhone) ? null : src.ContactPhone,
            Metadata = new Dictionary<string, string> { { "type", "organization" }, { "organizationId", src.Id } }
        };

    public Shared.Models.OrganizationBillingDetails MapTo(AddOrganizationBillingDetailsInput src) =>
        new()
        {
            Id = src.Id.ToSafeString(),
            CompanyName = src.CompanyName,
            Email = src.Email,
            OsmType = src.OsmType,
            OsmId = src.OsmId,
            PlaceId = src.PlaceId,
            Coordinates = src.Longitude is null || src.Latitude is null ? null : new Point(new Coordinate(src.Longitude.Value, src.Latitude.Value)),
            FormattedAddress = src.FormattedAddress,
            AddressLine1 = src.AddressLine1,
            AddressLine2 = src.AddressLine2,
            Suburb = src.Suburb,
            City = src.City,
            Province = src.Province,
            Zipcode = src.Zipcode,
            Country = src.Country,
            Organization = new Shared.Models.Organization { Id = src.OrganizationId }
        };

    public Shared.Models.OrganizationBillingDetails MapTo(UpdateOrganizationBillingDetailsInput src) =>
        new()
        {
            Id = src.Id,
            CompanyName = src.CompanyName,
            Email = src.Email,
            OsmType = src.OsmType,
            OsmId = src.OsmId,
            PlaceId = src.PlaceId,
            Coordinates = src.Longitude is null || src.Latitude is null ? null : new Point(new Coordinate(src.Longitude.Value, src.Latitude.Value)),
            FormattedAddress = src.FormattedAddress,
            AddressLine1 = src.AddressLine1,
            AddressLine2 = src.AddressLine2,
            Suburb = src.Suburb,
            City = src.City,
            Province = src.Province,
            Zipcode = src.Zipcode,
            Country = src.Country
        };

    public Shared.Models.OrganizationBillingDetails MapTo(AddBillingDetailsInput src) =>
        new()
        {
            Id = src.Id.ToSafeString(),
            CompanyName = src.CompanyName,
            Email = src.Email,
            AddressLine1 = src.AddressLine1,
            AddressLine2 = src.AddressLine2,
            Suburb = src.Suburb,
            City = src.City,
            Province = src.Province,
            Zipcode = src.Zipcode,
            Country = src.Country,
            Organization = new Shared.Models.Organization { Id = src.OrganizationId }
        };

    public Shared.Models.OrganizationBillingDetails MapTo(UpdateBillingDetailsInput src) =>
        new()
        {
            Id = src.Id.ToSafeString(),
            CompanyName = src.CompanyName,
            Email = src.Email,
            AddressLine1 = src.AddressLine1,
            AddressLine2 = src.AddressLine2,
            Suburb = src.Suburb,
            City = src.City,
            Province = src.Province,
            Zipcode = src.Zipcode,
            Country = src.Country
        };

    public BillingDetails MapToGrpcResponse(Shared.Models.OrganizationBillingDetails? src) =>
        src is null
            ? new BillingDetails { Id = string.Empty }
            : new BillingDetails
            {
                Id = src.Id,
                CompanyName = src.CompanyName.ToSafeString(),
                Email = src.Email,
                AddressLine1 = src.AddressLine1,
                AddressLine2 = src.AddressLine2.ToSafeString(),
                Suburb = src.Suburb.ToSafeString(),
                City = src.City,
                Province = src.Province.ToSafeString(),
                Zipcode = src.Zipcode,
                Country = src.Country,
                FormattedAddress = src.ToFormattedAddress(),
                OsmType = src.OsmType.ToSafeString(),
                OsmId = src.OsmId.ToSafeString(),
                PlaceId = src.PlaceId.ToSafeString(),
                Coordinates = src.Coordinates is null ? null : new Coordinates { Longitude = src.Coordinates.X, Latitude = src.Coordinates.Y }
            };

    public Shared.Models.OrganizationBillingDetails? MapTo(OrganizationBillingDetails? src) =>
        src is null
            ? null
            : new Shared.Models.OrganizationBillingDetails
            {
                Id = src.Id,
                CompanyName = src.CompanyName,
                Email = src.Email,
                AddressLine1 = src.AddressLine1,
                AddressLine2 = src.AddressLine2,
                Suburb = src.Suburb,
                City = src.City,
                Province = src.Province,
                Zipcode = src.Zipcode,
                Country = src.Country
            };

    public AccountCreateOptions MapToStripeAccountRequest(Shared.Database.Entities.Organization src) =>
        new()
        {
            BusinessProfile = new AccountBusinessProfileOptions
            {
                Name = src.Name,
                Url = string.IsNullOrWhiteSpace(src.Website) ? null : src.Website,
                SupportUrl = string.IsNullOrWhiteSpace(src.Website) ? null : src.Website,
                SupportEmail = string.IsNullOrWhiteSpace(src.ContactEmail) ? null : src.ContactEmail,
                SupportPhone = string.IsNullOrWhiteSpace(src.ContactPhone) ? null : src.ContactPhone
            },
            Company = new AccountCompanyOptions
            {
                Name = src.Name,
                Address = src.PhysicalAddress is null
                    ? null
                    : new AddressOptions
                    {
                        Line1 = src.PhysicalAddress?.AddressLine1.ToSafeString(),
                        Line2 = src.PhysicalAddress?.AddressLine2.ToSafeString(),
                        City = src.PhysicalAddress?.City.ToSafeString(),
                        State = src.PhysicalAddress?.Province.ToSafeString(),
                        PostalCode = src.PhysicalAddress?.Zipcode.ToSafeString(),
                        Country = src.PhysicalAddress?.Country.ToSafeString()
                    },
                Phone = string.IsNullOrWhiteSpace(src.ContactPhone) ? null : src.ContactPhone
            },
            BusinessType = "company",
            Email = string.IsNullOrWhiteSpace(src.ContactEmail) ? null : src.ContactEmail,
            Capabilities =
                new AccountCapabilitiesOptions
                {
                    CardPayments = new AccountCapabilitiesCardPaymentsOptions { Requested = true },
                    Transfers = new AccountCapabilitiesTransfersOptions { Requested = true }
                },
            Type = "standard",
            Metadata = new Dictionary<string, string> { { "organizationId", src.Id } }
        };

    public OrganizationStripeConnectAccount MapTo(
        Account src,
        string id,
        string name,
        bool isDefault,
        Shared.Database.Entities.Organization organization) =>
        new()
        {
            Id = id,
            IsDefault = isDefault,
            StripeAccountId = src.Id,
            Name = name,
            ChargesEnabled = src.ChargesEnabled,
            PayoutsEnabled = src.PayoutsEnabled,
            Type = src.Type.ToSafeString(),
            Country = src.Country,
            DefaultCurrency = src.DefaultCurrency,
            BusinessType = src.BusinessType,
            CompanyName = src.Company?.Name,
            Url = organization.Website,
            SupportUrl = organization.Website,
            ContactEmail = src.Email,
            ContactPhone = src.Company?.Phone,
            DetailsSubmitted = src.DetailsSubmitted,
            CapabilitiesCardPayments = src.Capabilities.CardPayments.ToSafeString(),
            CapabilitiesTransfers = src.Capabilities.Transfers.ToSafeString(),
            Organization = organization
        };

    public Shared.Models.OrganizationStripeConnectAccount MapTo(OrganizationStripeConnectAccount src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            DeletedAt = src.DeletedAt,
            IsDefault = src.IsDefault,
            StripeAccountId = src.StripeAccountId,
            Name = src.Name,
            ChargesEnabled = src.ChargesEnabled,
            PayoutsEnabled = src.PayoutsEnabled,
            Type = src.Type,
            Country = src.Country,
            DefaultCurrency = src.DefaultCurrency,
            BusinessType = src.BusinessType,
            Url = src.Url,
            SupportUrl = src.SupportUrl,
            CompanyName = src.CompanyName,
            ContactEmail = src.ContactEmail,
            ContactPhone = src.ContactPhone,
            DetailsSubmitted = src.DetailsSubmitted,
            CapabilitiesCardPayments = src.CapabilitiesCardPayments,
            CapabilitiesTransfers = src.CapabilitiesTransfers,
            OnboardingUrl = src.OnboardingUrl,
            Organization = MapTo(src.Organization, Constants.EmptyUri),
            OrganizationStripeConnectAccountAuthorization = MapTo(src.OrganizationStripeConnectAccountAuthorization)
        };

    public OrganizationStripeConnectAccountDetails? MapTo(Shared.Models.OrganizationStripeConnectAccount? src) =>
        src is null
            ? null
            : new OrganizationStripeConnectAccountDetails
            {
                Id = src.Id,
                IsDefault = src.IsDefault,
                Name = src.Name,
                ChargesEnabled = src.ChargesEnabled,
                PayoutsEnabled = src.PayoutsEnabled,
                Type = src.Type,
                Country = src.Country,
                DefaultCurrency = src.DefaultCurrency,
                BusinessType = src.BusinessType,
                CompanyName = src.CompanyName,
                Url = src.Url,
                SupportUrl = src.SupportUrl,
                ContactEmail = src.ContactEmail,
                ContactPhone = src.ContactPhone,
                DetailsSubmitted = src.DetailsSubmitted,
                CapabilitiesCardPayments = src.CapabilitiesCardPayments,
                CapabilitiesTransfers = src.CapabilitiesTransfers,
                OnboardingUrl = src.OnboardingUrl,
                IsOnboardingCompleted = src.IsOnboardingCompleted(),
                IsAuthorized = src.IsAuthorized(),
                Organization = MapTo(src.Organization)!
            };

    public OrganizationStripeConnectAccountEdge MapTo(Edge<Shared.Models.OrganizationStripeConnectAccount> src) => new(MapTo(src.Node)!, src.Cursor);

    public StripeConnectAccountEdge MapToGrpcResponse(Edge<Shared.Models.OrganizationStripeConnectAccount> src) =>
        new() { Cursor = src.Cursor, Node = MapToGrpcResponse(src.Node) };

    public OrganizationBankAccount MapTo(Shared.Models.OrganizationBankAccount src, Shared.Database.Entities.Organization organization) =>
        MergeTo(src, new OrganizationBankAccount(), organization);

    public OrganizationBankAccount MergeTo(
        Shared.Models.OrganizationBankAccount src,
        OrganizationBankAccount dest,
        Shared.Database.Entities.Organization organization)
    {
        dest.Id = src.Id;
        dest.IsDefault = src.IsDefault;
        dest.Name = src.Name;
        dest.BankName = src.BankName;
        dest.AccountHolderName = src.AccountHolderName;
        dest.AccountNumber = src.AccountNumber;
        dest.Country = src.Country;
        dest.Organization = organization;
        return dest;
    }

    public Shared.Models.OrganizationBankAccount MapTo(OrganizationBankAccount src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            IsDefault = src.IsDefault,
            Name = src.Name,
            BankName = src.BankName,
            AccountHolderName = src.AccountHolderName,
            AccountNumber = src.AccountNumber,
            Country = src.Country,
            Organization = MapTo(src.Organization, Constants.EmptyUri)
        };

    public Shared.Models.OrganizationBankAccount MapTo(AddOrganizationBankAccountInput src) =>
        new()
        {
            Id = src.Id.ToSafeString(),
            Name = src.Name,
            BankName = src.BankName,
            AccountHolderName = src.AccountHolderName,
            AccountNumber = src.AccountNumber,
            Country = src.Country,
            Organization = new Shared.Models.Organization { Id = src.OrganizationId }
        };

    public Shared.Models.OrganizationBankAccount MapTo(UpdateOrganizationBankAccountInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name,
            BankName = src.BankName,
            AccountHolderName = src.AccountHolderName,
            AccountNumber = src.AccountNumber,
            Country = src.Country
        };

    public OrganizationBankAccountDetails? MapTo(Shared.Models.OrganizationBankAccount? src) =>
        src is null
            ? null
            : new OrganizationBankAccountDetails
            {
                Id = src.Id,
                IsDefault = src.IsDefault,
                Name = src.Name,
                BankName = src.BankName,
                AccountHolderName = src.AccountHolderName,
                AccountNumber = src.AccountNumber,
                Country = src.Country,
                Organization = MapTo(src.Organization)!
            };

    public OrganizationBankAccountEdge MapTo(Edge<Shared.Models.OrganizationBankAccount> src) => new(MapTo(src.Node)!, src.Cursor);

    public BankAccountEdge MapToGrpcResponse(Edge<Shared.Models.OrganizationBankAccount> src) =>
        new() { Cursor = src.Cursor, Node = MapToGrpcResponse(src.Node) };

    public OrganizationTaxDetails MapTo(UpdateOrganizationTaxDetailsInput src) =>
        new()
        {
            TaxId = src.TaxId,
            TaxRatePercentage = src.TaxRatePercentage,
            Organization = new Shared.Models.Organization { Id = src.OrganizationId }
        };

    public Shared.Database.Entities.OrganizationTaxDetails MapToEntity(
        OrganizationTaxDetails src,
        Shared.Database.Entities.Organization organization) =>
        MergeToEntity(src, new Shared.Database.Entities.OrganizationTaxDetails(), organization);

    public Shared.Database.Entities.OrganizationTaxDetails MergeToEntity(
        OrganizationTaxDetails src,
        Shared.Database.Entities.OrganizationTaxDetails dest,
        Shared.Database.Entities.Organization organization)
    {
        dest.Id = src.Id;
        dest.TaxId = src.TaxId;
        dest.TaxRatePercentage = src.TaxRatePercentage;
        dest.Organization = organization;
        return dest;
    }

    public OrganizationPhysicalAddress MapTo(Shared.Models.OrganizationPhysicalAddress src, Shared.Database.Entities.Organization organization) =>
        MergeTo(src, new OrganizationPhysicalAddress(), organization);

    public OrganizationPhysicalAddress MergeTo(
        Shared.Models.OrganizationPhysicalAddress src,
        OrganizationPhysicalAddress dest,
        Shared.Database.Entities.Organization organization)
    {
        dest.Id = src.Id;
        dest.OsmType = src.OsmType;
        dest.OsmId = src.OsmId;
        dest.PlaceId = src.PlaceId;
        dest.Coordinates = src.Coordinates;
        dest.FormattedAddress = src.FormattedAddress;
        dest.AddressLine1 = src.AddressLine1;
        dest.AddressLine2 = src.AddressLine2;
        dest.Suburb = src.Suburb;
        dest.City = src.City;
        dest.Province = src.Province;
        dest.Zipcode = src.Zipcode;
        dest.Country = src.Country;
        dest.Organization = organization;
        return dest;
    }

    public Shared.Models.OrganizationPhysicalAddress MapTo(OrganizationPhysicalAddress src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            OsmType = src.OsmType,
            OsmId = src.OsmId,
            PlaceId = src.PlaceId,
            Coordinates = src.Coordinates,
            FormattedAddress = src.FormattedAddress,
            AddressLine1 = src.AddressLine1,
            AddressLine2 = src.AddressLine2,
            Suburb = src.Suburb,
            City = src.City,
            Province = src.Province,
            Zipcode = src.Zipcode,
            Country = src.Country,
            Organization = MapTo(src.Organization, Constants.EmptyUri)
        };

    public Shared.Models.OrganizationPhysicalAddress MapTo(AddOrganizationPhysicalAddressInput src) =>
        new()
        {
            Id = src.Id.ToSafeString(),
            OsmType = src.OsmType,
            OsmId = src.OsmId,
            PlaceId = src.PlaceId,
            Coordinates = src.Longitude is null || src.Latitude is null ? null : new Point(new Coordinate(src.Longitude.Value, src.Latitude.Value)),
            FormattedAddress = src.FormattedAddress,
            AddressLine1 = src.AddressLine1,
            AddressLine2 = src.AddressLine2,
            Suburb = src.Suburb,
            City = src.City,
            Province = src.Province,
            Zipcode = src.Zipcode,
            Country = src.Country,
            Organization = new Shared.Models.Organization { Id = src.OrganizationId }
        };

    public Shared.Models.OrganizationPhysicalAddress MapTo(UpdateOrganizationPhysicalAddressInput src) =>
        new()
        {
            Id = src.Id,
            OsmType = src.OsmType,
            OsmId = src.OsmId,
            PlaceId = src.PlaceId,
            Coordinates = src.Longitude is null || src.Latitude is null ? null : new Point(new Coordinate(src.Longitude.Value, src.Latitude.Value)),
            FormattedAddress = src.FormattedAddress,
            AddressLine1 = src.AddressLine1,
            AddressLine2 = src.AddressLine2,
            Suburb = src.Suburb,
            City = src.City,
            Province = src.Province,
            Zipcode = src.Zipcode,
            Country = src.Country
        };

    public OrganizationPhysicalAddressDetails MapTo(Shared.Models.OrganizationPhysicalAddress src) =>
        new()
        {
            Id = src.Id,
            OsmType = src.OsmType,
            OsmId = src.OsmId,
            PlaceId = src.PlaceId,
            Longitude = src.Coordinates?.X,
            Latitude = src.Coordinates?.Y,
            FormattedAddress = src.ToFormattedAddress(),
            MultilinesFormattedAddress = src.ToMultilinesFormattedAddress(),
            AddressLine1 = src.AddressLine1,
            AddressLine2 = src.AddressLine2,
            Suburb = src.Suburb,
            City = src.City,
            Province = src.Province,
            Zipcode = src.Zipcode,
            Country = src.Country,
            Organization = MapTo(src.Organization)!
        };

    public IEnumerable<InviteCustomerToJoinOrganizationDetails> MapTo(IEnumerable<JoinInvitation> src) =>
        src.Select(MapTo);

    public InviteCustomerToJoinOrganizationDetails MapTo(JoinInvitation src) =>
        new()
        {
            Id = src.Id,
            Email = src.Email,
            Status = new OrganizationInvitationStatusDetails { Type = src.Status, Name = src.Status.ToInvitationStatusName() },
            Role = src.Role,
            Organization = MapTo(src.Organization)!,
            CreatedBy = MapTo(src.CreatedBy)!,
            Invitee = MapTo(src.Invitee)
        };

    public Edge<JoinInvitation> MapTo(Edge<Shared.Database.Entities.JoinInvitation> src) => new(MapTo(src.Node), src.Cursor);

    public OrganizationJoinInvitationEdge MapTo(Edge<JoinInvitation> src) => new(MapTo(src.Node), src.Cursor);

    public OrganizationStripeConnectAccount MergeTo(Account src, OrganizationStripeConnectAccount dest)
    {
        dest.StripeAccountId = src.Id;
        dest.ChargesEnabled = src.ChargesEnabled;
        dest.PayoutsEnabled = src.PayoutsEnabled;
        dest.Type = src.Type.ToSafeString();
        dest.Country = src.Country;
        dest.DefaultCurrency = src.DefaultCurrency;
        dest.BusinessType = src.BusinessType;
        dest.CompanyName = src.BusinessProfile?.Name;
        dest.Url = src.BusinessProfile?.Url;
        dest.SupportUrl = src.BusinessProfile?.SupportUrl;
        dest.ContactEmail = src.Email;
        dest.ContactPhone = src.BusinessProfile?.SupportPhone;
        dest.DetailsSubmitted = src.DetailsSubmitted;
        dest.CapabilitiesCardPayments = src.Capabilities.CardPayments.ToSafeString();
        dest.CapabilitiesTransfers = src.Capabilities.Transfers.ToSafeString();
        return dest;
    }

    private static IEnumerable<global::Api.Shared.Services.Grpc.Skedular.Organization.V1.Tag> MapToGrpcResponse(IEnumerable<Tag> src) =>
        src.Select(MapToGrpcResponse);

    private static global::Api.Shared.Services.Grpc.Skedular.Organization.V1.Tag MapToGrpcResponse(Tag src) =>
        new() { Id = src.Id, Name = src.Name.ToSafeString(), Description = src.Description.ToSafeString(), Color = src.Color.ToSafeString() };

    private static IEnumerable<ResourceType> MapToGrpcResponseResourceType(IEnumerable<Tag> src) =>
        src
            .Where(item => OrganizationTagTypeConstants.ResourceTypes.Any(resourceType => resourceType == item.Type))
            .Select(MapToGrpcResponseResourceType);

    private static ResourceType MapToGrpcResponseResourceType(Tag src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Description = src.Description.ToSafeString(),
            Color = src.Color.ToSafeString(),
            TagType = src.Type.ToOrganizationTagType()
        };

    private IEnumerable<OrganizationMember> MapTo(
        IEnumerable<Shared.Database.Entities.OrganizationMember> src,
        Shared.Models.Organization organization) =>
        src.Select(item => MapTo(item, organization));

    private static Member MapToGrpcResponse(OrganizationMember src) =>
        new()
        {
            Id = src.Id,
            Role = src.Role switch
            {
                OrganizationMemberRole.Owner => Role.Owner,
                OrganizationMemberRole.Administrator => Role.Administrator,
                OrganizationMemberRole.Member => Role.Member,
                _ => throw new ArgumentOutOfRangeException()
            },
            Status = src.Status switch
            {
                OrganizationMemberStatus.Active => global::Api.Shared.Services.Grpc.Skedular.Organization.V1.OrganizationMemberStatus.Active,
                OrganizationMemberStatus.Inactive => global::Api.Shared.Services.Grpc.Skedular.Organization.V1.OrganizationMemberStatus.Inactive,
                _ => throw new ArgumentOutOfRangeException()
            },
            IsOrganizationOnboardingDone = src.IsOrganizationOnboardingDone ?? false,
            Customer = MapToGrpcResponse(src.Customer)
        };

    private static IEnumerable<Member> MapToGrpcResponse(IEnumerable<OrganizationMember> src) => src.Select(MapToGrpcResponse);

    private static global::Api.Shared.Services.Grpc.Skedular.Organization.V1.Customer MapToGrpcResponse(Customer src)
    {
        var customer = new global::Api.Shared.Services.Grpc.Skedular.Organization.V1.Customer
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            GivenName = src.GivenName.ToSafeString(),
            MiddleName = src.MiddleName.ToSafeString(),
            FamilyName = src.FamilyName.ToSafeString(),
            PhotoUrl = src.PhotoUrl.ToSafeString(),
            PhotoUrl24 = src.PhotoUrl24.ToSafeString(),
            PhotoUrl32 = src.PhotoUrl32.ToSafeString(),
            PhotoUrl48 = src.PhotoUrl48.ToSafeString(),
            PhotoUrl72 = src.PhotoUrl72.ToSafeString(),
            PhotoUrl192 = src.PhotoUrl192.ToSafeString(),
            PhotoUrl512 = src.PhotoUrl512.ToSafeString(),
            PhoneNumber = src.PhoneNumber.ToSafeString()
        };

        customer.Identities.AddRange(MapToGrpcResponse(src.Identities));

        return customer;
    }

    private static IEnumerable<global::Api.Shared.Services.Grpc.Skedular.Organization.V1.Identity> MapToGrpcResponse(IEnumerable<Identity> src) =>
        src.Select(MapToGrpcResponse);

    private static global::Api.Shared.Services.Grpc.Skedular.Organization.V1.Identity MapToGrpcResponse(Identity src) =>
        new() { Id = src.Id, Email = src.Email.ToSafeString(), EmailVerified = src.EmailVerified ?? false };

    private static OrganizationMember MapTo(Member src, Shared.Models.Organization organization) =>
        new()
        {
            Id = src.Id,
            Role = src.Role switch
            {
                Role.Owner => OrganizationMemberRole.Owner,
                Role.Administrator => OrganizationMemberRole.Administrator,
                Role.Member => OrganizationMemberRole.Member,
                _ => throw new ArgumentOutOfRangeException()
            },
            Status = src.Status switch
            {
                global::Api.Shared.Services.Grpc.Skedular.Organization.V1.OrganizationMemberStatus.Active => OrganizationMemberStatus.Active,
                global::Api.Shared.Services.Grpc.Skedular.Organization.V1.OrganizationMemberStatus.Inactive => OrganizationMemberStatus.Inactive,
                _ => throw new ArgumentOutOfRangeException()
            },
            IsOrganizationOnboardingDone = src.IsOrganizationOnboardingDone,
            Customer = new Customer { Id = src.Customer.Id },
            Organization = organization
        };

    private static CustomerDetails? MapTo(Customer? src) =>
        src is null
            ? null
            : new CustomerDetails
            {
                UniqueId = src.Id,
                Email = src.Identities.ToFirstEmail(),
                Name = src.Name,
                GivenName = src.GivenName,
                MiddleName = src.MiddleName,
                FamilyName = src.FamilyName,
                PhotoUrl = src.PhotoUrl,
                PhotoUrl24 = src.PhotoUrl24,
                PhotoUrl32 = src.PhotoUrl32,
                PhotoUrl48 = src.PhotoUrl48,
                PhotoUrl72 = src.PhotoUrl72,
                PhotoUrl192 = src.PhotoUrl192,
                PhotoUrl512 = src.PhotoUrl512,
                PhoneNumber = src.PhoneNumber
            };

    private OrganizationActiveOfferingDetails MapTo(OrganizationOffering? src)
    {
        if (src is null)
        {
            return new OrganizationActiveOfferingDetails();
        }

        var offering = src.Code.GetOffering();
        return new OrganizationActiveOfferingDetails
        {
            Id = src.Id,
            Code = src.Code.ToOfferingCode(),
            IsEnterprise = src.Code.IsEnterpriseOffering(),
            Name = offering.Name,
            Start = src.Start,
            End = src.End,
            UnitPrice = src.UnitPrice,
            FeatureSet = MapTo(offering),
            UnderPriceLines = offering.UnderPriceLines,
            Free = src.Code.IsFreeOffering(),
            EarlyBird = src.Code.IsEarlyBirdOffering()
        };
    }

    private static string MapTo(FeatureSetCode item) => Features.FeatureSet[item].Description;

    private static OrganizationIndustryMainCategoryReferenceDetails MapTo(IndustryMainCategory src)
    {
        var organizationIndustryMainCategoryReferenceDetails = new OrganizationIndustryMainCategoryReferenceDetails { Id = src.Id, Name = src.Name };

        organizationIndustryMainCategoryReferenceDetails.SubCategories =
            MapTo(src.IndustrySubCategories, organizationIndustryMainCategoryReferenceDetails);

        return organizationIndustryMainCategoryReferenceDetails;
    }

    private static IEnumerable<OrganizationIndustrySubCategoryReferenceDetails> MapTo(
        IEnumerable<IndustrySubCategory> src,
        OrganizationIndustryMainCategoryReferenceDetails? organizationIndustryMainCategoryReferenceDetails) =>
        src.Select(item => MapTo(item, organizationIndustryMainCategoryReferenceDetails));

    private static OrganizationIndustrySubCategoryReferenceDetails MapTo(
        IndustrySubCategory src,
        OrganizationIndustryMainCategoryReferenceDetails? organizationIndustryMainCategoryReferenceDetails) =>
        new()
        {
            Id = src.Id,
            Name = src.Name,
            MainCategoryName = organizationIndustryMainCategoryReferenceDetails is null
                ? src.IndustryMainCategory.Name
                : organizationIndustryMainCategoryReferenceDetails.Name
        };

    private IndustrySubCategory? MapTo(Shared.Database.Entities.IndustrySubCategory? src, IndustryMainCategory? industryMainCategory)
    {
        if (src is null)
        {
            return null;
        }

        var industrySubCategory = new IndustrySubCategory
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Name = src.Name,
            IndustryMainCategory = industryMainCategory ?? MapTo(src.IndustryMainCategory)
        };

        return industrySubCategory;
    }

    private IEnumerable<IndustrySubCategory> MapTo(
        IEnumerable<Shared.Database.Entities.IndustrySubCategory> src,
        IndustryMainCategory? industryMainCategory) =>
        src.Select(item => MapTo(item, industryMainCategory))!;

    private IndustryMainCategory MapTo(Shared.Database.Entities.IndustryMainCategory src)
    {
        var industryMainCategory = new IndustryMainCategory
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Name = src.Name
        };

        industryMainCategory.IndustrySubCategories = MapTo(src.IndustrySubCategories, industryMainCategory).ToList();

        return industryMainCategory;
    }

    private static IEnumerable<Identity> MapTo(IEnumerable<Shared.Database.Entities.Identity> src) => src.Select(MapTo);

    private static Identity MapTo(Shared.Database.Entities.Identity src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            EventRaisedAt = src.EventRaisedAt,
            Email = src.Email,
            EmailVerified = src.EmailVerified
        };

    private static IEnumerable<OrganizationOffering> MapTo(
        IEnumerable<Shared.Database.Entities.OrganizationOffering> src,
        Shared.Models.Organization organization) =>
        src.Select(item => MapTo(item, organization));

    private static OrganizationOffering MapTo(Shared.Database.Entities.OrganizationOffering src,
        Shared.Models.Organization organization) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Code = src.Code,
            Start = src.Start,
            End = src.End,
            AutoRenew = src.AutoRenew,
            UnitPrice = src.UnitPrice,
            Organization = organization
        };

    private static IEnumerable<DailyMemberCountRecording> MapTo(
        IEnumerable<Shared.Database.Entities.DailyMemberCountRecording> src,
        Shared.Models.Organization organization) =>
        src.Select(item => MapTo(item, organization));

    private static DailyMemberCountRecording MapTo(Shared.Database.Entities.DailyMemberCountRecording src, Shared.Models.Organization organization) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Organization = organization,
            Date = src.Date,
            Count = src.Count
        };

    private static IEnumerable<Location> MapTo(IEnumerable<Shared.Database.Entities.Location> src, Shared.Models.Organization organization) =>
        src.Select(item => MapTo(item, organization));

    private static Location MapTo(Shared.Database.Entities.Location src, Shared.Models.Organization organization) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            EventRaisedAt = src.EventRaisedAt,
            Organization = organization
        };

    private static IEnumerable<Team> MapTo(IEnumerable<Shared.Database.Entities.Team> src, Shared.Models.Organization organization) =>
        src.Select(item => MapTo(item, organization));

    private static Team MapTo(Shared.Database.Entities.Team src, Shared.Models.Organization organization) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            EventRaisedAt = src.EventRaisedAt,
            Organization = organization
        };

    private IEnumerable<JoinInvitation> MapTo(IEnumerable<Shared.Database.Entities.JoinInvitation> src, Shared.Models.Organization organization) =>
        src.Select(item => MapTo(item, organization));

    private JoinInvitation MapTo(Shared.Database.Entities.JoinInvitation src, Shared.Models.Organization organization) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Email = src.Email,
            Status = src.Status.ToInvitationStatus(),
            Organization = organization,
            CreatedBy = MapTo(src.CreatedBy)!,
            Invitee = MapTo(src.Invitee)
        };

    private Edge<OrganizationMember> MapTo(Edge<Shared.Database.Entities.OrganizationMember> src, Shared.Models.Organization organization) =>
        new(MapTo(src.Node, organization), src.Cursor);

    private static IEnumerable<AzureTenant> MapTo(IEnumerable<Shared.Database.Entities.AzureTenant> src, Shared.Models.Organization organization) =>
        src.Select(item => MapTo(item, organization));

    private static AzureTenant MapTo(Shared.Database.Entities.AzureTenant src, Shared.Models.Organization organization)
    {
        var azureTenant = new AzureTenant
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            DeletedAt = src.DeletedAt,
            Name = src.Name,
            Organization = organization
        };

        azureTenant.AzureTenantMembers = MapTo(src.AzureTenantMembers, azureTenant).ToList();

        return azureTenant;
    }

    private static IEnumerable<AzureTenantMember> MapTo(IEnumerable<Shared.Database.Entities.AzureTenantMember> src, AzureTenant azureTenant) =>
        src.Select(item => MapTo(item, azureTenant));

    private static AzureTenantMember MapTo(Shared.Database.Entities.AzureTenantMember src, AzureTenant azureTenant) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            DeletedAt = src.DeletedAt,
            Email = src.Email,
            Designation = src.Designation,
            Name = src.Name,
            GivenName = src.GivenName,
            FamilyName = src.FamilyName,
            PreferredLanguage = src.PreferredLanguage,
            PhotoUrl = src.PhotoUrl,
            PhotoUrl48 = src.PhotoUrl48,
            PhotoUrl64 = src.PhotoUrl64,
            PhotoUrl96 = src.PhotoUrl96,
            PhotoUrl120 = src.PhotoUrl120,
            PhotoUrl240 = src.PhotoUrl240,
            PhotoUrl360 = src.PhotoUrl360,
            PhotoUrl432 = src.PhotoUrl432,
            PhotoUrl504 = src.PhotoUrl504,
            PhotoUrl648 = src.PhotoUrl648,
            AzureTenant = azureTenant
        };

    private Edge<Tag> MapTo(Edge<Shared.Database.Entities.Tag> src, Shared.Models.Organization organization)
    {
        var tag = MapTo(src.Node);
        tag.Organization = organization;
        return new Edge<Tag>(tag, src.Cursor);
    }

    private IEnumerable<OrganizationMemberDetails> MapTo(IEnumerable<OrganizationMember> src) => src.Select(MapTo);

    private static IEnumerable<Tag> MapTo(IEnumerable<Shared.Database.Entities.Tag> src, Shared.Models.Organization organization) =>
        src.Select(item => MapTo(item, organization));

    private static Tag MapTo(Shared.Database.Entities.Tag src, Shared.Models.Organization organization) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Name = src.Name,
            Description = src.Description,
            Type = src.Type.ToOrganizationTagType(),
            Color = src.Color,
            Organization = organization
        };

    private static Shared.Models.OrganizationBillingDetails? MapTo(OrganizationBillingDetails? src, Shared.Models.Organization organization) =>
        src is null
            ? null
            : new Shared.Models.OrganizationBillingDetails
            {
                Id = src.Id,
                CompanyName = src.CompanyName,
                Email = src.Email,
                OsmType = src.OsmType,
                OsmId = src.OsmId,
                PlaceId = src.PlaceId,
                Coordinates = src.Coordinates,
                FormattedAddress = src.FormattedAddress,
                AddressLine1 = src.AddressLine1,
                AddressLine2 = src.AddressLine2,
                Suburb = src.Suburb,
                City = src.City,
                Province = src.Province,
                Zipcode = src.Zipcode,
                Country = src.Country,
                Organization = organization
            };

    private static GraphQL.Billing.OrganizationBillingDetails? MapToGraphQl(Shared.Models.OrganizationBillingDetails? src) =>
        src is null
            ? null
            : new GraphQL.Billing.OrganizationBillingDetails
            {
                Id = src.Id,
                CompanyName = src.CompanyName,
                Email = src.Email,
                OsmType = src.OsmType,
                OsmId = src.OsmId,
                PlaceId = src.PlaceId,
                Longitude = src.Coordinates?.X,
                Latitude = src.Coordinates?.Y,
                FormattedAddress = src.ToFormattedAddress(),
                MultilinesFormattedAddress = src.ToMultilinesFormattedAddress(),
                AddressLine1 = src.AddressLine1,
                AddressLine2 = src.AddressLine2,
                Suburb = src.Suburb,
                City = src.City,
                Province = src.Province,
                Zipcode = src.Zipcode,
                Country = src.Country
            };

    private static IEnumerable<OrganizationPaymentMethod> MapTo(IEnumerable<OrganizationStripePaymentMethod> src) => src.Select(MapTo);

    private static OrganizationPaymentMethod MapTo(OrganizationStripePaymentMethod src) =>
        new()
        {
            Id = src.Id,
            CardBrand = src.CardBrand,
            CardCountry = src.CardCountry,
            CardDescription = src.CardDescription,
            CardExpiryMonth = src.CardExpiryMonth,
            CardExpiryYear = src.CardExpiryYear,
            CardFingerprint = src.CardFingerprint,
            CardFunding = src.CardFunding,
            CardIssuer = src.CardIssuer,
            CardLastFourDigit = src.CardLastFourDigit
        };

    private static OrganizationStripeCustomer? MapTo(
        Shared.Database.Entities.OrganizationStripeCustomer? src,
        Shared.Models.Organization organization) =>
        src is null
            ? null
            : new OrganizationStripeCustomer
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                StripeCustomerId = src.StripeCustomerId,
                Organization = organization
            };

    private static IEnumerable<OrganizationStripePaymentMethod> MapTo(
        IEnumerable<Shared.Database.Entities.OrganizationStripePaymentMethod> src,
        Shared.Models.Organization organization) => src.Select(item => MapTo(item, organization));

    private static OrganizationStripePaymentMethod MapTo(
        Shared.Database.Entities.OrganizationStripePaymentMethod src,
        Shared.Models.Organization organization) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            SetupIntentId = src.SetupIntentId,
            PaymentMethodId = src.PaymentMethodId,
            CardBrand = src.CardBrand,
            CardCountry = src.CardCountry,
            CardDescription = src.CardDescription,
            CardExpiryMonth = src.CardExpiryMonth,
            CardExpiryYear = src.CardExpiryYear,
            CardFingerprint = src.CardFingerprint,
            CardFunding = src.CardFunding,
            CardIssuer = src.CardIssuer,
            CardLastFourDigit = src.CardLastFourDigit,
            Organization = organization
        };

    private static OrganizationSsoSettings? MapTo(Shared.Database.Entities.OrganizationSsoSettings? src) =>
        src is null
            ? null
            : new OrganizationSsoSettings
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                ModifiedAt = src.ModifiedAt,
                IsActive = src.IsActive,
                EntityId = src.EntityId,
                LoginUrl = src.LoginUrl,
                AppFederationMetadataUrl = src.AppFederationMetadataUrl
            };

    private static OrganizationTaxDetails? MapTo(Shared.Database.Entities.OrganizationTaxDetails? src) =>
        src is null
            ? null
            : new OrganizationTaxDetails
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                ModifiedAt = src.ModifiedAt,
                TaxId = src.TaxId,
                TaxRatePercentage = src.TaxRatePercentage
            };

    private static OrganizationStripeConnectAccountAuthorization? MapTo(
        Shared.Database.Entities.OrganizationStripeConnectAccountAuthorization? src) =>
        src is null
            ? null
            : new OrganizationStripeConnectAccountAuthorization
            {
                Id = src.Id, CreatedAt = src.CreatedAt, ModifiedAt = src.ModifiedAt, IsAuthorized = src.IsAuthorized
            };

    private static IEnumerable<Shared.Models.OrganizationStripeConnectAccount> MapTo(
        IEnumerable<OrganizationStripeConnectAccount> src,
        Shared.Models.Organization organization) => src.Select(item => MapTo(item, organization));

    private static Shared.Models.OrganizationStripeConnectAccount MapTo(
        OrganizationStripeConnectAccount src,
        Shared.Models.Organization organization) => new()
    {
        Id = src.Id,
        CreatedAt = src.CreatedAt,
        ModifiedAt = src.ModifiedAt,
        DeletedAt = src.DeletedAt,
        IsDefault = src.IsDefault,
        StripeAccountId = src.StripeAccountId,
        Name = src.Name,
        ChargesEnabled = src.ChargesEnabled,
        PayoutsEnabled = src.PayoutsEnabled,
        Type = src.Type,
        Country = src.Country,
        DefaultCurrency = src.DefaultCurrency,
        BusinessType = src.BusinessType,
        Url = src.Url,
        SupportUrl = src.SupportUrl,
        CompanyName = src.CompanyName,
        ContactEmail = src.ContactEmail,
        ContactPhone = src.ContactPhone,
        DetailsSubmitted = src.DetailsSubmitted,
        CapabilitiesCardPayments = src.CapabilitiesCardPayments,
        CapabilitiesTransfers = src.CapabilitiesTransfers,
        OnboardingUrl = src.OnboardingUrl,
        Organization = organization,
        OrganizationStripeConnectAccountAuthorization = MapTo(src.OrganizationStripeConnectAccountAuthorization)
    };

    private static StripeConnectAccount MapToGrpcResponse(Shared.Models.OrganizationStripeConnectAccount src) =>
        new()
        {
            Id = src.Id,
            IsDefault = src.IsDefault,
            StripeAccountId = src.StripeAccountId.ToSafeString(),
            Name = src.Name.ToSafeString(),
            ChargesEnabled = src.ChargesEnabled,
            PayoutsEnabled = src.PayoutsEnabled,
            Type = src.Type.ToSafeString(),
            Country = src.Country.ToSafeString(),
            DefaultCurrency = src.DefaultCurrency.ToSafeString(),
            BusinessType = src.BusinessType.ToSafeString(),
            CompanyName = src.CompanyName.ToSafeString(),
            Url = src.Url.ToSafeString(),
            SupportUrl = src.SupportUrl.ToSafeString(),
            ContactEmail = src.ContactEmail.ToSafeString(),
            ContactPhone = src.ContactPhone.ToSafeString(),
            DetailsSubmitted = src.DetailsSubmitted,
            CapabilitiesTransfers = src.CapabilitiesTransfers.ToSafeString(),
            CapabilitiesCardPayments = src.CapabilitiesCardPayments.ToSafeString(),
            OnboardingUrl = src.OnboardingUrl.ToSafeString(),
            OnboardingCompleted = src.IsOnboardingCompleted()
        };

    private static OrganizationSsoSettingsDetails? MapTo(OrganizationSsoSettings? src) =>
        src is null
            ? null
            : new OrganizationSsoSettingsDetails
            {
                Id = src.Id,
                IsActive = src.IsActive,
                EntityId = src.EntityId,
                LoginUrl = src.LoginUrl,
                AppFederationMetadataUrl = src.AppFederationMetadataUrl
            };

    private static GraphQL.TaxDetails.OrganizationTaxDetails? MapTo(OrganizationTaxDetails? src) =>
        src is null
            ? null
            : new GraphQL.TaxDetails.OrganizationTaxDetails { Id = src.Id, TaxId = src.TaxId, TaxRatePercentage = src.TaxRatePercentage };

    private static OrganizationPhysicalAddressDetails? MapToGraphQl(Shared.Models.OrganizationPhysicalAddress? src) =>
        src is null
            ? null
            : new OrganizationPhysicalAddressDetails
            {
                Id = src.Id,
                OsmType = src.OsmType,
                OsmId = src.OsmId,
                PlaceId = src.PlaceId,
                Longitude = src.Coordinates?.X,
                Latitude = src.Coordinates?.Y,
                FormattedAddress = src.ToFormattedAddress(),
                MultilinesFormattedAddress = src.ToMultilinesFormattedAddress(),
                AddressLine1 = src.AddressLine1,
                AddressLine2 = src.AddressLine2,
                Suburb = src.Suburb,
                City = src.City,
                Province = src.Province,
                Zipcode = src.Zipcode,
                Country = src.Country
            };

    private static Shared.Models.OrganizationPhysicalAddress? MapTo(OrganizationPhysicalAddress? src, Shared.Models.Organization organization) =>
        src is null
            ? null
            : new Shared.Models.OrganizationPhysicalAddress
            {
                Id = src.Id,
                OsmType = src.OsmType,
                OsmId = src.OsmId,
                PlaceId = src.PlaceId,
                Coordinates = src.Coordinates,
                FormattedAddress = src.FormattedAddress,
                AddressLine1 = src.AddressLine1,
                AddressLine2 = src.AddressLine2,
                Suburb = src.Suburb,
                City = src.City,
                Province = src.Province,
                Zipcode = src.Zipcode,
                Country = src.Country,
                Organization = organization
            };

    private static BankAccount MapToGrpcResponse(Shared.Models.OrganizationBankAccount src) =>
        new()
        {
            Id = src.Id,
            IsDefault = src.IsDefault,
            Name = src.Name.ToSafeString(),
            BankName = src.BankName.ToSafeString(),
            AccountHolderName = src.AccountHolderName.ToSafeString(),
            AccountNumber = src.AccountNumber.ToSafeString(),
            Country = src.Country.ToSafeString()
        };

    private static TaxDetails? MapToGrpcResponse(OrganizationTaxDetails? src) =>
        src is null
            ? null
            : new TaxDetails { Id = src.Id, TaxId = src.TaxId.ToSafeString(), TaxRatePercentage = Convert.ToDouble(src.TaxRatePercentage) };

    private static PhysicalAddress? MapToGrpcResponse(Shared.Models.OrganizationPhysicalAddress? src) =>
        src is null
            ? null
            : new PhysicalAddress
            {
                Id = src.Id,
                AddressLine1 = src.AddressLine1.ToSafeString(),
                AddressLine2 = src.AddressLine2.ToSafeString(),
                Suburb = src.Suburb.ToSafeString(),
                City = src.City.ToSafeString(),
                Province = src.Province.ToSafeString(),
                Zipcode = src.Zipcode.ToSafeString(),
                Country = src.Country.ToSafeString(),
                FormattedAddress = src.ToFormattedAddress(),
                OsmType = src.OsmType.ToSafeString(),
                OsmId = src.OsmId.ToSafeString(),
                PlaceId = src.PlaceId.ToSafeString(),
                Coordinates = src.Coordinates is null ? null : new Coordinates { Longitude = src.Coordinates.X, Latitude = src.Coordinates.Y }
            };
}
