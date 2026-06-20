using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Organization.Shared.Database;
using Organization.Shared.Database.Entities;

namespace Organization.Shared.Repositories;

public interface IOrganizationOfferingActiveMemberRepository : IRepository<OrganizationOfferingActiveMember>
{
    Task<IReadOnlyList<OrganizationOfferingActiveMember>> GetByOfferingIdAsync(
        string organizationOfferingId,
        CancellationToken cancellationToken);

    Task ReplaceAsync(
        IReadOnlyList<OrganizationOfferingActiveMember> existingActiveMembers,
        IReadOnlyList<OrganizationOfferingActiveMember> activeMembers,
        CancellationToken cancellationToken);

    OrganizationOfferingActiveMember Add(OrganizationOfferingActiveMember organizationOfferingActiveMember);
    OrganizationOfferingActiveMember Update(OrganizationOfferingActiveMember organizationOfferingActiveMember);
}

public class OrganizationOfferingActiveMemberRepository(OrganizationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<OrganizationDbContext, OrganizationOfferingActiveMember>(dbContext, timeProvider),
        IOrganizationOfferingActiveMemberRepository
{
    public async Task<IReadOnlyList<OrganizationOfferingActiveMember>> GetByOfferingIdAsync(
        string organizationOfferingId,
        CancellationToken cancellationToken) =>
        await DbContext.OrganizationOfferingActiveMember
            .Include(item => item.OrganizationMember)
            .Where(item => item.OrganizationOffering.Id == organizationOfferingId)
            .ToListAsync(cancellationToken);

    public async Task ReplaceAsync(
        IReadOnlyList<OrganizationOfferingActiveMember> existingActiveMembers,
        IReadOnlyList<OrganizationOfferingActiveMember> activeMembers,
        CancellationToken cancellationToken)
    {
        DbContext.OrganizationOfferingActiveMember.RemoveRange(existingActiveMembers);
        await DbContext.OrganizationOfferingActiveMember.AddRangeAsync(activeMembers, cancellationToken);
    }

    public OrganizationOfferingActiveMember Add(OrganizationOfferingActiveMember organizationOfferingActiveMember)
    {
        var now = TimeProvider.GetUtcNow();
        organizationOfferingActiveMember.CreatedAt = now;
        return DbContext.OrganizationOfferingActiveMember.Add(organizationOfferingActiveMember).Entity;
    }

    public OrganizationOfferingActiveMember Update(OrganizationOfferingActiveMember organizationOfferingActiveMember)
    {
        var now = TimeProvider.GetUtcNow();
        organizationOfferingActiveMember.ModifiedAt = now;
        return DbContext.OrganizationOfferingActiveMember.Update(organizationOfferingActiveMember).Entity;
    }
}
