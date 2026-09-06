using Enterprise.Shared.Messaging.Nats.Configuration;

namespace Enterprise.Shared.UnitTests.Messaging.Nats.Configuration.NatsConfigurationTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ValidateShould
{
    [Fact]
    public void Reject_missing_urls()
    {
        var options = new NatsConfiguration();

        Should.Throw<ArgumentException>(options.Validate);
    }

    [Fact]
    public void Accept_valid_options()
    {
        var options = new NatsConfiguration
        {
            Urls = ["nats://localhost:4222"],
        };

        Should.NotThrow(options.Validate);
    }
}
