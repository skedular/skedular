using Api.Shared.Services.Models;
using Enterprise.Shared;
using Google.Protobuf.WellKnownTypes;
using CdnFile = Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.CdnFile;
using CdnImageFile = Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.CdnImageFile;
using Product = Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.Product;
using ProductVersion = Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.ProductVersion;
using ProductVersionOneTimePricingV1 = Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.ProductVersionOneTimePricingV1;
using ProductVersionPerMinutePricingV1 = Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.ProductVersionPerMinutePricingV1;
using ProductVersionDailyPricingV1 = Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.ProductVersionDailyPricingV1;
using ProductVersionWeeklyPricingV1 = Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.ProductVersionWeeklyPricingV1;
using ProductVersionMonthlyPricingV1 = Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.ProductVersionMonthlyPricingV1;
using ProductVersionPricingOptions = Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.ProductVersionPricingOptions;
using PaymentMethod = Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.PaymentMethod;
using Currency = Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.Currency;

namespace Marketplace.Shared.Mappers;

public interface IMapper
{
    Product MapTo(Models.Product src);
}

public class Mapper : IMapper
{
    public Product MapTo(Models.Product src) =>
        new()
        {
            Id = src.Id,
            DeletedAt = src.DeletedAt?.ToTimestamp(),
            Inactive = src.Inactive,
            OrganizationId = src.Organization.Id,
            LatestProductVersion = MapTo(src.ProductVersions.OrderByDescending(item => item.CreatedAt).First())
        };

    private static ProductVersion MapTo(Models.ProductVersion src)
    {
        var productVersion = new ProductVersion
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Description = src.Name.ToSafeString(),
            Price = Convert.ToDouble(src.Price),
            PriceUnit = src.PriceUnit.ToPriceUnit(),
            IsPriceTaxInclusive = src.IsPriceTaxInclusive,
            Currency = src.Currency.ToCurrency(),
            MinDurationMinutes = src.MinDurationMinutes ?? -1,
            MaxDurationMinutes = src.MaxDurationMinutes ?? -1,
            BookAllLocationResources = src.BookAllLocationResources,
            NumberOfResourcesToBook = src.NumberOfResourcesToBook,
            MaxAllowedResourcesLockTimePaidViaCard = src.MaxAllowedResourcesLockTimePaidViaCard,
            MaxAllowedResourcesLockTimePaidViaBankTransfer = src.MaxAllowedResourcesLockTimePaidViaBankTransfer
        };

        productVersion.ProductTagIds.AddRange(src.ProductTags.Select(item => item.Id));
        productVersion.LocationTagIds.AddRange(src.LocationTags.Select(item => item.Id));
        productVersion.AcceptedBookingPaymentMethods.AddRange(src.AcceptedBookingPaymentMethods.Select(item => item.ToPaymentMethod()));
        productVersion.FeatureImages.AddRange(MapTo(src.FeatureImages));
        productVersion.PricingOptions.AddRange(MapTo(src.PricingOptions));

