using Api.Shared.Services.Models;

namespace Marketplace.Api;

public static class PriceExtensions
{
    public static string ToPriceToDisplay(this string price, Currency currency) =>
        currency switch
        {
            Currency.Nzd => $"NZ${price}",
            Currency.Usd => $"US${price}",
            _ => throw new ArgumentOutOfRangeException()
        };
}
