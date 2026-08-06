using Api.Shared.Services.Models;
using Enterprise.Shared;
using Google.Protobuf.WellKnownTypes;
using CdnFile = Api.Shared.Clients.Events.Skedular.Marketplace.V1.CdnFile;
using CdnImageFile = Api.Shared.Clients.Events.Skedular.Marketplace.V1.CdnImageFile;
using Product = Api.Shared.Clients.Events.Skedular.Marketplace.V1.Product;
using ProductVersion = Api.Shared.Clients.Events.Skedular.Marketplace.V1.ProductVersion;
using ProductPricing = Api.Shared.Clients.Events.Skedular.Marketplace.V1.ProductPricing;
using ProductPricingBillingMode = Api.Shared.Clients.Events.Skedular.Marketplace.V1.ProductPricingBillingMode;
using Currency = Api.Shared.Clients.Events.Skedular.Marketplace.V1.Currency;
using ListingMetadata = Api.Shared.Clients.Events.Skedular.Marketplace.V1.ListingMetadata;
using PaymentMethod = Api.Shared.Clients.Events.Skedular.Marketplace.V1.PaymentMethod;
using ProductType = Api.Shared.Clients.Events.Skedular.Marketplace.V1.ProductType;
using ProductPricingCadence = Api.Shared.Services.Models.ProductPricingCadence;
using ProductPricingCancellationPolicyType = Api.Shared.Clients.Events.Skedular.Marketplace.V1.ProductPricingCancellationPolicyType;
using ProductPricingCancellationRefundRule = Api.Shared.Clients.Events.Skedular.Marketplace.V1.ProductPricingCancellationRefundRule;

namespace Marketplace.Shared.Mappers;

public interface IEventMapper
{
    Product MapTo(Models.Product src);
}

public class EventMapper : IEventMapper
{
    public Product MapTo(Models.Product src) =>
        new()
        {
            Id = src.Id,
            DeletedAt = src.DeletedAt?.ToTimestamp(),
            Inactive = src.Inactive,
            OrganizationId = src.Organization.Id,
            LatestProductVersion = MapTo(src.ProductVersions.OrderByDescending(item => item.CreatedAt).First()),
        };

    private static ProductVersion MapTo(Models.ProductVersion src)
    {
        var productVersion = new ProductVersion
        {
            Id = src.Id,
            ListingMetadata = MapTo(src.ListingMetadata),
            Type = MapTo(src.Type),
            Currency = MapTo(src.Currency),
        };

        productVersion.TagIds.AddRange(src.OrganizationTags.Select(item => item.Id));
        productVersion.FeatureImages.AddRange(MapTo(src.FeatureImages));
        productVersion.PricingOptions.AddRange(MapTo(src.PricingOptions));

        return productVersion;
    }

    private static IEnumerable<CdnImageFile> MapTo(IEnumerable<Api.Shared.Services.Models.CdnImageFile> src) =>
        src.Select(MapTo);

    private static CdnImageFile MapTo(Api.Shared.Services.Models.CdnImageFile src) =>
        new()
        {
            Original = MapTo(src.Original),
            Thumbnail = MapTo(src.Thumbnail),
        };

    private static CdnFile? MapTo(Api.Shared.Services.Models.CdnFile? src) =>
        src is null
            ? null
            : new CdnFile
            {
                Url = src.Url.ToSafeString(),
                Height = src.Height.ToNullInt(),
                Width = src.Width.ToNullInt(),
            };

    private static IEnumerable<ProductPricing> MapTo(IEnumerable<Api.Shared.Services.Models.ProductPricing> src) =>
        src.Select(MapTo);

