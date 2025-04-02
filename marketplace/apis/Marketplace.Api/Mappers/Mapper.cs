using Api.Shared.Services.Models;
using Enterprise.Shared;
using Marketplace.Api.GraphQL;
using Marketplace.Shared.Models;
using Organization = Marketplace.Shared.Database.Entities.Organization;
using OrganizationTag = Marketplace.Shared.Database.Entities.OrganizationTag;

namespace Marketplace.Api.Mappers;

public interface IMapper
{
    Customer? MapTo(Shared.Database.Entities.Customer? src);
    Product MapTo(Shared.Database.Entities.Product src);
    ProductVersion MapTo(AddProductInput src);
    ProductVersion MapTo(UpdateProductInput src);
    ProductDetails? MapTo(Product? src);
    Shared.Database.Entities.Product MapTo(Product src, Organization organization);

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
            Organization = MapTo(src.Organization),
            ProductVersions = MapTo(src.ProductVersions).ToList()
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
            RecurrenceIntervalDays = src.RecurrenceIntervalDays,
            ForceContinuousSlots = src.ForceContinuousSlots,
            MaxSpreadDays = src.MaxSpreadDays,
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
            Currency = src.Currency,
            MinDurationMinutes = src.MinDurationMinutes,
            MaxDurationMinutes = src.MaxDurationMinutes,
            BookAllLocationResources = src.BookAllLocationResources,
            RecurrenceIntervalDays = src.RecurrenceIntervalDays,
            ForceContinuousSlots = src.ForceContinuousSlots,
            MaxSpreadDays = src.MaxSpreadDays,
            ProductTags = src.ProductTagIds.Select(item => new Shared.Models.OrganizationTag { Id = item }).ToList(),
            LocationTags = src.LocationTagIds.Select(item => new Shared.Models.OrganizationTag { Id = item }).ToList()
        };

    public ProductDetails? MapTo(Product? src) =>
        src is null
            ? null
            : new ProductDetails
            {
                Id = src.Id,
                Organization = MapTo(src.Organization),
                LatestProductVersion = MapTo(src.ProductVersions.OrderByDescending(item => item.CreatedAt).First())
            };

    public Shared.Database.Entities.Product MapTo(Product src, Organization organization) =>
        MergeTo(src, new Shared.Database.Entities.Product(), organization);

    public Shared.Database.Entities.ProductVersion MapTo(
        ProductVersion src,
        Shared.Database.Entities.Product product,
        ICollection<OrganizationTag> productTags,
        ICollection<OrganizationTag> locationTags) =>
        MergeTo(src, new Shared.Database.Entities.ProductVersion(), product, productTags, locationTags);

    private static OrganizationDetails MapTo(Shared.Models.Organization src) => new() { UniqueId = src.Id };

    private static Shared.Models.Organization MapTo(Organization src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            EventRaisedAt = src.EventRaisedAt,
            Tags = MapTo(src.Tags).ToList()
        };

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
            RecurrenceIntervalDays = src.RecurrenceIntervalDays,
            ForceContinuousSlots = src.ForceContinuousSlots,
            MaxSpreadDays = src.MaxSpreadDays,
            ProductTags = MapTo(src.ProductTags).ToList(),
            LocationTags = MapTo(src.LocationTags).ToList()
        };

    private static ProductVersionDetails MapTo(ProductVersion src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name,
            Description = src.Description,
            Price = src.Price.ToRoundedPrice(),
            PriceUnit = src.PriceUnit,
            Currency = src.Currency,
            MinDurationMinutes = src.MinDurationMinutes,
            MaxDurationMinutes = src.MaxDurationMinutes,
            BookAllLocationResources = src.BookAllLocationResources,
            RecurrenceIntervalDays = src.RecurrenceIntervalDays,
            ForceContinuousSlots = src.ForceContinuousSlots,
            MaxSpreadDays = src.MaxSpreadDays,
            ProductTags = MapTo(src.ProductTags).ToList(),
            LocationTags = MapTo(src.LocationTags).ToList()
        };

    private static IEnumerable<OrganizationTagDetails> MapTo(IEnumerable<Shared.Models.OrganizationTag> src) => src.Select(MapTo);

    private static OrganizationTagDetails MapTo(Shared.Models.OrganizationTag src) =>
        new() { UniqueId = src.Id, Name = src.Name, TagType = src.Type.ToNullableOrganizationTagType(), Color = src.Color };

    private static Shared.Database.Entities.Product MergeTo(Product src, Shared.Database.Entities.Product dest, Organization organization)
    {
        dest.Id = src.Id;
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
        dest.Name = src.Name;
        dest.Description = src.Description;
        dest.Price = src.Price;
        dest.PriceUnit = src.PriceUnit.ToPriceUnit();
        dest.Currency = src.Currency.ToCurrency();
        dest.MinDurationMinutes = src.MinDurationMinutes;
        dest.MaxDurationMinutes = src.MaxDurationMinutes;
        dest.BookAllLocationResources = src.BookAllLocationResources;
        dest.RecurrenceIntervalDays = src.RecurrenceIntervalDays;
        dest.ForceContinuousSlots = src.ForceContinuousSlots;
        dest.MaxSpreadDays = src.MaxSpreadDays;
        dest.ProductTags = productTags;
        dest.LocationTags = locationTags;
        dest.Product = product;

        return dest;
    }
}
