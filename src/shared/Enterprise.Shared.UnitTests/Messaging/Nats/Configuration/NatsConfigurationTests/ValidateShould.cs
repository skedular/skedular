using Enterprise.Shared.Messaging.Nats.Configuration;

namespace Enterprise.Shared.UnitTests.Messaging.Nats.Configuration.NatsConfigurationTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ValidateShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Reject_negative_redelivery_delay(string url)
    {
        var sut = new NatsConfiguration
        {
            Urls = [url],
            RedeliveryDelay = TimeSpan.FromSeconds(-1),
        };

        Should.Throw<ArgumentOutOfRangeException>(sut.Validate).ParamName.ShouldBe(nameof(NatsConfiguration.RedeliveryDelay));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Accept_default_and_immediate_redelivery(string url)
    {
        var sut = new NatsConfiguration
        {
            Urls = [url],
        };
        sut.RedeliveryDelay.ShouldBe(TimeSpan.FromSeconds(5));
        sut.Validate();
        sut.RedeliveryDelay = TimeSpan.Zero;
        sut.Validate();
    }

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
