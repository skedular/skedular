using Enterprise.Shared.Logging;
using FluentAssertions;
using Serilog.Core;
using Serilog.Events;
using Testing.Shared;
using Xunit;

namespace Enterprise.Shared.UnitTests.Logging.GitHashEnvironmentVariableEnricherTests;

public class EnrichShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Add_Env_Variable_When_Empty(
        GitHashEnvironmentVariableEnricher sut,
        ILogEventPropertyFactory propertyFactory)
    {
        Environment.SetEnvironmentVariable("GIT_COMMIT_HASH", string.Empty);

        var logEvent = new LogEvent(DateTimeOffset.MinValue, LogEventLevel.Debug, null, MessageTemplate.Empty, []);

        sut.Enrich(logEvent, propertyFactory);

        logEvent.Properties.Should().ContainKey("GitHash");
        var value = logEvent.Properties["GitHash"];
        value.ToString().Should().Be("\"[---]\"");
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Set_To_Env_Variable_When_Present(
        GitHashEnvironmentVariableEnricher sut,
        ILogEventPropertyFactory propertyFactory)
    {
        Environment.SetEnvironmentVariable("GIT_COMMIT_HASH", "test");

        var logEvent = new LogEvent(DateTimeOffset.MinValue, LogEventLevel.Debug, null, MessageTemplate.Empty, []);

        sut.Enrich(logEvent, propertyFactory);

        logEvent.Properties.Should().ContainKey("GitHash");
        var value = logEvent.Properties["GitHash"];
        value.ToString().Should().Be("\"test\"");
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Set_To_Shortened_Env_Variable_When_Too_Long(
        GitHashEnvironmentVariableEnricher sut,
        ILogEventPropertyFactory propertyFactory)
    {
        Environment.SetEnvironmentVariable("GIT_COMMIT_HASH", "12345678901234567890");

        var logEvent = new LogEvent(DateTimeOffset.MinValue, LogEventLevel.Debug, null, MessageTemplate.Empty, []);

        sut.Enrich(logEvent, propertyFactory);

        logEvent.Properties.Should().ContainKey("GitHash");
        var value = logEvent.Properties["GitHash"];
        value.ToString().Should().Be("\"123456\"");
    }
}
