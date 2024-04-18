using Api.Shared.Models;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Team.Shared.Database;
using Team.Shared.Database.Entities;

namespace Team.Shared.Repositories;

public interface IJoinInvitationRepository : IRepository<JoinInvitation>
{
    Task<JoinInvitation?> GetByIdAsync(string id, CancellationToken cancellationToken);

    Task<ICollection<JoinInvitation>> GetPendingByEmailAsync(
        ICollection<string> emails,
        CancellationToken cancellationToken);

    JoinInvitation Add(JoinInvitation joinInvitation);
    JoinInvitation Update(JoinInvitation joinInvitation);
    JoinInvitation Remove(JoinInvitation joinInvitation);
}

internal static class IJoinInvitationExtensions
{
    internal static IIncludableQueryable<JoinInvitation, Customer?> AddDependentObjects(
        this IQueryable<JoinInvitation> originalQuery) =>
        originalQuery
            .Include(query => query.Team)
            .Include(query => query.CreatedBy)
            .Include(query => query.Invitee);
}

public class JoinInvitationRepository(TeamDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<TeamDbContext, JoinInvitation>(dbContext), IJoinInvitationRepository
{
    public async Task<JoinInvitation?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.JoinInvitation
            .Where(query => query.Id == id)
            .AddDependentObjects()
            .OrderBy(query => query.Id)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<ICollection<JoinInvitation>> GetPendingByEmailAsync(
        ICollection<string> emails,
        CancellationToken cancellationToken) => await DbContext.JoinInvitation
        .Where(query => !query.DeletedAt.HasValue &&
                        query.Status == InvitationStatus.Pending && emails.Any(email =>
                            query.Invitee == null && query.Email != null && EF.Functions.ILike(query.Email, email)))
        .AddDependentObjects()
        .OrderBy(query => query.Id)
        .ToListAsync(cancellationToken);

    public JoinInvitation Add(JoinInvitation joinInvitation)
    {
        var now = timeProvider.GetUtcNow();
        joinInvitation.CreatedAt = now;
        return DbContext.JoinInvitation.Add(joinInvitation).Entity;
    }

    public JoinInvitation Update(JoinInvitation joinInvitation)
    {
        var now = timeProvider.GetUtcNow();
        joinInvitation.ModifiedAt = now;
        return DbContext.JoinInvitation.Update(joinInvitation).Entity;
    }

    public JoinInvitation Remove(JoinInvitation joinInvitation)
    {
        var now = timeProvider.GetUtcNow();
        joinInvitation.DeletedAt = now;
        return DbContext.JoinInvitation.Update(joinInvitation).Entity;
    }
}
