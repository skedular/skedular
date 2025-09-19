using Api.Shared.Services.Grpc.Skedular.Organization.V1;
using Api.Shared.Services.Models;
using Enterprise.Shared;
using Google.Protobuf.WellKnownTypes;
using Slack.Shared.Models;
using SlackNet;
using SlackNet.WebApi;
using Booking = Slack.Shared.Models.Booking;
using Constants = Slack.Shared.Constants.Constants;
using Customer = Slack.Shared.Models.Customer;
using Location = Slack.Shared.Models.Location;
using Organization = Slack.Shared.Database.Entities.Organization;
using OrganizationMember = Slack.Shared.Models.OrganizationMember;
using OrganizationPermissions = Api.Shared.Services.Grpc.Skedular.Booking.V1.OrganizationPermissions;
using Resource = Slack.Shared.Models.Resource;
using ResourceType = Slack.Shared.Models.ResourceType;
using Team = Slack.Shared.Models.Team;
using TeamPermissions = Api.Shared.Services.Grpc.Skedular.Booking.V1.TeamPermissions;
using UpdateInput = Api.Shared.Services.Grpc.Skedular.Booking.V1.UpdateInput;
using Workspace = Slack.Shared.Database.Entities.Workspace;
using WorkspaceChannel = Slack.Shared.Database.Entities.WorkspaceChannel;
using WorkspaceMember = Slack.Shared.Database.Entities.WorkspaceMember;

namespace Slack.Api.Mappers;

public interface IMapper
{
    Workspace MapTo(OauthV2AccessResponse src, Organization organization);
    Workspace MergeTo(OauthV2AccessResponse src, Workspace dest, Organization organization);
    WorkspaceMember MapToEntity(User src, Workspace workspace);
    Shared.Models.Workspace MapTo(Workspace src);
    Shared.Models.WorkspaceMember MapTo(WorkspaceMember src, Shared.Models.Workspace workspace);
    Booking MapTo(global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Booking src);
    UpdateInput MapTo(Booking src);
    OrganizationBookingPermissions MapTo(OrganizationPermissions src);
    TeamBookingPermissions MapTo(TeamPermissions src);
    WorkspaceChannel MapTo(Conversation src, Workspace workspace);
    Shared.Models.WorkspaceChannel? MapTo(WorkspaceChannel? src);
    Resource MapTo(global::Api.Shared.Services.Grpc.Skedular.Location.V1.Resource src);
    OrganizationCustomTag MapTo(CustomTag src);
    OrganizationZone MapTo(Zone src);
}

public class Mapper : IMapper
{
    public Workspace MapTo(OauthV2AccessResponse src, Organization organization) => MergeTo(src, new Workspace(), organization);

    public Workspace MergeTo(OauthV2AccessResponse src, Workspace dest, Organization organization)
    {
        dest.Id = src.Team!.Id;
        dest.Name = (string.IsNullOrWhiteSpace(src.Team?.Name) ? string.Empty : src.Team.Name).Truncate(Constants.MaxSlackWorkspaceNameLength);
        dest.BotUserId = src.BotUserId;
        dest.BotUserScope = src.Scope.Truncate(Constants.MaxSlackScopeLength);
        dest.BotUserAccessToken = src.AccessToken.Truncate(Constants.MaxSlackTokenLength);
        dest.BotRefreshToken = src.RefreshToken.ToSafeString().Truncate(Constants.MaxSlackTokenLength);
        dest.AuthedUserId = src.AuthedUser.Id;
        dest.AuthedUserScope = src.AuthedUser.Scope.Truncate(Constants.MaxSlackScopeLength);
        dest.AuthedUserAccessToken = src.AuthedUser.AccessToken.Truncate(Constants.MaxSlackTokenLength);
        dest.AuthedRefreshToken =
            (src.AuthedUser is null ? string.Empty : src.AuthedUser.RefreshToken.ToSafeString()).Truncate(Constants.MaxSlackTokenLength);
        dest.Organization = organization;
        return dest;
    }

    public WorkspaceMember MapToEntity(User src, Workspace workspace) => MergeToEntity(src, new WorkspaceMember(), workspace);

