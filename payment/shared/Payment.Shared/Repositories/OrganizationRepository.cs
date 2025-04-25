using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Payment.Shared.Database;
using Payment.Shared.Database.Entities;

namespace Payment.Shared.Repositories;

public interface IOrganizationRepository : IRepository<Organization>
{
    Task<Organization> UpsertNakedAsync(string id, CancellationToken cancellationToken);

    Task<Organization?> GetByIdAsync(
        string id,
        bool includeDeletedOrganizationMembers,
        bool includeAllOfferings,
        CancellationToken cancellationToken);

    Task<ICollection<Organization>> GetByIdsAsync(ICollection<string> ids, CancellationToken cancellationToken);
    Organization Add(Organization organization);
    Organization Update(Organization organization);
    Organization Remove(Organization organization);
}

internal static class OrganizationExtensions
{
    internal static IIncludableQueryable<Organization, StripePaymentMethod>
        AddDependentObjects(
            this IQueryable<Organization> originalQuery,
            bool includeDeletedOrganizationMembers,
            bool includeAllOfferings)
    {
        var updatedQuery = originalQuery
            .Include(query => query.OrganizationSsoSettings)
            .Include(query => query.StripeCustomer)
            .Include(query => query.Products)
            .ThenInclude(query => query.ProductVersions)
            .Include(query => query.PhysicalAddress)
            .Include(query =>
                query.OrganizationMembers.Where(organizationMember => includeDeletedOrganizationMembers || !organizationMember.DeletedAt.HasValue))
            .ThenInclude(query => query.Customer)
            .ThenInclude(query => query.Identities)
            .Include(query =>
                query.StripePaymentMethods.Where(organizationStripePaymentMethod => !organizationStripePaymentMethod.DeletedAt.HasValue));

        return includeAllOfferings
            ? updatedQuery
                .Include(query => query.OrganizationOfferings.OrderByDescending(organizationOffering => organizationOffering.End))
                .ThenInclude(query => query.StripePaymentIntent)
                .ThenInclude(query => query.StripePaymentMethod)
            : updatedQuery
                .Include(query => query.OrganizationOfferings
                    .Where(organizationOffering => !organizationOffering.DeletedAt.HasValue)
                    .OrderByDescending(organizationOffering => organizationOffering.End)
                    .Take(1))
                .ThenInclude(query => query.StripePaymentIntent)
                .ThenInclude(query => query.StripePaymentMethod);
    }
}

public class OrganizationRepository(PaymentDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<PaymentDbContext, Organization>(dbContext, timeProvider), IOrganizationRepository
{
    public override async Task<Organization> UpsertNakedAsync(string id, CancellationToken cancellationToken)
    {
        await base.UpsertNakedAsync(id, cancellationToken);

        return (await GetByIdAsync(id, true, true, cancellationToken))!;
    }

    public async Task<Organization?> GetByIdAsync(
        string id,
        bool includeDeletedOrganizationMembers,
        bool includeAllOfferings,
        CancellationToken cancellationToken) =>
        await DbContext.Organization
            .AddDependentObjects(includeDeletedOrganizationMembers, includeAllOfferings)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<ICollection<Organization>> GetByIdsAsync(ICollection<string> ids, CancellationToken cancellationToken) =>
        await DbContext.Organization
            .Where(query => ids.Contains(query.Id))
            .AddDependentObjects(false, false)
            .ToListAsync(cancellationToken);

    public Organization Add(Organization organization)
    {
        var now = TimeProvider.GetUtcNow();
        organization.CreatedAt = now;
        return DbContext.Organization.Add(organization).Entity;
    }

    public Organization Update(Organization organization)
    {
        var now = TimeProvider.GetUtcNow();
        organization.ModifiedAt = now;
        return DbContext.Organization.Update(organization).Entity;
    }

    public Organization Remove(Organization organization)
    {
        var now = TimeProvider.GetUtcNow();
        organization.DeletedAt = now;
        return DbContext.Organization.Update(organization).Entity;
    }
}
