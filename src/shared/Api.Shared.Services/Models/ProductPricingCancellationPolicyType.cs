namespace Api.Shared.Services.Models;

public enum ProductPricingCancellationPolicyType
{
    NotSet = 0,
    NoCancellation = 1,
    FullRefundBeforeCutoff = 2,
    TieredRefund = 3
}

public static class ProductPricingCancellationPolicyTypeConstants
{
    public const string NotSet = "NOT_SET";
    public const string NoCancellation = "NO_CANCELLATION";
    public const string FullRefundBeforeCutoff = "FULL_REFUND_BEFORE_CUTOFF";
    public const string TieredRefund = "TIERED_REFUND";
}

public static class ProductPricingCancellationPolicyTypeExtensions
{
    extension(ProductPricingCancellationPolicyType src)
    {
        public string ToProductPricingCancellationPolicyType() =>
            src switch
            {
                ProductPricingCancellationPolicyType.NotSet => ProductPricingCancellationPolicyTypeConstants.NotSet,
                ProductPricingCancellationPolicyType.NoCancellation => ProductPricingCancellationPolicyTypeConstants.NoCancellation,
                ProductPricingCancellationPolicyType.FullRefundBeforeCutoff => ProductPricingCancellationPolicyTypeConstants.FullRefundBeforeCutoff,
                ProductPricingCancellationPolicyType.TieredRefund => ProductPricingCancellationPolicyTypeConstants.TieredRefund,
                _ => throw new ArgumentOutOfRangeException(nameof(src), src,
                    $"Unexpected value for {nameof(src)}: {src}. Update enum mapping or caller input.")
            };

        public string ToProductPricingCancellationPolicyTypeName() =>
            src switch
            {
                ProductPricingCancellationPolicyType.NotSet => "Not Set",
                ProductPricingCancellationPolicyType.NoCancellation => "No Cancellation",
                ProductPricingCancellationPolicyType.FullRefundBeforeCutoff => "Full Refund Before Cutoff",
                ProductPricingCancellationPolicyType.TieredRefund => "Tiered Refund",
                _ => throw new ArgumentOutOfRangeException(nameof(src), src,
                    $"Unexpected value for {nameof(src)}: {src}. Update enum mapping or caller input.")
            };
    }

    extension(string src)
    {
        public ProductPricingCancellationPolicyType ToProductPricingCancellationPolicyType() =>
            src switch
            {
                ProductPricingCancellationPolicyTypeConstants.NotSet => ProductPricingCancellationPolicyType.NotSet,
                ProductPricingCancellationPolicyTypeConstants.NoCancellation => ProductPricingCancellationPolicyType.NoCancellation,
                ProductPricingCancellationPolicyTypeConstants.FullRefundBeforeCutoff => ProductPricingCancellationPolicyType.FullRefundBeforeCutoff,
                ProductPricingCancellationPolicyTypeConstants.TieredRefund => ProductPricingCancellationPolicyType.TieredRefund,
                _ => throw new ArgumentOutOfRangeException(nameof(src), src,
                    $"Unexpected value for {nameof(src)}: {src}. Update enum mapping or caller input.")
            };
    }
}
