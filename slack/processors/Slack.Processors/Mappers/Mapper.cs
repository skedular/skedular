using Api.Shared;
using Api.Shared.Clients.Events.UnityHub.Organization.V1.Value;
using Api.Shared.Models;
using Api.Shared.Services.Grpc.UnityHub.Customer.V1;
using Enterprise.Shared;
using Slack.Shared.Models;
using SlackNet;
using Location = Slack.Shared.Models.Location;
using Team = Slack.Shared.Models.Team;
using Organization = Slack.Shared.Models.Organization;
using Customer = Slack.Shared.Models.Customer;
using Desk = Slack.Shared.Models.Desk;
using Event = Api.Shared.Clients.Events.UnityHub.Customer.V1.Value.Event;
using Identity = Slack.Shared.Models.Identity;
using LocationTag = Slack.Shared.Models.LocationTag;
using OrganizationMember = Slack.Shared.Database.Entities.OrganizationMember;
using Workspace = Slack.Shared.Database.Entities.Workspace;
using WorkspaceChannel = Slack.Shared.Database.Entities.WorkspaceChannel;
using WorkspaceMember = Slack.Shared.Database.Entities.WorkspaceMember;

namespace Slack.Processors.Mappers;

public interface IMapper
{
    Customer? MapTo(Shared.Database.Entities.Customer? src);
    Customer MapTo(Event src);

    Shared.Database.Entities.Customer MapToEntity(
        Customer src,
        ICollection<Shared.Database.Entities.Identity> identities);

    Shared.Database.Entities.Customer MergeToEntity(
        Customer src,
        Shared.Database.Entities.Customer dest,
        ICollection<Shared.Database.Entities.Identity> identities);

    IEnumerable<Shared.Database.Entities.Identity> MapToEntity(
        IEnumerable<Identity> src,
        Shared.Database.Entities.Customer? customer);

    Shared.Database.Entities.Identity MapToEntity(Identity src, Shared.Database.Entities.Customer? customer);

    Shared.Database.Entities.Identity MergeToEntity(
        Identity src,
        Shared.Database.Entities.Identity dest,
        Shared.Database.Entities.Customer? customer);

    Location MapTo(Api.Shared.Clients.Events.UnityHub.Location.V1.Value.Event src);
    Shared.Database.Entities.Location MapToEntity(Location src);
    Shared.Database.Entities.Location MergeToEntity(Location src, Shared.Database.Entities.Location dest);
    Organization MapTo(Api.Shared.Clients.Events.UnityHub.Organization.V1.Value.Event src);
    Shared.Database.Entities.Organization MapToEntity(Organization src);
    Shared.Database.Entities.Organization MergeToEntity(Organization src, Shared.Database.Entities.Organization dest);
    Team MapTo(Api.Shared.Clients.Events.UnityHub.Team.V1.Value.Event src);
    Shared.Database.Entities.Team MapToEntity(Team src);
    Shared.Database.Entities.Team MergeToEntity(Team src, Shared.Database.Entities.Team dest);

    OrganizationMember MapToEntity(
        Shared.Models.OrganizationMember src,
        Shared.Database.Entities.Organization organization,
        Shared.Database.Entities.Customer customer);

    OrganizationMember MergeToEntity(
        Shared.Models.OrganizationMember src,
        OrganizationMember dest,
        Shared.Database.Entities.Organization organization,
        Shared.Database.Entities.Customer customer);

    WorkspaceChannel MapToEntity(Conversation src, Workspace workspace);
    WorkspaceChannel MergeToEntity(Conversation src, WorkspaceChannel dest, Workspace workspace);
    WorkspaceMember MapToEntity(User src, Workspace workspace);
    WorkspaceMember MergeToEntity(User src, WorkspaceMember dest, Workspace workspace);

    Admin_AddInput MapTo(
        WorkspaceMember src,
        string customerId,
        Shared.Database.Entities.Organization defaultOrganization,
        ICollection<Shared.Database.Entities.Location> defaultLocations);

