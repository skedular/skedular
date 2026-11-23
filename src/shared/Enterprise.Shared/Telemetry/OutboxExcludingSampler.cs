using Enterprise.Shared.Database;
using OpenTelemetry.Trace;

namespace Enterprise.Shared.Telemetry;

public sealed class OutboxExcludingSampler : Sampler
{
    private readonly Sampler _parentBasedSampler = new ParentBasedSampler(new AlwaysOnSampler());

    public override SamplingResult ShouldSample(in SamplingParameters samplingParameters) =>
        OutboxTelemetryFilter.IsOutboxActivity(samplingParameters.Name)
            ? new SamplingResult(SamplingDecision.Drop)
            : _parentBasedSampler.ShouldSample(in samplingParameters);
}
