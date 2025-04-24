using Api.Shared.Services.Models;
using Enterprise.Shared;
using HotChocolate.Types.Pagination;
using Marketplace.Api.GraphQL;
using Marketplace.Shared.Models;
using Organization = Marketplace.Shared.Database.Entities.Organization;
using OrganizationStripeConnectAccount = Marketplace.Shared.Database.Entities.OrganizationStripeConnectAccount;
using OrganizationTag = Marketplace.Shared.Database.Entities.OrganizationTag;

namespace Marketplace.Api.Mappers;

public interface IMapper
{
    Customer? MapTo(Shared.Database.Entities.Customer? src);
    Product MapTo(Shared.Database.Entities.Product src);
    ProductVersion MapTo(AddProductInput src);
    ProductVersion MapTo(UpdateProductInput src);
    ProductDetails? MapTo(Product? src);

    Shared.Database.Entities.Product MapTo(
        Product src,
        ProductVersion productVersion,
        Organization organization,
        ICollection<OrganizationTag> productTags,
        ICollection<OrganizationTag> locationTags,
        OrganizationStripeConnectAccount? organizationStripeConnectAccount);

    Shared.Database.Entities.Product MergeTo(
        ProductVersion src,
        Shared.Database.Entities.Product dest,
        Organization organization,
        ICollection<OrganizationTag> productTags,
        ICollection<OrganizationTag> locationTags,
        OrganizationStripeConnectAccount? organizationStripeConnectAccount);

    ProductEdge MapTo(Edge<Product> src);
    Product MapTo(Shared.Database.Entities.Product src, Shared.Models.Organization organization);
    Shared.Models.Organization MapTo(Organization src);

