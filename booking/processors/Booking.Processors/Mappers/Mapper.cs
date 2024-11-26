using Api.Shared.Clients.Events.UnityHub.Organization.V1.Value;
using Api.Shared.Models;
using Api.Shared.Services.Offering;
using Booking.Shared.Database.Entities;
using Enterprise.Shared;
using Customer = Booking.Shared.Database.Entities.Customer;
using Desk = Booking.Shared.Database.Entities.Desk;
using Event = Api.Shared.Clients.Events.UnityHub.Customer.V1.Value.Event;
using Location = Booking.Shared.Models.Location;
using LocationMember = Booking.Shared.Database.Entities.LocationMember;
using LocationTag = Booking.Shared.Database.Entities.LocationTag;
using Offering = Booking.Shared.Models.Offering;
using Organization = Booking.Shared.Models.Organization;
using OrganizationMember = Booking.Shared.Database.Entities.OrganizationMember;
using Team = Booking.Shared.Models.Team;
using TeamMember = Booking.Shared.Database.Entities.TeamMember;

namespace Booking.Processors.Mappers;

public interface IMapper
{
    Shared.Models.Customer MapTo(Event src);
    Organization MapTo(Api.Shared.Clients.Events.UnityHub.Organization.V1.Value.Event src);
    Location MapTo(Api.Shared.Clients.Events.UnityHub.Location.V1.Value.Event src);
    Team MapTo(Api.Shared.Clients.Events.UnityHub.Team.V1.Value.Event src);
    Shared.Models.Booking MapTo(Api.Shared.Clients.Events.UnityHub.Booking.V1.Value.Event src);
    Shared.Database.Entities.Organization MapToEntity(Organization src);
    Shared.Database.Entities.Organization MergeToEntity(Organization src, Shared.Database.Entities.Organization dest);
    Shared.Database.Entities.Location MapToEntity(Location src, Shared.Database.Entities.Organization? organization);

    Shared.Database.Entities.Location MergeToEntity(
        Location src,
        Shared.Database.Entities.Location dest,
        Shared.Database.Entities.Organization? organization);

    Shared.Database.Entities.Team MapToEntity(Team src, Shared.Database.Entities.Organization? organization);

    Shared.Database.Entities.Team MergeToEntity(
        Team src,
        Shared.Database.Entities.Team dest,
        Shared.Database.Entities.Organization? organization);

    OrganizationMember MapToEntity(
        Shared.Models.OrganizationMember src,
        Shared.Database.Entities.Organization organization,
        Customer customer);

    OrganizationMember MergeToEntity(
        Shared.Models.OrganizationMember src,
        OrganizationMember dest,
        Shared.Database.Entities.Organization organization,
        Customer customer);

    LocationMember MapToEntity(
        Shared.Models.LocationMember src,
        Shared.Database.Entities.Location organization,
        Customer customer);

    LocationMember MergeToEntity(
        Shared.Models.LocationMember src,
        LocationMember dest,
        Shared.Database.Entities.Location location,
        Customer customer);

    TeamMember MapToEntity(Shared.Models.TeamMember src, Shared.Database.Entities.Team organization, Customer customer);

    TeamMember MergeToEntity(
        Shared.Models.TeamMember src,
        TeamMember dest,
        Shared.Database.Entities.Team team,
        Customer customer);

    LocationTag MapToEntity(Shared.Models.LocationTag src, Shared.Database.Entities.Location location);

    LocationTag MergeToEntity(
        Shared.Models.LocationTag src,
        LocationTag dest,
        Shared.Database.Entities.Location location);

    Desk MapToEntity(
        Shared.Models.Desk src,
        Shared.Database.Entities.Location location,
        ICollection<LocationTag> locationTags);

    Desk MergeToEntity(
        Shared.Models.Desk src,
        Desk dest,
        Shared.Database.Entities.Location location,
        ICollection<LocationTag> locationTags);

    IEnumerable<Identity> MapToEntity(
        IEnumerable<Shared.Models.Identity> src,
        Customer? customer);

