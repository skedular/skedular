using Api.Shared.Services.Models;
using Enterprise.Shared;
using HotChocolate.Types.Pagination;
using Team.Api.GraphQL.Invitation;
using Team.Api.GraphQL.Member;
using Team.Api.GraphQL.Team;
using Team.Shared.Models;
using Customer = Team.Shared.Models.Customer;
using JoinInvitation = Team.Shared.Models.JoinInvitation;
using OrganizationMember = Team.Shared.Models.OrganizationMember;
using TeamEdge = Team.Api.GraphQL.Team.TeamEdge;
using TeamMember = Team.Shared.Models.TeamMember;

namespace Team.Api.Mappers;

public interface IGraphQlMapper
{
    TeamDetails? MapTo(Shared.Models.Team? src);
    TeamMemberDetails MapTo(TeamMember src);
    IEnumerable<TeamDetails> MapTo(IEnumerable<Shared.Models.Team> src);
    Shared.Models.Team MapTo(AddTeamInput src);
    Shared.Models.Team MapTo(UpdateTeamInput src);
    Shared.Models.Team MapTo(UpdateTeamAndTeamMembersInput src);
    TeamMember MapTo(AddTeamMemberInput src);
    IReadOnlyList<TeamMember> MapToTeamMembers(UpdateTeamMembersInput src);
    TeamEdge MapTo(Edge<Shared.Models.Team> src);
    IEnumerable<InviteCustomerToJoinTeamDetails> MapTo(IEnumerable<JoinInvitation> src);
    InviteCustomerToJoinTeamDetails MapTo(JoinInvitation src);
    TeamMemberEdge MapTo(Edge<TeamMember> src);
    TeamJoinInvitationEdge MapTo(Edge<JoinInvitation> src);
}

public class GraphQlMapper : IGraphQlMapper
{
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
                PrimaryLocationId = src.PrimaryLocation?.Id,
            };

    public TeamMemberDetails MapTo(TeamMember src) =>
        new()
        {
            Id = src.Id,
            Role = new TeamMemberRoleDetails
            {
                Type = src.Role,
                Name = src.Role.ToTeamMemberRoleName(),
            },
            Status = new TeamMemberStatusDetails
            {
                Type = src.Status,
                Name = src.Status.ToTeamMemberStatusName(),
            },
            CustomerId = src.Customer.Id,
            OrganizationMember = MapTo(src.OrganizationMember),
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
                new Organization
                {
                    Id = src.OrganizationId.ToSafeString(),
                    CustomDomain = src.OrganizationCustomDomain.ToSafeString(),
                },
            PrimaryLocation = string.IsNullOrWhiteSpace(src.PrimaryLocationId)
                ? null
                : new Location
                {
                    Id = src.PrimaryLocationId,
                },
            TeamMembers = src.CustomerIds
                .Select(item => new TeamMember
                {
                    Customer = new Customer
                    {
                        Id = item,
                    },
                })
                .Concat(src.OrganizationMemberIds.Select(item => new TeamMember
                {
                    OrganizationMember = new OrganizationMember
                    {
                        Id = item,
                    },
                }))
                .ToList(),
        };

    public Shared.Models.Team MapTo(UpdateTeamInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name,
            About = src.About,
            Timezone = src.Timezone,
            FeatureImages = src.FeatureImages.ToSafeCollection(),
            PrimaryLocation = string.IsNullOrWhiteSpace(src.PrimaryLocationId)
                ? null
                : new Location
                {
                    Id = src.PrimaryLocationId,
                },
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
                new Organization
                {
                    Id = src.OrganizationId.ToSafeString(),
                    CustomDomain = src.OrganizationCustomDomain.ToSafeString(),
                },
            PrimaryLocation = string.IsNullOrWhiteSpace(src.PrimaryLocationId)
                ? null
                : new Location
                {
                    Id = src.PrimaryLocationId,
                },
            TeamMembers = src.CustomerIds
                .Select(item => new TeamMember
                {
                    Customer = new Customer
                    {
                        Id = item,
                    },
                })
                .Concat(src.OrganizationMemberIds.Select(item => new TeamMember
                {
                    OrganizationMember = new OrganizationMember
                    {
                        Id = item,
                    },
                }))
                .ToList(),
        };

    public TeamMember MapTo(AddTeamMemberInput src)
    {
        var teamMember = new TeamMember
        {
            OrganizationMember = string.IsNullOrWhiteSpace(src.OrganizationMemberId)
                ? null
                : new OrganizationMember
                {
                    Id = src.OrganizationMemberId,
                },
        };

        if (!string.IsNullOrWhiteSpace(src.CustomerId))
        {
            teamMember.Customer = new Customer
            {
                Id = src.CustomerId,
            };
        }

        return teamMember;
    }

    public IReadOnlyList<TeamMember> MapToTeamMembers(UpdateTeamMembersInput src) =>
        src.CustomerIds
            .Select(item => new TeamMember
            {
                Customer = new Customer
                {
                    Id = item,
                },
            })
            .Concat(src.OrganizationMemberIds.Select(item => new TeamMember
            {
                OrganizationMember = new OrganizationMember
                {
                    Id = item,
                },
            }))
            .ToList();

    public TeamEdge MapTo(Edge<Shared.Models.Team> src) => new(MapTo(src.Node)!, src.Cursor);

    public IEnumerable<InviteCustomerToJoinTeamDetails> MapTo(IEnumerable<JoinInvitation> src) =>
        src.Select(MapTo);

    public InviteCustomerToJoinTeamDetails MapTo(JoinInvitation src) =>
        new()
        {
            Id = src.Id,
            Email = src.Email,
            Status = new TeamInvitationStatusDetails
            {
                Type = src.Status,
                Name = src.Status.ToInvitationStatusName(),
            },
            Role = src.Role,
            Team = MapTo(src.Team)!,
            CreatedById = src.CreatedBy.Id,
            InviteeId = src.Invitee?.Id,
        };

    public TeamMemberEdge MapTo(Edge<TeamMember> src) => new(MapTo(src.Node), src.Cursor);

    public TeamJoinInvitationEdge MapTo(Edge<JoinInvitation> src) => new(MapTo(src.Node), src.Cursor);

    private static TeamOrganizationMemberDetails? MapTo(OrganizationMember? src) =>
        src is null
            ? null
            : new TeamOrganizationMemberDetails
            {
                UniqueId = src.Id,
                CustomerId = src.Customer.Id,
            };
}
