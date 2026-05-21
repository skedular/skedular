using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Enterprise.Shared;
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
using Organization.Api.GraphQL.Xero;
using Organization.Shared.Models;
using Stripe;
using AddCustomTagInput = Organization.Api.GraphQL.Tag.AddCustomTagInput;
using AddProductTagInput = Organization.Api.GraphQL.Tag.AddProductTagInput;
using AzureTenant = Organization.Shared.Models.AzureTenant;
using AzureTenantMember = Organization.Shared.Models.AzureTenantMember;
using Customer = Organization.Shared.Models.Customer;
using DailyMemberCountRecording = Organization.Shared.Models.DailyMemberCountRecording;
using Identity = Organization.Shared.Models.Identity;
using IndustryMainCategory = Organization.Shared.Models.IndustryMainCategory;
using IndustrySubCategory = Organization.Shared.Models.IndustrySubCategory;
using JoinInvitation = Organization.Shared.Models.JoinInvitation;
using ListingMetadata = Api.Shared.Services.Models.ListingMetadata;
using Offering = Api.Shared.Services.Offering.Offering;
using OrganizationDailyBookingsTotal = Organization.Shared.Models.OrganizationDailyBookingsTotal;
using OrganizationMember = Organization.Shared.Models.OrganizationMember;
using OrganizationMemberAttendancePercentage = Organization.Shared.Models.OrganizationMemberAttendancePercentage;
using OrganizationOffering = Organization.Shared.Models.OrganizationOffering;
using Tag = Organization.Shared.Models.Tag;
using TermsOfUse = Organization.Shared.Database.Entities.TermsOfUse;
using OrganizationBankAccount = Organization.Shared.Database.Entities.OrganizationBankAccount;
using OrganizationBankAccountPatchRequest = Organization.Api.Models.OrganizationBankAccountPatchRequest;
using OrganizationBillingDetails = Organization.Shared.Database.Entities.OrganizationBillingDetails;
using OrganizationDetails = Organization.Api.GraphQL.Organization.OrganizationDetails;
using OrganizationSsoSettings = Organization.Shared.Models.OrganizationSsoSettings;
using OrganizationStripeConnectAccountPatchRequest = Organization.Api.Models.OrganizationStripeConnectAccountPatchRequest;
using OrganizationTaxDetails = Organization.Shared.Models.OrganizationTaxDetails;
using OrganizationStripeConnectAccount = Organization.Shared.Database.Entities.OrganizationStripeConnectAccount;
using OrganizationStripeConnectAccountAuthorization = Organization.Shared.Models.OrganizationStripeConnectAccountAuthorization;
using OrganizationStripeCustomer = Organization.Shared.Models.OrganizationStripeCustomer;
using OrganizationStripePaymentMethod = Organization.Shared.Models.OrganizationStripePaymentMethod;
using OrganizationXeroConnection = Organization.Shared.Models.OrganizationXeroConnection;
using UpdateOrganizationBillingDetailsInput = Organization.Api.GraphQL.Billing.UpdateOrganizationBillingDetailsInput;
using OrganizationPhysicalAddress = Organization.Shared.Database.Entities.OrganizationPhysicalAddress;
using OrganizationPatchRequest = Organization.Api.Models.OrganizationPatchRequest;
using OrganizationBillingDetailsPatchRequest = Organization.Api.Models.OrganizationBillingDetailsPatchRequest;
using OrganizationSsoSettingsPatchRequest = Organization.Api.Models.OrganizationSsoSettingsPatchRequest;
using OrganizationTaxDetailsPatchRequest = Organization.Api.Models.OrganizationTaxDetailsPatchRequest;
using OrganizationXeroConnectionPatchRequest = Organization.Api.Models.OrganizationXeroConnectionPatchRequest;


namespace Organization.Api.Mappers;

public interface IGraphQlMapper
{
    IEnumerable<Shared.Models.Organization> MapTo(IEnumerable<Shared.Database.Entities.Organization> src);
    Shared.Models.Organization MapTo(Shared.Database.Entities.Organization src, Uri stripeAuthorizeExistingConnectAccountUrl);
    OrganizationMember MapTo(Shared.Database.Entities.OrganizationMember src, Shared.Models.Organization organization);
    JoinInvitation MapTo(Shared.Database.Entities.JoinInvitation src);

