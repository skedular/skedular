namespace Api.Shared.Services.Models;

public enum Currency
{
    Nzd = 0,
    Usd = 1,
}

public static class CurrencyConstants
{
    public const string Nzd = "nzd";
    public const string Usd = "usd";
}

public static class CurrencyExtensions
{
    extension(Currency? src)
    {
        public string? ToNullableCurrency() =>
            src is null
                ? null
                : src switch
                {
                    Currency.Nzd => CurrencyConstants.Nzd,
                    Currency.Usd => CurrencyConstants.Usd,
                    _ => throw new ArgumentOutOfRangeException(nameof(src), src,
                        $"Unexpected value for {nameof(src)}: {src}. Update enum mapping or caller input."),
                };
    }

    extension(Currency src)
    {
        public string ToCurrency() =>
            src switch
            {
                Currency.Nzd => CurrencyConstants.Nzd,
                Currency.Usd => CurrencyConstants.Usd,
                _ => throw new ArgumentOutOfRangeException(nameof(src), src,
                    $"Unexpected value for {nameof(src)}: {src}. Update enum mapping or caller input."),
            };

        public string ToCurrencyName() =>
            src switch
            {
                Currency.Nzd => "NZD - $",
                Currency.Usd => "USD - $",
                _ => throw new ArgumentOutOfRangeException(nameof(src), src,
                    $"Unexpected value for {nameof(src)}: {src}. Update enum mapping or caller input."),
            };
    }

    extension(string src)
    {
        public string ToInvoiceCurrencyName() =>
            src switch
            {
                CurrencyConstants.Nzd => "NZD",
                CurrencyConstants.Usd => "USD",
                _ => throw new ArgumentOutOfRangeException(nameof(src), src,
                    $"Unexpected value for {nameof(src)}: {src}. Update enum mapping or caller input."),
            };

        public Currency ToCurrency() =>
            src.ToLowerInvariant() switch
            {
                CurrencyConstants.Nzd => Currency.Nzd,
                CurrencyConstants.Usd => Currency.Usd,
                _ => throw new ArgumentOutOfRangeException(nameof(src), src,
                    $"Unexpected value for {nameof(src)}: {src}. Update enum mapping or caller input."),
            };
    }

    extension(string? src)
    {
        public Currency? ToNullableCurrency() =>
            string.IsNullOrWhiteSpace(src)
                ? null
                : src.ToLowerInvariant() switch
                {
                    CurrencyConstants.Nzd => Currency.Nzd,
                    CurrencyConstants.Usd => Currency.Usd,
                    _ => throw new ArgumentOutOfRangeException(nameof(src), src,
                        $"Unexpected value for {nameof(src)}: {src}. Update enum mapping or caller input."),
                };
    }
}
