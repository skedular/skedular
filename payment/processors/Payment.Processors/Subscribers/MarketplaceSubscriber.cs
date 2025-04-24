using Api.Shared.Clients.Events.Skedular.Marketplace.V1.Key;
using Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Kafka.Consume;
using Payment.Shared.Database.Entities;
using Payment.Shared.Repositories;
using IMapper = Payment.Processors.Mappers.IMapper;
using Product = Payment.Shared.Models.Product;
using ProductVersion = Payment.Shared.Database.Entities.ProductVersion;
using Type = Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.Type;

namespace Payment.Processors.Subscribers;

public class MarketplaceSubscriber(ILogger<MarketplaceSubscriber> logger, IMapper mapper, IRepositoryFactory repositoryFactory)
    : IEventSubscriber<Key, Event>
{
    public async Task<EventSubscriberResult> HandleAsync(EventContext eventContext, Key key, Event @event, CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.ProductUpserted:
                {
                    ArgumentException.ThrowIfNullOrWhiteSpace(@event.Data.Product.OrganizationId);

                    var product = mapper.MapTo(@event);
                    var account = product.ProductVersions.Single().OrganizationStripeConnectAccount;
                    OrganizationStripeConnectAccount? accountEntity = null;
                    if (account is not null)
                    {
                        accountEntity =
                            await repositoryFactory.OrganizationStripeConnectAccountRepository.GetByIdAsync(account.Id, cancellationToken);
                        if (accountEntity is null)
                        {
                            throw new OrganizationStripeConnectAccountNotFound();
                        }
                    }

                    var organization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync(product.Organization.Id, cancellationToken);
                    var existingProduct = await repositoryFactory.ProductRepository.UpsertNakedAsync(product.Id, organization, cancellationToken);
                    if (existingProduct.EventRaisedAt > product.EventRaisedAt)
                    {
                        logger.LogInformation("Ignoring Product event. Event timestamp is older that what is already processed.");

                        return EventSubscriberResults.Success;
                    }

                    await HandleProductUpsertedEventAsync(
                        product,
                        existingProduct,
                        organization,
                        accountEntity,
                        cancellationToken);
                }
                break;

            case Type.ProductDeleted:
                {
                    var product = mapper.MapTo(@event);
                    var existingProduct = await repositoryFactory.ProductRepository.GetByIdAsync(product.Id, cancellationToken);
                    if (existingProduct is not null && existingProduct.EventRaisedAt > product.EventRaisedAt)
                    {
                        logger.LogInformation("Ignoring Product event. Event timestamp is older that what is already processed.");

                        return EventSubscriberResults.Success;
                    }

                    if (existingProduct is null)
                    {
                        return EventSubscriberResults.Success;
                    }

                    await HandleProductDeletedEventAsync(existingProduct, cancellationToken);
                }
                break;
        }

        return EventSubscriberResults.Success;
    }

    private async Task HandleProductUpsertedEventAsync(
        Product product,
        Shared.Database.Entities.Product existingProduct,
        Organization organization,
        OrganizationStripeConnectAccount? accountEntity,
        CancellationToken cancellationToken)
    {
        var productVersions = new List<ProductVersion>();
        foreach (var productVersion in product.ProductVersions)
        {
            var productVersionEntity =
                await repositoryFactory.ProductVersionRepository.UpsertNakedAsync(productVersion.Id, existingProduct, cancellationToken);

            productVersions.Add(
                repositoryFactory.ProductVersionRepository.Update(
                    mapper.MergeToEntity(productVersion, productVersionEntity, existingProduct, accountEntity)));
        }

        _ = repositoryFactory.ProductRepository.Update(mapper.MergeToEntity(product, existingProduct, organization, productVersions));

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleProductDeletedEventAsync(Shared.Database.Entities.Product existingProduct, CancellationToken cancellationToken)
    {
        _ = repositoryFactory.ProductRepository.Remove(existingProduct);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
