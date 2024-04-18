using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Organization.Api.Mappers;
using Organization.Shared.Models;
using Organization.Shared.Repositories;

namespace Organization.Api.Services;

public interface IIndustryMainCategoryService
{
    Task<ICollection<IndustryMainCategory>> GetAllAsync(CancellationToken cancellationToken);
}

public class IndustryMainCategoryService(IRepositoryFactory repositoryFactory, IMapper mapper, IMemoryCache memoryCache)
    : IIndustryMainCategoryService
{
    public async Task<ICollection<IndustryMainCategory>> GetAllAsync(CancellationToken cancellationToken) =>
        (await memoryCache.GetOrCreateAsync("organization-industry-main-categories",
            async cacheEntry =>
            {
                cacheEntry.SlidingExpiration = TimeSpan.FromHours(1);

                var industryMainCategories = await repositoryFactory.IndustryMainCategoryRepository
                    .Query(new Specification<Shared.Database.Entities.IndustryMainCategory>
                        {
                            Criteria = query => !query.DeletedAt.HasValue
                        }
                        .AddInclude(query => query.IndustrySubCategories)
                        .ApplyOrderBy(query => query.Name))
                    .AsNoTracking().ToListAsync(cancellationToken);

                return mapper.MapTo(industryMainCategories).ToList();
            }))!;
}
