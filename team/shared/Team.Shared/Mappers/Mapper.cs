using Api.Shared.Clients.Events.Skedular.Team.V1.Value;
using Api.Shared.Services.Models;
using Enterprise.Shared;
using Google.Protobuf.WellKnownTypes;
using Team.Shared.Models;
using OrganizationMember = Api.Shared.Clients.Events.Skedular.Team.V1.Value.OrganizationMember;
using TeamMember = Api.Shared.Clients.Events.Skedular.Team.V1.Value.TeamMember;

namespace Team.Shared.Mappers;

public interface IMapper
{
    Api.Shared.Clients.Events.Skedular.Team.V1.Value.Team MapTo(Models.Team src);
    InvitationToJoinTeam MapTo(JoinInvitation src, string? inviteeIdToOverride);
}

public class Mapper : IMapper
{
    public Api.Shared.Clients.Events.Skedular.Team.V1.Value.Team MapTo(Models.Team src)
    {
        var team = new Api.Shared.Clients.Events.Skedular.Team.V1.Value.Team
        {
            Id = src.Id,
            DeletedAt = src.DeletedAt?.ToTimestamp(),
            Name = src.Name.ToSafeString(),
            About = src.About.ToSafeString(),
            Timezone = src.Timezone.ToSafeString(),
            OrganizationId = src.Organization.Id,
            PrimaryLocationId = src.PrimaryLocation is null ? string.Empty : src.PrimaryLocation.Id
        };

        team.Members.AddRange(src.TeamMembers.Select(item => new TeamMember
        {
            Id = item.Id,
            CustomerId = item.Customer.Id,
            Role = item.Role switch
            {
                TeamMemberRole.Owner => Role.Owner,
                TeamMemberRole.Administrator => Role.Administrator,
                TeamMemberRole.Member => Role.Member,
                _ => throw new ArgumentOutOfRangeException()
            },
            Status = item.Status switch
            {
                TeamMemberStatus.Active => Status.Active,
                TeamMemberStatus.Inactive => Status.Inactive,
                _ => throw new ArgumentOutOfRangeException()
            },
            OrganizationMember = item.OrganizationMember is null
                ? null
                : new OrganizationMember
                {
                    OrganizationMemberId = item.OrganizationMember.Id,
                    CustomerId = item.OrganizationMember.Customer.Id,
                    OrganizationId = item.OrganizationMember.Organization.Id
                }
        }));

        return team;
    }

    public InvitationToJoinTeam MapTo(JoinInvitation src, string? inviteeIdToOverride) =>
        new()
        {
            Id = src.Id,
            DeletedAt = src.DeletedAt?.ToTimestamp(),
            TeamId = src.Team.Id,
            InvitedById = src.CreatedBy.Id,
            InviteeId = inviteeIdToOverride ?? (src.Invitee is null ? string.Empty : src.Invitee.Id)
        };
}
