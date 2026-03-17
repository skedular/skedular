using Api.Shared.Services.Grpc.Skedular.Team.V1;
using Api.Shared.Services.Models;
using Enterprise.Shared;
using HotChocolate.Types.Pagination;
using Team.Api.GraphQL.Invitation;
using Team.Api.GraphQL.Member;
using Team.Api.GraphQL.Team;
using CdnFile = Api.Shared.Services.Grpc.Skedular.Team.V1.CdnFile;
using CdnImageFile = Api.Shared.Services.Grpc.Skedular.Team.V1.CdnImageFile;
using Customer = Team.Shared.Models.Customer;
using Identity = Team.Shared.Models.Identity;
using JoinInvitation = Team.Shared.Models.JoinInvitation;
using Location = Team.Shared.Database.Entities.Location;
using Organization = Team.Shared.Database.Entities.Organization;
using OrganizationMember = Team.Shared.Models.OrganizationMember;
using OrganizationMemberRole = Api.Shared.Services.Models.OrganizationMemberRole;
using Permissions = Api.Shared.Services.Grpc.Skedular.Team.V1.Permissions;
using TeamEdge = Team.Api.GraphQL.Team.TeamEdge;
using TeamMember = Team.Shared.Models.TeamMember;
using TeamMemberStatus = Api.Shared.Services.Models.TeamMemberStatus;

namespace Team.Api.Mappers;

public interface IMapper
{
    TeamMember MapTo(Shared.Database.Entities.TeamMember src);
    Shared.Models.Team MapTo(Shared.Database.Entities.Team src);
    Shared.Database.Entities.Team MapTo(Shared.Models.Team src, Organization organization, Location? primaryLocation);

    Shared.Database.Entities.Team MergeTo(
        Shared.Models.Team src,
        Shared.Database.Entities.Team dest,
        Organization organization,
        Location? primaryLocation);

    TeamDetails? MapTo(Shared.Models.Team? src);
    TeamMember MapTo(Shared.Database.Entities.TeamMember src, Shared.Models.Team team);
    TeamMemberDetails MapTo(TeamMember src);
    IEnumerable<TeamDetails> MapTo(IEnumerable<Shared.Models.Team> src);
    Shared.Models.Team MapTo(AddTeamInput src);
    Shared.Models.Team MapTo(UpdateTeamInput src);
    Shared.Models.Team MapTo(UpdateTeamAndTeamMembersInput src);
    TeamMember MapTo(AddTeamMemberInput src);
    ICollection<TeamMember> MapToTeamMembers(UpdateTeamMembersInput src);
    JoinInvitation MapTo(Shared.Database.Entities.JoinInvitation src);
    global::Api.Shared.Services.Grpc.Skedular.Team.V1.Team MapToGrpcResponse(Shared.Models.Team src);
    Shared.Models.Team MapTo(AddInput src);
    Shared.Models.Team MapTo(UpdateInput src);
    TeamEdge MapTo(Edge<Shared.Models.Team> src);
    global::Api.Shared.Services.Grpc.Skedular.Team.V1.TeamEdge MapToGrpcResponse(Edge<Shared.Models.Team> src);
    IEnumerable<Edge<TeamMember>> MapTo(IEnumerable<Edge<Shared.Database.Entities.TeamMember>> src, Shared.Models.Team team);
    TeamMemberEdge MapTo(Edge<TeamMember> src);
    IEnumerable<InviteCustomerToJoinTeamDetails> MapTo(IEnumerable<JoinInvitation> src);
    InviteCustomerToJoinTeamDetails MapTo(JoinInvitation src);
    Edge<JoinInvitation> MapTo(Edge<Shared.Database.Entities.JoinInvitation> src);
    TeamJoinInvitationEdge MapTo(Edge<JoinInvitation> src);
}

public class Mapper : IMapper
{
    public TeamMember MapTo(Shared.Database.Entities.TeamMember src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Role = src.Role.ToTeamMemberRole(),
            Status = src.Status.ToTeamMemberStatus(),
            Customer = MapTo(src.Customer)!,
            OrganizationMember = MapTo(src.OrganizationMember)
        };


