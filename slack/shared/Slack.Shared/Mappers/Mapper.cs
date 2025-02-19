using Enterprise.Shared;
using Slack.Shared.Models;
using Booking = Slack.Shared.Models.Booking;
using Location = Slack.Shared.Models.Location;
using Team = Slack.Shared.Models.Team;
using Organization = Slack.Shared.Models.Organization;
using Customer = Slack.Shared.Models.Customer;
using Desk = Slack.Shared.Models.Desk;
using Room = Slack.Shared.Models.Room;
using OrganizationCustomTag = Api.Shared.Services.Grpc.Skedular.Booking.V1.OrganizationCustomTag;
using Identity = Slack.Shared.Models.Identity;
using OrganizationMember = Slack.Shared.Database.Entities.OrganizationMember;
using Workspace = Slack.Shared.Database.Entities.Workspace;
using WorkspaceMember = Slack.Shared.Database.Entities.WorkspaceMember;

namespace Slack.Shared.Mappers;

public interface IMapper
{
    Customer? MapTo(Database.Entities.Customer? src);
    Booking MapTo(Api.Shared.Services.Grpc.Skedular.Booking.V1.Booking src);
    Models.Workspace MapTo(Workspace src);
}

public class Mapper : IMapper
{
    public Customer? MapTo(Database.Entities.Customer? src)
    {
        if (src is null)
        {
            return null;
        }

        var customer = new Customer
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            DeletedAt = src.DeletedAt,
            EventRaisedAt = src.EventRaisedAt,
            Timezone = src.Timezone
        };

        customer.Identities = MapTo(src.Identities, customer).ToList();