    Shared.Database.Entities.Organization MapTo(
        Shared.Models.Organization src,
        TermsOfUse termsOfUse,
        IReadOnlyList<Shared.Database.Entities.IndustrySubCategory> industrySubCategories);

    Shared.Database.Entities.Organization MergeTo(
        Shared.Models.Organization src,
        Shared.Database.Entities.Organization dest,
        IReadOnlyList<Shared.Database.Entities.IndustrySubCategory> industrySubCategories);

    Shared.Models.TermsOfUse? MapTo(TermsOfUse? src);
    Customer? MapTo(Shared.Database.Entities.Customer? src);
    IEnumerable<IndustryMainCategory> MapTo(IEnumerable<Shared.Database.Entities.IndustryMainCategory> src);
    OrganizationTermsOfUse? MapTo(Shared.Models.TermsOfUse? src);
    IEnumerable<OrganizationIndustryMainCategoryReferenceDetails> MapTo(IEnumerable<IndustryMainCategory> src);
    IEnumerable<MyOrganizationDetails> MapTo(IEnumerable<Shared.Models.Organization> src);
    OrganizationMemberDetails MapTo(OrganizationMember src);
    OrganizationDetails? MapTo(Shared.Models.Organization? src);
    OrganizationPublicDetails? MapToPublic(Shared.Models.Organization? src);

    OrganizationAnalytics MapTo(
        IEnumerable<OrganizationMemberAttendancePercentage> organizationMemberAttendancePercentages,
        IEnumerable<OrganizationDailyBookingsTotal> organizationDailyBookingsTotals);

    Shared.Models.Organization MapTo(AddOrganizationInput src);
    OrganizationPatchRequest MapTo(UpdateOrganizationInput src);

    Shared.Database.Entities.OrganizationMember MapToEntity(
        OrganizationMember src,
        Shared.Database.Entities.Organization organization,
        Shared.Database.Entities.Customer customer);

    Shared.Database.Entities.OrganizationMember MergeToEntity(
        OrganizationMember src,
        Shared.Database.Entities.OrganizationMember dest,
        Shared.Database.Entities.Organization organization,
        Shared.Database.Entities.Customer customer);

    OrganizationEdge MapTo(Edge<Shared.Models.Organization> src);

    IEnumerable<Edge<OrganizationMember>> MapTo(
        IEnumerable<Edge<Shared.Database.Entities.OrganizationMember>> src,
        Shared.Models.Organization organization);

    OrganizationMemberEdge MapTo(Edge<OrganizationMember> src);
    Tag MapTo(Shared.Database.Entities.Tag src);
    Shared.Database.Entities.Tag MapTo(Tag src, Shared.Database.Entities.Organization organization);
    Shared.Database.Entities.Tag MergeTo(Tag src, Shared.Database.Entities.Tag dest, Shared.Database.Entities.Organization organization);
    IEnumerable<Edge<Tag>> MapTo(IEnumerable<Edge<Shared.Database.Entities.Tag>> src, Shared.Models.Organization organization);
    Tag MapTo(AddCustomTagInput src);
    Tag MapTo(AddZoneInput src);
    OrganizationTagDetails? MapTo(Tag? src);
    OrganizationTagEdge MapTo(Edge<Tag> src);
    IEnumerable<string> MapTo(Offering offering);
    OrganizationSsoSettingsPatchRequest MapTo(UpdateOrganizationSsoSettingsInput src);
    Shared.Database.Entities.OrganizationSsoSettings MapToEntity(OrganizationSsoSettings src, Shared.Database.Entities.Organization organization);

    Shared.Database.Entities.OrganizationSsoSettings MergeToEntity(
        OrganizationSsoSettings src,
        Shared.Database.Entities.OrganizationSsoSettings dest,
        Shared.Database.Entities.Organization organization);

    Tag MapTo(AddProductTagInput src);
    OrganizationBillingDetails MapTo(Shared.Models.OrganizationBillingDetails src, Shared.Database.Entities.Organization organization);

    OrganizationBillingDetails MergeToEntity(
        Shared.Models.OrganizationBillingDetails src,
        OrganizationBillingDetails dest,
        Shared.Database.Entities.Organization organization);

