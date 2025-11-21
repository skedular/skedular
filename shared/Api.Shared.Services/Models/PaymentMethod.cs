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
    extension(string src)
    {
        public PaymentMethod ToPaymentMethod() =>
            src switch
            {
                PaymentMethodConstants.Card => PaymentMethod.Card,
                PaymentMethodConstants.BankTransfer => PaymentMethod.BankTransfer,
                _ => throw new ArgumentOutOfRangeException()
            };

        public string ToPaymentMethodName() =>
            src switch
            {
                PaymentMethodConstants.Card => "Card",
                PaymentMethodConstants.BankTransfer => "Bank Transfer",
                _ => throw new ArgumentOutOfRangeException()
            };
    }

    extension(string? src)
    {
        public PaymentMethod? ToNullablePaymentMethod() =>
            string.IsNullOrWhiteSpace(src)
                ? null
                : src switch
                {
                    PaymentMethodConstants.Card => PaymentMethod.Card,
                    PaymentMethodConstants.BankTransfer => PaymentMethod.BankTransfer,
                    _ => throw new ArgumentOutOfRangeException()
                };
    }

    extension(PaymentMethod src)
    {
        public string ToPaymentMethod() =>
            src switch
            {
                PaymentMethod.Card => PaymentMethodConstants.Card,
                PaymentMethod.BankTransfer => PaymentMethodConstants.BankTransfer,
                _ => throw new ArgumentOutOfRangeException()
            };

        public string ToPaymentMethodName() =>
            src switch
            {
                PaymentMethod.Card => "Card",
                PaymentMethod.BankTransfer => "Bank Transfer",
                _ => throw new ArgumentOutOfRangeException()
            };
    }

    extension(PaymentMethod? src)
    {
        public string? ToNullablePaymentMethod() =>
            src is null
                ? null
                : src switch
                {
                    PaymentMethod.Card => PaymentMethodConstants.Card,
                    PaymentMethod.BankTransfer => PaymentMethodConstants.BankTransfer,
                    _ => throw new ArgumentOutOfRangeException()
                };
    }
}
