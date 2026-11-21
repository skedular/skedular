namespace Enterprise.Shared.Messaging.Nats.Configuration;

public sealed class NatsConfiguration
{
    public const string Key = "Nats";

    public string[] Urls { get; set; } = [];
    public string? Name { get; set; }
    public TimeSpan RedeliveryDelay { get; set; } = TimeSpan.FromSeconds(5);

    public void Validate()
    {
        if (Urls.Length == 0 || Urls.Any(string.IsNullOrWhiteSpace))
        {
            throw new ArgumentException("At least one NATS URL is required.", nameof(Urls));
        }

        if (RedeliveryDelay < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(RedeliveryDelay), "NATS redelivery delay cannot be negative.");
        }
    }
}