    CustomerCreateOptions MapToStripeCustomerCreateOption(Shared.Database.Entities.Organization src);
    OrganizationBillingDetailsPatchRequest MapTo(UpdateOrganizationBillingDetailsInput src);
    Shared.Models.OrganizationBillingDetails? MapTo(OrganizationBillingDetails? src);
    AccountCreateOptions MapToStripeAccountRequest(Shared.Database.Entities.Organization src);
    OrganizationStripeConnectAccount MapTo(Account src, string id, string name, bool isDefault, Shared.Database.Entities.Organization organization);
    Shared.Models.OrganizationStripeConnectAccount MapTo(OrganizationStripeConnectAccount src);
    OrganizationStripeConnectAccountDetails? MapTo(Shared.Models.OrganizationStripeConnectAccount? src);
    OrganizationStripeConnectAccountEdge MapTo(Edge<Shared.Models.OrganizationStripeConnectAccount> src);
    OrganizationStripeConnectAccountPatchRequest MapTo(UpdateOrganizationStripeConnectAccountInput src);
    OrganizationBankAccount MapTo(Shared.Models.OrganizationBankAccount src, Shared.Database.Entities.Organization organization);

    OrganizationBankAccount MergeTo(
        Shared.Models.OrganizationBankAccount src,
        OrganizationBankAccount dest,
        Shared.Database.Entities.Organization organization);

    Shared.Models.OrganizationBankAccount MapTo(OrganizationBankAccount src);
    Shared.Models.OrganizationBankAccount MapTo(AddOrganizationBankAccountInput src);
    OrganizationBankAccountPatchRequest MapTo(UpdateOrganizationBankAccountInput src);
    OrganizationBankAccountDetails? MapTo(Shared.Models.OrganizationBankAccount? src);
    OrganizationBankAccountEdge MapTo(Edge<Shared.Models.OrganizationBankAccount> src);
    OrganizationTaxDetailsPatchRequest MapTo(UpdateOrganizationTaxDetailsInput src);
    Shared.Database.Entities.OrganizationTaxDetails MapToEntity(OrganizationTaxDetails src, Shared.Database.Entities.Organization organization);

    Shared.Database.Entities.OrganizationTaxDetails MergeToEntity(
        OrganizationTaxDetails src,
        Shared.Database.Entities.OrganizationTaxDetails dest,
        Shared.Database.Entities.Organization organization);

    OrganizationXeroConnectionPatchRequest MapTo(UpdateOrganizationXeroConnectionInput src);

    OrganizationXeroConnectionDetails? MapTo(OrganizationXeroConnection? src);
    OrganizationPhysicalAddress MapTo(Shared.Models.OrganizationPhysicalAddress src, Shared.Database.Entities.Organization organization);

    OrganizationPhysicalAddress MergeTo(
        Shared.Models.OrganizationPhysicalAddress src,
        OrganizationPhysicalAddress dest,
        Shared.Database.Entities.Organization organization);

    IEnumerable<InviteCustomerToJoinOrganizationDetails> MapTo(IEnumerable<JoinInvitation> src);
    InviteCustomerToJoinOrganizationDetails MapTo(JoinInvitation src);
    Edge<JoinInvitation> MapTo(Edge<Shared.Database.Entities.JoinInvitation> src);
    OrganizationJoinInvitationEdge MapTo(Edge<JoinInvitation> src);
    OrganizationStripeConnectAccount MergeTo(Account src, OrganizationStripeConnectAccount dest);
}

public class GraphQlMapper : IGraphQlMapper
{
    public IEnumerable<Shared.Models.Organization> MapTo(IEnumerable<Shared.Database.Entities.Organization> src) =>
        src.Select(item => MapTo(item, Constants.EmptyUri));

