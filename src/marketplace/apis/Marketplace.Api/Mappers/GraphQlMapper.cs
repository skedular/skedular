using Api.Shared.Services.Models;
using Enterprise.Shared;
using HotChocolate.Types.Pagination;
using Marketplace.Api.GraphQL.Product;
using Marketplace.Shared.Models;
using Product = Marketplace.Shared.Models.Product;
using ProductVersion = Marketplace.Shared.Models.ProductVersion;

namespace Marketplace.Api.Mappers;

public interface IGraphQlMapper
{
    ProductVersion MapTo(AddProductInput src);
    ProductVersion MapTo(UpdateProductInput src);
    ProductDetails? MapTo(Product? src);
    ProductVersionDetails? MapTo(ProductVersion? src);
    ProductEdge MapTo(Edge<Product> src);
}

public class GraphQlMapper : IGraphQlMapper
{
    public ProductVersion MapTo(AddProductInput src) =>
        new()
        {
            ListingMetadata = src.ListingMetadata ?? ListingMetadata.Empty,
            Currency = src.Currency,
            Type = src.Type,
            FeatureImages = src.FeatureImages.ToSafeCollection(),
            OrganizationTags = src.TagIds.Select(item => new OrganizationTag
            {
                Id = item,
            }).ToList(),
            PricingOptions = src.PricingOptions.ToList(),
        };

    public ProductVersion MapTo(UpdateProductInput src) =>
        new()
        {
            ListingMetadata = src.ListingMetadata ?? ListingMetadata.Empty,
            Currency = src.Currency,
            Type = src.Type,
            FeatureImages = src.FeatureImages.ToSafeCollection(),
            OrganizationTags = src.TagIds.Select(item => new OrganizationTag
            {
                Id = item,
            }).ToList(),
            PricingOptions = src.PricingOptions.ToList(),
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
            Type = new ProductTypeDetails
            {
                Type = productVersion.Type,
                Name = productVersion.Type.ToProductTypeName(),
            },
            Currency = new CurrencyDetails
            {
                Type = productVersion.Currency,
                Name = productVersion.Currency.ToCurrencyName(),
            },
            FeatureImages = productVersion.FeatureImages,
            ProductTags = MapTo(productVersion.ProductTags),
            Amenities = MapTo(productVersion.Amenities),
            Organization = MapTo(src.Organization),
            LatestProductVersionId = src.ProductVersions.OrderByDescending(item => item.CreatedAt).First().Id,
            PricingOptions = productVersion.PricingOptions,
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
            Type = new ProductTypeDetails
            {
                Type = src.Type,
                Name = src.Type.ToProductTypeName(),
            },
            Currency = new CurrencyDetails
            {
                Type = src.Currency,
                Name = src.Currency.ToCurrencyName(),
            },
            FeatureImages = src.FeatureImages,
            ProductTags = MapTo(src.ProductTags),
            PricingOptions = src.PricingOptions,
            Organization = MapTo(src.Product.Organization),
        };
    }

    public ProductEdge MapTo(Edge<Product> src) => new(MapTo(src.Node)!, src.Cursor);

    private static OrganizationDetails MapTo(Organization src) => new()
    {
        Id = src.Id,
        CustomDomain = src.CustomDomain,
        Name = src.Name.ToSafeString(),
        Website = src.Website,
        LogoUrl = src.LogoUrl,
        CustomerFacingTermsAndConditionsUrl = src.CustomerFacingTermsAndConditionsUrl,
    };

    private static IEnumerable<OrganizationTagDetails> MapTo(IEnumerable<OrganizationTag> src) => src.Select(MapTo);

    private static OrganizationTagDetails MapTo(OrganizationTag src) => new()
    {
        Id = src.Id,
        Name = src.Name.ToSafeString(),
        Type = src.Type,
        Color = src.Color.ToSafeString(),
    };
}
