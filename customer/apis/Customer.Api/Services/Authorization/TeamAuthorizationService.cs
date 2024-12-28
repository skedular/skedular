using Api.Shared.Services.Models;
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
            MembershipType: TeamMembershipTypeConstants.Owner or TeamMembershipTypeConstants.Administrator
            or TeamMembershipTypeConstants.Member
        };
}
