# Database Module — Agent Notes

## Purpose

Provides EF Core base types, the repository/unit-of-work abstractions, and database-specific
registration helpers for PostgreSQL and SQL Server. Outbox entity configuration is in
`Outbox/Kafka/` and `Outbox/Temporal/` — see `Outbox/AGENTS.md`.

## Sub-modules

| Sub-module        | Namespace                              | Notes                                                             |
|-------------------|----------------------------------------|-------------------------------------------------------------------|
| Core abstractions | `Enterprise.Shared.Database`           | `EntityBase`, `IRepository<T>`, `IUnitOfWork`, `Specification<T>` |
| PostgreSQL        | `Enterprise.Shared.Database.Postgres`  | Aspire-aware registration, PostGIS, health checks                 |
| SQL Server        | `Enterprise.Shared.Database.SqlServer` | Parallel API to Postgres; use when targeting SQL Server           |

## Registration — PostgreSQL

```csharp
// Pooled DbContext (recommended for APIs and high-throughput services)
services.WithPooledDbContext<MyDbContext>(configuration, environment, connectionName: "mydb");

// With PostGIS (enables NetTopologySuite)
services.WithPooledDbContext<MyDbContext>(configuration, environment, "mydb", isPostgisEnabled: true);

// Pooled DbContextFactory (for background services and concurrent access)
services.WithPooledDbContextFactory<MyDbContext>(configuration, environment, "mydb");

// Non-pooled (for migration tools or single-use contexts)
services.WithDbContext<MyDbContext>(configuration, environment, "mydb");
```

All variants accept an optional `healthCheckName` to register a PostgreSQL readiness check.

**Config section key:** `ConnectionStrings:{connectionName}` (standard ASP.NET Core connection string).

**Prerequisites:** `AddCoreServices<TProgram>()` (registers `ApplicationConfiguration` for query
splitting behaviour) and a registered `NpgsqlDataSource`.

## Registration — SQL Server

Same API surface, parallel methods:

```csharp
services.WithPooledDbContext<MyDbContext>(configuration, environment, "mydb");
// WithPooledDbContextWithConnectionString, WithDbContext, WithDbContextFactory variants — identical shape.
```

## Base Types

| Type                    | Use                                                             |
|-------------------------|-----------------------------------------------------------------|
| `EntityBase`            | Owned entity with auto-generated `Id`, `CreatedAt`, `UpdatedAt` |
| `EntityBaseWithDeleted` | `EntityBase` + soft-delete `DeletedAt`                          |
| `ReplicatedEntityBase`  | Read-side entity replicated from another context                |
| `ModelBase`             | DTO/GraphQL model base without EF annotations                   |
| `ModelBaseWithDeleted`  | `ModelBase` + `IsDeleted` helper                                |

## Repository Pattern

```csharp
// Query through specifications — do not expose IQueryable outside the repository
public class MyRepository(MyDbContext db) : RepositoryBase<MyDbContext, MyEntity>(db), IMyRepository { }

// Implement unit-of-work through the DbContext
public class MyDbContext : DbContextBase<MyDbContext>, IUnitOfWork, IKafkaOutboxStore { ... }
```

- `IRepository<T>` exposes queryable access via `Specification<T>`.
- `SpecificationEvaluator` applies `Where`, `Include`, `OrderBy`, and pagination to an `IQueryable`.
- `IUnitOfWork` wraps `SaveChangesAsync` — always commit through the unit-of-work, not `DbContext` directly.

## Interceptors

`SelectForUpdateCommandInterceptor` rewrites `SELECT` statements inside a transaction to use
`FOR UPDATE SKIP LOCKED`. This is used by the outbox background services to claim rows safely.
Do not remove this interceptor from outbox-enabled contexts.

## DbContext Configuration

Call `ConfigureEntityBase(builder)` inside `OnModelCreating` for each entity extending `EntityBase`:

```csharp
builder.Entity<MyEntity>(b => b.ConfigureEntityBase());
```

## Health Checks

```csharp
services.AddDatabaseHealthCheck(dataSource, healthCheckName: "mydb");
```

Registers a readiness health check that verifies database connectivity. The health check appears on
`/health/readiness`.

## Rules

- Do not query `DbContext` or EF directly from integration tests. Use repository methods.
- Do not call `SaveChanges()` inside a loop — batch changes and call once per unit-of-work boundary.
- `QuerySplittingBehavior` defaults to `SplitQuery` (set in `ApplicationConfiguration`); override
  per-query with `.AsSingleQuery()` only when a split produces incorrect results.
- Migration assemblies must be co-located in the same assembly as the `DbContext`.
