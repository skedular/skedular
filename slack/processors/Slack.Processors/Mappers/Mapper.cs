using Api.Shared.Clients.Events.Skedular.Organization.V1.Value;
using Api.Shared.Services.Models;
using Enterprise.Shared;
using Slack.Shared.Database.Entities;
using Location = Slack.Shared.Models.Location;
using Team = Slack.Shared.Models.Team;
using Organization = Slack.Shared.Models.Organization;
using Customer = Slack.Shared.Models.Customer;
using Event = Api.Shared.Clients.Events.Skedular.Customer.V1.Value.Event;
using Identity = Slack.Shared.Models.Identity;
using Role = Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Role;
using OrganizationMember = Slack.Shared.Database.Entities.OrganizationMember;
using OrganizationType = Api.Shared.Clients.Events.Skedular.Organization.V1.Value.OrganizationType;

namespace Slack.Processors.Mappers;

public interface IMapper
{
    Customer MapTo(Event src);

    Shared.Database.Entities.Customer MergeToEntity(
        Customer src,
        Shared.Database.Entities.Customer dest,
        ICollection<Shared.Database.Entities.Identity> identities);

    Shared.Database.Entities.Identity MapToEntity(Identity src, Shared.Database.Entities.Customer? customer);

    Shared.Database.Entities.Identity MergeToEntity(
        Identity src,
        Shared.Database.Entities.Identity dest,
        Shared.Database.Entities.Customer? customer);

    Location MapTo(Api.Shared.Clients.Events.Skedular.Location.V1.Value.Event src);
    Shared.Database.Entities.Location MergeToEntity(Location src, Shared.Database.Entities.Location dest);
    Organization MapTo(Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Event src);
    Shared.Database.Entities.Organization MergeToEntity(Organization src, Shared.Database.Entities.Organization dest);
    Team MapTo(Api.Shared.Clients.Events.Skedular.Team.V1.Value.Event src);
    Shared.Database.Entities.Team MergeToEntity(Team src, Shared.Database.Entities.Team dest);

    OrganizationMember MapToEntity(
        Shared.Models.OrganizationMember src,
        Shared.Database.Entities.Organization organization,
        Shared.Database.Entities.Customer customer);

    OrganizationMember MergeToEntity(
        Shared.Models.OrganizationMember src,
        OrganizationMember dest,
        Shared.Database.Entities.Organization organization,
        Shared.Database.Entities.Customer customer);

    OrganizationSsoSetting MapTo(Shared.Models.OrganizationSsoSetting src, Shared.Database.Entities.Organization organization);

    OrganizationSsoSetting MergeTo(
        Shared.Models.OrganizationSsoSetting src,
        OrganizationSsoSetting dest,
        Shared.Database.Entities.Organization organization);
}

public class Mapper : IMapper
{
    public Customer MapTo(Event src)
    {
        var customer = src.Data.Customer;
        var deletedAt = customer.DeletedAt?.ToDateTimeOffset();
        var eventRaisedAt = src.Metadata.Time?.ToDateTimeOffset() ?? DateTimeOffset.MinValue;

        return new Customer
        {
            Id = customer.Id,
            DeletedAt = deletedAt,
            EventRaisedAt = eventRaisedAt,
            Timezone = customer.Timezone.ToSafeString(),
            Identities = customer.Identities
                .Select(item => new Identity { Id = item.Id, Email = item.Email.ToSafeString(), EmailVerified = item.EmailVerified })
                .ToList()
        };
    }

    public Shared.Database.Entities.Customer MergeToEntity(
        Customer src,
        Shared.Database.Entities.Customer dest,
        ICollection<Shared.Database.Entities.Identity> identities)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Timezone = src.Timezone;
        dest.Identities = identities;

        return dest;
    }

    public Shared.Database.Entities.Identity MapToEntity(Identity src, Shared.Database.Entities.Customer? customer) =>
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

    public Location MapTo(Api.Shared.Clients.Events.Skedular.Location.V1.Value.Event src)
    {
        var locationAfterState = src.Data.Location;
        var deletedAt = locationAfterState.DeletedAt?.ToDateTimeOffset();
        var eventRaisedAt = src.Metadata.Time?.ToDateTimeOffset() ?? DateTimeOffset.MinValue;

        return new Location
        {
            Id = locationAfterState.Id,
            DeletedAt = deletedAt,
            EventRaisedAt = eventRaisedAt,
            Timezone = locationAfterState.Timezone.ToSafeString()
        };
    }

    public Shared.Database.Entities.Location MergeToEntity(Location src, Shared.Database.Entities.Location dest)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Timezone = src.Timezone;
        return dest;
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
            Type = organizationAfterState.Type switch
            {
                OrganizationType.Private => Api.Shared.Services.Models.OrganizationType.Private,
                OrganizationType.Marketplace => Api.Shared.Services.Models.OrganizationType.Marketplace,
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
            Customer = new Customer { Id = item.CustomerId },
            Organization = organization
        }).ToList();

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
                Organization = organization
            };

        return organization;
    }

    public Shared.Database.Entities.Organization MergeToEntity(Organization src, Shared.Database.Entities.Organization dest)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.UniqueAlphanumericName = src.UniqueAlphanumericName;
        dest.Type = src.Type.ToOrganizationType();
        return dest;
    }

    public Team MapTo(Api.Shared.Clients.Events.Skedular.Team.V1.Value.Event src)
    {
        var teamAfterState = src.Data.Team;
        var deletedAt = teamAfterState.DeletedAt?.ToDateTimeOffset();
        var eventRaisedAt = src.Metadata.Time?.ToDateTimeOffset() ?? DateTimeOffset.MinValue;

        return new Team
        {
            Id = teamAfterState.Id, DeletedAt = deletedAt, EventRaisedAt = eventRaisedAt, Timezone = teamAfterState.Timezone.ToSafeString()
        };
    }

    public Shared.Database.Entities.Team MergeToEntity(Team src, Shared.Database.Entities.Team dest)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Timezone = src.Timezone;
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
