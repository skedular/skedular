using OpenTelemetry.Context.Propagation;

namespace Enterprise.Shared.Telemetry;

public interface IPropagationContextGetter
{
    PropagationContext? GetPropagationContext();
}
