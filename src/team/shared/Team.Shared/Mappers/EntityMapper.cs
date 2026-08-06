using Api.Shared.Services.Models;
using Enterprise.Shared;
using HotChocolate.Types.Pagination;
using Customer = Team.Shared.Models.Customer;
using Identity = Team.Shared.Models.Identity;
using JoinInvitation = Team.Shared.Models.JoinInvitation;
using Location = Team.Shared.Database.Entities.Location;
using Organization = Team.Shared.Database.Entities.Organization;
using OrganizationMember = Team.Shared.Models.OrganizationMember;
using TeamMember = Team.Shared.Models.TeamMember;

namespace Team.Shared.Mappers;

public interface IEntityMapper
{
    TeamMember MapTo(Database.Entities.TeamMember src);
    Models.Team MapTo(Database.Entities.Team src);
    Database.Entities.Team MapTo(Models.Team src, Organization organization, Location? primaryLocation);

    Database.Entities.Team MergeTo(
        Models.Team src,
        Database.Entities.Team dest,
        Organization organization,
        Location? primaryLocation);

    TeamMember MapTo(Database.Entities.TeamMember src, Models.Team team);
    JoinInvitation MapTo(Database.Entities.JoinInvitation src);
    IEnumerable<Edge<TeamMember>> MapTo(IEnumerable<Edge<Database.Entities.TeamMember>> src, Models.Team team);
    Edge<JoinInvitation> MapTo(Edge<Database.Entities.JoinInvitation> src);
}

public class EntityMapper : IEntityMapper
{
    public TeamMember MapTo(Database.Entities.TeamMember src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Role = src.Role.ToTeamMemberRole(),
            Status = src.Status.ToTeamMemberStatus(),
            Customer = MapTo(src.Customer)!,
            OrganizationMember = MapTo(src.OrganizationMember),
        };

    public Models.Team MapTo(Database.Entities.Team src)
    {
        var team = new Models.Team
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
            PrimaryLocation = MapTo(src.PrimaryLocation),
        };

        team.TeamMembers = MapTo(src.TeamMembers, team).ToList();
        team.JoinInvitations = MapTo(src.JoinInvitations, team).ToList();

        return team;
    }

    public Database.Entities.Team MapTo(Models.Team src, Organization organization, Location? primaryLocation) =>
        MergeTo(src, new Database.Entities.Team(), organization, primaryLocation);

    public Database.Entities.Team MergeTo(
        Models.Team src,
        Database.Entities.Team dest,
        Organization organization,
        Location? primaryLocation)
    {
        dest.Id = src.Id;
        dest.Name = src.Name;
        dest.About = src.About;
        dest.Timezone = src.Timezone;
        dest.FeatureImages = src.FeatureImages.ToList();
        dest.Organization = organization;
        dest.PrimaryLocation = primaryLocation;
        return dest;
    }

    public TeamMember MapTo(Database.Entities.TeamMember src, Models.Team team) =>
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
            OrganizationMember = MapTo(src.OrganizationMember),
        };

    public JoinInvitation MapTo(Database.Entities.JoinInvitation src) =>
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
            Invitee = MapTo(src.Invitee),
        };

    public IEnumerable<Edge<TeamMember>> MapTo(IEnumerable<Edge<Database.Entities.TeamMember>> src, Models.Team team) =>
        src.Select(item => MapTo(item, team));

    public Edge<JoinInvitation> MapTo(Edge<Database.Entities.JoinInvitation> src) =>
        new(MapTo(src.Node), src.Cursor);

    private static Customer? MapTo(Database.Entities.Customer? src) =>
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
                Identities = MapTo(src.Identities).ToList(),
            };

    private IEnumerable<TeamMember> MapTo(IEnumerable<Database.Entities.TeamMember> src, Models.Team team) =>
        src.Select(item => MapTo(item, team));

    private static Models.Organization MapTo(Organization src) =>
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
            IsOwnershipVerified = src.IsOwnershipVerified,
        };

    private OrganizationMember? MapTo(Database.Entities.OrganizationMember? src) =>
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
                Organization = MapTo(src.Organization),
            };

    private static Models.Location? MapTo(Location? src) =>
        src is null
            ? null
            : new Models.Location
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                EventRaisedAt = src.EventRaisedAt,
            };

    private static IEnumerable<Identity> MapTo(IEnumerable<Database.Entities.Identity> src) =>
        src.Select(MapTo);

    private static Identity MapTo(Database.Entities.Identity src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            EventRaisedAt = src.EventRaisedAt,
            Email = src.Email,
            EmailVerified = src.EmailVerified,
        };

    private IEnumerable<JoinInvitation> MapTo(IEnumerable<Database.Entities.JoinInvitation> src, Models.Team team) =>
        src.Select(item => MapTo(item, team));

    private JoinInvitation MapTo(Database.Entities.JoinInvitation src, Models.Team team) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            Email = src.Email,
            Status = src.Status.ToInvitationStatus(),
            Team = team,
            CreatedBy = MapTo(src.CreatedBy)!,
            Invitee = MapTo(src.Invitee),
        };

    private Edge<TeamMember> MapTo(Edge<Database.Entities.TeamMember> src, Models.Team team) =>
        new(MapTo(src.Node, team), src.Cursor);
}
