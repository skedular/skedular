using Api.Shared.Clients.Events.Skedular.Organization.V1.Value;
using Api.Shared.Models;
using Customer.Shared.Database.Entities;
using Desk = Customer.Shared.Models.Desk;
using Identity = Customer.Shared.Models.Identity;
using Location = Customer.Shared.Models.Location;
using Organization = Customer.Shared.Models.Organization;
using OrganizationTag = Customer.Shared.Models.OrganizationTag;
using Team = Customer.Shared.Models.Team;

namespace Customer.Processors.Mappers;

public interface IMapper
{
    Organization MapTo(Event src);
    Location MapTo(Api.Shared.Clients.Events.Skedular.Location.V1.Value.Event src);
    Team MapTo(Api.Shared.Clients.Events.Skedular.Team.V1.Value.Event src);
    Shared.Models.Customer? MapTo(Shared.Database.Entities.Customer? src);
    Shared.Database.Entities.Organization MapToEntity(Organization src);
    Shared.Database.Entities.Organization MergeToEntity(Organization src, Shared.Database.Entities.Organization dest);

    Shared.Database.Entities.Location MapToEntity(Location src, Shared.Database.Entities.Organization? organization);

    Shared.Database.Entities.Location MergeToEntity(
        Location src,
        Shared.Database.Entities.Location dest,
        Shared.Database.Entities.Organization? organization);

    Shared.Database.Entities.Desk MapToEntity(Desk src, Shared.Database.Entities.Location location);

    Shared.Database.Entities.Desk MergeToEntity(
        Desk src,
        Shared.Database.Entities.Desk dest,
        Shared.Database.Entities.Location location);

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
    public Organization MapTo(Event src)
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
            EventRaisedAt = eventRaisedAt,
            Name = item.Name,
            Type = item.TagType,
            Organization = organization
        }).ToList();

        return organization;
    }

    public Location MapTo(Api.Shared.Clients.Events.Skedular.Location.V1.Value.Event src)
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
                Api.Shared.Clients.Events.Skedular.Location.V1.Value.MembershipType.Owner => LocationMembershipType
                    .Owner,
                Api.Shared.Clients.Events.Skedular.Location.V1.Value.MembershipType.Administrator =>
                    LocationMembershipType.Administrator,
                Api.Shared.Clients.Events.Skedular.Location.V1.Value.MembershipType.Member =>
                    LocationMembershipType.Member,
                _ => throw new ArgumentOutOfRangeException()
            },
            Customer = new Shared.Models.Customer { Id = item.CustomerId },
            Location = location
        }).ToList();

        location.Desks = locationAfterState.Desks.Select(item => new Desk
        {
            Id = item.Id,
            EventRaisedAt = eventRaisedAt,
            Name = item.Name,
            Location = location
        }).ToList();

        return location;
    }

    public Team MapTo(Api.Shared.Clients.Events.Skedular.Team.V1.Value.Event src)
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
                Api.Shared.Clients.Events.Skedular.Team.V1.Value.MembershipType.Owner => TeamMembershipType
                    .Owner,
                Api.Shared.Clients.Events.Skedular.Team.V1.Value.MembershipType.Administrator =>
                    TeamMembershipType.Administrator,
                Api.Shared.Clients.Events.Skedular.Team.V1.Value.MembershipType.Member =>
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
                PhoneNumber = src.PhoneNumber,
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
                PreferredDesks = MapTo(src.PreferredDesks).ToList(),
                DefaultTeams = MapTo(src.DefaultTeams).ToList(),
                PreferredOrganizationTags = MapTo(src.PreferredOrganizationTags).ToList()
            };

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

    public Shared.Database.Entities.Desk MapToEntity(Desk src, Shared.Database.Entities.Location location) =>
        MergeToEntity(src, new Shared.Database.Entities.Desk(), location);

    public Shared.Database.Entities.Desk MergeToEntity(
        Desk src,
        Shared.Database.Entities.Desk dest,
        Shared.Database.Entities.Location location)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Name = src.Name;
        dest.Location = location;
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
                Name = src.Name,
                LogoUrl = src.LogoUrl
            };

    private static Location? MapTo(Shared.Database.Entities.Location? src, bool includeDesks) =>
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
                Desks = includeDesks ? MapTo(src.Desks).ToList() : []
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
                ModifiedAt = src.ModifiedAt,
                EventRaisedAt = src.EventRaisedAt,
                Name = src.Name,
                Location = MapTo(src.Location, false)!
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
                ModifiedAt = src.ModifiedAt,
                EventRaisedAt = src.EventRaisedAt,
                Name = src.Name,
                Type = src.Type
            };
}
