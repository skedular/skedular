using Api.Shared.Clients.Events.Skedular.Organization.V1.Value;
using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Booking.Shared.Database.Entities;
using Enterprise.Shared;
using Customer = Booking.Shared.Database.Entities.Customer;
using CustomerType = Api.Shared.Clients.Events.Skedular.Customer.V1.Value.CustomerType;
using Event = Api.Shared.Clients.Events.Skedular.Customer.V1.Value.Event;
using Location = Booking.Shared.Models.Location;
using Offering = Api.Shared.Services.Models.Offering;
using Organization = Booking.Shared.Models.Organization;
using OrganizationMember = Booking.Shared.Database.Entities.OrganizationMember;
using OrganizationSsoSetting = Booking.Shared.Models.OrganizationSsoSetting;
using OrganizationType = Api.Shared.Clients.Events.Skedular.Organization.V1.Value.OrganizationType;
using Product = Booking.Shared.Models.Product;
using ProductVersion = Booking.Shared.Models.ProductVersion;
using ProductPricingCadence = Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.ProductPricingCadence;
using Role = Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Role;
using Team = Booking.Shared.Models.Team;
using TeamMember = Booking.Shared.Database.Entities.TeamMember;
using ProductPricing = Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.ProductPricing;
using PaymentMethod = Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.PaymentMethod;
using Currency = Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.Currency;

namespace Booking.Processors.Mappers;

public interface IMapper
{
    Shared.Models.Customer MapTo(Event src);
    Organization MapTo(Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Event src);
    Location MapTo(Api.Shared.Clients.Events.Skedular.Location.V1.Value.Event src);
    Team MapTo(Api.Shared.Clients.Events.Skedular.Team.V1.Value.Event src);
    Shared.Database.Entities.Organization MergeToEntity(Organization src, Shared.Database.Entities.Organization dest);
    Product MapTo(Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.Event src);

    Shared.Database.Entities.Product MergeToEntity(
        Product src,
        Shared.Database.Entities.Product dest,
        Shared.Database.Entities.Organization organization,
        ICollection<Shared.Database.Entities.ProductVersion> productVersions);

    Shared.Database.Entities.ProductVersion MergeToEntity(
        ProductVersion src,
        Shared.Database.Entities.ProductVersion dest,
        Shared.Database.Entities.Product product,
        ICollection<OrganizationTag> productTags,
        ICollection<OrganizationTag> locationTags);

    Shared.Database.Entities.Location MergeToEntity(
        Location src,
        Shared.Database.Entities.Location dest,
        Shared.Database.Entities.Organization organization,
        ICollection<OrganizationTag> organizationTags);

    Shared.Database.Entities.Team MergeToEntity(Team src, Shared.Database.Entities.Team dest, Shared.Database.Entities.Organization organization);
    OrganizationMember MapToEntity(Shared.Models.OrganizationMember src, Shared.Database.Entities.Organization organization, Customer customer);

    OrganizationMember MergeToEntity(
        Shared.Models.OrganizationMember src,
        OrganizationMember dest,
        Shared.Database.Entities.Organization organization,
        Customer customer);

    TeamMember MapToEntity(Shared.Models.TeamMember src, Shared.Database.Entities.Team organization, Customer customer);
    TeamMember MergeToEntity(Shared.Models.TeamMember src, TeamMember dest, Shared.Database.Entities.Team team, Customer customer);
    Resource MapToEntity(Shared.Models.Resource src, Shared.Database.Entities.Location location, ICollection<OrganizationTag> organizationTags);

    Resource MergeToEntity(
        Shared.Models.Resource src,
        Resource dest,
        Shared.Database.Entities.Location? location,
        ICollection<OrganizationTag> organizationTags);

    Customer MergeToEntity(
        Shared.Models.Customer src,
        Customer dest,
        ICollection<Identity> identities,
        Shared.Database.Entities.Organization? defaultOrganization,
        ICollection<Shared.Database.Entities.Location> preferredLocations,
        ICollection<Resource> preferredResources,
        ICollection<OrganizationTag> preferredOrganizationTags);

