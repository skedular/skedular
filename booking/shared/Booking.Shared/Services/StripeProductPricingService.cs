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
    Task<StripeProduct> UpsertProductPricingAsync(
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
    public async Task<StripeProduct> UpsertProductPricingAsync(
        ProductVersion productVersion,
        Database.Entities.ProductVersion productVersionEntity,
        string stripeAccountId,
        CancellationToken cancellationToken)
    {
        StripeProduct stripeProductEntity;
        if (productVersionEntity.StripeProducts.Count == 0)
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
            stripeProductEntity = productVersionEntity.StripeProducts.First();
        }

        if (stripeProductEntity.StripePrice is null)
        {
            var stripePrice = await priceCreateService.CreateAsync(
                mapper.MapTo(productVersion, productVersion.Product, productVersion.Product.Organization.Id, stripeProductEntity.StripeProductId),
                new RequestOptions { IdempotencyKey = $"{productVersion.Id}-price", StripeAccount = stripeAccountId },
                cancellationToken);

            _ = repositoryFactory.StripePriceRepository.Add(new StripePrice
            {
                Id = randomHelper.Generate(), StripePriceId = stripePrice.Id, StripeProduct = stripeProductEntity
            });
        }

        return stripeProductEntity;
    }
}
