using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("LineItemDetails")]
public class LineItemDetails
{
    [GraphQLName("productVersion")] public ProductVersionDetails ProductVersionDetails { get; set; } = new();
    [GraphQLName("quantity")] public int Quantity { get; set; }
}
