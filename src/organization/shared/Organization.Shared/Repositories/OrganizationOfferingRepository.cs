using Api.Shared.Services.Offering;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Organization.Shared.Database;
using Organization.Shared.Database.Entities;

namespace Organization.Shared.Repositories;

public interface IOrganizationOfferingRepository : IRepository<OrganizationOffering>
{
    Task<OrganizationOffering?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<IReadOnlyList<OrganizationOffering>> GetActiveOfferingsAsync(CancellationToken cancellationToken);

    Task<OrganizationOffering?> GetCurrentActiveByOrganizationIdAsync(
        string organizationId,
        DateTimeOffset at,
        CancellationToken cancellationToken);

    Task<OrganizationOffering?> GetCurrentByOrganizationIdAndCodeAsync(
        string organizationId,
        OfferingCode offeringCode,
        DateTimeOffset at,
        bool includeDeleted,
        CancellationToken cancellationToken);

    Task<OrganizationOffering?> GetCurrentByCustomDomainAndCodeAsync(
        string customDomain,
        OfferingCode offeringCode,
        DateTimeOffset at,
        bool includeDeleted,
        CancellationToken cancellationToken);

    void Add(OrganizationOffering organizationOffering);
    void Remove(OrganizationOffering organizationOffering);
    void Undelete(OrganizationOffering organizationOffering);
    void Update(OrganizationOffering organizationOffering);
}

public class OrganizationOfferingRepository(OrganizationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<OrganizationDbContext, OrganizationOffering>(dbContext, timeProvider), IOrganizationOfferingRepository
{
    public async Task<OrganizationOffering?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.OrganizationOffering
            .Include(query => query.Organization)
            .ThenInclude(query => query.OrganizationStripeCustomer)
            .Include(query => query.Organization)
            .ThenInclude(query =>
                query.OrganizationStripePaymentMethods.Where(organizationStripePaymentMethod => !organizationStripePaymentMethod.DeletedAt.HasValue))
            .Include(query => query.OrganizationOfferingActiveMembers)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<IReadOnlyList<OrganizationOffering>> GetActiveOfferingsAsync(CancellationToken cancellationToken) =>
        await DbContext.OrganizationOffering
            .Where(query => !query.DeletedAt.HasValue)
            .Include(query => query.Organization)
            .ToListAsync(cancellationToken);

    /// <summary>
    ///     Returns the current active offering for the supplied organization at the specified point in time.
    /// </summary>
    /// <param name="organizationId">The organization identifier whose current offering should be resolved.</param>
    /// <param name="at">The point in time that must fall within the offering start and end range.</param>
    /// <param name="cancellationToken">The cancellation token for the database query.</param>
    /// <returns>The current active offering for the organization, or <see langword="null" /> when none is active at the requested time.</returns>
    /// <remarks>
    ///     This replaces the shared specification used by payment flows when they only need the currently effective offering for one organization.
    /// </remarks>
    public async Task<OrganizationOffering?> GetCurrentActiveByOrganizationIdAsync(
        string organizationId,
        DateTimeOffset at,
        CancellationToken cancellationToken) =>
        await DbContext.OrganizationOffering
            .Where(query => !query.DeletedAt.HasValue && query.Organization.Id == organizationId && query.Start <= at && query.End >= at)
            .OrderBy(query => query.Id)
            .FirstOrDefaultAsync(cancellationToken);

    /// <summary>
    ///     Returns the offering with the requested code for a specific organization at the supplied point in time.
    /// </summary>
    /// <param name="organizationId">The organization identifier whose offering should be resolved.</param>
    /// <param name="offeringCode">The offering code to match.</param>
    /// <param name="at">The point in time that must fall within the offering start and end range.</param>
    /// <param name="includeDeleted">Whether soft-deleted offerings should still be considered.</param>
    /// <param name="cancellationToken">The cancellation token for the database query.</param>
    /// <returns>The matching offering, or <see langword="null" /> when no offering satisfies the criteria.</returns>
    /// <remarks>
    ///     This variant keeps the organization-id lookup explicit so offering updates can include deleted rows when they are reviving or replacing an
    ///     existing record.
    /// </remarks>
    public async Task<OrganizationOffering?> GetCurrentByOrganizationIdAndCodeAsync(
        string organizationId,
        OfferingCode offeringCode,
        DateTimeOffset at,
        bool includeDeleted,
        CancellationToken cancellationToken) =>
        await DbContext.OrganizationOffering
            .Where(query =>
                (includeDeleted || !query.DeletedAt.HasValue) &&
                query.Organization.Id == organizationId &&
                query.Code == offeringCode &&
                query.Start <= at &&
                query.End >= at)
            .OrderBy(query => query.Id)
            .FirstOrDefaultAsync(cancellationToken);

    /// <summary>
    ///     Returns the offering with the requested code for a specific custom domain at the supplied point in time.
    /// </summary>
    /// <param name="customDomain">The organization custom domain whose offering should be resolved.</param>
    /// <param name="offeringCode">The offering code to match.</param>
    /// <param name="at">The point in time that must fall within the offering start and end range.</param>
    /// <param name="includeDeleted">Whether soft-deleted offerings should still be considered.</param>
    /// <param name="cancellationToken">The cancellation token for the database query.</param>
    /// <returns>The matching offering, or <see langword="null" /> when no offering satisfies the criteria.</returns>
    /// <remarks>
    ///     This mirrors the organization-id lookup for custom-domain callers so the service layer does not need another specification object.
    /// </remarks>
    public async Task<OrganizationOffering?> GetCurrentByCustomDomainAndCodeAsync(
        string customDomain,
        OfferingCode offeringCode,
        DateTimeOffset at,
        bool includeDeleted,
        CancellationToken cancellationToken) =>
        await DbContext.OrganizationOffering
            .Where(query =>
                (includeDeleted || !query.DeletedAt.HasValue) &&
                query.Organization.CustomDomain == customDomain &&
                query.Code == offeringCode &&
                query.Start <= at &&
                query.End >= at)
            .OrderBy(query => query.Id)
            .FirstOrDefaultAsync(cancellationToken);

    public void Add(OrganizationOffering organizationOffering)
    {
        var now = TimeProvider.GetUtcNow();
        organizationOffering.CreatedAt = now;
        DbContext.OrganizationOffering.Add(organizationOffering);
    }

    public void Remove(OrganizationOffering organizationOffering)
    {
        var now = TimeProvider.GetUtcNow();
        organizationOffering.DeletedAt = now;
        DbContext.OrganizationOffering.Update(organizationOffering);
    }

    public void Undelete(OrganizationOffering organizationOffering)
    {
        organizationOffering.DeletedAt = null;
        DbContext.OrganizationOffering.Update(organizationOffering);
    }

    public void Update(OrganizationOffering organizationOffering)
    {
        var now = TimeProvider.GetUtcNow();
        organizationOffering.ModifiedAt = now;
        DbContext.OrganizationOffering.Update(organizationOffering);
    }
}
