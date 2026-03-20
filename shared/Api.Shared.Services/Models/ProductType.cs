namespace Api.Shared.Services.Models;

public enum ProductType
{
    Resource,
    Event
}

public static class ProductTypeConstants
{
    public const string Resource = "RESOURCE";
    public const string Event = "EVENT";
}

public static class ProductTypeExtensions
{
    extension(ProductType src)
    {
        public string ToProductTypeName() =>
            src switch
            {
                ProductType.Resource => "Resource: Books the required matching resources for the chosen time",
                ProductType.Event => "Event: Books all matching resources across the location for the chosen time",
                _ => throw new ArgumentOutOfRangeException()
            };
    }

    extension(ProductType src)
    {
        public string ToProductType() =>
            src switch
            {
                ProductType.Resource => ProductTypeConstants.Resource,
                ProductType.Event => ProductTypeConstants.Event,
                _ => throw new ArgumentOutOfRangeException()
            };
    }

    extension(string src)
    {
        public ProductType ToProductType() =>
            src switch
            {
                ProductTypeConstants.Resource => ProductType.Resource,
                ProductTypeConstants.Event => ProductType.Event,
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}
