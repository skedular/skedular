using Marketplace.Api.Mappers;
using Marketplace.Shared.Models;
using Marketplace.Shared.Repositories;

namespace Marketplace.Api.Services;

public interface IProductVersionService
{
    Task<ProductVersion?> GetByIdAsync(string id, CancellationToken cancellationToken);
}

public class ProductVersionService(IRepositoryFactory repositoryFactory, IMapper mapper) : IProductVersionService
{
    public async Task<ProductVersion?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var existingProduct = await repositoryFactory.ProductVersionRepository.GetByIdAsync(id, cancellationToken);
        return existingProduct is null ? null : mapper.MapTo(existingProduct);
    }
}
