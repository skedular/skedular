using OpenTelemetry.Context.Propagation;

namespace Enterprise.Shared.Telemetry;

/// <summary>
///     The standard propagator for our purposes.
/// </summary>
public sealed class StandardTextMapPropagator() : CompositeTextMapPropagator(Propagators)
{
    private static readonly TextMapPropagator[] Propagators = [new TraceContextPropagator(), new BaggagePropagator()];

    public override ISet<string> Fields { get; } =
        new HashSet<string>(Propagators.SelectMany(propagator => propagator.Fields));
}
