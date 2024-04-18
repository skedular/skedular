using Enterprise.Shared;
using Enterprise.Shared.Database;
using Location.Shared.Database;
using Location.Shared.Database.Entities;

namespace Location.Shared.Repositories;

public interface IOrganizationMemberRepository : IRepository<OrganizationMember>
{
    OrganizationMember Add(OrganizationMember organizationMember);
    OrganizationMember Update(OrganizationMember organizationMember);
    void RemoveRange(ICollection<OrganizationMember> organizationMembers);
}

public class OrganizationMemberRepository(LocationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<LocationDbContext, OrganizationMember>(dbContext), IOrganizationMemberRepository
{
    public OrganizationMember Add(OrganizationMember organizationMember)
    {
        var now = timeProvider.GetUtcNow();
        organizationMember.CreatedAt = now;
        return DbContext.OrganizationMember.Add(organizationMember).Entity;
    }

    public void RemoveRange(ICollection<OrganizationMember> organizationMembers)
    {
        var now = timeProvider.GetUtcNow();
        organizationMembers.ForEach(organizationMember => organizationMember.DeletedAt = now);
        DbContext.OrganizationMember.RemoveRange(organizationMembers);
    }

    public OrganizationMember Update(OrganizationMember organizationMember)
    {
        var now = timeProvider.GetUtcNow();
        organizationMember.ModifiedAt = now;
        return DbContext.OrganizationMember.Update(organizationMember).Entity;
    }
}
