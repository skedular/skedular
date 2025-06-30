using HotChocolate;

// ReSharper disable ClassNeverInstantiated.Global

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("LineItemInput")]
public class LineItemInput
{
    [GraphQLName("productVersionId")] public string ProductVersionId { get; set; } = string.Empty;
    [GraphQLName("quantity")] public int Quantity { get; set; }
}
