using Api.Shared.Clients.Events.Skedular.Organization.V1.Value;
using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Enterprise.Shared;
using Stripe;
using Address = Payment.Shared.Models.Address;
using Customer = Payment.Shared.Models.Customer;
using Event = Api.Shared.Clients.Events.Skedular.Customer.V1.Value.Event;
using Identity = Payment.Shared.Models.Identity;
using Organization = Payment.Shared.Models.Organization;
using OrganizationMember = Payment.Shared.Database.Entities.OrganizationMember;
using OrganizationOffering = Payment.Shared.Database.Entities.OrganizationOffering;
using OrganizationSsoSetting = Payment.Shared.Database.Entities.OrganizationSsoSetting;
using OrganizationStripeConnectAccount = Payment.Shared.Database.Entities.OrganizationStripeConnectAccount;

namespace Payment.Processors.Mappers;

public interface IMapper
{
    Customer MapTo(Event src);
    Organization MapTo(Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Event src);

    Shared.Database.Entities.Customer MergeToEntity(
        Customer src,
        Shared.Database.Entities.Customer dest,
        ICollection<Shared.Database.Entities.Identity> identities);

    Shared.Database.Entities.Identity MapToEntity(Identity src, Shared.Database.Entities.Customer? customer);

    Shared.Database.Entities.Identity MergeToEntity(
        Identity src,
        Shared.Database.Entities.Identity dest,
        Shared.Database.Entities.Customer? customer);

    Shared.Database.Entities.Organization MapToEntity(Organization src);
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

    OrganizationOffering MapToEntity(Shared.Models.OrganizationOffering src, Shared.Database.Entities.Organization organization);

    OrganizationOffering MergeToEntity(
        Shared.Models.OrganizationOffering src,
        OrganizationOffering dest,
        Shared.Database.Entities.Organization organization);

    OrganizationSsoSetting MapTo(Shared.Models.OrganizationSsoSetting src, Shared.Database.Entities.Organization organization);

    OrganizationSsoSetting MergeTo(
        Shared.Models.OrganizationSsoSetting src,
        OrganizationSsoSetting dest,
        Shared.Database.Entities.Organization organization);

    Shared.Database.Entities.Address MapTo(Address src, Shared.Database.Entities.Organization organization);

    Shared.Database.Entities.Address MergeTo(
        Address src,
        Shared.Database.Entities.Address dest,
        Shared.Database.Entities.Organization organization);

    OrganizationStripeConnectAccount MergeTo(Account src, OrganizationStripeConnectAccount dest);
    Shared.Models.OrganizationStripeConnectAccount MapTo(OrganizationStripeConnectAccount src);
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
            Identities = customer.Identities.Select(item => new Identity { Id = item.Id }).ToList()
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
            Type = organizationAfterState.Type.ToOrganizationType(),
            MemberVisibilityPolicy = organizationAfterState.MemberVisibilityPolicy.ToOrganizationMemberVisibilityPolicy(),
            ContactEmail = organizationAfterState.ContactEmail,
            ContactPhone = organizationAfterState.ContactPhone,
            PhysicalAddress = MapTo(organizationAfterState.PhysicalAddress)
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

        organization.OrganizationOfferings =
        [
            new Shared.Models.OrganizationOffering
            {
                Id = organizationAfterState.Offering.Id,
                EventRaisedAt = eventRaisedAt,
                Code = organizationAfterState.Offering.Code.ToOfferingCode(),
                Start = organizationAfterState.Offering.Start.ToDateTimeOffset(),
                End = organizationAfterState.Offering.End.ToDateTimeOffset(),
                Organization = organization
            }
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
                Organization = organization
            };

