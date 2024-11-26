using Api.Shared.Clients.Events.UnityHub.Organization.V1.Value;
using Api.Shared.Models;
using Customer.Shared.Database.Entities;
using Enterprise.Shared;
using Desk = Customer.Shared.Models.Desk;
using Event = Api.Shared.Clients.Events.UnityHub.Customer.V1.Value.Event;
using Identity = Customer.Shared.Models.Identity;
using Location = Customer.Shared.Models.Location;
using LocationTag = Customer.Shared.Models.LocationTag;
using Organization = Customer.Shared.Models.Organization;
using OrganizationTag = Customer.Shared.Models.OrganizationTag;
using Team = Customer.Shared.Models.Team;

namespace Customer.Processors.Mappers;

public interface IMapper
{
    Shared.Models.Customer MapTo(Event src);
    Organization MapTo(Api.Shared.Clients.Events.UnityHub.Organization.V1.Value.Event src);
    Location MapTo(Api.Shared.Clients.Events.UnityHub.Location.V1.Value.Event src);
    Team MapTo(Api.Shared.Clients.Events.UnityHub.Team.V1.Value.Event src);
    Shared.Models.Customer? MapTo(Shared.Database.Entities.Customer? src);

    Shared.Database.Entities.Customer MapToEntity(
        Shared.Models.Customer src,
        ICollection<Shared.Database.Entities.Identity> identities,
        Shared.Database.Entities.Organization? defaultOrganization,
        ICollection<Shared.Database.Entities.Location> defaultLocations,
        ICollection<Shared.Database.Entities.Team> defaultTeams,
        ICollection<Shared.Database.Entities.LocationTag> preferredLocationTags,
        ICollection<Shared.Database.Entities.Desk> preferredDesks,
        ICollection<Shared.Database.Entities.OrganizationTag> preferredOrganizationTags);

    Shared.Database.Entities.Customer MergeToEntity(
        Shared.Models.Customer src,
        Shared.Database.Entities.Customer dest,
        ICollection<Shared.Database.Entities.Identity> identities,
        Shared.Database.Entities.Organization? defaultOrganization,
        ICollection<Shared.Database.Entities.Location> defaultLocations,
        ICollection<Shared.Database.Entities.Team> defaultTeams,
        ICollection<Shared.Database.Entities.LocationTag> preferredLocationTags,
        ICollection<Shared.Database.Entities.Desk> preferredDesks,
        ICollection<Shared.Database.Entities.OrganizationTag> preferredOrganizationTags);

    Shared.Database.Entities.Identity MapToEntity(Identity src, Shared.Database.Entities.Customer? customer);

    Shared.Database.Entities.Identity MergeToEntity(
        Identity src,
        Shared.Database.Entities.Identity dest,
        Shared.Database.Entities.Customer? customer);

    IEnumerable<Shared.Database.Entities.Identity> MapToEntity(
        IEnumerable<Identity> src,
        Shared.Database.Entities.Customer? customer);

    Shared.Database.Entities.Organization MapToEntity(Organization src);
    Shared.Database.Entities.Organization MergeToEntity(Organization src, Shared.Database.Entities.Organization dest);

    Shared.Database.Entities.Location MapToEntity(Location src, Shared.Database.Entities.Organization? organization);

    Shared.Database.Entities.Location MergeToEntity(
        Location src,
        Shared.Database.Entities.Location dest,
        Shared.Database.Entities.Organization? organization);

    Shared.Database.Entities.LocationTag MapToEntity(LocationTag src, Shared.Database.Entities.Location location);

    Shared.Database.Entities.LocationTag MergeToEntity(
        LocationTag src,
        Shared.Database.Entities.LocationTag dest,
        Shared.Database.Entities.Location location);

    Shared.Database.Entities.Desk MapToEntity(
        Desk src,
        Shared.Database.Entities.Location location,
        ICollection<Shared.Database.Entities.LocationTag> locationTags,
        ICollection<Shared.Database.Entities.OrganizationTag> organizationTags);

    Shared.Database.Entities.Desk MergeToEntity(
        Desk src,
        Shared.Database.Entities.Desk dest,
        Shared.Database.Entities.Location location,
        ICollection<Shared.Database.Entities.LocationTag> locationTags,
        ICollection<Shared.Database.Entities.OrganizationTag> organizationTags);

