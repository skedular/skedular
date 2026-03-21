using System.Diagnostics;
using AutoFixture.Xunit3;
using Enterprise.Shared.Kafka.Telemetry;
using Enterprise.Shared.Telemetry;
using FakeItEasy;

// using FluentAssertions;
// using FluentAssertions.Execution;

namespace Enterprise.Shared.UnitTests.Kafka.Telemetry.KafkaActivityStarterTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class StartActivityFromContextShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Create_Trace_If_Default(
        [Frozen] IActivityAccessor activityAccessor,
        IActivitySource activitySource,
        KafkaActivityStarter starter)
    {
        A.CallTo(() =>
                activityAccessor.GetActivitySource(TelemetryKeys.ConsumerActivitySourceName))
            .Returns(activitySource);

        ActivityContext received = default;

        A.CallTo(() => activitySource.StartActivity(
                "something receive", ActivityKind.Consumer,
                A<ActivityContext>.Ignored,
                A<IEnumerable<KeyValuePair<string, object?>>>.Ignored))
            .Invokes((
                    string _,
                    ActivityKind _,
                    ActivityContext parentContext,
                    IEnumerable<KeyValuePair<string, object?>> _) =>
                received = parentContext);

        starter.StartActivityFromContext("something", KafkaOperationType.Consume, default,
            0);

        received.TraceId.ShouldNotBe(default);
        received.TraceId.ShouldNotBe(new ActivityTraceId());
        received.SpanId.ShouldNotBe(default);
        received.SpanId.ShouldNotBe(new ActivitySpanId());
        received.TraceFlags.ShouldBe(ActivityTraceFlags.Recorded);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Use_Trace_If_Provided(
        [Frozen] IActivityAccessor activityAccessor,
        IActivitySource activitySource,
        KafkaActivityStarter starter)
    {
        var expected = new ActivityContext(ActivityTraceId.CreateRandom(),
            ActivitySpanId.CreateRandom(), ActivityTraceFlags.Recorded);

        A.CallTo(() =>
                activityAccessor.GetActivitySource(TelemetryKeys.ConsumerActivitySourceName))
            .Returns(activitySource);

        ActivityContext received = default;

        A.CallTo(() => activitySource.StartActivity(
                "something receive", ActivityKind.Consumer,
                A<ActivityContext>.Ignored,
                A<IEnumerable<KeyValuePair<string, object?>>>.Ignored))
            .Invokes((
                    string _,
                    ActivityKind _,
                    ActivityContext parentContext,
                    IEnumerable<KeyValuePair<string, object?>> _) =>
                received = parentContext);

        starter.StartActivityFromContext("something", KafkaOperationType.Consume, expected, 0);

        received.ShouldBe(expected);
    }
}
