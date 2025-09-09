using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Types.Relay;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("BookingResourceDetails")]
public class BookingResourceDetails
{
    [GraphQLName("resourceId")] [ID] public string ResourceId { get; set; } = string.Empty;
    [GraphQLName("locationId")] public string? LocationId { get; set; }
    [GraphQLName("customerIds")] public IEnumerable<string> CustomerIds { get; set; } = [];
}

[ObjectType<BookingResourceDetails>]
public static partial class BookingResourceDetailsType
{
    static partial void Configure(IObjectTypeDescriptor<BookingResourceDetails> descriptor)
    {
        descriptor.Ignore(item => item.ResourceId);
        descriptor.Ignore(item => item.LocationId);
        descriptor.Ignore(item => item.CustomerIds);
    }

    public static ResourceDetails GetResource([Parent] BookingResourceDetails item) => new(item.ResourceId);

    public static LocationDetails? GetLocation([Parent] BookingResourceDetails item) => string.IsNullOrWhiteSpace(item.LocationId)
        ? null
        : new LocationDetails(item.LocationId);

    public static IEnumerable<CustomerDetails> GetCustomers([Parent] BookingResourceDetails item) =>
        item.CustomerIds.Select(id => new CustomerDetails(id));
}
