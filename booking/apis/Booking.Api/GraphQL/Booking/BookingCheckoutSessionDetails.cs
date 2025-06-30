using Api.Shared.Services.Models;
using HotChocolate;
using HotChocolate.Types.Relay;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("BookingCheckoutSessionDetails")]
public class BookingCheckoutSessionDetails
{
    [GraphQLName("uniqueId")] [ID] public required string UniqueId { get; set; }
    [GraphQLName("checkoutUrl")] public string CheckoutUrl { get; set; } = string.Empty;
    [GraphQLName("paymentStatus")] public PaymentStatus PaymentStatus { get; set; }
    [GraphQLName("amountTotal")] public string? AmountTotal { get; set; }
    [GraphQLName("amountTotalToDisplay")] public string AmountTotalToDisplay { get; set; } = string.Empty;
    [GraphQLName("currency")] public string? Currency { get; set; }
    [GraphQLName("currencyToDisplay")] public string CurrencyToDisplay { get; set; } = string.Empty;
}
