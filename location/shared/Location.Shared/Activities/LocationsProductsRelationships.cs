using Api.Shared.Services.Models;
using Enterprise.Shared.Random;
using Location.Shared.Database.Entities;
using Location.Shared.Repositories;
using Temporalio.Activities;

namespace Location.Shared.Activities;

public class LocationsProductsRelationships(IRepositoryFactory repositoryFactory, IRandomHelper randomHelper)
{
    [Activity]
    public async Task ComputeLocationAndProductsRelationshipsAsync(string organizationId)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var products = await repositoryFactory.ProductRepository.GetByOrganizationIdAsync(organizationId, cancellationToken);
        if (products.Count == 0)
        {
            return;
        }

        var existingPrecomputedLocationProducts =
            await repositoryFactory.PrecomputedLocationProductRepository.GetByOrganizationIdAsync(organizationId, cancellationToken);

        var locations = await repositoryFactory.LocationRepository.GetByOrganizationIdAsync(organizationId, cancellationToken);

        foreach (var product in products)
        {
            var productTagIds = product.ProductVersions.First().ProductTags.Select(item => item.Id);

            foreach (var location in locations)
            {
                var organizationTags = location.Resources.SelectMany(item => item.OrganizationTags).ToList();
                var organizationTagIds = organizationTags.Select(item => item.Id).Distinct().ToList();
                if (!organizationTagIds.Any(item => productTagIds.Any(productTagId => productTagId == item)))
                {
                    continue;
                }

                repositoryFactory.PrecomputedLocationProductRepository.Add(new PrecomputedLocationProduct
                {
                    Id = randomHelper.Generate(),
                    Organization = location.Organization,
                    Location = location,
                    Product = product,
                    OrganizationTags = organizationTagIds
                        .Select(item => organizationTags.First(tag => tag.Id == item))
                        .Where(item => !string.IsNullOrWhiteSpace(item.Type))
                        .Where(item => OrganizationTagTypeConstants.ResourceTypes.Contains(item.Type!.ToOrganizationTagType()))
                        .ToList()
                });
            }
        }

        repositoryFactory.PrecomputedLocationProductRepository.RemoveRange(existingPrecomputedLocationProducts);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
