using Api.Shared.Services.Models;
using Enterprise.Shared;
using Slack.Shared.Models;
using SlackNet;
using SlackNet.WebApi;
using Constants = Slack.Shared.Constants.Constants;
using Customer = Slack.Shared.Models.Customer;
using Organization = Slack.Shared.Database.Entities.Organization;
using OrganizationMember = Slack.Shared.Models.OrganizationMember;
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
    WorkspaceChannel MapTo(Conversation src, Workspace workspace);
    Shared.Models.WorkspaceChannel? MapTo(WorkspaceChannel? src);
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
            IsOwnershipVerified = src.IsOwnershipVerified,
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
}