        return productVersion;
    }

    private static IEnumerable<CdnImageFile> MapTo(IEnumerable<Api.Shared.Services.Models.CdnImageFile> src) =>
        src.Select(MapTo)!;

    private static CdnImageFile? MapTo(Api.Shared.Services.Models.CdnImageFile? src) =>
        src is null ? null : new CdnImageFile { Original = MapTo(src.Original), Thumbnail = MapTo(src.Thumbnail) };

    private static CdnFile? MapTo(Api.Shared.Services.Models.CdnFile? src) =>
        src is null ? null : new CdnFile { Url = src.Url.ToSafeString(), Height = src.Height.ToNullInt(), Width = src.Width.ToNullInt() };

    private static IEnumerable<ProductVersionPricingOptions> MapTo(IEnumerable<Api.Shared.Services.Models.ProductVersionPricingOptions> src) =>
        src.Select(MapTo);

    private static ProductVersionPricingOptions MapTo(Api.Shared.Services.Models.ProductVersionPricingOptions src) =>
        new()
        {
            Cadence = src.Cadence switch
            {
                ProductVersionPricingCadence.OneTimeV1 => Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.ProductVersionPricingCadence
                    .OneTimeV1,
                ProductVersionPricingCadence.PerMinuteV1 => Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.ProductVersionPricingCadence
                    .PerMinuteV1,
                ProductVersionPricingCadence.DailyV1 => Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.ProductVersionPricingCadence.DailyV1,
                ProductVersionPricingCadence.WeeklyV1 => Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.ProductVersionPricingCadence
                    .WeeklyV1,
                ProductVersionPricingCadence.MonthlyV1 => Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.ProductVersionPricingCadence
                    .MonthlyV1,
                _ => throw new ArgumentOutOfRangeException()
            },
            OneTimeV1 = MapTo(src.OneTimeV1),
            PerMinuteV1 = MapTo(src.PerMinuteV1),
            DailyV1 = MapTo(src.DailyV1),
            WeeklyV1 = MapTo(src.WeeklyV1),
            MonthlyV1 = MapTo(src.MonthlyV1)
        };

    private static ProductVersionOneTimePricingV1? MapTo(Api.Shared.Services.Models.ProductVersionOneTimePricingV1? src)
    {
        if (src is null)
        {
            return null;
        }

        var pricing = new ProductVersionOneTimePricingV1
        {
            Index = src.Index,
            Name = src.Name.ToSafeString(),
            Description = src.Description.ToSafeString(),
            Price = Convert.ToDouble(src.Price),
            IsTaxInclusive = src.IsTaxInclusive,
            Currency = MapTo(src.Currency)
        };

        pricing.AcceptedBookingPaymentMethods.AddRange(MapTo(src.AcceptedPaymentMethods));

        return pricing;
    }

    private static ProductVersionPerMinutePricingV1? MapTo(Api.Shared.Services.Models.ProductVersionPerMinutePricingV1? src)
    {
        if (src is null)
        {
            return null;
        }

        var pricing = new ProductVersionPerMinutePricingV1
        {
            Index = src.Index,
            Name = src.Name.ToSafeString(),
            Description = src.Description.ToSafeString(),
            Price = Convert.ToDouble(src.Price),
            IsTaxInclusive = src.IsTaxInclusive,
            Currency = MapTo(src.Currency)
        };

        pricing.AcceptedBookingPaymentMethods.AddRange(MapTo(src.AcceptedPaymentMethods));

        return pricing;
    }

    private static ProductVersionDailyPricingV1? MapTo(Api.Shared.Services.Models.ProductVersionDailyPricingV1? src)
    {
        if (src is null)
        {
            return null;
        }

        var pricing = new ProductVersionDailyPricingV1
        {
            Index = src.Index,
            Name = src.Name.ToSafeString(),
            Description = src.Description.ToSafeString(),
            Price = Convert.ToDouble(src.Price),
            IsTaxInclusive = src.IsTaxInclusive,
            Currency = MapTo(src.Currency)
        };

        pricing.AcceptedBookingPaymentMethods.AddRange(MapTo(src.AcceptedPaymentMethods));

        return pricing;
    }

    private static ProductVersionWeeklyPricingV1? MapTo(Api.Shared.Services.Models.ProductVersionWeeklyPricingV1? src)
    {
        if (src is null)
        {
            return null;
        }

        var pricing = new ProductVersionWeeklyPricingV1
        {
            Index = src.Index,
            Name = src.Name.ToSafeString(),
            Description = src.Description.ToSafeString(),
            Price = Convert.ToDouble(src.Price),
            IsTaxInclusive = src.IsTaxInclusive,
            Currency = MapTo(src.Currency)
        };

        pricing.AcceptedBookingPaymentMethods.AddRange(MapTo(src.AcceptedPaymentMethods));

        return pricing;
    }

    private static ProductVersionMonthlyPricingV1? MapTo(Api.Shared.Services.Models.ProductVersionMonthlyPricingV1? src)
    {
        if (src is null)
        {
            return null;
        }

        var pricing = new ProductVersionMonthlyPricingV1
        {
            Index = src.Index,
            Name = src.Name.ToSafeString(),
            Description = src.Description.ToSafeString(),
            Price = Convert.ToDouble(src.Price),
            IsTaxInclusive = src.IsTaxInclusive,
            Currency = MapTo(src.Currency)
        };

        pricing.AcceptedBookingPaymentMethods.AddRange(MapTo(src.AcceptedPaymentMethods));

        return pricing;
    }

    private static IEnumerable<PaymentMethod> MapTo(IEnumerable<Api.Shared.Services.Models.PaymentMethod> src) =>
        src.Select(MapTo);

    private static PaymentMethod MapTo(Api.Shared.Services.Models.PaymentMethod src) =>
        src switch
        {
            Api.Shared.Services.Models.PaymentMethod.Card => PaymentMethod.Card,
            Api.Shared.Services.Models.PaymentMethod.BankTransfer => PaymentMethod.BankTransfer,
            _ => throw new ArgumentOutOfRangeException(nameof(src), src, null)
        };

    private static Currency MapTo(Api.Shared.Services.Models.Currency src) =>
        src switch
        {
            Api.Shared.Services.Models.Currency.Nzd => Currency.Nzd,
            Api.Shared.Services.Models.Currency.Usd => Currency.Usd,
            _ => throw new ArgumentOutOfRangeException(nameof(src), src, null)
        };
}
