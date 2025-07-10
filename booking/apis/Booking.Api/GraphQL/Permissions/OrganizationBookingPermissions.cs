using HotChocolate;

namespace Booking.Api.GraphQL.Permissions;

[GraphQLName("OrganizationBookingPermissions")]
public class OrganizationBookingPermissions
{
    [GraphQLName("canViewBookings")] public bool CanViewBookings { get; set; }
    [GraphQLName("canAddBooking")] public bool CanAddBooking { get; set; }
    [GraphQLName("canUpdateBooking")] public bool CanUpdateBooking { get; set; }
    [GraphQLName("canDeleteBooking")] public bool CanDeleteBooking { get; set; }

    [GraphQLName("canModifyPaymentMethod")]
    public bool CanModifyPaymentMethod { get; set; }
}