    public UpdateInput MapTo(Booking src)
    {
        var updateInput = new UpdateInput
        {
            Id = src.Id, From = src.From.ToTimestamp(), Until = src.Until.ToTimestamp(), Notes = src.Notes.ToSafeString()
        };

        updateInput.CustomerIds.AddRange(src.InvolvedCustomers.Select(item => item.Id));
        updateInput.OrganizationIds.AddRange(src.InvolvedOrganizations.Select(item => item.Id));
        updateInput.TeamIds.AddRange(src.InvolvedTeams.Select(item => item.Id));
        updateInput.ResourceIds.AddRange(src.Resources.Select(item => item.Id));

        return updateInput;
    }

    OrganizationBookingPermissions IMapper.MapTo(
        OrganizationPermissions src) =>
        new()
        {
            CanViewBookings = src.CanViewBookings,
            CanAddBooking = src.CanAddBooking,
            CanUpdateBooking = src.CanUpdateBooking,
            CanDeleteBooking = src.CanDeleteBooking
        };

    public TeamBookingPermissions MapTo(TeamPermissions src) =>
        new()
        {
            CanViewBookings = src.CanViewBookings,
            CanAddBooking = src.CanAddBooking,
            CanUpdateBooking = src.CanUpdateBooking,
            CanDeleteBooking = src.CanDeleteBooking
        };

