# Telemetry Module — Agent Notes

## Purpose

Configures OpenTelemetry traces and metrics with a standard set of instrumentations, exporters, and
a reusable context propagation API for passing trace context across async and messaging boundaries.

## Registration

```csharp
// Called automatically by AddCoreServices<TProgram>()
services.ConfigureOpenTelemetry(configuration, appName);
```

For hosts that do not call `AddCoreServices`, register directly:

```csharp
services.ConfigureOpenTelemetry(configuration, appName);
```

**Config section key:** `OpenTelemetry` — see `Telemetry/Configurations/OpenTelemetryConfiguration.cs`.

## Configuration Reference

```json
{
  "OpenTelemetry": {
    "ConsoleEnabled": false,
    "MetricsIngestEnabled": true,
    "EntityFrameworkEnabled": true,
    "MeterProviderName": "my-service"
  }
}
```

- `MetricsIngestEnabled` — enables the custom application meter (`MeterProviderName`).
- `EntityFrameworkEnabled` — adds EF Core query instrumentation.
- `ConsoleEnabled` — writes traces and metrics to stdout (development / debugging only).

## Registrations Made by `ConfigureOpenTelemetry`

| Service                                                   | Description                                                 |
|-----------------------------------------------------------|-------------------------------------------------------------|
| `IActivityAccessor`                                       | Get or start activities from a named `ActivitySource`       |
| `IActivityGetter`                                         | Read the current ambient `Activity`                         |
| `TextMapPropagator` (`StandardTextMapPropagator`)         | W3C TraceContext + Baggage propagator                       |
| `IActivityPropagator<T>`                                  | Generic context inject/extract API (open-generic singleton) |
| `IPropagationContextGetter`                               | Extract a `PropagationContext` from any carrier             |
| `IPropagatorFunctionProvider<IDictionary<string,string>>` | Dictionary-based propagation (outbox / Temporal)            |
| `IPropagatorFunctionProvider<IPropagatorEntity>`          | Entity-based propagation                                    |

## Using `IActivityPropagator<T>`

```csharp
// Inject IDictionary activity context into a message header dict
dictionaryPropagator.PropagateActivity(headers);

// Start a child activity from an existing propagation context
using var activity = dictionaryPropagator.StartActivityFromPropagationContext(
    headers, activitySource, "operation-name", ActivityKind.Consumer);
```

## Adding Custom Metrics

```csharp
// Implement ITaggable on your metric input type
public class MyMetricTags : ITaggable { ... }

// Create a Meter in your service
private static readonly Meter s_meter = new(MeterProviderNaming.MeterProviderName);
private static readonly Counter<long> s_counter = s_meter.CreateCounter<long>(MetricNames.MyOperation);
```

Use `MetricTaggingExtensions.ToTagList(ITaggable)` to convert tag objects to `TagList` for recording.

## Rules

- Do not create `ActivitySource` instances directly in domain code — use `IActivityAccessor` to
  retrieve or start activities so the source name stays consistent.
- Health check routes are filtered from traces automatically; do not add custom filters for them.
- The custom meter name must match `OpenTelemetryConfiguration.MeterProviderName` in config; do not
  hardcode a string.
