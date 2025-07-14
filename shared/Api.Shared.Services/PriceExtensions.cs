using Api.Shared.Services.Models;

namespace Api.Shared.Services;

public static class PriceExtensions
{
    public static string ToPriceToDisplay(this string price, Currency currency) =>
        currency switch
        {
            Currency.Nzd => "NZ$" + price,
            Currency.Usd => "US$" + price,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToCurrencyToDisplay(this Currency currency) =>
        currency switch
        {
            Currency.Nzd => "NZ$",
            Currency.Usd => "US$",
            _ => throw new ArgumentOutOfRangeException()
        };
}
