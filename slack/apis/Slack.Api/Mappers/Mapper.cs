using Api.Shared;
using Api.Shared.Models;
using Api.Shared.Services.Grpc.UnityHub.Customer.V1;
using Api.Shared.Services.Grpc.UnityHub.Organization.V1;
using Api.Shared.Services.Grpc.UnityHub.Slack.V1;
using Enterprise.Shared;
using Google.Protobuf.WellKnownTypes;
using Slack.Shared.Models;
using SlackNet;
using SlackNet.WebApi;
using Admin_AddInput = Api.Shared.Services.Grpc.UnityHub.Customer.V1.Admin_AddInput;
using Booking = Slack.Shared.Models.Booking;
using Customer = Slack.Shared.Models.Customer;
using Desk = Slack.Shared.Models.Desk;
using Identity = Api.Shared.Services.Grpc.UnityHub.Customer.V1.Identity;
using Location = Slack.Shared.Database.Entities.Location;
using LocationPermissions = Slack.Shared.Models.LocationPermissions;
using Member = Api.Shared.Services.Grpc.UnityHub.Organization.V1.Member;
using MembershipType = Api.Shared.Services.Grpc.UnityHub.Organization.V1.MembershipType;
using Organization = Slack.Shared.Database.Entities.Organization;
using OrganizationPermissions = Slack.Shared.Models.OrganizationPermissions;
using OrganizationTag = Slack.Shared.Models.OrganizationTag;
using Permissions = Api.Shared.Services.Grpc.UnityHub.Location.V1.Permissions;
using Team = Slack.Shared.Models.Team;
using TeamPermissions = Slack.Shared.Models.TeamPermissions;
using UpdateInput = Api.Shared.Services.Grpc.UnityHub.Booking.V1.UpdateInput;
using Workspace = Slack.Shared.Database.Entities.Workspace;
using WorkspaceChannel = Slack.Shared.Database.Entities.WorkspaceChannel;
using WorkspaceMember = Slack.Shared.Database.Entities.WorkspaceMember;

namespace Slack.Api.Mappers;

public interface IMapper
{
    Customer? MapTo(Shared.Database.Entities.Customer? src);
    Workspace MapTo(OauthV2AccessResponse src, Organization organization);
    Workspace MergeTo(OauthV2AccessResponse src, Workspace dest, Organization organization);
    WorkspaceMember MapToEntity(User src, Workspace workspace);

    Admin_AddInput MapTo(
        WorkspaceMember src,
        string customerId,
        Organization defaultOrganization,
        ICollection<Location> defaultLocations);

    Admin_AddIdentityInput MapTo(WorkspaceMember src, string customerId);
    Customer MapTo(global::Api.Shared.Services.Grpc.UnityHub.Customer.V1.Customer src);
    Shared.Models.Organization MapTo(global::Api.Shared.Services.Grpc.UnityHub.Organization.V1.Organization src);
    Shared.Models.Workspace MapTo(Workspace src);
    Shared.Models.WorkspaceMember MapTo(WorkspaceMember src, Shared.Models.Workspace workspace);
    Shared.Models.Location MapTo(global::Api.Shared.Services.Grpc.UnityHub.Location.V1.Location src);
    Booking MapTo(global::Api.Shared.Services.Grpc.UnityHub.Booking.V1.Booking src);

    OrganizationBillingPermissions MapTo(
        global::Api.Shared.Services.Grpc.UnityHub.Billing.V1.OrganizationPermissions src);

    OrganizationPermissions MapTo(global::Api.Shared.Services.Grpc.UnityHub.Organization.V1.Permissions src);
    LocationPermissions MapTo(Permissions src);
    TeamPermissions MapTo(global::Api.Shared.Services.Grpc.UnityHub.Team.V1.Permissions src);
    OrganizationMember MapTo(Member src);
    Team MapTo(global::Api.Shared.Services.Grpc.UnityHub.Team.V1.Team src);
    UpdateInput MapTo(Booking src);

    OrganizationBookingPermissions MapTo(
        global::Api.Shared.Services.Grpc.UnityHub.Booking.V1.OrganizationPermissions src);

