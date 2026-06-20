using Marketplace.Api.Models;
using Marketplace.Api.Services;
using Marketplace.Api.Services.Authorization;
using Marketplace.Shared.Mappers;
using Marketplace.Shared.Models;
using Marketplace.Shared.Repositories;
using Microsoft.Extensions.Logging;
using Testing.Shared.Assertions;
using ListingMetadataModel = Api.Shared.Services.Models.ListingMetadata;
using CustomerEntity = Marketplace.Shared.Database.Entities.Customer;
using CustomerModel = Marketplace.Shared.Models.Customer;
using OrganizationEntity = Marketplace.Shared.Database.Entities.Organization;
using ProductEntity = Marketplace.Shared.Database.Entities.Product;
using ProductVersionEntity = Marketplace.Shared.Database.Entities.ProductVersion;

namespace Marketplace.Api.UnitTests.Services.ProductServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class UpdatePatchAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Authorization_Rejection_And_Rethrow(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IProductRepository productRepository,
        [Frozen] ICustomerService customerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IEntityMapper entityMapper,
        [Frozen] ILogger<ProductService> logger,
        ProductService sut,
        CancellationToken cancellationToken)
    {
        var customer = new CustomerModel { Id = "cust-1" };
        var customerEntity = new CustomerEntity { Id = "cust-1" };
        var orgEntity = new OrganizationEntity { Id = "org-1" };
        var existingProduct = new ProductEntity { Id = "product-1", Organization = orgEntity };
        var productVersion = new ProductVersion
        {
            Product = new Product { Id = "product-1" }, CreatedAt = DateTimeOffset.UtcNow, PricingOptions = []
        };
        var productModel = new Product { Id = "product-1", Organization = new Organization { Id = "org-1" }, ProductVersions = [productVersion] };
        var request = new ProductPatchRequest(
            "product-1",
            new HashSet<ProductPatchField> { ProductPatchField.Tags },
            productVersion);

        A.CallTo(() => repositoryFactory.ProductRepository).Returns(productRepository);
        A.CallTo(() => customerService.GetCustomerAsync(cancellationToken)).Returns((customer, customerEntity));
        A.CallTo(() => productRepository.GetByIdAsync("product-1", cancellationToken)).Returns(existingProduct);
        A.CallTo(() => entityMapper.MapTo(existingProduct)).Returns(productModel);
        A.CallTo(() => organizationAuthorizationService.CanModifyProductAsync("org-1", "cust-1", cancellationToken))
            .Returns(new ValueTask<bool>(false));

        await Should.ThrowAsync<UnauthorizedAccessException>(() => sut.UpdateAsync(request, cancellationToken));

        LogAssertions.ACallToLog(logger, LogLevel.Warning)
            .Where(call => call.GetArgument<IReadOnlyList<KeyValuePair<string, object>>>(2)!.ToString()!
                .Contains("rejected by authorization"))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Error_And_Rethrow_On_General_Failure(
        [Frozen] ICustomerService customerService,
        [Frozen] ILogger<ProductService> logger,
        ProductService sut,
        CancellationToken cancellationToken)
    {
        var request = new ProductPatchRequest(
            "product-1",
            new HashSet<ProductPatchField> { ProductPatchField.Tags },
            new ProductVersion { PricingOptions = [] });

        A.CallTo(() => customerService.GetCustomerAsync(cancellationToken))
            .ThrowsAsync(new InvalidOperationException("service failure"));

        await Should.ThrowAsync<InvalidOperationException>(() => sut.UpdateAsync(request, cancellationToken));

        LogAssertions.ACallToLog(logger, LogLevel.Error)
            .Where(call => call.GetArgument<IReadOnlyList<KeyValuePair<string, object>>>(2)!.ToString()!
                .Contains("Product patch autosave failed"))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Autosave_Started(
        [Frozen] ICustomerService customerService,
        [Frozen] ILogger<ProductService> logger,
        ProductService sut,
        CancellationToken cancellationToken)
    {
        var request = new ProductPatchRequest(
            "product-1",
            new HashSet<ProductPatchField> { ProductPatchField.Tags },
            new ProductVersion { PricingOptions = [] });

        A.CallTo(() => customerService.GetCustomerAsync(cancellationToken))
            .ThrowsAsync(new InvalidOperationException("forced early failure"));

        await Should.ThrowAsync<InvalidOperationException>(() => sut.UpdateAsync(request, cancellationToken));

        LogAssertions.ACallToLogInfoContaining(logger, "Product patch autosave started")
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Append_Product_Version_Without_Updating_Product_Row(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IProductRepository productRepository,
        [Frozen] IProductVersionRepository productVersionRepository,
        [Frozen] ICustomerService customerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IEntityMapper entityMapper,
        ProductService sut,
        CancellationToken cancellationToken)
    {
        var customer = new CustomerModel { Id = "cust-1" };
        var customerEntity = new CustomerEntity { Id = "cust-1" };
        var orgEntity = new OrganizationEntity { Id = "org-1" };
        var existingProduct = new ProductEntity { Id = "product-1", Organization = orgEntity };
        var currentVersion = new ProductVersion
        {
            Product = new Product { Id = "product-1" },
            CreatedAt = DateTimeOffset.UtcNow,
            PricingOptions = []
        };
        var productModel = new Product { Id = "product-1", Organization = new Organization { Id = "org-1" }, ProductVersions = [currentVersion] };
        var productVersionEntity = new ProductVersionEntity { Id = "version-2", Product = existingProduct };
        var request = new ProductPatchRequest(
            "product-1",
            new HashSet<ProductPatchField> { ProductPatchField.ListingMetadata },
            new ProductVersion { ListingMetadata = ListingMetadataModel.Empty, PricingOptions = [] });

        A.CallTo(() => repositoryFactory.ProductRepository).Returns(productRepository);
        A.CallTo(() => repositoryFactory.ProductVersionRepository).Returns(productVersionRepository);
        A.CallTo(() => customerService.GetCustomerAsync(cancellationToken)).Returns((customer, customerEntity));
        A.CallTo(() => productRepository.GetByIdAsync("product-1", cancellationToken)).Returns(existingProduct);
        A.CallTo(() => entityMapper.MapTo(existingProduct)).Returns(productModel);
        A.CallTo(() => organizationAuthorizationService.CanModifyProductAsync("org-1", "cust-1", cancellationToken))
            .Returns(new ValueTask<bool>(true));
        A.CallTo(() => productVersionRepository.Add(A<ProductVersionEntity>._)).Returns(productVersionEntity);
        A.CallTo(() => entityMapper.MapTo(productVersionEntity, existingProduct)).Returns(currentVersion);

        await sut.UpdateAsync(request, cancellationToken);

        A.CallTo(() => productRepository.Update(A<ProductEntity>._)).MustNotHaveHappened();
        A.CallTo(() => productVersionRepository.Add(A<ProductVersionEntity>._)).MustHaveHappenedOnceExactly();
    }
}