    private static ProductPricing MapTo(Api.Shared.Services.Models.ProductPricing src)
    {
        var productPricing = new ProductPricing
        {
            Id = src.Id,
            Index = src.Index,
            ListingMetadata = MapTo(src.ListingMetadata),
            PurchaseCadence = MapTo(src.PurchaseCadence),
            BookingCadence = MapTo(src.BookingCadence),
            Price = Convert.ToDouble(src.Price),
            IsTaxInclusive = src.IsTaxInclusive,
            SupportsSubscriptionAutoRenewal = src.SupportsSubscriptionAutoRenewal,
            MinDurationMinutes = src.MinDurationMinutes.ToNullInt(),
            MaxDurationMinutes = src.MaxDurationMinutes.ToNullInt(),
            MaxAllowedResourcesLockTimePaidViaCard = src.MaxAllowedResourcesLockTimePaidViaCard,
            MaxAllowedResourcesLockTimePaidViaBankTransfer = src.MaxAllowedResourcesLockTimePaidViaBankTransfer,
            NumberOfResourcesToBook = src.NumberOfResourcesToBook,
            CancellationPolicyType = MapTo(src.CancellationPolicyType),
            BillingMode = MapTo(src.BillingMode),
            RequiredDaysPerWeek = src.RequiredDaysPerWeek.ToNullInt(),
        };

        productPricing.AcceptedBookingPaymentMethods.AddRange(MapTo(src.AcceptedPaymentMethods));
        productPricing.CancellationRefundRules.AddRange(MapTo(src.CancellationRefundRules));
        productPricing.AvailableDays.AddRange((src.AvailableDays ?? []).Select(item => item.ToDayOfWeek()));

        return productPricing;
    }

    private static IEnumerable<ProductPricingCancellationRefundRule> MapTo(
        IEnumerable<Api.Shared.Services.Models.ProductPricingCancellationRefundRule> src) =>
        src.Select(MapTo);

    private static ProductPricingCancellationRefundRule MapTo(Api.Shared.Services.Models.ProductPricingCancellationRefundRule src) =>
        new()
        {
            MinutesBefore = src.MinutesBefore,
            RefundPercentage = src.RefundPercentage,
        };

    private static IEnumerable<PaymentMethod> MapTo(IEnumerable<Api.Shared.Services.Models.PaymentMethod> src) =>
        src.Select(MapTo);

    private static PaymentMethod MapTo(Api.Shared.Services.Models.PaymentMethod src) =>
        src switch
        {
            Api.Shared.Services.Models.PaymentMethod.Card => PaymentMethod.Card,
            Api.Shared.Services.Models.PaymentMethod.BankTransfer => PaymentMethod.BankTransfer,
            _ => throw new ArgumentOutOfRangeException(nameof(src), src,
                $"Unexpected value for {nameof(src)}: {src}. Update enum mapping or caller input."),
        };

    private static Api.Shared.Clients.Events.Skedular.Marketplace.V1.ProductPricingCadence MapTo(ProductPricingCadence src) =>
        src switch
        {
            ProductPricingCadence.NotSet => Api.Shared.Clients.Events.Skedular.Marketplace.V1.ProductPricingCadence.NotSet,
            ProductPricingCadence.OneTime => Api.Shared.Clients.Events.Skedular.Marketplace.V1.ProductPricingCadence.OneTime,
            ProductPricingCadence.PerMinute => Api.Shared.Clients.Events.Skedular.Marketplace.V1.ProductPricingCadence.PerMinute,
            ProductPricingCadence.Per15Minutes => Api.Shared.Clients.Events.Skedular.Marketplace.V1.ProductPricingCadence.Per15Minutes,
            ProductPricingCadence.Per30Minutes => Api.Shared.Clients.Events.Skedular.Marketplace.V1.ProductPricingCadence.Per30Minutes,
            ProductPricingCadence.PerHour => Api.Shared.Clients.Events.Skedular.Marketplace.V1.ProductPricingCadence.PerHour,
            ProductPricingCadence.HalfDay => Api.Shared.Clients.Events.Skedular.Marketplace.V1.ProductPricingCadence.HalfDay,
            ProductPricingCadence.Daily => Api.Shared.Clients.Events.Skedular.Marketplace.V1.ProductPricingCadence.Daily,
            ProductPricingCadence.Weekly => Api.Shared.Clients.Events.Skedular.Marketplace.V1.ProductPricingCadence.Weekly,
            ProductPricingCadence.Fortnightly => Api.Shared.Clients.Events.Skedular.Marketplace.V1.ProductPricingCadence.Fortnightly,
            ProductPricingCadence.Monthly => Api.Shared.Clients.Events.Skedular.Marketplace.V1.ProductPricingCadence.Monthly,
            ProductPricingCadence.TwoMonths => Api.Shared.Clients.Events.Skedular.Marketplace.V1.ProductPricingCadence.TwoMonths,
            ProductPricingCadence.Quarterly => Api.Shared.Clients.Events.Skedular.Marketplace.V1.ProductPricingCadence.Quarterly,
            ProductPricingCadence.FourMonths => Api.Shared.Clients.Events.Skedular.Marketplace.V1.ProductPricingCadence.FourMonths,
            ProductPricingCadence.FiveMonths => Api.Shared.Clients.Events.Skedular.Marketplace.V1.ProductPricingCadence.FiveMonths,
            ProductPricingCadence.SixMonths => Api.Shared.Clients.Events.Skedular.Marketplace.V1.ProductPricingCadence.SixMonths,
            ProductPricingCadence.Yearly => Api.Shared.Clients.Events.Skedular.Marketplace.V1.ProductPricingCadence.Yearly,
            _ => throw new ArgumentOutOfRangeException(null,
                "Unexpected value encountered. Update enum mapping or caller input to include this case."),
        };

