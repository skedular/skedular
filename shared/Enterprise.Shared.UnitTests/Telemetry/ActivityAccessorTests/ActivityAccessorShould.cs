using System.Diagnostics;
using Enterprise.Shared.Telemetry;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.UnitTests.Telemetry.ActivityAccessorTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ActivityAccessorShould
{
    private static ActivityAccessor BuildSut(
        IActivityGetter activityGetter,
        ILogger<ActivityAccessor> logger,
        params IActivitySource[] sources)
        => new(activityGetter, sources, logger);

    [Theory]
    [AutoFakeItEasyData]
    public void AddEvent_does_nothing_when_no_current_activity(
        IActivityGetter activityGetter,
        ILogger<ActivityAccessor> logger)
    {
        A.CallTo(() => activityGetter.GetCurrent()).Returns(null);
        var sut = BuildSut(activityGetter, logger);

        Should.NotThrow(() => sut.AddEvent("my-event", "prefix", new Dictionary<string, string>()));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void AddEvent_throws_when_name_is_empty(
        IActivityGetter activityGetter,
        ILogger<ActivityAccessor> logger)
    {
        var sut = BuildSut(activityGetter, logger);

        Should.Throw<ArgumentException>(() => sut.AddEvent("", "prefix", new Dictionary<string, string>()));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void AddEvent_throws_when_prefix_is_empty(
        IActivityGetter activityGetter,
        ILogger<ActivityAccessor> logger)
    {
        var sut = BuildSut(activityGetter, logger);

        Should.Throw<ArgumentException>(() => sut.AddEvent("name", "", new Dictionary<string, string>()));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void AddEvent_throws_when_tags_is_null(
        IActivityGetter activityGetter,
        ILogger<ActivityAccessor> logger)
    {
        var sut = BuildSut(activityGetter, logger);

        Should.Throw<ArgumentNullException>(() => sut.AddEvent("name", "prefix", null!));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void AddException_does_nothing_when_no_current_activity(
        IActivityGetter activityGetter,
        ILogger<ActivityAccessor> logger)
    {
        A.CallTo(() => activityGetter.GetCurrent()).Returns(null);
        var sut = BuildSut(activityGetter, logger);

        Should.NotThrow(() => sut.AddException(new Exception("test")));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void GetActivitySource_returns_noop_when_source_not_found(
        IActivityGetter activityGetter,
        ILogger<ActivityAccessor> logger,
        string sourceName)
    {
        var sut = BuildSut(activityGetter, logger);

        var source = sut.GetActivitySource(sourceName);

        source.ShouldNotBeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void GetActivitySource_returns_registered_source(
        IActivityGetter activityGetter,
        ILogger<ActivityAccessor> logger,
        IActivitySource activitySource,
        string sourceName)
    {
        A.CallTo(() => activitySource.Name).Returns(sourceName);
        var sut = BuildSut(activityGetter, logger, activitySource);

        var result = sut.GetActivitySource(sourceName);

        result.ShouldBeSameAs(activitySource);
    }
}
