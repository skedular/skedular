using Api.Shared.Services.Models;
using Customer.Shared.Database.Entities;
using CustomerBillingDetails = Customer.Shared.Models.CustomerBillingDetails;
using Event = Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Event;
using Identity = Customer.Shared.Models.Identity;
using Location = Customer.Shared.Models.Location;
using Organization = Customer.Shared.Models.Organization;
using OrganizationMember = Customer.Shared.Database.Entities.OrganizationMember;
using OrganizationSsoSetting = Customer.Shared.Models.OrganizationSsoSetting;
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
    Shared.Database.Entities.Organization MergeToEntity(Organization src, Shared.Database.Entities.Organization dest);

    Shared.Database.Entities.Location MergeToEntity(
        Location src,
        Shared.Database.Entities.Location dest,
        Shared.Database.Entities.Organization organization);

    Resource MapToEntity(Shared.Models.Resource src, Shared.Database.Entities.Location location);
    Resource MergeToEntity(Shared.Models.Resource src, Resource dest, Shared.Database.Entities.Location location);
    Shared.Database.Entities.Team MergeToEntity(Team src, Shared.Database.Entities.Team dest, Shared.Database.Entities.Organization organization);

    OrganizationMember MapToEntity(
        Shared.Models.OrganizationMember src,
        Shared.Database.Entities.Organization organization,
        Shared.Database.Entities.Customer customer);

    OrganizationMember MergeToEntity(
        Shared.Models.OrganizationMember src,
        OrganizationMember dest,
        Shared.Database.Entities.Organization organization,
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
        var eventRaisedAt = src.Metadata.Time?.ToDateTimeOffset() ?? DateTimeOffset.MinValue;

        var organization = new Organization
        {
            Id = organizationAfterState.Id,
            DeletedAt = deletedAt,
            EventRaisedAt = eventRaisedAt,
            UniqueAlphanumericName =
                string.IsNullOrWhiteSpace(organizationAfterState.UniqueAlphanumericName) ? null : organizationAfterState.UniqueAlphanumericName,
            Name = organizationAfterState.Name,
            LogoUrl = organizationAfterState.LogoUrl,
            Type = organizationAfterState.Type.ToOrganizationType(),
            MemberVisibilityPolicy = organizationAfterState.MemberVisibilityPolicy.ToOrganizationMemberVisibilityPolicy()
        };

        organization.OrganizationMembers = organizationAfterState.Members.Select(item => new Shared.Models.OrganizationMember
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
        }).ToList();

        organization.Tags = organizationAfterState.Tags.Select(item => new OrganizationTag
        {
            Id = item.Id,
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
            Organization = new Organization { Id = locationAfterState.OrganizationId }
        };

        location.Resources = locationAfterState.Resources.Select(item =>
            new Shared.Models.Resource
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
                BillingDetails = MapTo(src.BillingDetails),
                IsOnboardingDone = src.IsOnboardingDone,
                Identities = MapTo(src.Identities).ToList(),
                DefaultOrganization = MapTo(src.DefaultOrganization),
                PreferredLocations = MapTo(src.PreferredLocations).ToList(),
                PreferredResources = MapTo(src.PreferredResources).ToList(),
                PreferredTeams = MapTo(src.PreferredTeams).ToList(),
                PreferredOrganizationTags = MapTo(src.PreferredOrganizationTags).ToList()
            };

    public Shared.Database.Entities.Organization MergeToEntity(Organization src, Shared.Database.Entities.Organization dest)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.UniqueAlphanumericName = src.UniqueAlphanumericName;
        dest.Name = src.Name;
        dest.LogoUrl = src.LogoUrl;
        dest.Type = src.Type.ToOrganizationType();
        dest.MemberVisibilityPolicy = src.MemberVisibilityPolicy.ToOrganizationMemberVisibilityPolicy();
        return dest;
    }

    public Shared.Database.Entities.Location MergeToEntity(
        Location src,
        Shared.Database.Entities.Location dest,
        Shared.Database.Entities.Organization organization)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Name = src.Name;
        dest.Organization = organization;
        return dest;
    }

    public Resource MapToEntity(Shared.Models.Resource src, Shared.Database.Entities.Location location) =>
        MergeToEntity(src, new Resource(), location);

    public Resource MergeToEntity(Shared.Models.Resource src, Resource dest, Shared.Database.Entities.Location location)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Name = src.Name;
        dest.Location = location;
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
        dest.Role = src.Role.ToNullableTeamMemberRole();
        dest.Status = src.Status.ToTeamMemberStatus();
        dest.Team = team;
        dest.Customer = customer;
        dest.OrganizationMember = organizationMember;
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
                UniqueAlphanumericName = src.UniqueAlphanumericName,
                Name = src.Name,
                LogoUrl = src.LogoUrl,
                Type = src.Type.ToOrganizationType(),
                MemberVisibilityPolicy = src.MemberVisibilityPolicy.ToOrganizationMemberVisibilityPolicy()
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
                Name = src.Name,
                Organization = MapTo(src.Organization),
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
                AddressLine1 = src.AddressLine1,
                AddressLine2 = src.AddressLine2,
                Suburb = src.Suburb,
                City = src.City,
                Province = src.Province,
                Zipcode = src.Zipcode,
                Country = src.Country
            };
}
