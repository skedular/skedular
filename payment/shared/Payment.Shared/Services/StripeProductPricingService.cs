using Enterprise.Shared.Random;
using Payment.Shared.Database.Entities;
using Payment.Shared.Mappers;
using Payment.Shared.Repositories;
using Stripe;
using Product = Stripe.Product;
using ProductVersion = Payment.Shared.Models.ProductVersion;

namespace Payment.Shared.Services;

public interface IStripeProductPricingService
{
    Task<(StripeProduct, StripePrice)> UpsertProductPricingAsync(
        ProductVersion productVersion,
        Database.Entities.ProductVersion productVersionEntity,
        StripeConnectAccount stripeConnectAccount,
        CancellationToken cancellationToken);
}

public class StripeProductPricingService(
    IRepositoryFactory repositoryFactory,
    IMapper mapper,
    IRandomHelper randomHelper,
    ICreatable<Product, ProductCreateOptions> productCreateService,
    ICreatable<Price, PriceCreateOptions> priceCreateService) : IStripeProductPricingService
{
    public async Task<(StripeProduct, StripePrice)> UpsertProductPricingAsync(
        ProductVersion productVersion,
        Database.Entities.ProductVersion productVersionEntity,
        StripeConnectAccount stripeConnectAccount,
        CancellationToken cancellationToken)
    {
        StripeProduct stripeProductEntity;
        if (productVersionEntity.StripeProduct is null)
        {
            var stripeProduct = await productCreateService.CreateAsync(
                mapper.MapToProduct(productVersion, productVersion.Product, productVersion.Product.Organization.Id),
                new RequestOptions { IdempotencyKey = productVersion.Id, StripeAccount = stripeConnectAccount?.StripeAccountId },
                cancellationToken);

            stripeProductEntity = repositoryFactory.StripeProductRepository.Add(new StripeProduct
            {
                Id = randomHelper.Generate(), StripeProductId = stripeProduct.Id, ProductVersion = productVersionEntity
            });
        }
        else
        {
            stripeProductEntity = productVersionEntity.StripeProduct;
        }

        StripePrice stripePriceEntity;
        if (productVersionEntity.StripePrice is null)
        {
            var stripePrice = await priceCreateService.CreateAsync(
                mapper.MapToPrice(
                    productVersion,
                    productVersion.Product,
                    productVersion.Product.Organization.Id,
                    stripeProductEntity.StripeProductId),
                new RequestOptions { IdempotencyKey = $"{productVersion.Id}-price", StripeAccount = stripeConnectAccount?.StripeAccountId },
                cancellationToken);

            stripePriceEntity = repositoryFactory.StripePriceRepository.Add(new StripePrice
            {
                Id = randomHelper.Generate(), StripePriceId = stripePrice.Id, ProductVersion = productVersionEntity
            });
        }
        else
        {
            stripePriceEntity = productVersionEntity.StripePrice;
        }

        return (stripeProductEntity, stripePriceEntity);
    }
}
