using Marketplace.Api.Mappers;
using Marketplace.Shared.Models;
using Marketplace.Shared.Services.Cache;

namespace Marketplace.Api.Services;

public interface IProductVersionService
{
    Task<ProductVersion?> GetByIdAsync(string id, CancellationToken cancellationToken);
}

public class ProductVersionService(IMapper mapper, ICachedProductVersionService cachedProductVersionService) : IProductVersionService
{
    public async Task<ProductVersion?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var existingProduct = await cachedProductVersionService.GetByIdAsync(id, cancellationToken);
        return existingProduct is null ? null : mapper.MapTo(existingProduct, existingProduct.Product);
    }
}
