using Api.Shared.Grpc.Skedular.Customer.Admin.V1;
using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Enterprise.Shared;
using Microsoft.Graph.Models;
using Organization.Shared.Models;
using Organization.Shared.Models.PricingCatalog;
using Organization.Shared.Services.Pricing;
using AzureTenant = Organization.Shared.Database.Entities.AzureTenant;
using Customer = Organization.Shared.Models.Customer;
using CustomerType = Api.Shared.Grpc.Skedular.Customer.Core.V1.CustomerType;
using Identity = Organization.Shared.Models.Identity;
using OrganizationStripePaymentMethod = Organization.Shared.Database.Entities.OrganizationStripePaymentMethod;
using PaymentMethod = Stripe.PaymentMethod;
using PersonalInformationVisibility = Api.Shared.Grpc.Skedular.Customer.Core.V1.PersonalInformationVisibility;
using ListingMetadata = Api.Shared.Services.Models.ListingMetadata;
using OrganizationMember = Organization.Shared.Database.Entities.OrganizationMember;
using OrganizationSpacesSubscription = Organization.Shared.Models.PricingCatalog.OrganizationSpacesSubscription;
using OrganizationXeroConnection = Organization.Shared.Models.OrganizationXeroConnection;

namespace Organization.Shared.Mappers;

public interface IEntityMapper
{
    OrganizationStripePaymentMethod MapTo(PaymentMethod paymentMethod, string setupIntentId, Database.Entities.Organization organization);
    Models.Organization MapTo(Database.Entities.Organization src);
    OrganizationOffering MapTo(Database.Entities.OrganizationOffering src, Models.Organization organization);
    AzureTenantMember MapTo(User src);
    Database.Entities.AzureTenantMember MapTo(AzureTenantMember src, AzureTenant azureTenant);
    Database.Entities.AzureTenantMember MergeToEntity(AzureTenantMember src, Database.Entities.AzureTenantMember dest, AzureTenant azureTenant);
    Admin_AddIdentityInput MapTo(Database.Entities.AzureTenantMember src, string customerId);
    Admin_UpdateIdentityInput MapToUpdateIdentityInput(Database.Entities.AzureTenantMember src, string customerId);
    Admin_AddInput MapTo(Database.Entities.AzureTenantMember src, string customerId, Database.Entities.Organization defaultOrganization);

    OrganizationMember MapToEntity(
        Models.OrganizationMember src,
        Database.Entities.Organization organization,
        Database.Entities.Customer customer);
}

public class EntityMapper : IEntityMapper
{
    public OrganizationStripePaymentMethod MapTo(PaymentMethod paymentMethod, string setupIntentId, Database.Entities.Organization organization) =>
        new()
        {
            SetupIntentId = setupIntentId,
            PaymentMethodId = paymentMethod.Id,
            CardBrand = paymentMethod.Card?.Brand,
            CardCountry = paymentMethod.Card?.Country,
            CardDescription = paymentMethod.Card?.Description,
            CardExpiryMonth = paymentMethod.Card is null ? null : (byte)paymentMethod.Card.ExpMonth,
            CardExpiryYear = paymentMethod.Card is null ? null : (short)paymentMethod.Card.ExpYear,
            CardFingerprint = paymentMethod.Card?.Fingerprint,
            CardFunding = paymentMethod.Card?.Funding,
            CardIssuer = paymentMethod.Card?.Issuer,
            CardLastFourDigit = paymentMethod.Card?.Last4,
            Organization = organization,
        };

    public Models.Organization MapTo(Database.Entities.Organization src) => MapTo(src, true);

    public AzureTenantMember MapTo(User src) =>
        new()
        {
            Id = src.Id!,
            Email = src.Mail,
            Designation = src.JobTitle,
            Name = src.DisplayName,
            GivenName = src.GivenName,
            FamilyName = src.Surname,
            PreferredLanguage = src.PreferredLanguage,
        };

    public Database.Entities.AzureTenantMember MapTo(AzureTenantMember src, AzureTenant azureTenant) =>
        MergeToEntity(src, new Database.Entities.AzureTenantMember(), azureTenant);