    LocationBookingPermissions MapTo(global::Api.Shared.Services.Grpc.UnityHub.Booking.V1.LocationPermissions src);
    TeamBookingPermissions MapTo(global::Api.Shared.Services.Grpc.UnityHub.Booking.V1.TeamPermissions src);
    WorkspaceChannel MapTo(Conversation src, Workspace workspace);
    Shared.Models.WorkspaceChannel? MapTo(WorkspaceChannel? src);
    Customer MapTo(global::Api.Shared.Services.Grpc.UnityHub.Team.V1.Customer src);
    Desk MapTo(global::Api.Shared.Services.Grpc.UnityHub.Location.V1.Desk src);

    Workspace MapToEntity(Shared.Models.Workspace src, Organization organization);
    Workspace MergeToEntity(Shared.Models.Workspace src, Workspace dest, Organization organization);
    Shared.Models.Workspace MapTo(Admin_AddWorkspaceInput src);
    global::Api.Shared.Services.Grpc.UnityHub.Slack.V1.Workspace MapTo(Shared.Models.Workspace src);

    OrganizationDeskType MapTo(DeskType src);
    OrganizationZone MapTo(Zone src);
}

public class Mapper : IMapper
{
    public Customer? MapTo(Shared.Database.Entities.Customer? src)
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

    public Workspace MapTo(OauthV2AccessResponse src, Organization organization) =>
        MergeTo(src, new Workspace(), organization);

    public Workspace MergeTo(OauthV2AccessResponse src, Workspace dest, Organization organization)
    {
        dest.Id = src.Team!.Id;
        dest.Name =
            (string.IsNullOrWhiteSpace(src.Team?.Name) ? string.Empty : src.Team.Name).Truncate(Constants
                .MaxSlackWorkspaceNameLength);
        dest.BotUserId = src.BotUserId;
        dest.BotUserScope = src.Scope.Truncate(Constants.MaxSlackScopeLength);
        dest.BotUserAccessToken = src.AccessToken.Truncate(Constants.MaxTokenLength);
        dest.BotRefreshToken = src.RefreshToken.ToSafeString().Truncate(Constants.MaxTokenLength);
        dest.AuthedUserId = src.AuthedUser.Id;
        dest.AuthedUserScope = src.AuthedUser.Scope.Truncate(Constants.MaxSlackScopeLength);
        dest.AuthedUserAccessToken = src.AuthedUser.AccessToken.Truncate(Constants.MaxTokenLength);
        dest.AuthedRefreshToken =
            (src.AuthedUser is null ? string.Empty : src.AuthedUser.RefreshToken.ToSafeString()).Truncate(
                Constants.MaxTokenLength);
        dest.Organization = organization;
        return dest;
    }

    public WorkspaceMember MapToEntity(User src, Workspace workspace) =>
        MergeToEntity(src, new WorkspaceMember(), workspace);

    public Admin_AddInput MapTo(
        WorkspaceMember src,
        string customerId,
        Organization defaultOrganization,
        ICollection<Location> defaultLocations)
    {
        var input = new Admin_AddInput
        {
            Id = customerId,
            Designation = src.Designation.ToSafeString(),
            Name = src.Name.ToSafeString(),
            GivenName = src.GivenName.ToSafeString(),
            FamilyName = src.FamilyName.ToSafeString(),
            Timezone = src.Timezone.ToSafeString(),
            PhotoUrl = src.PhotoUrl.ToSafeString(),
            PhotoUrl24 = src.PhotoUrl24.ToSafeString(),
            PhotoUrl32 = src.PhotoUrl32.ToSafeString(),
            PhotoUrl48 = src.PhotoUrl48.ToSafeString(),
            PhotoUrl72 = src.PhotoUrl72.ToSafeString(),
            PhotoUrl192 = src.PhotoUrl192.ToSafeString(),
            PhotoUrl512 = src.PhotoUrl512.ToSafeString(),
            IsOrganizationOnboardingDone = false,
            IsLocationOnboardingDone = false,
            IsDefaultOrganizationOnboardingDone = false,
            IsDefaultLocationOnboardingDone = false,
            IsPreferredZoneOnboardingDone = false,
            IsPreferredDeskOnboardingDone = false,
            DefaultOrganization =
                new global::Api.Shared.Services.Grpc.UnityHub.Customer.V1.Organization { Id = defaultOrganization.Id }
        };

        input.Identities.Add(
            new Identity { Id = src.Id, Email = src.Email, EmailVerified = true });

        input.DefaultLocations.AddRange(defaultLocations.Select(item =>
            new global::Api.Shared.Services.Grpc.UnityHub.Customer.V1.Location
            {
                Id = item.Id,
                Organization =
                    new global::Api.Shared.Services.Grpc.UnityHub.Customer.V1.Organization
                    {
                        Id = defaultOrganization.Id
                    }
            }));

        return input;
    }

