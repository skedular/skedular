namespace Api.Shared.Services.Models;

public enum MarketplaceBookingSubscriptionCancellationMode
{
    Immediate,
    AtPeriodEnd
}

public static class MarketplaceBookingSubscriptionCancellationModeExtensions
{
    extension(MarketplaceBookingSubscriptionCancellationMode src)
    {
        public string ToMarketplaceBookingSubscriptionCancellationModeName() =>
            src switch
            {
                MarketplaceBookingSubscriptionCancellationMode.Immediate => "Immediate",
                MarketplaceBookingSubscriptionCancellationMode.AtPeriodEnd => "At Period End",
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}
