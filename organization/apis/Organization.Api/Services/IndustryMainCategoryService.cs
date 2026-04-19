using Microsoft.Extensions.Caching.Memory;
using Organization.Api.Mappers;
using Organization.Shared.Models;
using Organization.Shared.Repositories;

namespace Organization.Api.Services;

public interface IIndustryMainCategoryService
{
    Task<ICollection<IndustryMainCategory>> GetAllAsync(CancellationToken cancellationToken);
}

public class IndustryMainCategoryService(IRepositoryFactory repositoryFactory, IMapper mapper, IMemoryCache memoryCache, TimeProvider timeProvider)
    : IIndustryMainCategoryService
{
    public async Task<ICollection<IndustryMainCategory>> GetAllAsync(CancellationToken cancellationToken) =>
        (await memoryCache.GetOrCreateAsync("organization-industry-main-categories",
            async cacheEntry =>
            {
                cacheEntry.AbsoluteExpiration = timeProvider.GetUtcNow().AddHours(1);

                var industryMainCategories = await repositoryFactory.IndustryMainCategoryRepository.GetAllActiveWithSubCategoriesAsync(cancellationToken);

                return mapper.MapTo(industryMainCategories).ToList();
            }))!;
}