    Customer MapToEntity(
        Shared.Models.Customer src,
        ICollection<Identity> identities,
        Shared.Database.Entities.Organization? defaultOrganization,
        ICollection<Shared.Database.Entities.Location> defaultLocations,
        ICollection<Shared.Database.Entities.Team> defaultTeams,
        ICollection<LocationTag> preferredLocationTags,
        ICollection<Desk> preferredDesks);

    Customer MergeToEntity(
        Shared.Models.Customer src,
        Customer dest,
        ICollection<Identity> identities,
        Shared.Database.Entities.Organization? defaultOrganization,
        ICollection<Shared.Database.Entities.Location> defaultLocations,
        ICollection<Shared.Database.Entities.Team> defaultTeams,
        ICollection<LocationTag> preferredLocationTags,
        ICollection<Desk> preferredDesks);

    Identity MapToEntity(Shared.Models.Identity src, Customer? customer);

    Identity MergeToEntity(
        Shared.Models.Identity src,
        Identity dest,
        Customer? customer);

    Shared.Database.Entities.Booking MapToEntity(
        Shared.Models.Booking src,
        Customer customer,
        Shared.Database.Entities.Organization? organization,
        Shared.Database.Entities.Location? location,
        ICollection<Desk> desks,
        Shared.Database.Entities.Team? team);

    Shared.Database.Entities.Booking MergeToEntity(
        Shared.Models.Booking src,
        Shared.Database.Entities.Booking dest,
        Customer customer,
        Shared.Database.Entities.Organization? organization,
        Shared.Database.Entities.Location? location,
        ICollection<Desk> desks,
        Shared.Database.Entities.Team? team);

    OrganizationTag MergeToEntity(
        Shared.Models.OrganizationTag src,
        OrganizationTag dest,
        Shared.Database.Entities.Organization organization);

    OrganizationTag MapToEntity(
        Shared.Models.OrganizationTag src,
        Shared.Database.Entities.Organization organization);
}

