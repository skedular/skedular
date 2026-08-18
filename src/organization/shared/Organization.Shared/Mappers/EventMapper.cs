using System.Globalization;
using Api.Shared.Clients.Events.Skedular.Organization.V1;
using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Enterprise.Shared;
using Google.Protobuf.WellKnownTypes;
using Organization.Shared.Models;
using Offering = Api.Shared.Clients.Events.Skedular.Organization.V1.Offering;
using OrganizationMember = Api.Shared.Clients.Events.Skedular.Organization.V1.OrganizationMember;
using OrganizationSsoSettings = Organization.Shared.Models.OrganizationSsoSettings;
using OrganizationTaxDetails = Organization.Shared.Models.OrganizationTaxDetails;
using OrganizationType = Api.Shared.Services.Models.OrganizationType;
using PhysicalAddress = Api.Shared.Clients.Events.Skedular.Organization.V1.PhysicalAddress;
using Tag = Api.Shared.Clients.Events.Skedular.Organization.V1.Tag;
using CdnFile = Api.Shared.Clients.Events.Skedular.Organization.V1.CdnFile;
using CdnImageFile = Api.Shared.Clients.Events.Skedular.Organization.V1.CdnImageFile;
using Currency = Api.Shared.Services.Models.Currency;
using ListingMetadata = Api.Shared.Services.Models.ListingMetadata;
using Models_CdnFile = Api.Shared.Services.Models.CdnFile;
using Models_CdnImageFile = Api.Shared.Services.Models.CdnImageFile;
using OrganizationBillingCycle = Api.Shared.Services.Models.OrganizationBillingCycle;
using OrganizationMemberRole = Api.Shared.Services.Models.OrganizationMemberRole;
using OrganizationMemberStatus = Api.Shared.Services.Models.OrganizationMemberStatus;

namespace Organization.Shared.Mappers;

public interface IEventMapper
{
    Api.Shared.Clients.Events.Skedular.Organization.V1.Organization MapTo(Models.Organization src);
}

