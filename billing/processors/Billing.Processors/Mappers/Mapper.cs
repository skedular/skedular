using Api.Shared.Clients.Events.UnityHub.Organization.V1.Value;
using Api.Shared.Models;
using Api.Shared.Services.Offering;
using Customer = Billing.Shared.Models.Customer;
using Event = Api.Shared.Clients.Events.UnityHub.Customer.V1.Value.Event;
using Identity = Billing.Shared.Database.Entities.Identity;
using Organization = Billing.Shared.Models.Organization;
using OrganizationMember = Billing.Shared.Database.Entities.OrganizationMember;
using OrganizationOffering = Billing.Shared.Database.Entities.OrganizationOffering;

namespace Billing.Processors.Mappers;

public interface IMapper
{
    Customer MapTo(Event src);
    Organization MapTo(Api.Shared.Clients.Events.UnityHub.Organization.V1.Value.Event src);
    Shared.Database.Entities.Customer MapToEntity(Customer src, ICollection<Identity> identities);

    Shared.Database.Entities.Customer MergeToEntity(
        Customer src,
        Shared.Database.Entities.Customer dest,
        ICollection<Identity> identities);

    Identity MapToEntity(Shared.Models.Identity src, Shared.Database.Entities.Customer? customer);
    Identity MergeToEntity(Shared.Models.Identity src, Identity dest, Shared.Database.Entities.Customer? customer);

    IEnumerable<Identity> MapToEntity(IEnumerable<Shared.Models.Identity> src,
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

    OrganizationOffering MapToEntity(
        Shared.Models.OrganizationOffering src,
        Shared.Database.Entities.Organization organization);

    OrganizationOffering MergeToEntity(
        Shared.Models.OrganizationOffering src,
        OrganizationOffering dest,
        Shared.Database.Entities.Organization organization);

    Shared.Models.OrganizationOffering MapTo(OrganizationOffering src);
}

public class Mapper : IMapper
{
    public Customer MapTo(Event src)
    {
        var customer = src.Data.AfterState;
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

    public Organization MapTo(Api.Shared.Clients.Events.UnityHub.Organization.V1.Value.Event src)
    {
        var organizationAfterState = src.Data.OrganizationAfterState;
        var deletedAt = organizationAfterState.DeletedAt?.ToDateTimeOffset();
        var eventRaisedAt = src.Metadata.Time?.ToDateTimeOffset() ?? DateTimeOffset.MinValue;

        var organization = new Organization
        {
            Id = organizationAfterState.Id,
            DeletedAt = deletedAt,
            EventRaisedAt = eventRaisedAt,
            Name = organizationAfterState.Name
        };

        organization.OrganizationMembers = organizationAfterState.Members.Select(item =>
        {
            return new Shared.Models.OrganizationMember
            {
                Id = item.Id,
                MembershipType = item.MembershipType switch
                {
                    MembershipType.Owner => OrganizationMembershipType.Owner,
                    MembershipType.Administrator => OrganizationMembershipType.Administrator,
                    MembershipType.Member => OrganizationMembershipType.Member,
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
                TotalCost = organizationAfterState.Offering.ActiveCustomerIds.Count *
                            organizationAfterState.Offering.UnitPrice,
                Organization = organization
            }
        ];

        return organization;
    }

    public Shared.Database.Entities.Customer MapToEntity(
        Customer src,
        ICollection<Identity> identities) =>
        MergeToEntity(src, new Shared.Database.Entities.Customer(), identities);

    public Shared.Database.Entities.Customer MergeToEntity(
        Customer src,
        Shared.Database.Entities.Customer dest,
        ICollection<Identity> identities)
    {
        dest.Id = src.Id;
        dest.Identities = identities;
        return dest;
    }

    public Identity MapToEntity(Shared.Models.Identity src, Shared.Database.Entities.Customer? customer) =>
        MergeToEntity(src, new Identity(), customer);

    public Identity MergeToEntity(Shared.Models.Identity src, Identity dest,
        Shared.Database.Entities.Customer? customer)
    {
        dest.Id = src.Id;
        if (customer is not null)
        {
            dest.Customer = customer;
        }
        return dest;
    }

    public Shared.Database.Entities.Organization MapToEntity(Organization src) =>
        MergeToEntity(src, new Shared.Database.Entities.Organization());

    public Shared.Database.Entities.Organization MergeToEntity(Organization src,
        Shared.Database.Entities.Organization dest)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Name = src.Name;
        return dest;
    }

    public OrganizationMember MapToEntity(Shared.Models.OrganizationMember src,
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

    public OrganizationOffering MapToEntity(Shared.Models.OrganizationOffering src,
        Shared.Database.Entities.Organization organization) =>
        MergeToEntity(src, new OrganizationOffering(), organization);

    public OrganizationOffering MergeToEntity(Shared.Models.OrganizationOffering src, OrganizationOffering dest,
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

    public IEnumerable<Identity>
        MapToEntity(IEnumerable<Shared.Models.Identity> src, Shared.Database.Entities.Customer? customer) =>
        src.Select(identity => MapToEntity(identity, customer));

    private static Customer MapTo(Shared.Database.Entities.Customer src) =>
        new()
        {
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Id = src.Id,
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
            BillingContactCountry = src.BillingContactCountry
        };

        organization.OrganizationMembers = MapTo(src.OrganizationMembers, organization).ToList();
        organization.OrganizationOfferings = MapTo(src.OrganizationOfferings, organization).ToList();

        return organization;
    }

    private static IEnumerable<Shared.Models.OrganizationMember> MapTo(
        IEnumerable<OrganizationMember> src,
        Organization organization) => src.Select(item => MapTo(item, organization));

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

    private static IEnumerable<Shared.Models.OrganizationOffering> MapTo(
        IEnumerable<OrganizationOffering> src,
        Organization organization)
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
