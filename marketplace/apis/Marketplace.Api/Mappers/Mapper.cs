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

    Shared.Database.Entities.Product MergeTo(
        ProductVersion src,
        Shared.Database.Entities.Product dest,
        Organization organization,
        ICollection<OrganizationTag> productTags,
        ICollection<OrganizationTag> locationTags);

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
                Timezone = src.Timezone
            };

    public Product MapTo(Shared.Database.Entities.Product src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Inactive = src.Inactive,
            Name = src.Name,
            Description = src.Description,
            Price = src.Price,
            PriceUnit = src.PriceUnit.ToPriceUnit(),
            IsPriceTaxInclusive = src.IsPriceTaxInclusive,
            Currency = src.Currency.ToCurrency(),
            MinDurationMinutes = src.MinDurationMinutes,
            MaxDurationMinutes = src.MaxDurationMinutes,
            BookAllLocationResources = src.BookAllLocationResources,
            RecurrenceWindowDays = src.RecurrenceWindowDays,
            RequireConsecutiveDays = src.RequireConsecutiveDays,
            MaxBookingSpreadDays = src.MaxBookingSpreadDays,
            NumberOfResourcesToBook = src.NumberOfResourcesToBook,
            PrimaryFeatureImage = src.PrimaryFeatureImage,
            MaxAllowedResourcesLockTimePaidViaCard = src.MaxAllowedResourcesLockTimePaidViaCard,
            MaxAllowedResourcesLockTimePaidViaBankTransfer = src.MaxAllowedResourcesLockTimePaidViaBankTransfer,
            AcceptedBookingPaymentMethods = src.AcceptedBookingPaymentMethods.Select(item => item.ToPaymentMethod()).ToList(),
            ProductTags = MapTo(src.ProductTags).ToList(),
            LocationTags = MapTo(src.LocationTags).ToList(),
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
            RecurrenceWindowDays = src.RecurrenceWindowDays,
            RequireConsecutiveDays = src.RequireConsecutiveDays,
            MaxBookingSpreadDays = src.MaxBookingSpreadDays,
            NumberOfResourcesToBook = src.NumberOfResourcesToBook,
            PrimaryFeatureImage = src.PrimaryFeatureImage,
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
            RecurrenceWindowDays = src.RecurrenceWindowDays,
            RequireConsecutiveDays = src.RequireConsecutiveDays,
            MaxBookingSpreadDays = src.MaxBookingSpreadDays,
            NumberOfResourcesToBook = src.NumberOfResourcesToBook,
            PrimaryFeatureImage = src.PrimaryFeatureImage,
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
            RecurrenceWindowDays = src.RecurrenceWindowDays,
            RequireConsecutiveDays = src.RequireConsecutiveDays,
            MaxBookingSpreadDays = src.MaxBookingSpreadDays,
            NumberOfResourcesToBook = src.NumberOfResourcesToBook,
            PrimaryFeatureImage = src.PrimaryFeatureImage,
            MaxAllowedResourcesLockTimePaidViaCard = src.MaxAllowedResourcesLockTimePaidViaCard,
            MaxAllowedResourcesLockTimePaidViaBankTransfer = src.MaxAllowedResourcesLockTimePaidViaBankTransfer,
            AcceptedBookingPaymentMethods = src.AcceptedBookingPaymentMethods.ToList(),
            ProductTags = src.ProductTagIds.Select(item => new Shared.Models.OrganizationTag { Id = item }).ToList(),
            LocationTags = src.LocationTagIds.Select(item => new Shared.Models.OrganizationTag { Id = item }).ToList()
        };

    public ProductDetails? MapTo(Product? src)
    {
        if (src is null)
        {
            return null;
        }

        var roundedPrice = src.Price.ToRoundedPrice();

        return new ProductDetails
        {
            Id = src.Id,
            Inactive = src.Inactive,
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
            RecurrenceWindowDays = src.RecurrenceWindowDays,
            RequireConsecutiveDays = src.RequireConsecutiveDays,
            MaxBookingSpreadDays = src.MaxBookingSpreadDays,
            NumberOfResourcesToBook = src.NumberOfResourcesToBook,
            PrimaryFeatureImage = src.PrimaryFeatureImage,
            MaxAllowedResourcesLockTimePaidViaCard = src.MaxAllowedResourcesLockTimePaidViaCard,
            MaxAllowedResourcesLockTimePaidViaBankTransfer = src.MaxAllowedResourcesLockTimePaidViaBankTransfer,
            AcceptedBookingPaymentMethods = src.AcceptedBookingPaymentMethods
                .Select(item => new PaymentMethodTypeDetails { Type = item, Name = item.ToPaymentMethodName() }),
            ProductTagIds = src.ProductTags.Select(item => item.Id),
            LocationTagIds = src.LocationTags.Select(item => item.Id),
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
            RecurrenceWindowDays = src.RecurrenceWindowDays,
            RequireConsecutiveDays = src.RequireConsecutiveDays,
            MaxBookingSpreadDays = src.MaxBookingSpreadDays,
            NumberOfResourcesToBook = src.NumberOfResourcesToBook,
            PrimaryFeatureImage = src.PrimaryFeatureImage,
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

    public Shared.Database.Entities.Product MergeTo(
        ProductVersion src,
        Shared.Database.Entities.Product dest,
        Organization organization,
        ICollection<OrganizationTag> productTags,
        ICollection<OrganizationTag> locationTags)
    {
        dest.Organization = organization;
        dest.Name = src.Name;
        dest.Description = src.Description;
        dest.Price = src.Price;
        dest.PriceUnit = src.PriceUnit.ToPriceUnit();
        dest.IsPriceTaxInclusive = src.IsPriceTaxInclusive;
        dest.Currency = src.Currency.ToCurrency();
        dest.MinDurationMinutes = src.MinDurationMinutes;
        dest.MaxDurationMinutes = src.MaxDurationMinutes;
        dest.BookAllLocationResources = src.BookAllLocationResources;
        dest.RecurrenceWindowDays = src.RecurrenceWindowDays;
        dest.RequireConsecutiveDays = src.RequireConsecutiveDays;
        dest.MaxBookingSpreadDays = src.MaxBookingSpreadDays;
        dest.NumberOfResourcesToBook = src.NumberOfResourcesToBook;
        dest.PrimaryFeatureImage = src.PrimaryFeatureImage;
        dest.MaxAllowedResourcesLockTimePaidViaCard = src.MaxAllowedResourcesLockTimePaidViaCard;
        dest.MaxAllowedResourcesLockTimePaidViaBankTransfer = src.MaxAllowedResourcesLockTimePaidViaBankTransfer;
        dest.AcceptedBookingPaymentMethods = src.AcceptedBookingPaymentMethods.Select(item => item.ToPaymentMethod()).ToList();
        dest.ProductTags = productTags;
        dest.LocationTags = locationTags;
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
            Name = src.Name,
            Description = src.Description,
            Price = src.Price,
            PriceUnit = src.PriceUnit.ToPriceUnit(),
            IsPriceTaxInclusive = src.IsPriceTaxInclusive,
            Currency = src.Currency.ToCurrency(),
            MinDurationMinutes = src.MinDurationMinutes,
            MaxDurationMinutes = src.MaxDurationMinutes,
            BookAllLocationResources = src.BookAllLocationResources,
            RecurrenceWindowDays = src.RecurrenceWindowDays,
            RequireConsecutiveDays = src.RequireConsecutiveDays,
            MaxBookingSpreadDays = src.MaxBookingSpreadDays,
            NumberOfResourcesToBook = src.NumberOfResourcesToBook,
            PrimaryFeatureImage = src.PrimaryFeatureImage,
            MaxAllowedResourcesLockTimePaidViaCard = src.MaxAllowedResourcesLockTimePaidViaCard,
            MaxAllowedResourcesLockTimePaidViaBankTransfer = src.MaxAllowedResourcesLockTimePaidViaBankTransfer,
            AcceptedBookingPaymentMethods = src.AcceptedBookingPaymentMethods.Select(item => item.ToPaymentMethod()).ToList(),
            ProductTags = MapTo(src.ProductTags).ToList(),
            LocationTags = MapTo(src.LocationTags).ToList(),
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

        dest.Name = productVersion.Name;
        dest.Description = productVersion.Description;
        dest.Price = productVersion.Price;
        dest.PriceUnit = productVersion.PriceUnit.ToPriceUnit();
        dest.IsPriceTaxInclusive = productVersion.IsPriceTaxInclusive;
        dest.Currency = productVersion.Currency.ToCurrency();
        dest.MinDurationMinutes = productVersion.MinDurationMinutes;
        dest.MaxDurationMinutes = productVersion.MaxDurationMinutes;
        dest.BookAllLocationResources = productVersion.BookAllLocationResources;
        dest.RecurrenceWindowDays = productVersion.RecurrenceWindowDays;
        dest.RequireConsecutiveDays = productVersion.RequireConsecutiveDays;
        dest.MaxBookingSpreadDays = productVersion.MaxBookingSpreadDays;
        dest.NumberOfResourcesToBook = productVersion.NumberOfResourcesToBook;
        dest.PrimaryFeatureImage = productVersion.PrimaryFeatureImage;
        dest.MaxAllowedResourcesLockTimePaidViaCard = productVersion.MaxAllowedResourcesLockTimePaidViaCard;
        dest.MaxAllowedResourcesLockTimePaidViaBankTransfer = productVersion.MaxAllowedResourcesLockTimePaidViaBankTransfer;
        dest.AcceptedBookingPaymentMethods = productVersion.AcceptedBookingPaymentMethods.Select(item => item.ToPaymentMethod()).ToList();
        dest.ProductTags = productTags;
        dest.LocationTags = locationTags;
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
        dest.RecurrenceWindowDays = src.RecurrenceWindowDays;
        dest.RequireConsecutiveDays = src.RequireConsecutiveDays;
        dest.MaxBookingSpreadDays = src.MaxBookingSpreadDays;
        dest.NumberOfResourcesToBook = src.NumberOfResourcesToBook;
        dest.PrimaryFeatureImage = src.PrimaryFeatureImage;
        dest.MaxAllowedResourcesLockTimePaidViaCard = src.MaxAllowedResourcesLockTimePaidViaCard;
        dest.MaxAllowedResourcesLockTimePaidViaBankTransfer = src.MaxAllowedResourcesLockTimePaidViaBankTransfer;
        dest.AcceptedBookingPaymentMethods = src.AcceptedBookingPaymentMethods.Select(item => item.ToPaymentMethod()).ToList();
        dest.ProductTags = productTags;
        dest.LocationTags = locationTags;
        dest.Product = product;
        return dest;
    }
}
