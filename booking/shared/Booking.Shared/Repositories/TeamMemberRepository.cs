using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared;
using Enterprise.Shared.Database;

namespace Booking.Shared.Repositories;

public interface ITeamMemberRepository : IRepository<TeamMember>
{
    TeamMember Add(TeamMember teamMember);
    TeamMember Update(TeamMember teamMember);
    void RemoveRange(ICollection<TeamMember> teamMembers);
}

public class TeamMemberRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, TeamMember>(dbContext), ITeamMemberRepository
{
    public TeamMember Add(TeamMember teamMember)
    {
        var now = timeProvider.GetUtcNow();
        teamMember.CreatedAt = now;
        return DbContext.TeamMember.Add(teamMember).Entity;
    }

    public void RemoveRange(ICollection<TeamMember> teamMembers)
    {
        var now = timeProvider.GetUtcNow();
        teamMembers.ForEach(teamMember => teamMember.DeletedAt = now);
        DbContext.TeamMember.RemoveRange(teamMembers);
    }

    public TeamMember Update(TeamMember teamMember)
    {
        var now = timeProvider.GetUtcNow();
        teamMember.ModifiedAt = now;
        return DbContext.TeamMember.Update(teamMember).Entity;
    }
}
