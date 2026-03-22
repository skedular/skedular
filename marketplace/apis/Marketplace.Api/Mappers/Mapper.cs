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
    ProductVersion MapTo(Shared.Database.Entities.ProductVersion src, Shared.Database.Entities.Product product);
    ProductVersion MapTo(AddProductInput src);
    ProductVersion MapTo(UpdateProductInput src);
    ProductDetails? MapTo(Product? src);
    ProductVersionDetails? MapTo(ProductVersion? src);
    Shared.Database.Entities.Product MapTo(Product src, Organization organization);
    Shared.Database.Entities.Product MergeTo(Shared.Database.Entities.Product dest, Organization organization);
    ProductEdge MapTo(Edge<Product> src);
    Product MapTo(Shared.Database.Entities.Product src, Shared.Models.Organization organization);
    Shared.Models.Organization MapTo(Organization src);

    Shared.Database.Entities.ProductVersion MapTo(
        ProductVersion src,
        Shared.Database.Entities.Product product,
        ICollection<OrganizationTag> productTags);
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

    public Product MapTo(Shared.Database.Entities.Product src)
    {
        var product = new Product
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Inactive = src.Inactive,
            Organization = MapTo(src.Organization)
        };

        product.ProductVersions = MapTo(src.ProductVersions, src).ToList();

        return product;
    }

    public ProductVersion MapTo(Shared.Database.Entities.ProductVersion src, Shared.Database.Entities.Product product) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            Type = src.Type.ToProductType(),
            Currency = src.Currency.ToCurrency(),
            ListingMetadata = src.ListingMetadata ?? ListingMetadata.Empty,
            FeatureImages = src.FeatureImages.ToSafeCollection(),
            OrganizationTags = MapTo(src.OrganizationTags).ToList(),
            PricingOptions = src.PricingOptions,
            Product = new Product { Id = product.Id, Inactive = product.Inactive, Organization = MapTo(product.Organization) }
        };

    public ProductVersion MapTo(AddProductInput src) =>
        new()
        {
            ListingMetadata = src.ListingMetadata ?? ListingMetadata.Empty,
            Currency = src.Currency,
            Type = src.Type,
            FeatureImages = src.FeatureImages.ToSafeCollection(),
            OrganizationTags = src.TagIds.Select(item => new Shared.Models.OrganizationTag { Id = item }).ToList(),
            PricingOptions = src.PricingOptions.ToList()
        };

    public ProductVersion MapTo(UpdateProductInput src) =>
        new()
        {
            ListingMetadata = src.ListingMetadata ?? ListingMetadata.Empty,
            Currency = src.Currency,
            Type = src.Type,
            FeatureImages = src.FeatureImages.ToSafeCollection(),
            OrganizationTags = src.TagIds.Select(item => new Shared.Models.OrganizationTag { Id = item }).ToList(),
            PricingOptions = src.PricingOptions.ToList()
        };

    public ProductDetails? MapTo(Product? src)
    {
        if (src is null || src.ProductVersions.Count == 0)
        {
            return null;
        }

        var productVersion = src.ProductVersions.First();

        return new ProductDetails
        {
            Id = src.Id,
            Inactive = src.Inactive,
            ListingMetadata = productVersion.ListingMetadata,
            Type = new ProductTypeDetails { Type = productVersion.Type, Name = productVersion.Type.ToProductTypeName() },
            Currency = new CurrencyDetails { Type = productVersion.Currency, Name = productVersion.Currency.ToCurrencyName() },
            FeatureImages = productVersion.FeatureImages,
            ProductTags = MapTo(productVersion.ProductTags),
            Amenities = MapTo(productVersion.Amenities),
            Organization = MapTo(src.Organization),
            LatestProductVersionId = src.ProductVersions.OrderByDescending(item => item.CreatedAt).First().Id,
            PricingOptions = productVersion.PricingOptions
        };
    }

    public ProductVersionDetails? MapTo(ProductVersion? src)
    {
        if (src is null)
        {
            return null;
        }

        return new ProductVersionDetails
        {
            Id = src.Id,
            ListingMetadata = src.ListingMetadata,
            Type = new ProductTypeDetails { Type = src.Type, Name = src.Type.ToProductTypeName() },
            Currency = new CurrencyDetails { Type = src.Currency, Name = src.Currency.ToCurrencyName() },
            FeatureImages = src.FeatureImages,
            ProductTags = MapTo(src.ProductTags),
            PricingOptions = src.PricingOptions,
            Organization = MapTo(src.Product.Organization)
        };
    }

    public Shared.Database.Entities.Product MapTo(Product src, Organization organization) =>
        MergeTo(src, new Shared.Database.Entities.Product(), organization);

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
            ProductVersions = MapTo(src.ProductVersions, src).ToList()
        };

    public Shared.Models.Organization MapTo(Organization src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            EventRaisedAt = src.EventRaisedAt,
            CustomDomain = src.CustomDomain,
            Name = src.Name,
            Website = src.Website,
            LogoUrl = src.LogoUrl,
            CustomerFacingTermsAndConditionsUrl = src.CustomerFacingTermsAndConditionsUrl,
            Tags = MapTo(src.Tags).ToList()
        };

    public Shared.Database.Entities.ProductVersion MapTo(
        ProductVersion src,
        Shared.Database.Entities.Product product,
        ICollection<OrganizationTag> productTags) =>
        MergeTo(src, new Shared.Database.Entities.ProductVersion(), product, productTags);

    private static OrganizationDetails MapTo(Shared.Models.Organization src) => new()
    {
        Id = src.Id,
        CustomDomain = src.CustomDomain,
        Name = src.Name.ToSafeString(),
        Website = src.Website,
        LogoUrl = src.LogoUrl,
        CustomerFacingTermsAndConditionsUrl = src.CustomerFacingTermsAndConditionsUrl
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

    private IEnumerable<ProductVersion> MapTo(IEnumerable<Shared.Database.Entities.ProductVersion> src, Shared.Database.Entities.Product product) =>
        src.Select(item => MapTo(item, product));

    private static Shared.Database.Entities.Product MergeTo(Product src, Shared.Database.Entities.Product dest, Organization organization)
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
        ICollection<OrganizationTag> organizationTags)
    {
        dest.Id = src.Id;
        dest.ListingMetadata = src.ListingMetadata;
        dest.Type = src.Type.ToProductType();
        dest.Currency = src.Currency.ToCurrency();
        dest.FeatureImages = src.FeatureImages;
        dest.OrganizationTags = organizationTags;
        dest.Product = product;
        dest.PricingOptions = src.PricingOptions;
        return dest;
    }

    private static IEnumerable<OrganizationTagDetails> MapTo(IEnumerable<Shared.Models.OrganizationTag> src) => src.Select(MapTo);

    private static OrganizationTagDetails MapTo(Shared.Models.OrganizationTag src) => new()
    {
        Id = src.Id, Name = src.Name.ToSafeString(), Type = src.Type, Color = src.Color.ToSafeString()
    };
}
