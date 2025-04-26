using Api.Shared.Services.Models;
using Enterprise.Shared;
using HotChocolate.Types.Pagination;
using Notification.Api.GraphQL;
using Notification.Shared.Models;

namespace Notification.Api.Mappers;

public interface IMapper
{
    Customer? MapTo(Shared.Database.Entities.Customer? src);
    Edge<Shared.Models.Notification> MapTo(Edge<Shared.Database.Entities.Notification> src);
    NotificationEdge MapTo(Edge<Shared.Models.Notification> src);
}

public class Mapper : IMapper
{
    public Customer? MapTo(Shared.Database.Entities.Customer? src) =>
        src is null
            ? null
            : new Customer
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                EventRaisedAt = src.EventRaisedAt,
                Name = src.Name,
                GivenName = src.GivenName,
                MiddleName = src.MiddleName,
                FamilyName = src.FamilyName,
                PhotoUrl = src.PhotoUrl,
                PhotoUrl24 = src.PhotoUrl24,
                PhotoUrl32 = src.PhotoUrl32,
                PhotoUrl48 = src.PhotoUrl48,
                PhotoUrl72 = src.PhotoUrl72,
                PhotoUrl192 = src.PhotoUrl192,
                PhotoUrl512 = src.PhotoUrl512,
                Identities = MapTo(src.Identities).ToList()
            };

    public Edge<Shared.Models.Notification> MapTo(Edge<Shared.Database.Entities.Notification> src) =>
        new(MapTo(src.Node), src.Cursor);

    public NotificationEdge MapTo(Edge<Shared.Models.Notification> src) => new(MapTo(src.Node), src.Cursor);

    private GraphQL.Notification MapTo(Shared.Models.Notification src) =>
        new()
        {
            Id = src.Id,
            SourceId = src.SourceId,
            EventRaisedAt = src.EventRaisedAt,
            NotificationType = src.Type switch
            {
                NotificationTypeConstants.InvitationToJoinOrganization => NotificationType.InvitationToJoinOrganization,
                NotificationTypeConstants.InvitationToJoinLocation => NotificationType.InvitationToJoinLocation,
                NotificationTypeConstants.InvitationToJoinTeam => NotificationType.InvitationToJoinTeam,
                _ => throw new ArgumentOutOfRangeException()
            },
            InvitedBy = MapTo(src.InvitedBy),
            Invitee = MapTo(src.Invitee),
            Organization = MapTo(src.Organization),
            Location = MapTo(src.Location),
            Team = MapTo(src.Team)
        };

    private Shared.Models.Notification MapTo(Shared.Database.Entities.Notification src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            EventRaisedAt = src.EventRaisedAt,
            SourceId = src.SourceId,
            Type = src.Type,
            InvitedBy = MapTo(src.InvitedBy),
            Invitee = MapTo(src.Invitee),
            Organization = MapTo(src.Organization),
            Location = MapTo(src.Location),
            Team = MapTo(src.Team)
        };

    private static Organization? MapTo(Shared.Database.Entities.Organization? src) =>
        src is null
            ? null
            : new Organization
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                EventRaisedAt = src.EventRaisedAt,
                Name = src.Name,
                LogoUrl = src.LogoUrl,
                Type = src.Type.ToOrganizationType(),
                MemberVisibilityPolicy = src.MemberVisibilityPolicy.ToOrganizationMemberVisibilityPolicy()
            };

    private static Location? MapTo(Shared.Database.Entities.Location? src) =>
        src is null
            ? null
            : new Location
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                EventRaisedAt = src.EventRaisedAt,
                Name = src.Name
            };

    private static Team? MapTo(Shared.Database.Entities.Team? src) =>
        src is null
            ? null
            : new Team
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                EventRaisedAt = src.EventRaisedAt,
                Name = src.Name
            };

    private static CustomerDetails? MapTo(Customer? src) =>
        src is null
            ? null
            : new CustomerDetails
            {
                UniqueId = src.Id,
                Name = src.Name,
                GivenName = src.GivenName,
                MiddleName = src.MiddleName,
                FamilyName = src.FamilyName,
                PhotoUrl = src.PhotoUrl,
                PhotoUrl24 = src.PhotoUrl24,
                PhotoUrl32 = src.PhotoUrl32,
                PhotoUrl48 = src.PhotoUrl48,
                PhotoUrl72 = src.PhotoUrl72,
                PhotoUrl192 = src.PhotoUrl192,
                PhotoUrl512 = src.PhotoUrl512,
                Emails = src.Identities.Select(item => item.Email).Where(item => !string.IsNullOrWhiteSpace(item))!
            };

    private static OrganizationDetails? MapTo(Organization? src) =>
        src is null ? null : new OrganizationDetails { UniqueId = src.Id, Name = src.Name.ToSafeString(), LogoUrl = src.LogoUrl };

    private static LocationDetails? MapTo(Location? src) =>
        src is null ? null : new LocationDetails { UniqueId = src.Id, Name = src.Name.ToSafeString() };

    private static TeamDetails? MapTo(Team? src) =>
        src is null ? null : new TeamDetails { UniqueId = src.Id, Name = src.Name.ToSafeString() };

    private static IEnumerable<Identity> MapTo(IEnumerable<Shared.Database.Entities.Identity> src) => src.Select(MapTo);

    private static Identity MapTo(Shared.Database.Entities.Identity src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            EventRaisedAt = src.EventRaisedAt,
            Email = src.Email,
            EmailVerified = src.EmailVerified
        };
}
