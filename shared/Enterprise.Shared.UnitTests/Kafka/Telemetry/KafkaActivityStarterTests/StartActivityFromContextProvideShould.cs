using System.Diagnostics;
using Enterprise.Shared.Kafka.Telemetry;
using Enterprise.Shared.Telemetry;

namespace Enterprise.Shared.UnitTests.Kafka.Telemetry.KafkaActivityStarterTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class StartActivityFromContextProvideShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Use_producer_source_for_provide_type(
        [Frozen] IActivityAccessor activityAccessor,
        IActivitySource activitySource,
        KafkaActivityStarter starter)
    {
        A.CallTo(() => activityAccessor.GetActivitySource(TelemetryKeys.ProducerActivitySourceName))
            .Returns(activitySource);

        starter.StartActivityFromContext("some-topic", KafkaOperationType.Provide, default, null);

        A.CallTo(() => activityAccessor.GetActivitySource(TelemetryKeys.ProducerActivitySourceName))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Pass_partition_tag_when_provided(
        [Frozen] IActivityAccessor activityAccessor,
        IActivitySource activitySource,
        KafkaActivityStarter starter)
    {
        A.CallTo(() => activityAccessor.GetActivitySource(A<string>._))
            .Returns(activitySource);

        IEnumerable<KeyValuePair<string, object?>>? capturedTags = null;
        A.CallTo(() => activitySource.StartActivity(
                A<string>._,
                A<ActivityKind>._,
                A<ActivityContext>._,
                A<IEnumerable<KeyValuePair<string, object?>>>._))
            .Invokes((string _, ActivityKind _, ActivityContext _, IEnumerable<KeyValuePair<string, object?>> tags) =>
                capturedTags = tags);

        starter.StartActivityFromContext("topic", KafkaOperationType.Consume, default, partition: 3);

        capturedTags.ShouldNotBeNull();
        capturedTags!.ShouldContain(kv => kv.Key == SemanticConventions.MessagingKafkaPartition && kv.Value!.ToString() == "3");
    }
}