        return organization;
    }

    public Shared.Database.Entities.Customer MergeToEntity(
        Customer src,
        Shared.Database.Entities.Customer dest,
        ICollection<Shared.Database.Entities.Identity> identities)
    {
        dest.Id = src.Id;
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
        if (customer is not null)
        {
            dest.Customer = customer;
        }

        return dest;
    }

    public Shared.Database.Entities.Organization MapToEntity(Organization src) => MergeToEntity(src, new Shared.Database.Entities.Organization());

    public Shared.Database.Entities.Organization MergeToEntity(Organization src, Shared.Database.Entities.Organization dest)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Name = src.Name;
        dest.Type = src.Type.ToOrganizationType();
        dest.MemberVisibilityPolicy = src.MemberVisibilityPolicy.ToOrganizationMemberVisibilityPolicy();
        dest.ContactEmail = src.ContactEmail;
        dest.ContactPhone = src.ContactPhone;
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

    public OrganizationOffering MapToEntity(Shared.Models.OrganizationOffering src, Shared.Database.Entities.Organization organization) =>
        MergeToEntity(src, new OrganizationOffering(), organization);

    public OrganizationOffering MergeToEntity(
        Shared.Models.OrganizationOffering src,
        OrganizationOffering dest,
        Shared.Database.Entities.Organization organization)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Code = src.Code;
        dest.Start = src.Start;
        dest.End = src.End;
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

    public Shared.Database.Entities.Address MapTo(Address src, Shared.Database.Entities.Organization organization) =>
        MergeTo(src, new Shared.Database.Entities.Address(), organization);

    public Shared.Database.Entities.Address MergeTo(
        Address src,
        Shared.Database.Entities.Address dest,
        Shared.Database.Entities.Organization organization)
    {
        dest.Id = src.Id;
        dest.AddressLine1 = src.AddressLine1;
        dest.AddressLine2 = src.AddressLine2;
        dest.Suburb = src.Suburb;
        dest.City = src.City;
        dest.Province = src.Province;
        dest.Zipcode = src.Zipcode;
        dest.Country = src.Country;
        dest.Latitude = src.Latitude;
        dest.Longitude = src.Longitude;
        dest.Organization = organization;
        return dest;
    }

    public OrganizationStripeConnectAccount MergeTo(Account src, OrganizationStripeConnectAccount dest)
    {
        dest.Id = src.Id;
        dest.ChargesEnabled = src.ChargesEnabled;
        dest.PayoutsEnabled = src.PayoutsEnabled;
        dest.Type = src.Type.ToSafeString();
        dest.Country = src.Country.ToSafeString();
        dest.DefaultCurrency = src.DefaultCurrency.ToSafeString();
        dest.BusinessType = src.BusinessType.ToSafeString();
        dest.CompanyName = src.Company is null ? string.Empty : src.Company.Name.ToSafeString();
        dest.Email = src.Email.ToSafeString();
        dest.Phone = src.Company is null ? string.Empty : src.Company.Phone.ToSafeString();
        dest.CapabilitiesCardPayments = src.Capabilities.CardPayments.ToSafeString();
        dest.CapabilitiesTransfers = src.Capabilities.Transfers.ToSafeString();
        return dest;
    }

    public Shared.Models.OrganizationStripeConnectAccount MapTo(OrganizationStripeConnectAccount src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            DeletedAt = src.DeletedAt,
            Name = src.Name,
            ChargesEnabled = src.ChargesEnabled,
            PayoutsEnabled = src.PayoutsEnabled,
            Type = src.Type,
            Country = src.Country,
            DefaultCurrency = src.DefaultCurrency,
            BusinessType = src.BusinessType,
            CompanyName = src.CompanyName,
            Email = src.Email,
            Phone = src.Phone,
            CapabilitiesCardPayments = src.CapabilitiesCardPayments,
            CapabilitiesTransfers = src.CapabilitiesTransfers,
            OnboardingUrl = src.OnboardingUrl,
            OnboardingCompletedAt = src.OnboardingCompletedAt,
            Organization = new Organization { Id = src.Organization.Id }
        };

    private static Address? MapTo(Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Address? src) =>
        src is null
            ? null
            : new Address
            {
                Id = src.Id,
                AddressLine1 = src.AddressLine1,
                AddressLine2 = src.AddressLine2,
                Suburb = src.Suburb,
                City = src.City,
                Province = src.Province,
                Zipcode = src.Zipcode,
                Country = src.Country
            };
}
