using Api.Shared.Clients.Events.Skedular.Organization.V1.Value;
using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Booking.Shared.Database.Entities;
using Enterprise.Shared;
using Customer = Booking.Shared.Database.Entities.Customer;
using Desk = Booking.Shared.Database.Entities.Desk;
using Room = Booking.Shared.Database.Entities.Room;
using Event = Api.Shared.Clients.Events.Skedular.Customer.V1.Value.Event;
using Location = Booking.Shared.Models.Location;
using LocationMember = Booking.Shared.Database.Entities.LocationMember;
using Offering = Api.Shared.Services.Models.Offering;
using Organization = Booking.Shared.Models.Organization;
using OrganizationMember = Booking.Shared.Database.Entities.OrganizationMember;
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
    Shared.Database.Entities.Organization MapToEntity(Organization src);
    Shared.Database.Entities.Organization MergeToEntity(Organization src, Shared.Database.Entities.Organization dest);
    Shared.Database.Entities.Location MapToEntity(Location src, Shared.Database.Entities.Organization? organization);

    Shared.Database.Entities.Location MergeToEntity(
        Location src,
        Shared.Database.Entities.Location dest,
        Shared.Database.Entities.Organization? organization);

    Shared.Database.Entities.Team MapToEntity(Team src, Shared.Database.Entities.Organization? organization);
    Shared.Database.Entities.Team MergeToEntity(Team src, Shared.Database.Entities.Team dest, Shared.Database.Entities.Organization? organization);
    OrganizationMember MapToEntity(Shared.Models.OrganizationMember src, Shared.Database.Entities.Organization organization, Customer customer);

    OrganizationMember MergeToEntity(
        Shared.Models.OrganizationMember src,
        OrganizationMember dest,
        Shared.Database.Entities.Organization organization,
        Customer customer);

    LocationMember MapToEntity(Shared.Models.LocationMember src, Shared.Database.Entities.Location organization, Customer customer);

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

    Resource MapToEntity(
        Shared.Models.Resource src,
        Shared.Database.Entities.Location location,
        ICollection<OrganizationTag> organizationTags);

    Resource MergeToEntity(
        Shared.Models.Resource src,
        Resource dest,
        Shared.Database.Entities.Location? location,
        ICollection<OrganizationTag> organizationTags);

    Desk MapToEntity(Shared.Models.Desk src, Shared.Database.Entities.Location location, ICollection<OrganizationTag> organizationTags);
    Desk MergeToEntity(Shared.Models.Desk src, Desk dest, Shared.Database.Entities.Location location, ICollection<OrganizationTag> organizationTags);
    Room MapToEntity(Shared.Models.Room src, Shared.Database.Entities.Location location, ICollection<OrganizationTag> organizationTags);
    Room MergeToEntity(Shared.Models.Room src, Room dest, Shared.Database.Entities.Location location, ICollection<OrganizationTag> organizationTags);
    IEnumerable<Identity> MapToEntity(IEnumerable<Shared.Models.Identity> src, Customer? customer);

    Customer MapToEntity(
        Shared.Models.Customer src,
        ICollection<Identity> identities,
        Shared.Database.Entities.Organization? defaultOrganization,
        ICollection<Shared.Database.Entities.Location> preferredLocations,
        ICollection<Shared.Database.Entities.Team> preferredTeams,
        ICollection<Desk> preferredDesks,
        ICollection<Room> preferredRooms,
        ICollection<OrganizationTag> preferredOrganizationTags);

    Customer MergeToEntity(
        Shared.Models.Customer src,
        Customer dest,
        ICollection<Identity> identities,
        Shared.Database.Entities.Organization? defaultOrganization,
        ICollection<Shared.Database.Entities.Location> preferredLocations,
        ICollection<Shared.Database.Entities.Team> preferredTeams,
        ICollection<Desk> preferredDesks,
        ICollection<Room> preferredRooms,
        ICollection<OrganizationTag> preferredOrganizationTags);

    Identity MapToEntity(Shared.Models.Identity src, Customer? customer);
    Identity MergeToEntity(Shared.Models.Identity src, Identity dest, Customer? customer);
    OrganizationTag MapToEntity(Shared.Models.OrganizationTag src, Shared.Database.Entities.Organization organization);
    OrganizationTag MergeToEntity(Shared.Models.OrganizationTag src, OrganizationTag dest, Shared.Database.Entities.Organization organization);
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
            PreferredLocations = customer.PreferredLocations.Select(item => new Location
            {
                Id = item.Id, Organization = string.IsNullOrWhiteSpace(item.OrganizationId) ? null : new Organization { Id = item.OrganizationId }
            }).ToList(),
            PreferredDesks = customer.PreferredDesks.Select(item =>
                new Shared.Models.Desk { Id = item.Id, Location = new Location { Id = item.LocationId } }).ToList(),
            PreferredRooms = customer.PreferredRooms.Select(item =>
                new Shared.Models.Room { Id = item.Id, Location = new Location { Id = item.LocationId } }).ToList(),
            PreferredTeams = customer.PreferredTeams.Select(item => new Team
            {
                Id = item.Id, Organization = string.IsNullOrWhiteSpace(item.OrganizationId) ? null : new Organization { Id = item.OrganizationId }
            }).ToList(),
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
            }
        };

        organization.OrganizationMembers = organizationAfterState.Members.Select(item =>
        {
            return new Shared.Models.OrganizationMember
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
            };
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
            Organization =
                string.IsNullOrWhiteSpace(locationAfterState.OrganizationId) ? null : new Organization { Id = locationAfterState.OrganizationId }
        };

        location.LocationMembers = locationAfterState.Members.Select(item => new Shared.Models.LocationMember
        {
            Id = item.Id,
            DeletedAt = deletedAt,
            EventRaisedAt = eventRaisedAt,
            Role = item.Role switch
            {
                Api.Shared.Clients.Events.Skedular.Location.V1.Value.Role.Owner => LocationMemberRole.Owner,
                Api.Shared.Clients.Events.Skedular.Location.V1.Value.Role.Administrator => LocationMemberRole.Administrator,
                Api.Shared.Clients.Events.Skedular.Location.V1.Value.Role.Member => LocationMemberRole.Member,
                _ => throw new ArgumentOutOfRangeException()
            },
            Customer = new Shared.Models.Customer { Id = item.CustomerId },
            Location = location
        }).ToList();

        var organizationTags = location.Organization is null
            ? []
            : locationAfterState.Desks
                .SelectMany(item => item.TagIds)
                .Concat(locationAfterState.Rooms.SelectMany(item => item.TagIds))
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
            OrganizationTags = organizationTags.Where(tag => item.TagIds.Contains(tag.Id)).ToList(),
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
            Color = item.Color,
            OrganizationTags = organizationTags.Where(tag => item.TagIds.Contains(tag.Id)).ToList(),
            Location = location
        }).ToList();

        location.Rooms = locationAfterState.Rooms.Select(item => new Shared.Models.Room
        {
            Id = item.Id,
            DeletedAt = deletedAt,
            EventRaisedAt = eventRaisedAt,
            Name = item.Name,
            Deactivated = item.Deactivated,
            RequireBookingApproval = item.RequireBookingApproval,
            Color = item.Color,
            OrganizationTags = organizationTags.Where(tag => item.TagIds.Contains(tag.Id)).ToList(),
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
            Organization = string.IsNullOrWhiteSpace(teamAfterState.OrganizationId) ? null : new Organization { Id = teamAfterState.OrganizationId }
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

    public Shared.Database.Entities.Organization MapToEntity(Organization src) => MergeToEntity(src, new Shared.Database.Entities.Organization());

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

    public Shared.Database.Entities.Location MapToEntity(Location src, Shared.Database.Entities.Organization? organization) =>
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
        dest.Role = src.Role.ToNullableOrganizationMemberRole();
        dest.Status = src.Status.ToOrganizationMemberStatus();
        dest.Organization = organization;
        dest.Customer = customer;
        return dest;
    }

    public LocationMember MapToEntity(Shared.Models.LocationMember src, Shared.Database.Entities.Location location, Customer customer) =>
        MergeToEntity(src, new LocationMember(), location, customer);

    public LocationMember MergeToEntity(
        Shared.Models.LocationMember src,
        LocationMember dest,
        Shared.Database.Entities.Location location,
        Customer customer)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Role = src.Role.ToNullableLocationMemberRole();
        dest.Location = location;
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
        dest.Location = location;
        dest.OrganizationTags = organizationTags;
        return dest;
    }

    public Desk MapToEntity(
        Shared.Models.Desk src,
        Shared.Database.Entities.Location location,
        ICollection<OrganizationTag> organizationTags) =>
        MergeToEntity(src, new Desk(), location, organizationTags);

    public Desk MergeToEntity(
        Shared.Models.Desk src,
        Desk dest, Shared.Database.Entities.Location location,
        ICollection<OrganizationTag> organizationTags)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Name = src.Name;
        dest.Deactivated = src.Deactivated;
        dest.RequireBookingApproval = src.RequireBookingApproval;
        dest.Color = src.Color;
        dest.Location = location;
        dest.OrganizationTags = organizationTags;
        return dest;
    }

    public Room MapToEntity(
        Shared.Models.Room src,
        Shared.Database.Entities.Location location,
        ICollection<OrganizationTag> organizationTags) =>
        MergeToEntity(src, new Room(), location, organizationTags);

    public Room MergeToEntity(
        Shared.Models.Room src,
        Room dest, Shared.Database.Entities.Location location,
        ICollection<OrganizationTag> organizationTags)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Name = src.Name;
        dest.Deactivated = src.Deactivated;
        dest.RequireBookingApproval = src.RequireBookingApproval;
        dest.Color = src.Color;
        dest.Location = location;
        dest.OrganizationTags = organizationTags;
        return dest;
    }

    public IEnumerable<Identity> MapToEntity(IEnumerable<Shared.Models.Identity> src, Customer? customer) =>
        src.Select(identity => MapToEntity(identity, customer));

    public Customer MapToEntity(
        Shared.Models.Customer src,
        ICollection<Identity> identities,
        Shared.Database.Entities.Organization? defaultOrganization,
        ICollection<Shared.Database.Entities.Location> preferredLocations,
        ICollection<Shared.Database.Entities.Team> preferredTeams,
        ICollection<Desk> preferredDesks,
        ICollection<Room> preferredRooms,
        ICollection<OrganizationTag> preferredOrganizationTags) =>
        MergeToEntity(src,
            new Customer(),
            identities,
            defaultOrganization,
            preferredLocations,
            preferredTeams,
            preferredDesks,
            preferredRooms,
            preferredOrganizationTags);

    public Customer MergeToEntity(
        Shared.Models.Customer src,
        Customer dest,
        ICollection<Identity> identities,
        Shared.Database.Entities.Organization? defaultOrganization,
        ICollection<Shared.Database.Entities.Location> preferredLocations,
        ICollection<Shared.Database.Entities.Team> preferredTeams,
        ICollection<Desk> preferredDesks,
        ICollection<Room> preferredRooms,
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
        dest.PreferredDesks = preferredDesks;
        dest.PreferredRooms = preferredRooms;
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

    public OrganizationTag MapToEntity(Shared.Models.OrganizationTag src, Shared.Database.Entities.Organization organization) =>
        MergeToEntity(src, new OrganizationTag(), organization);
}
