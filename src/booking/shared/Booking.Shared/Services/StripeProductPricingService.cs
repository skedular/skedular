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
    /// <summary>
    ///     Ensures only the price selected for a checkout exists in Stripe. Editing an
    ///     offer must not eagerly create Stripe Prices for unpublished/intermediate
    ///     pricing options.
    /// </summary>
    ValueTask EnsureProductPricingAsync(
        ProductVersion productVersion,
        ProductPricing pricing,
        string stripeAccountId,
        CancellationToken cancellationToken);

    string? GetOneTimePriceId(ProductVersion productVersion, ProductPricing pricing, string stripeAccountId);
    string? GetRecurringPriceId(ProductVersion productVersion, ProductPricing pricing, string stripeAccountId);
}

public class StripeProductPricingService(
    IRepositoryFactory repositoryFactory,
    IEntityMapper entityMapper,
    IRandomHelper randomHelper,
    ICreatable<Product, ProductCreateOptions> productCreateService,
    ICreatable<Price, PriceCreateOptions> priceCreateService) : IStripeProductPricingService
{
    public string? GetRecurringPriceId(ProductVersion productVersion, ProductPricing pricing, string stripeAccountId) =>
        GetPriceId(productVersion, pricing, stripeAccountId, true);

    public string? GetOneTimePriceId(ProductVersion productVersion, ProductPricing pricing, string stripeAccountId) =>
        GetPriceId(productVersion, pricing, stripeAccountId, false);

    public async ValueTask EnsureProductPricingAsync(
        ProductVersion productVersion,
        ProductPricing pricing,
        string stripeAccountId,
        CancellationToken cancellationToken)
    {
        var stripeProductEntity = productVersion.StripeProducts.FirstOrDefault(item => item.StripeAccountId == stripeAccountId);
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
                MembershipTerm = pricing.MembershipTerm.ToMembershipTerms(),
                BillingMode = pricing.BillingMode.ToProductPricingBillingMode(),
                NumberOfResourcesToBook = pricing.NumberOfResourcesToBook,
                StripeProductId = stripeProduct.Id,
                StripeAccountId = stripeAccountId,
                ProductVersion = productVersion,
            });

            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        }

        var oneTimePrice = stripeProductEntity.StripePrices.SingleOrDefault(item =>
            !item.IsRecurring && item.StripeAccountId == stripeAccountId && item.ProductPricingId == pricing.Id &&
            item.UnitAmount == pricing.Price && item.Currency == productVersion.Currency &&
            item.MembershipTerm == pricing.MembershipTerm.ToMembershipTerms() &&
            item.BillingMode == pricing.BillingMode.ToProductPricingBillingMode() && item.IsTaxInclusive == pricing.IsTaxInclusive);
        if (oneTimePrice is null)
        {
            var priceId = randomHelper.Generate();
            var stripePrice = await priceCreateService.CreateAsync(
                entityMapper.MapTo(pricing, stripeProductEntity),
                new RequestOptions
                {
                    IdempotencyKey = $"{productVersion.Id}-{priceId}-price",
                    StripeAccount = stripeAccountId,
                },
                cancellationToken);

            oneTimePrice = repositoryFactory.StripePriceRepository.Add(new StripePrice
            {
                Id = priceId,
                StripeProductId = stripeProductEntity.Id,
                StripeAccountId = stripeAccountId,
                ProductPricingId = pricing.Id,
                StripePriceId = stripePrice.Id,
                Currency = productVersion.Currency ?? string.Empty,
                UnitAmount = pricing.Price,
                MembershipTerm = pricing.MembershipTerm.ToMembershipTerms(),
                BillingMode = pricing.BillingMode.ToProductPricingBillingMode(),
                IsTaxInclusive = pricing.IsTaxInclusive,
                StripeProduct = stripeProductEntity,
            });
            stripeProductEntity.StripePrices.Add(oneTimePrice);
        }

        var recurringPrice = stripeProductEntity.StripePrices.SingleOrDefault(item =>
            item.IsRecurring &&
            item.StripeAccountId == stripeAccountId &&
            item.ProductPricingId == pricing.Id &&
            item.UnitAmount == pricing.Price &&
            item.Currency == productVersion.Currency &&
            item.MembershipTerm == pricing.MembershipTerm.ToMembershipTerms() &&
            item.BillingMode == pricing.BillingMode.ToProductPricingBillingMode() &&
            item.IsTaxInclusive == pricing.IsTaxInclusive);
        if (pricing.SupportsSubscriptionAutoRenewal && recurringPrice is null)
        {
            var subscriptionPriceId = randomHelper.Generate();
            var subscriptionPriceOptions = entityMapper.MapTo(pricing, stripeProductEntity);
            subscriptionPriceOptions.Recurring = ToStripeRecurring(pricing.MembershipTerm);
            var subscriptionPrice = await priceCreateService.CreateAsync(
                subscriptionPriceOptions,
                new RequestOptions
                {
                    IdempotencyKey = $"{productVersion.Id}-{subscriptionPriceId}-subscription-price",
                    StripeAccount = stripeAccountId,
                },
                cancellationToken);
            recurringPrice = repositoryFactory.StripePriceRepository.Add(new StripePrice
            {
                Id = subscriptionPriceId,
                StripeProductId = stripeProductEntity.Id,
                StripeAccountId = stripeAccountId,
                ProductPricingId = pricing.Id,
                StripePriceId = subscriptionPrice.Id,
                Currency = productVersion.Currency ?? string.Empty,
                UnitAmount = pricing.Price,
                MembershipTerm = pricing.MembershipTerm.ToMembershipTerms(),
                BillingMode = pricing.BillingMode.ToProductPricingBillingMode(),
                IsTaxInclusive = pricing.IsTaxInclusive,
                StripeProduct = stripeProductEntity,
                IsRecurring = true,
            });
            stripeProductEntity.StripePrices.Add(recurringPrice);
        }

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static string? GetPriceId(ProductVersion productVersion, ProductPricing pricing, string stripeAccountId, bool isRecurring)
    {
        var stripePrices = productVersion.StripeProducts
            .FirstOrDefault(item => item.StripeAccountId == stripeAccountId)?
            .StripePrices ?? productVersion.StripeProducts
            .Where(item => string.IsNullOrWhiteSpace(item.StripeAccountId) && item.ProductPricingId == pricing.Id)
            .Select(item => item.StripePrices)
            .SingleOrDefault();
        if (stripePrices is null)
        {
            return null;
        }

        var exactMatch = stripePrices.SingleOrDefault(item =>
            item.IsRecurring == isRecurring &&
            item.StripeAccountId == stripeAccountId &&
            item.ProductPricingId == pricing.Id &&
            item.UnitAmount == pricing.Price &&
            item.Currency == productVersion.Currency &&
            item.MembershipTerm == pricing.MembershipTerm.ToMembershipTerms() &&
            item.BillingMode == pricing.BillingMode.ToProductPricingBillingMode() &&
            item.IsTaxInclusive == pricing.IsTaxInclusive);
        // Existing one-time rows predate the full catalog fingerprint. They can be
        // safely adopted only while there is exactly one candidate on the account.
        return exactMatch?.StripePriceId ?? stripePrices.Where(item => item.IsRecurring == isRecurring)
            .Take(2)
            .SingleOrDefault()
            ?.StripePriceId;
    }

    private static PriceRecurringOptions ToStripeRecurring(MembershipTerm membershipTerm) => membershipTerm switch
    {
        MembershipTerm.Daily => new PriceRecurringOptions
        {
            Interval = "day",
            IntervalCount = 1,
        },
        MembershipTerm.Weekly => new PriceRecurringOptions
        {
            Interval = "week",
            IntervalCount = 1,
        },
        MembershipTerm.Fortnightly => new PriceRecurringOptions
        {
            Interval = "week",
            IntervalCount = 2,
        },
        MembershipTerm.Monthly => new PriceRecurringOptions
        {
            Interval = "month",
            IntervalCount = 1,
        },
        MembershipTerm.TwoMonths => new PriceRecurringOptions
        {
            Interval = "month",
            IntervalCount = 2,
        },
        MembershipTerm.Quarterly => new PriceRecurringOptions
        {
            Interval = "month",
            IntervalCount = 3,
        },
        MembershipTerm.FourMonths => new PriceRecurringOptions
        {
            Interval = "month",
            IntervalCount = 4,
        },
        MembershipTerm.FiveMonths => new PriceRecurringOptions
        {
            Interval = "month",
            IntervalCount = 5,
        },
        MembershipTerm.SixMonths => new PriceRecurringOptions
        {
            Interval = "month",
            IntervalCount = 6,
        },
        MembershipTerm.Yearly => new PriceRecurringOptions
        {
            Interval = "year",
            IntervalCount = 1,
        },
        _ => throw new InvalidOperationException("An auto-renewing marketplace price requires a supported membership term."),
    };
}
