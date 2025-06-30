using HotChocolate;
using HotChocolate.Types.Relay;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("Booking_ProductVersionDetails")]
public class ProductVersionDetails
{
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; } = string.Empty;
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("price")] public string Price { get; set; } = string.Empty;
    [GraphQLName("priceToDisplay")] public string PriceToDisplay { get; set; } = string.Empty;
    [GraphQLName("priceUnit")] public PriceUnitDetails PriceUnit { get; set; } = new();
    [GraphQLName("currency")] public CurrencyDetails Currency { get; set; } = new();
    [GraphQLName("organization")] public OrganizationDetails Organization { get; set; } = new();
}
