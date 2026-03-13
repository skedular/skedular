using Api.Shared.Services.Models;
using Enterprise.Shared;
using Google.Protobuf.WellKnownTypes;
using CdnFile = Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.CdnFile;
using CdnImageFile = Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.CdnImageFile;
using Product = Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.Product;
using ProductVersion = Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.ProductVersion;
using ProductPricing = Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.ProductPricing;
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
            Id = src.Id, Name = src.Name.ToSafeString(), ListingMetadata = MapTo(src.ListingMetadata), Currency = MapTo(src.Currency)
        };

        productVersion.TagIds.AddRange(src.OrganizationTags.Select(item => item.Id));
        productVersion.FeatureImages.AddRange(MapTo(src.FeatureImages));
        productVersion.PricingOptions.AddRange(MapTo(src.PricingOptions));

        return productVersion;
    }

    private static IEnumerable<CdnImageFile> MapTo(IEnumerable<Api.Shared.Services.Models.CdnImageFile> src) =>
        src.Select(MapTo);

    private static CdnImageFile MapTo(Api.Shared.Services.Models.CdnImageFile src) =>
        new() { Original = MapTo(src.Original), Thumbnail = MapTo(src.Thumbnail) };

    private static CdnFile? MapTo(Api.Shared.Services.Models.CdnFile? src) =>
        src is null ? null : new CdnFile { Url = src.Url.ToSafeString(), Height = src.Height.ToNullInt(), Width = src.Width.ToNullInt() };

    private static IEnumerable<ProductPricing> MapTo(IEnumerable<Api.Shared.Services.Models.ProductPricing> src) =>
        src.Select(MapTo);

    private static ProductPricing MapTo(Api.Shared.Services.Models.ProductPricing src)
    {
        var productPricing = new ProductPricing
        {
            Id = src.Id,
            Index = src.Index,
            Name = src.Name.ToSafeString(),
            ListingMetadata = MapTo(src.ListingMetadata),
            Cadence = src.Cadence switch
            {
                ProductPricingCadence.OneTimeV1 => Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.ProductPricingCadence.OneTimeV1,
                ProductPricingCadence.PerMinuteV1 => Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.ProductPricingCadence.PerMinuteV1,
                ProductPricingCadence.PerHourV1 => Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.ProductPricingCadence.PerHourV1,
                ProductPricingCadence.DailyV1 => Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.ProductPricingCadence.DailyV1,
                ProductPricingCadence.WeeklyV1 => Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.ProductPricingCadence.WeeklyV1,
                ProductPricingCadence.MonthlyV1 => Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.ProductPricingCadence.MonthlyV1,
                _ => throw new ArgumentOutOfRangeException()
            },
            Price = Convert.ToDouble(src.Price),
            IsTaxInclusive = src.IsTaxInclusive,
            MinDurationMinutes = src.MinDurationMinutes.ToNullInt(),
            MaxDurationMinutes = src.MaxDurationMinutes.ToNullInt(),
            MaxAllowedResourcesLockTimePaidViaCard = src.MaxAllowedResourcesLockTimePaidViaCard,
            MaxAllowedResourcesLockTimePaidViaBankTransfer = src.MaxAllowedResourcesLockTimePaidViaBankTransfer,
            NumberOfResourcesToBook = src.NumberOfResourcesToBook
        };

        productPricing.AcceptedBookingPaymentMethods.AddRange(MapTo(src.AcceptedPaymentMethods));

        return productPricing;
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
    
    private static Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.ListingMetadata MapTo(ListingMetadata src)
    {
        var listingMetadata = new Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.ListingMetadata
        {
            About = src.About.ToSafeString(), Title = src.Title.ToSafeString(), SubTitle = src.SubTitle.ToSafeString()
        };

        listingMetadata.IncludedFeatures.AddRange(src.IncludedFeatures.ToSafeCollection().Select(item => item.ToSafeString()));

        return listingMetadata;
    }
}
