namespace Slack.Shared.Models;

public class LocationBookingPermissions
{
    public bool CanViewBookings { get; set; }
    public bool CanAddBooking { get; set; }
    public bool CanUpdateBooking { get; set; }
    public bool CanDeleteBooking { get; set; }
    public bool CanAddBookingOnBehalf { get; set; }
    public bool CanUpdateBookingOnBehalf { get; set; }
    public bool CanDeleteBookingOnBehalf { get; set; }
}