    Shared.Database.Entities.ProductVersion MapTo(
        ProductVersion src,
        Shared.Database.Entities.Product product,
        ICollection<OrganizationTag> productTags,
        ICollection<OrganizationTag> locationTags,
        OrganizationStripeConnectAccount? organizationStripeConnectAccount);
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
            Currency = src.Currency.ToCurrency(),
            MinDurationMinutes = src.MinDurationMinutes,
            MaxDurationMinutes = src.MaxDurationMinutes,
            BookAllLocationResources = src.BookAllLocationResources,
            RecurrenceWindowDays = src.RecurrenceWindowDays,
            RequireConsecutiveDays = src.RequireConsecutiveDays,
            MaxBookingSpreadDays = src.MaxBookingSpreadDays,
            NumberOfResourcesToBook = src.NumberOfResourcesToBook,
            ProductTags = MapTo(src.ProductTags).ToList(),
            LocationTags = MapTo(src.LocationTags).ToList(),
            Organization = MapTo(src.Organization),
            ProductVersions = MapTo(src.ProductVersions).ToList(),
            OrganizationStripeConnectAccount = MapTo(src.OrganizationStripeConnectAccount)
        };

    public ProductVersion MapTo(AddProductInput src) =>
        new()
        {
            Name = src.Name,
            Description = src.Description,
            Price = decimal.Parse(src.Price),
            PriceUnit = src.PriceUnit,
            Currency = src.Currency,
            MinDurationMinutes = src.MinDurationMinutes,
            MaxDurationMinutes = src.MaxDurationMinutes,
            BookAllLocationResources = src.BookAllLocationResources,
            RecurrenceWindowDays = src.RecurrenceWindowDays,
            RequireConsecutiveDays = src.RequireConsecutiveDays,
            MaxBookingSpreadDays = src.MaxBookingSpreadDays,
            NumberOfResourcesToBook = src.NumberOfResourcesToBook,
            ProductTags = src.ProductTagIds.Select(item => new Shared.Models.OrganizationTag { Id = item }).ToList(),
            LocationTags = src.LocationTagIds.Select(item => new Shared.Models.OrganizationTag { Id = item }).ToList(),
            OrganizationStripeConnectAccount = string.IsNullOrWhiteSpace(src.OrganizationStripeConnectAccountId)
                ? null
                : new Shared.Models.OrganizationStripeConnectAccount { Id = src.OrganizationStripeConnectAccountId }
        };

    public ProductVersion MapTo(UpdateProductInput src) =>
        new()
        {
            Name = src.Name,
            Description = src.Description,
            Price = decimal.Parse(src.Price),
            PriceUnit = src.PriceUnit,
            Currency = src.Currency,
            MinDurationMinutes = src.MinDurationMinutes,
            MaxDurationMinutes = src.MaxDurationMinutes,
            BookAllLocationResources = src.BookAllLocationResources,
            RecurrenceWindowDays = src.RecurrenceWindowDays,
            RequireConsecutiveDays = src.RequireConsecutiveDays,
            MaxBookingSpreadDays = src.MaxBookingSpreadDays,
            NumberOfResourcesToBook = src.NumberOfResourcesToBook,
            ProductTags = src.ProductTagIds.Select(item => new Shared.Models.OrganizationTag { Id = item }).ToList(),
            LocationTags = src.LocationTagIds.Select(item => new Shared.Models.OrganizationTag { Id = item }).ToList(),
            OrganizationStripeConnectAccount = string.IsNullOrWhiteSpace(src.OrganizationStripeConnectAccountId)
                ? null
                : new Shared.Models.OrganizationStripeConnectAccount { Id = src.OrganizationStripeConnectAccountId }
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
            Currency = new CurrencyDetails { Type = src.Currency, Name = src.Currency.ToCurrencyName() },
            MinDurationMinutes = src.MinDurationMinutes,
            MaxDurationMinutes = src.MaxDurationMinutes,
            BookAllLocationResources = src.BookAllLocationResources,
            RecurrenceWindowDays = src.RecurrenceWindowDays,
            RequireConsecutiveDays = src.RequireConsecutiveDays,
            MaxBookingSpreadDays = src.MaxBookingSpreadDays,
            NumberOfResourcesToBook = src.NumberOfResourcesToBook,
            ProductTags = MapTo(src.ProductTags).ToList(),
            LocationTags = MapTo(src.LocationTags).ToList(),
            Organization = MapTo(src.Organization),
            OrganizationStripeConnectAccountDetails = MapToGraphQl(src.OrganizationStripeConnectAccount),
            LatestProductVersionId = src.ProductVersions.OrderByDescending(item => item.CreatedAt).First().Id
        };
    }

    public Shared.Database.Entities.Product MapTo(
        Product src,
        ProductVersion productVersion,
        Organization organization,
        ICollection<OrganizationTag> productTags,
        ICollection<OrganizationTag> locationTags,
        OrganizationStripeConnectAccount? organizationStripeConnectAccount) =>
        MergeTo(
            src,
            productVersion,
            new Shared.Database.Entities.Product(),
            organization,
            productTags,
            locationTags,
            organizationStripeConnectAccount);

    public Shared.Database.Entities.Product MergeTo(
        ProductVersion src,
        Shared.Database.Entities.Product dest,
        Organization organization,
        ICollection<OrganizationTag> productTags,
        ICollection<OrganizationTag> locationTags,
        OrganizationStripeConnectAccount? organizationStripeConnectAccount)
    {
        dest.Organization = organization;
        dest.Name = src.Name;
        dest.Description = src.Description;
        dest.Price = src.Price;
        dest.PriceUnit = src.PriceUnit.ToPriceUnit();
        dest.Currency = src.Currency.ToCurrency();
        dest.MinDurationMinutes = src.MinDurationMinutes;
        dest.MaxDurationMinutes = src.MaxDurationMinutes;
        dest.BookAllLocationResources = src.BookAllLocationResources;
        dest.RecurrenceWindowDays = src.RecurrenceWindowDays;
        dest.RequireConsecutiveDays = src.RequireConsecutiveDays;
        dest.MaxBookingSpreadDays = src.MaxBookingSpreadDays;
        dest.NumberOfResourcesToBook = src.NumberOfResourcesToBook;
        dest.ProductTags = productTags;
        dest.LocationTags = locationTags;
        dest.OrganizationStripeConnectAccount = organizationStripeConnectAccount;
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
            Currency = src.Currency.ToCurrency(),
            MinDurationMinutes = src.MinDurationMinutes,
            MaxDurationMinutes = src.MaxDurationMinutes,
            BookAllLocationResources = src.BookAllLocationResources,
            RecurrenceWindowDays = src.RecurrenceWindowDays,
            RequireConsecutiveDays = src.RequireConsecutiveDays,
            MaxBookingSpreadDays = src.MaxBookingSpreadDays,
            NumberOfResourcesToBook = src.NumberOfResourcesToBook,
            ProductTags = MapTo(src.ProductTags).ToList(),
            LocationTags = MapTo(src.LocationTags).ToList(),
            Organization = organization,
            OrganizationStripeConnectAccount = MapTo(src.OrganizationStripeConnectAccount),
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
            Tags = MapTo(src.Tags).ToList()
        };

    public Shared.Database.Entities.ProductVersion MapTo(
        ProductVersion src,
        Shared.Database.Entities.Product product,
        ICollection<OrganizationTag> productTags,
        ICollection<OrganizationTag> locationTags,
        OrganizationStripeConnectAccount? organizationStripeConnectAccount) =>
        MergeTo(src, new Shared.Database.Entities.ProductVersion(), product, productTags, locationTags, organizationStripeConnectAccount);

    private static OrganizationDetails MapTo(Shared.Models.Organization src) => new() { UniqueId = src.Id };

    private static OrganizationStripeConnectAccountDetails? MapToGraphQl(Shared.Models.OrganizationStripeConnectAccount? src) =>
        src is null ? null : new OrganizationStripeConnectAccountDetails { UniqueId = src.Id, Name = src.Name.ToSafeString() };

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

    private static IEnumerable<ProductVersion> MapTo(IEnumerable<Shared.Database.Entities.ProductVersion> src) => src.Select(MapTo);

    private static ProductVersion MapTo(Shared.Database.Entities.ProductVersion src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            Name = src.Name,
            Description = src.Description,
            Price = src.Price,
            PriceUnit = src.PriceUnit.ToPriceUnit(),
            Currency = src.Currency.ToCurrency(),
            MinDurationMinutes = src.MinDurationMinutes,
            MaxDurationMinutes = src.MaxDurationMinutes,
            BookAllLocationResources = src.BookAllLocationResources,
            RecurrenceWindowDays = src.RecurrenceWindowDays,
            RequireConsecutiveDays = src.RequireConsecutiveDays,
            MaxBookingSpreadDays = src.MaxBookingSpreadDays,
            NumberOfResourcesToBook = src.NumberOfResourcesToBook,
            ProductTags = MapTo(src.ProductTags).ToList(),
            LocationTags = MapTo(src.LocationTags).ToList(),
            OrganizationStripeConnectAccount = MapTo(src.OrganizationStripeConnectAccount)
        };

    private static IEnumerable<OrganizationTagDetails> MapTo(IEnumerable<Shared.Models.OrganizationTag> src) => src.Select(MapTo);

    private static OrganizationTagDetails MapTo(Shared.Models.OrganizationTag src) =>
        new() { UniqueId = src.Id, Name = src.Name, TagType = src.Type.ToNullableOrganizationTagType(), Color = src.Color };

    private static Shared.Database.Entities.Product MergeTo(
        Product src,
        ProductVersion productVersion,
        Shared.Database.Entities.Product dest,
        Organization organization,
        ICollection<OrganizationTag> productTags,
        ICollection<OrganizationTag> locationTags,
        OrganizationStripeConnectAccount? organizationStripeConnectAccount)
    {
        dest.Id = src.Id;
        dest.Inactive = src.Inactive;
        dest.Organization = organization;

        dest.Name = productVersion.Name;
        dest.Description = productVersion.Description;
        dest.Price = productVersion.Price;
        dest.PriceUnit = productVersion.PriceUnit.ToPriceUnit();
        dest.Currency = productVersion.Currency.ToCurrency();
        dest.MinDurationMinutes = productVersion.MinDurationMinutes;
        dest.MaxDurationMinutes = productVersion.MaxDurationMinutes;
        dest.BookAllLocationResources = productVersion.BookAllLocationResources;
        dest.RecurrenceWindowDays = productVersion.RecurrenceWindowDays;
        dest.RequireConsecutiveDays = productVersion.RequireConsecutiveDays;
        dest.MaxBookingSpreadDays = productVersion.MaxBookingSpreadDays;
        dest.NumberOfResourcesToBook = productVersion.NumberOfResourcesToBook;
        dest.ProductTags = productTags;
        dest.LocationTags = locationTags;
        dest.OrganizationStripeConnectAccount = organizationStripeConnectAccount;
        return dest;
    }

    private static Shared.Database.Entities.ProductVersion MergeTo(
        ProductVersion src,
        Shared.Database.Entities.ProductVersion dest,
        Shared.Database.Entities.Product product,
        ICollection<OrganizationTag> productTags,
        ICollection<OrganizationTag> locationTags,
        OrganizationStripeConnectAccount? organizationStripeConnectAccount)
    {
        dest.Id = src.Id;
        dest.Name = src.Name;
        dest.Description = src.Description;
        dest.Price = src.Price;
        dest.PriceUnit = src.PriceUnit.ToPriceUnit();
        dest.Currency = src.Currency.ToCurrency();
        dest.MinDurationMinutes = src.MinDurationMinutes;
        dest.MaxDurationMinutes = src.MaxDurationMinutes;
        dest.BookAllLocationResources = src.BookAllLocationResources;
        dest.RecurrenceWindowDays = src.RecurrenceWindowDays;
        dest.RequireConsecutiveDays = src.RequireConsecutiveDays;
        dest.MaxBookingSpreadDays = src.MaxBookingSpreadDays;
        dest.NumberOfResourcesToBook = src.NumberOfResourcesToBook;
        dest.ProductTags = productTags;
        dest.LocationTags = locationTags;
        dest.OrganizationStripeConnectAccount = organizationStripeConnectAccount;
        dest.Product = product;
        return dest;
    }

    private static Shared.Models.OrganizationStripeConnectAccount? MapTo(OrganizationStripeConnectAccount? src) =>
        src is null
            ? null
            : new Shared.Models.OrganizationStripeConnectAccount
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                Name = src.Name
            };

    private static OrganizationStripeConnectAccount? MapTo(Shared.Models.OrganizationStripeConnectAccount? src) =>
        src is null
            ? null
            : new OrganizationStripeConnectAccount
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                Name = src.Name
            };
}
