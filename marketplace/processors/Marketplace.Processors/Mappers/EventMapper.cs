using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Enterprise.Shared;
using Marketplace.Shared.Database.Entities;
using Event = Api.Shared.Clients.Events.Skedular.Customer.V1.Event;
using Customer = Marketplace.Shared.Models.Customer;
using CustomerType = Api.Shared.Clients.Events.Skedular.Customer.V1.CustomerType;
using Identity = Marketplace.Shared.Database.Entities.Identity;
using Offering = Api.Shared.Services.Models.Offering;
using Organization = Marketplace.Shared.Models.Organization;
using OrganizationMember = Marketplace.Shared.Database.Entities.OrganizationMember;
using OrganizationMemberStatus = Api.Shared.Clients.Events.Skedular.Organization.V1.OrganizationMemberStatus;
using OrganizationMemberRole = Api.Shared.Clients.Events.Skedular.Organization.V1.OrganizationMemberRole;
using OrganizationTag = Marketplace.Shared.Models.OrganizationTag;
using OrganizationType = Api.Shared.Clients.Events.Skedular.Organization.V1.OrganizationType;

namespace Marketplace.Processors.Mappers;

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
                _ => throw new ArgumentOutOfRangeException()
            },
            Identities = customer.Identities
                .Select(item => new Shared.Models.Identity { Id = item.Id, Email = item.Email, EmailVerified = item.EmailVerified })
                .ToList()
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
            Name = organizationAfterState.Name.ToSafeString(),
            Website = organizationAfterState.Website.ToSafeString(),
            LogoUrl = organizationAfterState.LogoUrl.ToSafeString(),
            CustomerFacingTermsAndConditionsUrl = organizationAfterState.CustomerFacingTermsAndConditionsUrl.ToSafeString(),
            Offering = new Offering
            {
                Id = organizationAfterState.Offering.Id,
                Code = organizationAfterState.Offering.Code.ToOfferingCode(),
                Start = organizationAfterState.Offering.Start.ToDateTimeOffset(),
                End = organizationAfterState.Offering.End.ToDateTimeOffset(),
                ActiveCustomerIds = organizationAfterState.Offering.ActiveCustomerIds.ToArray()
            },
            Type = organizationAfterState.Type switch
            {
                OrganizationType.Private => Api.Shared.Services.Models.OrganizationType.Private,
                OrganizationType.Marketplace => Api.Shared.Services.Models.OrganizationType.Marketplace,
                OrganizationType.Individual => Api.Shared.Services.Models.OrganizationType.Individual,
                _ => throw new ArgumentOutOfRangeException()
            },
            IsOwnershipVerified = organizationAfterState.IsOwnershipVerified
        };

        organization.OrganizationMembers = organizationAfterState.Members.Select(item => new Shared.Models.OrganizationMember
        {
            Id = item.Id,
            Role = item.Role switch
            {
                OrganizationMemberRole.Owner => Api.Shared.Services.Models.OrganizationMemberRole.Owner,
                OrganizationMemberRole.Administrator => Api.Shared.Services.Models.OrganizationMemberRole.Administrator,
                OrganizationMemberRole.Member => Api.Shared.Services.Models.OrganizationMemberRole.Member,
                _ => throw new ArgumentOutOfRangeException()
            },
            Status = item.Status switch
            {
                OrganizationMemberStatus.Active => Api.Shared.Services.Models.OrganizationMemberStatus.Active,
                OrganizationMemberStatus.Inactive => Api.Shared.Services.Models.OrganizationMemberStatus.Inactive,
                _ => throw new ArgumentOutOfRangeException()
            },
            Customer = new Customer { Id = item.CustomerId },
            Organization = organization
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
                IsActive = organizationAfterState.SsoSettings.IsActive,
                Organization = organization
            };

        return organization;
    }

    public Shared.Database.Entities.Organization MergeToEntity(Organization src, Shared.Database.Entities.Organization dest)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.CustomDomain = src.CustomDomain;
        dest.Name = src.Name;
        dest.Website = src.Website;
        dest.LogoUrl = src.LogoUrl;
        dest.CustomerFacingTermsAndConditionsUrl = src.CustomerFacingTermsAndConditionsUrl;
        dest.Offering = src.Offering;
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
        dest.IsActive = src.IsActive;
        dest.Organization = organization;

        return dest;
    }
}
