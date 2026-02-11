using Api.Shared.Services;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;

namespace Booking.Shared.Services;

public interface IProductService
{
    Task<ICollection<ProductVersion>> GetProductVersionsAsync(List<string> productVersionIds, CancellationToken cancellationToken);
}

public class ProductService(IRepositoryFactory repositoryFactory) : IProductService
{
    public async Task<ICollection<ProductVersion>> GetProductVersionsAsync(List<string> productVersionIds, CancellationToken cancellationToken)
    {
        if (productVersionIds.Count == 0)
        {
            return [];
        }

        productVersionIds = productVersionIds.Distinct().ToList();
        var productVersions = await repositoryFactory.ProductVersionRepository.GetByIdsAsync(productVersionIds, cancellationToken);

        return productVersions.Count != productVersionIds.Count || !productVersions.All(item => productVersionIds.Contains(item.Id))
            ? throw new ProductNotFound()
            : productVersions;
    }
}
