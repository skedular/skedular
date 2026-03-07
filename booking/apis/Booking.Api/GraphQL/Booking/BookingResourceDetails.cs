using HotChocolate;
using HotChocolate.Types;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("BookingResourceDetails")]
public class BookingResourceDetails
{
    [GraphQLName("resource")] public ResourceDetails Resource { get; set; } = new();
    [GraphQLName("locationId")] public string? LocationId { get; set; }
    [GraphQLName("customerIds")] public IEnumerable<string> CustomerIds { get; set; } = [];
}

[ObjectType<BookingResourceDetails>]
public static partial class BookingResourceDetailsType
{
    static partial void Configure(IObjectTypeDescriptor<BookingResourceDetails> descriptor)
    {
        descriptor.Ignore(item => item.LocationId);
        descriptor.Ignore(item => item.CustomerIds);
    }

    public static LocationDetails? GetLocation([Parent] BookingResourceDetails item) => string.IsNullOrWhiteSpace(item.LocationId)
        ? null
        : new LocationDetails(item.LocationId);

    public static IEnumerable<CustomerDetails> GetCustomers([Parent] BookingResourceDetails item) =>
        item.CustomerIds.Select(id => new CustomerDetails(id));
}
