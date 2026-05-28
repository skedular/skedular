using Api.Shared.Services.Models;
using Enterprise.Shared;
using Customer = Marketplace.Shared.Models.Customer;
using Organization = Marketplace.Shared.Database.Entities.Organization;
using OrganizationTag = Marketplace.Shared.Database.Entities.OrganizationTag;
using Product = Marketplace.Shared.Models.Product;
using ProductVersion = Marketplace.Shared.Models.ProductVersion;

namespace Marketplace.Shared.Mappers;

public interface IEntityMapper
{
    Customer? MapTo(Database.Entities.Customer? src);
    Product MapTo(Database.Entities.Product src);
    ProductVersion MapTo(Database.Entities.ProductVersion src, Database.Entities.Product product);
    Database.Entities.Product MapTo(Product src, Organization organization);
    Database.Entities.Product MergeTo(Database.Entities.Product dest, Organization organization);
    Product MapTo(Database.Entities.Product src, Models.Organization organization);
    Models.Organization MapTo(Organization src);

    Database.Entities.ProductVersion MapTo(
        ProductVersion src,
        Database.Entities.Product product,
        IEnumerable<OrganizationTag> productTags);
}

public class EntityMapper : IEntityMapper
{
    public Customer? MapTo(Database.Entities.Customer? src) =>
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

    public Product MapTo(Database.Entities.Product src)
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

    public ProductVersion MapTo(Database.Entities.ProductVersion src, Database.Entities.Product product) =>
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
            PricingOptions = src.PricingOptions.ToList(),
            Product = new Product { Id = product.Id, Inactive = product.Inactive, Organization = MapTo(product.Organization) }
        };

    public Database.Entities.Product MapTo(Product src, Organization organization) =>
        MergeTo(src, new Database.Entities.Product(), organization);

    public Database.Entities.Product MergeTo(Database.Entities.Product dest, Organization organization)
    {
        dest.Organization = organization;
        return dest;
    }

    public Product MapTo(Database.Entities.Product src, Models.Organization organization) =>
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

    public Models.Organization MapTo(Organization src) =>
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

    public Database.Entities.ProductVersion MapTo(
        ProductVersion src,
        Database.Entities.Product product,
        IEnumerable<OrganizationTag> productTags) =>
        MergeTo(src, new Database.Entities.ProductVersion(), product, productTags);

    private static IEnumerable<Models.OrganizationTag> MapTo(IEnumerable<OrganizationTag> src) => src.Select(MapTo);

    private static Models.OrganizationTag MapTo(OrganizationTag src) =>
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

    private IEnumerable<ProductVersion> MapTo(IEnumerable<Database.Entities.ProductVersion> src, Database.Entities.Product product) =>
        src.Select(item => MapTo(item, product));

    private static Database.Entities.Product MergeTo(Product src, Database.Entities.Product dest, Organization organization)
    {
        dest.Id = src.Id;
        dest.Inactive = src.Inactive;
        dest.Organization = organization;
        return dest;
    }

    private static Database.Entities.ProductVersion MergeTo(
        ProductVersion src,
        Database.Entities.ProductVersion dest,
        Database.Entities.Product product,
        IEnumerable<OrganizationTag> organizationTags)
    {
        dest.Id = src.Id;
        dest.ListingMetadata = src.ListingMetadata;
        dest.Type = src.Type.ToProductType();
        dest.Currency = src.Currency.ToCurrency();
        dest.FeatureImages = src.FeatureImages.ToList();
        dest.OrganizationTags = organizationTags.ToList();
        dest.Product = product;
        dest.PricingOptions = src.PricingOptions.ToList();
        return dest;
    }
}