    Identity MapToEntity(Shared.Models.Identity src, Customer? customer);
    Identity MergeToEntity(Shared.Models.Identity src, Identity dest, Customer? customer);
    OrganizationTag MapToEntity(Shared.Models.OrganizationTag src, Shared.Database.Entities.Organization organization);
    OrganizationTag MergeToEntity(Shared.Models.OrganizationTag src, OrganizationTag dest, Shared.Database.Entities.Organization organization);
    Shared.Database.Entities.OrganizationSsoSetting MapTo(OrganizationSsoSetting src, Shared.Database.Entities.Organization organization);

    Shared.Database.Entities.OrganizationSsoSetting MergeTo(
        OrganizationSsoSetting src,
        Shared.Database.Entities.OrganizationSsoSetting dest,
        Shared.Database.Entities.Organization organization);
}

public class Mapper : IMapper
{
    public Shared.Models.Customer MapTo(Event src)
    {
        var customer = src.Data.Customer;
        var deletedAt = customer.DeletedAt?.ToDateTimeOffset();
        var eventRaisedAt = src.Metadata.Time?.ToDateTimeOffset() ?? DateTimeOffset.MinValue;

        return new Shared.Models.Customer
        {
            Id = customer.Id,
            DeletedAt = deletedAt,
            EventRaisedAt = eventRaisedAt,
            Designation = customer.Designation,
            Title = customer.Title,
            Timezone = customer.Timezone,
            Locale = customer.Locale,
            Name = customer.Name,
            GivenName = customer.GivenName,
            MiddleName = customer.MiddleName,
            FamilyName = customer.FamilyName,
            PhotoUrl = customer.PhotoUrl,
            PhotoUrl24 = customer.PhotoUrl24,
            PhotoUrl32 = customer.PhotoUrl32,
            PhotoUrl48 = customer.PhotoUrl48,
            PhotoUrl72 = customer.PhotoUrl72,
            PhotoUrl192 = customer.PhotoUrl192,
            PhotoUrl512 = customer.PhotoUrl512,
            PhoneNumber = customer.PhoneNumber,
            Type = customer.Type switch
            {
                CustomerType.Guest => Api.Shared.Services.Models.CustomerType.Guest,
                CustomerType.Registered => Api.Shared.Services.Models.CustomerType.Registered,
                _ => throw new ArgumentOutOfRangeException()
            },
            Identities = customer.Identities.Select(item =>
                new Shared.Models.Identity { Id = item.Id, Email = item.Email.ToSafeString(), EmailVerified = item.EmailVerified }).ToList(),
            DefaultOrganization = string.IsNullOrWhiteSpace(customer.PreferredOrganizationId)
                ? null
                : new Organization { Id = customer.PreferredOrganizationId },
            PreferredLocations =
                customer.PreferredLocations
                    .Select(item => new Location { Id = item.Id, Organization = new Organization { Id = item.OrganizationId } }).ToList(),
            PreferredResources = customer.PreferredResources.Select(item =>
                new Shared.Models.Resource { Id = item.Id, Location = new Location { Id = item.LocationId } }).ToList(),
            PreferredOrganizationTags = customer.PreferredOrganizationTags.Select(item =>
                new Shared.Models.OrganizationTag { Id = item.Id, Organization = new Organization { Id = item.OrganizationId } }).ToList()
        };
    }

