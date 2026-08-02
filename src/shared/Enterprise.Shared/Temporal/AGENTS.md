# Temporal Module — Agent Notes

## Purpose

Registers the Temporal .NET SDK worker and client with environment-qualified task queues, mTLS support, configurable
capacity and rate limits, and the outbox writers needed for transactional workflow execution.

## Registration

### Worker host (runs workflows and activities)

```csharp
services.AddTemporalWorker(
    configuration,
    deploymentName: "my-service",
    buildId: "1.0.0",
    connectionName: "temporal");   // Aspire connection string name
```

This also registers `AddTemporalOutboxService()` so `ITemporalOutboxWorkflowExecutor` and
`ITemporalSignalOutboxWorkflowExecutor` are available immediately.

### Client-only host (starts workflows, no worker)

```csharp
services.AddTemporalClient(configuration, connectionName: "temporal");
```

**Config section key:** `Temporal` — see `Temporal/Configurations/TemporalConfiguration.cs`.

**Prerequisites:** `AddCoreServices<TProgram>()` (registers `ApplicationConfiguration` for environment prefix).
`AddTemporalOutboxBackgroundService<TDbContext>()` must be called separately if the host needs the outbox drain loop.

## Task Queue Naming

The task queue is automatically prefixed with the environment name from `ApplicationConfiguration`:

```
{environment}.{Temporal:Worker:TaskQueue}
```

e.g. `development.bookings-worker` or `production.bookings-worker`.

The deployment name receives the same prefix so Temporal versioning stays environment-scoped.

## Configuration Reference

```json
{
  "Temporal": {
    "Worker": {
      "TaskQueue": "my-worker",
      "Capacity": {
        "MaxConcurrentWorkflowTaskExecutors": 100,
        "MaxConcurrentActivityExecutors": 100,
        "MaxConcurrentLocalActivityExecutors": 100,
        "MaxConcurrentWorkflowTaskPollers": 5,
        "MaxConcurrentActivityTaskPollers": 5
      },
      "RateLimits": {
        "MaxWorkerActivitiesPerSecond": null,
        "MaxTaskQueueActivitiesPerSecond": null
      },
      "Cache": { "MaxInstances": 1000 }
    },
    "Connection": {
      "Namespace": "default",
      "Target": "localhost:7233",
      "Mtls": {
        "KeyFile": "/path/to/client.key",
        "CertChainFile": "/path/to/client.pem"
      }
    }
  }
}
```

`Connection.Target` may be overridden by the Aspire connection string at runtime.
`Mtls` is optional; omit the section entirely when mTLS is not used.

## Workflow Type Helpers

```csharp
// Returns the fully qualified CLR name used as the Temporal workflow type
typeof(MyWorkflow).ToWorkflowType();

// Returns "MyWorkflow.MySignalMethod" for use as a signal type string
typeof(MyWorkflow).GetMethod("MySignalMethod").ToWorkflowSignalType();
```

These helpers live in `Temporal/Extensions.cs` and are used internally by the outbox executors.

## ITemporalHelperService

`ITemporalHelperService` provides a `ToId(...)` method for constructing deterministic workflow IDs from structured
inputs. Use this service instead of building IDs inline in calling code.

## Rules

- Always use the environment-prefixed task queue — never hardcode a raw queue name in a workflow registration call.
- Workflow IDs must be constructed through a domain `WorkflowIdService`, not inline at the call site.
- Do not register workflows or activities directly on `ITemporalClient`; use the worker builder's
  `.AddWorkflow<T>()` / `.AddActivity<T>()` methods.
- mTLS cert paths should come from configuration or secrets, not from hardcoded paths.
