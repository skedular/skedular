using Api.Shared;
using Api.Shared.Clients.Events.Skedular.Organization.V1.Value;
using Api.Shared.Services.Models;
using Enterprise.Shared;
using SlackNet;
using Location = Slack.Shared.Models.Location;
using Team = Slack.Shared.Models.Team;
using Organization = Slack.Shared.Models.Organization;
using Customer = Slack.Shared.Models.Customer;
using Event = Api.Shared.Clients.Events.Skedular.Customer.V1.Value.Event;
using Identity = Slack.Shared.Models.Identity;
using Role = Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Role;
using OrganizationMember = Slack.Shared.Database.Entities.OrganizationMember;
using Workspace = Slack.Shared.Database.Entities.Workspace;
using WorkspaceChannel = Slack.Shared.Database.Entities.WorkspaceChannel;

namespace Slack.Processors.Mappers;

public interface IMapper
{
    Customer MapTo(Event src);
    Shared.Database.Entities.Customer MapToEntity(Customer src, ICollection<Shared.Database.Entities.Identity> identities);

    Shared.Database.Entities.Customer MergeToEntity(
        Customer src,
        Shared.Database.Entities.Customer dest,
        ICollection<Shared.Database.Entities.Identity> identities);

    IEnumerable<Shared.Database.Entities.Identity> MapToEntity(IEnumerable<Identity> src, Shared.Database.Entities.Customer? customer);
    Shared.Database.Entities.Identity MapToEntity(Identity src, Shared.Database.Entities.Customer? customer);

    Shared.Database.Entities.Identity MergeToEntity(
        Identity src,
        Shared.Database.Entities.Identity dest,
        Shared.Database.Entities.Customer? customer);

    Location MapTo(Api.Shared.Clients.Events.Skedular.Location.V1.Value.Event src);
    Shared.Database.Entities.Location MapToEntity(Location src);
    Shared.Database.Entities.Location MergeToEntity(Location src, Shared.Database.Entities.Location dest);
    Organization MapTo(Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Event src);
    Shared.Database.Entities.Organization MapToEntity(Organization src);
    Shared.Database.Entities.Organization MergeToEntity(Organization src, Shared.Database.Entities.Organization dest);
    Team MapTo(Api.Shared.Clients.Events.Skedular.Team.V1.Value.Event src);
    Shared.Database.Entities.Team MapToEntity(Team src);
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

    WorkspaceChannel MapToEntity(Conversation src, Workspace workspace);
    WorkspaceChannel MergeToEntity(Conversation src, WorkspaceChannel dest, Workspace workspace);
    Workspace MergeToEntity(SlackNet.Team src, Workspace dest);
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
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Timezone = src.Timezone;
        dest.Identities = identities;

        return dest;
    }

    public IEnumerable<Shared.Database.Entities.Identity> MapToEntity(
        IEnumerable<Identity> src,
        Shared.Database.Entities.Customer? customer) =>
        src.Select(identity => MapToEntity(identity, customer));

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

    public Shared.Database.Entities.Location MapToEntity(Location src) => MergeToEntity(src, new Shared.Database.Entities.Location());

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

    public Shared.Database.Entities.Organization MapToEntity(Organization src) => MergeToEntity(src, new Shared.Database.Entities.Organization());

    public Shared.Database.Entities.Organization MergeToEntity(Organization src, Shared.Database.Entities.Organization dest)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
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

    public Shared.Database.Entities.Team MapToEntity(Team src) => MergeToEntity(src, new Shared.Database.Entities.Team());

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

    public WorkspaceChannel MapToEntity(Conversation src, Workspace workspace) => MergeToEntity(src, new WorkspaceChannel(), workspace);

    public WorkspaceChannel MergeToEntity(Conversation src, WorkspaceChannel dest, Workspace workspace)
    {
        dest.Id = src.Id;
        dest.Name = src.Name.Truncate(Constants.MaxUrlLength);
        dest.Topic = src.Topic.Value;
        dest.Purpose = src.Purpose.Value;
        dest.IsPrivate = src.IsPrivate;
        dest.IsGeneral = src.IsGeneral;
        dest.IsGroup = src.IsGroup;
        dest.IsShared = src.IsShared;
        dest.IsMember = src.IsMember;
        dest.Workspace = workspace;
        return dest;
    }

    public Workspace MergeToEntity(SlackNet.Team src, Workspace dest)
    {
        dest.Name = src.Name;
        dest.Domain = src.Domain;
        dest.EmailDomain = src.EmailDomain;
        dest.EnterpriseId = src.EnterpriseId;
        dest.EnterpriseName = src.EnterpriseName;
        return dest;
    }
}
