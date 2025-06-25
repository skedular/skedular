using Api.Shared.Clients.Events.Skedular.Organization.V1.Value;
using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Booking.Shared.Database.Entities;
using Enterprise.Shared;
using BookingCheckoutSession = Booking.Shared.Models.BookingCheckoutSession;
using Customer = Booking.Shared.Database.Entities.Customer;
using Event = Api.Shared.Clients.Events.Skedular.Customer.V1.Value.Event;
using Location = Booking.Shared.Models.Location;
using Offering = Api.Shared.Services.Models.Offering;
using Organization = Booking.Shared.Models.Organization;
using OrganizationMember = Booking.Shared.Database.Entities.OrganizationMember;
using OrganizationSsoSetting = Booking.Shared.Models.OrganizationSsoSetting;
using PaymentStatus = Api.Shared.Clients.Events.Skedular.Payment.V1.Value.PaymentStatus;
using Product = Booking.Shared.Models.Product;
using ProductVersion = Booking.Shared.Models.ProductVersion;
using ResourceBookingSlot = Booking.Shared.Models.ResourceBookingSlot;
using Role = Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Role;
using Team = Booking.Shared.Models.Team;
using TeamMember = Booking.Shared.Database.Entities.TeamMember;

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

    TeamMember MergeToEntity(
        Shared.Models.TeamMember src,
        TeamMember dest,
        Shared.Database.Entities.Team team,
        Customer customer);

    Resource MapToEntity(
        Shared.Models.Resource src,
        Shared.Database.Entities.Location location,
        ICollection<OrganizationTag> organizationTags);

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
        ICollection<Shared.Database.Entities.Team> preferredTeams,
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

    BookingCheckoutSession MapTo(Api.Shared.Clients.Events.Skedular.Payment.V1.Value.Event src);

    Shared.Database.Entities.BookingCheckoutSession MergeToEntity(
        BookingCheckoutSession src,
        Shared.Database.Entities.BookingCheckoutSession dest,
        Shared.Database.Entities.Booking booking);

    Shared.Models.Booking MapTo(Shared.Database.Entities.Booking src);
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
            PreferredTeams =
                customer.PreferredTeams.Select(item => new Team { Id = item.Id, Organization = new Organization { Id = item.OrganizationId } })
                    .ToList(),
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
            Name = organizationAfterState.Name,
            LogoUrl = organizationAfterState.LogoUrl,
            Offering = new Offering
            {
                Id = organizationAfterState.Offering.Id,
                Code = organizationAfterState.Offering.Code.ToOfferingCode(),
                Start = organizationAfterState.Offering.Start.ToDateTimeOffset(),
                End = organizationAfterState.Offering.End.ToDateTimeOffset(),
                ActiveCustomerIds = organizationAfterState.Offering.ActiveCustomerIds.ToArray()
            },
            Type = organizationAfterState.Type.ToOrganizationType(),
            MemberVisibilityPolicy = organizationAfterState.MemberVisibilityPolicy.ToOrganizationMemberVisibilityPolicy()
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
            Name = item.Name,
            Type = item.Type.ToNullableOrganizationTagType(),
            Color = item.Color,
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
            Name = locationAfterState.Name,
            OpeningHours = MapTo(locationAfterState.OpeningHours),
            Organization = new Organization { Id = locationAfterState.OrganizationId }
        };

        location.OrganizationTags = locationAfterState.TagIds
            .Select(item => new Shared.Models.OrganizationTag { Id = item, Organization = location.Organization }).ToList();

        var resourceOrganizationTags = locationAfterState.Resources
            .SelectMany(item => item.TagIds)
            .Select(item => new Shared.Models.OrganizationTag { Id = item, Organization = location.Organization });

        location.Resources = locationAfterState.Resources.Select(item => new Shared.Models.Resource
        {
            Id = item.Id,
            DeletedAt = deletedAt,
            EventRaisedAt = eventRaisedAt,
            Name = item.Name,
            Inactive = item.Inactive,
            RequireBookingApproval = item.RequireBookingApproval,
            Color = item.Color,
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
            Name = teamAfterState.Name,
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
        dest.Name = src.Name;
        dest.LogoUrl = src.LogoUrl;
        dest.Offering = src.Offering;
        dest.Type = src.Type.ToOrganizationType();
        dest.MemberVisibilityPolicy = src.MemberVisibilityPolicy.ToOrganizationMemberVisibilityPolicy();
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
        dest.Price = src.Price;
        dest.PriceUnit = src.PriceUnit.ToPriceUnit();
        dest.PricePerMinute = src.Price;
        dest.Currency = src.Currency.ToCurrency();
        dest.MinDurationMinutes = src.MinDurationMinutes;
        dest.MaxDurationMinutes = src.MaxDurationMinutes;
        dest.BookAllLocationResources = src.BookAllLocationResources;
        dest.RecurrenceWindowDays = src.RecurrenceWindowDays;
        dest.RequireConsecutiveDays = src.RequireConsecutiveDays;
        dest.MaxBookingSpreadDays = src.MaxBookingSpreadDays;
        dest.NumberOfResourcesToBook = src.NumberOfResourcesToBook;
        dest.Product = product;
        dest.ProductTags = productTags;
        dest.LocationTags = locationTags;
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
        dest.Name = src.Name;
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
        dest.Name = src.Name;
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
        dest.Name = src.Name;
        dest.Inactive = src.Inactive;
        dest.RequireBookingApproval = src.RequireBookingApproval;
        dest.Color = src.Color;
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
        ICollection<Shared.Database.Entities.Team> preferredTeams,
        ICollection<Resource> preferredResources,
        ICollection<OrganizationTag> preferredOrganizationTags)
    {
        dest.Id = src.Id;
        dest.Name = src.Name;
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
        dest.Identities = identities;
        dest.DefaultOrganization = defaultOrganization;
        dest.PreferredLocations = preferredLocations;
        dest.PreferredResources = preferredResources;
        dest.PreferredTeams = preferredTeams;
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

    public BookingCheckoutSession MapTo(Api.Shared.Clients.Events.Skedular.Payment.V1.Value.Event src)
    {
        var bookingCheckoutSession = src.Data.BookingCheckoutSession;
        var deletedAt = bookingCheckoutSession.DeletedAt?.ToDateTimeOffset();
        var eventRaisedAt = src.Metadata.Time?.ToDateTimeOffset() ?? DateTimeOffset.MinValue;

        return new BookingCheckoutSession
        {
            Id = bookingCheckoutSession.Id,
            DeletedAt = deletedAt,
            EventRaisedAt = eventRaisedAt,
            CheckoutUrl = bookingCheckoutSession.CheckoutUrl,
            PaymentStatus = bookingCheckoutSession.PaymentStatus switch
            {
                PaymentStatus.NoPaymentRequired => Api.Shared.Services.Models.PaymentStatus.NoPaymentRequired,
                PaymentStatus.Pending => Api.Shared.Services.Models.PaymentStatus.Pending,
                PaymentStatus.Paid => Api.Shared.Services.Models.PaymentStatus.Paid,
                PaymentStatus.Unpaid => Api.Shared.Services.Models.PaymentStatus.Unpaid,
                PaymentStatus.Expired => Api.Shared.Services.Models.PaymentStatus.Expired,
                _ => throw new ArgumentOutOfRangeException()
            },
            AmountTotal =
                string.IsNullOrWhiteSpace(bookingCheckoutSession.AmountTotal) ? null : bookingCheckoutSession.AmountTotal.FromRoundedPrice(),
            Currency = string.IsNullOrWhiteSpace(bookingCheckoutSession.Currency) ? null : bookingCheckoutSession.Currency,
            Booking = new Shared.Models.Booking { Id = bookingCheckoutSession.BookingId }
        };
    }

    public Shared.Database.Entities.BookingCheckoutSession MergeToEntity(
        BookingCheckoutSession src,
        Shared.Database.Entities.BookingCheckoutSession dest,
        Shared.Database.Entities.Booking booking)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.CheckoutUrl = src.CheckoutUrl;
        dest.PaymentStatus = src.PaymentStatus.ToPaymentStatus();
        dest.AmountTotal = src.AmountTotal;
        dest.Currency = src.Currency;
        dest.Booking = booking;
        return dest;
    }

    public Shared.Models.Booking MapTo(Shared.Database.Entities.Booking src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            From = src.From,
            Until = src.Until,
            Notes = src.Notes,
            Type = src.Type.ToBookingType(),
            Status = src.Status.ToBookingStatus(),
            IsPaymentRequired = src.IsPaymentRequired,
            Schedules = src.Schedules,
            LineItems = src.LineItems,
            BookedOnMarketplace = src.BookedOnMarketplace,
            ResourceBookingSlots = MapTo(src.ResourceBookingSlots).ToList(),
            InvolvedCustomers = MapTo(src.InvolvedCustomers).ToList(),
            InvolvedOrganizations = MapTo(src.InvolvedOrganizations).ToList(),
            InvolvedLocations = MapTo(src.InvolvedLocations).ToList(),
            InvolvedTeams = MapTo(src.InvolvedTeams).ToList(),
            PaidByCustomer = MapTo(src.PaidByCustomer),
            PaidByOrganization = MapTo(src.PaidByOrganization),
            CreatedByCustomer = MapTo(src.CreatedByCustomer),
            LastModifiedByCustomer = MapTo(src.LastModifiedByCustomer),
            DeletedByCustomer = MapTo(src.DeletedByCustomer),
            BookingCheckoutSession = MapTo(src.BookingCheckoutSession),
            ProductVersions = MapTo(src.ProductVersions).ToList()
        };

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
            Price = src.Price.FromRoundedPrice(),
            PriceUnit = src.PriceUnit.ToPriceUnit(),
            PricePerMinute = src.Price.FromRoundedPrice(),
            Currency = src.Currency.ToCurrency(),
            MinDurationMinutes = src.MinDurationMinutes == -1 ? null : src.MinDurationMinutes,
            MaxDurationMinutes = src.MaxDurationMinutes == -1 ? null : src.MaxDurationMinutes,
            BookAllLocationResources = src.BookAllLocationResources,
            RecurrenceWindowDays = src.RecurrenceWindowDays,
            RequireConsecutiveDays = src.RequireConsecutiveDays,
            MaxBookingSpreadDays = src.MaxBookingSpreadDays == -1 ? null : src.MaxBookingSpreadDays,
            NumberOfResourcesToBook = src.NumberOfResourcesToBook,
            ProductTags = src.ProductTagIds.Select(item => new Shared.Models.OrganizationTag { Id = item }).ToList(),
            LocationTags = src.LocationTagIds.Select(item => new Shared.Models.OrganizationTag { Id = item }).ToList(),
            Product = product
        };

    private static IEnumerable<ResourceBookingSlot> MapTo(IEnumerable<Shared.Database.Entities.ResourceBookingSlot> src) => src.Select(MapTo);

    private static ResourceBookingSlot MapTo(Shared.Database.Entities.ResourceBookingSlot src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            Available = src.Available,
            Start = src.Start,
            Customers = MapTo(src.Customers).ToList(),
            Resource = MapTo(src.Resource)
        };

    private static IEnumerable<Shared.Models.Customer> MapTo(IEnumerable<Customer> src) => src.Select(MapTo)!;

    private static Shared.Models.Customer? MapTo(Customer? src) =>
        src is null
            ? null
            : new Shared.Models.Customer
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
                Identities = MapTo(src.Identities).ToList()
            };

    private static IEnumerable<Shared.Models.Identity> MapTo(IEnumerable<Identity> src) => src.Select(MapTo);

    private static Shared.Models.Identity MapTo(Identity src) =>
        new() { Id = src.Id, Email = src.Email, EmailVerified = src.EmailVerified };

    private static Shared.Models.Resource MapTo(Resource src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            EventRaisedAt = src.EventRaisedAt,
            Name = src.Name,
            Capacity = src.Capacity,
            Inactive = src.Inactive,
            RequireBookingApproval = src.RequireBookingApproval,
            Color = src.Color,
            OrganizationTags = MapTo(src.OrganizationTags).ToList()
        };

    private static IEnumerable<Shared.Models.OrganizationTag> MapTo(IEnumerable<OrganizationTag> src) => src.Select(MapTo);

    private static Shared.Models.OrganizationTag MapTo(OrganizationTag src) =>
        new() { Id = src.Id, Name = src.Name, Type = src.Type.ToNullableOrganizationTagType(), Color = src.Color };

    private static IEnumerable<Organization> MapTo(IEnumerable<Shared.Database.Entities.Organization> src) => src.Select(MapTo)!;

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
                Name = src.Name,
                LogoUrl = src.LogoUrl,
                Offering = src.Offering,
                Type = src.Type.ToOrganizationType(),
                MemberVisibilityPolicy = src.MemberVisibilityPolicy.ToOrganizationMemberVisibilityPolicy()
            };

    private IEnumerable<Location> MapTo(IEnumerable<Shared.Database.Entities.Location> src) => src.Select(MapTo)!;

    public Location? MapTo(Shared.Database.Entities.Location? src) =>
        src is null
            ? null
            : new Location
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                EventRaisedAt = src.EventRaisedAt,
                Name = src.Name,
                OrganizationTags = MapTo(src.OrganizationTags).ToList()
            };

    private static IEnumerable<Team> MapTo(IEnumerable<Shared.Database.Entities.Team> src) => src.Select(MapTo)!;

    private static Team? MapTo(Shared.Database.Entities.Team? src) =>
        src is null
            ? null
            : new Team
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                EventRaisedAt = src.EventRaisedAt,
                Name = src.Name
            };

    private static BookingCheckoutSession? MapTo(Shared.Database.Entities.BookingCheckoutSession? src) =>
        src is null
            ? null
            : new BookingCheckoutSession
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                ModifiedAt = src.ModifiedAt,
                DeletedAt = src.DeletedAt,
                EventRaisedAt = src.EventRaisedAt,
                PaymentStatus = src.PaymentStatus?.ToNullablePaymentStatus() ?? Api.Shared.Services.Models.PaymentStatus.Pending,
                AmountTotal = src.AmountTotal,
                Currency = src.Currency,
                CheckoutUrl = src.CheckoutUrl.ToSafeString()
            };

    private static IEnumerable<ProductVersion> MapTo(IEnumerable<Shared.Database.Entities.ProductVersion> src) =>
        src.Select(MapTo);

    private static ProductVersion MapTo(Shared.Database.Entities.ProductVersion src)
    {
        ArgumentNullException.ThrowIfNull(src.PriceUnit);
        ArgumentNullException.ThrowIfNull(src.PricePerMinute);
        ArgumentNullException.ThrowIfNull(src.Currency);
        ArgumentNullException.ThrowIfNull(src.BookAllLocationResources);
        ArgumentNullException.ThrowIfNull(src.RecurrenceWindowDays);
        ArgumentNullException.ThrowIfNull(src.RequireConsecutiveDays);
        ArgumentNullException.ThrowIfNull(src.NumberOfResourcesToBook);

        return new ProductVersion
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            Name = src.Name.ToSafeString(),
            Price = src.Price ?? 0,
            PriceUnit = src.PriceUnit.ToPriceUnit(),
            PricePerMinute = src.PricePerMinute.Value,
            Currency = src.Currency.ToCurrency(),
            MinDurationMinutes = src.MinDurationMinutes,
            MaxDurationMinutes = src.MaxDurationMinutes,
            BookAllLocationResources = src.BookAllLocationResources.Value,
            RecurrenceWindowDays = src.RecurrenceWindowDays.Value,
            RequireConsecutiveDays = src.RequireConsecutiveDays.Value,
            MaxBookingSpreadDays = src.MaxBookingSpreadDays,
            NumberOfResourcesToBook = src.NumberOfResourcesToBook.Value
        };
    }
}