    public Database.Entities.AzureTenantMember MergeToEntity(AzureTenantMember src, Database.Entities.AzureTenantMember dest, AzureTenant azureTenant)
    {
        dest.Id = src.Id;
        dest.Email = src.Email;
        dest.Designation = src.Designation;
        dest.Name = src.Name;
        dest.GivenName = src.GivenName;
        dest.FamilyName = src.FamilyName;
        dest.PreferredLanguage = src.PreferredLanguage;
        dest.PhotoUrl = src.PhotoUrl;
        dest.PhotoUrl48 = src.PhotoUrl48;
        dest.PhotoUrl64 = src.PhotoUrl64;
        dest.PhotoUrl96 = src.PhotoUrl96;
        dest.PhotoUrl120 = src.PhotoUrl120;
        dest.PhotoUrl240 = src.PhotoUrl240;
        dest.PhotoUrl360 = src.PhotoUrl360;
        dest.PhotoUrl432 = src.PhotoUrl432;
        dest.PhotoUrl504 = src.PhotoUrl504;
        dest.PhotoUrl648 = src.PhotoUrl648;
        dest.AzureTenant = azureTenant;
        return dest;
    }

    Admin_AddIdentityInput IEntityMapper.MapTo(Database.Entities.AzureTenantMember src, string customerId) =>
        new()
        {
            Id = src.Id,
            Email = src.Email.ToSafeString(),
            EmailVerified = true,
            CustomerId = customerId,
        };

    public Admin_UpdateIdentityInput MapToUpdateIdentityInput(Database.Entities.AzureTenantMember src, string customerId) =>
        new()
        {
            Id = src.Id,
            Email = src.Email.ToSafeString(),
            EmailVerified = true,
            CustomerId = customerId,
        };

    public Admin_AddInput MapTo(Database.Entities.AzureTenantMember src, string customerId, Database.Entities.Organization defaultOrganization)
    {
        var input = new Admin_AddInput
        {
            Id = customerId,
            Designation = src.Designation.ToSafeString(),
            GivenName = src.GivenName.ToSafeString(),
            FamilyName = src.FamilyName.ToSafeString(),
            IsOnboardingDone = true,
            DefaultOrganizationId = defaultOrganization.Id,
            PersonalInformationVisibility = PersonalInformationVisibility.Visible,
            Type = CustomerType.Registered,
        };

        input.Identities.Add(new Api.Shared.Grpc.Skedular.Customer.Core.V1.Identity
        {
            Id = src.Id,
            Email = src.Email,
            EmailVerified = true,
        });

        return input;
    }

    public OrganizationMember MapToEntity(
        Models.OrganizationMember src,
        Database.Entities.Organization organization,
        Database.Entities.Customer customer) =>
        MergeToEntity(src, new OrganizationMember(), organization, customer);

    public OrganizationOffering MapTo(Database.Entities.OrganizationOffering src, Models.Organization organization)
    {
        var organizationOffering = new OrganizationOffering
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
            FixedPrice = src.FixedPrice,
            Currency = src.Currency.ToCurrency(),
            PurchasedUserCapacity = src.PurchasedUserCapacity,
            PurchasedLocationCapacity = src.PurchasedLocationCapacity,
            PurchasedTeamCapacity = src.PurchasedTeamCapacity,
            DiscountPercentage = src.DiscountPercentage,
            SpacesBillingStartsAt = src.SpacesBillingStartsAt,
            Organization = organization,
        };

        organizationOffering.OrganizationOfferingActiveMembers = src.OrganizationOfferingActiveMembers
            .Select(item => new OrganizationOfferingActiveMember
            {
                Id = item.Id,
                CreatedAt = src.CreatedAt,
                ModifiedAt = src.ModifiedAt,
                OrganizationMember = MapTo(item.OrganizationMember, organization),
                OrganizationOffering = organizationOffering,
            })
            .ToList();

