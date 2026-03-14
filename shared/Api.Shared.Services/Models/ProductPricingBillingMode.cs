namespace Api.Shared.Services.Models;

public enum ProductPricingBillingMode
{
    NotSet = 0,
    Upfront = 1,
    InArrears = 2
}

public static class ProductPricingBillingModeExtensions
{
    extension(ProductPricingBillingMode src)
    {
        public string ToProductPricingBillingModeName() =>
            src switch
            {
                ProductPricingBillingMode.NotSet => "Not Set",
                ProductPricingBillingMode.Upfront => "Upfront",
                ProductPricingBillingMode.InArrears => "In Arrears",
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}