    public Admin_AddIdentityInput MapTo(WorkspaceMember src, string customerId) =>
        new() { Id = src.Id, Email = src.Email, EmailVerified = true, CustomerId = customerId };

    public Customer MapTo(global::Api.Shared.Services.Grpc.UnityHub.Customer.V1.Customer src)
    {
        var customer = new Customer
        {
            Id = src.Id,
            Designation = src.Designation,
            Title = src.Title,
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
            Timezone = src.Timezone,
            Locale = src.Locale,
            IsOrganizationOnboardingDone = src.IsOrganizationOnboardingDone,
            IsLocationOnboardingDone = src.IsLocationOnboardingDone,
            IsDefaultOrganizationOnboardingDone = src.IsDefaultOrganizationOnboardingDone,
            IsDefaultLocationOnboardingDone = src.IsDefaultLocationOnboardingDone,
            IsPreferredZoneOnboardingDone = src.IsPreferredZoneOnboardingDone,
            IsPreferredDeskOnboardingDone = src.IsPreferredDeskOnboardingDone
        };

        customer.Identities = src.Identities.Select(item => new Shared.Models.Identity
        {
            Id = item.Id, Email = item.Email.ToSafeString(), EmailVerified = item.EmailVerified, Customer = customer
        }).ToList();

        customer.DefaultOrganization = string.IsNullOrWhiteSpace(src.DefaultOrganization?.Id)
            ? null
            : new Shared.Models.Organization
            {
                Id = src.DefaultOrganization.Id, Name = src.DefaultOrganization.Name.ToSafeString()
            };

        customer.DefaultLocations =
            src.DefaultLocations.Select(item => new Shared.Models.Location
            {
                Id = item.Id,
                Name = item.Name.ToSafeString(),
                Organization = string.IsNullOrWhiteSpace(item.Organization?.Id)
                    ? null
                    : new Shared.Models.Organization { Id = item.Organization.Id }
            }).ToList();

        customer.DefaultTeams =
            src.DefaultTeams.Select(item => new Team
            {
                Id = item.Id,
                Name = item.Name.ToSafeString(),
                Organization = string.IsNullOrWhiteSpace(item.Organization?.Id)
                    ? null
                    : new Shared.Models.Organization { Id = item.Organization.Id }
            }).ToList();

        customer.PreferredDesks =
            src.PreferredDesks.Select(item => new Desk
            {
                Id = item.Id,
                Name = item.Name.ToSafeString(),
                Location = new Shared.Models.Location { Id = item.Location.Id }
            }).ToList();

        customer.PreferredOrganizationTags =
            src.PreferredOrganizationTags.Select(item => new OrganizationTag
            {
                Id = item.Id,
                Name = item.Name.ToSafeString(),
                Type = item.Type.ToSafeString(),
                Organization = new Shared.Models.Organization { Id = item.Organization.Id }
            }).ToList();

        return customer;
    }

    public Shared.Models.Organization
        MapTo(global::Api.Shared.Services.Grpc.UnityHub.Organization.V1.Organization src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            About = src.About.ToSafeString(),
            Website = src.Website.ToSafeString(),
            AgreedToTermsOfUse = src.AgreedToTermsOfUse,
            LogoUrl = src.LogoUrl.ToSafeString(),
            HasAttachedPaymentMethod = src.HasAttachedPaymentMethod,
            HasFutureBooking = src.HasFutureBooking
        };

    public OrganizationBillingPermissions MapTo(
        global::Api.Shared.Services.Grpc.UnityHub.Billing.V1.OrganizationPermissions src) =>
        new() { CanViewBillingInfo = src.CanViewBillingInfo, CanManageBillingInfo = src.CanManageBillingInfo };

