using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;

namespace Booking.Shared.Repositories;

public interface ITeamMemberRepository : IRepository<TeamMember>
{
    TeamMember Add(TeamMember teamMember);
    TeamMember Update(TeamMember teamMember);
    void RemoveRange(ICollection<TeamMember> teamMembers);
    Task<ICollection<TeamMember>> GetByTeamIdAsync(string teamId, CancellationToken cancellationToken);
}

public class TeamMemberRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, TeamMember>(dbContext, timeProvider), ITeamMemberRepository
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

    public async Task<ICollection<TeamMember>> GetByTeamIdAsync(string teamId, CancellationToken cancellationToken) =>
        await DbContext.TeamMember
            .Where(query => query.Team.Id == teamId)
            .Include(query => query.Customer)
            .ToListAsync(cancellationToken);
}
