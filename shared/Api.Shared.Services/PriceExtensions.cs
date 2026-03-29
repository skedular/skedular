using Api.Shared.Services.Models;

namespace Api.Shared.Services;

public static class PriceExtensions
{
    extension(string price)
    {
        public string ToPriceToDisplay(Currency currency) =>
            currency switch
            {
                Currency.Nzd => "NZ$" + price,
                Currency.Usd => "US$" + price,
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}