    private static ProductPricingBillingMode MapTo(Api.Shared.Services.Models.ProductPricingBillingMode src) =>
        src switch
        {
            Api.Shared.Services.Models.ProductPricingBillingMode.NotSet => ProductPricingBillingMode.NotSet,
            Api.Shared.Services.Models.ProductPricingBillingMode.Upfront => ProductPricingBillingMode.Upfront,
            Api.Shared.Services.Models.ProductPricingBillingMode.InArrears => ProductPricingBillingMode.InArrears,
            _ => throw new ArgumentOutOfRangeException(nameof(src), src,
                $"Unexpected value for {nameof(src)}: {src}. Update enum mapping or caller input."),
        };

    private static ProductPricingCancellationPolicyType MapTo(Api.Shared.Services.Models.ProductPricingCancellationPolicyType src) =>
        src switch
        {
            Api.Shared.Services.Models.ProductPricingCancellationPolicyType.NotSet => ProductPricingCancellationPolicyType.NotSet,
            Api.Shared.Services.Models.ProductPricingCancellationPolicyType.NoCancellation => ProductPricingCancellationPolicyType.NoCancellation,
            Api.Shared.Services.Models.ProductPricingCancellationPolicyType.FullRefundBeforeCutoff => ProductPricingCancellationPolicyType
                .FullRefundBeforeCutoff,
            Api.Shared.Services.Models.ProductPricingCancellationPolicyType.TieredRefund => ProductPricingCancellationPolicyType.TieredRefund,
            _ => throw new ArgumentOutOfRangeException(nameof(src), src,
                $"Unexpected value for {nameof(src)}: {src}. Update enum mapping or caller input."),
        };

    private static Currency MapTo(Api.Shared.Services.Models.Currency src) =>
        src switch
        {
            Api.Shared.Services.Models.Currency.Nzd => Currency.Nzd,
            Api.Shared.Services.Models.Currency.Usd => Currency.Usd,
            _ => throw new ArgumentOutOfRangeException(nameof(src), src,
                $"Unexpected value for {nameof(src)}: {src}. Update enum mapping or caller input."),
        };

    private static ListingMetadata MapTo(Api.Shared.Services.Models.ListingMetadata src)
    {
        var listingMetadata = new ListingMetadata
        {
            About = src.About.ToSafeString(),
            Title = src.Title.ToSafeString(),
            SubTitle = src.SubTitle.ToSafeString(),
        };

        listingMetadata.IncludedFeatures.AddRange(src.IncludedFeatures.ToSafeCollection().Select(item => item.ToSafeString()));

        return listingMetadata;
    }

    private static ProductType MapTo(Api.Shared.Services.Models.ProductType src) =>
        src switch
        {
            Api.Shared.Services.Models.ProductType.Resource => ProductType.Resource,
            Api.Shared.Services.Models.ProductType.Event => ProductType.Event,
            _ => throw new ArgumentOutOfRangeException(nameof(src), src,
                $"Unexpected value for {nameof(src)}: {src}. Update enum mapping or caller input."),
        };
}
