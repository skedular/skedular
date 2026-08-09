using Api.Shared.Services.Models;
using Enterprise.Shared;
using MsTeams.Shared.Models;
using Event = Api.Shared.Clients.Events.Skedular.Customer.V1.Event;
using Customer = MsTeams.Shared.Models.Customer;
using CustomerType = Api.Shared.Clients.Events.Skedular.Customer.V1.CustomerType;
using Identity = MsTeams.Shared.Database.Entities.Identity;
using Location = MsTeams.Shared.Models.Location;
using Organization = MsTeams.Shared.Models.Organization;
using OrganizationMember = MsTeams.Shared.Database.Entities.OrganizationMember;
using OrganizationMemberRole = Api.Shared.Clients.Events.Skedular.Organization.V1.OrganizationMemberRole;
using OrganizationSsoSetting = MsTeams.Shared.Database.Entities.OrganizationSsoSetting;
using OrganizationType = Api.Shared.Clients.Events.Skedular.Organization.V1.OrganizationType;
using OrganizationMemberStatus = Api.Shared.Clients.Events.Skedular.Organization.V1.OrganizationMemberStatus;
using Team = MsTeams.Shared.Models.Team;

namespace MsTeams.Processors.Mappers;

public interface IEventMapper
{
    Customer MapTo(Event src);
    Shared.Database.Entities.Customer MergeToEntity(Customer src, Shared.Database.Entities.Customer dest, IEnumerable<Identity> identities);
    Identity MapToEntity(Shared.Models.Identity src, Shared.Database.Entities.Customer? customer);
    Identity MergeToEntity(Shared.Models.Identity src, Identity dest, Shared.Database.Entities.Customer? customer);
    Organization MapTo(Api.Shared.Clients.Events.Skedular.Organization.V1.Event src);
    Shared.Database.Entities.Organization MergeToEntity(Organization src, Shared.Database.Entities.Organization dest);

    OrganizationMember MapToEntity(
        Shared.Models.OrganizationMember src,
        Shared.Database.Entities.Organization organization,
        Shared.Database.Entities.Customer customer);

    OrganizationMember MergeToEntity(
        Shared.Models.OrganizationMember src,
        OrganizationMember dest,
        Shared.Database.Entities.Organization organization,
        Shared.Database.Entities.Customer customer);

    Location MapTo(Api.Shared.Clients.Events.Skedular.Location.V1.Event src);
    Shared.Database.Entities.Location MergeToEntity(Location src, Shared.Database.Entities.Location dest);
    Team MapTo(Api.Shared.Clients.Events.Skedular.Team.V1.Event src);
    Shared.Database.Entities.Team MergeToEntity(Team src, Shared.Database.Entities.Team dest);
    OrganizationSsoSetting MapTo(Shared.Models.OrganizationSsoSetting src, Shared.Database.Entities.Organization organization);

    OrganizationSsoSetting MergeTo(
        Shared.Models.OrganizationSsoSetting src,
        OrganizationSsoSetting dest,
        Shared.Database.Entities.Organization organization);
}

public class EventMapper : IEventMapper
{
    public Customer MapTo(Event src)
    {
        var customer = src.Data.Customer;
        var deletedAt = customer.DeletedAt?.ToDateTimeOffset();
        var eventRaisedAt = src.Metadata.Time.ToDateTimeOffset();

        return new Customer
        {
            Id = customer.Id,
            DeletedAt = deletedAt,
            EventRaisedAt = eventRaisedAt,
            Type = customer.Type switch
            {
                CustomerType.Guest => Api.Shared.Services.Models.CustomerType.Guest,
                CustomerType.Registered => Api.Shared.Services.Models.CustomerType.Registered,
                _ => throw new ArgumentOutOfRangeException(nameof(customer.Type), customer.Type,
                    $"Unexpected value for {nameof(customer.Type)}: {customer.Type}. Update enum mapping or caller input."),
            },
            Identities =
            [
                .. customer.Identities
                    .Select(item => new Shared.Models.Identity
                    {
                        Id = item.Id,
                        Email = item.Email,
                        EmailVerified = item.EmailVerified,
                    }),
            ],
        };
    }

    public Shared.Database.Entities.Customer MergeToEntity(Customer src, Shared.Database.Entities.Customer dest, IEnumerable<Identity> identities)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Type = src.Type.ToNullableCustomerType();
        dest.Identities = [.. identities];

