using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Grpc.Skedular.Location.Graphql.V1;
using Api.Shared.Grpc.Skedular.Marketplace.Core.V1;
using Api.Shared.Grpc.Skedular.Organization.Tags.V1;
using Api.Shared.Services.Models;
using Enterprise.Shared.Grpc;
using Location.Shared.Repositories;
using Location.Shared.Services;
using Location.Shared.Services.Cache;
using Temporalio.Activities;
using GraphQlConstants = Location.Shared.GraphQL.Constants;

namespace Location.Shared.Activities;

public class HostLocationProvisioning(
    OrganizationConfiguration organizationConfiguration,
    LocationConfiguration locationConfiguration,
    MarketplaceConfiguration marketplaceConfiguration,
    OrganizationTagsService.OrganizationTagsServiceClient organizationTagsServiceClient,
    MarketplaceService.MarketplaceServiceClient marketplaceServiceClient,
    LocationGraphqlService.LocationGraphqlServiceClient locationGraphqlServiceClient,
    IAutoResourceService autoResourceService,
    IRepositoryFactory repositoryFactory,
    ICachedLocationService cachedLocationService)
{
    [Activity]
    public async Task ProvisionAsync(string organizationId, string locationId, string locationName)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var productTagId = HostLocationSystemIds.ProductTag(locationId);
        var productTag = await organizationTagsServiceClient.AddProductTagAsync(
            new AddProductTagInput
            {
                Id = productTagId,
                OrganizationId = organizationId,
                Name = $"Host Location {locationId}",
                Description = "System-managed product tag for a Host location.",
                Color = "#4169E1"
            },
            organizationConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);

        await autoResourceService.EnsureForHostLocationAsync(locationId, productTag.Id, cancellationToken);

        await marketplaceServiceClient.EnsureHostLocationDraftProductAsync(
            new EnsureHostLocationDraftProductInput
            {
                Id = HostLocationSystemIds.Product(locationId),
                OrganizationId = organizationId,
                ProductTagId = productTag.Id,
                LocationName = locationName
            },
            marketplaceConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);

        var productId = HostLocationSystemIds.Product(locationId);
        var locationProducts = await repositoryFactory.PrecomputedLocationProductRepository
            .GetByLocationAsync(locationId, cancellationToken);
        if (locationProducts.All(item => item.Product.Id != productId))
        {
            throw new InvalidOperationException(
                "Host Location Product has not yet been replicated into the Location pricing projection.");
        }

        await cachedLocationService.RemoveByIdAsync(locationId, cancellationToken);

        await locationGraphqlServiceClient.RaiseGraphqlChangeAsync(
            new RaiseGraphqlChangeInput { TopicName = GraphQlConstants.ListingProductReadyTopicName, Id = locationId },
            locationConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);
    }

    [Activity]
    public async Task DeprovisionAsync(string organizationId, string locationId)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        await marketplaceServiceClient.RemoveHostLocationDraftProductAsync(
            new RemoveHostLocationDraftProductInput { Id = HostLocationSystemIds.Product(locationId), OrganizationId = organizationId },
            marketplaceConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);
    }
}
