using OpenTelemetry.Context.Propagation;

namespace Enterprise.Shared.Telemetry;

/// <summary>
///     The standard propagator for our purposes.
/// </summary>
public sealed class StandardTextMapPropagator() : CompositeTextMapPropagator(s_propagators)
{
    private static readonly TextMapPropagator[] s_propagators = [new TraceContextPropagator(), new BaggagePropagator()];

    public override ISet<string> Fields { get; } =
        new HashSet<string>(s_propagators.Where(propagator => propagator.Fields is not null)
            .SelectMany(propagator => propagator.Fields!));
}
