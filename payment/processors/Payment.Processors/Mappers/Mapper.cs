using Api.Shared.Clients.Events.UnityHub.Organization.V1.Value;
using Api.Shared.Models;
using Api.Shared.Services.Offering;
using Payment.Shared.Models;
using Event = Api.Shared.Clients.Events.UnityHub.Customer.V1.Value.Event;
using Identity = Payment.Shared.Models.Identity;
using Organization = Payment.Shared.Models.Organization;
using OrganizationMember = Payment.Shared.Database.Entities.OrganizationMember;
using OrganizationOffering = Payment.Shared.Database.Entities.OrganizationOffering;

namespace Payment.Processors.Mappers;

public interface IMapper
{
    Customer MapTo(Event src);
    Organization MapTo(Api.Shared.Clients.Events.UnityHub.Organization.V1.Value.Event src);

    Shared.Database.Entities.Customer MapToEntity(Customer src,
        ICollection<Shared.Database.Entities.Identity> identities);

    Shared.Database.Entities.Customer MergeToEntity(
        Customer src,
        Shared.Database.Entities.Customer dest,
        ICollection<Shared.Database.Entities.Identity> identities);

    Shared.Database.Entities.Identity MapToEntity(Identity src, Shared.Database.Entities.Customer? customer);

    Shared.Database.Entities.Identity MergeToEntity(Identity src, Shared.Database.Entities.Identity dest,
        Shared.Database.Entities.Customer? customer);

    IEnumerable<Shared.Database.Entities.Identity> MapToEntity(IEnumerable<Identity> src,
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
            Identities = customer.Identities.Select(item => new Identity { Id = item.Id }).ToList()
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
                Organization = organization
            }
        ];

        return organization;
    }

    public Shared.Database.Entities.Customer MapToEntity(
        Customer src,
        ICollection<Shared.Database.Entities.Identity> identities) =>
        MergeToEntity(src, new Shared.Database.Entities.Customer(), identities);

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

    public Shared.Database.Entities.Identity MergeToEntity(Identity src, Shared.Database.Entities.Identity dest,
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
        dest.Organization = organization;
        return dest;
    }

    public IEnumerable<Shared.Database.Entities.Identity>
        MapToEntity(IEnumerable<Identity> src, Shared.Database.Entities.Customer? customer) =>
        src.Select(identity => MapToEntity(identity, customer));
}
