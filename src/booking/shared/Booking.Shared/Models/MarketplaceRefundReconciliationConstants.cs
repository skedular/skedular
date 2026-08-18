namespace Booking.Shared.Models;

public static class MarketplaceExternalRefundReconciliationProviderConstants
{
    public const string Stripe = "STRIPE";
    public const string BankTransfer = "BANK_TRANSFER";
    public const string StripePayout = "STRIPE_PAYOUT";
}

public static class MarketplaceStripeRefundPathConstants
{
    public const string PlatformFunded = "PlatformFunded";
    public const string TransferReversal = "TransferReversal";
}
