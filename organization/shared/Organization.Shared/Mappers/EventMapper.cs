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
using OrganizationMemberStatus = Api.Shared.Clients.Events.Skedular.Organization.V1.OrganizationMemberStatus;
using OrganizationMemberRole = Api.Shared.Clients.Events.Skedular.Organization.V1.OrganizationMemberRole;
using OrganizationBillingCycle = Api.Shared.Clients.Events.Skedular.Organization.V1.OrganizationBillingCycle;
using Tag = Api.Shared.Clients.Events.Skedular.Organization.V1.Tag;
using CdnFile = Api.Shared.Clients.Events.Skedular.Organization.V1.CdnFile;
using CdnImageFile = Api.Shared.Clients.Events.Skedular.Organization.V1.CdnImageFile;
using ListingMetadata = Api.Shared.Services.Models.ListingMetadata;

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
        var organization = new Api.Shared.Clients.Events.Skedular.Organization.V1.Organization
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
                OrganizationType.Private => Api.Shared.Clients.Events.Skedular.Organization.V1.OrganizationType.Private,
                OrganizationType.Marketplace => Api.Shared.Clients.Events.Skedular.Organization.V1.OrganizationType.Marketplace,
                OrganizationType.Individual => Api.Shared.Clients.Events.Skedular.Organization.V1.OrganizationType.Individual,
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
            RefundNotificationEmails = { src.RefundNotificationEmails },
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

    private static Api.Shared.Clients.Events.Skedular.Organization.V1.OrganizationSsoSettings? MapTo(OrganizationSsoSettings? src) =>
        src is null
            ? null
            : new Api.Shared.Clients.Events.Skedular.Organization.V1.OrganizationSsoSettings
            {
                Id = src.Id,
                IsActive = src.IsActive,
                EntityId = src.EntityId.ToSafeString(),
                LoginUrl = src.LoginUrl.ToSafeString(),
                AppFederationMetadataUrl = src.AppFederationMetadataUrl.ToSafeString()
            };

    private static Api.Shared.Clients.Events.Skedular.Organization.V1.OrganizationTaxDetails? MapTo(OrganizationTaxDetails? src) =>
        src is null
            ? null
            : new Api.Shared.Clients.Events.Skedular.Organization.V1.OrganizationTaxDetails
            {
                Id = src.Id, TaxId = src.TaxId.ToSafeString(), TaxRatePercentage = Convert.ToDouble(src.TaxRatePercentage)
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

    private static IEnumerable<CdnImageFile> MapTo(IEnumerable<Api.Shared.Services.Models.CdnImageFile> src) =>
        src.Select(MapTo);

    private static CdnImageFile MapTo(Api.Shared.Services.Models.CdnImageFile src) =>
        new() { Original = MapTo(src.Original), Thumbnail = MapTo(src.Thumbnail) };

    private static CdnFile? MapTo(Api.Shared.Services.Models.CdnFile? src) =>
        src is null ? null : new CdnFile { Url = src.Url.ToSafeString(), Height = src.Height.ToNullInt(), Width = src.Width.ToNullInt() };

    private static Api.Shared.Clients.Events.Skedular.Organization.V1.ListingMetadata MapTo(ListingMetadata src)
    {
        var listingMetadata = new Api.Shared.Clients.Events.Skedular.Organization.V1.ListingMetadata
        {
            About = src.About.ToSafeString(), Title = src.Title.ToSafeString(), SubTitle = src.SubTitle.ToSafeString()
        };

        listingMetadata.IncludedFeatures.AddRange(src.IncludedFeatures.ToSafeCollection().Select(item => item.ToSafeString()));

        return listingMetadata;
    }
}
