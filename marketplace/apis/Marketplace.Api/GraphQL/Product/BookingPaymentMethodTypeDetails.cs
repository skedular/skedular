using Api.Shared.Services.Models;
using HotChocolate;

namespace Marketplace.Api.GraphQL.Product;

[GraphQLName("Marketplace_BookingPaymentMethodTypeDetails")]
public class BookingPaymentMethodTypeDetails
{
    [GraphQLName("type")] public BookingPaymentMethod Type { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
}
