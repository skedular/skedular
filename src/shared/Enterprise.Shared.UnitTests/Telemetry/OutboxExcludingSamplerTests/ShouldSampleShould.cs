using System.Diagnostics;
using Enterprise.Shared.Outbox.Kafka;
using Enterprise.Shared.Telemetry;
using OpenTelemetry.Trace;

namespace Enterprise.Shared.UnitTests.Telemetry.OutboxExcludingSamplerTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ShouldSampleShould
{
    [Fact]
    public void Drop_Outbox_Operations()
    {
        var parameters = new SamplingParameters(
            default,
            ActivityTraceId.CreateRandom(),
            TelemetryKeys.KafkaEventPoll,
            ActivityKind.Internal);

        var result = new OutboxExcludingSampler().ShouldSample(in parameters);

        result.Decision.ShouldBe(SamplingDecision.Drop);
    }

    [Fact]
    public void Record_Application_Operations()
    {
        var parameters = new SamplingParameters(
            default,
            ActivityTraceId.CreateRandom(),
            "application-operation",
            ActivityKind.Internal);

        var result = new OutboxExcludingSampler().ShouldSample(in parameters);

        result.Decision.ShouldBe(SamplingDecision.RecordAndSample);
    }
}
