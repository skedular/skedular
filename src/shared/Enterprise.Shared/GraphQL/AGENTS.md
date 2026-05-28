# GraphQL Module — Agent Notes

## Purpose

Registers and configures a HotChocolate GraphQL server with cost analysis, Redis or in-memory
subscriptions, introspection controls, and per-request context propagation.

## Registration

```csharp
services.AddGraphql(
    configuration,
    schemaName: "management-api",  // HotChocolate schema name; must match the Fusion subgraph
                                   // schema-settings.json `name` field
    configure: builder =>
    {
        builder
            .AddQueryType<Query>()
            .AddMutationType<Mutation>()
            .AddSubscriptionType<Subscription>();
    },
    useRedisSubscriptions: true,   // false → in-memory (local dev / tests)
    useAuthorization: true);

// HTTP endpoint mapping is performed automatically by UseWebApplicationDefaults<TProgram>().
// If you bypass that helper, call MapGraphqlEndpoints(configuration) yourself.
```

**Config section key:** `GraphQL` — see `GraphQL/Configurations/GraphqlConfig.cs`.

`MapGraphqlEndpoints` discovers every schema registered through `AddGraphql` (via the
`GraphqlSchemaRegistration` singletons it adds to DI) and maps each one to its configured `Path`.
It is a no-op when no schemas are registered, when the `GraphQL` configuration section is absent,
or when a registration's `Path` is empty — so it is safe to call from non-GraphQL hosts
(processors, job runners).

### Multiple subgraphs in one host

A single domain can host more than one GraphQL subgraph. Call `AddGraphql` once per subgraph with
distinct `schemaName` values; each registration becomes its own named HotChocolate schema and its
own HTTP route. The Redis subscription topic prefix is keyed off the schema name
(`{environment}:{schemaName}:`) so subgraphs do not collide on topics.

> Today every schema in a host shares the same top-level `GraphQL` config section (so they share
> `Path`, `IntrospectionEnabled`, etc.). When a real second subgraph is wired into the same host,
> introduce per-schema config (e.g. `GraphQL:Management:Path`, `GraphQL:Catalog:Path`) so each
> subgraph can be mounted on its own route.

## Configuration Reference

```json
{
  "GraphQL": {
    "Path": "/graphql",
    "IntrospectionEnabled": true,
    "NitroEnabled": true,
    "IncludeCookies": true,
    "DisableTelemetry": false,
    "CollectOperationPlanTelemetry": false,
    "AllowErrorHandlingModeOverride": false,
    "ExecutionTimeout": "00:00:30",
    "IncludeExceptionDetails": false
  }
}
```

The Fusion-specific properties map directly onto `HotChocolate.Fusion.Execution.FusionRequestOptions`
via `ModifyRequestOptions` in the gateway bootstrap. `ExecutionTimeout` is optional — when omitted
HotChocolate's default (30 s, 100 ms minimum) is used.

## Subscriptions

- Redis subscriptions require a registered `IConnectionMultiplexer` (via `Cache/Extensions.cs`
  `AddRedis`). Topics are prefixed with `{environment}:{schemaName}:` from `ApplicationConfiguration`
  plus the `schemaName` passed to `AddGraphql`.
- Use `IGraphQlTopicEventSender` (wrapper around `ITopicEventSender`) to send subscription events
  from domain services without a direct HotChocolate dependency.

## Pagination

The `Pagination/` module provides cursor-based (keyset) pagination helpers:

```csharp
await query.ToPaginatedAsync(paginationInput, keysetFields, cancellationToken);
```

Returns `PaginatedInfo<T>` with edges, `PageInfo`, and total count. Use `Connection<T>` / `Node<T>` /
`PageInfo` types from `GraphQL/Types/` as the GraphQL-facing output types.

## Error Handling

`GraphqlErrorFilter` (registered by `AddGraphql`) catches unhandled exceptions and converts them to
GraphQL error responses. Do not let domain exceptions propagate unfiltered to the client.

## Context Propagation

`RequestContextPropagationHandler` (`GraphQL/Handlers/`) is a `DelegatingHandler` that forwards
request-scoped headers (e.g. correlation ID) to downstream HTTP calls made during GraphQL field
resolution.

`RewriteHostHandler` (`GraphQL/Handlers/`) is a generic `DelegatingHandler` that rewrites the
scheme/host/port of the outgoing request to a configured target while preserving path and query.
Used by the Fusion gateway to support a single composed `gateway.far` artifact across environments
where subgraph URLs are supplied at runtime via configuration.

## Rules

- Always add `HotChocolate.Fusion.SourceSchema` annotations when the schema is part of a federated
  gateway — do not expose raw types without subgraph annotations.
- Do not hand-edit exported `schema.graphql` / `schema.graphqls` files; regenerate via
  `scripts/generate-graphql.ps1`.
- Follow the choice-type pattern (see root `AGENTS.md` → *GraphQL Choice Types*) for enum-backed
  selection fields.
- Always pass an explicit `schemaName` to `AddGraphql`. Do not derive it from
  `ApplicationConfiguration.Domain` — a single domain may host multiple subgraphs.
