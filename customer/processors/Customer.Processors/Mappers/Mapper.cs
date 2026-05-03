using Api.Shared.Services.Models;
using Enterprise.Shared;
using CustomerBillingDetails = Customer.Shared.Models.CustomerBillingDetails;
using Event = Api.Shared.Clients.Events.Skedular.Organization.V1.Event;
using Identity = Customer.Shared.Models.Identity;
using Location = Customer.Shared.Models.Location;
using LocationType = Api.Shared.Clients.Events.Skedular.Location.V1.LocationType;
using Organization = Customer.Shared.Models.Organization;
using OrganizationMember = Customer.Shared.Database.Entities.OrganizationMember;
using OrganizationSsoSetting = Customer.Shared.Models.OrganizationSsoSetting;
using OrganizationTag = Customer.Shared.Models.OrganizationTag;
using OrganizationType = Api.Shared.Clients.Events.Skedular.Organization.V1.OrganizationType;
using Resource = Customer.Shared.Database.Entities.Resource;
using OrganizationMemberRole = Api.Shared.Clients.Events.Skedular.Organization.V1.OrganizationMemberRole;
using OrganizationMemberStatus = Api.Shared.Clients.Events.Skedular.Organization.V1.OrganizationMemberStatus;

namespace Customer.Processors.Mappers;

public interface IMapper
{
    Organization MapTo(Event src);
    Location MapTo(Api.Shared.Clients.Events.Skedular.Location.V1.Event src);
    Shared.Models.Customer? MapTo(Shared.Database.Entities.Customer? src);
    Shared.Database.Entities.Organization MergeToEntity(Organization src, Shared.Database.Entities.Organization dest);

    Shared.Database.Entities.Location MergeToEntity(
        Location src,
        Shared.Database.Entities.Location dest,
        Shared.Database.Entities.Organization organization);

    Resource MapToEntity(Shared.Models.Resource src, Shared.Database.Entities.Location location);
    Resource MergeToEntity(Shared.Models.Resource src, Resource dest, Shared.Database.Entities.Location location);

    OrganizationMember MapToEntity(
        Shared.Models.OrganizationMember src,
        Shared.Database.Entities.Organization organization,
        Shared.Database.Entities.Customer customer);

    OrganizationMember MergeToEntity(
        Shared.Models.OrganizationMember src,
        OrganizationMember dest,
        Shared.Database.Entities.Organization organization,
        Shared.Database.Entities.Customer customer);

    Shared.Database.Entities.OrganizationTag MapToEntity(OrganizationTag src, Shared.Database.Entities.Organization organization);

    Shared.Database.Entities.OrganizationTag MergeToEntity(
        OrganizationTag src,
        Shared.Database.Entities.OrganizationTag dest,
        Shared.Database.Entities.Organization organization);

    Shared.Database.Entities.OrganizationSsoSetting MapTo(OrganizationSsoSetting src, Shared.Database.Entities.Organization organization);

    Shared.Database.Entities.OrganizationSsoSetting MergeTo(
        OrganizationSsoSetting src,
        Shared.Database.Entities.OrganizationSsoSetting dest,
        Shared.Database.Entities.Organization organization);
}

