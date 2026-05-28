namespace Booking.Shared.Models;

public class OrganizationPermissions
{
    public bool CanViewBookings { get; set; }
    public bool CanAddBooking { get; set; }
    public bool CanUpdateBooking { get; set; }
    public bool CanDeleteBooking { get; set; }
    public bool CanModifyPaymentMethod { get; set; }
}
