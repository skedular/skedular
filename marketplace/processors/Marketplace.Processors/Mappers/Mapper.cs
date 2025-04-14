using Api.Shared.Clients.Events.Skedular.Organization.V1.Value;
using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Marketplace.Shared.Database.Entities;
using Event = Api.Shared.Clients.Events.Skedular.Customer.V1.Value.Event;
using Customer = Marketplace.Shared.Models.Customer;
using Identity = Marketplace.Shared.Database.Entities.Identity;
using Offering = Api.Shared.Services.Models.Offering;
using Organization = Marketplace.Shared.Models.Organization;
using OrganizationMember = Marketplace.Shared.Database.Entities.OrganizationMember;
using Status = Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Status;
using OrganizationTag = Marketplace.Shared.Models.OrganizationTag;

namespace Marketplace.Processors.Mappers;

public interface IMapper
{
    Customer MapTo(Event src);

    Shared.Database.Entities.Customer MergeToEntity(Customer src, Shared.Database.Entities.Customer dest, ICollection<Identity> identities);
    Identity MapToEntity(Shared.Models.Identity src, Shared.Database.Entities.Customer? customer);
    Identity MergeToEntity(Shared.Models.Identity src, Identity dest, Shared.Database.Entities.Customer? customer);
    Organization MapTo(Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Event src);
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

    Shared.Database.Entities.OrganizationTag MapToEntity(OrganizationTag src, Shared.Database.Entities.Organization organization);

    Shared.Database.Entities.OrganizationTag MergeToEntity(
        OrganizationTag src,
        Shared.Database.Entities.OrganizationTag dest,
        Shared.Database.Entities.Organization organization);

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
            Identities = customer.Identities
                .Select(item => new Shared.Models.Identity { Id = item.Id, Email = item.Email, EmailVerified = item.EmailVerified })
                .ToList()
        };
    }

    public Shared.Database.Entities.Customer MergeToEntity(Customer src, Shared.Database.Entities.Customer dest, ICollection<Identity> identities)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Timezone = src.Timezone;
        dest.Identities = identities;

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
                Customer = new Customer { Id = item.CustomerId },
                Organization = organization
            };
        }).ToList();

        organization.Tags = organizationAfterState.Tags.Select(item => new OrganizationTag
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
            : new Shared.Models.OrganizationSsoSetting
            {
                Id = organizationAfterState.SsoSettings.Id,
                EventRaisedAt = eventRaisedAt,
                EntityId = organizationAfterState.SsoSettings.EntityId,
                LoginUrl = organizationAfterState.SsoSettings.LoginUrl,
                AppFederationMetadataUrl = organizationAfterState.SsoSettings.AppFederationMetadataUrl,
                Organization = organization
            };

        return organization;
    }

    public Shared.Database.Entities.Organization MergeToEntity(Organization src, Shared.Database.Entities.Organization dest)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Offering = src.Offering;
        dest.Type = src.Type.ToOrganizationType();
        dest.MemberVisibilityPolicy = src.MemberVisibilityPolicy.ToOrganizationMemberVisibilityPolicy();
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
        dest.Organization = organization;

        return dest;
    }
}