public class Mapper : IMapper
{
    public Organization MapTo(Event src)
    {
        var organizationAfterState = src.Data.Organization;
        var deletedAt = organizationAfterState.DeletedAt?.ToDateTimeOffset();
        var eventRaisedAt = src.Metadata.Time.ToDateTimeOffset();

        var organization = new Organization
        {
            Id = organizationAfterState.Id,
            DeletedAt = deletedAt,
            EventRaisedAt = eventRaisedAt,
            CustomDomain = string.IsNullOrWhiteSpace(organizationAfterState.CustomDomain) ? null : organizationAfterState.CustomDomain,
            Type = organizationAfterState.Type switch
            {
                OrganizationType.Private => Api.Shared.Services.Models.OrganizationType.Private,
                OrganizationType.Marketplace => Api.Shared.Services.Models.OrganizationType.Marketplace,
                OrganizationType.Individual => Api.Shared.Services.Models.OrganizationType.Individual,
                _ => throw new ArgumentOutOfRangeException()
            },
            IsOwnershipVerified = organizationAfterState.IsOwnershipVerified
        };

        organization.OrganizationMembers = organizationAfterState.Members.Select(item => new Shared.Models.OrganizationMember
        {
            Id = item.Id,
            EventRaisedAt = eventRaisedAt,
            Role = item.Role switch
            {
                OrganizationMemberRole.Owner => Api.Shared.Services.Models.OrganizationMemberRole.Owner,
                OrganizationMemberRole.Administrator => Api.Shared.Services.Models.OrganizationMemberRole.Administrator,
                OrganizationMemberRole.Member => Api.Shared.Services.Models.OrganizationMemberRole.Member,
                _ => throw new ArgumentOutOfRangeException()
            },
            Status = item.Status switch
            {
                OrganizationMemberStatus.Active => Api.Shared.Services.Models.OrganizationMemberStatus.Active,
                OrganizationMemberStatus.Inactive => Api.Shared.Services.Models.OrganizationMemberStatus.Inactive,
                _ => throw new ArgumentOutOfRangeException()
            },
            Customer = new Shared.Models.Customer { Id = item.CustomerId },
            Organization = organization
        }).ToList();

        organization.Tags = organizationAfterState.Tags.Select(item => new OrganizationTag
        {
            Id = item.Id,
            EventRaisedAt = eventRaisedAt,
            Name = item.Name.ToSafeString(),
            Type = item.Type.ToNullableOrganizationTagType(),
            Color = item.Color.ToSafeString(),
            Organization = organization
        }).ToList();

        organization.OrganizationSsoSettings = organizationAfterState.SsoSettings is null
            ? null
            : new OrganizationSsoSetting
            {
                Id = organizationAfterState.SsoSettings.Id,
                EventRaisedAt = eventRaisedAt,
                EntityId = organizationAfterState.SsoSettings.EntityId,
                LoginUrl = organizationAfterState.SsoSettings.LoginUrl,
                AppFederationMetadataUrl = organizationAfterState.SsoSettings.AppFederationMetadataUrl,
                IsActive = organizationAfterState.SsoSettings.IsActive,
                Organization = organization
            };

        return organization;
    }

    public Location MapTo(Api.Shared.Clients.Events.Skedular.Location.V1.Event src)
    {
        var locationAfterState = src.Data.Location;
        var deletedAt = locationAfterState.DeletedAt?.ToDateTimeOffset();
        var eventRaisedAt = src.Metadata.Time.ToDateTimeOffset();

        var location = new Location
        {
            Id = locationAfterState.Id,
            DeletedAt = deletedAt,
            EventRaisedAt = eventRaisedAt,
            Organization = new Organization { Id = locationAfterState.OrganizationId },
            Type = locationAfterState.Type switch
            {
                LocationType.Private => Api.Shared.Services.Models.LocationType.Private,
                LocationType.Marketplace => Api.Shared.Services.Models.LocationType.Marketplace,
                _ => throw new ArgumentOutOfRangeException()
            }
        };

        location.Resources = locationAfterState.Resources.Select(item =>
            new Shared.Models.Resource { Id = item.Id, DeletedAt = deletedAt, EventRaisedAt = eventRaisedAt, Location = location }).ToList();

        return location;
    }

    public Shared.Models.Customer? MapTo(Shared.Database.Entities.Customer? src) =>
        src is null
            ? null
            : new Shared.Models.Customer
            {
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                Id = src.Id,
                Title = src.Title,
                Designation = src.Designation,
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
                Timezone = src.Timezone,
                Locale = src.Locale,
                PhoneNumber = src.PhoneNumber,
                BillingDetails = MapTo(src.BillingDetails),
                IsOnboardingDone = src.IsOnboardingDone,
                Identities = MapTo(src.Identities).ToList(),
                DefaultOrganization = MapTo(src.DefaultOrganization),
                PreferredLocations = MapTo(src.PreferredLocations).ToList(),
                PreferredResources = MapTo(src.PreferredResources).ToList(),
                PreferredOrganizationTags = MapTo(src.PreferredOrganizationTags).ToList(),
                FavouriteLocations = MapTo(src.FavouriteLocations).ToList(),
                PersonalInformationVisibility = src.PersonalInformationVisibility.ToPersonalInformationVisibility(),
                Type = src.Type.ToCustomerType()
            };