    Shared.Database.Entities.Team MapToEntity(Team src, Shared.Database.Entities.Organization? organization);

    Shared.Database.Entities.Team MergeToEntity(
        Team src,
        Shared.Database.Entities.Team dest,
        Shared.Database.Entities.Organization? organization);

    OrganizationMember MapToEntity(
        Shared.Models.OrganizationMember src,
        Shared.Database.Entities.Organization organization,
        Shared.Database.Entities.Customer customer);

    OrganizationMember MergeToEntity(
        Shared.Models.OrganizationMember src,
        OrganizationMember dest,
        Shared.Database.Entities.Organization organization,
        Shared.Database.Entities.Customer customer);

    LocationMember MapToEntity(
        Shared.Models.LocationMember src,
        Shared.Database.Entities.Location location,
        Shared.Database.Entities.Customer customer);

    LocationMember MergeToEntity(
        Shared.Models.LocationMember src,
        LocationMember dest,
        Shared.Database.Entities.Location location,
        Shared.Database.Entities.Customer customer);

    TeamMember MapToEntity(
        Shared.Models.TeamMember src,
        Shared.Database.Entities.Team team,
        Shared.Database.Entities.Customer customer,
        OrganizationMember? organizationMember);

    TeamMember MergeToEntity(
        Shared.Models.TeamMember src,
        TeamMember dest,
        Shared.Database.Entities.Team team,
        Shared.Database.Entities.Customer customer,
        OrganizationMember? organizationMember);

    Shared.Database.Entities.OrganizationTag MergeToEntity(
        OrganizationTag src,
        Shared.Database.Entities.OrganizationTag dest,
        Shared.Database.Entities.Organization organization);

    Shared.Database.Entities.OrganizationTag MapToEntity(
        OrganizationTag src,
        Shared.Database.Entities.Organization organization);
}