public class EventMapper : IEventMapper
{
    public Api.Shared.Clients.Events.Skedular.Organization.V1.Organization MapTo(Models.Organization src)
    {
        var organizationOffering = src.OrganizationOfferings.Where(item => !item.DeletedAt.HasValue).OrderByDescending(item => item.End).First();
        var activeCustomerIds = organizationOffering.OrganizationOfferingActiveMembers
            .Select(item => item.OrganizationMember.Customer.Id)
            .ToArray();
        var entitlementDecision = new PricingEntitlementEvaluator().EvaluateActiveUserCount(
            new Api.Shared.Services.Models.Offering
            {
                Id = organizationOffering.Id,
                Code = organizationOffering.Code,
                Start = organizationOffering.Start,
                End = organizationOffering.End,
                PurchasedUserCapacity = organizationOffering.PurchasedUserCapacity,
                PurchasedLocationCapacity = organizationOffering.PurchasedLocationCapacity,
                PurchasedTeamCapacity = organizationOffering.PurchasedTeamCapacity,
                ActiveCustomerIds = activeCustomerIds,
            },
            activeCustomerIds.Length);

        var eventOffering = new Offering
        {
            Id = organizationOffering.Id,
            OrganizationId = src.Id,
            Code = organizationOffering.Code.ToOfferingCode(),
            Start = organizationOffering.Start.ToTimestamp(),
            End = organizationOffering.End.ToTimestamp(),
            AutoRenew = organizationOffering.AutoRenew,
            CurrentActiveUserCount = activeCustomerIds.Length.ToString(CultureInfo.InvariantCulture),
            IsInteractionAllowed = entitlementDecision.IsAllowed,
            EntitlementReasonCode = entitlementDecision.ReasonCode.ToString(),
            HostCommissionPercentage = decimal.ToDouble(organizationOffering.HostCommissionPercentage),
            SpacesProductEnabled = src.Type == OrganizationType.Marketplace && IsSpacesOffering(organizationOffering.Code),
            Currency = organizationOffering.Currency switch
            {
                Currency.Nzd => Api.Shared.Clients.Events.Skedular.Organization.V1.Currency.Nzd,
                Currency.Usd => Api.Shared.Clients.Events.Skedular.Organization.V1.Currency.Usd,
                _ => throw new ArgumentOutOfRangeException(null,
                    "Unexpected value encountered. Update enum mapping or caller input to include this case."),
            },
        };
        if (organizationOffering.UnitPrice.HasValue)
        {
            eventOffering.UnitPrice = organizationOffering.UnitPrice.Value;
        }

        if (organizationOffering.FixedPrice.HasValue)
        {
            eventOffering.FixedPrice = organizationOffering.FixedPrice.Value;
        }

        if (organizationOffering.PurchasedUserCapacity.HasValue)
        {
            eventOffering.PurchasedUserCapacity = organizationOffering.PurchasedUserCapacity.Value;
        }

        if (organizationOffering.PurchasedLocationCapacity.HasValue)
        {
            eventOffering.PurchasedLocationCapacity = organizationOffering.PurchasedLocationCapacity.Value;
        }

        if (organizationOffering.PurchasedTeamCapacity.HasValue)
        {
            eventOffering.PurchasedTeamCapacity = organizationOffering.PurchasedTeamCapacity.Value;
        }

        var spacesTrialStartedAt = src.SpacesTrialStartedAt ??
                                   (src.Type == OrganizationType.Marketplace &&
                                    organizationOffering.Code == OfferingCode.SpacesFreeTierV1
                                       ? src.CreatedAt
                                       : null);
        if (spacesTrialStartedAt.HasValue)
        {
            eventOffering.SpacesTrialStartedAt = spacesTrialStartedAt.Value.ToTimestamp();
            eventOffering.SpacesTrialEndsAt = spacesTrialStartedAt.Value.AddDays(14).ToTimestamp();
        }

        if (organizationOffering.SpacesBillingStartsAt.HasValue)
        {
            eventOffering.SpacesNextBillingAt = organizationOffering.SpacesBillingStartsAt.Value.ToTimestamp();
        }

        var organization = new Api.Shared.Clients.Events.Skedular.Organization.V1.Organization
        {
            Id = src.Id,
            DeletedAt = src.DeletedAt?.ToTimestamp(),
            CustomDomain = src.CustomDomain.ToSafeString(),
            Name = src.Name.ToSafeString(),
            MarketplaceListingMetadata = MapTo(src.MarketplaceListingMetadata),
            Website = src.Website.ToSafeString(),
            CustomerFacingTermsAndConditionsUrl = src.CustomerFacingTermsAndConditionsUrl.ToSafeString(),
            LogoUrl = src.LogoUrl.ToSafeString(),
            Type = src.Type switch
            {
                OrganizationType.Private => Api.Shared.Clients.Events.Skedular.Organization.V1.OrganizationType.Private,
                OrganizationType.Marketplace => Api.Shared.Clients.Events.Skedular.Organization.V1.OrganizationType.Marketplace,
                OrganizationType.Host => Api.Shared.Clients.Events.Skedular.Organization.V1.OrganizationType.Host,
                _ => throw new ArgumentOutOfRangeException(null,
                    "Unexpected value encountered. Update enum mapping or caller input to include this case."),
            },
            BillingCycle = src.BillingCycle switch
            {
                OrganizationBillingCycle.Weekly => Api.Shared.Clients.Events.Skedular.Organization.V1.OrganizationBillingCycle.Weekly,
                OrganizationBillingCycle.Fortnightly => Api.Shared.Clients.Events.Skedular.Organization.V1.OrganizationBillingCycle.Fortnightly,
                OrganizationBillingCycle.Monthly => Api.Shared.Clients.Events.Skedular.Organization.V1.OrganizationBillingCycle.Monthly,
                _ => throw new ArgumentOutOfRangeException(null,
                    "Unexpected value encountered. Update enum mapping or caller input to include this case."),
            },
            ContactEmail = src.ContactEmail.ToSafeString(),
            ContactPhone = src.ContactPhone.ToSafeString(),
            RefundNotificationEmails =
            {
                src.RefundNotificationEmails,
            },
            Offering = eventOffering,
            SsoSettings = MapTo(src.OrganizationSsoSettings),
            TaxDetails = MapTo(src.OrganizationTaxDetails),
            PhysicalAddress = MapTo(src.PhysicalAddress),
            HasAttachedPaymentMethod = src.HasAttachedPaymentMethod,
            IsOwnershipVerified = src.IsOwnershipVerified ?? false,
        };

        organization.AzureTenantIds.AddRange(src.AzureTenants.Select(item => item.Id));

        organization.Tags.AddRange(src.Tags.Select(item => new Tag
        {
            Id = item.Id,
            Name = item.Name.ToSafeString(),
            Description = item.Description.ToSafeString(),
            Type = item.Type.ToOrganizationTagType(),
            Color = item.Color.ToSafeString(),
        }));

        organization.Offering.ActiveCustomerIds.AddRange(activeCustomerIds);

        organization.Members.AddRange(src.OrganizationMembers.Select(item => new OrganizationMember
        {
            Id = item.Id,
            CustomerId = item.Customer.Id,
            Role = item.Role switch
            {
                OrganizationMemberRole.Owner => Api.Shared.Clients.Events.Skedular.Organization.V1.OrganizationMemberRole.Owner,
                OrganizationMemberRole.Administrator => Api.Shared.Clients.Events.Skedular.Organization.V1.OrganizationMemberRole.Administrator,
                OrganizationMemberRole.Member => Api.Shared.Clients.Events.Skedular.Organization.V1.OrganizationMemberRole.Member,
                _ => throw new ArgumentOutOfRangeException(null,
                    "Unexpected value encountered. Update enum mapping or caller input to include this case."),
            },
            Status = item.Status switch
            {
                OrganizationMemberStatus.Active => Api.Shared.Clients.Events.Skedular.Organization.V1.OrganizationMemberStatus.Active,
                OrganizationMemberStatus.Inactive => Api.Shared.Clients.Events.Skedular.Organization.V1.OrganizationMemberStatus.Inactive,
                _ => throw new ArgumentOutOfRangeException(null,
                    "Unexpected value encountered. Update enum mapping or caller input to include this case."),
            },
        }));

        organization.FeatureImages.AddRange(MapTo([.. src.FeatureImages]));

        return organization;
    }

