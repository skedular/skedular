namespace Api.Shared.Clients.Configurations.Grpc;

public class BookingConfiguration
{
    public const string Key = "Booking";

    public string ApiKey { get; set; } = string.Empty;
    public Uri? GrpcUrl { get; set; }
}
