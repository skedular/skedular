using Api.Shared.Clients.Events.UnityHub.Organization.V1.Value;
using Api.Shared.Models;
using Api.Shared.Services.Grpc.UnityHub.Customer.V1;
using Enterprise.Shared;
using Microsoft.Graph.Models;
using MsTeams.Shared.Database.Entities;
using Event = Api.Shared.Clients.Events.UnityHub.Customer.V1.Value.Event;
using Customer = MsTeams.Shared.Models.Customer;
using Identity = MsTeams.Shared.Database.Entities.Identity;
using Location = MsTeams.Shared.Models.Location;
using Organization = MsTeams.Shared.Models.Organization;
using OrganizationMember = MsTeams.Shared.Database.Entities.OrganizationMember;
using Team = MsTeams.Shared.Models.Team;

namespace MsTeams.Processors.Mappers;

public interface IMapper
{
    TenantMember MapToEntity(User src);
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

    Organization MapTo(Api.Shared.Clients.Events.UnityHub.Organization.V1.Value.Event src);
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

    Location MapTo(Api.Shared.Clients.Events.UnityHub.Location.V1.Value.Event src);
    Shared.Database.Entities.Location MapToEntity(Location src);
    Shared.Database.Entities.Location MergeToEntity(Location src, Shared.Database.Entities.Location dest);
    Team MapTo(Api.Shared.Clients.Events.UnityHub.Team.V1.Value.Event src);
    Shared.Database.Entities.Team MapToEntity(Team src);
    Shared.Database.Entities.Team MergeToEntity(Team src, Shared.Database.Entities.Team dest);
    Admin_AddIdentityInput MapTo(TenantMember src, string customerId);

    Admin_AddInput MapTo(
        TenantMember src,
        string customerId,
        Shared.Database.Entities.Organization defaultOrganization,
        ICollection<Shared.Database.Entities.Location> defaultLocations);
}

public class Mapper : IMapper
{
    public TenantMember MapToEntity(User src) =>
        new()
        {
            Id = src.Id!,
            GivenName = src.GivenName,
            Surname = src.Surname,
            JobTitle = src.JobTitle,
            Email = src.Mail,
            PrincipalName = src.UserPrincipalName,
            PreferredLanguage = src.PreferredLanguage
        };

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

    public Organization MapTo(Api.Shared.Clients.Events.UnityHub.Organization.V1.Value.Event src)
    {
        var organizationAfterState = src.Data.OrganizationAfterState;
        var deletedAt = organizationAfterState.DeletedAt?.ToDateTimeOffset();
        var eventRaisedAt = src.Metadata.Time?.ToDateTimeOffset() ?? DateTimeOffset.MinValue;

        var organization = new Organization
        {
            Id = organizationAfterState.Id, DeletedAt = deletedAt, EventRaisedAt = eventRaisedAt
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
        dest.Organization = organization;
        dest.Customer = customer;
        return dest;
    }

    public Location MapTo(Api.Shared.Clients.Events.UnityHub.Location.V1.Value.Event src)
    {
        var locationAfterState = src.Data.LocationAfterState;
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

    public Team MapTo(Api.Shared.Clients.Events.UnityHub.Team.V1.Value.Event src)
    {
        var teamAfterState = src.Data.TeamAfterState;
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

    public Admin_AddIdentityInput MapTo(TenantMember src, string customerId) =>
        new() { Id = src.Id, Email = src.Email, EmailVerified = true, CustomerId = customerId };

    public Admin_AddInput MapTo(
        TenantMember src,
        string customerId,
        Shared.Database.Entities.Organization defaultOrganization,
        ICollection<Shared.Database.Entities.Location> defaultLocations)
    {
        var input = new Admin_AddInput
        {
            Id = customerId,
            Designation = src.JobTitle.ToSafeString(),
            GivenName = src.GivenName.ToSafeString(),
            FamilyName = src.Surname.ToSafeString(),
            IsOrganizationOnboardingDone = true,
            IsLocationOnboardingDone = true,
            IsDefaultOrganizationOnboardingDone = true,
            IsDefaultLocationOnboardingDone = true,
            IsPreferredZoneOnboardingDone = false,
            IsPreferredDeskOnboardingDone = false,
            DefaultOrganization =
                new Api.Shared.Services.Grpc.UnityHub.Customer.V1.Organization { Id = defaultOrganization.Id }
        };

        input.Identities.Add(
            new Api.Shared.Services.Grpc.UnityHub.Customer.V1.Identity
            {
                Id = src.Id, Email = src.Email, EmailVerified = true
            });

        input.DefaultLocations.AddRange(defaultLocations.Select(item =>
            new Api.Shared.Services.Grpc.UnityHub.Customer.V1.Location
            {
                Id = item.Id,
                Organization =
                    new Api.Shared.Services.Grpc.UnityHub.Customer.V1.Organization { Id = defaultOrganization.Id }
            }));

        return input;
    }
}