    public Organization MapTo(Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Event src)
    {
        var organizationAfterState = src.Data.Organization;
        var deletedAt = organizationAfterState.DeletedAt?.ToDateTimeOffset();
        var eventRaisedAt = src.Metadata.Time?.ToDateTimeOffset() ?? DateTimeOffset.MinValue;

        var organization = new Organization
        {
            Id = organizationAfterState.Id,
            DeletedAt = deletedAt,
            EventRaisedAt = eventRaisedAt,
            UniqueAlphanumericName =
                string.IsNullOrWhiteSpace(organizationAfterState.UniqueAlphanumericName) ? null : organizationAfterState.UniqueAlphanumericName,
            Name = organizationAfterState.Name,
            ContactEmail = organizationAfterState.ContactEmail,
            ContactPhone = organizationAfterState.ContactPhone,
            IsOwnershipVerified = organizationAfterState.IsOwnershipVerified,
            LogoUrl = organizationAfterState.LogoUrl,
            Offering = new Offering
            {
                Id = organizationAfterState.Offering.Id,
                Code = organizationAfterState.Offering.Code.ToOfferingCode(),
                Start = organizationAfterState.Offering.Start.ToDateTimeOffset(),
                End = organizationAfterState.Offering.End.ToDateTimeOffset(),
                ActiveCustomerIds = organizationAfterState.Offering.ActiveCustomerIds.ToArray()
            },
            Type = organizationAfterState.Type switch
            {
                OrganizationType.Private => Api.Shared.Services.Models.OrganizationType.Private,
                OrganizationType.Marketplace => Api.Shared.Services.Models.OrganizationType.Marketplace,
                OrganizationType.Individual => Api.Shared.Services.Models.OrganizationType.Individual,
                _ => throw new ArgumentOutOfRangeException()
            }
        };

        organization.OrganizationMembers = organizationAfterState.Members.Select(item => new Shared.Models.OrganizationMember
        {
            Id = item.Id,
            Role = item.Role switch
            {
                Role.Owner => OrganizationMemberRole.Owner,
                Role.Administrator => OrganizationMemberRole.Administrator,
                Role.Member => OrganizationMemberRole.Member,
                _ => throw new ArgumentOutOfRangeException()
            },
            Status = item.Status switch
            {
                Status.Active => OrganizationMemberStatus.Active,
                Status.Inactive => OrganizationMemberStatus.Inactive,
                _ => throw new ArgumentOutOfRangeException()
            },
            Customer = new Shared.Models.Customer { Id = item.CustomerId },
            Organization = organization
        }).ToList();

        organization.Tags = organizationAfterState.Tags.Select(item => new Shared.Models.OrganizationTag
        {
            Id = item.Id,
            DeletedAt = deletedAt,
            EventRaisedAt = eventRaisedAt,
            Type = item.Type.ToNullableOrganizationTagType(),
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

    public Location MapTo(Api.Shared.Clients.Events.Skedular.Location.V1.Value.Event src)
    {
        var locationAfterState = src.Data.Location;
        var deletedAt = locationAfterState.DeletedAt?.ToDateTimeOffset();
        var eventRaisedAt = src.Metadata.Time?.ToDateTimeOffset() ?? DateTimeOffset.MinValue;

        var location = new Location
        {
            Id = locationAfterState.Id,
            DeletedAt = deletedAt,
            EventRaisedAt = eventRaisedAt,
            OpeningHours = MapTo(locationAfterState.OpeningHours),
            Organization = new Organization { Id = locationAfterState.OrganizationId }
        };

        location.OrganizationTags = locationAfterState.TagIds
            .Select(item => new Shared.Models.OrganizationTag { Id = item, Organization = location.Organization })
            .ToList();

        var resourceOrganizationTags = locationAfterState.Resources
            .SelectMany(item => item.TagIds)
            .Select(item => new Shared.Models.OrganizationTag { Id = item, Organization = location.Organization });

        location.Resources = locationAfterState.Resources.Select(item => new Shared.Models.Resource
        {
            Id = item.Id,
            DeletedAt = deletedAt,
            EventRaisedAt = eventRaisedAt,
            Inactive = item.Inactive,
            RequireBookingApproval = item.RequireBookingApproval,
            Capacity = item.Capacity,
            IsAvailableHoursOverridden = item.IsAvailableHoursOverridden,
            AvailableHours = item.AvailableHours is null ? null : MapTo(item.AvailableHours),
            OrganizationTags = resourceOrganizationTags.Where(tag => item.TagIds.Contains(tag.Id)).ToList(),
            Location = location
        }).ToList();

        return location;
    }

    public Team MapTo(Api.Shared.Clients.Events.Skedular.Team.V1.Value.Event src)
    {
        var teamAfterState = src.Data.Team;
        var deletedAt = teamAfterState.DeletedAt?.ToDateTimeOffset();
        var eventRaisedAt = src.Metadata.Time?.ToDateTimeOffset() ?? DateTimeOffset.MinValue;

        var team = new Team
        {
            Id = teamAfterState.Id,
            DeletedAt = deletedAt,
            EventRaisedAt = eventRaisedAt,
            Organization = new Organization { Id = teamAfterState.OrganizationId }
        };

        team.TeamMembers = teamAfterState.Members.Select(item => new Shared.Models.TeamMember
        {
            Id = item.Id,
            DeletedAt = deletedAt,
            EventRaisedAt = eventRaisedAt,
            Role = item.Role switch
            {
                Api.Shared.Clients.Events.Skedular.Team.V1.Value.Role.Owner => TeamMemberRole.Owner,
                Api.Shared.Clients.Events.Skedular.Team.V1.Value.Role.Administrator => TeamMemberRole.Administrator,
                Api.Shared.Clients.Events.Skedular.Team.V1.Value.Role.Member => TeamMemberRole.Member,
                _ => throw new ArgumentOutOfRangeException()
            },
            Status = item.Status switch
            {
                Api.Shared.Clients.Events.Skedular.Team.V1.Value.Status.Active => TeamMemberStatus.Active,
                Api.Shared.Clients.Events.Skedular.Team.V1.Value.Status.Inactive => TeamMemberStatus.Inactive,
                _ => throw new ArgumentOutOfRangeException()
            },
            Customer = new Shared.Models.Customer { Id = item.CustomerId },
            Team = team
        }).ToList();

        return team;
    }

    public Shared.Database.Entities.Organization MergeToEntity(Organization src, Shared.Database.Entities.Organization dest)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.UniqueAlphanumericName = src.UniqueAlphanumericName;
        dest.Name = src.Name;
        dest.ContactEmail = src.ContactEmail;
        dest.ContactPhone = src.ContactPhone;
        dest.IsOwnershipVerified = src.IsOwnershipVerified;
        dest.LogoUrl = src.LogoUrl;
        dest.Offering = src.Offering;
        dest.Type = src.Type.ToOrganizationType();
        return dest;
    }

    public Product MapTo(Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.Event src)
    {
        var productAfterState = src.Data.Product;
        var deletedAt = productAfterState.DeletedAt?.ToDateTimeOffset();
        var eventRaisedAt = src.Metadata.Time?.ToDateTimeOffset() ?? DateTimeOffset.MinValue;

        var product = new Product
        {
            Id = productAfterState.Id,
            DeletedAt = deletedAt,
            EventRaisedAt = eventRaisedAt,
            Organization = new Organization { Id = productAfterState.OrganizationId }
        };

        product.ProductVersions = new List<ProductVersion> { MapTo(productAfterState.LatestProductVersion, product) };

        return product;
    }

    public Shared.Database.Entities.Product MergeToEntity(
        Product src,
        Shared.Database.Entities.Product dest,
        Shared.Database.Entities.Organization organization,
        ICollection<Shared.Database.Entities.ProductVersion> productVersions)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Organization = organization;
        dest.ProductVersions = productVersions;
        return dest;
    }

    public Shared.Database.Entities.ProductVersion MergeToEntity(
        ProductVersion src,
        Shared.Database.Entities.ProductVersion dest,
        Shared.Database.Entities.Product product,
        ICollection<OrganizationTag> productTags,
        ICollection<OrganizationTag> locationTags)
    {
        dest.Id = src.Id;
        dest.Name = src.Name;
        dest.Currency = src.Currency.ToCurrency();
        dest.Product = product;
        dest.ProductTags = productTags;
        dest.LocationTags = locationTags;
        dest.PricingOptions = src.PricingOptions;
        return dest;
    }

    public Shared.Database.Entities.Location MergeToEntity(
        Location src,
        Shared.Database.Entities.Location dest,
        Shared.Database.Entities.Organization organization,
        ICollection<OrganizationTag> organizationTags)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.OpeningHours = src.OpeningHours;
        dest.Organization = organization;
        dest.OrganizationTags = organizationTags;
        return dest;
    }

