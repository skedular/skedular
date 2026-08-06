using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Mappers;
using Booking.Shared.Repositories;
using Enterprise.Shared.Random;
using Stripe;
using Product = Stripe.Product;

namespace Booking.Shared.Services;

public interface IStripeProductPricingService
{
    ValueTask UpsertProductPricingAsync(ProductVersion productVersion, string stripeAccountId, CancellationToken cancellationToken);
}

public class StripeProductPricingService(
    IRepositoryFactory repositoryFactory,
    IEntityMapper entityMapper,
    IRandomHelper randomHelper,
    ICreatable<Product, ProductCreateOptions> productCreateService,
    ICreatable<Price, PriceCreateOptions> priceCreateService,
    IProductVersionHelperService productVersionHelperService) : IStripeProductPricingService
{
    public async ValueTask UpsertProductPricingAsync(ProductVersion productVersion, string stripeAccountId, CancellationToken cancellationToken)
    {
        var pricingOptions = productVersion.PricingOptions;
        ArgumentNullException.ThrowIfNull(pricingOptions);

        foreach (var pricing in pricingOptions)
        {
            var stripeProductEntity = productVersionHelperService.FindMatchingPricing(productVersion.StripeProducts.ToList(), pricing);
            if (stripeProductEntity is null)
            {
                var productId = randomHelper.Generate();
                var stripeProduct = await productCreateService.CreateAsync(
                    entityMapper.MapTo(pricing, productVersion),
                    new RequestOptions
                    {
                        IdempotencyKey = $"{productVersion.Id}-{productId}",
                        StripeAccount = stripeAccountId,
                    },
                    cancellationToken);

                stripeProductEntity = repositoryFactory.StripeProductRepository.Add(new StripeProduct
                {
                    Id = productId,
                    ProductPricingId = pricing.Id,
                    PricingCadence = pricing.PurchaseCadence.ToProductPricingCadence(),
                    BillingMode = pricing.BillingMode.ToProductPricingBillingMode(),
                    NumberOfResourcesToBook = pricing.NumberOfResourcesToBook,
                    StripeProductId = stripeProduct.Id,
                    StripeAccountId = stripeAccountId,
                    ProductVersion = productVersion,
                });

                await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            }

            if (stripeProductEntity.StripePrice is not null)
            {
                continue;
            }

            var priceId = randomHelper.Generate();
            var stripePrice = await priceCreateService.CreateAsync(
                entityMapper.MapTo(pricing, stripeProductEntity),
                new RequestOptions
                {
                    IdempotencyKey = $"{productVersion.Id}-{priceId}-price",
                    StripeAccount = stripeAccountId,
                },
                cancellationToken);

            _ = repositoryFactory.StripePriceRepository.Add(new StripePrice
            {
                Id = priceId,
                StripePriceId = stripePrice.Id,
                StripeProduct = stripeProductEntity,
            });

            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
