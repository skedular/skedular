using HotChocolate;
using HotChocolate.Types;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("BookingResourceDetails")]
public class BookingResourceDetails
{
    [GraphQLName("resource")] public ResourceDetails Resource { get; set; } = new();
    [GraphQLName("location")] public LocationDetails? Location { get; set; }
    [GraphQLName("customerIds")] public IEnumerable<string> CustomerIds { get; set; } = [];
}

[ObjectType<BookingResourceDetails>]
public static partial class BookingResourceDetailsType
{
    static partial void Configure(IObjectTypeDescriptor<BookingResourceDetails> descriptor) => descriptor.Ignore(item => item.CustomerIds);

    public static IEnumerable<CustomerDetails> GetCustomers([Parent] BookingResourceDetails item) =>
        item.CustomerIds.Select(id => new CustomerDetails(id));
}