public class Mapper : IMapper
{
    public Shared.Models.Customer MapTo(Event src)
    {
        var customer = src.Data.AfterState;
        var deletedAt = customer.DeletedAt?.ToDateTimeOffset();

        return new Shared.Models.Customer
        {
            Id = customer.Id,
            DeletedAt = deletedAt,
            Designation = customer.Designation,
            Title = customer.Title,
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
            Timezone = customer.Timezone,
            Locale = customer.Locale,
            IsOrganizationOnboardingDone = customer.Settings.IsOrganizationOnboardingDone,
            IsLocationOnboardingDone = customer.Settings.IsLocationOnboardingDone,
            IsTeamOnboardingDone = customer.Settings.IsTeamOnboardingDone,
            IsDefaultOrganizationOnboardingDone = customer.Settings.IsDefaultOrganizationOnboardingDone,
            IsDefaultLocationOnboardingDone = customer.Settings.IsDefaultLocationOnboardingDone,
            IsPreferredZoneOnboardingDone = customer.Settings.IsPreferredZoneOnboardingDone,
            IsPreferredDeskOnboardingDone = customer.Settings.IsPreferredDeskOnboardingDone,
            Identities = customer.Identities.Select(item =>
                    new Identity
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
            PreferredLocationTags = customer.DefaultLocationTags
                .Select(item => new LocationTag { Id = item.Id, Location = new Location { Id = item.LocationId } })
                .ToList(),
            PreferredDesks = customer.DefaultDesks.Select(item =>
                new Desk { Id = item.Id, Location = new Location { Id = item.LocationId } }).ToList(),
            DefaultTeams = customer.DefaultTeams.Select(item => new Team
            {
                Id = item.Id,
                Organization =
                    string.IsNullOrWhiteSpace(item.OrganizationId)
                        ? null
                        : new Organization { Id = item.OrganizationId }
            }).ToList(),
            PreferredOrganizationTags = customer.DefaultOrganizationTags
                .Select(item =>
                    new OrganizationTag { Id = item.Id, Organization = new Organization { Id = item.OrganizationId } })
                .ToList()
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
            LogoUrl = organizationAfterState.LogoUrl
        };

        organization.OrganizationMembers = organizationAfterState.Members.Select(item =>
        {
            return new Shared.Models.OrganizationMember
            {
                Id = item.Id,
                EventRaisedAt = eventRaisedAt,
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

        organization.Tags = organizationAfterState.Tags.Select(item => new OrganizationTag
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

        location.Tags = locationAfterState.Tags.Select(item => new LocationTag
        {
            Id = item.Id,
            DeletedAt = deletedAt,
            EventRaisedAt = eventRaisedAt,
            Name = item.Name,
            Type = item.TagType,
            Location = location
        }).ToList();

        var organizationTags = location.Organization is null
            ? []
            : locationAfterState.Desks.SelectMany(item => item.OrganizationTagIds).Select(item =>
                new Shared.Models.OrganizationTag { Id = item, Organization = location.Organization });

        location.Desks = locationAfterState.Desks.Select(item => new Desk
        {
            Id = item.Id,
            DeletedAt = deletedAt,
            EventRaisedAt = eventRaisedAt,
            Name = item.Name,
            Tags = location.Tags.Where(tag => item.LocationTagIds.Contains(tag.Id)).ToList(),
            OrganizationTags = organizationTags.Where(tag => item.OrganizationTagIds.Contains(tag.Id)).ToList(),
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
            OrganizationMember =
                string.IsNullOrWhiteSpace(item.OrganizationMember?.OrganizationId) ||
                string.IsNullOrWhiteSpace(item.OrganizationMember?.OrganizationMemberId)
                    ? null
                    : new Shared.Models.OrganizationMember
                    {
                        Id = item.OrganizationMember.OrganizationMemberId,
                        Organization = new Organization { Id = item.OrganizationMember.OrganizationId },
                        Customer = new Shared.Models.Customer { Id = item.OrganizationMember.CustomerId }
                    },
            Team = team
        }).ToList();

        return team;
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
                IsOrganizationOnboardingDone = src.IsOrganizationOnboardingDone,
                IsLocationOnboardingDone = src.IsLocationOnboardingDone,
                IsTeamOnboardingDone = src.IsTeamOnboardingDone,
                IsDefaultOrganizationOnboardingDone = src.IsDefaultOrganizationOnboardingDone,
                IsDefaultLocationOnboardingDone = src.IsDefaultLocationOnboardingDone,
                IsPreferredZoneOnboardingDone = src.IsPreferredZoneOnboardingDone,
                IsPreferredDeskOnboardingDone = src.IsPreferredDeskOnboardingDone,
                Identities = MapTo(src.Identities).ToList(),
                DefaultOrganization = MapTo(src.DefaultOrganization),
                DefaultLocations = MapTo(src.DefaultLocations).ToList(),
                PreferredLocationTags = MapTo(src.PreferredLocationTags, false).ToList(),
                PreferredDesks = MapTo(src.PreferredDesks).ToList(),
                DefaultTeams = MapTo(src.DefaultTeams).ToList(),
                PreferredOrganizationTags = MapTo(src.PreferredOrganizationTags).ToList(),
            };

    public Shared.Database.Entities.Customer MapToEntity(
        Shared.Models.Customer src,
        ICollection<Shared.Database.Entities.Identity> identities,
        Shared.Database.Entities.Organization? defaultOrganization,
        ICollection<Shared.Database.Entities.Location> defaultLocations,
        ICollection<Shared.Database.Entities.Team> defaultTeams,
        ICollection<Shared.Database.Entities.LocationTag> preferredLocationTags,
        ICollection<Shared.Database.Entities.Desk> preferredDesks,
        ICollection<Shared.Database.Entities.OrganizationTag> preferredOrganizationTags) =>
        MergeToEntity(
            src,
            new Shared.Database.Entities.Customer(),
            identities,
            defaultOrganization,
            defaultLocations,
            defaultTeams,
            preferredLocationTags,
            preferredDesks,
            preferredOrganizationTags);

    public Shared.Database.Entities.Customer MergeToEntity(
        Shared.Models.Customer src,
        Shared.Database.Entities.Customer dest,
        ICollection<Shared.Database.Entities.Identity> identities,
        Shared.Database.Entities.Organization? defaultOrganization,
        ICollection<Shared.Database.Entities.Location> defaultLocations,
        ICollection<Shared.Database.Entities.Team> defaultTeams,
        ICollection<Shared.Database.Entities.LocationTag> preferredLocationTags,
        ICollection<Shared.Database.Entities.Desk> preferredDesks,
        ICollection<Shared.Database.Entities.OrganizationTag> preferredOrganizationTags)
    {
        dest.Id = src.Id;
        dest.Designation = src.Designation;
        dest.Title = src.Title;
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
        dest.Timezone = src.Timezone;
        dest.Locale = src.Locale;
        dest.IsOrganizationOnboardingDone = src.IsOrganizationOnboardingDone;
        dest.IsLocationOnboardingDone = src.IsLocationOnboardingDone;
        dest.IsTeamOnboardingDone = src.IsTeamOnboardingDone;
        dest.IsDefaultOrganizationOnboardingDone =
            src.IsDefaultOrganizationOnboardingDone;
        dest.IsDefaultLocationOnboardingDone = src.IsDefaultLocationOnboardingDone;
        dest.IsPreferredZoneOnboardingDone = src.IsPreferredZoneOnboardingDone;
        dest.IsPreferredDeskOnboardingDone = src.IsPreferredDeskOnboardingDone;
        dest.Identities = identities;
        dest.DefaultOrganization = defaultOrganization;
        dest.DefaultLocations = defaultLocations;
        dest.PreferredLocationTags = preferredLocationTags;
        dest.PreferredDesks = preferredDesks;
        dest.DefaultTeams = defaultTeams;
        dest.PreferredOrganizationTags = preferredOrganizationTags;
        return dest;
    }

    public Shared.Database.Entities.Identity MapToEntity(
        Identity src,
        Shared.Database.Entities.Customer? customer) =>
        MergeToEntity(src, new Shared.Database.Entities.Identity(), customer);

    public Shared.Database.Entities.Identity MergeToEntity(
        Identity src,
        Shared.Database.Entities.Identity dest,
        Shared.Database.Entities.Customer? customer)
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

    public Shared.Database.Entities.Organization MapToEntity(Organization src) =>
        MergeToEntity(src, new Shared.Database.Entities.Organization());

    public Shared.Database.Entities.Organization MergeToEntity(Organization src,
        Shared.Database.Entities.Organization dest)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Name = src.Name;
        dest.LogoUrl = src.LogoUrl;
        return dest;
    }

    public Shared.Database.Entities.Location MapToEntity(Location src,
        Shared.Database.Entities.Organization? organization) =>
        MergeToEntity(src, new Shared.Database.Entities.Location(), organization);

    public Shared.Database.Entities.Location MergeToEntity(Location src, Shared.Database.Entities.Location dest,
        Shared.Database.Entities.Organization? organization)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Name = src.Name;
        dest.Organization = organization;
        return dest;
    }

    public Shared.Database.Entities.LocationTag
        MapToEntity(LocationTag src, Shared.Database.Entities.Location location) =>
        MergeToEntity(src, new Shared.Database.Entities.LocationTag(), location);

    public Shared.Database.Entities.LocationTag MergeToEntity(
        LocationTag src,
        Shared.Database.Entities.LocationTag dest,
        Shared.Database.Entities.Location location)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Name = src.Name;
        dest.Type = src.Type;
        dest.Location = location;
        return dest;
    }

    public Shared.Database.Entities.Desk MapToEntity(
        Desk src,
        Shared.Database.Entities.Location location,
        ICollection<Shared.Database.Entities.LocationTag> locationTags,
        ICollection<Shared.Database.Entities.OrganizationTag> organizationTags) =>
        MergeToEntity(src, new Shared.Database.Entities.Desk(), location, locationTags, organizationTags);

    public Shared.Database.Entities.Desk MergeToEntity(
        Desk src,
        Shared.Database.Entities.Desk dest,
        Shared.Database.Entities.Location location,
        ICollection<Shared.Database.Entities.LocationTag> locationTags,
        ICollection<Shared.Database.Entities.OrganizationTag> organizationTags)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Name = src.Name;
        dest.Location = location;
        dest.Tags = locationTags;
        dest.OrganizationTags = organizationTags;
        return dest;
    }

    public Shared.Database.Entities.Team MapToEntity(Team src, Shared.Database.Entities.Organization? organization) =>
        MergeToEntity(src, new Shared.Database.Entities.Team(), organization);

    public Shared.Database.Entities.Team MergeToEntity(Team src, Shared.Database.Entities.Team dest,
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
        dest.MembershipType = src.MembershipType;
        dest.Organization = organization;
        dest.Customer = customer;
        return dest;
    }

    public LocationMember MapToEntity(
        Shared.Models.LocationMember src,
        Shared.Database.Entities.Location location,
        Shared.Database.Entities.Customer customer) =>
        MergeToEntity(src, new LocationMember(), location, customer);

    public LocationMember MergeToEntity(
        Shared.Models.LocationMember src,
        LocationMember dest,
        Shared.Database.Entities.Location location,
        Shared.Database.Entities.Customer customer)
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
        Shared.Database.Entities.Customer customer,
        OrganizationMember? organizationMember) =>
        MergeToEntity(src, new TeamMember(), team, customer, organizationMember);

