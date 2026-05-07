using Api.Shared.Services.Models;
using Enterprise.Shared;
using SlackNet;
using Customer = Slack.Shared.Models.Customer;
using Identity = Slack.Shared.Models.Identity;
using Organization = Slack.Shared.Models.Organization;
using OrganizationMember = Slack.Shared.Database.Entities.OrganizationMember;
using Workspace = Slack.Shared.Database.Entities.Workspace;
using WorkspaceChannel = Slack.Shared.Database.Entities.WorkspaceChannel;
using WorkspaceMember = Slack.Shared.Database.Entities.WorkspaceMember;

namespace Slack.Shared.Mappers;

public interface IEntityMapper
{
    Customer? MapTo(Database.Entities.Customer? src);
    Models.Workspace MapTo(Workspace src);
    WorkspaceMember MapToEntity(User src, Workspace workspace);
    WorkspaceMember MergeToEntity(User src, WorkspaceMember dest, Workspace workspace);
    WorkspaceChannel MapToEntity(Conversation src, Workspace workspace);
    WorkspaceChannel MergeToEntity(Conversation src, WorkspaceChannel dest, Workspace workspace);
    Workspace MergeToEntity(Team src, Workspace dest);
}

public class EntityMapper : IEntityMapper
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
            Timezone = src.Timezone,
            Type = src.Type.ToNullableCustomerType()
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
            Organization = MapTo(src.Organization)
        };

        workspace.WorkspaceMembers = MapTo(src.WorkspaceMembers, workspace).ToList();

        return workspace;
    }

    public WorkspaceMember MapToEntity(User src, Workspace workspace) => MergeToEntity(src, new WorkspaceMember(), workspace);

    public WorkspaceMember MergeToEntity(User src, WorkspaceMember dest, Workspace workspace)
    {
        dest.Id = src.Id;
        dest.Email = src.Profile.Email.ToSafeString();
        dest.Designation = src.Profile.Title.ToSafeString().Truncate(Api.Shared.Services.Constants.MaxPersonDesignationLength);
        dest.Name = src.Profile.RealName.ToSafeString().Truncate(Api.Shared.Services.Constants.MaxPersonNameLength);
        dest.GivenName = src.Profile.FirstName.ToSafeString().Truncate(Api.Shared.Services.Constants.MaxGivenNameLength);
        dest.FamilyName = src.Profile.LastName.ToSafeString().Truncate(Api.Shared.Services.Constants.MaxFamilyNameLength);
        dest.Timezone = src.Tz.ToSafeString().Truncate(Api.Shared.Services.Constants.MaxTimezoneLength);
        dest.IsAdmin = src.IsAdmin;
        dest.IsOwner = src.IsOwner;
        dest.IsPrimaryOwner = src.IsPrimaryOwner;
        dest.Locale = src.Locale.ToSafeString().Truncate(Api.Shared.Services.Constants.MaxLocaleLength);
        dest.PhotoUrl = src.Profile.ImageOriginal.Truncate(Api.Shared.Services.Constants.MaxUrlLength);
        dest.PhotoUrl24 = src.Profile.Image24.Truncate(Api.Shared.Services.Constants.MaxUrlLength);
        dest.PhotoUrl32 = src.Profile.Image32.Truncate(Api.Shared.Services.Constants.MaxUrlLength);
        dest.PhotoUrl48 = src.Profile.Image48.Truncate(Api.Shared.Services.Constants.MaxUrlLength);
        dest.PhotoUrl72 = src.Profile.Image72.Truncate(Api.Shared.Services.Constants.MaxUrlLength);
        dest.PhotoUrl192 = src.Profile.Image192.Truncate(Api.Shared.Services.Constants.MaxUrlLength);
        dest.PhotoUrl512 = src.Profile.Image512.Truncate(Api.Shared.Services.Constants.MaxUrlLength);
        dest.Workspace = workspace;
        return dest;
    }

    public WorkspaceChannel MapToEntity(Conversation src, Workspace workspace) => MergeToEntity(src, new WorkspaceChannel(), workspace);

    public WorkspaceChannel MergeToEntity(Conversation src, WorkspaceChannel dest, Workspace workspace)
    {
        dest.Id = src.Id;
        dest.Name = src.Name.Truncate(Api.Shared.Services.Constants.MaxUrlLength);
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

    public Workspace MergeToEntity(Team src, Workspace dest)
    {
        dest.Name = src.Name;
        dest.Domain = src.Domain;
        dest.EmailDomain = src.EmailDomain;
        dest.EnterpriseId = src.EnterpriseId;
        dest.EnterpriseName = src.EnterpriseName;
        return dest;
    }

    private Organization MapTo(Database.Entities.Organization src)
    {
        var organization = new Organization
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            DeletedAt = src.DeletedAt,
            EventRaisedAt = src.EventRaisedAt,
            CustomDomain = src.CustomDomain,
            Type = src.Type.ToOrganizationType(),
            IsOwnershipVerified = src.IsOwnershipVerified,
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
}
