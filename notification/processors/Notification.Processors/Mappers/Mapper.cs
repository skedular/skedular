using Enterprise.Shared;
using Enterprise.Shared.Random;
using Notification.Shared.Models;
using Customer = Notification.Shared.Database.Entities.Customer;
using Event = Api.Shared.Clients.Events.Skedular.Customer.V1.Value.Event;
using Identity = Notification.Shared.Database.Entities.Identity;
using Location = Notification.Shared.Models.Location;
using Organization = Notification.Shared.Models.Organization;
using Team = Notification.Shared.Models.Team;

namespace Notification.Processors.Mappers;

public interface IMapper
{
    Shared.Models.Customer MapTo(Event src);
    Organization MapTo(Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Event src);
    Location MapTo(Api.Shared.Clients.Events.Skedular.Location.V1.Value.Event src);
    Team MapTo(Api.Shared.Clients.Events.Skedular.Team.V1.Value.Event src);
    Shared.Database.Entities.Organization MapToEntity(Organization src);
    Shared.Database.Entities.Organization MergeToEntity(Organization src, Shared.Database.Entities.Organization dest);
    Shared.Database.Entities.Location MapToEntity(Location src);
    Shared.Database.Entities.Location MergeToEntity(Location src, Shared.Database.Entities.Location dest);
    Shared.Database.Entities.Team MapToEntity(Team src);
    Shared.Database.Entities.Team MergeToEntity(Team src, Shared.Database.Entities.Team dest);
    IEnumerable<Identity> MapToEntity(IEnumerable<Shared.Models.Identity> src, Customer? customer);
    Customer MapToEntity(Shared.Models.Customer src, ICollection<Identity> identities);
    Customer MergeToEntity(Shared.Models.Customer src, Customer dest, ICollection<Identity> identities);
    Identity MapToEntity(Shared.Models.Identity src, Customer? customer);
    Identity MergeToEntity(Shared.Models.Identity src, Identity dest, Customer? customer);
    Shared.Models.Notification MapInvitationToJoinOrganizationToNotification(Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Event src);
    Shared.Models.Notification MapInvitationToJoinTeamToNotification(Api.Shared.Clients.Events.Skedular.Team.V1.Value.Event src);

    Shared.Database.Entities.Notification MapToEntity(
        Shared.Models.Notification src,
        Customer? invitedBy,
        Customer? invitee,
        Shared.Database.Entities.Organization? organization,
        Shared.Database.Entities.Location? location,
        Shared.Database.Entities.Team? team);

    Shared.Database.Entities.Notification MergeToEntity(
        Shared.Models.Notification src,
        Shared.Database.Entities.Notification dest,
        Customer? invitedBy,
        Customer? invitee,
        Shared.Database.Entities.Organization? organization,
        Shared.Database.Entities.Location? location,
        Shared.Database.Entities.Team? team);
}

public class Mapper(IRandomHelper randomHelper) : IMapper
{
    public Shared.Models.Customer MapTo(Event src)
    {
        var customer = src.Data.Customer;
        var deletedAt = customer.DeletedAt?.ToDateTimeOffset();
        var eventRaisedAt = src.Metadata.Time?.ToDateTimeOffset() ?? DateTimeOffset.MinValue;

        return new Shared.Models.Customer
        {
            Id = customer.Id,
            DeletedAt = deletedAt,
            EventRaisedAt = eventRaisedAt,
            Name = customer.Name,
            GivenName = customer.GivenName,
            MiddleName = customer.MiddleName,
            FamilyName = customer.FamilyName,
            PhotoUrl = customer.PhotoUrl,
            PhotoUrl24 = customer.PhotoUrl24,
            PhotoUrl32 = customer.PhotoUrl32,
            PhotoUrl48 = customer.PhotoUrl48,
            PhotoUrl72 = customer.PhotoUrl72,
            PhotoUrl192 = customer.PhotoUrl192,
            PhotoUrl512 = customer.PhotoUrl512,
            Identities = customer.Identities.Select(item =>
                    new Shared.Models.Identity { Id = item.Id, Email = item.Email.ToSafeString(), EmailVerified = item.EmailVerified })
                .ToList()
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
            LogoUrl = organizationAfterState.LogoUrl
        };

        return organization;
    }

    public Location MapTo(Api.Shared.Clients.Events.Skedular.Location.V1.Value.Event src)
    {
        var locationAfterState = src.Data.Location;
        var deletedAt = locationAfterState.DeletedAt?.ToDateTimeOffset();
        var eventRaisedAt = src.Metadata.Time?.ToDateTimeOffset() ?? DateTimeOffset.MinValue;

        var location = new Location
        {
            Id = locationAfterState.Id,
            DeletedAt = deletedAt,
            EventRaisedAt = eventRaisedAt,
            Name = locationAfterState.Name,
            Organization = new Organization { Id = locationAfterState.OrganizationId }
        };

        return location;
    }

    public Team MapTo(Api.Shared.Clients.Events.Skedular.Team.V1.Value.Event src)
    {
        var teamAfterState = src.Data.Team;
        var deletedAt = teamAfterState.DeletedAt?.ToDateTimeOffset();
        var eventRaisedAt = src.Metadata.Time?.ToDateTimeOffset() ?? DateTimeOffset.MinValue;

        var team = new Team
        {
            Id = teamAfterState.Id,
            DeletedAt = deletedAt,
            EventRaisedAt = eventRaisedAt,
            Name = teamAfterState.Name,
            Organization = new Organization { Id = teamAfterState.OrganizationId }
        };

        return team;
    }

