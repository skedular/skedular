using Api.Shared.Clients.Events.Skedular.Marketplace.V1.Key;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Kafka.Consume;
using Enterprise.Shared.Random;
using Payment.Shared.Database.Entities;
using Payment.Shared.Repositories;
using Stripe;
using Event = Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.Event;
using IMapper = Payment.Processors.Mappers.IMapper;
using Product = Payment.Shared.Models.Product;
using ProductVersion = Payment.Shared.Database.Entities.ProductVersion;
using Type = Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.Type;

namespace Payment.Processors.Subscribers;

public class MarketplaceSubscriber(
    ILogger<MarketplaceSubscriber> logger,
    IMapper mapper,
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    ICreatable<Stripe.Product, ProductCreateOptions> productCreateService,
    IUpdatable<Stripe.Product, ProductUpdateOptions> productUpdateService,
    ICreatable<Price, PriceCreateOptions> priceCreateService)
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
                    StripeConnectAccount? accountEntity = null;
                    if (account is not null)
                    {
                        accountEntity =
                            await repositoryFactory.StripeConnectAccountRepository.GetByIdAsync(account.Id, cancellationToken);
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
                        @event,
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
        Event @event,
        Product product,
        Shared.Database.Entities.Product existingProduct,
        Organization organization,
        StripeConnectAccount? accountEntity,
        CancellationToken cancellationToken)
    {
        var productVersions = new List<ProductVersion>();
        foreach (var productVersion in product.ProductVersions)
        {
            var existingProductVersionEntity = await repositoryFactory.ProductVersionRepository.GetByIdAsync(productVersion.Id, cancellationToken);
            if (existingProductVersionEntity is null)
            {
                existingProductVersionEntity = mapper.MapToEntity(productVersion, existingProduct, accountEntity);

                if (accountEntity is not null)
                {
                    var stripeProduct = await productCreateService.CreateAsync(
                        mapper.MapToProduct(productVersion, product, organization.Id),
                        new RequestOptions { IdempotencyKey = productVersion.Id, StripeAccount = accountEntity.StripeAccountId },
                        cancellationToken);

                    var stripeProductEntity = repositoryFactory.StripeProductRepository.Add(new StripeProduct
                    {
                        Id = randomHelper.Generate(), StripeProductId = stripeProduct.Id
                    });

                    var stripePrice = await priceCreateService.CreateAsync(
                        mapper.MapToPrice(productVersion, product, organization.Id, stripeProduct.Id),
                        new RequestOptions { IdempotencyKey = @event.Metadata.Id, StripeAccount = accountEntity.StripeAccountId },
                        cancellationToken);

                    var stripePriceEntity = repositoryFactory.StripePriceRepository.Add(new StripePrice
                    {
                        Id = randomHelper.Generate(), StripePriceId = stripePrice.Id
                    });

                    existingProductVersionEntity.StripeProduct = stripeProductEntity;
                    existingProductVersionEntity.StripePrice = stripePriceEntity;
                }

                existingProductVersionEntity = repositoryFactory.ProductVersionRepository.Add(existingProductVersionEntity);
            }
            else
            {
                existingProductVersionEntity = mapper.MergeToEntity(productVersion, existingProductVersionEntity, existingProduct, accountEntity);
                if (existingProductVersionEntity.StripeProduct is null)
                {
                    if (accountEntity is not null)
                    {
                        var stripeProduct = await productCreateService.CreateAsync(
                            mapper.MapToProduct(productVersion, product, organization.Id),
                            new RequestOptions { IdempotencyKey = productVersion.Id, StripeAccount = accountEntity.StripeAccountId },
                            cancellationToken);

                        var stripeProductEntity = repositoryFactory.StripeProductRepository.Add(new StripeProduct
                        {
                            Id = randomHelper.Generate(), StripeProductId = stripeProduct.Id
                        });

                        var stripePrice = await priceCreateService.CreateAsync(
                            mapper.MapToPrice(productVersion, product, organization.Id, stripeProduct.Id),
                            new RequestOptions { IdempotencyKey = @event.Metadata.Id, StripeAccount = accountEntity.StripeAccountId },
                            cancellationToken);

                        var stripePriceEntity = repositoryFactory.StripePriceRepository.Add(new StripePrice
                        {
                            Id = randomHelper.Generate(), StripePriceId = stripePrice.Id
                        });

                        existingProductVersionEntity.StripeProduct = stripeProductEntity;
                        existingProductVersionEntity.StripePrice = stripePriceEntity;
                    }
                }
                else
                {
                    if (accountEntity is not null)
                    {
                        var stripeProduct = await productUpdateService.UpdateAsync(
                            existingProductVersionEntity.StripeProduct.StripeProductId,
                            mapper.MergeToProduct(productVersion, product, organization.Id),
                            new RequestOptions { IdempotencyKey = @event.Metadata.Id, StripeAccount = accountEntity.StripeAccountId },
                            cancellationToken);

                        existingProductVersionEntity.StripeProduct.StripeProductId = stripeProduct.Id;
                        existingProductVersionEntity.StripeProduct =
                            repositoryFactory.StripeProductRepository.Update(existingProductVersionEntity.StripeProduct);
                    }
                }

                existingProductVersionEntity = repositoryFactory.ProductVersionRepository.Update(existingProductVersionEntity);
            }

            productVersions.Add(existingProductVersionEntity);
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
