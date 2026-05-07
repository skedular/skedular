using Microsoft.Extensions.Caching.Memory;
using Organization.Api.Mappers;
using Organization.Shared.Models;
using Organization.Shared.Repositories;

namespace Organization.Api.Services;

public interface IIndustryMainCategoryService
{
    Task<IReadOnlyList<IndustryMainCategory>> GetAllAsync(CancellationToken cancellationToken);
}

public class IndustryMainCategoryService(
    IRepositoryFactory repositoryFactory,
    IGraphQlMapper graphQlMapper,
    IMemoryCache memoryCache,
    TimeProvider timeProvider)
    : IIndustryMainCategoryService
{
    public async Task<IReadOnlyList<IndustryMainCategory>> GetAllAsync(CancellationToken cancellationToken) =>
        (await memoryCache.GetOrCreateAsync("organization-industry-main-categories",
            async cacheEntry =>
            {
                cacheEntry.AbsoluteExpiration = timeProvider.GetUtcNow().AddHours(1);

                var industryMainCategories =
                    await repositoryFactory.IndustryMainCategoryRepository.GetAllActiveWithSubCategoriesAsync(cancellationToken);

                return graphQlMapper.MapTo(industryMainCategories).ToList();
            }))!;
}
