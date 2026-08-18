namespace Booking.Shared.Models.Entitlements;

public enum EntitlementStatus
{
    Pending = 0,
    Active = 1,
    Expired = 2,
    Cancelled = 3,
}

public enum CreditLedgerTransactionType
{
    Granted = 0,
    Consumed = 1,
    Released = 2,
    Forfeited = 3,
    Expired = 4,
    Adjusted = 5,
}

public static class CreditLedgerTransactionTypeExtensions
{
    public static string ToPersistedValue(this CreditLedgerTransactionType value) => value switch
    {
        CreditLedgerTransactionType.Granted => "GRANTED",
        CreditLedgerTransactionType.Consumed => "CONSUMED",
        CreditLedgerTransactionType.Released => "RELEASED",
        CreditLedgerTransactionType.Forfeited => "FORFEITED",
        CreditLedgerTransactionType.Expired => "EXPIRED",
        CreditLedgerTransactionType.Adjusted => "ADJUSTED",
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unsupported credit ledger transaction type."),
    };

    public static CreditLedgerTransactionType FromPersistedValue(string value) => value switch
    {
        "GRANTED" => CreditLedgerTransactionType.Granted,
        "CONSUMED" => CreditLedgerTransactionType.Consumed,
        "RELEASED" => CreditLedgerTransactionType.Released,
        "FORFEITED" => CreditLedgerTransactionType.Forfeited,
        "EXPIRED" => CreditLedgerTransactionType.Expired,
        "ADJUSTED" => CreditLedgerTransactionType.Adjusted,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unsupported credit ledger transaction value."),
    };
}