        return dest;
    }

    public Identity MapToEntity(Shared.Models.Identity src, Shared.Database.Entities.Customer? customer) =>
        MergeToEntity(src, new Identity(), customer);

    public Identity MergeToEntity(Shared.Models.Identity src,
        Identity dest,
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

    public Organization MapTo(Api.Shared.Clients.Events.Skedular.Organization.V1.Event src)
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
                OrganizationType.Host => Api.Shared.Services.Models.OrganizationType.Host,
                _ => throw new ArgumentOutOfRangeException(nameof(organizationAfterState.Type), organizationAfterState.Type,
                    $"Unexpected value for {nameof(organizationAfterState.Type)}: {organizationAfterState.Type}. Update enum mapping or caller input."),
            },
            IsOwnershipVerified = organizationAfterState.IsOwnershipVerified,
        };

        organization.AzureTenants =
        [
            .. organizationAfterState.AzureTenantIds
                .Select(item => new AzureTenant
                {
                    Id = item,
                    Organization = organization,
                    EventRaisedAt = eventRaisedAt,
                }),
        ];

        organization.OrganizationMembers =
        [
            .. organizationAfterState.Members.Select(item => new Shared.Models.OrganizationMember
            {
                Id = item.Id,
                Role = item.Role switch
                {
                    OrganizationMemberRole.Owner => Api.Shared.Services.Models.OrganizationMemberRole.Owner,
                    OrganizationMemberRole.Administrator => Api.Shared.Services.Models.OrganizationMemberRole.Administrator,
                    OrganizationMemberRole.Member => Api.Shared.Services.Models.OrganizationMemberRole.Member,
                    _ => throw new ArgumentOutOfRangeException(nameof(item.Role), item.Role,
                        $"Unexpected value for {nameof(item.Role)}: {item.Role}. Update enum mapping or caller input."),
                },
                Status = item.Status switch
                {
                    OrganizationMemberStatus.Active => Api.Shared.Services.Models.OrganizationMemberStatus.Active,
                    OrganizationMemberStatus.Inactive => Api.Shared.Services.Models.OrganizationMemberStatus.Inactive,
                    _ => throw new ArgumentOutOfRangeException(nameof(item.Status), item.Status,
                        $"Unexpected value for {nameof(item.Status)}: {item.Status}. Update enum mapping or caller input."),
                },
                Customer = new Customer
                {
                    Id = item.CustomerId,
                },
                Organization = organization,
            }),
        ];

        organization.OrganizationSsoSettings = organizationAfterState.SsoSettings is null
            ? null
            : new Shared.Models.OrganizationSsoSetting
            {
                Id = organizationAfterState.SsoSettings.Id,
                EventRaisedAt = eventRaisedAt,
                EntityId = organizationAfterState.SsoSettings.EntityId,
                LoginUrl = organizationAfterState.SsoSettings.LoginUrl,
                AppFederationMetadataUrl = organizationAfterState.SsoSettings.AppFederationMetadataUrl,
                IsActive = organizationAfterState.SsoSettings.IsActive,
                Organization = organization,
            };

        return organization;
    }

    public Shared.Database.Entities.Organization MergeToEntity(Organization src, Shared.Database.Entities.Organization dest)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.CustomDomain = src.CustomDomain;
        dest.Type = src.Type.ToOrganizationType();
        dest.IsOwnershipVerified = src.IsOwnershipVerified;
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

    public Location MapTo(Api.Shared.Clients.Events.Skedular.Location.V1.Event src)
    {
        var locationAfterState = src.Data.Location;
        var deletedAt = locationAfterState.DeletedAt?.ToDateTimeOffset();
        var eventRaisedAt = src.Metadata.Time.ToDateTimeOffset();

        return new Location
        {
            Id = locationAfterState.Id,
            DeletedAt = deletedAt,
            EventRaisedAt = eventRaisedAt,
            Timezone = locationAfterState.Timezone.ToSafeString(),
        };
    }

    public Shared.Database.Entities.Location MergeToEntity(Location src, Shared.Database.Entities.Location dest)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Timezone = src.Timezone;
        return dest;
    }

    public Team MapTo(Api.Shared.Clients.Events.Skedular.Team.V1.Event src)
    {
        var teamAfterState = src.Data.Team;
        var deletedAt = teamAfterState.DeletedAt?.ToDateTimeOffset();
        var eventRaisedAt = src.Metadata.Time.ToDateTimeOffset();

        return new Team
        {
            Id = teamAfterState.Id,
            DeletedAt = deletedAt,
            EventRaisedAt = eventRaisedAt,
            Timezone = teamAfterState.Timezone.ToSafeString(),
        };
    }

    public Shared.Database.Entities.Team MergeToEntity(Team src, Shared.Database.Entities.Team dest)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Timezone = src.Timezone;
        return dest;
    }

    public OrganizationSsoSetting MapTo(Shared.Models.OrganizationSsoSetting src, Shared.Database.Entities.Organization organization) =>
        MergeTo(src, new OrganizationSsoSetting(), organization);

    public OrganizationSsoSetting MergeTo(
        Shared.Models.OrganizationSsoSetting src,
        OrganizationSsoSetting dest,
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
}
