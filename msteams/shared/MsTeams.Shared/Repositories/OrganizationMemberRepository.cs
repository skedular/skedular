using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.Postgres;
using Microsoft.EntityFrameworkCore;
using MsTeams.Shared.Database;
using MsTeams.Shared.Database.Entities;

namespace MsTeams.Shared.Repositories;

public interface IOrganizationMemberRepository : IRepository<OrganizationMember>
{
    OrganizationMember Add(OrganizationMember organizationMember);
    OrganizationMember Update(OrganizationMember organizationMember);
    void RemoveRange(ICollection<OrganizationMember> organizationMembers);

    Task<ICollection<OrganizationMember>> GetByOrganizationIdAsync(
        string organizationId,
        CancellationToken cancellationToken);
}

public class OrganizationMemberRepository(MsTeamsDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<MsTeamsDbContext, OrganizationMember>(dbContext, timeProvider), IOrganizationMemberRepository
{
    public OrganizationMember Add(OrganizationMember organizationMember)
    {
        var now = TimeProvider.GetUtcNow();
        organizationMember.CreatedAt = now;
        return DbContext.OrganizationMember.Add(organizationMember).Entity;
    }

    public void RemoveRange(ICollection<OrganizationMember> organizationMembers)
    {
        var now = TimeProvider.GetUtcNow();
        organizationMembers.ForEach(organizationMember => organizationMember.DeletedAt = now);
        DbContext.OrganizationMember.UpdateRange(organizationMembers);
    }

    public OrganizationMember Update(OrganizationMember organizationMember)
    {
        var now = TimeProvider.GetUtcNow();
        organizationMember.ModifiedAt = now;
        return DbContext.OrganizationMember.Update(organizationMember).Entity;
    }

    public async Task<ICollection<OrganizationMember>> GetByOrganizationIdAsync(
        string organizationId,
        CancellationToken cancellationToken) =>
        await DbContext.OrganizationMember
            .Where(query => query.Organization.Id == organizationId)
            .AsSingleQuery()
            .Include(query => query.Customer)
            .ToListAsync(cancellationToken);
}
