using Api.Shared.Services.Models;
using Core.Shared.Database.Entities;
using Event = Api.Shared.Clients.Events.Skedular.Customer.V1.Event;
using Customer = Core.Shared.Models.Customer;
using CustomerType = Api.Shared.Clients.Events.Skedular.Customer.V1.CustomerType;
using Identity = Core.Shared.Database.Entities.Identity;
using Organization = Core.Shared.Models.Organization;
using OrganizationMember = Core.Shared.Database.Entities.OrganizationMember;
using OrganizationType = Api.Shared.Clients.Events.Skedular.Organization.V1.OrganizationType;
using OrganizationMemberRole = Api.Shared.Clients.Events.Skedular.Organization.V1.OrganizationMemberRole;
using OrganizationMemberStatus = Api.Shared.Clients.Events.Skedular.Organization.V1.OrganizationMemberStatus;

namespace Core.Processors.Mappers;

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
            Identities = customer.Identities
                .Select(item => new Shared.Models.Identity
                {
                    Id = item.Id,
                    Email = item.Email,
                    EmailVerified = item.EmailVerified,
                })
                .ToList(),
        };
    }

    public Shared.Database.Entities.Customer MergeToEntity(Customer src, Shared.Database.Entities.Customer dest, IEnumerable<Identity> identities)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Type = src.Type.ToNullableCustomerType();
        dest.Identities = identities.ToList();

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

        organization.OrganizationMembers = organizationAfterState.Members.Select(item => new Shared.Models.OrganizationMember
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
