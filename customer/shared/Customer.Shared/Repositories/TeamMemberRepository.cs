using Customer.Shared.Database;
using Customer.Shared.Database.Entities;
using Enterprise.Shared;
using Enterprise.Shared.Database;

namespace Customer.Shared.Repositories;

public interface ITeamMemberRepository : IRepository<TeamMember>
{
    TeamMember Add(TeamMember teamMember);
    TeamMember Update(TeamMember teamMember);
    void RemoveRange(ICollection<TeamMember> teamMembers);
}

public class TeamMemberRepository(CustomerDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<CustomerDbContext, TeamMember>(dbContext, timeProvider), ITeamMemberRepository
{
    public TeamMember Add(TeamMember teamMember)
    {
        var now = TimeProvider.GetUtcNow();
        teamMember.CreatedAt = now;
        return DbContext.TeamMember.Add(teamMember).Entity;
    }

    public void RemoveRange(ICollection<TeamMember> teamMembers)
    {
        var now = TimeProvider.GetUtcNow();
        teamMembers.ForEach(teamMember => teamMember.DeletedAt = now);
        DbContext.TeamMember.UpdateRange(teamMembers);
    }

    public TeamMember Update(TeamMember teamMember)
    {
        var now = TimeProvider.GetUtcNow();
        teamMember.ModifiedAt = now;
        return DbContext.TeamMember.Update(teamMember).Entity;
    }
}
