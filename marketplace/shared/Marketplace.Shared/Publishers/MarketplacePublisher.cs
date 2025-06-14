using Api.Shared.Clients.Events.Skedular.Marketplace.V1.Key;
using Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Kafka.Produce;
using Enterprise.Shared.Models;
using Marketplace.Shared.Mappers;
using Event = Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.Event;
using Product = Marketplace.Shared.Models.Product;
using Type = Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.Type;

namespace Marketplace.Shared.Publishers;

public interface IMarketplacePublisher
{
    Task PublishProductsAsync(IEnumerable<Product> products, CancellationToken cancellationToken);
}

public class MarketplacePublisher(
    ApplicationConfiguration applicationConfiguration,
    IMapper mapper,
    IContext context,
    IKafkaPublisher<Key, Event> publisher)
    : IMarketplacePublisher
{
    public async Task PublishProductsAsync(IEnumerable<Product> products, CancellationToken cancellationToken) =>
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