    Admin_AddIdentityInput MapTo(WorkspaceMember src, string customerId);
    Booking MapTo(Api.Shared.Services.Grpc.UnityHub.Booking.V1.Booking src);
    Shared.Models.Workspace MapTo(Workspace src);
}

public class Mapper : IMapper
{
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
            Timezone = customer.Timezone.ToSafeString(),
            Identities = customer.Identities
                .Select(item => new Identity { Id = item.Id, Email = item.Email, EmailVerified = item.EmailVerified })
                .ToList()
        };
    }

    public Shared.Database.Entities.Customer MapToEntity(
        Customer src,
        ICollection<Shared.Database.Entities.Identity> identities) =>
        MergeToEntity(src, new Shared.Database.Entities.Customer(), identities);

    public Shared.Database.Entities.Customer MergeToEntity(
        Customer src,
        Shared.Database.Entities.Customer dest,
        ICollection<Shared.Database.Entities.Identity> identities)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Timezone = src.Timezone;
        dest.Identities = identities;

        return dest;
    }

    public IEnumerable<Shared.Database.Entities.Identity> MapToEntity(
        IEnumerable<Identity> src,
        Shared.Database.Entities.Customer? customer) =>
        src.Select(identity => MapToEntity(identity, customer));

    public Shared.Database.Entities.Identity MapToEntity(Identity src, Shared.Database.Entities.Customer? customer) =>
        MergeToEntity(src, new Shared.Database.Entities.Identity(), customer);

    public Shared.Database.Entities.Identity MergeToEntity(
        Identity src,
        Shared.Database.Entities.Identity dest,
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

    public WorkspaceChannel MapToEntity(Conversation src, Workspace workspace) =>
        MergeToEntity(src, new WorkspaceChannel(), workspace);

    public WorkspaceChannel MergeToEntity(Conversation src, WorkspaceChannel dest, Workspace workspace)
    {
        dest.Id = src.Id;
        dest.Name = src.Name.Truncate(Constants.MaxUrlLength);
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

    public WorkspaceMember MapToEntity(User src, Workspace workspace) =>
        MergeToEntity(src, new WorkspaceMember(), workspace);

    public WorkspaceMember MergeToEntity(User src, WorkspaceMember dest, Workspace workspace)
    {
        dest.Id = src.Id;
        dest.Email = src.Profile.Email.ToSafeString();
        dest.Designation = src.Profile.Title.ToSafeString().Truncate(Constants.MaxDesignationLength);
        dest.Name = src.Profile.RealName.ToSafeString().Truncate(Constants.MaxPersonNameLength);
        dest.GivenName = src.Profile.FirstName.ToSafeString().Truncate(Constants.MaxGivenNameLength);
        dest.FamilyName = src.Profile.LastName.ToSafeString().Truncate(Constants.MaxFamilyNameLength);
        dest.Timezone = src.Tz.ToSafeString().Truncate(Constants.MaxTimezoneLength);
        dest.IsAdmin = src.IsAdmin;
        dest.IsOwner = src.IsOwner;
        dest.IsPrimaryOwner = src.IsPrimaryOwner;
        dest.Locale = src.Locale.ToSafeString().Truncate(Constants.MaxLocaleLength);
        dest.PhotoUrl = src.Profile.ImageOriginal.Truncate(Constants.MaxUrlLength);
        dest.PhotoUrl24 = src.Profile.Image24.Truncate(Constants.MaxUrlLength);
        dest.PhotoUrl32 = src.Profile.Image32.Truncate(Constants.MaxUrlLength);
        dest.PhotoUrl48 = src.Profile.Image48.Truncate(Constants.MaxUrlLength);
        dest.PhotoUrl72 = src.Profile.Image72.Truncate(Constants.MaxUrlLength);
        dest.PhotoUrl192 = src.Profile.Image192.Truncate(Constants.MaxUrlLength);
        dest.PhotoUrl512 = src.Profile.Image512.Truncate(Constants.MaxUrlLength);
        dest.Workspace = workspace;
        return dest;
    }

    public Admin_AddInput MapTo(
        WorkspaceMember src,
        string customerId,
        Shared.Database.Entities.Organization defaultOrganization,
        ICollection<Shared.Database.Entities.Location> defaultLocations)
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

    public Admin_AddIdentityInput MapTo(WorkspaceMember src, string customerId) =>
        new() { Id = src.Id, Email = src.Email, EmailVerified = true, CustomerId = customerId };

    public Booking MapTo(Api.Shared.Services.Grpc.UnityHub.Booking.V1.Booking src) =>
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

    private IEnumerable<Shared.Models.WorkspaceMember> MapTo(IEnumerable<WorkspaceMember> src,
        Shared.Models.Workspace workspace) => src.Select(item => MapTo(item, workspace));

    private Organization MapTo(Shared.Database.Entities.Organization src)
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

    private IEnumerable<Shared.Models.OrganizationMember> MapTo(
        IEnumerable<OrganizationMember> src,
        Organization organization) => src.Select(item => MapTo(item, organization));

    private Shared.Models.OrganizationMember MapTo(
        OrganizationMember src,
        Organization organization) =>
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

    private static IEnumerable<Identity> MapTo(
        IEnumerable<Shared.Database.Entities.Identity> src,
        Customer customer) =>
        src.Select(item => MapTo(item, customer));

    private static Identity MapTo(Shared.Database.Entities.Identity src, Customer customer) =>
        new() { Id = src.Id, CreatedAt = src.CreatedAt, ModifiedAt = src.ModifiedAt, Customer = customer };

    private static Customer MapTo(Api.Shared.Services.Grpc.UnityHub.Booking.V1.Customer src) =>
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

    private static IEnumerable<Identity> MapTo(
        IEnumerable<Api.Shared.Services.Grpc.UnityHub.Booking.V1.Identity> src) => src.Select(MapTo);

    private static Identity MapTo(Api.Shared.Services.Grpc.UnityHub.Booking.V1.Identity src) =>
        new() { Id = src.Id, Email = src.Email.ToSafeString(), EmailVerified = src.EmailVerified };

    private static Organization? MapTo(
        Api.Shared.Services.Grpc.UnityHub.Booking.V1.Organization? src) =>
        string.IsNullOrWhiteSpace(src?.Id)
            ? null
            : new Organization { Id = src.Id, Name = src.Name.ToSafeString() };

    private static Location? MapTo(Api.Shared.Services.Grpc.UnityHub.Booking.V1.Location? src) =>
        string.IsNullOrWhiteSpace(src?.Id)
            ? null
            : new Location { Id = src.Id, Name = src.Name.ToSafeString() };

    private static IEnumerable<Desk> MapTo(
        IEnumerable<Api.Shared.Services.Grpc.UnityHub.Booking.V1.Desk> src) => src.Select(MapTo);

    private static Desk MapTo(Api.Shared.Services.Grpc.UnityHub.Booking.V1.Desk src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Tags = MapTo(src.LocationTags).ToList(),
            Location = string.IsNullOrWhiteSpace(src.Location?.Id)
                ? null
                : new Location { Id = src.Id, Name = src.Name }
        };

    private static IEnumerable<LocationTag> MapTo(
        IEnumerable<Api.Shared.Services.Grpc.UnityHub.Booking.V1.LocationTag> src) => src.Select(MapTo);

    private static LocationTag MapTo(Api.Shared.Services.Grpc.UnityHub.Booking.V1.LocationTag src) =>
        new() { Id = src.Id, Name = src.Name.ToSafeString(), Type = src.TagType.ToSafeString() };

    private static Team? MapTo(Api.Shared.Services.Grpc.UnityHub.Booking.V1.Team? src) =>
        string.IsNullOrWhiteSpace(src?.Id) ? null : new Team { Id = src.Id, Name = src.Name.ToSafeString() };

    private static Shared.Models.WorkspaceMember MapTo(WorkspaceMember src, Shared.Models.Workspace workspace) =>
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
}
