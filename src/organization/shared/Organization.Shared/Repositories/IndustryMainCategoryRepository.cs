using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Organization.Shared.Database;
using Organization.Shared.Database.Entities;

namespace Organization.Shared.Repositories;

public interface IIndustryMainCategoryRepository : IRepository<IndustryMainCategory>
{
    Task<IReadOnlyList<IndustryMainCategory>> GetAllActiveWithSubCategoriesAsync(CancellationToken cancellationToken);
}

public class IndustryMainCategoryRepository(OrganizationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<OrganizationDbContext, IndustryMainCategory>(dbContext, timeProvider),
        IIndustryMainCategoryRepository
{
    /// <summary>
    ///     Returns all active industry main categories together with their subcategories.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token for the database query.</param>
    /// <returns>The active main categories ordered by name with <c>IndustrySubCategories</c> populated.</returns>
    /// <remarks>
    ///     This keeps the category and subcategory read inside the repository and preserves the original untracked behavior that existed before the shared
    ///     specification was removed.
    /// </remarks>
    public async Task<IReadOnlyList<IndustryMainCategory>> GetAllActiveWithSubCategoriesAsync(CancellationToken cancellationToken) =>
        await DbContext.IndustryMainCategory
            .AsNoTrackingWithIdentityResolution()
            .Where(query => !query.DeletedAt.HasValue)
            .Include(query => query.IndustrySubCategories)
            .OrderBy(query => query.Name)
            .ToListAsync(cancellationToken);
}
