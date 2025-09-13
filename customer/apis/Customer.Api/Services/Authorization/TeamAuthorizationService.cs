using Api.Shared.Services.Models;
using Customer.Shared.Database.Entities;

namespace Customer.Api.Services.Authorization;

public interface ITeamAuthorizationService
{
    bool CanAddTeamAsDefault(Team team, string customerId);
}

public class TeamAuthorizationService : ITeamAuthorizationService
{
    public bool CanAddTeamAsDefault(Team team, string customerId) =>
        team.TeamMembers.SingleOrDefault(item => item.Customer.Id == customerId) is
        {
            Status: TeamMemberStatusConstants.Active,
            Role: TeamMemberRoleConstants.Owner or TeamMemberRoleConstants.Administrator or TeamMemberRoleConstants.Member
        };
}
