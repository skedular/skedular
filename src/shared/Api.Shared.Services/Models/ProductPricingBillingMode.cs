namespace Api.Shared.Services.Models;

public enum ProductPricingBillingMode
{
    NotSet = 0,
    Upfront = 1,
    InArrears = 2,
}

public static class ProductPricingBillingModeConstants
{
    public const string NotSet = "NOT_SET";
    public const string Upfront = "UPFRONT";
    public const string InArrears = "IN_ARREARS";
}

public static class ProductPricingBillingModeExtensions
{
    extension(ProductPricingBillingMode src)
    {
        public string ToProductPricingBillingMode() =>
            src switch
            {
                ProductPricingBillingMode.NotSet => ProductPricingBillingModeConstants.NotSet,
                ProductPricingBillingMode.Upfront => ProductPricingBillingModeConstants.Upfront,
                ProductPricingBillingMode.InArrears => ProductPricingBillingModeConstants.InArrears,
                _ => throw new ArgumentOutOfRangeException(nameof(src), src,
                    $"Unexpected value for {nameof(src)}: {src}. Update enum mapping or caller input."),
            };

        public string ToProductPricingBillingModeName() =>
            src switch
            {
                ProductPricingBillingMode.NotSet => "Not Set",
                ProductPricingBillingMode.Upfront => "Upfront",
                ProductPricingBillingMode.InArrears => "In Arrears",
                _ => throw new ArgumentOutOfRangeException(nameof(src), src,
                    $"Unexpected value for {nameof(src)}: {src}. Update enum mapping or caller input."),
            };
    }

    extension(string src)
    {
        public ProductPricingBillingMode ToProductPricingBillingMode() =>
            src switch
            {
                ProductPricingBillingModeConstants.NotSet => ProductPricingBillingMode.NotSet,
                ProductPricingBillingModeConstants.Upfront => ProductPricingBillingMode.Upfront,
                ProductPricingBillingModeConstants.InArrears => ProductPricingBillingMode.InArrears,
                _ => throw new ArgumentOutOfRangeException(nameof(src), src,
                    $"Unexpected value for {nameof(src)}: {src}. Update enum mapping or caller input."),
            };
    }
}
