# Cache Module — Agent Notes

## Purpose

Registers StackExchange.Redis as both `IConnectionMultiplexer` and `IDistributedCache`. Other modules
that depend on Redis (e.g. `HybridCache`, GraphQL Redis subscriptions) require this module to be
registered first.

## Registration

```csharp
services.AddRedis(configuration, connectionName: "redis");
```

**Config source:** `ConnectionStrings:{connectionName}` (standard ASP.NET Core connection string).

The connection string is passed directly to `ConnectionMultiplexer.Connect(...)` so any valid
StackExchange.Redis connection string format is accepted (e.g. `localhost:6379` or a full options
string with `password`, `ssl`, etc.).

## What Gets Registered

| Service                  | Implementation                            | Lifetime  |
|--------------------------|-------------------------------------------|-----------|
| `IConnectionMultiplexer` | `ConnectionMultiplexer`                   | Singleton |
| `IDistributedCache`      | `RedisCache` (StackExchange.Redis-backed) | Singleton |

## HybridCache

`AddHybridCaching()` (in root `Extensions.cs`) layers `HybridCache` on top of this Redis registration.
Call `AddRedis` before `AddHybridCaching` when using both:

```csharp
services.AddRedis(configuration, "redis");
builder.AddHybridCaching();   // uses the IConnectionMultiplexer already registered
```

## GraphQL Redis Subscriptions

`AddGraphql(..., useRedisSubscriptions: true)` also requires a registered `IConnectionMultiplexer`.
Ensure `AddRedis` is called before `AddGraphql`.

## Rules

- Do not register `IConnectionMultiplexer` manually in host code — always go through `AddRedis` so
  the connection string stays in configuration.
- For local development without Redis, use `useRedisSubscriptions: false` in `AddGraphql` and swap
  out `IDistributedCache` with an in-memory implementation; do not disable Redis registration
  unconditionally in production.
