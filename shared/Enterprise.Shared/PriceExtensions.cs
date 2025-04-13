using System.Globalization;

namespace Enterprise.Shared;

public static class PriceExtensions
{
    public static string ToRoundedPrice(this decimal price) => price.ToString("0.##", CultureInfo.InvariantCulture);
    public static decimal FromRoundedPrice(this string price) => decimal.Parse(price);
}