    public OrganizationPermissions MapTo(global::Api.Shared.Services.Grpc.UnityHub.Organization.V1.Permissions src) =>
        new()
        {
            CanView = src.CanView,
            CanModify = src.CanModify,
            CanDelete = src.CanDelete,
            CanInvitePeople = src.CanInvitePeople,
            CanCancelPeopleExistingInvitations = src.CanCancelPeopleExistingInvitations,
            CanViewAnalytics = src.CanViewAnalytics
        };

    public LocationPermissions MapTo(Permissions src) => new()
    {
        CanView = src.CanView,
        CanModify = src.CanModify,
        CanDelete = src.CanDelete,
        CanInvitePeople = src.CanInvitePeople,
        CanCancelPeopleExistingInvitations = src.CanCancelPeopleExistingInvitations,
        CanViewAnalytics = src.CanViewAnalytics
    };

    public TeamPermissions MapTo(global::Api.Shared.Services.Grpc.UnityHub.Team.V1.Permissions src) => new()
    {
        CanView = src.CanView,
        CanModify = src.CanModify,
        CanDelete = src.CanDelete,
        CanInvitePeople = src.CanInvitePeople,
        CanCancelPeopleExistingInvitations = src.CanCancelPeopleExistingInvitations
    };

    public OrganizationMember
        MapTo(Member src) =>
        new()
        {
            Id = src.Id,
            MembershipType = src.MembershipType switch
            {
                MembershipType.Owner => OrganizationMembershipType.Owner,
                MembershipType.Administrator => OrganizationMembershipType.Administrator,
                MembershipType.Member => OrganizationMembershipType.Member,
                _ => throw new ArgumentOutOfRangeException()
            },
            Customer = MapTo(src.Customer)
        };

    public Team MapTo(global::Api.Shared.Services.Grpc.UnityHub.Team.V1.Team src)
    {
        var team = new Team
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            About = src.About.ToSafeString(),
            Timezone = src.Timezone.ToSafeString(),
            Organization = string.IsNullOrWhiteSpace(src.OrganizationId)
                ? null
                : new Shared.Models.Organization { Id = src.OrganizationId },
            Permissions = new TeamPermissions
            {
                CanView = src.Permissions.CanView,
                CanModify = src.Permissions.CanModify,
                CanDelete = src.Permissions.CanDelete,
                CanInvitePeople = src.Permissions.CanInvitePeople,
                CanCancelPeopleExistingInvitations = src.Permissions.CanCancelPeopleExistingInvitations
            },
            HasFutureBooking = src.HasFutureBooking
        };

        team.TeamMembers = MapTo(src.Members, team).ToList();