    public Shared.Models.Team MapTo(Shared.Database.Entities.Team src)
    {
        var team = new Shared.Models.Team
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Name = src.Name,
            About = src.About,
            Timezone = src.Timezone,
            FeatureImages = src.FeatureImages.ToSafeCollection(),
            Organization = MapTo(src.Organization),
            PrimaryLocation = MapTo(src.PrimaryLocation)
        };

        team.TeamMembers = MapTo(src.TeamMembers, team).ToList();
        team.JoinInvitations = MapTo(src.JoinInvitations, team).ToList();

        return team;
    }

    public Shared.Database.Entities.Team MapTo(Shared.Models.Team src, Organization organization, Location? primaryLocation) =>
        MergeTo(src, new Shared.Database.Entities.Team(), organization, primaryLocation);

    public Shared.Database.Entities.Team MergeTo(
        Shared.Models.Team src,
        Shared.Database.Entities.Team dest,
        Organization organization,
        Location? primaryLocation)
    {
        dest.Id = src.Id;
        dest.Name = src.Name;
        dest.About = src.About;
        dest.Timezone = src.Timezone;
        dest.FeatureImages = src.FeatureImages;
        dest.Organization = organization;
        dest.PrimaryLocation = primaryLocation;
        return dest;
    }

    public TeamDetails? MapTo(Shared.Models.Team? src) =>
        src is null
            ? null
            : new TeamDetails
            {
                Id = src.Id,
                Name = src.Name,
                About = src.About,
                Timezone = src.Timezone,
                FeatureImages = src.FeatureImages,
                CanModify = src.Permissions.CanModify,
                CanDelete = src.Permissions.CanDelete,
                CanInvitePeople = src.Permissions.CanInvitePeople,
                OrganizationId = src.Organization.Id,
                OrganizationCustomDomain = src.Organization.CustomDomain.ToSafeString(),
                PrimaryLocationId = src.PrimaryLocation?.Id
            };

    public TeamMember MapTo(Shared.Database.Entities.TeamMember src, Shared.Models.Team team) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Role = src.Role.ToTeamMemberRole(),
            Status = src.Status.ToTeamMemberStatus(),
            Customer = MapTo(src.Customer)!,
            Team = team,
            OrganizationMember = MapTo(src.OrganizationMember)
        };

    public TeamMemberDetails MapTo(TeamMember src) =>
        new()
        {
            Id = src.Id,
            Role = src.Role,
            Status = src.Status,
            CustomerId = src.Customer.Id,
            OrganizationMember = MapTo(src.OrganizationMember)
        };

    public IEnumerable<TeamDetails> MapTo(IEnumerable<Shared.Models.Team> src) =>
        src.Select(MapTo)!;

    public Shared.Models.Team MapTo(AddTeamInput src) =>
        new()
        {
            Id = src.Id.ToSafeString(),
            Name = src.Name,
            About = src.About,
            Timezone = src.Timezone,
            FeatureImages = src.FeatureImages.ToSafeCollection(),
            Organization =
                new Shared.Models.Organization { Id = src.OrganizationId.ToSafeString(), CustomDomain = src.OrganizationCustomDomain.ToSafeString() },
            PrimaryLocation = string.IsNullOrWhiteSpace(src.PrimaryLocationId) ? null : new Shared.Models.Location { Id = src.PrimaryLocationId },
            TeamMembers = src.CustomerIds
                .Select(item => new TeamMember { Customer = new Customer { Id = item } })
                .Concat(src.OrganizationMemberIds.Select(item => new TeamMember { OrganizationMember = new OrganizationMember { Id = item } }))
                .ToList()
        };

    public Shared.Models.Team MapTo(UpdateTeamInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name,
            About = src.About,
            Timezone = src.Timezone,
            FeatureImages = src.FeatureImages.ToSafeCollection(),
            PrimaryLocation = string.IsNullOrWhiteSpace(src.PrimaryLocationId) ? null : new Shared.Models.Location { Id = src.PrimaryLocationId }
        };

    public Shared.Models.Team MapTo(UpdateTeamAndTeamMembersInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name,
            About = src.About,
            Timezone = src.Timezone,
            FeatureImages = src.FeatureImages.ToSafeCollection(),
            Organization =
                new Shared.Models.Organization { Id = src.OrganizationId.ToSafeString(), CustomDomain = src.OrganizationCustomDomain.ToSafeString() },
            PrimaryLocation = string.IsNullOrWhiteSpace(src.PrimaryLocationId) ? null : new Shared.Models.Location { Id = src.PrimaryLocationId },
            TeamMembers = src.CustomerIds
                .Select(item => new TeamMember { Customer = new Customer { Id = item } })
                .Concat(src.OrganizationMemberIds.Select(item => new TeamMember { OrganizationMember = new OrganizationMember { Id = item } }))
                .ToList()
        };

    public TeamMember MapTo(AddTeamMemberInput src)
    {
        var teamMember = new TeamMember
        {
            OrganizationMember = string.IsNullOrWhiteSpace(src.OrganizationMemberId)
                ? null
                : new OrganizationMember { Id = src.OrganizationMemberId }
        };

        if (!string.IsNullOrWhiteSpace(src.CustomerId))
        {
            teamMember.Customer = new Customer { Id = src.CustomerId };
        }

        return teamMember;
    }

    public ICollection<TeamMember> MapToTeamMembers(UpdateTeamMembersInput src) =>
        src.CustomerIds
            .Select(item => new TeamMember { Customer = new Customer { Id = item } })
            .Concat(src.OrganizationMemberIds.Select(item => new TeamMember { OrganizationMember = new OrganizationMember { Id = item } }))
            .ToList();

    public JoinInvitation MapTo(Shared.Database.Entities.JoinInvitation src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            Email = src.Email,
            Status = src.Status.ToInvitationStatus(),
            Role = src.Role.ToTeamMemberRole(),
            Team = MapTo(src.Team),
            CreatedBy = MapTo(src.CreatedBy)!,
            Invitee = MapTo(src.Invitee)
        };

    public global::Api.Shared.Services.Grpc.Skedular.Team.V1.Team MapToGrpcResponse(Shared.Models.Team src)
    {
        var team = new global::Api.Shared.Services.Grpc.Skedular.Team.V1.Team
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            About = src.About.ToSafeString(),
            Timezone = src.Timezone.ToSafeString(),
            OrganizationId = string.IsNullOrWhiteSpace(src.Organization.Id) ? string.Empty : src.Organization.Id,
            PrimaryLocationId = src.PrimaryLocation is null ? string.Empty : src.PrimaryLocation.Id.ToSafeString(),
            Permissions = new Permissions
            {
                CanView = src.Permissions.CanView,
                CanModify = src.Permissions.CanModify,
                CanDelete = src.Permissions.CanDelete,
                CanInvitePeople = src.Permissions.CanInvitePeople,
                CanCancelPeopleExistingInvitations = src.Permissions.CanCancelPeopleExistingInvitations
            }
        };

        team.Members.AddRange(MapToGrpcResponse(src.TeamMembers));
        team.FeatureImages.AddRange(MapTo(src.FeatureImages));

        return team;
    }


    public Shared.Models.Team MapTo(AddInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name,
            About = src.About,
            Timezone = src.Timezone,
            FeatureImages = MapTo(src.FeatureImages).ToList(),
            Organization = new Shared.Models.Organization { Id = src.OrganizationId },
            PrimaryLocation = string.IsNullOrWhiteSpace(src.PrimaryLocationId) ? null : new Shared.Models.Location { Id = src.PrimaryLocationId },
            TeamMembers = src.Members.Select(item => MapTo(item, new Shared.Models.Team { Id = src.Id })).ToList()
        };

    public Shared.Models.Team MapTo(UpdateInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name,
            About = src.About,
            Timezone = src.Timezone,
            FeatureImages = MapTo(src.FeatureImages).ToList(),
            Organization = new Shared.Models.Organization { Id = src.OrganizationId },
            PrimaryLocation = string.IsNullOrWhiteSpace(src.PrimaryLocationId) ? null : new Shared.Models.Location { Id = src.PrimaryLocationId },
            TeamMembers = src.Members.Select(item => MapTo(item, new Shared.Models.Team { Id = src.Id })).ToList()
        };

    public TeamEdge MapTo(Edge<Shared.Models.Team> src) => new(MapTo(src.Node)!, src.Cursor);

    public global::Api.Shared.Services.Grpc.Skedular.Team.V1.TeamEdge MapToGrpcResponse(Edge<Shared.Models.Team> src) =>
        new() { Cursor = src.Cursor, Node = MapToGrpcResponse(src.Node) };

    public IEnumerable<Edge<TeamMember>> MapTo(IEnumerable<Edge<Shared.Database.Entities.TeamMember>> src, Shared.Models.Team team) =>
        src.Select(item => MapTo(item, team));

    public TeamMemberEdge MapTo(Edge<TeamMember> src) => new(MapTo(src.Node), src.Cursor);


    public IEnumerable<InviteCustomerToJoinTeamDetails> MapTo(IEnumerable<JoinInvitation> src) =>
        src.Select(MapTo);

    public InviteCustomerToJoinTeamDetails MapTo(JoinInvitation src) =>
        new()
        {
            Id = src.Id,
            Email = src.Email,
            Status = new TeamInvitationStatusDetails { Type = src.Status, Name = src.Status.ToInvitationStatusName() },
            Role = src.Role,
            Team = MapTo(src.Team)!,
            CreatedById = src.CreatedBy.Id,
            InviteeId = src.Invitee?.Id
        };

    public Edge<JoinInvitation> MapTo(Edge<Shared.Database.Entities.JoinInvitation> src) => new(MapTo(src.Node), src.Cursor);
    public TeamJoinInvitationEdge MapTo(Edge<JoinInvitation> src) => new(MapTo(src.Node), src.Cursor);

    private static Customer? MapTo(Shared.Database.Entities.Customer? src) =>
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
                Type = src.Type.ToNullableCustomerType(),
                Identities = MapTo(src.Identities).ToList()
            };

    private IEnumerable<TeamMember> MapTo(IEnumerable<Shared.Database.Entities.TeamMember> src, Shared.Models.Team team) =>
        src.Select(item => MapTo(item, team));

    private static IEnumerable<global::Api.Shared.Services.Grpc.Skedular.Team.V1.TeamMember> MapToGrpcResponse(IEnumerable<TeamMember> src) =>
        src.Select(MapToGrpcResponse);

    private static global::Api.Shared.Services.Grpc.Skedular.Team.V1.TeamMember MapToGrpcResponse(TeamMember src) =>
        new()
        {
            Id = src.Id,
            Role = src.Role switch
            {
                TeamMemberRole.Owner => Role.Owner,
                TeamMemberRole.Administrator => Role.Administrator,
                TeamMemberRole.Member => Role.Member,
                _ => throw new ArgumentOutOfRangeException()
            },
            Status = src.Status switch
            {
                TeamMemberStatus.Active => global::Api.Shared.Services.Grpc.Skedular.Team.V1.TeamMemberStatus.Active,
                TeamMemberStatus.Inactive => global::Api.Shared.Services.Grpc.Skedular.Team.V1.TeamMemberStatus.Inactive,
                _ => throw new ArgumentOutOfRangeException()
            },
            CustomerId = src.Customer.Id.ToSafeString(),
            OrganizationMember = src.OrganizationMember is null || string.IsNullOrWhiteSpace(src.OrganizationMember.Id)
                ? null
                : new global::Api.Shared.Services.Grpc.Skedular.Team.V1.OrganizationMember
                {
                    Id = src.OrganizationMember.Id,
                    Role = src.OrganizationMember.Role switch
                    {
                        OrganizationMemberRole.Owner => Role.Owner,
                        OrganizationMemberRole.Administrator => Role.Administrator,
                        OrganizationMemberRole.Member => Role.Member,
                        _ => throw new ArgumentOutOfRangeException()
                    },
                    CustomerId = src.OrganizationMember.Customer.Id.ToSafeString()
                }
        };

    private static TeamMember MapTo(global::Api.Shared.Services.Grpc.Skedular.Team.V1.TeamMember src, Shared.Models.Team team) =>
        new()
        {
            Id = src.Id,
            Role = src.Role switch
            {
                Role.Owner => TeamMemberRole.Owner,
                Role.Administrator => TeamMemberRole.Administrator,
                Role.Member => TeamMemberRole.Member,
                _ => throw new ArgumentOutOfRangeException()
            },
            Status = src.Status switch
            {
                global::Api.Shared.Services.Grpc.Skedular.Team.V1.TeamMemberStatus.Active => TeamMemberStatus.Active,
                global::Api.Shared.Services.Grpc.Skedular.Team.V1.TeamMemberStatus.Inactive => TeamMemberStatus.Inactive,
                _ => throw new ArgumentOutOfRangeException()
            },
            Customer = new Customer { Id = src.CustomerId },
            OrganizationMember = src.OrganizationMember is null || string.IsNullOrWhiteSpace(src.OrganizationMember.Id)
                ? null
                : new OrganizationMember { Id = src.OrganizationMember.Id, Customer = new Customer { Id = src.OrganizationMember.CustomerId } },
            Team = team
        };

    private static Shared.Models.Organization MapTo(Organization src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            EventRaisedAt = src.EventRaisedAt,
            CustomDomain = src.CustomDomain,
            Name = src.Name,
            LogoUrl = src.LogoUrl,
            Offering = src.Offering,
            Type = src.Type.ToOrganizationType(),
            IsOwnershipVerified = src.IsOwnershipVerified
        };

    private OrganizationMember? MapTo(Shared.Database.Entities.OrganizationMember? src) =>
        src is null
            ? null
            : new OrganizationMember
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                Role = src.Role.ToNullableOrganizationMemberRole(),
                Status = src.Status.ToOrganizationMemberStatus(),
                Customer = MapTo(src.Customer)!,
                Organization = MapTo(src.Organization)
            };

    private static TeamOrganizationMemberDetails? MapTo(OrganizationMember? src) =>
        src is null ? null : new TeamOrganizationMemberDetails { UniqueId = src.Id, CustomerId = src.Customer.Id };

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

    private IEnumerable<JoinInvitation> MapTo(IEnumerable<Shared.Database.Entities.JoinInvitation> src, Shared.Models.Team team) =>
        src.Select(item => MapTo(item, team));

    private JoinInvitation MapTo(Shared.Database.Entities.JoinInvitation src, Shared.Models.Team team) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            Email = src.Email,
            Status = src.Status.ToInvitationStatus(),
            Team = team,
            CreatedBy = MapTo(src.CreatedBy)!,
            Invitee = MapTo(src.Invitee)
        };

    private Edge<TeamMember> MapTo(Edge<Shared.Database.Entities.TeamMember> src, Shared.Models.Team team) => new(MapTo(src.Node, team), src.Cursor);

    private static Shared.Models.Location? MapTo(Location? src) =>
        src is null
            ? null
            : new Shared.Models.Location
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                EventRaisedAt = src.EventRaisedAt
            };

    private static IEnumerable<global::Api.Shared.Services.Models.CdnImageFile> MapTo(IEnumerable<CdnImageFile> src) =>
        src.Select(MapTo)!;

    private static global::Api.Shared.Services.Models.CdnImageFile? MapTo(CdnImageFile? src) =>
        src is null ? null : new global::Api.Shared.Services.Models.CdnImageFile(MapTo(src.Original), MapTo(src.Thumbnail));

    private static global::Api.Shared.Services.Models.CdnFile? MapTo(CdnFile? src) =>
        src is null ? null : new global::Api.Shared.Services.Models.CdnFile(src.Url, src.Height.FromNullInt(), src.Width.FromNullInt());

    private static IEnumerable<CdnImageFile> MapTo(IEnumerable<global::Api.Shared.Services.Models.CdnImageFile> src) =>
        src.Select(MapTo);

    private static CdnImageFile MapTo(global::Api.Shared.Services.Models.CdnImageFile src) =>
        new() { Original = MapTo(src.Original), Thumbnail = MapTo(src.Thumbnail) };

    private static CdnFile? MapTo(global::Api.Shared.Services.Models.CdnFile? src) =>
        src is null ? null : new CdnFile { Url = src.Url.ToSafeString(), Height = src.Height.ToNullInt(), Width = src.Width.ToNullInt() };
}