    private static bool IsSpacesOffering(OfferingCode offeringCode) =>
        offeringCode is OfferingCode.EarlyBirdV1 or
            OfferingCode.SpacesFreeTierV1 or
            OfferingCode.SpacesGrowthV1 or
            OfferingCode.SpacesBusinessV1 or
            OfferingCode.SpacesContactUsV1;

    private static Api.Shared.Clients.Events.Skedular.Organization.V1.OrganizationSsoSettings? MapTo(OrganizationSsoSettings? src) =>
        src is null
            ? null
            : new Api.Shared.Clients.Events.Skedular.Organization.V1.OrganizationSsoSettings
            {
                Id = src.Id,
                IsActive = src.IsActive,
                EntityId = src.EntityId.ToSafeString(),
                LoginUrl = src.LoginUrl.ToSafeString(),
                AppFederationMetadataUrl = src.AppFederationMetadataUrl.ToSafeString(),
            };

    private static Api.Shared.Clients.Events.Skedular.Organization.V1.OrganizationTaxDetails? MapTo(OrganizationTaxDetails? src) =>
        src is null
            ? null
            : new Api.Shared.Clients.Events.Skedular.Organization.V1.OrganizationTaxDetails
            {
                Id = src.Id,
                TaxId = src.TaxId.ToSafeString(),
                TaxRatePercentage = Convert.ToDouble(src.TaxRatePercentage),
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
                Coordinates = src.Coordinates is null
                    ? null
                    : new Coordinates
                    {
                        Longitude = src.Coordinates.X,
                        Latitude = src.Coordinates.Y,
                    },
            };

    private static Api.Shared.Clients.Events.Skedular.Organization.V1.ListingMetadata MapTo(ListingMetadata src)
    {
        var listingMetadata = new Api.Shared.Clients.Events.Skedular.Organization.V1.ListingMetadata
        {
            About = src.About.ToSafeString(),
            Title = src.Title.ToSafeString(),
            SubTitle = src.SubTitle.ToSafeString(),
        };

        listingMetadata.IncludedFeatures.AddRange(src.IncludedFeatures.ToSafeCollection().Select(item => item.ToSafeString()));

        return listingMetadata;
    }

    private static CdnImageFile[] MapTo(Models_CdnImageFile[] src) => [.. src.Select(MapTo)];

    private static CdnImageFile MapTo(Models_CdnImageFile src) =>
        new()
        {
            Original = MapTo(src.Original),
            Thumbnail = MapTo(src.Thumbnail),
        };

    private static CdnFile? MapTo(Models_CdnFile? src) =>
        src is null
            ? null
            : new CdnFile
            {
                Url = src.Url.ToSafeString(),
                Height = src.Height.ToNullInt(),
                Width = src.Width.ToNullInt(),
            };
}