    public Shared.Database.Entities.Organization MapToEntity(Organization src) =>
        MergeToEntity(src, new Shared.Database.Entities.Organization());

    public Shared.Database.Entities.Organization MergeToEntity(
        Organization src,
        Shared.Database.Entities.Organization dest)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Name = src.Name;
        dest.LogoUrl = src.LogoUrl;
        return dest;
    }

    public IEnumerable<Identity>
        MapToEntity(IEnumerable<Shared.Models.Identity> src, Customer? customer) =>
        src.Select(identity => MapToEntity(identity, customer));

    public Customer MapToEntity(
        Shared.Models.Customer src,
        ICollection<Identity> identities) =>
        MergeToEntity(src, new Customer(), identities);

    public Customer MergeToEntity(
        Shared.Models.Customer src,
        Customer dest,
        ICollection<Identity> identities)
    {
        dest.Id = src.Id;
        dest.Name = src.Name;
        dest.GivenName = src.GivenName;
        dest.MiddleName = src.MiddleName;
        dest.FamilyName = src.FamilyName;
        dest.PhotoUrl = src.PhotoUrl;
        dest.PhotoUrl24 = src.PhotoUrl24;
        dest.PhotoUrl32 = src.PhotoUrl32;
        dest.PhotoUrl48 = src.PhotoUrl48;
        dest.PhotoUrl72 = src.PhotoUrl72;
        dest.PhotoUrl192 = src.PhotoUrl192;
        dest.PhotoUrl512 = src.PhotoUrl512;
        dest.Identities = identities;
        return dest;
    }

    public Identity MapToEntity(Shared.Models.Identity src, Customer? customer) =>
        MergeToEntity(src, new Identity(), customer);

    public Identity MergeToEntity(
        Shared.Models.Identity src,
        Identity dest,
        Customer? customer)
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

    public Shared.Models.Notification MapInvitationToJoinOrganizationToNotification(
        Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Event src)
    {
        var notification = src.Data.InvitationToJoinOrganization;
        var deletedAt = notification.DeletedAt?.ToDateTimeOffset();
        var eventRaisedAt = src.Metadata.Time?.ToDateTimeOffset() ?? DateTimeOffset.MinValue;

        return new Shared.Models.Notification
        {
            Id = randomHelper.Generate(),
            DeletedAt = deletedAt,
            EventRaisedAt = eventRaisedAt,
            SourceId = notification.Id,
            Type = NotificationTypeConstants.InvitationToJoinOrganization,
            InvitedBy = new Shared.Models.Customer { Id = notification.InvitedById },
            Invitee = new Shared.Models.Customer { Id = notification.InviteeId },
            Organization = new Organization { Id = notification.OrganizationId }
        };
    }

    public Shared.Models.Notification MapInvitationToJoinTeamToNotification(
        Api.Shared.Clients.Events.Skedular.Team.V1.Value.Event src)
    {
        var notification = src.Data.InvitationToJoinTeam;
        var deletedAt = notification.DeletedAt?.ToDateTimeOffset();
        var eventRaisedAt = src.Metadata.Time?.ToDateTimeOffset() ?? DateTimeOffset.MinValue;

        return new Shared.Models.Notification
        {
            Id = randomHelper.Generate(),
            DeletedAt = deletedAt,
            EventRaisedAt = eventRaisedAt,
            SourceId = notification.Id,
            Type = NotificationTypeConstants.InvitationToJoinTeam,
            InvitedBy = new Shared.Models.Customer { Id = notification.InvitedById },
            Invitee = new Shared.Models.Customer { Id = notification.InviteeId },
            Team = new Team { Id = notification.TeamId }
        };
    }

    public Shared.Database.Entities.Notification MapToEntity(
        Shared.Models.Notification src,
        Customer? invitedBy,
        Customer? invitee,
        Shared.Database.Entities.Organization? organization,
        Shared.Database.Entities.Location? location,
        Shared.Database.Entities.Team? team) =>
        MergeToEntity(
            src,
            new Shared.Database.Entities.Notification(),
            invitedBy,
            invitee,
            organization,
            location,
            team);

    public Shared.Database.Entities.Notification MergeToEntity(
        Shared.Models.Notification src,
        Shared.Database.Entities.Notification dest,
        Customer? invitedBy,
        Customer? invitee,
        Shared.Database.Entities.Organization? organization,
        Shared.Database.Entities.Location? location,
        Shared.Database.Entities.Team? team)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.SourceId = src.SourceId;
        dest.Type = src.Type;
        dest.InvitedBy = invitedBy;
        dest.Invitee = invitee;
        dest.Organization = organization;
        dest.Location = location;
        dest.Team = team;
        return dest;
    }

    public Shared.Database.Entities.Location MapToEntity(Location src) =>
        MergeToEntity(src, new Shared.Database.Entities.Location());

    public Shared.Database.Entities.Location MergeToEntity(Location src, Shared.Database.Entities.Location dest)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Name = src.Name;
        return dest;
    }

    public Shared.Database.Entities.Team MapToEntity(Team src) =>
        MergeToEntity(src, new Shared.Database.Entities.Team());

    public Shared.Database.Entities.Team MergeToEntity(
        Team src,
        Shared.Database.Entities.Team dest)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Name = src.Name;
        return dest;
    }
}
