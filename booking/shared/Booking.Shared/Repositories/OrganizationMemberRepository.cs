using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Microsoft.EntityFrameworkCore;

namespace Booking.Shared.Repositories;

public interface IOrganizationMemberRepository : IRepository<OrganizationMember>
{
    Task<IReadOnlyList<OrganizationMember>> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken);
    Task<IReadOnlyList<OrganizationMember>> GetByOrganizationIdAsync(string organizationId, CancellationToken cancellationToken);
    OrganizationMember Add(OrganizationMember organizationMember);
    OrganizationMember Update(OrganizationMember organizationMember);
    void RemoveRange(IEnumerable<OrganizationMember> organizationMembers);
}

public class OrganizationMemberRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, OrganizationMember>(dbContext, timeProvider), IOrganizationMemberRepository
{
    public async Task<IReadOnlyList<OrganizationMember>> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken) =>
        await DbContext.OrganizationMember
            .Where(query => !query.DeletedAt.HasValue &&
                            query.Customer.Id == customerId &&
                            !query.Organization.DeletedAt.HasValue)
            .AsSingleQuery()
            .Include(query => query.Organization)
            .Include(query => query.Customer)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<OrganizationMember>> GetByOrganizationIdAsync(string organizationId, CancellationToken cancellationToken) =>
        await DbContext.OrganizationMember
            .Where(query => query.Organization.Id == organizationId)
            .AsSingleQuery()
            .Include(query => query.Customer)
            .ToListAsync(cancellationToken);

    public OrganizationMember Add(OrganizationMember organizationMember)
    {
        var now = TimeProvider.GetUtcNow();
        organizationMember.CreatedAt = now;
        return DbContext.OrganizationMember.Add(organizationMember).Entity;
    }

    public void RemoveRange(IEnumerable<OrganizationMember> organizationMembers)
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
}
