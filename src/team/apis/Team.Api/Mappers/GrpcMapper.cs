using Api.Shared.Grpc.Skedular.Team.Core.V1;
using Api.Shared.Services.Models;
using Enterprise.Shared;
using HotChocolate.Types.Pagination;
using Team.Shared.Models;
using CdnFile = Api.Shared.Grpc.Skedular.Team.Core.V1.CdnFile;
using CdnImageFile = Api.Shared.Grpc.Skedular.Team.Core.V1.CdnImageFile;
using Customer = Team.Shared.Models.Customer;
using OrganizationMember = Team.Shared.Models.OrganizationMember;
using OrganizationMemberRole = Api.Shared.Services.Models.OrganizationMemberRole;
using Permissions = Api.Shared.Grpc.Skedular.Team.Core.V1.Permissions;
using TeamMember = Team.Shared.Models.TeamMember;
using TeamMemberStatus = Api.Shared.Services.Models.TeamMemberStatus;

namespace Team.Api.Mappers;

public interface IGrpcMapper
{
    global::Api.Shared.Grpc.Skedular.Team.Core.V1.Team MapToGrpcResponse(Shared.Models.Team src);
    TeamEdge MapToGrpcResponse(Edge<Shared.Models.Team> src);
    Shared.Models.Team MapTo(AddInput src);
    Shared.Models.Team MapTo(UpdateInput src);
}

public class GrpcMapper : IGrpcMapper
{
    public global::Api.Shared.Grpc.Skedular.Team.Core.V1.Team MapToGrpcResponse(Shared.Models.Team src)
    {
        var team = new global::Api.Shared.Grpc.Skedular.Team.Core.V1.Team
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

    public TeamEdge MapToGrpcResponse(Edge<Shared.Models.Team> src) =>
        new() { Cursor = src.Cursor, Node = MapToGrpcResponse(src.Node) };

    public Shared.Models.Team MapTo(AddInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name,
            About = src.About,
            Timezone = src.Timezone,
            FeatureImages = MapTo(src.FeatureImages).ToList(),
            Organization = new Organization { Id = src.OrganizationId },
            PrimaryLocation = string.IsNullOrWhiteSpace(src.PrimaryLocationId) ? null : new Location { Id = src.PrimaryLocationId },
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
            Organization = new Organization { Id = src.OrganizationId },
            PrimaryLocation = string.IsNullOrWhiteSpace(src.PrimaryLocationId) ? null : new Location { Id = src.PrimaryLocationId },
            TeamMembers = src.Members.Select(item => MapTo(item, new Shared.Models.Team { Id = src.Id })).ToList()
        };

    private static IEnumerable<global::Api.Shared.Grpc.Skedular.Team.Core.V1.TeamMember> MapToGrpcResponse(IEnumerable<TeamMember> src) =>
        src.Select(MapToGrpcResponse);

    private static global::Api.Shared.Grpc.Skedular.Team.Core.V1.TeamMember MapToGrpcResponse(TeamMember src) =>
        new()
        {
            Id = src.Id,
            Role = src.Role switch
            {
                TeamMemberRole.Owner => Role.Owner,
                TeamMemberRole.Administrator => Role.Administrator,
                TeamMemberRole.Member => Role.Member,
                _ => throw new ArgumentOutOfRangeException(nameof(src.Role), src.Role,
                    $"Unexpected value for {nameof(src.Role)}: {src.Role}. Update enum mapping or caller input.")
            },
            Status = src.Status switch
            {
                TeamMemberStatus.Active => global::Api.Shared.Grpc.Skedular.Team.Core.V1.TeamMemberStatus.Active,
                TeamMemberStatus.Inactive => global::Api.Shared.Grpc.Skedular.Team.Core.V1.TeamMemberStatus.Inactive,
                _ => throw new ArgumentOutOfRangeException(nameof(src.Status), src.Status,
                    $"Unexpected value for {nameof(src.Status)}: {src.Status}. Update enum mapping or caller input.")
            },
            CustomerId = src.Customer.Id.ToSafeString(),
            OrganizationMember = src.OrganizationMember is null || string.IsNullOrWhiteSpace(src.OrganizationMember.Id)
                ? null
                : new global::Api.Shared.Grpc.Skedular.Team.Core.V1.OrganizationMember
                {
                    Id = src.OrganizationMember.Id,
                    Role = src.OrganizationMember.Role switch
                    {
                        OrganizationMemberRole.Owner => Role.Owner,
                        OrganizationMemberRole.Administrator => Role.Administrator,
                        OrganizationMemberRole.Member => Role.Member,
                        _ => throw new ArgumentOutOfRangeException(nameof(src.OrganizationMember.Role), src.OrganizationMember.Role,
                            $"Unexpected value for {nameof(src.OrganizationMember.Role)}: {src.OrganizationMember.Role}. Update enum mapping or caller input.")
                    },
                    CustomerId = src.OrganizationMember.Customer.Id.ToSafeString()
                }
        };

    private static TeamMember MapTo(global::Api.Shared.Grpc.Skedular.Team.Core.V1.TeamMember src, Shared.Models.Team team) =>
        new()
        {
            Id = src.Id,
            Role = src.Role switch
            {
                Role.Owner => TeamMemberRole.Owner,
                Role.Administrator => TeamMemberRole.Administrator,
                Role.Member => TeamMemberRole.Member,
                _ => throw new ArgumentOutOfRangeException(nameof(src.Role), src.Role,
                    $"Unexpected value for {nameof(src.Role)}: {src.Role}. Update enum mapping or caller input.")
            },
            Status = src.Status switch
            {
                global::Api.Shared.Grpc.Skedular.Team.Core.V1.TeamMemberStatus.Active => TeamMemberStatus.Active,
                global::Api.Shared.Grpc.Skedular.Team.Core.V1.TeamMemberStatus.Inactive => TeamMemberStatus.Inactive,
                _ => throw new ArgumentOutOfRangeException(nameof(src.Status), src.Status,
                    $"Unexpected value for {nameof(src.Status)}: {src.Status}. Update enum mapping or caller input.")
            },
            Customer = new Customer { Id = src.CustomerId },
            OrganizationMember = src.OrganizationMember is null || string.IsNullOrWhiteSpace(src.OrganizationMember.Id)
                ? null
                : new OrganizationMember { Id = src.OrganizationMember.Id, Customer = new Customer { Id = src.OrganizationMember.CustomerId } },
            Team = team
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
