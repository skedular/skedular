namespace Slack.Shared.Configurations;

public class BookingConfiguration
{
    public const string Key = "Booking";

    public string ApiKey { get; set; } = string.Empty;
    public Uri? GrpcUrl { get; set; }
}