    public TeamMember MergeToEntity(
        Shared.Models.TeamMember src,
        TeamMember dest,
        Shared.Database.Entities.Team team,
        Shared.Database.Entities.Customer customer,
        OrganizationMember? organizationMember)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.MembershipType = src.MembershipType;
        dest.Team = team;
        dest.Customer = customer;
        dest.OrganizationMember = organizationMember;
        return dest;
    }

    public Shared.Database.Entities.OrganizationTag MergeToEntity(
        OrganizationTag src,
        Shared.Database.Entities.OrganizationTag dest,
        Shared.Database.Entities.Organization organization)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Name = src.Name;
        dest.Type = src.Type;
        dest.Organization = organization;
        return dest;
    }

    public Shared.Database.Entities.OrganizationTag
        MapToEntity(OrganizationTag src, Shared.Database.Entities.Organization organization) =>
        MergeToEntity(src, new Shared.Database.Entities.OrganizationTag(), organization);

    public IEnumerable<Shared.Database.Entities.Identity>
        MapToEntity(IEnumerable<Identity> src, Shared.Database.Entities.Customer? customer) =>
        src.Select(identity => MapToEntity(identity, customer));

    private static IEnumerable<Location> MapTo(IEnumerable<Shared.Database.Entities.Location?>? src) =>
        (src is null ? [] : src.Where(item => item is not null).Select(item => MapTo(item, true, true)))!;

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
                Name = src.Name,
                LogoUrl = src.LogoUrl
            };

    private static Location? MapTo(Shared.Database.Entities.Location? src, bool includeTags, bool includeDesks) =>
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
                Organization = MapTo(src.Organization),
                Tags = includeTags ? MapTo(src.Tags, true).ToList() : [],
                Desks = includeDesks ? MapTo(src.Desks).ToList() : []
            };

    private static IEnumerable<LocationTag> MapTo(
        IEnumerable<Shared.Database.Entities.LocationTag?>? src,
        bool includeDesks) =>
        (src is null ? [] : src.Where(item => item is not null).Select(item => MapTo(item, includeDesks)))!;

    private static LocationTag? MapTo(Shared.Database.Entities.LocationTag? src, bool includeDesks) =>
        src is null
            ? null
            : new LocationTag
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                EventRaisedAt = src.EventRaisedAt,
                Name = src.Name,
                Type = src.Type,
                TaggedDesks = includeDesks ? MapTo(src.TaggedDesks).ToList() : []
            };

    private static IEnumerable<OrganizationTag> MapTo(
        IEnumerable<Shared.Database.Entities.OrganizationTag?>? src,
        bool includeDesks) =>
        (src is null ? [] : src.Where(item => item is not null).Select(item => MapTo(item, includeDesks)))!;

    private static OrganizationTag? MapTo(Shared.Database.Entities.OrganizationTag? src, bool includeDesks) =>
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
                Type = src.Type,
                TaggedDesks = includeDesks ? MapTo(src.TaggedDesks).ToList() : []
            };

    private static IEnumerable<Desk> MapTo(IEnumerable<Shared.Database.Entities.Desk?>? src) =>
        (src is null ? [] : src.Where(item => item is not null).Select(MapTo))!;

    private static Desk? MapTo(Shared.Database.Entities.Desk? src) =>
        src is null
            ? null
            : new Desk
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                EventRaisedAt = src.EventRaisedAt,
                Name = src.Name,
                Location = MapTo(src.Location, false, false)!,
                Tags = MapTo(src.Tags, false).ToList(),
                OrganizationTags = MapTo(src.OrganizationTags, false).ToList()
            };

    private static IEnumerable<Team> MapTo(IEnumerable<Shared.Database.Entities.Team?>? src) =>
        (src is null ? [] : src.Where(item => item is not null).Select(MapTo))!;

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
                Name = src.Name,
                Organization = MapTo(src.Organization)
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
                Type = src.Type,
            };

}
