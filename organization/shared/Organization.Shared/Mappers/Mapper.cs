using Api.Shared.Clients.Events.Skedular.Organization.V1.Value;
using Api.Shared.Services.Grpc.Skedular.Customer.V1;
using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Enterprise.Shared;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Graph.Models;
using Organization.Shared.Models;
using AzureTenant = Organization.Shared.Database.Entities.AzureTenant;
using Customer = Organization.Shared.Models.Customer;
using CustomerType = Api.Shared.Services.Grpc.Skedular.Customer.V1.CustomerType;
using Identity = Organization.Shared.Models.Identity;
using Offering = Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Offering;
using OrganizationMember = Api.Shared.Clients.Events.Skedular.Organization.V1.Value.OrganizationMember;
using OrganizationSsoSettings = Organization.Shared.Models.OrganizationSsoSettings;
using OrganizationTaxDetails = Organization.Shared.Models.OrganizationTaxDetails;
using OrganizationStripePaymentMethod = Organization.Shared.Database.Entities.OrganizationStripePaymentMethod;
using OrganizationType = Api.Shared.Services.Models.OrganizationType;
using PaymentMethod = Stripe.PaymentMethod;
using PersonalInformationVisibility = Api.Shared.Services.Grpc.Skedular.Customer.V1.PersonalInformationVisibility;
using PhysicalAddress = Api.Shared.Clients.Events.Skedular.Organization.V1.Value.PhysicalAddress;
using OrganizationMemberStatus = Api.Shared.Clients.Events.Skedular.Organization.V1.Value.OrganizationMemberStatus;
using OrganizationMemberRole = Api.Shared.Clients.Events.Skedular.Organization.V1.Value.OrganizationMemberRole;
using OrganizationBillingCycle = Api.Shared.Clients.Events.Skedular.Organization.V1.Value.OrganizationBillingCycle;
using Tag = Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Tag;
using CdnFile = Api.Shared.Clients.Events.Skedular.Organization.V1.Value.CdnFile;
using CdnImageFile = Api.Shared.Clients.Events.Skedular.Organization.V1.Value.CdnImageFile;
using ListingMetadata = Api.Shared.Services.Models.ListingMetadata;

namespace Organization.Shared.Mappers;

public interface IMapper
{
    Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Organization MapTo(Models.Organization src);
    OrganizationStripePaymentMethod MapTo(PaymentMethod paymentMethod, string setupIntentId, Database.Entities.Organization organization);
    Models.Organization MapTo(Database.Entities.Organization src);
    AzureTenantMember MapTo(User src);
    Database.Entities.AzureTenantMember MapTo(AzureTenantMember src, AzureTenant azureTenant);
    Database.Entities.AzureTenantMember MergeToEntity(AzureTenantMember src, Database.Entities.AzureTenantMember dest, AzureTenant azureTenant);
    Admin_AddIdentityInput MapTo(Database.Entities.AzureTenantMember src, string customerId);
    Admin_UpdateIdentityInput MapToUpdateIdentityInput(Database.Entities.AzureTenantMember src, string customerId);
    Admin_AddInput MapTo(Database.Entities.AzureTenantMember src, string customerId, Database.Entities.Organization defaultOrganization);

    Database.Entities.OrganizationMember MapToEntity(
        Models.OrganizationMember src,
        Database.Entities.Organization organization,
        Database.Entities.Customer customer);
}

