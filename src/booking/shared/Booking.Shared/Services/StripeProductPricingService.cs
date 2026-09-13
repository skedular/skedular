using System.Globalization;
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

    Task<string> EnsureRecurringPriceAsync(
        ProductVersion productVersion,
        ProductPricing pricing,
        string stripeAccountId,
        decimal unitAmount,
        MembershipTerm recurringTerm,
        CancellationToken cancellationToken);
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

    public async Task<string> EnsureRecurringPriceAsync(
        ProductVersion productVersion,
        ProductPricing pricing,
        string stripeAccountId,
        decimal unitAmount,
        MembershipTerm recurringTerm,
        CancellationToken cancellationToken)
    {
        // Recurring prices are schedule-specific. Ensure only the shared Product here;
        // one-time catalog entries are created separately by EnsureProductPricingAsync.
        var stripeProduct = await EnsureStripeProductAsync(productVersion, pricing, stripeAccountId, cancellationToken);
        var persistedTerm = recurringTerm.ToMembershipTerms();
        var persistedBillingMode = pricing.BillingMode.ToProductPricingBillingMode();
        var existing = stripeProduct.StripePrices.SingleOrDefault(item =>
            item.IsRecurring &&
            item.StripeAccountId == stripeAccountId &&
            item.ProductPricingId == pricing.Id &&
            item.UnitAmount == unitAmount &&
            item.Currency == productVersion.Currency &&
            item.MembershipTerm == persistedTerm &&
            item.BillingMode == persistedBillingMode &&
            item.IsTaxInclusive == pricing.IsTaxInclusive);
        if (existing is not null)
        {
            return existing.StripePriceId;
        }

        var priceOptions = entityMapper.MapTo(pricing, stripeProduct);
        priceOptions.UnitAmountDecimal = unitAmount * 100;
        priceOptions.Recurring = ToStripeRecurring(recurringTerm);
        var stripePrice = await priceCreateService.CreateAsync(
            priceOptions,
            new RequestOptions
            {
                IdempotencyKey = CreatePriceIdempotencyKey(
                    productVersion,
                    pricing,
                    stripeAccountId,
                    true,
                    unitAmount,
                    recurringTerm),
                StripeAccount = stripeAccountId,
            },
            cancellationToken);

        var persistedPrice = repositoryFactory.StripePriceRepository.Add(new StripePrice
        {
            Id = randomHelper.Generate(),
            StripeProductId = stripeProduct.Id,
            StripeAccountId = stripeAccountId,
            ProductPricingId = pricing.Id,
            StripePriceId = stripePrice.Id,
            Currency = productVersion.Currency ?? string.Empty,
            UnitAmount = unitAmount,
            MembershipTerm = persistedTerm,
            BillingMode = persistedBillingMode,
            IsTaxInclusive = pricing.IsTaxInclusive,
            StripeProduct = stripeProduct,
            IsRecurring = true,
        });
        stripeProduct.StripePrices.Add(persistedPrice);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        return persistedPrice.StripePriceId;
    }

    public async ValueTask EnsureProductPricingAsync(
        ProductVersion productVersion,
        ProductPricing pricing,
        string stripeAccountId,
        CancellationToken cancellationToken)
    {
        var stripeProductEntity = await EnsureStripeProductAsync(productVersion, pricing, stripeAccountId, cancellationToken);

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
                    IdempotencyKey = CreatePriceIdempotencyKey(productVersion, pricing, stripeAccountId, false,
                        pricing.Price, pricing.MembershipTerm),
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

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<StripeProduct> EnsureStripeProductAsync(
        ProductVersion productVersion,
        ProductPricing pricing,
        string stripeAccountId,
        CancellationToken cancellationToken)
    {
        var existing = productVersion.StripeProducts.FirstOrDefault(item => item.StripeAccountId == stripeAccountId);
        if (existing is not null)
        {
            return existing;
        }

        var stripeProduct = await productCreateService.CreateAsync(
            entityMapper.MapTo(pricing, productVersion),
            new RequestOptions
            {
                IdempotencyKey = $"marketplace-product:{stripeAccountId}:{productVersion.Id}",
                StripeAccount = stripeAccountId,
            },
            cancellationToken);

        var productEntity = repositoryFactory.StripeProductRepository.Add(new StripeProduct
        {
            Id = randomHelper.Generate(),
            ProductPricingId = pricing.Id,
            MembershipTerm = pricing.MembershipTerm.ToMembershipTerms(),
            BillingMode = pricing.BillingMode.ToProductPricingBillingMode(),
            NumberOfResourcesToBook = pricing.NumberOfResourcesToBook,
            StripeProductId = stripeProduct.Id,
            StripeAccountId = stripeAccountId,
            ProductVersion = productVersion,
        });
        productVersion.StripeProducts.Add(productEntity);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        return productEntity;
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
        if (exactMatch is not null)
        {
            return exactMatch.StripePriceId;
        }

        // A recurring price must never be guessed. Reusing a non-matching recurring
        // price could silently charge a previous amount, term, currency, or tax mode.
        // The legacy single-candidate fallback is retained only for old one-time rows.
        return isRecurring
            ? null
            : stripePrices.Where(item => item.IsRecurring == isRecurring)
                .Take(2)
                .SingleOrDefault()
                ?.StripePriceId;
    }

    private static string CreatePriceIdempotencyKey(
        ProductVersion productVersion,
        ProductPricing pricing,
        string stripeAccountId,
        bool recurring,
        decimal unitAmount,
        MembershipTerm recurringTerm) =>
        $"marketplace-price:{stripeAccountId}:{productVersion.Id}:{pricing.Id}:{(recurring ? "recurring" : "one-time")}:{unitAmount.ToString(CultureInfo.InvariantCulture)}:{productVersion.Currency}:{recurringTerm}:{pricing.BillingMode}:{pricing.IsTaxInclusive}";

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
