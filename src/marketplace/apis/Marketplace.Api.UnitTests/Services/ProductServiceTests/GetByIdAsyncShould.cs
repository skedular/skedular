using Api.Shared.Services.Models;
using Marketplace.Api.Services;
using Marketplace.Shared.Services.Cache;
using ProductEntity = Marketplace.Shared.Database.Entities.Product;
using OrganizationEntity = Marketplace.Shared.Database.Entities.Organization;

namespace Marketplace.Api.UnitTests.Services.ProductServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetByIdAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Hide_Unverified_Host_Product(
        [Frozen] ICachedProductService cachedProductService,
        ProductService sut,
        string productId,
        CancellationToken cancellationToken)
    {
        var product = new ProductEntity
        {
            Id = productId, Organization = new OrganizationEntity { Type = OrganizationTypeConstants.Host, IsOwnershipVerified = false }
        };
        A.CallTo(() => cachedProductService.GetByIdAsync(productId, cancellationToken)).Returns(product);

        var result = await sut.GetByIdAsync(productId, cancellationToken);

        result.ShouldBeNull();
    }
}
