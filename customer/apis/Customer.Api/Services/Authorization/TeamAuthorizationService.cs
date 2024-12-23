using Api.Shared.Models;
using Customer.Shared.Database.Entities;

namespace Customer.Api.Services.Authorization;

public interface ITeamAuthorizationService
{
    bool CanAddTeamAsDefault(Team team, Shared.Database.Entities.Customer customer);
}

public class TeamAuthorizationService : ITeamAuthorizationService
{
    public bool CanAddTeamAsDefault(Team team, Shared.Database.Entities.Customer customer) =>
        team.TeamMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            MembershipType: TeamMembershipType.Owner or TeamMembershipType.Administrator or TeamMembershipType.Member
        };
}