public class Mapper : IMapper
{
    public Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Organization MapTo(Models.Organization src)
    {
        var organizationOffering = src.OrganizationOfferings.Where(item => !item.DeletedAt.HasValue).OrderByDescending(item => item.End).First();
        var organization = new Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Organization
        {
            Id = src.Id,
            DeletedAt = src.DeletedAt?.ToTimestamp(),
            CustomDomain = src.CustomDomain.ToSafeString(),
            Name = src.Name.ToSafeString(),
            ListingMetadata = MapTo(src.ListingMetadata),
            MarketplaceListingMetadata = MapTo(src.MarketplaceListingMetadata),
            Website = src.Website.ToSafeString(),
            CustomerFacingTermsAndConditionsUrl = src.CustomerFacingTermsAndConditionsUrl.ToSafeString(),
            LogoUrl = src.LogoUrl.ToSafeString(),
            Type = src.Type switch
            {
                OrganizationType.Private => Api.Shared.Clients.Events.Skedular.Organization.V1.Value.OrganizationType.Private,
                OrganizationType.Marketplace => Api.Shared.Clients.Events.Skedular.Organization.V1.Value.OrganizationType.Marketplace,
                OrganizationType.Individual => Api.Shared.Clients.Events.Skedular.Organization.V1.Value.OrganizationType.Individual,
                _ => throw new ArgumentOutOfRangeException()
            },
            BillingCycle = src.BillingCycle switch
            {
                Api.Shared.Services.Models.OrganizationBillingCycle.Weekly => OrganizationBillingCycle.Weekly,
                Api.Shared.Services.Models.OrganizationBillingCycle.Fortnightly => OrganizationBillingCycle.Fortnightly,
                Api.Shared.Services.Models.OrganizationBillingCycle.Monthly => OrganizationBillingCycle.Monthly,
                _ => throw new ArgumentOutOfRangeException()
            },
            ContactEmail = src.ContactEmail.ToSafeString(),
            ContactPhone = src.ContactPhone.ToSafeString(),
            Offering = new Offering
            {
                Id = organizationOffering.Id,
                OrganizationId = src.Id,
                Code = organizationOffering.Code.ToOfferingCode(),
                Start = organizationOffering.Start.ToTimestamp(),
                End = organizationOffering.End.ToTimestamp(),
                AutoRenew = organizationOffering.AutoRenew,
                UnitPrice = organizationOffering.UnitPrice
            },
            SsoSettings = MapTo(src.OrganizationSsoSettings),
            TaxDetails = MapTo(src.OrganizationTaxDetails),
            PhysicalAddress = MapTo(src.PhysicalAddress),
            HasAttachedPaymentMethod = src.HasAttachedPaymentMethod,
            IsOwnershipVerified = src.IsOwnershipVerified ?? false
        };

        organization.AzureTenantIds.AddRange(src.AzureTenants.Select(item => item.Id));

        organization.Tags.AddRange(src.Tags.Select(item => new Tag
        {
            Id = item.Id,
            Name = item.Name.ToSafeString(),
            Description = item.Description.ToSafeString(),
            Type = item.Type.ToOrganizationTagType(),
            Color = item.Color.ToSafeString()
        }));

        organization.Offering.ActiveCustomerIds.AddRange(
            organizationOffering.OrganizationOfferingActiveMembers.Select(item => item.OrganizationMember.Customer.Id));

        organization.Members.AddRange(src.OrganizationMembers.Select(item => new OrganizationMember
        {
            Id = item.Id,
            CustomerId = item.Customer.Id,
            Role = item.Role switch
            {
                Api.Shared.Services.Models.OrganizationMemberRole.Owner => OrganizationMemberRole.Owner,
                Api.Shared.Services.Models.OrganizationMemberRole.Administrator => OrganizationMemberRole.Administrator,
                Api.Shared.Services.Models.OrganizationMemberRole.Member => OrganizationMemberRole.Member,
                _ => throw new ArgumentOutOfRangeException()
            },
            Status = item.Status switch
            {
                Api.Shared.Services.Models.OrganizationMemberStatus.Active => OrganizationMemberStatus.Active,
                Api.Shared.Services.Models.OrganizationMemberStatus.Inactive => OrganizationMemberStatus.Inactive,
                _ => throw new ArgumentOutOfRangeException()
            }
        }));

        organization.FeatureImages.AddRange(MapTo(src.FeatureImages));

        return organization;
    }

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
            Organization = organization
        };

    public Models.Organization MapTo(Database.Entities.Organization src)
    {
        var organization = new Models.Organization
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
            ContactEmail = src.ContactEmail,
            ContactPhone = src.ContactPhone,
            IsOwnershipVerified = src.IsOwnershipVerified,
            FeatureImages = src.FeatureImages.ToSafeCollection(),
            TermsOfUse = MapTo(src.TermsOfUse),
            IndustrySubCategories = MapTo(src.IndustrySubCategories).ToList()
        };

        organization.OrganizationMembers = MapTo(src.OrganizationMembers, organization).ToList();
        organization.OrganizationOfferings = MapTo(src.OrganizationOfferings, organization).ToList();
        organization.DailyMemberCountRecordings = MapTo(src.DailyMemberCountRecordings, organization).ToList();
        organization.JoinInvitations = MapTo(src.JoinInvitations, organization).ToList();
        organization.Tags = MapTo(src.Tags, organization).ToList();
        organization.OrganizationStripeCustomer = MapTo(src.OrganizationStripeCustomer, organization);
        organization.OrganizationStripePaymentMethods = MapTo(src.OrganizationStripePaymentMethods, organization).ToList();
        organization.OrganizationStripeConnectAccounts = MapTo(src.OrganizationStripeConnectAccounts, organization).ToList();

        return organization;
    }

    public AzureTenantMember MapTo(User src) =>
        new()
        {
            Id = src.Id!,
            Email = src.Mail,
            Designation = src.JobTitle,
            Name = src.DisplayName,
            GivenName = src.GivenName,
            FamilyName = src.Surname,
            PreferredLanguage = src.PreferredLanguage
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

    Admin_AddIdentityInput IMapper.MapTo(Database.Entities.AzureTenantMember src, string customerId) =>
        new() { Id = src.Id, Email = src.Email.ToSafeString(), EmailVerified = true, CustomerId = customerId };

    public Admin_UpdateIdentityInput MapToUpdateIdentityInput(Database.Entities.AzureTenantMember src, string customerId) =>
        new() { Id = src.Id, Email = src.Email.ToSafeString(), EmailVerified = true, CustomerId = customerId };

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
            Type = CustomerType.Registered
        };

        input.Identities.Add(new Api.Shared.Services.Grpc.Skedular.Customer.V1.Identity { Id = src.Id, Email = src.Email, EmailVerified = true });

        return input;
    }

    public Database.Entities.OrganizationMember MapToEntity(
        Models.OrganizationMember src,
        Database.Entities.Organization organization,
        Database.Entities.Customer customer) =>
        MergeToEntity(src, new Database.Entities.OrganizationMember(), organization, customer);

    private static OrganizationStripeConnectAccountAuthorization? MapTo(
        Database.Entities.OrganizationStripeConnectAccountAuthorization? src) =>
        src is null
            ? null
            : new OrganizationStripeConnectAccountAuthorization
            {
                Id = src.Id, CreatedAt = src.CreatedAt, ModifiedAt = src.ModifiedAt, IsAuthorized = src.IsAuthorized
            };

    private static Api.Shared.Clients.Events.Skedular.Organization.V1.Value.OrganizationSsoSettings? MapTo(OrganizationSsoSettings? src) =>
        src is null
            ? null
            : new Api.Shared.Clients.Events.Skedular.Organization.V1.Value.OrganizationSsoSettings
            {
                Id = src.Id,
                IsActive = src.IsActive,
                EntityId = src.EntityId.ToSafeString(),
                LoginUrl = src.LoginUrl.ToSafeString(),
                AppFederationMetadataUrl = src.AppFederationMetadataUrl.ToSafeString()
            };

    private static Api.Shared.Clients.Events.Skedular.Organization.V1.Value.OrganizationTaxDetails? MapTo(OrganizationTaxDetails? src) =>
        src is null
            ? null
            : new Api.Shared.Clients.Events.Skedular.Organization.V1.Value.OrganizationTaxDetails
            {
                Id = src.Id, TaxId = src.TaxId.ToSafeString(), TaxRatePercentage = Convert.ToDouble(src.TaxRatePercentage)
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
                Terms = src.Terms
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
                IndustryMainCategory = MapTo(src.IndustryMainCategory)
            };

    private static IndustryMainCategory MapTo(Database.Entities.IndustryMainCategory src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Name = src.Name
        };

    private static IEnumerable<Models.OrganizationMember> MapTo(
        IEnumerable<Database.Entities.OrganizationMember> src,
        Models.Organization organization) =>
        src.Select(item => MapTo(item, organization));

    private static Models.OrganizationMember MapTo(Database.Entities.OrganizationMember src, Models.Organization organization) =>
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
                Identities = MapTo(src.Identities).ToList()
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
            EmailVerified = src.EmailVerified
        };

    private static IEnumerable<OrganizationOffering> MapTo(
        IEnumerable<Database.Entities.OrganizationOffering> src,
        Models.Organization organization) =>
        src.Select(item => MapTo(item, organization));

    private static OrganizationOffering MapTo(Database.Entities.OrganizationOffering src, Models.Organization organization)
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
            Organization = organization
        };

        organizationOffering.OrganizationOfferingActiveMembers = src.OrganizationOfferingActiveMembers
            .Select(item => new OrganizationOfferingActiveMember
            {
                Id = item.Id,
                CreatedAt = src.CreatedAt,
                ModifiedAt = src.ModifiedAt,
                OrganizationMember = MapTo(item.OrganizationMember, organization),
                OrganizationOffering = organizationOffering
            })
            .ToList();

        return organizationOffering;
    }

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
            Count = src.Count
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
            Invitee = MapTo(src.Invitee)
        };

    private static IEnumerable<Models.Tag> MapTo(IEnumerable<Database.Entities.Tag> src, Models.Organization organization) =>
        src.Select(item => MapTo(item, organization));

    private static Models.Tag MapTo(Database.Entities.Tag src, Models.Organization organization) =>
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

    private static PhysicalAddress? MapTo(OrganizationPhysicalAddress? src) =>
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
                CountryCode = src.CountryCode.ToSafeString(),
                FormattedAddress = src.ToFormattedAddress(),
                OsmType = src.OsmType.ToSafeString(),
                OsmId = src.OsmId.ToSafeString(),
                PlaceId = src.PlaceId.ToSafeString(),
                Coordinates = src.Coordinates is null ? null : new Coordinates { Longitude = src.Coordinates.X, Latitude = src.Coordinates.Y }
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
                Organization = organization
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
            Organization = organization
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
            OrganizationStripeConnectAccountAuthorization = MapTo(src.OrganizationStripeConnectAccountAuthorization)
        };

    private static Database.Entities.OrganizationMember MergeToEntity(
        Models.OrganizationMember src,
        Database.Entities.OrganizationMember dest,
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

    private static IEnumerable<CdnImageFile> MapTo(IEnumerable<Api.Shared.Services.Models.CdnImageFile> src) =>
        src.Select(MapTo);

    private static CdnImageFile MapTo(Api.Shared.Services.Models.CdnImageFile src) =>
        new() { Original = MapTo(src.Original), Thumbnail = MapTo(src.Thumbnail) };

    private static CdnFile? MapTo(Api.Shared.Services.Models.CdnFile? src) =>
        src is null ? null : new CdnFile { Url = src.Url.ToSafeString(), Height = src.Height.ToNullInt(), Width = src.Width.ToNullInt() };

    private static Api.Shared.Clients.Events.Skedular.Organization.V1.Value.ListingMetadata MapTo(ListingMetadata src)
    {
        var listingMetadata = new Api.Shared.Clients.Events.Skedular.Organization.V1.Value.ListingMetadata
        {
            About = src.About.ToSafeString(), Title = src.Title.ToSafeString(), SubTitle = src.SubTitle.ToSafeString()
        };

        listingMetadata.IncludedFeatures.AddRange(src.IncludedFeatures.ToSafeCollection().Select(item => item.ToSafeString()));

        return listingMetadata;
    }
}
