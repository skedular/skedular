using Api.Shared.Clients.Events.Skedular.Marketplace.V1.Key;
using Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Models;
using Enterprise.Shared.Outbox.Publishers;
using Marketplace.Shared.Mappers;
using Event = Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.Event;
using Product = Marketplace.Shared.Models.Product;
using Type = Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.Type;

namespace Marketplace.Shared.Publishers;

public interface IMarketplaceOutboxPublisher
{
    void PublishProducts(IEnumerable<Product> products, IUnitOfWork unitOfWork);
}

public class MarketplaceOutboxPublisher(
    ApplicationConfiguration applicationConfiguration,
    IMapper mapper,
    IContext context,
    IOutboxEventPublisher<Key, Event> publisher)
    : IMarketplaceOutboxPublisher
{
    public void PublishProducts(IEnumerable<Product> products, IUnitOfWork unitOfWork)
    {
        foreach (var product in products)
        {
            publisher.Publish(
                new Key { ProductId = product.Id },
                new Event
                {
                    Metadata = Event.NewMetadata(
                        applicationConfiguration.DomainSource,
                        applicationConfiguration.AppSource,
                        product.IsNotDeleted() ? Type.ProductUpserted : Type.ProductDeleted,
                        context.GetCorrelationId()),
                    Data = new Data { Product = mapper.MapTo(product) }
                },
                unitOfWork);
        }
    }
}
