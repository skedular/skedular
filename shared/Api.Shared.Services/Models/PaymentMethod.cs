namespace Api.Shared.Services.Models;

public enum PaymentMethod
{
    Card,
    BankTransfer
}

public static class PaymentMethodConstants
{
    public const string Card = "CARD";
    public const string BankTransfer = "BANK_TRANSFER";
}

public static class PaymentMethodExtensions
{
    public static PaymentMethod ToPaymentMethod(this string src) =>
        src switch
        {
            PaymentMethodConstants.Card => PaymentMethod.Card,
            PaymentMethodConstants.BankTransfer => PaymentMethod.BankTransfer,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToPaymentMethod(this PaymentMethod src) =>
        src switch
        {
            PaymentMethod.Card => PaymentMethodConstants.Card,
            PaymentMethod.BankTransfer => PaymentMethodConstants.BankTransfer,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static PaymentMethod? ToNullablePaymentMethod(this string? src) =>
        string.IsNullOrWhiteSpace(src)
            ? null
            : src switch
            {
                PaymentMethodConstants.Card => PaymentMethod.Card,
                PaymentMethodConstants.BankTransfer => PaymentMethod.BankTransfer,
                _ => throw new ArgumentOutOfRangeException()
            };

    public static string? ToNullablePaymentMethod(this PaymentMethod? src) =>
        src is null
            ? null
            : src switch
            {
                PaymentMethod.Card => PaymentMethodConstants.Card,
                PaymentMethod.BankTransfer => PaymentMethodConstants.BankTransfer,
                _ => throw new ArgumentOutOfRangeException()
            };

    public static string ToPaymentMethodName(this PaymentMethod src) =>
        src switch
        {
            PaymentMethod.Card => "Card",
            PaymentMethod.BankTransfer => "Bank Transfer",
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToPaymentMethodName(this string src) =>
        src switch
        {
            PaymentMethodConstants.Card => "Card",
            PaymentMethodConstants.BankTransfer => "Bank Transfer",
            _ => throw new ArgumentOutOfRangeException()
        };
}
