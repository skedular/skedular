using Api.Shared.Clients.Events.Skedular.Organization.V1.Value;
using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Billing.Shared.Database.Entities;
using Customer = Billing.Shared.Models.Customer;
using Event = Api.Shared.Clients.Events.Skedular.Customer.V1.Value.Event;
using Identity = Billing.Shared.Database.Entities.Identity;
using Organization = Billing.Shared.Models.Organization;
using OrganizationMember = Billing.Shared.Database.Entities.OrganizationMember;
using OrganizationOffering = Billing.Shared.Database.Entities.OrganizationOffering;

namespace Billing.Processors.Mappers;

public interface IMapper
{
    Customer MapTo(Event src);
    Organization MapTo(Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Event src);
    Shared.Database.Entities.Customer MergeTo(Customer src, Shared.Database.Entities.Customer dest, ICollection<Identity> identities);
    Identity MapTo(Shared.Models.Identity src, Shared.Database.Entities.Customer? customer);
    Identity MergeTo(Shared.Models.Identity src, Identity dest, Shared.Database.Entities.Customer? customer);
    Shared.Database.Entities.Organization MergeTo(Organization src, Shared.Database.Entities.Organization dest);

    OrganizationMember MapTo(
        Shared.Models.OrganizationMember src,
        Shared.Database.Entities.Organization organization,
        Shared.Database.Entities.Customer customer);

    OrganizationMember MergeTo(
        Shared.Models.OrganizationMember src,
        OrganizationMember dest,
        Shared.Database.Entities.Organization organization,
        Shared.Database.Entities.Customer customer);

    OrganizationOffering MapTo(Shared.Models.OrganizationOffering src, Shared.Database.Entities.Organization organization);

    OrganizationOffering MergeTo(
        Shared.Models.OrganizationOffering src,
        OrganizationOffering dest,
        Shared.Database.Entities.Organization organization);

    Shared.Models.OrganizationOffering MapTo(OrganizationOffering src);
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
            Identities = customer.Identities.Select(item => new Shared.Models.Identity { Id = item.Id }).ToList()
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