public class Mapper : IMapper
{
    public Shared.Models.Customer MapTo(Event src)
    {
        var customer = src.Data.AfterState;
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
                    new Shared.Models.Identity
                    {
                        Id = item.Id, Email = item.Email.ToSafeString(), EmailVerified = item.EmailVerified
                    })
                .ToList(),
            DefaultOrganization = string.IsNullOrWhiteSpace(customer.DefaultOrganizationId)
                ? null
                : new Organization { Id = customer.DefaultOrganizationId },
            DefaultLocations = customer.DefaultLocations.Select(item => new Location
            {
                Id = item.Id,
                Organization =
                    string.IsNullOrWhiteSpace(item.OrganizationId)
                        ? null
                        : new Organization { Id = item.OrganizationId }
            }).ToList(),
            PreferredLocationTags = customer.DefaultLocationTags.Select(item =>
                    new Shared.Models.LocationTag { Id = item.Id, Location = new Location { Id = item.LocationId } })
                .ToList(),
            PreferredDesks = customer.DefaultDesks.Select(item =>
                new Shared.Models.Desk { Id = item.Id, Location = new Location { Id = item.LocationId } }).ToList(),
            DefaultTeams = customer.DefaultTeams.Select(item => new Team
            {
                Id = item.Id,
                Organization =
                    string.IsNullOrWhiteSpace(item.OrganizationId)
                        ? null
                        : new Organization { Id = item.OrganizationId }
            }).ToList()
        };
    }

    public Organization MapTo(Api.Shared.Clients.Events.UnityHub.Organization.V1.Value.Event src)
    {
        var organizationAfterState = src.Data.OrganizationAfterState;
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
            }
        };

        organization.OrganizationMembers = organizationAfterState.Members.Select(item =>
        {
            return new Shared.Models.OrganizationMember
            {
                Id = item.Id,
                MembershipType = item.MembershipType switch
                {
                    MembershipType.Owner => OrganizationMembershipType.Owner,
                    MembershipType.Administrator => OrganizationMembershipType.Administrator,
                    MembershipType.Member => OrganizationMembershipType.Member,
                    _ => throw new ArgumentOutOfRangeException()
                },
                Customer = new Shared.Models.Customer { Id = item.CustomerId },
                Organization = organization
            };
        }).ToList();

        organization.Tags = organizationAfterState.Tags.Select(item => new Shared.Models.OrganizationTag
        {
            Id = item.Id,
            DeletedAt = deletedAt,
            EventRaisedAt = eventRaisedAt,
            Name = item.Name,
            Type = item.TagType,
            Organization = organization
        }).ToList();

        return organization;
    }

    public Location MapTo(Api.Shared.Clients.Events.UnityHub.Location.V1.Value.Event src)
    {
        var locationAfterState = src.Data.LocationAfterState;
        var deletedAt = locationAfterState.DeletedAt?.ToDateTimeOffset();
        var eventRaisedAt = src.Metadata.Time?.ToDateTimeOffset() ?? DateTimeOffset.MinValue;

        var location = new Location
        {
            Id = locationAfterState.Id,
            DeletedAt = deletedAt,
            EventRaisedAt = eventRaisedAt,
            Name = locationAfterState.Name,
            Organization =
                string.IsNullOrWhiteSpace(locationAfterState.OrganizationId)
                    ? null
                    : new Organization { Id = locationAfterState.OrganizationId }
        };

        location.LocationMembers = locationAfterState.Members.Select(item => new Shared.Models.LocationMember
        {
            Id = item.Id,
            DeletedAt = deletedAt,
            EventRaisedAt = eventRaisedAt,
            MembershipType = item.MembershipType switch
            {
                Api.Shared.Clients.Events.UnityHub.Location.V1.Value.MembershipType.Owner => LocationMembershipType
                    .Owner,
                Api.Shared.Clients.Events.UnityHub.Location.V1.Value.MembershipType.Administrator =>
                    LocationMembershipType.Administrator,
                Api.Shared.Clients.Events.UnityHub.Location.V1.Value.MembershipType.Member =>
                    LocationMembershipType.Member,
                _ => throw new ArgumentOutOfRangeException()
            },
            Customer = new Shared.Models.Customer { Id = item.CustomerId },
            Location = location
        }).ToList();

        location.Tags = locationAfterState.Tags.Select(item => new Shared.Models.LocationTag
        {
            Id = item.Id,
            DeletedAt = deletedAt,
            EventRaisedAt = eventRaisedAt,
            Name = item.Name,
            Type = item.TagType,
            Location = location
        }).ToList();

        location.Desks = locationAfterState.Desks.Select(item => new Shared.Models.Desk
        {
            Id = item.Id,
            DeletedAt = deletedAt,
            EventRaisedAt = eventRaisedAt,
            Name = item.Name,
            Deactivated = item.Deactivated,
            RequireBookingApproval = item.RequireBookingApproval,
            Tags = location.Tags.Where(tag => item.LocationTagIds.Contains(tag.Id)).ToList(),
            Location = location
        }).ToList();

        return location;
    }

    public Team MapTo(Api.Shared.Clients.Events.UnityHub.Team.V1.Value.Event src)
    {
        var teamAfterState = src.Data.TeamAfterState;
        var deletedAt = teamAfterState.DeletedAt?.ToDateTimeOffset();
        var eventRaisedAt = src.Metadata.Time?.ToDateTimeOffset() ?? DateTimeOffset.MinValue;

        var team = new Team
        {
            Id = teamAfterState.Id,
            DeletedAt = deletedAt,
            EventRaisedAt = eventRaisedAt,
            Name = teamAfterState.Name,
            Organization =
                string.IsNullOrWhiteSpace(teamAfterState.OrganizationId)
                    ? null
                    : new Organization { Id = teamAfterState.OrganizationId }
        };

        team.TeamMembers = teamAfterState.Members.Select(item => new Shared.Models.TeamMember
        {
            Id = item.Id,
            DeletedAt = deletedAt,
            EventRaisedAt = eventRaisedAt,
            MembershipType = item.MembershipType switch
            {
                Api.Shared.Clients.Events.UnityHub.Team.V1.Value.MembershipType.Owner => TeamMembershipType
                    .Owner,
                Api.Shared.Clients.Events.UnityHub.Team.V1.Value.MembershipType.Administrator =>
                    TeamMembershipType.Administrator,
                Api.Shared.Clients.Events.UnityHub.Team.V1.Value.MembershipType.Member =>
                    TeamMembershipType.Member,
                _ => throw new ArgumentOutOfRangeException()
            },
            Customer = new Shared.Models.Customer { Id = item.CustomerId },
            Team = team
        }).ToList();

        return team;
    }

    public Shared.Models.Booking MapTo(Api.Shared.Clients.Events.UnityHub.Booking.V1.Value.Event src)
    {
        var booking = src.Data.AfterState;
        var deletedAt = booking.DeletedAt?.ToDateTimeOffset();

        return new Shared.Models.Booking
        {
            Id = booking.Id,
            DeletedAt = deletedAt,
            From = booking.From.ToDateTimeOffset(),
            To = booking.To.ToDateTimeOffset(),
            Notes = booking.Notes,
            Customer = new Shared.Models.Customer { Id = booking.CustomerId },
            Organization =
                string.IsNullOrWhiteSpace(booking.OrganizationId)
                    ? null
                    : new Organization { Id = booking.OrganizationId },
            Location =
                string.IsNullOrWhiteSpace(booking.LocationId) ? null : new Location { Id = booking.LocationId },
            Desks = booking.DeskIds.Select(item => new Shared.Models.Desk { Id = item }).ToList(),
            Team = string.IsNullOrWhiteSpace(booking.TeamId) ? null : new Team { Id = booking.TeamId }
        };
    }

    public Shared.Database.Entities.Organization MapToEntity(Organization src) =>
        MergeToEntity(src, new Shared.Database.Entities.Organization());

    public Shared.Database.Entities.Organization MergeToEntity(
        Organization src,
        Shared.Database.Entities.Organization dest)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Name = src.Name;
        dest.LogoUrl = src.LogoUrl;
        dest.Offering = src.Offering;
        return dest;
    }

    public Shared.Database.Entities.Location MapToEntity(
        Location src,
        Shared.Database.Entities.Organization? organization) =>
        MergeToEntity(src, new Shared.Database.Entities.Location(), organization);

    public Shared.Database.Entities.Location MergeToEntity(
        Location src,
        Shared.Database.Entities.Location dest,
        Shared.Database.Entities.Organization? organization)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Name = src.Name;
        dest.Organization = organization;
        return dest;
    }

    public Shared.Database.Entities.Team MapToEntity(Team src, Shared.Database.Entities.Organization? organization) =>
        MergeToEntity(src, new Shared.Database.Entities.Team(), organization);

    public Shared.Database.Entities.Team MergeToEntity(
        Team src,
        Shared.Database.Entities.Team dest,
        Shared.Database.Entities.Organization? organization)
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
        Customer customer) =>
        MergeToEntity(src, new OrganizationMember(), organization, customer);

    public OrganizationMember MergeToEntity(
        Shared.Models.OrganizationMember src,
        OrganizationMember dest,
        Shared.Database.Entities.Organization organization,
        Customer customer)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.MembershipType = src.MembershipType;
        dest.Organization = organization;
        dest.Customer = customer;
        return dest;
    }

    public LocationMember MapToEntity(
        Shared.Models.LocationMember src,
        Shared.Database.Entities.Location location,
        Customer customer) =>
        MergeToEntity(src, new LocationMember(), location, customer);

    public LocationMember MergeToEntity(
        Shared.Models.LocationMember src,
        LocationMember dest,
        Shared.Database.Entities.Location location,
        Customer customer)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.MembershipType = src.MembershipType;
        dest.Location = location;
        dest.Customer = customer;
        return dest;
    }

    public TeamMember MapToEntity(
        Shared.Models.TeamMember src,
        Shared.Database.Entities.Team team,
        Customer customer) =>
        MergeToEntity(src, new TeamMember(), team, customer);

    public TeamMember MergeToEntity(
        Shared.Models.TeamMember src,
        TeamMember dest,
        Shared.Database.Entities.Team team,
        Customer customer)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.MembershipType = src.MembershipType;
        dest.Team = team;
        dest.Customer = customer;
        return dest;
    }

    public LocationTag MapToEntity(Shared.Models.LocationTag src, Shared.Database.Entities.Location location) =>
        MergeToEntity(src, new LocationTag(), location);

    public LocationTag MergeToEntity(
        Shared.Models.LocationTag src,
        LocationTag dest,
        Shared.Database.Entities.Location location)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Name = src.Name;
        dest.Type = src.Type;
        dest.Location = location;
        return dest;
    }

    public Desk MapToEntity(
        Shared.Models.Desk src,
        Shared.Database.Entities.Location location,
        ICollection<LocationTag> locationTags) =>
        MergeToEntity(src, new Desk(), location, locationTags);

    public Desk MergeToEntity(
        Shared.Models.Desk src,
        Desk dest, Shared.Database.Entities.Location location,
        ICollection<LocationTag> locationTags)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Name = src.Name;
        dest.Deactivated = src.Deactivated;
        dest.RequireBookingApproval = src.RequireBookingApproval;
        dest.Location = location;
        dest.Tags = locationTags;
        return dest;
    }

    public IEnumerable<Identity>
        MapToEntity(IEnumerable<Shared.Models.Identity> src, Customer? customer) =>
        src.Select(identity => MapToEntity(identity, customer));

    public Customer MapToEntity(
        Shared.Models.Customer src,
        ICollection<Identity> identities,
        Shared.Database.Entities.Organization? defaultOrganization,
        ICollection<Shared.Database.Entities.Location> defaultLocations,
        ICollection<Shared.Database.Entities.Team> defaultTeams,
        ICollection<LocationTag> preferredLocationTags,
        ICollection<Desk> preferredDesks) =>
        MergeToEntity(src,
            new Customer(),
            identities,
            defaultOrganization,
            defaultLocations,
            defaultTeams,
            preferredLocationTags,
            preferredDesks);

    public Customer MergeToEntity(
        Shared.Models.Customer src,
        Customer dest,
        ICollection<Identity> identities,
        Shared.Database.Entities.Organization? defaultOrganization,
        ICollection<Shared.Database.Entities.Location> defaultLocations,
        ICollection<Shared.Database.Entities.Team> defaultTeams,
        ICollection<LocationTag> preferredLocationTags,
        ICollection<Desk> preferredDesks)
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
        dest.DefaultLocations = defaultLocations;
        dest.PreferredLocationTags = preferredLocationTags;
        dest.PreferredDesks = preferredDesks;
        dest.DefaultTeams = defaultTeams;
        return dest;
    }

    public Identity MapToEntity(Shared.Models.Identity src, Customer? customer) =>
        MergeToEntity(src, new Identity(), customer);

    public Identity MergeToEntity(
        Shared.Models.Identity src,
        Identity dest,
        Customer? customer)
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

    public Shared.Database.Entities.Booking MapToEntity(
        Shared.Models.Booking src,
        Customer customer,
        Shared.Database.Entities.Organization? organization,
        Shared.Database.Entities.Location? location,
        ICollection<Desk> desks,
        Shared.Database.Entities.Team? team) =>
        MergeToEntity(src, new Shared.Database.Entities.Booking(), customer, organization, location, desks, team);

    public Shared.Database.Entities.Booking MergeToEntity(
        Shared.Models.Booking src,
        Shared.Database.Entities.Booking dest,
        Customer customer,
        Shared.Database.Entities.Organization? organization,
        Shared.Database.Entities.Location? location,
        ICollection<Desk> desks,
        Shared.Database.Entities.Team? team)
    {
        dest.Id = src.Id;
        dest.From = src.From;
        dest.To = src.To;
        dest.Notes = src.Notes;
        dest.Customer = customer;
        dest.Organization = organization;
        dest.Location = location;
        dest.Desks = desks;
        dest.Team = team;
        return dest;
    }

    public OrganizationTag MergeToEntity(
        Shared.Models.OrganizationTag src,
        OrganizationTag dest,
        Shared.Database.Entities.Organization organization)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Name = src.Name;
        dest.Type = src.Type;
        dest.Organization = organization;
        return dest;
    }

    public OrganizationTag MapToEntity(
        Shared.Models.OrganizationTag src,
        Shared.Database.Entities.Organization organization) =>
        MergeToEntity(src, new OrganizationTag(), organization);
}
