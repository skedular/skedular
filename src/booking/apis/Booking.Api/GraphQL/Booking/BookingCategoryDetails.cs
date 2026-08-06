using Api.Shared.Services.Models;
using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("BookingCategoryDetails")]
public class BookingCategoryDetails
{
    [GraphQLName("category")]
    public BookingCategory Category { get; set; }

    [GraphQLName("name")]
    public string Name { get; set; } = string.Empty;
}
