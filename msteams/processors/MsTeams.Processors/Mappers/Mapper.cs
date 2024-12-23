using Api.Shared.Clients.Events.Skedular.Organization.V1.Value;
using Api.Shared.Models;
using Enterprise.Shared;
using Microsoft.Graph.Models;
using MsTeams.Shared.Models;
using AzureTenant = MsTeams.Shared.Database.Entities.AzureTenant;
using Event = Api.Shared.Clients.Events.Skedular.Customer.V1.Value.Event;
using Customer = MsTeams.Shared.Models.Customer;
using Identity = MsTeams.Shared.Database.Entities.Identity;
using Location = MsTeams.Shared.Models.Location;
using Organization = MsTeams.Shared.Models.Organization;
using OrganizationMember = MsTeams.Shared.Database.Entities.OrganizationMember;
using Status = Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Status;
using Team = MsTeams.Shared.Models.Team;

namespace MsTeams.Processors.Mappers;

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

    Location MapTo(Api.Shared.Clients.Events.Skedular.Location.V1.Value.Event src);
    Shared.Database.Entities.Location MapToEntity(Location src);
    Shared.Database.Entities.Location MergeToEntity(Location src, Shared.Database.Entities.Location dest);
    Team MapTo(Api.Shared.Clients.Events.Skedular.Team.V1.Value.Event src);
    Shared.Database.Entities.Team MapToEntity(Team src);
    Shared.Database.Entities.Team MergeToEntity(Team src, Shared.Database.Entities.Team dest);

    AzureTenantTeam MapTo(Microsoft.Graph.Models.Team src);
    Shared.Database.Entities.AzureTenantTeam MapTo(AzureTenantTeam src, AzureTenant azureTenant);

    Shared.Database.Entities.AzureTenantTeam MergeToEntity(
        AzureTenantTeam src,
        Shared.Database.Entities.AzureTenantTeam dest,
        AzureTenant azureTenant);

    AzureTenantTeamChannel MapTo(Channel src);

    Shared.Database.Entities.AzureTenantTeamChannel MapTo(
        AzureTenantTeamChannel src,
        Shared.Database.Entities.AzureTenantTeam azureTenantTeam);

    Shared.Database.Entities.AzureTenantTeamChannel MergeToEntity(
        AzureTenantTeamChannel src,
        Shared.Database.Entities.AzureTenantTeamChannel dest,
        Shared.Database.Entities.AzureTenantTeam azureTenantTeam);
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

        var organization = new Organization
        {
            Id = organizationAfterState.Id, DeletedAt = deletedAt, EventRaisedAt = eventRaisedAt
        };

        organization.AzureTenants = organizationAfterState.AzureTenantIds
            .Select(item =>
                new Shared.Models.AzureTenant { Id = item, Organization = organization, EventRaisedAt = eventRaisedAt })
            .ToList();

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
        dest.MembershipType = src.MembershipType;
        dest.Status = src.Status;
        dest.Organization = organization;
        dest.Customer = customer;
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

    public Shared.Database.Entities.Location MapToEntity(Location src) =>
        MergeToEntity(src, new Shared.Database.Entities.Location());

    public Shared.Database.Entities.Location MergeToEntity(Location src, Shared.Database.Entities.Location dest)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Timezone = src.Timezone;
        return dest;
    }

    public Team MapTo(Api.Shared.Clients.Events.Skedular.Team.V1.Value.Event src)
    {
        var teamAfterState = src.Data.Team;
        var deletedAt = teamAfterState.DeletedAt?.ToDateTimeOffset();
        var eventRaisedAt = src.Metadata.Time?.ToDateTimeOffset() ?? DateTimeOffset.MinValue;

        return new Team
        {
            Id = teamAfterState.Id,
            DeletedAt = deletedAt,
            EventRaisedAt = eventRaisedAt,
            Timezone = teamAfterState.Timezone.ToSafeString()
        };
    }

    public Shared.Database.Entities.Team MapToEntity(Team src) =>
        MergeToEntity(src, new Shared.Database.Entities.Team());

    public Shared.Database.Entities.Team MergeToEntity(Team src, Shared.Database.Entities.Team dest)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Timezone = src.Timezone;
        return dest;
    }

    public AzureTenantTeam MapTo(Microsoft.Graph.Models.Team src) =>
        new() { Id = src.Id!, Name = src.DisplayName!, Description = src.Description!, WebUrl = src.WebUrl! };

    public Shared.Database.Entities.AzureTenantTeam MapTo(AzureTenantTeam src, AzureTenant azureTenant) =>
        MergeToEntity(src, new Shared.Database.Entities.AzureTenantTeam(), azureTenant);

    public Shared.Database.Entities.AzureTenantTeam MergeToEntity(
        AzureTenantTeam src,
        Shared.Database.Entities.AzureTenantTeam dest,
        AzureTenant azureTenant)
    {
        dest.Id = src.Id;
        dest.Name = src.Name;
        dest.Description = src.Description;
        dest.WebUrl = src.WebUrl;
        dest.AzureTenant = azureTenant;
        return dest;
    }

    public AzureTenantTeamChannel MapTo(Channel src) =>
        new()
        {
            Id = src.Id!,
            Name = src.DisplayName!,
            Description = src.Description!,
            WebUrl = src.WebUrl!,
            Email = src.Email!
        };

    public Shared.Database.Entities.AzureTenantTeamChannel MapTo(
        AzureTenantTeamChannel src,
        Shared.Database.Entities.AzureTenantTeam azureTenantTeam) =>
        MergeToEntity(src, new Shared.Database.Entities.AzureTenantTeamChannel(), azureTenantTeam);

    public Shared.Database.Entities.AzureTenantTeamChannel MergeToEntity(
        AzureTenantTeamChannel src,
        Shared.Database.Entities.AzureTenantTeamChannel dest,
        Shared.Database.Entities.AzureTenantTeam azureTenantTeam)
    {
        dest.Id = src.Id;
        dest.Name = src.Name;
        dest.Description = src.Description;
        dest.WebUrl = src.WebUrl;
        dest.Email = src.Email;
        dest.AzureTenantTeam = azureTenantTeam;
        return dest;
    }
}
