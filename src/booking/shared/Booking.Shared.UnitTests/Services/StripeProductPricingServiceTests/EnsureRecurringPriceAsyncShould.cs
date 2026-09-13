using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Mappers;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Database;
using Stripe;
using Product = Stripe.Product;

namespace Booking.Shared.UnitTests.Services.StripeProductPricingServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public sealed class EnsureRecurringPriceAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task CreateOnlyTheScheduleSpecificRecurringPrice(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IStripeProductRepository stripeProductRepository,
        [Frozen]
        IStripePriceRepository stripePriceRepository,
        [Frozen]
        IUnitOfWork unitOfWork,
        [Frozen]
        IEntityMapper entityMapper,
        [Frozen]
        ICreatable<Product, ProductCreateOptions> productCreateService,
        [Frozen]
        ICreatable<Price, PriceCreateOptions> priceCreateService,
        StripeProductPricingService sut,
        CancellationToken cancellationToken)
    {
        var pricing = ProductPricing.Empty("pricing") with
        {
            Price = 120,
            MembershipTerm = MembershipTerm.SixMonths,
            BillingMode = ProductPricingBillingMode.Upfront,
            SupportsSubscriptionAutoRenewal = true,
        };
        var productVersion = new ProductVersion
        {
            Id = "product-version",
            Currency = "NZD",
            StripeProducts = [],
        };
        var productCreateOptions = new ProductCreateOptions();
        var priceCreateOptions = new PriceCreateOptions();
        var stripeProduct = new Product
        {
            Id = "prod_1",
        };
        var stripePrice = new Price
        {
            Id = "price_1",
        };

        A.CallTo(() => entityMapper.MapTo(pricing, productVersion)).Returns(productCreateOptions);
        A.CallTo(() => entityMapper.MapTo(pricing, A<StripeProduct>._)).Returns(priceCreateOptions);
        A.CallTo(() => productCreateService.CreateAsync(productCreateOptions, A<RequestOptions>._, cancellationToken))
            .Returns(stripeProduct);
        A.CallTo(() => priceCreateService.CreateAsync(priceCreateOptions, A<RequestOptions>._, cancellationToken))
            .Returns(stripePrice);
        A.CallTo(() => stripeProductRepository.Add(A<StripeProduct>._))
            .ReturnsLazily((StripeProduct product) => product);
        A.CallTo(() => stripePriceRepository.Add(A<StripePrice>._))
            .ReturnsLazily((StripePrice price) => price);
        A.CallTo(() => repositoryFactory.StripeProductRepository).Returns(stripeProductRepository);
        A.CallTo(() => repositoryFactory.StripePriceRepository).Returns(stripePriceRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);

        var result = await sut.EnsureRecurringPriceAsync(
            productVersion,
            pricing,
            "acct_1",
            20,
            MembershipTerm.Monthly,
            cancellationToken);

        result.ShouldBe("price_1");
        A.CallTo(() => productCreateService.CreateAsync(productCreateOptions, A<RequestOptions>._, cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => priceCreateService.CreateAsync(
                A<PriceCreateOptions>.That.Matches(options => options.Recurring != null),
                A<RequestOptions>._,
                cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => priceCreateService.CreateAsync(
                A<PriceCreateOptions>.That.Matches(options => options.Recurring == null),
                A<RequestOptions>._,
                cancellationToken))
            .MustNotHaveHappened();
    }
}
