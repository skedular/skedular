using Api.Shared.Clients.Events.Skedular.Marketplace.V1;
using Enterprise.Shared.Kafka.Consume;
using Location.Shared.Database.Entities;
using Location.Shared.Repositories;
using Location.Shared.Services;
using Location.Shared.Workflows;
using IMapper = Location.Processors.Mappers.IMapper;
using Product = Location.Shared.Models.Product;
using ProductVersion = Location.Shared.Database.Entities.ProductVersion;
using Type = Api.Shared.Clients.Events.Skedular.Marketplace.V1.Type;

namespace Location.Processors.Subscribers;

public class MarketplaceSubscriber(
    ILogger<MarketplaceSubscriber> logger,
    IMapper mapper,
    IRepositoryFactory repositoryFactory,
    ITemporalService temporalService)
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
                    var organization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync(product.Organization.Id, cancellationToken);
                    var existingProduct = await repositoryFactory.ProductRepository.UpsertNakedAsync(product.Id, organization, cancellationToken);
                    if (existingProduct.EventRaisedAt > product.EventRaisedAt)
                    {
                        logger.LogInformation("Ignoring Product event. Event timestamp is older that what is already processed.");

                        return EventSubscriberResults.Success;
                    }

                    await HandleProductUpsertedEventAsync(product, existingProduct, organization, cancellationToken);
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
        CancellationToken cancellationToken)
    {
        var organizationTags = new List<OrganizationTag>();
        var organizationTagIds = product.ProductVersions.SelectMany(item => item.OrganizationTags.Select(tag => tag.Id)).Distinct().ToList();
        foreach (var tagId in organizationTagIds)
        {
            organizationTags.Add(await repositoryFactory.OrganizationTagRepository.UpsertNakedAsync(tagId, organization, cancellationToken));
        }

        var productVersions = new List<ProductVersion>();
        foreach (var productVersion in product.ProductVersions)
        {
            var productVersionEntity =
                await repositoryFactory.ProductVersionRepository.UpsertNakedAsync(productVersion.Id, existingProduct, cancellationToken);

            var tags = organizationTags.Where(item => productVersion.OrganizationTags.Select(tag => tag.Id).Contains(item.Id)).ToList();
            productVersions.Add(
                repositoryFactory.ProductVersionRepository.Update(
                    mapper.MergeToEntity(productVersion, productVersionEntity, existingProduct, tags)));
        }

        var untouchedProductVersions = existingProduct.ProductVersions
            .Where(item => !productVersions.Select(productVersion => productVersion.Id).Contains(item.Id)).ToList();

        _ = repositoryFactory.ProductRepository.Update(
            mapper.MergeToEntity(product, existingProduct, organization, untouchedProductVersions.Concat(productVersions)));

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        await temporalService.StartComputeOrganizationLocationsAndProductsRelationshipsAsync(
            new ComputeOrganizationLocationsAndProductsRelationshipsInput(organization.Id),
            cancellationToken);
    }

    private async Task HandleProductDeletedEventAsync(Shared.Database.Entities.Product existingProduct, CancellationToken cancellationToken)
    {
        _ = repositoryFactory.ProductRepository.Remove(existingProduct);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
