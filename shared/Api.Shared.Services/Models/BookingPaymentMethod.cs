namespace Api.Shared.Services.Models;

public enum BookingPaymentMethod
{
    Card,
    BankAccount
}

public static class BookingPaymentMethodConstants
{
    public const string Card = "CARD";
    public const string BankAccount = "BANK_ACCOUNT";
}

public static class BookingPaymentMethodExtensions
{
    public static BookingPaymentMethod? ToBookingPaymentMethod(this string? src) =>
        string.IsNullOrWhiteSpace(src)
            ? null
            : src switch
            {
                BookingPaymentMethodConstants.Card => BookingPaymentMethod.Card,
                BookingPaymentMethodConstants.BankAccount => BookingPaymentMethod.BankAccount,
                _ => throw new ArgumentOutOfRangeException()
            };

    public static string? ToBookingPaymentMethod(this BookingPaymentMethod? src) =>
        src is null
            ? null
            : src switch
            {
                BookingPaymentMethod.Card => BookingPaymentMethodConstants.Card,
                BookingPaymentMethod.BankAccount => BookingPaymentMethodConstants.BankAccount,
                _ => throw new ArgumentOutOfRangeException()
            };

    public static string ToBookingPaymentMethodName(this BookingPaymentMethod src) =>
        src switch
        {
            BookingPaymentMethod.Card => "Card",
            BookingPaymentMethod.BankAccount => "Bank Account",
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToBookingPaymentMethodName(this string src) =>
        src switch
        {
            BookingPaymentMethodConstants.Card => "Card",
            BookingPaymentMethodConstants.BankAccount => "Bank Account",
            _ => throw new ArgumentOutOfRangeException()
        };
}
