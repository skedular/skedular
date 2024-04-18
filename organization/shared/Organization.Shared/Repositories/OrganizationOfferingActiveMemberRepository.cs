using Enterprise.Shared.Database;
using Organization.Shared.Database;
using Organization.Shared.Database.Entities;

namespace Organization.Shared.Repositories;

public interface IOrganizationOfferingActiveMemberRepository : IRepository<OrganizationOfferingActiveMember>
{
    OrganizationOfferingActiveMember Add(OrganizationOfferingActiveMember organizationOfferingActiveMember);
    OrganizationOfferingActiveMember Update(OrganizationOfferingActiveMember organizationOfferingActiveMember);
}

public class OrganizationOfferingActiveMemberRepository(OrganizationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<OrganizationDbContext, OrganizationOfferingActiveMember>(dbContext),
        IOrganizationOfferingActiveMemberRepository
{
    public OrganizationOfferingActiveMember Add(OrganizationOfferingActiveMember organizationOfferingActiveMember)
    {
        var now = timeProvider.GetUtcNow();
        organizationOfferingActiveMember.CreatedAt = now;
        return DbContext.OrganizationOfferingActiveMember.Add(organizationOfferingActiveMember).Entity;
    }

    public OrganizationOfferingActiveMember Update(OrganizationOfferingActiveMember organizationOfferingActiveMember)
    {
        var now = timeProvider.GetUtcNow();
        organizationOfferingActiveMember.ModifiedAt = now;
        return DbContext.OrganizationOfferingActiveMember.Update(organizationOfferingActiveMember).Entity;
    }
}
