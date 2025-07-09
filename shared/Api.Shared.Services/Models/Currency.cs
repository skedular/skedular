namespace Api.Shared.Services.Models;

public enum Currency
{
    Nzd,
    Usd
}

public static class CurrencyConstants
{
    public const string Nzd = "nzd";
    public const string Usd = "usd";
}

public static class CurrencyExtensions
{
    public static Currency ToCurrency(this string src) =>
        src switch
        {
            CurrencyConstants.Nzd => Currency.Nzd,
            CurrencyConstants.Usd => Currency.Usd,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToCurrency(this Currency src) =>
        src switch
        {
            Currency.Nzd => CurrencyConstants.Nzd,
            Currency.Usd => CurrencyConstants.Usd,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToCurrencyName(this Currency src) =>
        src switch
        {
            Currency.Nzd => "NZD - $",
            Currency.Usd => "USD - $",
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToCurrencyName(this string src) =>
        src switch
        {
            CurrencyConstants.Nzd => "NZD - $",
            CurrencyConstants.Usd => "USD - $",
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToInvoiceCurrencyName(this string src) =>
        src switch
        {
            CurrencyConstants.Nzd => "NZD",
            CurrencyConstants.Usd => "USD",
            _ => throw new ArgumentOutOfRangeException()
        };
}