    public WorkspaceChannel MapTo(Conversation src, Workspace workspace) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString().Truncate(Constants.MaxSlackChannelNameLength),
            Topic = src.Topic.Value.ToSafeString().Truncate(Constants.MaxSlackChannelTopicLength),
            Purpose = src.Purpose.Value.ToSafeString().Truncate(Constants.MaxSlackChannelPurposeLength),
            IsPrivate = src.IsPrivate,
            IsGeneral = src.IsGeneral,
            IsGroup = src.IsGroup,
            IsShared = src.IsShared,
            IsMember = src.IsMember,
            Workspace = workspace
        };

    public Shared.Models.WorkspaceChannel? MapTo(WorkspaceChannel? src) =>
        src is null
            ? null
            : new Shared.Models.WorkspaceChannel
            {
                Id = src.Id,
                Name = src.Name,
                Topic = src.Topic,
                Purpose = src.Purpose,
                IsPrivate = src.IsPrivate,
                IsGeneral = src.IsGeneral,
                IsGroup = src.IsGroup,
                IsShared = src.IsShared,
                IsMember = src.IsMember
            };

    public Shared.Models.Workspace MapTo(Workspace src)
    {
        var workspace = new Shared.Models.Workspace
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
            Organization = MapTo(src.Organization)
        };

        workspace.WorkspaceMembers = MapTo(src.WorkspaceMembers, workspace).ToList();

        return workspace;
    }

    public Shared.Models.WorkspaceMember MapTo(WorkspaceMember src, Shared.Models.Workspace workspace) =>
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

    public Booking MapTo(global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Booking src) =>
        new()
        {
            Id = src.Id,
            From = src.From.ToDateTimeOffset(),
            Until = src.To.ToDateTimeOffset(),
            Notes = src.Notes.ToSafeString(),
            InvolvedCustomers = MapTo(src.InvolvedCustomers).ToList(),
            InvolvedOrganizations = MapTo(src.InvolvedOrganizations).ToList(),
            InvolvedLocations = MapTo(src.InvolvedLocations).ToList(),
            InvolvedTeams = MapTo(src.InvolvedTeams).ToList(),
            Resources = MapTo(src.Resources).ToList()
        };

    public Resource MapTo(global::Api.Shared.Services.Grpc.Skedular.Location.V1.Resource src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Inactive = src.Inactive,
            Color = src.Color.ToSafeString(),
            Capacity = src.Capacity,
            ResourceType = MapTo(src.ResourceType),
            RequireBookingApproval = src.RequireBookingApproval,
            OrganizationCustomTags = MapTo(src.OrganizationCustomTags).ToList(),
            OrganizationZones = MapTo(src.OrganizationZones).ToList(),
            OrganizationProductTags = MapTo(src.OrganizationProductTags).ToList()
        };

    public OrganizationCustomTag MapTo(CustomTag src) =>
        new() { Id = src.Id, Name = src.Name.ToSafeString(), Description = src.Description.ToSafeString() };

    public OrganizationZone MapTo(Zone src) =>
        new() { Id = src.Id, Name = src.Name.ToSafeString(), Description = src.Description.ToSafeString(), Color = src.Color.ToSafeString() };

    private Customer? MapTo(Shared.Database.Entities.Customer? src)
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

    private static Shared.Models.Organization MapTo(global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Organization src) =>
        new() { Id = src.Id, UniqueAlphanumericName = src.UniqueAlphanumericName.ToSafeString(), Name = src.Name.ToSafeString() };

    private static Location MapTo(global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Location src) =>
        new() { Id = src.Id, Name = src.Name.ToSafeString() };

    private static Team MapTo(global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Team src) => new() { Id = src.Id, Name = src.Name.ToSafeString() };

    private static IEnumerable<Resource> MapTo(IEnumerable<global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Resource> src) => src.Select(MapTo);

    private static Resource MapTo(global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Resource src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Color = src.Color.ToSafeString(),
            Capacity = src.Capacity,
            ResourceType = MapTo(src.ResourceType),
            OrganizationCustomTags = MapTo(src.OrganizationCustomTags).ToList(),
            OrganizationZones = MapTo(src.OrganizationZones).ToList(),
            Location = string.IsNullOrWhiteSpace(src.Location?.Id) ? null : new Location { Id = src.Id, Name = src.Name }
        };

    private static Customer MapTo(global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Customer src) =>
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

    private static IEnumerable<Identity> MapTo(IEnumerable<global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Identity> src) =>
        src.Select(MapTo);

    private static Identity MapTo(global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Identity src) =>
        new() { Id = src.Id, Email = src.Email.ToSafeString(), EmailVerified = src.EmailVerified };

    private IEnumerable<Shared.Models.WorkspaceMember> MapTo(IEnumerable<WorkspaceMember> src,
        Shared.Models.Workspace workspace) => src.Select(item => MapTo(item, workspace));

    private static WorkspaceMember MergeToEntity(User src, WorkspaceMember dest, Workspace workspace)
    {
        dest.Id = src.Id;
        dest.Email = src.Profile.Email.ToSafeString();
        dest.Designation = src.Profile.Title.ToSafeString();
        dest.Name = src.Profile.RealName.ToSafeString();
        dest.GivenName = src.Profile.FirstName.ToSafeString();
        dest.FamilyName = src.Profile.LastName.ToSafeString();
        dest.Timezone = src.Tz.ToSafeString();
        dest.IsAdmin = src.IsAdmin;
        dest.IsOwner = src.IsOwner;
        dest.IsPrimaryOwner = src.IsPrimaryOwner;
        dest.Locale = src.Locale.ToSafeString();
        dest.PhotoUrl = src.Profile.ImageOriginal;
        dest.PhotoUrl24 = src.Profile.Image24;
        dest.PhotoUrl32 = src.Profile.Image32;
        dest.PhotoUrl48 = src.Profile.Image48;
        dest.PhotoUrl72 = src.Profile.Image72;
        dest.PhotoUrl192 = src.Profile.Image192;
        dest.PhotoUrl512 = src.Profile.Image512;
        dest.Workspace = workspace;
        return dest;
    }

    private Shared.Models.Organization MapTo(Organization src)
    {
        var organization = new Shared.Models.Organization
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            DeletedAt = src.DeletedAt,
            EventRaisedAt = src.EventRaisedAt,
            UniqueAlphanumericName = src.UniqueAlphanumericName,
            Type = src.Type.ToOrganizationType(),
            SlackChannelDailyUpdateLastSentAt = src.SlackChannelDailyUpdateLastSentAt
        };

        organization.OrganizationMembers = MapTo(src.OrganizationMembers, organization).ToList();

        return organization;
    }

    private IEnumerable<OrganizationMember> MapTo(
        IEnumerable<Shared.Database.Entities.OrganizationMember> src,
        Shared.Models.Organization organization) => src.Select(item => MapTo(item, organization));

    private OrganizationMember MapTo(Shared.Database.Entities.OrganizationMember src, Shared.Models.Organization organization) =>
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

    private static IEnumerable<Identity> MapTo(IEnumerable<Shared.Database.Entities.Identity> src, Customer customer) =>
        src.Select(item => MapTo(item, customer));

    private static Identity MapTo(Shared.Database.Entities.Identity src, Customer customer) =>
        new() { Id = src.Id, CreatedAt = src.CreatedAt, ModifiedAt = src.ModifiedAt, Customer = customer };

    private static IEnumerable<OrganizationCustomTag> MapTo(
        IEnumerable<global::Api.Shared.Services.Grpc.Skedular.Location.V1.OrganizationCustomTag> src) =>
        src.Select(MapTo);

    private static OrganizationCustomTag MapTo(global::Api.Shared.Services.Grpc.Skedular.Location.V1.OrganizationCustomTag src) =>
        new() { Id = src.Id, Name = src.Name.ToSafeString() };

    private static IEnumerable<OrganizationZone> MapTo(IEnumerable<global::Api.Shared.Services.Grpc.Skedular.Location.V1.OrganizationZone> src) =>
        src.Select(MapTo);

    private static OrganizationZone MapTo(global::Api.Shared.Services.Grpc.Skedular.Location.V1.OrganizationZone src) =>
        new() { Id = src.Id, Name = src.Name.ToSafeString(), Color = src.Color.ToSafeString() };

    private static IEnumerable<OrganizationProductTag> MapTo(
        IEnumerable<global::Api.Shared.Services.Grpc.Skedular.Location.V1.OrganizationProductTag> src) =>
        src.Select(MapTo);

    private static OrganizationProductTag MapTo(global::Api.Shared.Services.Grpc.Skedular.Location.V1.OrganizationProductTag src) =>
        new() { Id = src.Id, Name = src.Name.ToSafeString() };

    private static IEnumerable<OrganizationCustomTag> MapTo(
        IEnumerable<global::Api.Shared.Services.Grpc.Skedular.Booking.V1.OrganizationCustomTag> src) =>
        src.Select(MapTo);

    private static OrganizationCustomTag MapTo(global::Api.Shared.Services.Grpc.Skedular.Booking.V1.OrganizationCustomTag src) =>
        new() { Id = src.Id, Name = src.Name.ToSafeString() };

    private static IEnumerable<OrganizationZone> MapTo(IEnumerable<global::Api.Shared.Services.Grpc.Skedular.Booking.V1.OrganizationZone> src) =>
        src.Select(MapTo);

    private static OrganizationZone MapTo(global::Api.Shared.Services.Grpc.Skedular.Booking.V1.OrganizationZone src) =>
        new() { Id = src.Id, Name = src.Name.ToSafeString(), Color = src.Color.ToSafeString() };

    private static ResourceType MapTo(global::Api.Shared.Services.Grpc.Skedular.Location.V1.ResourceType src) =>
        new() { Id = src.Id, Name = src.Name.ToSafeString(), Color = src.Color.ToSafeString(), Type = src.TagType.ToNullableOrganizationTagType() };

    private static ResourceType MapTo(global::Api.Shared.Services.Grpc.Skedular.Booking.V1.ResourceType src) =>
        new() { Id = src.Id, Name = src.Name.ToSafeString(), Color = src.Color.ToSafeString() };

    private static IEnumerable<Customer> MapTo(IEnumerable<global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Customer> src) =>
        src.Select(MapTo);

    private static IEnumerable<Shared.Models.Organization>
        MapTo(IEnumerable<global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Organization> src) =>
        src.Select(MapTo);

    private static IEnumerable<Location> MapTo(IEnumerable<global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Location> src) =>
        src.Select(MapTo);

    private static IEnumerable<Team> MapTo(IEnumerable<global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Team> src) =>
        src.Select(MapTo);
}