    public Shared.Models.Organization MapTo(Shared.Database.Entities.Organization src, Uri stripeAuthorizeExistingConnectAccountUrl)
    {
        var organization = new Shared.Models.Organization
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            CustomDomain = src.CustomDomain,
            Name = src.Name,
            ListingMetadata = src.ListingMetadata ?? ListingMetadata.Empty,
            MarketplaceListingMetadata = src.MarketplaceListingMetadata ?? ListingMetadata.Empty,
            Website = src.Website,
            CustomerFacingTermsAndConditionsUrl = src.CustomerFacingTermsAndConditionsUrl,
            AgreedToTermsOfUse = src.AgreedToTermsOfUse,
            LogoUrl = src.LogoUrl,
            Type = src.Type.ToOrganizationType(),
            BillingCycle = src.BillingCycle.ToOrganizationBillingCycle(),
            InvoiceDueInDays = src.InvoiceDueInDays,
            ContactEmail = src.ContactEmail,
            ContactPhone = src.ContactPhone,
            RefundNotificationEmails = src.RefundNotificationEmails.ToSafeCollection(),
            IsOwnershipVerified = src.IsOwnershipVerified,
            FeatureImages = src.FeatureImages.ToSafeCollection(),
            StripeAuthorizeExistingConnectAccountUrl = stripeAuthorizeExistingConnectAccountUrl,
            TermsOfUse = MapTo(src.TermsOfUse),
            IndustrySubCategories = MapTo(src.IndustrySubCategories, null).ToList(),
            OrganizationSsoSettings = MapTo(src.OrganizationSsoSettings),
            OrganizationTaxDetails = MapTo(src.OrganizationTaxDetails),
            OrganizationXeroConnection = MapTo(src.OrganizationXeroConnection)
        };

