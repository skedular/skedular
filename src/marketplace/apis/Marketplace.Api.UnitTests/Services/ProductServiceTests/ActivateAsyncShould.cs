using Api.Shared.Services;
using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Marketplace.Api.Services;
using Marketplace.Api.Services.Authorization;
using Marketplace.Shared.Models;
using Marketplace.Shared.Repositories;
using Offering = Api.Shared.Services.Models.Offering;
using Organization = Marketplace.Shared.Database.Entities.Organization;
using Product = Marketplace.Shared.Database.Entities.Product;
using ProductVersion = Marketplace.Shared.Database.Entities.ProductVersion;

namespace Marketplace.Api.UnitTests.Services.ProductServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ActivateAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Reject_Incomplete_Host_Location_Draft(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        ICustomerService customerService,
        [Frozen]
        IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen]
        IProductRepository productRepository,
        [Frozen]
        IOrganizationRepository organizationRepository,
        ProductService sut,
        string locationId,
        string organizationId,
        string customerId,
        CancellationToken cancellationToken)
    {
        var productId = HostLocationSystemIds.Product(locationId);
        var customer = new Customer
        {
            Id = customerId,
        };
        var customerEntity = new Shared.Database.Entities.Customer
        {
            Id = customerId,
        };
        var organization = new Organization
        {
            Id = organizationId,
            Type = OrganizationTypeConstants.Host,
            IsOwnershipVerified = true,
        };
        var product = new Product
        {
            Id = productId,
            Organization = organization,
            ProductVersions =
            [
                new ProductVersion
                {
                    PricingOptions = [],
                    ListingMetadata = null,
                },
            ],
        };

        A.CallTo(() => repositoryFactory.ProductRepository).Returns(productRepository);
        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => customerService.GetCustomerAsync(cancellationToken)).Returns((customer, customerEntity));
        A.CallTo(() => productRepository.GetByIdsAsync(A<IReadOnlyList<string>>._, cancellationToken)).Returns([product]);
        A.CallTo(() => organizationRepository.GetByIdsOrCustomDomainsAsync(A<IReadOnlyList<string>>._, null, cancellationToken))
            .Returns([organization]);
        A.CallTo(() => organizationAuthorizationService.CanModifyProductAsync(organizationId, customerId, cancellationToken)).Returns(true);

        await Should.ThrowAsync<InvalidOperationException>(() => sut.ActivateAsync([productId], cancellationToken));

        A.CallTo(() => productRepository.Update(A<Product>._)).MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Reject_Unverified_Host_Organization(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        ICustomerService customerService,
        [Frozen]
        IProductRepository productRepository,
        [Frozen]
        IOrganizationRepository organizationRepository,
        ProductService sut,
        string productId,
        string organizationId,
        string customerId,
        CancellationToken cancellationToken)
    {
        var customer = new Customer
        {
            Id = customerId,
        };
        var customerEntity = new Shared.Database.Entities.Customer
        {
            Id = customerId,
        };
        var organization = new Organization
        {
            Id = organizationId,
            Type = OrganizationTypeConstants.Host,
            IsOwnershipVerified = false,
        };
        var product = new Product
        {
            Id = productId,
            Organization = organization,
        };

        A.CallTo(() => repositoryFactory.ProductRepository).Returns(productRepository);
        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => customerService.GetCustomerAsync(cancellationToken)).Returns((customer, customerEntity));
        A.CallTo(() => productRepository.GetByIdsAsync(A<IReadOnlyList<string>>._, cancellationToken)).Returns([product]);
        A.CallTo(() => organizationRepository.GetByIdsOrCustomDomainsAsync(
                A<IReadOnlyList<string>>._, null, cancellationToken))
            .Returns([organization]);

        await Should.ThrowAsync<UnauthorizedAccessException>(() => sut.ActivateAsync([productId], cancellationToken));

        A.CallTo(() => productRepository.Update(A<Product>._)).MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Reject_Expired_Spaces_Organization(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        ICustomerService customerService,
        [Frozen]
        ISpacesAccessEvaluator spacesAccessEvaluator,
        [Frozen]
        IProductRepository productRepository,
        [Frozen]
        IOrganizationRepository organizationRepository,
        ProductService sut,
        string productId,
        string organizationId,
        string customerId,
        CancellationToken cancellationToken)
    {
        var customer = new Customer
        {
            Id = customerId,
        };
        var customerEntity = new Shared.Database.Entities.Customer
        {
            Id = customerId,
        };
        var organization = new Organization
        {
            Id = organizationId,
            Type = OrganizationTypeConstants.Marketplace,
            Offering = new Offering
            {
                Code = OfferingCode.SpacesFreeTierV1,
            },
        };
        var product = new Product
        {
            Id = productId,
            Organization = organization,
        };
        var denied = new SpacesAccessDecision(
            false,
            SpacesSubscriptionStatus.TrialExpired,
            SpacesAccessReasonCode.TrialExpired,
            SpacesAccessAction.CreateOrModify,
            OfferingCode.SpacesFreeTierV1,
            null,
            null,
            0,
            false,
            false,
            true,
            true,
            null,
            false);

        A.CallTo(() => repositoryFactory.ProductRepository).Returns(productRepository);
        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => customerService.GetCustomerAsync(cancellationToken)).Returns((customer, customerEntity));
        A.CallTo(() => productRepository.GetByIdsAsync(
                A<IReadOnlyList<string>>._, cancellationToken))
            .Returns([product]);
        A.CallTo(() => organizationRepository.GetByIdsOrCustomDomainsAsync(
                A<IReadOnlyList<string>>._, null, cancellationToken))
            .Returns([organization]);
        A.CallTo(() => spacesAccessEvaluator.Evaluate(
                A<DateTimeOffset>._,
                organization.Offering,
                SpacesAccessAction.CreateOrModify))
            .Returns(denied);

        var exception = await Should.ThrowAsync<SpacesAccessDenied>(() => sut.ActivateAsync([productId], cancellationToken));

        exception.Status.ShouldBe(SpacesSubscriptionStatus.TrialExpired);
        A.CallTo(() => productRepository.Update(A<Product>._)).MustNotHaveHappened();
    }
}
