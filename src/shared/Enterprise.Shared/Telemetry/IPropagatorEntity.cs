namespace Enterprise.Shared.Telemetry;

public interface IPropagatorEntity
{
    string? TraceContext { get; set; }
}
