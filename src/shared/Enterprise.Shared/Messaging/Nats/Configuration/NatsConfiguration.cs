namespace Enterprise.Shared.Messaging.Nats.Configuration;

public sealed class NatsConfiguration
{
    public const string Key = "Nats";

    public string[] Urls { get; set; } = [];
    public string? Name { get; set; }

    public void Validate()
    {
        if (Urls.Length == 0 || Urls.Any(string.IsNullOrWhiteSpace))
        {
            throw new ArgumentException("At least one NATS URL is required.", nameof(Urls));
        }
    }
}