        return customer;
    }

    public Models.Workspace MapTo(Workspace src)
    {
        var workspace = new Models.Workspace
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            DeletedAt = src.DeletedAt,
            Name = src.Name,
            Domain = src.Domain,
            EmailDomain = src.EmailDomain,
            EnterpriseId = src.EnterpriseId,
            EnterpriseName = src.EnterpriseName,
            BotUserId = src.BotUserId,
            BotUserScope = src.BotUserScope,
            BotUserAccessToken = src.BotUserAccessToken,
            BotRefreshToken = src.BotRefreshToken,
            AuthedUserId = src.AuthedUserId,
            AuthedUserScope = src.AuthedUserScope,
            AuthedUserAccessToken = src.AuthedUserAccessToken,
            AuthedRefreshToken = src.AuthedRefreshToken,
            LastRefreshedAt = src.LastRefreshedAt,
            ChannelsLastRefreshedAt = src.ChannelsLastRefreshedAt,
            MembersLastRefreshedAt = src.MembersLastRefreshedAt,
            Organization = MapTo(src.Organization)
        };

        workspace.WorkspaceMembers = MapTo(src.WorkspaceMembers, workspace).ToList();

        return workspace;
    }

    public Booking MapTo(Api.Shared.Services.Grpc.Skedular.Booking.V1.Booking src) =>
        new()
        {
            Id = src.Id,
            From = src.From.ToDateTimeOffset(),
            To = src.To.ToDateTimeOffset(),
            Notes = src.Notes.ToSafeString(),
            Customer = MapTo(src.Customer),
            Organization = MapTo(src.Organization),
            Location = MapTo(src.Location),
            Desks = MapTo(src.Desks).ToList(),
            Rooms = MapTo(src.Rooms).ToList(),
            Team = MapTo(src.Team)
        };

    private Organization MapTo(Database.Entities.Organization src)
    {
        var organization = new Organization
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            DeletedAt = src.DeletedAt,
            EventRaisedAt = src.EventRaisedAt,
            SlackChannelDailyUpdateLastSentAt = src.SlackChannelDailyUpdateLastSentAt
        };

        organization.OrganizationMembers = MapTo(src.OrganizationMembers, organization).ToList();

        return organization;
    }

    private static IEnumerable<Models.WorkspaceMember> MapTo(IEnumerable<WorkspaceMember> src, Models.Workspace workspace) =>
        src.Select(item => MapTo(item, workspace));

    private static Models.WorkspaceMember MapTo(WorkspaceMember src, Models.Workspace workspace) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            DeletedAt = src.DeletedAt,
            Email = src.Email,
            Designation = src.Designation,
            Name = src.Name,
            GivenName = src.GivenName,
            FamilyName = src.FamilyName,
            Timezone = src.Timezone,
            IsAdmin = src.IsAdmin,
            IsOwner = src.IsOwner,
            IsPrimaryOwner = src.IsPrimaryOwner,
            Locale = src.Locale,
            PhotoUrl = src.PhotoUrl,
            PhotoUrl24 = src.PhotoUrl24,
            PhotoUrl32 = src.PhotoUrl32,
            PhotoUrl48 = src.PhotoUrl48,
            PhotoUrl72 = src.PhotoUrl72,
            PhotoUrl192 = src.PhotoUrl192,
            PhotoUrl512 = src.PhotoUrl512,
            LastProfileStatusUpdatedAt = src.LastProfileStatusUpdatedAt,
            AutomaticallyUpdateProfileStatus = src.AutomaticallyUpdateProfileStatus,
            Workspace = workspace
        };

    private IEnumerable<Models.OrganizationMember> MapTo(IEnumerable<OrganizationMember> src, Organization organization) =>
        src.Select(item => MapTo(item, organization));

    private Models.OrganizationMember MapTo(OrganizationMember src, Organization organization) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            DeletedAt = src.DeletedAt,
            EventRaisedAt = src.EventRaisedAt,
            Organization = organization,
            Customer = MapTo(src.Customer)!
        };

    private static IEnumerable<Identity> MapTo(IEnumerable<Database.Entities.Identity> src, Customer customer) =>
        src.Select(item => MapTo(item, customer));

    private static Identity MapTo(Database.Entities.Identity src, Customer customer) =>
        new() { Id = src.Id, CreatedAt = src.CreatedAt, ModifiedAt = src.ModifiedAt, Customer = customer };

    private static Customer MapTo(Api.Shared.Services.Grpc.Skedular.Booking.V1.Customer src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            GivenName = src.GivenName.ToSafeString(),
            MiddleName = src.MiddleName.ToSafeString(),
            FamilyName = src.FamilyName.ToSafeString(),
            PhotoUrl = src.PhotoUrl.ToSafeString(),
            PhotoUrl24 = src.PhotoUrl24.ToSafeString(),
            PhotoUrl32 = src.PhotoUrl32.ToSafeString(),
            PhotoUrl48 = src.PhotoUrl48.ToSafeString(),
            PhotoUrl72 = src.PhotoUrl72.ToSafeString(),
            PhotoUrl192 = src.PhotoUrl192.ToSafeString(),
            PhotoUrl512 = src.PhotoUrl512.ToSafeString(),
            Identities = MapTo(src.Identities).ToList()
        };

    private static IEnumerable<Identity> MapTo(IEnumerable<Api.Shared.Services.Grpc.Skedular.Booking.V1.Identity> src) => src.Select(MapTo);

    private static Identity MapTo(Api.Shared.Services.Grpc.Skedular.Booking.V1.Identity src) =>
        new() { Id = src.Id, Email = src.Email.ToSafeString(), EmailVerified = src.EmailVerified };

    private static Organization? MapTo(Api.Shared.Services.Grpc.Skedular.Booking.V1.Organization? src) =>
        string.IsNullOrWhiteSpace(src?.Id)
            ? null
            : new Organization { Id = src.Id, Name = src.Name.ToSafeString() };

    private static Location? MapTo(Api.Shared.Services.Grpc.Skedular.Booking.V1.Location? src) =>
        string.IsNullOrWhiteSpace(src?.Id)
            ? null
            : new Location { Id = src.Id, Name = src.Name.ToSafeString() };

    private static IEnumerable<Desk> MapTo(IEnumerable<Api.Shared.Services.Grpc.Skedular.Booking.V1.Desk> src) => src.Select(MapTo);

    private static Desk MapTo(Api.Shared.Services.Grpc.Skedular.Booking.V1.Desk src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Color = src.Color.ToSafeString(),
            OrganizationCustomTags = MapTo(src.OrganizationCustomTags).ToList(),
            OrganizationZones = MapTo(src.OrganizationZones).ToList(),
            Location = string.IsNullOrWhiteSpace(src.Location?.Id) ? null : new Location { Id = src.Id, Name = src.Name }
        };

    private static IEnumerable<Room> MapTo(IEnumerable<Api.Shared.Services.Grpc.Skedular.Booking.V1.Room> src) => src.Select(MapTo);

    private static Room MapTo(Api.Shared.Services.Grpc.Skedular.Booking.V1.Room src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Color = src.Color.ToSafeString(),
            OrganizationCustomTags = MapTo(src.OrganizationCustomTags).ToList(),
            OrganizationZones = MapTo(src.OrganizationZones).ToList(),
            Location = string.IsNullOrWhiteSpace(src.Location?.Id) ? null : new Location { Id = src.Id, Name = src.Name }
        };

    private static IEnumerable<Models.OrganizationCustomTag> MapTo(IEnumerable<OrganizationCustomTag> src) => src.Select(MapTo);
    private static Models.OrganizationCustomTag MapTo(OrganizationCustomTag src) => new() { Id = src.Id, Name = src.Name.ToSafeString() };

    private static IEnumerable<OrganizationZone> MapTo(IEnumerable<Api.Shared.Services.Grpc.Skedular.Booking.V1.OrganizationZone> src) =>
        src.Select(MapTo);

    private static OrganizationZone MapTo(Api.Shared.Services.Grpc.Skedular.Booking.V1.OrganizationZone src) =>
        new() { Id = src.Id, Name = src.Name.ToSafeString(), Color = src.Color.ToSafeString() };

    private static Team? MapTo(Api.Shared.Services.Grpc.Skedular.Booking.V1.Team? src) =>
        string.IsNullOrWhiteSpace(src?.Id) ? null : new Team { Id = src.Id, Name = src.Name.ToSafeString() };
}
