using Api.Shared.Services;
using Api.Shared.Services.Models;
using Enterprise.Shared;
using HotChocolate.Types.Pagination;
using Marketplace.Api.GraphQL.Product;
using Customer = Marketplace.Shared.Models.Customer;
using Organization = Marketplace.Shared.Database.Entities.Organization;
using OrganizationTag = Marketplace.Shared.Database.Entities.OrganizationTag;
using Product = Marketplace.Shared.Models.Product;
using ProductVersion = Marketplace.Shared.Models.ProductVersion;

namespace Marketplace.Api.Mappers;

public interface IMapper
{
    Customer? MapTo(Shared.Database.Entities.Customer? src);
    Product MapTo(Shared.Database.Entities.Product src);
    ProductVersion MapTo(Shared.Database.Entities.ProductVersion src);
    ProductVersion MapTo(AddProductInput src);
    ProductVersion MapTo(UpdateProductInput src);
    ProductDetails? MapTo(Product? src);
    ProductVersionDetails? MapTo(ProductVersion? src);

    Shared.Database.Entities.Product MapTo(
        Product src,
        ProductVersion productVersion,
        Organization organization,
        ICollection<OrganizationTag> productTags,
        ICollection<OrganizationTag> locationTags);

    Shared.Database.Entities.Product MergeTo(Shared.Database.Entities.Product dest, Organization organization);

    ProductEdge MapTo(Edge<Product> src);
    Product MapTo(Shared.Database.Entities.Product src, Shared.Models.Organization organization);
    Shared.Models.Organization MapTo(Organization src);

    Shared.Database.Entities.ProductVersion MapTo(
        ProductVersion src,
        Shared.Database.Entities.Product product,
        ICollection<OrganizationTag> productTags,
        ICollection<OrganizationTag> locationTags);
}

