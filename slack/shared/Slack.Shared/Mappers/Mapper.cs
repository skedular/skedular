using Api.Shared.Services.Grpc.Skedular.Customer.V1;
using Api.Shared.Services.Models;
using Enterprise.Shared;
using Slack.Shared.Models;
using SlackNet;
using Booking = Slack.Shared.Models.Booking;
using Location = Slack.Shared.Models.Location;
using Team = Slack.Shared.Models.Team;
using Organization = Slack.Shared.Models.Organization;
using Customer = Slack.Shared.Models.Customer;
using OrganizationCustomTag = Api.Shared.Services.Grpc.Skedular.Booking.V1.OrganizationCustomTag;
using Identity = Slack.Shared.Models.Identity;
using OrganizationMember = Slack.Shared.Database.Entities.OrganizationMember;
using Workspace = Slack.Shared.Database.Entities.Workspace;
using WorkspaceMember = Slack.Shared.Database.Entities.WorkspaceMember;
using Admin_AddInput = Api.Shared.Services.Grpc.Skedular.Customer.V1.Admin_AddInput;
using Resource = Slack.Shared.Models.Resource;
using WorkspaceChannel = Slack.Shared.Database.Entities.WorkspaceChannel;

namespace Slack.Shared.Mappers;

public interface IMapper
{
    Customer? MapTo(Database.Entities.Customer? src);
    Booking MapTo(Api.Shared.Services.Grpc.Skedular.Booking.V1.Booking src);
    Models.Workspace MapTo(Workspace src);
    WorkspaceMember MapToEntity(User src, Workspace workspace);
    WorkspaceMember MergeToEntity(User src, WorkspaceMember dest, Workspace workspace);
    WorkspaceChannel MapToEntity(Conversation src, Workspace workspace);
    WorkspaceChannel MergeToEntity(Conversation src, WorkspaceChannel dest, Workspace workspace);
    Workspace MergeToEntity(SlackNet.Team src, Workspace dest);
    Admin_AddIdentityInput MapTo(WorkspaceMember src, string customerId);
    Admin_UpdateIdentityInput MapToUpdateIdentityInput(WorkspaceMember src, string customerId);

    Admin_AddInput MapTo(
        WorkspaceMember src,
        string customerId,
        Database.Entities.Organization defaultOrganization,
        ICollection<Database.Entities.Location> preferredLocations);
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
            Until = src.To.ToDateTimeOffset(),
            Notes = src.Notes.ToSafeString(),
            Resources = MapTo(src.Resources).ToList(),
            InvolvedCustomers = MapTo(src.InvolvedCustomers).ToList(),
            InvolvedOrganizations = MapTo(src.InvolvedOrganizations).ToList(),
            InvolvedLocations = MapTo(src.InvolvedLocations).ToList(),
            InvolvedTeams = MapTo(src.InvolvedTeams).ToList()
        };

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

    public Workspace MergeToEntity(SlackNet.Team src, Workspace dest)
    {
        dest.Name = src.Name;
        dest.Domain = src.Domain;
        dest.EmailDomain = src.EmailDomain;
        dest.EnterpriseId = src.EnterpriseId;
        dest.EnterpriseName = src.EnterpriseName;
        return dest;
    }

    public Admin_AddIdentityInput MapTo(WorkspaceMember src, string customerId) =>
        new() { Id = src.Id, Email = src.Email.ToSafeString(), EmailVerified = true, CustomerId = customerId };

    public Admin_UpdateIdentityInput MapToUpdateIdentityInput(WorkspaceMember src, string customerId) =>
        new() { Id = src.Id, Email = src.Email.ToSafeString(), EmailVerified = true, CustomerId = customerId };

    public Admin_AddInput MapTo(
        WorkspaceMember src,
        string customerId,
        Database.Entities.Organization defaultOrganization,
        ICollection<Database.Entities.Location> preferredLocations)
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
            IsOnboardingDone = true,
            DefaultOrganization = new Api.Shared.Services.Grpc.Skedular.Customer.V1.Organization { Id = defaultOrganization.Id }
        };

        input.Identities.Add(new Api.Shared.Services.Grpc.Skedular.Customer.V1.Identity { Id = src.Id, Email = src.Email, EmailVerified = true });

        input.PreferredLocations.AddRange(preferredLocations.Select(item =>
            new Api.Shared.Services.Grpc.Skedular.Customer.V1.Location
            {
                Id = item.Id, Organization = new Api.Shared.Services.Grpc.Skedular.Customer.V1.Organization { Id = defaultOrganization.Id }
            }));

        return input;
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
            Type = src.Type.ToOrganizationType(),
            MemberVisibilityPolicy = src.MemberVisibilityPolicy.ToOrganizationMemberVisibilityPolicy(),
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

    private static IEnumerable<Resource> MapTo(IEnumerable<Api.Shared.Services.Grpc.Skedular.Booking.V1.Resource> src) => src.Select(MapTo);

    private static Resource MapTo(Api.Shared.Services.Grpc.Skedular.Booking.V1.Resource src) =>
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

    private static IEnumerable<Models.OrganizationCustomTag> MapTo(IEnumerable<OrganizationCustomTag> src) => src.Select(MapTo);
    private static Models.OrganizationCustomTag MapTo(OrganizationCustomTag src) => new() { Id = src.Id, Name = src.Name.ToSafeString() };

    private static IEnumerable<OrganizationZone> MapTo(IEnumerable<Api.Shared.Services.Grpc.Skedular.Booking.V1.OrganizationZone> src) =>
        src.Select(MapTo);

    private static OrganizationZone MapTo(Api.Shared.Services.Grpc.Skedular.Booking.V1.OrganizationZone src) =>
        new() { Id = src.Id, Name = src.Name.ToSafeString(), Color = src.Color.ToSafeString() };

    private static Team? MapTo(Api.Shared.Services.Grpc.Skedular.Booking.V1.Team? src) =>
        string.IsNullOrWhiteSpace(src?.Id) ? null : new Team { Id = src.Id, Name = src.Name.ToSafeString() };

    private static ResourceType MapTo(Api.Shared.Services.Grpc.Skedular.Booking.V1.ResourceType src) =>
        new() { Id = src.Id, Name = src.Name.ToSafeString(), Color = src.Color.ToSafeString() };

    private static IEnumerable<Customer> MapTo(IEnumerable<Api.Shared.Services.Grpc.Skedular.Booking.V1.Customer> src) => src.Select(MapTo);
    private static IEnumerable<Organization> MapTo(IEnumerable<Api.Shared.Services.Grpc.Skedular.Booking.V1.Organization> src) => src.Select(MapTo)!;
    private static IEnumerable<Location> MapTo(IEnumerable<Api.Shared.Services.Grpc.Skedular.Booking.V1.Location> src) => src.Select(MapTo)!;
    private static IEnumerable<Team> MapTo(IEnumerable<Api.Shared.Services.Grpc.Skedular.Booking.V1.Team> src) => src.Select(MapTo)!;
}
