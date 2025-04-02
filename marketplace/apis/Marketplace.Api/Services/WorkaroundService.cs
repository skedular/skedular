using Marketplace.Api.Mappers;
using Marketplace.Shared.Publishers;
using Marketplace.Shared.Repositories;

namespace Marketplace.Api.Services;

public interface IWorkaroundService
{
    Task RepublishAllOrganizationProductsAsync(string organizationId, CancellationToken cancellationToken);
    Task RepublishAllProductsAsync(CancellationToken cancellationToken);
}

public class WorkaroundService(IRepositoryFactory repositoryFactory, IMapper mapper, IMarketplacePublisher marketplacePublisher) : IWorkaroundService
{
    public async Task RepublishAllOrganizationProductsAsync(string organizationId, CancellationToken cancellationToken)
    {
        var products = await repositoryFactory.ProductRepository.GetAllByOrganizationIdAsync(organizationId, cancellationToken);

        await marketplacePublisher.PublishProductsAsync(products.Select(mapper.MapTo), cancellationToken);
    }

    public async Task RepublishAllProductsAsync(CancellationToken cancellationToken)
    {
        var products = await repositoryFactory.ProductRepository.GetAllAsync(cancellationToken);

        await marketplacePublisher.PublishProductsAsync(products.Select(mapper.MapTo), cancellationToken);
    }
}