        organization.OrganizationMembers = MapTo(src.OrganizationMembers, organization).ToList();
        organization.OrganizationOfferings = MapTo(src.OrganizationOfferings, organization).ToList();
        organization.DailyMemberCountRecordings = MapTo(src.DailyMemberCountRecordings, organization).ToList();
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
        IReadOnlyList<Shared.Database.Entities.IndustrySubCategory> industrySubCategories) =>
        new()
        {
            Id = src.Id,
            CustomDomain = src.CustomDomain,
            Name = src.Name,
            ListingMetadata = src.ListingMetadata,
            MarketplaceListingMetadata = src.MarketplaceListingMetadata,
            Website = src.Website,
            CustomerFacingTermsAndConditionsUrl = src.CustomerFacingTermsAndConditionsUrl,
            AgreedToTermsOfUse = src.AgreedToTermsOfUse,
            LogoUrl = src.LogoUrl,
            Type = src.Type.ToOrganizationType(),
            BillingCycle = src.BillingCycle.ToOrganizationBillingCycle(),
            InvoiceDueInDays = src.InvoiceDueInDays,
            ContactEmail = src.ContactEmail,
            ContactPhone = src.ContactPhone,
            RefundNotificationEmails = src.RefundNotificationEmails.ToList(),
            IsOwnershipVerified = src.IsOwnershipVerified,
            FeatureImages = src.FeatureImages.ToList(),
            TermsOfUse = termsOfUse,
            IndustrySubCategories = industrySubCategories.ToList()
        };

    public Shared.Database.Entities.Organization MergeTo(
        Shared.Models.Organization src,
        Shared.Database.Entities.Organization dest,
        IReadOnlyList<Shared.Database.Entities.IndustrySubCategory> industrySubCategories)
    {
        dest.Id = src.Id;
        dest.CustomDomain = src.CustomDomain;
        dest.Name = src.Name;
        dest.ListingMetadata = src.ListingMetadata;
        dest.MarketplaceListingMetadata = src.MarketplaceListingMetadata;
        dest.Website = src.Website;
        dest.CustomerFacingTermsAndConditionsUrl = src.CustomerFacingTermsAndConditionsUrl;
        dest.AgreedToTermsOfUse = src.AgreedToTermsOfUse;
        dest.LogoUrl = src.LogoUrl;
        dest.Type = src.Type.ToOrganizationType();
        dest.BillingCycle = src.BillingCycle.ToOrganizationBillingCycle();
        dest.InvoiceDueInDays = src.InvoiceDueInDays;
        dest.ContactEmail = src.ContactEmail;
        dest.ContactPhone = src.ContactPhone;
        dest.RefundNotificationEmails = src.RefundNotificationEmails.ToList();
        dest.IsOwnershipVerified = src.IsOwnershipVerified;
        dest.FeatureImages = src.FeatureImages.ToList();
        dest.IndustrySubCategories = industrySubCategories.ToList();
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
                Type = src.Type.ToNullableCustomerType(),
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

    public IEnumerable<MyOrganizationDetails> MapTo(IEnumerable<Shared.Models.Organization> src) => src.Select(MapToMyOrganizationDetails);

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
            CustomDomain = src.CustomDomain,
            Name = src.Name,
            ListingMetadata = src.ListingMetadata,
            MarketplaceListingMetadata = src.MarketplaceListingMetadata,
            Website = src.Website,
            CustomerFacingTermsAndConditionsUrl = src.CustomerFacingTermsAndConditionsUrl,
            AgreedToTermsOfUse = src.AgreedToTermsOfUse,
            LogoUrl = src.LogoUrl,
            Type = new OrganizationTypeDetails { Type = src.Type, Name = src.Type.ToOrganizationTypeName() },
            ContactEmail = src.ContactEmail,
            ContactPhone = src.ContactPhone,
            RefundNotificationEmails = src.RefundNotificationEmails,
            IsOwnershipVerified = src.IsOwnershipVerified ?? false,
            FeatureImages = src.FeatureImages,
            StripeAuthorizeExistingConnectAccountUrl = src.StripeAuthorizeExistingConnectAccountUrl.ToString(),
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
            IsMyOnboardingDone = src.IsMyOnboardingDone,
            ResourceTypes = src.Tags
                .Where(item => OrganizationTagTypeConstants.ResourceTypes.Any(resourceType => resourceType == item.Type))
                .Select(item => MapTo(item)!),
            LocationSpaceTypes = src.Tags
                .Where(item => OrganizationTagTypeConstants.LocationSpaceTypes.Any(resourceType => resourceType == item.Type))
                .Select(item => MapTo(item)!),
            Amenities = src.Tags
                .Where(item => OrganizationTagTypeConstants.Amenities.Any(resourceType => resourceType == item.Type))
                .Select(item => MapTo(item)!),
            SsoSettings = MapTo(src.OrganizationSsoSettings),
            TaxDetails = MapTo(src.OrganizationTaxDetails),
            XeroConnection = MapTo(src.OrganizationXeroConnection),
            BillingCycle = new OrganizationBillingCycleDetails { Type = src.BillingCycle, Name = src.BillingCycle.ToOrganizationBillingCycleName() },
            InvoiceDueInDays = src.InvoiceDueInDays
        };
    }

    public OrganizationPublicDetails? MapToPublic(Shared.Models.Organization? src) =>
        src is null
            ? null
            : new OrganizationPublicDetails
            {
                Id = src.Id,
                CustomDomain = src.CustomDomain,
                Name = src.Name,
                ListingMetadata = src.ListingMetadata,
                MarketplaceListingMetadata = src.MarketplaceListingMetadata,
                Website = src.Website,
                CustomerFacingTermsAndConditionsUrl = src.CustomerFacingTermsAndConditionsUrl,
                LogoUrl = src.LogoUrl,
                ContactEmail = src.ContactEmail,
                ContactPhone = src.ContactPhone,
                FeatureImages = src.FeatureImages,
                IndustrySubCategories = src.IndustrySubCategories.Select(item => MapTo(item, null)),
                PhysicalAddress = MapToGraphQl(src.PhysicalAddress),
                ResourceTypes = src.Tags
                    .Where(item => OrganizationTagTypeConstants.ResourceTypes.Any(resourceType => resourceType == item.Type))
                    .Select(item => MapTo(item)!),
                LocationSpaceTypes = src.Tags
                    .Where(item => OrganizationTagTypeConstants.LocationSpaceTypes.Any(resourceType => resourceType == item.Type))
                    .Select(item => MapTo(item)!),
                Amenities = src.Tags
                    .Where(item => OrganizationTagTypeConstants.Amenities.Any(resourceType => resourceType == item.Type))
                    .Select(item => MapTo(item)!)
            };

    public OrganizationMemberDetails MapTo(OrganizationMember src) =>
        new()
        {
            Id = src.Id,
            Role = new OrganizationMemberRoleDetails { Type = src.Role, Name = src.Role.ToOrganizationMemberRoleName() },
            Status = new OrganizationMemberStatusDetails { Type = src.Status, Name = src.Status.ToOrganizationMemberStatusName() },
            IsOrganizationOnboardingDone = src.IsOrganizationOnboardingDone ?? false,
            CustomerId = src.Customer.Id
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
            CustomDomain = src.CustomDomain.ToSafeString(),
            Name = src.Name,
            ListingMetadata = src.ListingMetadata ?? ListingMetadata.Empty,
            MarketplaceListingMetadata = src.MarketplaceListingMetadata ?? ListingMetadata.Empty,
            Website = src.Website,
            LogoUrl = src.LogoUrl,
            CustomerFacingTermsAndConditionsUrl = src.CustomerFacingTermsAndConditionsUrl,
            Type = src.Type,
            BillingCycle = src.BillingCycle,
            InvoiceDueInDays = src.InvoiceDueInDays,
            ContactEmail = src.ContactEmail,
            ContactPhone = src.ContactPhone,
            RefundNotificationEmails = src.RefundNotificationEmails.ToSafeCollection(),
            FeatureImages = src.FeatureImages.ToSafeCollection(),
            AgreedToTermsOfUse = src.AgreedToTermsOfUse,
            IndustrySubCategories = src.IndustrySubCategoryIds.Select(item => new IndustrySubCategory { Id = item }).ToList(),
            TermsOfUse = new Shared.Models.TermsOfUse { Id = src.TermsOfUseId }
        };

    public OrganizationPatchRequest MapTo(UpdateOrganizationInput src) =>
        new(
            src.Id.ToSafeString(),
            src.CustomDomain.ToSafeString(),
            src.FieldsToUpdate.ToHashSet(),
            src.Name,
            src.Description,
            src.Title,
            src.SubTitle,
            src.Website,
            src.LogoUrl,
            src.CustomerFacingTermsAndConditionsUrl,
            src.BillingCycle,
            src.InvoiceDueInDays,
            src.ContactEmail,
            src.ContactPhone,
            src.RefundNotificationEmails.ToSafeCollection(),
            src.IndustrySubCategoryIds.ToSafeCollection(),
            src.FeatureImages.ToSafeCollection(),
            src.MarketplaceListingMetadata,
            MapTo(src.PhysicalAddress));

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

    public OrganizationEdge MapTo(Edge<Shared.Models.Organization> src) => new(MapTo(src.Node)!, src.Cursor);

    public IEnumerable<Edge<OrganizationMember>> MapTo(
        IEnumerable<Edge<Shared.Database.Entities.OrganizationMember>> src,
        Shared.Models.Organization organization) =>
        src.Select(item => MapTo(item, organization));

    public OrganizationMemberEdge MapTo(Edge<OrganizationMember> src) => new(MapTo(src.Node), src.Cursor);

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
            Organization = new Shared.Models.Organization
            {
                Id = src.OrganizationId.ToSafeString(), CustomDomain = src.OrganizationCustomDomain.ToSafeString()
            },
            Type = OrganizationTagType.Custom,
            Color = src.Color
        };

    public Tag MapTo(AddZoneInput src) =>
        new()
        {
            Id = src.Id.ToSafeString(),
            Name = src.Name,
            Description = src.Description,
            Organization = new Shared.Models.Organization
            {
                Id = src.OrganizationId.ToSafeString(), CustomDomain = src.OrganizationCustomDomain.ToSafeString()
            },
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
                Type = src.Type,
                Color = src.Color
            };

    public OrganizationTagEdge MapTo(Edge<Tag> src) => new(MapTo(src.Node)!, src.Cursor);

    public IEnumerable<string> MapTo(Offering offering) => offering.FeatureSets.Select(MapTo);

    public OrganizationSsoSettingsPatchRequest MapTo(UpdateOrganizationSsoSettingsInput src)
    {
        var organization = new Shared.Models.Organization
        {
            Id = src.OrganizationId.ToSafeString(), CustomDomain = src.OrganizationCustomDomain.ToSafeString()
        };

        return new OrganizationSsoSettingsPatchRequest(
            src.OrganizationId,
            src.OrganizationCustomDomain,
            src.FieldsToUpdate.ToHashSet(),
            new OrganizationSsoSettings
            {
                IsActive = src.IsActive,
                EntityId = src.EntityId,
                LoginUrl = src.LoginUrl,
                AppFederationMetadataUrl = src.AppFederationMetadataUrl,
                Organization = organization
            });
    }

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
            Organization = new Shared.Models.Organization
            {
                Id = src.OrganizationId.ToSafeString(), CustomDomain = src.OrganizationCustomDomain.ToSafeString()
            },
            Type = OrganizationTagType.Product,
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
        dest.CountryCode = src.CountryCode;
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

    public OrganizationBillingDetailsPatchRequest MapTo(UpdateOrganizationBillingDetailsInput src) =>
        new(
            src.OrganizationId,
            src.OrganizationCustomDomain,
            src.FieldsToUpdate,
            src.CompanyName,
            src.Email,
            src.OsmType,
            src.OsmId,
            src.PlaceId,
            src.Longitude,
            src.Latitude,
            src.FormattedAddress,
            src.AddressLine1,
            src.AddressLine2,
            src.Suburb,
            src.City,
            src.Province,
            src.Zipcode,
            src.Country,
            src.CountryCode);

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
                Country = src.Country,
                CountryCode = src.CountryCode
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

    public OrganizationStripeConnectAccountPatchRequest MapTo(UpdateOrganizationStripeConnectAccountInput src) =>
        new(src.Id, src.FieldsToUpdate, src.Name);

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
            Organization = new Shared.Models.Organization
            {
                Id = src.OrganizationId.ToSafeString(), CustomDomain = src.OrganizationCustomDomain.ToSafeString()
            }
        };

    public OrganizationBankAccountPatchRequest MapTo(UpdateOrganizationBankAccountInput src) =>
        new(src.Id, src.FieldsToUpdate, src.Name, src.BankName, src.AccountHolderName, src.AccountNumber, src.Country);

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

    public OrganizationTaxDetailsPatchRequest MapTo(UpdateOrganizationTaxDetailsInput src) =>
        new(
            src.OrganizationId.ToSafeString(),
            src.OrganizationCustomDomain.ToSafeString(),
            src.FieldsToUpdate.ToHashSet(),
            src.TaxId,
            src.TaxRatePercentage);

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

    public OrganizationXeroConnectionPatchRequest MapTo(UpdateOrganizationXeroConnectionInput src) =>
        new(
            src.OrganizationId.ToSafeString(),
            src.OrganizationCustomDomain.ToSafeString(),
            src.FieldsToUpdate,
            src.TenantId,
            src.TenantName,
            src.BillingMode,
            src.Scopes,
            src.IsActive,
            src.SendInvoicesViaXero,
            src.AutoReconcilePayments,
            src.DefaultSalesAccountCode,
            src.DefaultReceivablesAccountCode,
            src.DefaultTrackingCategory1,
            src.DefaultTrackingCategory2,
            src.DefaultBrandingThemeId,
            src.DefaultReferencePrefix);

    public OrganizationXeroConnectionDetails? MapTo(OrganizationXeroConnection? src) =>
        src is null
            ? null
            : new OrganizationXeroConnectionDetails
            {
                Id = src.Id,
                TenantId = src.TenantId,
                TenantName = src.TenantName,
                BillingMode = src.BillingMode,
                Scopes = src.Scopes,
                IsActive = src.IsActive,
                SendInvoicesViaXero = src.SendInvoicesViaXero,
                AutoReconcilePayments = src.AutoReconcilePayments,
                DefaultSalesAccountCode = src.DefaultSalesAccountCode,
                DefaultReceivablesAccountCode = src.DefaultReceivablesAccountCode,
                DefaultTrackingCategory1 = src.DefaultTrackingCategory1,
                DefaultTrackingCategory2 = src.DefaultTrackingCategory2,
                DefaultBrandingThemeId = src.DefaultBrandingThemeId,
                DefaultReferencePrefix = src.DefaultReferencePrefix,
                AccessTokenExpiresAt = src.AccessTokenExpiresAt,
                RefreshTokenExpiresAt = src.RefreshTokenExpiresAt,
                LastSuccessfulSyncAt = src.LastSuccessfulSyncAt,
                LastError = src.LastError,
                HasAccessToken = src.HasAccessToken,
                HasRefreshToken = src.HasRefreshToken
            };

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
        dest.CountryCode = src.CountryCode;
        dest.Organization = organization;
        return dest;
    }

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
            CreatedById = src.CreatedBy.Id,
            InviteeId = src.Invitee?.Id
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

    private static Shared.Models.OrganizationPhysicalAddress? MapTo(OrganizationPhysicalAddressPatchInput? src) =>
        src is null
            ? null
            : new Shared.Models.OrganizationPhysicalAddress
            {
                OsmType = src.OsmType,
                OsmId = src.OsmId,
                PlaceId = src.PlaceId,
                Coordinates =
                    src.Longitude is null || src.Latitude is null ? null : new Point(new Coordinate(src.Longitude.Value, src.Latitude.Value)),
                FormattedAddress = src.FormattedAddress,
                AddressLine1 = src.AddressLine1,
                AddressLine2 = src.AddressLine2,
                Suburb = src.Suburb,
                City = src.City,
                Province = src.Province,
                Zipcode = src.Zipcode,
                Country = src.Country,
                CountryCode = src.CountryCode
            };

    private IEnumerable<OrganizationMember> MapTo(
        IEnumerable<Shared.Database.Entities.OrganizationMember> src,
        Shared.Models.Organization organization) =>
        src.Select(item => MapTo(item, organization));

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

    private IEnumerable<JoinInvitation> MapTo(IEnumerable<Shared.Database.Entities.JoinInvitation> src, Shared.Models.Organization organization) =>
        src.Select(item => MapTo(item, organization));

    private JoinInvitation MapTo(Shared.Database.Entities.JoinInvitation src, Shared.Models.Organization organization) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
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
                CountryCode = src.CountryCode,
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
                Country = src.Country,
                CountryCode = src.CountryCode
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

    private static OrganizationXeroConnection? MapTo(Shared.Database.Entities.OrganizationXeroConnection? src) =>
        src is null
            ? null
            : new OrganizationXeroConnection
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                ModifiedAt = src.ModifiedAt,
                TenantId = src.TenantId,
                TenantName = src.TenantName,
                BillingMode = src.BillingMode.ToOrganizationXeroBillingMode(),
                Scopes = src.Scopes,
                IsActive = src.IsActive,
                SendInvoicesViaXero = src.SendInvoicesViaXero,
                AutoReconcilePayments = src.AutoReconcilePayments,
                DefaultSalesAccountCode = src.DefaultSalesAccountCode,
                DefaultReceivablesAccountCode = src.DefaultReceivablesAccountCode,
                DefaultTrackingCategory1 = src.DefaultTrackingCategory1,
                DefaultTrackingCategory2 = src.DefaultTrackingCategory2,
                DefaultBrandingThemeId = src.DefaultBrandingThemeId,
                DefaultReferencePrefix = src.DefaultReferencePrefix,
                AccessTokenExpiresAt = src.AccessTokenExpiresAt,
                RefreshTokenExpiresAt = src.RefreshTokenExpiresAt,
                LastSuccessfulSyncAt = src.LastSuccessfulSyncAt,
                LastError = src.LastError,
                AccessTokenEncrypted = src.AccessTokenEncrypted,
                RefreshTokenEncrypted = src.RefreshTokenEncrypted,
                HasAccessToken = !string.IsNullOrWhiteSpace(src.AccessTokenEncrypted),
                HasRefreshToken = !string.IsNullOrWhiteSpace(src.RefreshTokenEncrypted)
            };

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
                Country = src.Country,
                CountryCode = src.CountryCode
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
                CountryCode = src.CountryCode,
                Organization = organization
            };

    private static MyOrganizationDetails MapToMyOrganizationDetails(Shared.Models.Organization src) =>
        new()
        {
            Id = src.Id,
            CustomDomain = src.CustomDomain,
            Name = src.Name,
            ListingMetadata = src.ListingMetadata,
            Website = src.Website,
            CustomerFacingTermsAndConditionsUrl = src.CustomerFacingTermsAndConditionsUrl,
            LogoUrl = src.LogoUrl,
            Type = new OrganizationTypeDetails { Type = src.Type, Name = src.Type.ToOrganizationTypeName() },
            ContactEmail = src.ContactEmail,
            ContactPhone = src.ContactPhone,
            FeatureImages = src.FeatureImages,
            IsMyOnboardingDone = src.IsMyOnboardingDone
        };
}
