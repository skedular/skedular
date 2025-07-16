using Booking.Shared.Database.Entities;
using Booking.Shared.Mappers;
using Booking.Shared.Repositories;
using Enterprise.Shared.Random;
using Stripe;
using Product = Stripe.Product;
using ProductVersion = Booking.Shared.Models.ProductVersion;

namespace Booking.Shared.Services;

public interface IStripeProductPricingService
{
    Task<(StripeProduct, StripePrice)> UpsertProductPricingAsync(
        ProductVersion productVersion,
        Database.Entities.ProductVersion productVersionEntity,
        string stripeAccountId,
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
        string stripeAccountId,
        CancellationToken cancellationToken)
    {
        StripeProduct stripeProductEntity;
        if (productVersionEntity.StripeProduct is null)
        {
            var stripeProduct = await productCreateService.CreateAsync(
                mapper.MapTo(productVersion, productVersion.Product, productVersion.Product.Organization.Id),
                new RequestOptions { IdempotencyKey = productVersion.Id, StripeAccount = stripeAccountId },
                cancellationToken);

            stripeProductEntity = repositoryFactory.StripeProductRepository.Add(new StripeProduct
            {
                Id = randomHelper.Generate(),
                StripeProductId = stripeProduct.Id,
                StripeAccountId = stripeAccountId,
                ProductVersion = productVersionEntity
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
                mapper.MapTo(productVersion, productVersion.Product, productVersion.Product.Organization.Id, stripeProductEntity.StripeProductId),
                new RequestOptions { IdempotencyKey = $"{productVersion.Id}-price", StripeAccount = stripeAccountId },
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