public class Mapper : IMapper
{
    public Customer? MapTo(Shared.Database.Entities.Customer? src) =>
        src is null
            ? null
            : new Customer
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                EventRaisedAt = src.EventRaisedAt,
                Type = src.Type.ToNullableCustomerType()
            };

    public Product MapTo(Shared.Database.Entities.Product src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Inactive = src.Inactive,
            Organization = MapTo(src.Organization),
            ProductVersions = MapTo(src.ProductVersions).ToList()
        };

    public ProductVersion MapTo(Shared.Database.Entities.ProductVersion src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            Name = src.Name,
            Description = src.Description,
            Price = src.Price,
            PriceUnit = src.PriceUnit.ToPriceUnit(),
            IsPriceTaxInclusive = src.IsPriceTaxInclusive,
            Currency = src.Currency.ToCurrency(),
            MinDurationMinutes = src.MinDurationMinutes,
            MaxDurationMinutes = src.MaxDurationMinutes,
            BookAllLocationResources = src.BookAllLocationResources,
            NumberOfResourcesToBook = src.NumberOfResourcesToBook,
            FeatureImages = src.FeatureImages.ToSafeCollection(),
            MaxAllowedResourcesLockTimePaidViaCard = src.MaxAllowedResourcesLockTimePaidViaCard,
            MaxAllowedResourcesLockTimePaidViaBankTransfer = src.MaxAllowedResourcesLockTimePaidViaBankTransfer,
            AcceptedBookingPaymentMethods = src.AcceptedBookingPaymentMethods.Select(item => item.ToPaymentMethod()).ToList(),
            ProductTags = MapTo(src.ProductTags).ToList(),
            LocationTags = MapTo(src.LocationTags).ToList()
        };

    public ProductVersion MapTo(AddProductInput src) =>
        new()
        {
            Name = src.Name,
            Description = src.Description,
            Price = decimal.Parse(src.Price),
            PriceUnit = src.PriceUnit,
            IsPriceTaxInclusive = src.IsPriceTaxInclusive,
            Currency = src.Currency,
            MinDurationMinutes = src.MinDurationMinutes,
            MaxDurationMinutes = src.MaxDurationMinutes,
            BookAllLocationResources = src.BookAllLocationResources,
            NumberOfResourcesToBook = src.NumberOfResourcesToBook,
            FeatureImages = src.FeatureImages,
            MaxAllowedResourcesLockTimePaidViaCard = src.MaxAllowedResourcesLockTimePaidViaCard,
            MaxAllowedResourcesLockTimePaidViaBankTransfer = src.MaxAllowedResourcesLockTimePaidViaBankTransfer,
            AcceptedBookingPaymentMethods = src.AcceptedBookingPaymentMethods.ToList(),
            ProductTags = src.ProductTagIds.Select(item => new Shared.Models.OrganizationTag { Id = item }).ToList(),
            LocationTags = src.LocationTagIds.Select(item => new Shared.Models.OrganizationTag { Id = item }).ToList()
        };

    public ProductVersion MapTo(UpdateProductInput src) =>
        new()
        {
            Name = src.Name,
            Description = src.Description,
            Price = decimal.Parse(src.Price),
            PriceUnit = src.PriceUnit,
            IsPriceTaxInclusive = src.IsPriceTaxInclusive,
            Currency = src.Currency,
            MinDurationMinutes = src.MinDurationMinutes,
            MaxDurationMinutes = src.MaxDurationMinutes,
            BookAllLocationResources = src.BookAllLocationResources,
            NumberOfResourcesToBook = src.NumberOfResourcesToBook,
            FeatureImages = src.FeatureImages,
            MaxAllowedResourcesLockTimePaidViaCard = src.MaxAllowedResourcesLockTimePaidViaCard,
            MaxAllowedResourcesLockTimePaidViaBankTransfer = src.MaxAllowedResourcesLockTimePaidViaBankTransfer,
            AcceptedBookingPaymentMethods = src.AcceptedBookingPaymentMethods.ToList(),
            ProductTags = src.ProductTagIds.Select(item => new Shared.Models.OrganizationTag { Id = item }).ToList(),
            LocationTags = src.LocationTagIds.Select(item => new Shared.Models.OrganizationTag { Id = item }).ToList()
        };

    public ProductDetails? MapTo(Product? src)
    {
        if (src is null || src.ProductVersions.Count == 0)
        {
            return null;
        }

        var productVersion = src.ProductVersions.First();

        var roundedPrice = productVersion.Price.ToRoundedPrice();

        return new ProductDetails
        {
            Id = src.Id,
            Inactive = src.Inactive,
            Name = productVersion.Name,
            Description = productVersion.Description,
            Price = roundedPrice,
            PriceToDisplay = roundedPrice.ToPriceToDisplay(productVersion.Currency),
            CurrencyToDisplay = productVersion.Currency.ToCurrencyToDisplay(),
            PriceUnit = new PriceUnitDetails { Type = productVersion.PriceUnit, Name = productVersion.PriceUnit.ToPriceUnitName() },
            IsPriceTaxInclusive = productVersion.IsPriceTaxInclusive,
            Currency = new CurrencyDetails { Type = productVersion.Currency, Name = productVersion.Currency.ToCurrencyName() },
            MinDurationMinutes = productVersion.MinDurationMinutes,
            MaxDurationMinutes = productVersion.MaxDurationMinutes,
            BookAllLocationResources = productVersion.BookAllLocationResources,
            NumberOfResourcesToBook = productVersion.NumberOfResourcesToBook,
            FeatureImages = productVersion.FeatureImages,
            MaxAllowedResourcesLockTimePaidViaCard = productVersion.MaxAllowedResourcesLockTimePaidViaCard,
            MaxAllowedResourcesLockTimePaidViaBankTransfer = productVersion.MaxAllowedResourcesLockTimePaidViaBankTransfer,
            AcceptedBookingPaymentMethods = productVersion.AcceptedBookingPaymentMethods
                .Select(item => new PaymentMethodTypeDetails { Type = item, Name = item.ToPaymentMethodName() }),
            ProductTagIds = productVersion.ProductTags.Select(item => item.Id),
            LocationTagIds = productVersion.LocationTags.Select(item => item.Id),
            OrganizationId = src.Organization.Id,
            OrganizationUniqueAlphanumericName = src.Organization.UniqueAlphanumericName.ToSafeString(),
            LatestProductVersionId = src.ProductVersions.OrderByDescending(item => item.CreatedAt).First().Id
        };
    }

    public ProductVersionDetails? MapTo(ProductVersion? src)
    {
        if (src is null)
        {
            return null;
        }

        var roundedPrice = src.Price.ToRoundedPrice();

        return new ProductVersionDetails
        {
            Id = src.Id,
            Name = src.Name,
            Description = src.Description,
            Price = roundedPrice,
            PriceToDisplay = roundedPrice.ToPriceToDisplay(src.Currency),
            CurrencyToDisplay = src.Currency.ToCurrencyToDisplay(),
            PriceUnit = new PriceUnitDetails { Type = src.PriceUnit, Name = src.PriceUnit.ToPriceUnitName() },
            IsPriceTaxInclusive = src.IsPriceTaxInclusive,
            Currency = new CurrencyDetails { Type = src.Currency, Name = src.Currency.ToCurrencyName() },
            MinDurationMinutes = src.MinDurationMinutes,
            MaxDurationMinutes = src.MaxDurationMinutes,
            BookAllLocationResources = src.BookAllLocationResources,
            NumberOfResourcesToBook = src.NumberOfResourcesToBook,
            FeatureImages = src.FeatureImages,
            MaxAllowedResourcesLockTimePaidViaCard = src.MaxAllowedResourcesLockTimePaidViaCard,
            MaxAllowedResourcesLockTimePaidViaBankTransfer = src.MaxAllowedResourcesLockTimePaidViaBankTransfer,
            AcceptedBookingPaymentMethods = src.AcceptedBookingPaymentMethods
                .Select(item => new PaymentMethodTypeDetails { Type = item, Name = item.ToPaymentMethodName() }),
            ProductTagIds = src.ProductTags.Select(item => item.Id),
            LocationTagIds = src.LocationTags.Select(item => item.Id)
        };
    }

    public Shared.Database.Entities.Product MapTo(
        Product src,
        ProductVersion productVersion,
        Organization organization,
        ICollection<OrganizationTag> productTags,
        ICollection<OrganizationTag> locationTags) =>
        MergeTo(
            src,
            productVersion,
            new Shared.Database.Entities.Product(),
            organization,
            productTags,
            locationTags);

    public Shared.Database.Entities.Product MergeTo(Shared.Database.Entities.Product dest, Organization organization)
    {
        dest.Organization = organization;
        return dest;
    }

    public ProductEdge MapTo(Edge<Product> src) => new(MapTo(src.Node)!, src.Cursor);

    public Product MapTo(Shared.Database.Entities.Product src, Shared.Models.Organization organization) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Inactive = src.Inactive,
            Organization = organization,
            ProductVersions = MapTo(src.ProductVersions).ToList()
        };

    public Shared.Models.Organization MapTo(Organization src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            EventRaisedAt = src.EventRaisedAt,
            UniqueAlphanumericName = src.UniqueAlphanumericName,
            Tags = MapTo(src.Tags).ToList()
        };

    public Shared.Database.Entities.ProductVersion MapTo(
        ProductVersion src,
        Shared.Database.Entities.Product product,
        ICollection<OrganizationTag> productTags,
        ICollection<OrganizationTag> locationTags) =>
        MergeTo(src, new Shared.Database.Entities.ProductVersion(), product, productTags, locationTags);

    private static IEnumerable<Shared.Models.OrganizationTag> MapTo(IEnumerable<OrganizationTag> src) => src.Select(MapTo);

    private static Shared.Models.OrganizationTag MapTo(OrganizationTag src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Name = src.Name,
            Type = src.Type.ToNullableOrganizationTagType(),
            Color = src.Color
        };

    private IEnumerable<ProductVersion> MapTo(IEnumerable<Shared.Database.Entities.ProductVersion> src) => src.Select(MapTo);

    private static Shared.Database.Entities.Product MergeTo(
        Product src,
        ProductVersion productVersion,
        Shared.Database.Entities.Product dest,
        Organization organization,
        ICollection<OrganizationTag> productTags,
        ICollection<OrganizationTag> locationTags)
    {
        dest.Id = src.Id;
        dest.Inactive = src.Inactive;
        dest.Organization = organization;
        return dest;
    }

    private static Shared.Database.Entities.ProductVersion MergeTo(
        ProductVersion src,
        Shared.Database.Entities.ProductVersion dest,
        Shared.Database.Entities.Product product,
        ICollection<OrganizationTag> productTags,
        ICollection<OrganizationTag> locationTags)
    {
        dest.Id = src.Id;
        dest.Name = src.Name;
        dest.Description = src.Description;
        dest.Price = src.Price;
        dest.PriceUnit = src.PriceUnit.ToPriceUnit();
        dest.IsPriceTaxInclusive = src.IsPriceTaxInclusive;
        dest.Currency = src.Currency.ToCurrency();
        dest.MinDurationMinutes = src.MinDurationMinutes;
        dest.MaxDurationMinutes = src.MaxDurationMinutes;
        dest.BookAllLocationResources = src.BookAllLocationResources;
        dest.NumberOfResourcesToBook = src.NumberOfResourcesToBook;
        dest.FeatureImages = src.FeatureImages;
        dest.MaxAllowedResourcesLockTimePaidViaCard = src.MaxAllowedResourcesLockTimePaidViaCard;
        dest.MaxAllowedResourcesLockTimePaidViaBankTransfer = src.MaxAllowedResourcesLockTimePaidViaBankTransfer;
        dest.AcceptedBookingPaymentMethods = src.AcceptedBookingPaymentMethods.Select(item => item.ToPaymentMethod()).ToList();
        dest.ProductTags = productTags;
        dest.LocationTags = locationTags;
        dest.Product = product;
        return dest;
    }
}
