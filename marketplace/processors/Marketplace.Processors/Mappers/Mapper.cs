using Api.Shared.Clients.Events.Skedular.Organization.V1.Value;
using Api.Shared.Services.Models;
using Event = Api.Shared.Clients.Events.Skedular.Customer.V1.Value.Event;
using Customer = Marketplace.Shared.Models.Customer;
using Identity = Marketplace.Shared.Database.Entities.Identity;
using Organization = Marketplace.Shared.Models.Organization;
using OrganizationMember = Marketplace.Shared.Database.Entities.OrganizationMember;
using Status = Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Status;

namespace Marketplace.Processors.Mappers;

public interface IMapper
{
    Customer MapTo(Event src);

    Shared.Database.Entities.Customer MapToEntity(
        Customer src,
        ICollection<Identity> identities);

    Shared.Database.Entities.Customer MergeToEntity(
        Customer src,
        Shared.Database.Entities.Customer dest,
        ICollection<Identity> identities);

    IEnumerable<Identity> MapToEntity(
        IEnumerable<Shared.Models.Identity> src,
        Shared.Database.Entities.Customer? customer);

    Identity MapToEntity(Shared.Models.Identity src, Shared.Database.Entities.Customer? customer);

    Identity MergeToEntity(
        Shared.Models.Identity src,
        Identity dest,
        Shared.Database.Entities.Customer? customer);

    Organization MapTo(Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Event src);
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
                .Select(item =>
                    new Shared.Models.Identity { Id = item.Id, Email = item.Email, EmailVerified = item.EmailVerified })
                .ToList()
        };
    }

    public Shared.Database.Entities.Customer MapToEntity(Customer src, ICollection<Identity> identities) =>
        MergeToEntity(src, new Shared.Database.Entities.Customer(), identities);

    public Shared.Database.Entities.Customer MergeToEntity(Customer src, Shared.Database.Entities.Customer dest,
        ICollection<Identity> identities)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Timezone = src.Timezone;
        dest.Identities = identities;

        return dest;
    }

    public IEnumerable<Identity> MapToEntity(
        IEnumerable<Shared.Models.Identity> src,
        Shared.Database.Entities.Customer? customer) =>
        src.Select(identity => MapToEntity(identity, customer));

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

        var organization = new Organization { Id = organizationAfterState.Id, DeletedAt = deletedAt, EventRaisedAt = eventRaisedAt };

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

        return organization;
    }

    public Shared.Database.Entities.Organization MapToEntity(Organization src) =>
        MergeToEntity(src, new Shared.Database.Entities.Organization());

    public Shared.Database.Entities.Organization MergeToEntity(Organization src,
        Shared.Database.Entities.Organization dest)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
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
}
