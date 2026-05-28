using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Organization.Shared.Database;
using Organization.Shared.Database.Entities;

namespace Organization.Shared.Repositories;

public interface IIndustrySubCategoryRepository : IRepository<IndustrySubCategory>
{
    Task<IReadOnlyList<IndustrySubCategory>> GetByIdsWithMainCategoryAsync(IReadOnlyList<string> ids, CancellationToken cancellationToken);
    Task<IReadOnlyList<IndustrySubCategory>> GetActiveByIdsWithMainCategoryAsync(IReadOnlyList<string> ids, CancellationToken cancellationToken);
}

public class IndustrySubCategoryRepository(OrganizationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<OrganizationDbContext, IndustrySubCategory>(dbContext, timeProvider),
        IIndustrySubCategoryRepository
{
    /// <summary>
    ///     Returns the requested industry subcategories together with their owning main category.
    /// </summary>
    /// <param name="ids">The industry subcategory identifiers to load.</param>
    /// <param name="cancellationToken">The cancellation token for the database query.</param>
    /// <returns>The matching subcategories with <c>IndustryMainCategory</c> populated.</returns>
    /// <remarks>
    ///     This repository lookup replaces the specification-based variant used when callers need the selected subcategories and their parent category in
    ///     one roundtrip.
    /// </remarks>
    public async Task<IReadOnlyList<IndustrySubCategory>> GetByIdsWithMainCategoryAsync(
        IReadOnlyList<string> ids,
        CancellationToken cancellationToken) =>
        await DbContext.IndustrySubCategory
            .Where(query => ids.Contains(query.Id))
            .Include(query => query.IndustryMainCategory)
            .ToListAsync(cancellationToken);

    /// <summary>
    ///     Returns the requested active industry subcategories together with their owning main category.
    /// </summary>
    /// <param name="ids">The industry subcategory identifiers to load.</param>
    /// <param name="cancellationToken">The cancellation token for the database query.</param>
    /// <returns>The matching non-deleted subcategories with <c>IndustryMainCategory</c> populated.</returns>
    /// <remarks>
    ///     This active-only form is used by update flows so deleted subcategories are filtered inside the repository without reintroducing shared query
    ///     composition.
    /// </remarks>
    public async Task<IReadOnlyList<IndustrySubCategory>> GetActiveByIdsWithMainCategoryAsync(
        IReadOnlyList<string> ids,
        CancellationToken cancellationToken) =>
        await DbContext.IndustrySubCategory
            .Where(query => !query.DeletedAt.HasValue && ids.Contains(query.Id))
            .Include(query => query.IndustryMainCategory)
            .ToListAsync(cancellationToken);
}