        organization.OrganizationOfferings =
        [
            new Shared.Models.OrganizationOffering
            {
                Id = organizationAfterState.Offering.Id,
                EventRaisedAt = eventRaisedAt,
                Code = organizationAfterState.Offering.Code.ToOfferingCode(),
                Start = organizationAfterState.Offering.Start.ToDateTimeOffset(),
                End = organizationAfterState.Offering.End.ToDateTimeOffset(),
                UnitPrice = organizationAfterState.Offering.UnitPrice,
                TotalNumberOfActiveCustomers = organizationAfterState.Offering.ActiveCustomerIds.Count,
                TotalCost = organizationAfterState.Offering.ActiveCustomerIds.Count * organizationAfterState.Offering.UnitPrice,
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

    public Shared.Database.Entities.Customer MergeTo(Customer src, Shared.Database.Entities.Customer dest, ICollection<Identity> identities)
    {
        dest.Id = src.Id;
        dest.Title = src.Title;
        dest.Name = src.Name;
        dest.GivenName = src.GivenName;
        dest.MiddleName = src.MiddleName;
        dest.FamilyName = src.FamilyName;
        dest.BillingContactCompanyName = src.BillingContactCompanyName;
        dest.BillingContactEmail = src.BillingContactEmail;
        dest.BillingContactAddressLine1 = src.BillingContactAddressLine1;
        dest.BillingContactAddressLine2 = src.BillingContactAddressLine2;
        dest.BillingContactSuburb = src.BillingContactSuburb;
        dest.BillingContactCity = src.BillingContactCity;
        dest.BillingContactProvince = src.BillingContactProvince;
        dest.BillingContactZipcode = src.BillingContactZipcode;
        dest.BillingContactCountry = src.BillingContactCountry;
        dest.Identities = identities;
        return dest;
    }

    public Identity MapTo(Shared.Models.Identity src, Shared.Database.Entities.Customer? customer) =>
        MergeTo(src, new Identity(), customer);

    public Identity MergeTo(Shared.Models.Identity src, Identity dest, Shared.Database.Entities.Customer? customer)
    {
        dest.Id = src.Id;
        if (customer is not null)
        {
            dest.Customer = customer;
        }

        return dest;
    }

    public Shared.Database.Entities.Organization MergeTo(Organization src, Shared.Database.Entities.Organization dest)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Name = src.Name;
        dest.Type = src.Type.ToOrganizationType();
        dest.MemberVisibilityPolicy = src.MemberVisibilityPolicy.ToOrganizationMemberVisibilityPolicy();
        return dest;
    }

    public OrganizationMember MapTo(
        Shared.Models.OrganizationMember src,
        Shared.Database.Entities.Organization organization,
        Shared.Database.Entities.Customer customer) =>
        MergeTo(src, new OrganizationMember(), organization, customer);

    public OrganizationMember MergeTo(
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

    public OrganizationOffering MapTo(Shared.Models.OrganizationOffering src, Shared.Database.Entities.Organization organization) =>
        MergeTo(src, new OrganizationOffering(), organization);

    public OrganizationOffering MergeTo(
        Shared.Models.OrganizationOffering src,
        OrganizationOffering dest,
        Shared.Database.Entities.Organization organization)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Code = src.Code;
        dest.Start = src.Start;
        dest.End = src.End;
        dest.UnitPrice = src.UnitPrice;
        dest.TotalNumberOfActiveCustomers = src.TotalNumberOfActiveCustomers;
        dest.TotalCost = src.TotalCost;
        dest.Organization = organization;
        return dest;
    }

    public Shared.Models.OrganizationOffering MapTo(OrganizationOffering src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            DeletedAt = src.DeletedAt,
            EventRaisedAt = src.EventRaisedAt,
            Code = src.Code,
            Start = src.Start,
            End = src.End,
            UnitPrice = src.UnitPrice,
            TotalNumberOfActiveCustomers = src.TotalNumberOfActiveCustomers,
            TotalCost = src.TotalCost,
            InvoiceDate = src.InvoiceDate,
            Organization = MapTo(src.Organization)
        };

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

    private static Customer MapTo(Shared.Database.Entities.Customer src) =>
        new()
        {
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Id = src.Id,
            Title = src.Title,
            Name = src.Name,
            GivenName = src.GivenName,
            MiddleName = src.MiddleName,
            FamilyName = src.FamilyName,
            BillingContactCompanyName = src.BillingContactCompanyName,
            BillingContactEmail = src.BillingContactEmail,
            BillingContactAddressLine1 = src.BillingContactAddressLine1,
            BillingContactAddressLine2 = src.BillingContactAddressLine2,
            BillingContactSuburb = src.BillingContactSuburb,
            BillingContactCity = src.BillingContactCity,
            BillingContactProvince = src.BillingContactProvince,
            BillingContactZipcode = src.BillingContactZipcode,
            BillingContactCountry = src.BillingContactCountry,
            Identities = MapTo(src.Identities).ToList()
        };

    private static Organization MapTo(Shared.Database.Entities.Organization src)
    {
        var organization = new Organization
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            EventRaisedAt = src.EventRaisedAt,
            Name = src.Name,
            BillingContactEmail = src.BillingContactEmail,
            BillingContactAddressLine1 = src.BillingContactAddressLine1,
            BillingContactAddressLine2 = src.BillingContactAddressLine2,
            BillingContactSuburb = src.BillingContactSuburb,
            BillingContactCity = src.BillingContactCity,
            BillingContactProvince = src.BillingContactProvince,
            BillingContactZipcode = src.BillingContactZipcode,
            BillingContactCountry = src.BillingContactCountry,
            Type = src.Type.ToOrganizationType(),
            MemberVisibilityPolicy = src.MemberVisibilityPolicy.ToOrganizationMemberVisibilityPolicy()
        };

        organization.OrganizationMembers = MapTo(src.OrganizationMembers, organization).ToList();
        organization.OrganizationOfferings = MapTo(src.OrganizationOfferings, organization).ToList();

        return organization;
    }

    private static IEnumerable<Shared.Models.OrganizationMember> MapTo(IEnumerable<OrganizationMember> src, Organization organization) =>
        src.Select(item => MapTo(item, organization));

    private static Shared.Models.OrganizationMember MapTo(OrganizationMember src, Organization organization) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            EventRaisedAt = src.EventRaisedAt,
            Customer = MapTo(src.Customer),
            Organization = organization
        };

    private static IEnumerable<Shared.Models.OrganizationOffering> MapTo(IEnumerable<OrganizationOffering> src, Organization organization)
        => src.Select(item => MapTo(item, organization));

    private static Shared.Models.OrganizationOffering MapTo(OrganizationOffering src, Organization organization) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            DeletedAt = src.DeletedAt,
            EventRaisedAt = src.EventRaisedAt,
            Organization = organization,
            Code = src.Code,
            Start = src.Start,
            End = src.End,
            UnitPrice = src.UnitPrice,
            TotalNumberOfActiveCustomers = src.TotalNumberOfActiveCustomers,
            TotalCost = src.TotalCost,
            InvoiceDate = src.InvoiceDate
        };

    private static IEnumerable<Shared.Models.Identity> MapTo(IEnumerable<Identity?>? src) =>
        (src is null ? [] : src.Where(item => item is not null).Select(MapTo))!;

    private static Shared.Models.Identity? MapTo(Identity? src) =>
        src is null
            ? null
            : new Shared.Models.Identity { Id = src.Id, CreatedAt = src.CreatedAt, ModifiedAt = src.ModifiedAt };
}
