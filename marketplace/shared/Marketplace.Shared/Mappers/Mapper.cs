using Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value;
using Enterprise.Shared;
using Google.Protobuf.WellKnownTypes;
using CdnFile = Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.CdnFile;
using CdnImageFile = Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.CdnImageFile;
using Product = Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.Product;
using ProductVersion = Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.ProductVersion;
using ProductPricing = Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.ProductPricing;
using ProductPricingBillingMode = Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.ProductPricingBillingMode;
using Currency = Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.Currency;
using ProductPricingCadence = Api.Shared.Services.Models.ProductPricingCadence;

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
        var productVersion = new ProductVersion { Id = src.Id, ListingMetadata = MapTo(src.ListingMetadata), Currency = MapTo(src.Currency) };

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
            ListingMetadata = MapTo(src.ListingMetadata),
            Cadence = src.Cadence switch
            {
                ProductPricingCadence.NotSet => Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.ProductPricingCadence.NotSet,
                ProductPricingCadence.OneTime => Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.ProductPricingCadence.OneTime,
                ProductPricingCadence.PerMinute => Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.ProductPricingCadence.PerMinute,
                ProductPricingCadence.Per15Minutes => Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.ProductPricingCadence.Per15Minutes,
                ProductPricingCadence.Per30Minutes => Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.ProductPricingCadence.Per30Minutes,
                ProductPricingCadence.PerHour => Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.ProductPricingCadence.PerHour,
                ProductPricingCadence.HalfDay => Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.ProductPricingCadence.HalfDay,
                ProductPricingCadence.Daily => Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.ProductPricingCadence.Daily,
                ProductPricingCadence.Weekly => Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.ProductPricingCadence.Weekly,
                ProductPricingCadence.Fortnightly => Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.ProductPricingCadence.Fortnightly,
                ProductPricingCadence.Monthly => Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.ProductPricingCadence.Monthly,
                ProductPricingCadence.TwoMonths => Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.ProductPricingCadence.TwoMonths,
                ProductPricingCadence.Quarterly => Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.ProductPricingCadence.Quarterly,
                ProductPricingCadence.FourMonths => Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.ProductPricingCadence.FourMonths,
                ProductPricingCadence.FiveMonths => Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.ProductPricingCadence.FiveMonths,
                ProductPricingCadence.SixMonths => Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.ProductPricingCadence.SixMonths,
                ProductPricingCadence.Yearly => Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.ProductPricingCadence.Yearly,
                _ => throw new ArgumentOutOfRangeException()
            },
            Price = Convert.ToDouble(src.Price),
            IsTaxInclusive = src.IsTaxInclusive,
            MinDurationMinutes = src.MinDurationMinutes.ToNullInt(),
            MaxDurationMinutes = src.MaxDurationMinutes.ToNullInt(),
            MaxAllowedResourcesLockTimePaidViaCard = src.MaxAllowedResourcesLockTimePaidViaCard,
            MaxAllowedResourcesLockTimePaidViaBankTransfer = src.MaxAllowedResourcesLockTimePaidViaBankTransfer,
            NumberOfResourcesToBook = src.NumberOfResourcesToBook,
            BillingMode = MapTo(src.BillingMode)
        };

        return productPricing;
    }

    private static ProductPricingBillingMode MapTo(Api.Shared.Services.Models.ProductPricingBillingMode src) =>
        src switch
        {
            Api.Shared.Services.Models.ProductPricingBillingMode.NotSet => ProductPricingBillingMode.NotSet,
            Api.Shared.Services.Models.ProductPricingBillingMode.Upfront => ProductPricingBillingMode.Upfront,
            Api.Shared.Services.Models.ProductPricingBillingMode.InArrears => ProductPricingBillingMode.InArrears,
            _ => throw new ArgumentOutOfRangeException(nameof(src), src, null)
        };

    private static Currency MapTo(Api.Shared.Services.Models.Currency src) =>
        src switch
        {
            Api.Shared.Services.Models.Currency.Nzd => Currency.Nzd,
            Api.Shared.Services.Models.Currency.Usd => Currency.Usd,
            _ => throw new ArgumentOutOfRangeException(nameof(src), src, null)
        };

    private static ListingMetadata MapTo(Api.Shared.Services.Models.ListingMetadata src)
    {
        var listingMetadata = new ListingMetadata
        {
            About = src.About.ToSafeString(), Title = src.Title.ToSafeString(), SubTitle = src.SubTitle.ToSafeString()
        };

        listingMetadata.IncludedFeatures.AddRange(src.IncludedFeatures.ToSafeCollection().Select(item => item.ToSafeString()));

        return listingMetadata;
    }
}
