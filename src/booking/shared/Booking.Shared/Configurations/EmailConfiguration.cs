namespace Booking.Shared.Configurations;

public class EmailConfiguration
{
    public const string Key = "Email";

    public string BookingInvoiceEmailSender { get; set; } = string.Empty;
}
