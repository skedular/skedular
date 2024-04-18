namespace Slack.Shared.Configurations;

public class BookingConfiguration
{
    public const string Key = "Booking";

    public string ApiKey { get; set; }
    public Uri? GrpcUrl { get; set; }
}
