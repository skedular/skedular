using Api.Shared.Clients.Events.Skedular.Marketplace.V1;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Models;
using Enterprise.Shared.Outbox.Kafka;
using Marketplace.Shared.Mappers;
using Event = Api.Shared.Clients.Events.Skedular.Marketplace.V1.Event;
using Product = Marketplace.Shared.Models.Product;
using Type = Api.Shared.Clients.Events.Skedular.Marketplace.V1.Type;

namespace Marketplace.Shared.Publishers;

public interface IMarketplaceOutboxPublisher
{
    void PublishProducts(IEnumerable<Product> products, IUnitOfWork unitOfWork);
}

public class MarketplaceOutboxPublisher(
    ApplicationConfiguration applicationConfiguration,
    IEventMapper eventMapper,
    IContext context,
    IKafkaOutboxEventPublisher<Key, Event> publisher) : IMarketplaceOutboxPublisher
{
    public void PublishProducts(IEnumerable<Product> products, IUnitOfWork unitOfWork)
    {
        foreach (var product in products)
        {
            publisher.Publish(
                new Key
                {
                    ProductId = product.Id,
                },
                new Event
                {
                    Metadata = Event.NewMetadata(
                        applicationConfiguration.DomainSource,
                        applicationConfiguration.AppSource,
                        product.IsDeleted() ? Type.ProductDeleted : Type.ProductUpserted,
                        context.GetCorrelationId()),
                    Data = new Data
                    {
                        Product = eventMapper.MapTo(product),
                    },
                },
                unitOfWork);
        }
    }
}