        return team;
    }

    public UpdateInput MapTo(Booking src)
    {
        var updateInput = new UpdateInput
        {
            Id = src.Id,
            From = src.From.ToTimestamp(),
            To = src.To.ToTimestamp(),
            Notes = src.Notes.ToSafeString(),
            CustomerId = src.Customer.Id,
            OrganizationId = string.IsNullOrWhiteSpace(src.Organization?.Id) ? string.Empty : src.Organization?.Id,
            LocationId = string.IsNullOrWhiteSpace(src.Location?.Id) ? string.Empty : src.Location?.Id,
            TeamId = string.IsNullOrWhiteSpace(src.Team?.Id) ? string.Empty : src.Team?.Id
        };

        updateInput.DeskIds.AddRange(src.Desks.Select(item => item.Id));

        return updateInput;
    }

    OrganizationBookingPermissions IMapper.MapTo(
        global::Api.Shared.Services.Grpc.UnityHub.Booking.V1.OrganizationPermissions src) =>
        new()
        {
            CanViewBookings = src.CanViewBookings,
            CanAddBooking = src.CanAddBooking,
            CanUpdateBooking = src.CanUpdateBooking,
            CanDeleteBooking = src.CanDeleteBooking,
            CanAddBookingOnBehalf = src.CanAddBookingOnBehalf,
            CanUpdateBookingOnBehalf = src.CanUpdateBookingOnBehalf,
            CanDeleteBookingOnBehalf = src.CanDeleteBookingOnBehalf
        };

    public LocationBookingPermissions MapTo(
        global::Api.Shared.Services.Grpc.UnityHub.Booking.V1.LocationPermissions src) =>
        new()
        {
            CanViewBookings = src.CanViewBookings,
            CanAddBooking = src.CanAddBooking,
            CanUpdateBooking = src.CanUpdateBooking,
            CanDeleteBooking = src.CanDeleteBooking,
            CanAddBookingOnBehalf = src.CanAddBookingOnBehalf,
            CanUpdateBookingOnBehalf = src.CanUpdateBookingOnBehalf,
            CanDeleteBookingOnBehalf = src.CanDeleteBookingOnBehalf
        };

    public TeamBookingPermissions MapTo(global::Api.Shared.Services.Grpc.UnityHub.Booking.V1.TeamPermissions src) =>
        new()
        {
            CanViewBookings = src.CanViewBookings,
            CanAddBooking = src.CanAddBooking,
            CanUpdateBooking = src.CanUpdateBooking,
            CanDeleteBooking = src.CanDeleteBooking,
            CanAddBookingOnBehalf = src.CanAddBookingOnBehalf,
            CanUpdateBookingOnBehalf = src.CanUpdateBookingOnBehalf,
            CanDeleteBookingOnBehalf = src.CanDeleteBookingOnBehalf
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

    public Customer MapTo(global::Api.Shared.Services.Grpc.UnityHub.Team.V1.Customer src) =>
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
            PhotoUrl512 = src.PhotoUrl512.ToSafeString()
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
            BotUserId = src.BotUserId,
            BotUserScope = src.BotUserScope,
            BotUserAccessToken = src.BotUserAccessToken,
            BotRefreshToken = src.BotRefreshToken,
            AuthedUserId = src.AuthedUserId,
            AuthedUserScope = src.AuthedUserScope,
            AuthedUserAccessToken = src.AuthedUserAccessToken,
            AuthedRefreshToken = src.AuthedRefreshToken,
            MembersLastRefreshedAt = src.MembersLastRefreshedAt,
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

    public Shared.Models.Location MapTo(global::Api.Shared.Services.Grpc.UnityHub.Location.V1.Location src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            About = src.About.ToSafeString(),
            Timezone = src.Timezone.ToSafeString(),
            Organization = string.IsNullOrWhiteSpace(src.OrganizationId)
                ? null
                : new Shared.Models.Organization { Id = src.OrganizationId },
            Desks = MapTo(src.Desks).ToList(),
            Permissions = new LocationPermissions
            {
                CanView = src.Permissions.CanView,
                CanModify = src.Permissions.CanModify,
                CanDelete = src.Permissions.CanDelete,
                CanInvitePeople = src.Permissions.CanInvitePeople,
                CanCancelPeopleExistingInvitations = src.Permissions.CanCancelPeopleExistingInvitations,
                CanViewAnalytics = src.Permissions.CanViewAnalytics
            },
            HasFutureBooking = src.HasFutureBooking
        };

    public Booking MapTo(global::Api.Shared.Services.Grpc.UnityHub.Booking.V1.Booking src) =>
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
            Team = MapTo(src.Team)
        };

    public Desk MapTo(global::Api.Shared.Services.Grpc.UnityHub.Location.V1.Desk src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Deactivated = src.Deactivated,
            RequireBookingApproval = src.RequireBookingApproval,
            OrganizationDeskTypes = MapTo(src.OrganizationDeskTypes).ToList(),
            OrganizationZones = MapTo(src.OrganizationZones).ToList()
        };

    public Workspace MapToEntity(Shared.Models.Workspace src, Organization organization) =>
        MergeToEntity(src, new Workspace(), organization);

    public Workspace MergeToEntity(Shared.Models.Workspace src, Workspace dest, Organization organization)
    {
        dest.Id = src.Id;
        dest.Name = src.Name;
        dest.BotUserId = src.BotUserId;
        dest.BotUserScope = src.BotUserScope;
        dest.BotUserAccessToken = src.BotUserAccessToken;
        dest.BotRefreshToken = src.BotRefreshToken;
        dest.AuthedUserId = src.AuthedUserId;
        dest.AuthedUserScope = src.AuthedUserScope;
        dest.AuthedUserAccessToken = src.AuthedUserAccessToken;
        dest.AuthedRefreshToken = src.AuthedRefreshToken;
        dest.Organization = organization;
        return dest;
    }

    public Shared.Models.Workspace MapTo(Admin_AddWorkspaceInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            BotUserId = src.BotUserId.ToSafeString(),
            BotUserScope = src.BotUserScope.ToSafeString(),
            BotUserAccessToken = src.BotUserAccessToken.ToSafeString(),
            BotRefreshToken = src.BotRefreshToken.ToSafeString(),
            AuthedUserId = src.AuthedUserId.ToSafeString(),
            AuthedUserScope = src.AuthedUserScope.ToSafeString(),
            AuthedUserAccessToken = src.AuthedUserAccessToken.ToSafeString(),
            AuthedRefreshToken = src.AuthedRefreshToken.ToSafeString(),
            Organization = new Shared.Models.Organization { Id = src.OrganizationId.ToSafeString() }
        };

    public global::Api.Shared.Services.Grpc.UnityHub.Slack.V1.Workspace MapTo(Shared.Models.Workspace src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            BotUserId = src.BotUserId.ToSafeString(),
            BotUserScope = src.BotUserScope.ToSafeString(),
            BotUserAccessToken = src.BotUserAccessToken.ToSafeString(),
            BotRefreshToken = src.BotRefreshToken.ToSafeString(),
            AuthedUserId = src.AuthedUserId.ToSafeString(),
            AuthedUserScope = src.AuthedUserScope.ToSafeString(),
            AuthedUserAccessToken = src.AuthedUserAccessToken.ToSafeString(),
            AuthedRefreshToken = src.AuthedRefreshToken.ToSafeString()
        };

    public OrganizationDeskType MapTo(DeskType src) =>
        new() { Id = src.Id, Name = src.Name.ToSafeString(), Description = src.Description.ToSafeString() };

    public OrganizationZone MapTo(Zone src) =>
        new() { Id = src.Id, Name = src.Name.ToSafeString(), Description = src.Description.ToSafeString() };

    private IEnumerable<TeamMember> MapTo(
        IEnumerable<global::Api.Shared.Services.Grpc.UnityHub.Team.V1.Member> src,
        Team team) =>
        src.Select(item => MapTo(item, team));

    private TeamMember MapTo(global::Api.Shared.Services.Grpc.UnityHub.Team.V1.Member src, Team team) =>
        new()
        {
            Id = src.Id,
            MembershipType = src.MembershipType switch
            {
                global::Api.Shared.Services.Grpc.UnityHub.Team.V1.MembershipType.Owner => TeamMembershipType.Owner,
                global::Api.Shared.Services.Grpc.UnityHub.Team.V1.MembershipType.Administrator => TeamMembershipType
                    .Administrator,
                global::Api.Shared.Services.Grpc.UnityHub.Team.V1.MembershipType.Member => TeamMembershipType.Member,
                _ => throw new ArgumentOutOfRangeException()
            },
            Customer = MapTo(src.Customer),
            OrganizationMember = src.OrganizationMember is null || string.IsNullOrWhiteSpace(src.OrganizationMember.Id)
                ? null
                : new OrganizationMember
                {
                    Id = src.OrganizationMember.Id, Customer = MapTo(src.OrganizationMember.Customer)
                },
            Team = team
        };

    private static Customer
        MapTo(global::Api.Shared.Services.Grpc.UnityHub.Organization.V1.Customer src) =>
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

    private static IEnumerable<Shared.Models.Identity>
        MapTo(IEnumerable<global::Api.Shared.Services.Grpc.UnityHub.Organization.V1.Identity> src) => src.Select(MapTo);

    private static Shared.Models.Identity
        MapTo(global::Api.Shared.Services.Grpc.UnityHub.Organization.V1.Identity src) =>
        new() { Id = src.Id, Email = src.Email.ToSafeString(), EmailVerified = src.EmailVerified };

    private static Shared.Models.Organization? MapTo(
        global::Api.Shared.Services.Grpc.UnityHub.Booking.V1.Organization? src) =>
        string.IsNullOrWhiteSpace(src?.Id)
            ? null
            : new Shared.Models.Organization { Id = src.Id, Name = src.Name.ToSafeString() };

    private static Shared.Models.Location? MapTo(global::Api.Shared.Services.Grpc.UnityHub.Booking.V1.Location? src) =>
        string.IsNullOrWhiteSpace(src?.Id)
            ? null
            : new Shared.Models.Location { Id = src.Id, Name = src.Name.ToSafeString() };

    private static Team? MapTo(global::Api.Shared.Services.Grpc.UnityHub.Booking.V1.Team? src) =>
        string.IsNullOrWhiteSpace(src?.Id) ? null : new Team { Id = src.Id, Name = src.Name.ToSafeString() };

    private static IEnumerable<Desk> MapTo(
        IEnumerable<global::Api.Shared.Services.Grpc.UnityHub.Booking.V1.Desk> src) => src.Select(MapTo);

    private static Desk MapTo(global::Api.Shared.Services.Grpc.UnityHub.Booking.V1.Desk src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            OrganizationDeskTypes = MapTo(src.OrganizationDeskTypes).ToList(),
            OrganizationZones = MapTo(src.OrganizationZones).ToList(),
            Location = string.IsNullOrWhiteSpace(src.Location?.Id)
                ? null
                : new Shared.Models.Location { Id = src.Id, Name = src.Name }
        };

    private static Customer MapTo(global::Api.Shared.Services.Grpc.UnityHub.Booking.V1.Customer src) =>
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

    private static IEnumerable<Shared.Models.Identity> MapTo(
        IEnumerable<global::Api.Shared.Services.Grpc.UnityHub.Booking.V1.Identity> src) => src.Select(MapTo);

    private static Shared.Models.Identity MapTo(global::Api.Shared.Services.Grpc.UnityHub.Booking.V1.Identity src) =>
        new() { Id = src.Id, Email = src.Email.ToSafeString(), EmailVerified = src.EmailVerified };

    private IEnumerable<Desk> MapTo(IEnumerable<global::Api.Shared.Services.Grpc.UnityHub.Location.V1.Desk> src) =>
        src.Select(MapTo);

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
            SlackChannelDailyUpdateLastSentAt = src.SlackChannelDailyUpdateLastSentAt
        };

        organization.OrganizationMembers = MapTo(src.OrganizationMembers, organization).ToList();

        return organization;
    }

    private IEnumerable<OrganizationMember> MapTo(
        IEnumerable<Shared.Database.Entities.OrganizationMember> src,
        Shared.Models.Organization organization) => src.Select(item => MapTo(item, organization));

    private OrganizationMember MapTo(
        Shared.Database.Entities.OrganizationMember src,
        Shared.Models.Organization organization) =>
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

    private static IEnumerable<Shared.Models.Identity> MapTo(
        IEnumerable<Shared.Database.Entities.Identity> src,
        Customer customer) =>
        src.Select(item => MapTo(item, customer));

    private static Shared.Models.Identity MapTo(Shared.Database.Entities.Identity src, Customer customer) =>
        new() { Id = src.Id, CreatedAt = src.CreatedAt, ModifiedAt = src.ModifiedAt, Customer = customer };

    private static IEnumerable<OrganizationDeskType> MapTo(
        IEnumerable<global::Api.Shared.Services.Grpc.UnityHub.Location.V1.OrganizationDeskType> src) =>
        src.Select(MapTo);

    private static OrganizationDeskType MapTo(
        global::Api.Shared.Services.Grpc.UnityHub.Location.V1.OrganizationDeskType src) =>
        new() { Id = src.Id, Name = src.Name.ToSafeString() };

    private static IEnumerable<OrganizationZone> MapTo(
        IEnumerable<global::Api.Shared.Services.Grpc.UnityHub.Location.V1.OrganizationZone> src) =>
        src.Select(MapTo);

    private static OrganizationZone MapTo(
        global::Api.Shared.Services.Grpc.UnityHub.Location.V1.OrganizationZone src) =>
        new() { Id = src.Id, Name = src.Name.ToSafeString() };

    private static IEnumerable<OrganizationDeskType> MapTo(
        IEnumerable<global::Api.Shared.Services.Grpc.UnityHub.Booking.V1.OrganizationDeskType> src) =>
        src.Select(MapTo);

    private static OrganizationDeskType MapTo(
        global::Api.Shared.Services.Grpc.UnityHub.Booking.V1.OrganizationDeskType src) =>
        new() { Id = src.Id, Name = src.Name.ToSafeString() };

    private static IEnumerable<OrganizationZone> MapTo(
        IEnumerable<global::Api.Shared.Services.Grpc.UnityHub.Booking.V1.OrganizationZone> src) => src.Select(MapTo);

    private static OrganizationZone MapTo(global::Api.Shared.Services.Grpc.UnityHub.Booking.V1.OrganizationZone src) =>
        new() { Id = src.Id, Name = src.Name.ToSafeString() };
}
