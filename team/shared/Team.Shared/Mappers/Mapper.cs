using Api.Shared.Clients.Events.Skedular.Team.V1.Value;
using Api.Shared.Models;
using Enterprise.Shared;
using Team.Shared.Models;
using OrganizationMember = Api.Shared.Clients.Events.Skedular.Team.V1.Value.OrganizationMember;

namespace Team.Shared.Mappers;

public interface IMapper
{
    Api.Shared.Clients.Events.Skedular.Team.V1.Value.Team MapTo(Models.Team src);
    public InvitationToJoinTeam MapTo(JoinInvitation src, string? inviteeIdToOverride);
}

public class Mapper : IMapper
{
    public Api.Shared.Clients.Events.Skedular.Team.V1.Value.Team MapTo(Models.Team src)
    {
        var team = new Api.Shared.Clients.Events.Skedular.Team.V1.Value.Team
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            About = src.About.ToSafeString(),
            Timezone = src.Timezone.ToSafeString(),
            OrganizationId = src.Organization is null ? string.Empty : src.Organization.Id,
            PrimaryLocationId = src.PrimaryLocation is null ? string.Empty : src.PrimaryLocation.Id
        };

        team.Members.AddRange(src.TeamMembers.Select(item =>
        {
            return new Member
            {
                Id = item.Id,
                CustomerId = item.Customer.Id,
                MembershipType = item.MembershipType switch
                {
                    TeamMembershipType.Owner => MembershipType.Owner,
                    TeamMembershipType.Administrator => MembershipType.Administrator,
                    TeamMembershipType.Member => MembershipType.Member,
                    _ => throw new ArgumentOutOfRangeException()
                },
                OrganizationMember = item.OrganizationMember is null
                    ? null
                    : new OrganizationMember
                    {
                        OrganizationMemberId = item.OrganizationMember.Id,
                        CustomerId = item.OrganizationMember.Customer!.Id,
                        OrganizationId = item.OrganizationMember.Organization!.Id
                    }
            };
        }));

        return team;
    }

    public InvitationToJoinTeam MapTo(JoinInvitation src, string? inviteeIdToOverride) =>
        new()
        {
            Id = src.Id,
            TeamId = src.Team.Id,
            InvitedById = src.CreatedBy.Id,
            InviteeId = inviteeIdToOverride ?? (src.Invitee is null ? string.Empty : src.Invitee.Id)
        };
}
