namespace Api.Shared.Services.Models;

public enum BookingPaymentMethod
{
    Card,
    BankTransfer
}

public static class BookingPaymentMethodConstants
{
    public const string Card = "CARD";
    public const string BankTransfer = "BANK_TRANSFER";
}

public static class BookingPaymentMethodExtensions
{
    public static BookingPaymentMethod ToBookingPaymentMethod(this string src) =>
        src switch
        {
            BookingPaymentMethodConstants.Card => BookingPaymentMethod.Card,
            BookingPaymentMethodConstants.BankTransfer => BookingPaymentMethod.BankTransfer,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToBookingPaymentMethod(this BookingPaymentMethod src) =>
        src switch
        {
            BookingPaymentMethod.Card => BookingPaymentMethodConstants.Card,
            BookingPaymentMethod.BankTransfer => BookingPaymentMethodConstants.BankTransfer,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static BookingPaymentMethod? ToNullableBookingPaymentMethod(this string? src) =>
        string.IsNullOrWhiteSpace(src)
            ? null
            : src switch
            {
                BookingPaymentMethodConstants.Card => BookingPaymentMethod.Card,
                BookingPaymentMethodConstants.BankTransfer => BookingPaymentMethod.BankTransfer,
                _ => throw new ArgumentOutOfRangeException()
            };

    public static string? ToNullableBookingPaymentMethod(this BookingPaymentMethod? src) =>
        src is null
            ? null
            : src switch
            {
                BookingPaymentMethod.Card => BookingPaymentMethodConstants.Card,
                BookingPaymentMethod.BankTransfer => BookingPaymentMethodConstants.BankTransfer,
                _ => throw new ArgumentOutOfRangeException()
            };

    public static string ToBookingPaymentMethodName(this BookingPaymentMethod src) =>
        src switch
        {
            BookingPaymentMethod.Card => "Card",
            BookingPaymentMethod.BankTransfer => "Bank Transfer",
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToBookingPaymentMethodName(this string src) =>
        src switch
        {
            BookingPaymentMethodConstants.Card => "Card",
            BookingPaymentMethodConstants.BankTransfer => "Bank Transfer",
            _ => throw new ArgumentOutOfRangeException()
        };
}
