using System.Diagnostics;
using AutoFixture.Xunit3;
using Enterprise.Shared.Kafka.Telemetry;
using Enterprise.Shared.Telemetry;
using FakeItEasy;
using FluentAssertions;
using FluentAssertions.Execution;
using Testing.Shared;
using Xunit;

namespace Enterprise.Shared.UnitTests.Kafka.Telemetry.KafkaActivityStarterTests;

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
                    IEnumerable<KeyValuePair<string, object>> _) =>
                received = parentContext);

        starter.StartActivityFromContext("something", KafkaOperationType.Consume, default,
            0);

        using (new AssertionScope())
        {
            received.TraceId.Should().NotBeNull();
            received.TraceId.Should().NotBe(new ActivityTraceId());
            received.SpanId.Should().NotBeNull();
            received.SpanId.Should().NotBe(new ActivitySpanId());
            received.TraceFlags.Should().Be(ActivityTraceFlags.Recorded);
        }
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
                    IEnumerable<KeyValuePair<string, object>> _) =>
                received = parentContext);

        starter.StartActivityFromContext("something", KafkaOperationType.Consume, expected,
            0);

        received.Should().Be(expected);
    }
}
