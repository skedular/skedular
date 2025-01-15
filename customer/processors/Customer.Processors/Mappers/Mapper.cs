using Api.Shared.Services.Models;
using Desk = Customer.Shared.Models.Desk;
using Event = Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Event;
using Identity = Customer.Shared.Models.Identity;
using Location = Customer.Shared.Models.Location;
using LocationMember = Customer.Shared.Database.Entities.LocationMember;
using Organization = Customer.Shared.Models.Organization;
using OrganizationMember = Customer.Shared.Database.Entities.OrganizationMember;
using OrganizationTag = Customer.Shared.Models.OrganizationTag;
using Role = Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Role;
using Status = Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Status;
using Team = Customer.Shared.Models.Team;
using TeamMember = Customer.Shared.Database.Entities.TeamMember;

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
        var organizationAfterState = src.Data.Organization;
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

        organization.Tags = organizationAfterState.Tags.Select(item => new OrganizationTag
        {
            Id = item.Id,
            EventRaisedAt = eventRaisedAt,
            Name = item.Name,
            Type = item.TagType switch
            {
                OrganizationTagTypeConstants.Custom => OrganizationTagType.Custom,
                OrganizationTagTypeConstants.Zone => OrganizationTagType.Zone,
                _ => throw new ArgumentOutOfRangeException()
            },
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
                string.IsNullOrWhiteSpace(locationAfterState.OrganizationId)
                    ? null
                    : new Organization { Id = locationAfterState.OrganizationId }
        };

        location.LocationMembers = locationAfterState.Members.Select(item => new Shared.Models.LocationMember
        {
            Id = item.Id,
            DeletedAt = deletedAt,
            EventRaisedAt = eventRaisedAt,
            Role = item.Role switch
            {
                Api.Shared.Clients.Events.Skedular.Location.V1.Value.Role.Owner => LocationMemberRole.Owner,
                Api.Shared.Clients.Events.Skedular.Location.V1.Value.Role.Administrator => LocationMemberRole
                    .Administrator,
                Api.Shared.Clients.Events.Skedular.Location.V1.Value.Role.Member => LocationMemberRole
                    .Member,
                _ => throw new ArgumentOutOfRangeException()
            },
            Customer = new Shared.Models.Customer { Id = item.CustomerId },
            Location = location
        }).ToList();

        location.Desks = locationAfterState.Desks.Select(item => new Desk
        {
            Id = item.Id,
            DeletedAt = deletedAt,
            EventRaisedAt = eventRaisedAt,
            Name = item.Name,
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
        dest.Role = src.Role switch
        {
            OrganizationMemberRole.Owner => OrganizationMemberRoleConstants.Owner,
            OrganizationMemberRole.Administrator => OrganizationMemberRoleConstants.Administrator,
            OrganizationMemberRole.Member => OrganizationMemberRoleConstants.Member,
            _ => throw new ArgumentOutOfRangeException()
        };
        dest.Status = src.Status switch
        {
            OrganizationMemberStatus.Active => OrganizationMemberStatusConstants.Active,
            OrganizationMemberStatus.Inactive => OrganizationMemberStatusConstants.Inactive,
            _ => throw new ArgumentOutOfRangeException()
        };
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
        dest.Role = src.Role switch
        {
            LocationMemberRole.Owner => LocationRoleConstants.Owner,
            LocationMemberRole.Administrator => LocationRoleConstants.Administrator,
            LocationMemberRole.Member => LocationRoleConstants.Member,
            _ => throw new ArgumentOutOfRangeException()
        };
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
        dest.Role = src.Role switch
        {
            TeamMemberRole.Owner => TeamMemberRoleConstants.Owner,
            TeamMemberRole.Administrator => TeamMemberRoleConstants.Administrator,
            TeamMemberRole.Member => TeamMemberRoleConstants.Member,
            _ => throw new ArgumentOutOfRangeException()
        };
        dest.Status = src.Status switch
        {
            TeamMemberStatus.Active => TeamMemberStatusConstants.Active,
            TeamMemberStatus.Inactive => TeamMemberStatusConstants.Inactive,
            _ => throw new ArgumentOutOfRangeException()
        };
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
        dest.Type = src.Type switch
        {
            OrganizationTagType.Custom => OrganizationTagTypeConstants.Custom,
            OrganizationTagType.Zone => OrganizationTagTypeConstants.Zone,
            _ => throw new ArgumentOutOfRangeException()
        };
        dest.Color = src.Color;
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
                DeletedAt = src.DeletedAt,
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
                Type = src.Type switch
                {
                    OrganizationTagTypeConstants.Custom => OrganizationTagType.Custom,
                    OrganizationTagTypeConstants.Zone => OrganizationTagType.Zone,
                    _ => throw new ArgumentOutOfRangeException()
                },
                Color = src.Color
            };
}