    public Shared.Database.Entities.Team MergeToEntity(
        Team src,
        Shared.Database.Entities.Team dest,
        Shared.Database.Entities.Organization organization)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Organization = organization;
        return dest;
    }

    public OrganizationMember MapToEntity(
        Shared.Models.OrganizationMember src,
        Shared.Database.Entities.Organization organization,
        Customer customer) => MergeToEntity(src, new OrganizationMember(), organization, customer);

    public OrganizationMember MergeToEntity(
        Shared.Models.OrganizationMember src,
        OrganizationMember dest,
        Shared.Database.Entities.Organization organization,
        Customer customer)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Role = src.Role.ToNullableOrganizationMemberRole();
        dest.Status = src.Status.ToOrganizationMemberStatus();
        dest.Organization = organization;
        dest.Customer = customer;
        return dest;
    }

    public TeamMember MapToEntity(Shared.Models.TeamMember src, Shared.Database.Entities.Team team, Customer customer) =>
        MergeToEntity(src, new TeamMember(), team, customer);

    public TeamMember MergeToEntity(
        Shared.Models.TeamMember src,
        TeamMember dest,
        Shared.Database.Entities.Team team,
        Customer customer)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Role = src.Role.ToNullableTeamMemberRole();
        dest.Status = src.Status.ToTeamMemberStatus();
        dest.Team = team;
        dest.Customer = customer;
        return dest;
    }

    public Resource MapToEntity(
        Shared.Models.Resource src,
        Shared.Database.Entities.Location location,
        ICollection<OrganizationTag> organizationTags) =>
        MergeToEntity(src, new Resource(), location, organizationTags);

    public Resource MergeToEntity(
        Shared.Models.Resource src,
        Resource dest,
        Shared.Database.Entities.Location? location,
        ICollection<OrganizationTag> organizationTags)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Inactive = src.Inactive;
        dest.RequireBookingApproval = src.RequireBookingApproval;
        dest.Capacity = src.Capacity;
        dest.IsAvailableHoursOverridden = src.IsAvailableHoursOverridden;
        dest.AvailableHours = src.AvailableHours;
        dest.Location = location;
        dest.OrganizationTags = organizationTags;
        return dest;
    }

    public Customer MergeToEntity(
        Shared.Models.Customer src,
        Customer dest,
        ICollection<Identity> identities,
        Shared.Database.Entities.Organization? defaultOrganization,
        ICollection<Shared.Database.Entities.Location> preferredLocations,
        ICollection<Resource> preferredResources,
        ICollection<OrganizationTag> preferredOrganizationTags)
    {
        dest.Id = src.Id;
        dest.Name = src.Name;
        dest.Designation = src.Designation;
        dest.Title = src.Title;
        dest.Timezone = src.Timezone;
        dest.Locale = src.Locale;
        dest.GivenName = src.GivenName;
        dest.MiddleName = src.MiddleName;
        dest.FamilyName = src.FamilyName;
        dest.PhotoUrl = src.PhotoUrl;
        dest.PhotoUrl24 = src.PhotoUrl24;
        dest.PhotoUrl32 = src.PhotoUrl32;
        dest.PhotoUrl48 = src.PhotoUrl48;
        dest.PhotoUrl72 = src.PhotoUrl72;
        dest.PhotoUrl192 = src.PhotoUrl192;
        dest.PhotoUrl512 = src.PhotoUrl512;
        dest.PhoneNumber = src.PhoneNumber;
        dest.Type = src.Type.ToNullableCustomerType();
        dest.Identities = identities;
        dest.DefaultOrganization = defaultOrganization;
        dest.PreferredLocations = preferredLocations;
        dest.PreferredResources = preferredResources;
        dest.PreferredOrganizationTags = preferredOrganizationTags;
        return dest;
    }

    public Identity MapToEntity(Shared.Models.Identity src, Customer? customer) => MergeToEntity(src, new Identity(), customer);

    public Identity MergeToEntity(Shared.Models.Identity src, Identity dest, Customer? customer)
    {
        dest.Id = src.Id;
        dest.Email = src.Email;
        dest.EmailVerified = src.EmailVerified;
        if (customer is not null)
        {
            dest.Customer = customer;
        }

        return dest;
    }

    public OrganizationTag MergeToEntity(Shared.Models.OrganizationTag src, OrganizationTag dest, Shared.Database.Entities.Organization organization)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Type = src.Type.ToNullableOrganizationTagType();
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

    public OrganizationTag MapToEntity(Shared.Models.OrganizationTag src, Shared.Database.Entities.Organization organization) =>
        MergeToEntity(src, new OrganizationTag(), organization);

    private static OpeningHours MapTo(Api.Shared.Clients.Events.Skedular.Location.V1.Value.OpeningHours src) =>
        new(
            MapTo(src.WeekOpeningHours),
            src.ClosedDates.Select(item => item.ToDateTimeOffset()).ToList(),
            src.DatesWithVariedOpeningHours.ToDictionary(item => item.Date.ToDateTimeOffset(), item => MapTo(item.OpeningHoursDetails)));

    private static WeekOpeningHours MapTo(Api.Shared.Clients.Events.Skedular.Location.V1.Value.WeekOpeningHours src) =>
        new(
            MapTo(src.Monday),
            MapTo(src.Tuesday),
            MapTo(src.Wednesday),
            MapTo(src.Thursday),
            MapTo(src.Friday),
            MapTo(src.Saturday),
            MapTo(src.Sunday));

    private static OpeningHoursDetails MapTo(Api.Shared.Clients.Events.Skedular.Location.V1.Value.OpeningHoursDetails src) =>
        new(
            src.Closed,
            src.OpenAllDay,
            string.IsNullOrWhiteSpace(src.From) ? null : TimeOnly.Parse(src.From),
            string.IsNullOrWhiteSpace(src.Until) ? null : TimeOnly.Parse(src.Until));

    private static ProductVersion MapTo(Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.ProductVersion src, Product product) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Currency = MapTo(src.Currency),
            ProductTags = src.ProductTagIds.Select(item => new Shared.Models.OrganizationTag { Id = item }).ToList(),
            LocationTags = src.LocationTagIds.Select(item => new Shared.Models.OrganizationTag { Id = item }).ToList(),
            Product = product,
            PricingOptions = MapTo(src.PricingOptions).ToList()
        };

    private static IEnumerable<Api.Shared.Services.Models.ProductPricing> MapTo(IEnumerable<ProductPricing> src) =>
        src.Select(MapTo);

    private static Api.Shared.Services.Models.ProductPricing MapTo(ProductPricing src) =>
        new(
            src.Id,
            src.Index,
            src.Name.ToSafeString(),
            src.Description.ToSafeString(),
            src.Cadence switch
            {
                ProductPricingCadence.OneTimeV1 => Api.Shared.Services.Models.ProductPricingCadence.OneTimeV1,
                ProductPricingCadence.PerMinuteV1 => Api.Shared.Services.Models.ProductPricingCadence.PerMinuteV1,
                ProductPricingCadence.PerHourV1 => Api.Shared.Services.Models.ProductPricingCadence.PerHourV1,
                ProductPricingCadence.DailyV1 => Api.Shared.Services.Models.ProductPricingCadence.DailyV1,
                ProductPricingCadence.WeeklyV1 => Api.Shared.Services.Models.ProductPricingCadence.WeeklyV1,
                ProductPricingCadence.MonthlyV1 => Api.Shared.Services.Models.ProductPricingCadence.MonthlyV1,
                _ => throw new ArgumentOutOfRangeException()
            },
            Convert.ToDecimal(src.Price),
            src.IsTaxInclusive,
            MapTo(src.AcceptedBookingPaymentMethods).ToList(),
            src.MinDurationMinutes.FromNullInt(),
            src.MaxDurationMinutes.FromNullInt(),
            src.MaxAllowedResourcesLockTimePaidViaCard,
            src.MaxAllowedResourcesLockTimePaidViaBankTransfer,
            src.NumberOfResourcesToBook);

    private static IEnumerable<Api.Shared.Services.Models.PaymentMethod> MapTo(IEnumerable<PaymentMethod> src) =>
        src.Select(MapTo);

    private static Api.Shared.Services.Models.PaymentMethod MapTo(PaymentMethod src) =>
        src switch
        {
            PaymentMethod.Card => Api.Shared.Services.Models.PaymentMethod.Card,
            PaymentMethod.BankTransfer => Api.Shared.Services.Models.PaymentMethod.BankTransfer,
            _ => throw new ArgumentOutOfRangeException(nameof(src), src, null)
        };

    private static Api.Shared.Services.Models.Currency MapTo(Currency src) =>
        src switch
        {
            Currency.Nzd => Api.Shared.Services.Models.Currency.Nzd,
            Currency.Usd => Api.Shared.Services.Models.Currency.Usd,
            _ => throw new ArgumentOutOfRangeException(nameof(src), src, null)
        };
}
