using HotChocolate;
using HotChocolate.Types;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("LineItemDetails")]
public class LineItemDetails
{
    [GraphQLName("productVersionId")] public string ProductVersionId { get; set; } = string.Empty;
    [GraphQLName("quantity")] public int Quantity { get; set; }
}

[ObjectType<LineItemDetails>]
public static partial class LineItemDetailsType
{
    static partial void Configure(IObjectTypeDescriptor<LineItemDetails> descriptor) => descriptor.Ignore(item => item.ProductVersionId);

    public static ProductVersionDetails GetProductVersion([Parent] LineItemDetails item) => new(item.ProductVersionId);
}