    public Shared.Database.Entities.Organization MergeToEntity(Organization src, Shared.Database.Entities.Organization dest)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.CustomDomain = src.CustomDomain;
        dest.Type = src.Type.ToOrganizationType();
        dest.IsOwnershipVerified = src.IsOwnershipVerified;
        return dest;
    }

    public Shared.Database.Entities.Location MergeToEntity(
        Location src,
        Shared.Database.Entities.Location dest,
        Shared.Database.Entities.Organization organization)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Type = src.Type.ToNullableLocationType();
        dest.Organization = organization;
        return dest;
    }

    public Resource MapToEntity(Shared.Models.Resource src, Shared.Database.Entities.Location location) =>
        MergeToEntity(src, new Resource(), location);

    public Resource MergeToEntity(Shared.Models.Resource src, Resource dest, Shared.Database.Entities.Location location)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Location = location;
        return dest;
    }

    public OrganizationMember MapToEntity(
        Shared.Models.OrganizationMember src,
        Shared.Database.Entities.Organization organization,
        Shared.Database.Entities.Customer customer) =>
        MergeToEntity(src, new OrganizationMember(), organization, customer);

    public OrganizationMember MergeToEntity(
        Shared.Models.OrganizationMember src,
        OrganizationMember dest,
        Shared.Database.Entities.Organization organization,
        Shared.Database.Entities.Customer customer)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Role = src.Role.ToNullableOrganizationMemberRole();
        dest.Status = src.Status.ToOrganizationMemberStatus();
        dest.Organization = organization;
        dest.Customer = customer;
        return dest;
    }

    public Shared.Database.Entities.OrganizationTag MapToEntity(OrganizationTag src, Shared.Database.Entities.Organization organization) =>
        MergeToEntity(src, new Shared.Database.Entities.OrganizationTag(), organization);

    public Shared.Database.Entities.OrganizationTag MergeToEntity(
        OrganizationTag src,
        Shared.Database.Entities.OrganizationTag dest,
        Shared.Database.Entities.Organization organization)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Name = src.Name;
        dest.Type = src.Type.ToNullableOrganizationTagType();
        dest.Color = src.Color;
        dest.Organization = organization;
        return dest;
    }

    public Shared.Database.Entities.OrganizationSsoSetting MapTo(OrganizationSsoSetting src, Shared.Database.Entities.Organization organization) =>
        MergeTo(src, new Shared.Database.Entities.OrganizationSsoSetting(), organization);

    public Shared.Database.Entities.OrganizationSsoSetting MergeTo(
        OrganizationSsoSetting src,
        Shared.Database.Entities.OrganizationSsoSetting dest,
        Shared.Database.Entities.Organization organization)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.EntityId = src.EntityId;
        dest.LoginUrl = src.LoginUrl;
        dest.AppFederationMetadataUrl = src.AppFederationMetadataUrl;
        dest.IsActive = src.IsActive;
        dest.Organization = organization;

        return dest;
    }

    private static IEnumerable<Location> MapTo(IEnumerable<Shared.Database.Entities.Location?>? src) =>
        (src is null ? [] : src.Where(item => item is not null).Select(item => MapTo(item, true)))!;

    private static IEnumerable<Identity> MapTo(IEnumerable<Shared.Database.Entities.Identity?>? src) =>
        (src is null ? [] : src.Where(item => item is not null).Select(MapTo))!;

    private static Identity? MapTo(Shared.Database.Entities.Identity? src) =>
        src is null
            ? null
            : new Identity
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                ModifiedAt = src.ModifiedAt,
                Email = src.Email,
                EmailVerified = src.EmailVerified
            };

    private static Organization? MapTo(Shared.Database.Entities.Organization? src) =>
        src is null
            ? null
            : new Organization
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                EventRaisedAt = src.EventRaisedAt,
                CustomDomain = src.CustomDomain,
                Type = src.Type.ToOrganizationType(),
                IsOwnershipVerified = src.IsOwnershipVerified
            };

    private static Location? MapTo(Shared.Database.Entities.Location? src, bool includeResources) =>
        src is null
            ? null
            : new Location
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                EventRaisedAt = src.EventRaisedAt,
                Organization = MapTo(src.Organization),
                Type = src.Type.ToNullableLocationType(),
                Resources = includeResources ? MapTo(src.Resources).ToList() : []
            };

    private static IEnumerable<Shared.Models.Resource> MapTo(IEnumerable<Resource?>? src) =>
        (src is null ? [] : src.Where(item => item is not null).Select(MapTo))!;

    private static Shared.Models.Resource? MapTo(Resource? src) =>
        src is null
            ? null
            : new Shared.Models.Resource
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                EventRaisedAt = src.EventRaisedAt,
                Location = MapTo(src.Location, false)!
            };

    private static IEnumerable<OrganizationTag> MapTo(IEnumerable<Shared.Database.Entities.OrganizationTag?>? src) =>
        (src is null ? [] : src.Where(item => item is not null).Select(MapTo))!;

    private static OrganizationTag? MapTo(Shared.Database.Entities.OrganizationTag? src) =>
        src is null
            ? null
            : new OrganizationTag
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                EventRaisedAt = src.EventRaisedAt,
                Name = src.Name,
                Type = src.Type.ToNullableOrganizationTagType(),
                Color = src.Color
            };

    private static CustomerBillingDetails? MapTo(Shared.Database.Entities.CustomerBillingDetails? src) =>
        src is null
            ? null
            : new CustomerBillingDetails
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                ModifiedAt = src.ModifiedAt,
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
                CountryCode = src.CountryCode
            };
}
