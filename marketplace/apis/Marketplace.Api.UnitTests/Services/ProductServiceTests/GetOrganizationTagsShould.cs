using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Marketplace.Api.Mappers;
using Marketplace.Api.Services;
using Marketplace.Api.Services.Authorization;
using Marketplace.Shared.Models;
using Marketplace.Shared.Publishers;
using Marketplace.Shared.Repositories;
using Marketplace.Shared.Services.Cache;
using Microsoft.EntityFrameworkCore.Storage;
using Organization = Marketplace.Shared.Database.Entities.Organization;
using OrganizationTag = Marketplace.Shared.Database.Entities.OrganizationTag;
using Product = Marketplace.Shared.Database.Entities.Product;

namespace Marketplace.Api.UnitTests.Services.ProductServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetOrganizationTagsShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Load_Organisation_Tags_Through_The_Repository_Method_When_Adding_A_Product(
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IRandomHelper randomHelper,
        [Frozen] ICustomerService customerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IMarketplaceOutboxPublisher marketplaceOutboxPublisher,
        [Frozen] IMapper mapper,
        [Frozen] ICachedProductService cachedProductService,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] IOrganizationTagRepository organizationTagRepository,
        [Frozen] IProductRepository productRepository,
        [Frozen] IProductVersionRepository productVersionRepository,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IDbContextTransaction transaction,
        ProductService sut,
        CancellationToken cancellationToken)
    {
        var customer = new Customer { Id = "customer-1" };
        var existingCustomer = new Shared.Database.Entities.Customer { Id = "customer-1" };
        var organization = new Organization { Id = "org-1" };
        var organizationTag = new OrganizationTag { Id = "tag-1", Organization = organization };
        var productVersion = new ProductVersion
        {
            Type = ProductType.Resource,
            Currency = Currency.Nzd,
            OrganizationTags = [new Shared.Models.OrganizationTag { Id = "tag-1", Type = OrganizationTagType.Product }],
            PricingOptions =
            [
                new ProductPricing(
                    "pricing-1",
                    0,
                    ListingMetadata.Empty,
                    ProductPricingCadence.PerHour,
                    ProductPricingCadence.PerHour,
                    10m,
                    true,
                    false,
                    [PaymentMethod.Card],
                    ProductPricingBillingMode.Upfront,
                    null,
                    null,
                    30,
                    30,
                    1,
                    ProductPricingCancellationPolicyType.NoCancellation,
                    [])
            ]
        };
        var productEntity = new Product { Id = "product-1", Organization = organization };
        var productVersionEntity = new Shared.Database.Entities.ProductVersion { Id = "version-1", Product = productEntity };
        var mappedProduct = new Shared.Models.Product { Id = "product-1" };

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.OrganizationTagRepository).Returns(organizationTagRepository);
        A.CallTo(() => repositoryFactory.ProductRepository).Returns(productRepository);
        A.CallTo(() => repositoryFactory.ProductVersionRepository).Returns(productVersionRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => customerService.GetCustomerAsync(cancellationToken)).Returns((customer, existingCustomer));
        A.CallTo(() => randomHelper.Generate()).ReturnsNextFromSequence("product-1", "version-1");
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync("org-1", null, cancellationToken)).Returns(organization);
        A.CallTo(() => organizationAuthorizationService.CanModifyProductAsync("org-1", "customer-1", cancellationToken)).Returns(true);
        A.CallTo(() => organizationTagRepository.GetActiveByIdsForOrganizationAsync(
                A<ICollection<string>>.That.Matches(ids => ids.SequenceEqual(new[] { "tag-1" })),
                "org-1",
                null,
                cancellationToken))
            .Returns([organizationTag]);
        A.CallTo(() => mapper.MapTo(A<Shared.Models.Product>.That.Matches(product => product.Id == "product-1" && product.Inactive), organization))
            .Returns(productEntity);
        A.CallTo(() => mapper.MapTo(productVersion, productEntity, A<ICollection<OrganizationTag>>._))
            .Returns(productVersionEntity);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => productRepository.Add(productEntity)).Returns(productEntity);
        A.CallTo(() => mapper.MapTo(productEntity)).Returns(mappedProduct);
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).Returns(1);
        A.CallTo(() => transaction.CommitAsync(cancellationToken)).Returns(Task.CompletedTask);

        var result = await sut.AddAsync(null, "org-1", null, productVersion, cancellationToken);

        result.Id.ShouldBe("product-1");
        A.CallTo(() => organizationTagRepository.GetActiveByIdsForOrganizationAsync(
                A<ICollection<string>>.That.Matches(ids => ids.SequenceEqual(new[] { "tag-1" })),
                "org-1",
                null,
                cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => marketplaceOutboxPublisher.PublishProducts(A<ICollection<Shared.Models.Product>>._, unitOfWork))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => cachedProductService.UpdateByIdAsync("product-1", cancellationToken)).MustHaveHappenedOnceExactly();
    }
}