        return organizationOffering;
    }

    private Models.Organization MapTo(Database.Entities.Organization src, bool includeSpacesSubscription)
    {
        var organization = new Models.Organization
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            CustomDomain = src.CustomDomain,
            Name = src.Name,
            MarketplaceListingMetadata = src.MarketplaceListingMetadata ?? ListingMetadata.Empty,
            Website = src.Website,
            CustomerFacingTermsAndConditionsUrl = src.CustomerFacingTermsAndConditionsUrl,
            AgreedToTermsOfUse = src.AgreedToTermsOfUse,
            LogoUrl = src.LogoUrl,
            Type = src.Type.ToOrganizationType(),
            SpacesTrialStartedAt = src.SpacesTrialStartedAt,
            BillingCycle = src.BillingCycle.ToOrganizationBillingCycle(),
            ContactEmail = src.ContactEmail,
            ContactPhone = src.ContactPhone,
            RefundNotificationEmails = src.RefundNotificationEmails.ToSafeCollection(),
            IsOwnershipVerified = src.IsOwnershipVerified,
            FeatureImages = src.FeatureImages.ToSafeCollection(),
            TermsOfUse = MapTo(src.TermsOfUse),
            IndustrySubCategories = MapTo(src.IndustrySubCategories).ToList(),
        };

        organization.OrganizationMembers = MapTo(src.OrganizationMembers, organization).ToList();
        organization.OrganizationOfferings = MapTo(src.OrganizationOfferings, organization).ToList();
        organization.OrganizationSpacesSubscription = includeSpacesSubscription
            ? src.OrganizationOfferings.Where(item => IsSpacesOffering(src, item)).Select(item => MapToSpacesSubscription(item, src))
                .SingleOrDefault()
            : null;
        organization.DailyMemberCountRecordings = MapTo(src.DailyMemberCountRecordings, organization).ToList();
        organization.JoinInvitations = MapTo(src.JoinInvitations, organization).ToList();
        organization.Tags = MapTo(src.Tags, organization).ToList();
        organization.OrganizationStripeCustomer = MapTo(src.OrganizationStripeCustomer, organization);
        organization.OrganizationStripePaymentMethods = MapTo(src.OrganizationStripePaymentMethods, organization).ToList();
        organization.OrganizationStripeConnectAccounts = MapTo(src.OrganizationStripeConnectAccounts, organization).ToList();
        organization.OrganizationXeroConnection = MapTo(src.OrganizationXeroConnection, organization);

        return organization;
    }

    private static OrganizationStripeConnectAccountAuthorization? MapTo(
        Database.Entities.OrganizationStripeConnectAccountAuthorization? src) =>
        src is null
            ? null
            : new OrganizationStripeConnectAccountAuthorization
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                ModifiedAt = src.ModifiedAt,
                IsAuthorized = src.IsAuthorized,
            };

    private static OrganizationXeroConnection? MapTo(
        Database.Entities.OrganizationXeroConnection? src,
        Models.Organization organization) =>
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
                HasRefreshToken = !string.IsNullOrWhiteSpace(src.RefreshTokenEncrypted),
                Organization = organization,
            };

    private static TermsOfUse? MapTo(Database.Entities.TermsOfUse? src) =>
        src is null
            ? null
            : new TermsOfUse
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                Active = src.Active,
                Terms = src.Terms,
            };

    private static IEnumerable<IndustrySubCategory> MapTo(IEnumerable<Database.Entities.IndustrySubCategory> src) => src.Select(MapTo)!;

    private static IndustrySubCategory? MapTo(Database.Entities.IndustrySubCategory? src) =>
        src is null
            ? null
            : new IndustrySubCategory
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                Name = src.Name,
                IndustryMainCategory = MapTo(src.IndustryMainCategory),
            };

    private static IndustryMainCategory MapTo(Database.Entities.IndustryMainCategory src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Name = src.Name,
        };

    private static IEnumerable<Models.OrganizationMember> MapTo(
        IEnumerable<OrganizationMember> src,
        Models.Organization organization) =>
        src.Select(item => MapTo(item, organization));

    private static Models.OrganizationMember MapTo(OrganizationMember src, Models.Organization organization) =>
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
            Organization = organization,
        };

    private static Customer? MapTo(Database.Entities.Customer? src) =>
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
                Identities = MapTo(src.Identities).ToList(),
            };

    private static IEnumerable<Identity> MapTo(IEnumerable<Database.Entities.Identity> src) => src.Select(MapTo);

    private static Identity MapTo(Database.Entities.Identity src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            EventRaisedAt = src.EventRaisedAt,
            Email = src.Email,
            EmailVerified = src.EmailVerified,
        };

    private IEnumerable<OrganizationOffering> MapTo(
        IEnumerable<Database.Entities.OrganizationOffering> src,
        Models.Organization organization) =>
        src.Select(item => MapTo(item, organization));

    private OrganizationSpacesSubscription MapToSpacesSubscription(
        Database.Entities.OrganizationOffering src,
        Database.Entities.Organization organization) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            Organization = MapTo(organization, false),
            PlanCode = src.Code.ToPricingCatalogSubscriptionPlanCode(),
            CommercialModel = GetSpacesCommercialModel(src.Code.ToPricingCatalogSubscriptionPlanCode()),
            CurrentPeriodStart = src.Start,
            CurrentPeriodEnd = src.End,
            UsageLimit = src.Code == OfferingCode.EarlyBirdV1 ? null : src.PurchasedTeamCapacity,
            RolloverDate = src.End,
            CustomCapacity = src.Code == OfferingCode.SpacesContactUsV1 ? src.PurchasedTeamCapacity : null,
            CatalogVersion = src.CatalogVersion ?? src.Code.GetCurrentCatalogVersion(),
            Status = src.Code.IsEarlyBirdOffering() ? OrganizationOfferingPlanStatus.Legacy : OrganizationOfferingPlanStatus.Active,
        };

    private static PricingCatalogCommercialModel GetSpacesCommercialModel(PricingCatalogSubscriptionPlanCode planCode) =>
        planCode switch
        {
            PricingCatalogSubscriptionPlanCode.Free => PricingCatalogCommercialModel.Free,
            PricingCatalogSubscriptionPlanCode.Growth or PricingCatalogSubscriptionPlanCode.Business => PricingCatalogCommercialModel.UsageBased,
            PricingCatalogSubscriptionPlanCode.ContactUs => PricingCatalogCommercialModel.CapacityBased,
            _ => PricingCatalogCommercialModel.Free,
        };

    private static bool IsSpacesOffering(Database.Entities.Organization organization, Database.Entities.OrganizationOffering offering) =>
        organization.Type == OrganizationTypeConstants.Marketplace &&
        !offering.DeletedAt.HasValue &&
        offering.Code is OfferingCode.EarlyBirdV1
            or OfferingCode.SpacesFreeTierV1
            or OfferingCode.SpacesGrowthV1
            or OfferingCode.SpacesBusinessV1
            or OfferingCode.SpacesContactUsV1;

    private static IEnumerable<DailyMemberCountRecording> MapTo(
        IEnumerable<Database.Entities.DailyMemberCountRecording> src,
        Models.Organization organization) =>
        src.Select(item => MapTo(item, organization));

    private static DailyMemberCountRecording MapTo(Database.Entities.DailyMemberCountRecording src, Models.Organization organization) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Organization = organization,
            Date = src.Date,
            Count = src.Count,
        };

    private static IEnumerable<JoinInvitation> MapTo(IEnumerable<Database.Entities.JoinInvitation> src, Models.Organization organization) =>
        src.Select(item => MapTo(item, organization));

    private static JoinInvitation MapTo(Database.Entities.JoinInvitation src, Models.Organization organization) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            Email = src.Email,
            Status = src.Status.ToInvitationStatus(),
            Organization = organization,
            CreatedBy = MapTo(src.CreatedBy)!,
            Invitee = MapTo(src.Invitee),
        };

    private static IEnumerable<Tag> MapTo(IEnumerable<Database.Entities.Tag> src, Models.Organization organization) =>
        src.Select(item => MapTo(item, organization));

    private static Tag MapTo(Database.Entities.Tag src, Models.Organization organization) =>
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
            Organization = organization,
        };


    private static OrganizationStripeCustomer? MapTo(Database.Entities.OrganizationStripeCustomer? src, Models.Organization organization) =>
        src is null
            ? null
            : new OrganizationStripeCustomer
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                StripeCustomerId = src.StripeCustomerId,
                Organization = organization,
            };

    private static IEnumerable<Models.OrganizationStripePaymentMethod> MapTo(
        IEnumerable<OrganizationStripePaymentMethod> src,
        Models.Organization organization) =>
        src.Select(item => MapTo(item, organization));

    private static Models.OrganizationStripePaymentMethod MapTo(OrganizationStripePaymentMethod src, Models.Organization organization) =>
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
            Organization = organization,
        };

    private static IEnumerable<OrganizationStripeConnectAccount> MapTo(
        IEnumerable<Database.Entities.OrganizationStripeConnectAccount> src,
        Models.Organization organization) => src.Select(item => MapTo(item, organization));

    private static OrganizationStripeConnectAccount MapTo(Database.Entities.OrganizationStripeConnectAccount src, Models.Organization organization) =>
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
            Organization = organization,
            OrganizationStripeConnectAccountAuthorization = MapTo(src.OrganizationStripeConnectAccountAuthorization),
        };

    private static OrganizationMember MergeToEntity(
        Models.OrganizationMember src,
        OrganizationMember dest,
        Database.Entities.Organization organization,
        Database.Entities.Customer customer)
    {
        dest.Id = src.Id;
        dest.Role = src.Role.ToOrganizationMemberRole();
        dest.Status = src.Status.ToOrganizationMemberStatus();
        dest.IsOrganizationOnboardingDone = src.IsOrganizationOnboardingDone;
        dest.Organization = organization;
        dest.Customer = customer;
        return dest;
    }
}
