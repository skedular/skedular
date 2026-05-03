using Api.Shared.Clients.Events.Skedular.Marketplace.V1;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Kafka.Produce;
using Enterprise.Shared.Models;
using Marketplace.Shared.Mappers;
using Event = Api.Shared.Clients.Events.Skedular.Marketplace.V1.Event;
using Product = Marketplace.Shared.Models.Product;
using Type = Api.Shared.Clients.Events.Skedular.Marketplace.V1.Type;

namespace Marketplace.Shared.Publishers;

public interface IMarketplacePublisher
{
    Task PublishProductsAsync(IReadOnlyList<Product> products, CancellationToken cancellationToken);
}

public class MarketplacePublisher(
    ApplicationConfiguration applicationConfiguration,
    IMapper mapper,
    IContext context,
    IKafkaPublisher<Key, Event> publisher)
    : IMarketplacePublisher
{
    public async Task PublishProductsAsync(IReadOnlyList<Product> products, CancellationToken cancellationToken) =>
        await Task.WhenAll(products.Select(product => publisher.PublishAsync(
            new Key { ProductId = product.Id },
            new Event
            {
                Metadata = Event.NewMetadata(
                    applicationConfiguration.DomainSource,
                    applicationConfiguration.AppSource,
                    product.IsDeleted() ? Type.ProductDeleted : Type.ProductUpserted,
                    context.GetCorrelationId()),
                Data = new Data { Product = mapper.MapTo(product) }
            },
            cancellationToken)));
}
