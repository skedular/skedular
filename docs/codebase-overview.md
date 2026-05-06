# Codebase Overview

## What is Skedular?

Skedular is a scheduling and resource-booking platform delivered as a mono-repository. It is built around a set of independently deployable backend microservices (domains), a federated GraphQL gateway, and a React/Relay web application.

---

## Repository Layout

```
unityhubio/
├── api-definitions/        # Source-of-truth API contracts (OpenAPI, GraphQL, gRPC, protobuf events)
├── all-in-one/             # Composite Aspire projects for running all services together
├── booking/                # Booking domain
├── core/                   # Core domain
├── customer/               # Customer domain
├── gateway/                # GraphQL federation gateway
├── location/               # Location domain
├── marketplace/            # Marketplace domain
├── msteams/                # Microsoft Teams integration
├── organization/           # Organization domain
├── slack/                  # Slack integration
├── system/                 # System-level utilities
├── team/                   # Team domain
├── shared/                 # Cross-cutting .NET libraries
├── web/                    # Frontend web application
├── scripts/                # Developer and CI helper scripts
├── docs/                   # Documentation (you are here)
└── docker-compose*.yml     # Local infrastructure definitions
```

---

## Domain Structure

Every domain follows the same internal layout:

```
<domain>/
├── apis/           # HTTP API project + unit tests
├── domain/         # Aspire AppHost, integration tests, and fake dependencies
├── jobs/           # Background jobs + unit tests
├── processors/     # Kafka event processors + unit tests
└── shared/         # Domain-internal shared projects (EF migrations, repositories, etc.)
```

| Domain         | Responsibility                                                                                  |
| -------------- | ----------------------------------------------------------------------------------------------- |
| `organization` | Tenants, users, SSO, billing settings, Xero connection                                          |
| `booking`      | Resource reservations, recurring bookings, marketplace subscriptions, payment/invoice workflows |
| `location`     | Venues, floors, rooms, desks, and resources                                                     |
| `marketplace`  | Product catalogue, pricing, subscription plans                                                  |
| `customer`     | Customer profiles and contact data                                                              |
| `team`         | Internal team management                                                                        |
| `core`         | Platform-wide cross-cutting concerns                                                            |
| `gateway`      | Apollo/HotChocolate GraphQL federation gateway                                                  |

---

## Technology Stack

| Layer            | Technology                                                              |
| ---------------- | ----------------------------------------------------------------------- |
| Backend language | C# / .NET 10                                                            |
| API style        | GraphQL (primary, federated), REST/OpenAPI (secondary), gRPC (internal) |
| Frontend         | React 18, Relay, Next.js, MUI                                           |
| Event bus        | Apache Kafka (protobuf-serialised events)                               |
| Workflow engine  | Temporal                                                                |
| Database         | PostgreSQL (per-domain schema, Entity Framework Core)                   |
| Cache            | Redis                                                                   |
| Auth             | Keycloak (OIDC), SSO via SAML (Azure AD / Auth0)                        |
| Infrastructure   | Docker Compose (local), Aspire (service orchestration)                  |

---

## API Contract Definitions

All API contracts live under `api-definitions/` and are the **source of truth**. Generated code must never be edited by hand.

| Contract type   | Source location                                   | Generated output                                                                                                                             |
| --------------- | ------------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------- |
| OpenAPI (REST)  | `api-definitions/openapi/skedular/*.yaml`         | C# controller bases in `shared/Api.Shared.Services/OpenApi`, C# clients in `shared/Api.Shared.Clients/OpenApi`, TypeScript clients in `web/` |
| GraphQL schema  | per-domain `schema.graphql` (exported by service) | Composed gateway schema at `api-definitions/graphql/skedular/v1/schema.graphql`, Relay artifacts                                             |
| Protobuf events | `api-definitions/events/skedular/*.proto`         | C# event classes compiled into `shared/Api.Shared.Clients/obj`                                                                               |
| gRPC            | `api-definitions/grpc/skedular/**/*.proto`           | C# generated by consuming `.csproj` files at build time                                                                                      |

Run `make generate` to regenerate all surfaces in the correct order.

---

## Shared Libraries

`shared/` contains cross-cutting .NET projects consumed by all domains:

| Project                 | Purpose                                                                                            |
| ----------------------- | -------------------------------------------------------------------------------------------------- |
| `Api.Shared`            | Portable (netstandard2.0) shared event contracts, Kafka topic attributes, and metadata factories   |
| `Api.Shared.Clients`    | Typed clients and protobuf event metadata for inter-service communication; depends on `Api.Shared` |
| `Api.Shared.Services`   | Generated OpenAPI controller bases, shared middleware, service extensions                          |
| `Enterprise.Shared`     | Xero SDK factory, token encryption, and other enterprise-level services                            |
| `Infrastructure.Shared` | Common EF helpers, outbox patterns, and persistence utilities                                      |
| `Testing.Shared`        | Test fixtures, builders, and helpers shared across all test projects                               |
| `Skedularctl`           | CLI tool for developer tasks                                                                       |

---

## Frontend Application

The web application lives in `web/apps/webapp/`. It is a **Next.js** + **Relay** application that communicates exclusively through the GraphQL federation gateway.

- TypeScript clients generated from OpenAPI specs via `web/apps/webapp/scripts/generate.sh`
- GraphQL Relay artifacts regenerated whenever backend schemas change
- Custom MUI typography wrappers in `src/components/commons` — use these instead of importing `@mui/material/Typography` directly in feature components

---

## Event-Driven Architecture

Services publish and consume domain events over Kafka. Events are defined as protobuf schemas under `api-definitions/events/skedular/`.

Shared event contracts (`IEvent`, `IEventExtensions`, `KafkaTopicAttribute`, `KafkaTopicHelper`, `EventMetadataFactory`) are defined in `shared/Api.Shared` (netstandard2.0 for cross-platform compatibility). Domain-specific event metadata companions live in `shared/Api.Shared.Clients/Events/Skedular/`.

Each domain's `processors/` project contains the Kafka consumer/processor logic that uses these event contracts.

An event catalog is maintained under `docs/event-catalog/` and browsable via EventCatalog. See [Event Documentation Strategy ADR](adr-event-catalog.md) for context.

---

## Workflow Engine

Long-running business processes (subscription renewals, payment workflows, recurring booking reconciliation) are implemented as [Temporal](https://temporal.io) workflows. Workflow IDs are constructed through each domain's `WorkflowIdService` — never inline at call sites.

---

## Further Reading

| Document                       | Location                                                                                              |
| ------------------------------ | ----------------------------------------------------------------------------------------------------- |
| Architecture Decision Records  | [docs/adr-index.md](adr-index.md)                                                                     |
| SSO Integration                | [docs/sso-integration.md](sso-integration.md)                                                         |
| Event Catalog ADR              | [docs/adr-event-catalog.md](adr-event-catalog.md)                                                     |
| System Context (C4 L1)         | [docs/architecture/level-1-system-context/](architecture/level-1-system-context/)                     |
| Container Diagram (C4 L2)      | [docs/architecture/level-2-container-diagram/](architecture/level-2-container-diagram/)               |
| Component Diagrams (C4 L3)     | [docs/architecture/level-3-component-diagram-\*/](architecture/)                                      |
| Xero Bank Transfer Integration | [docs/architecture/xero-bank-transfer-integration.md](architecture/xero-bank-transfer-integration.md) |
| Local setup                    | [README.md](../README.md)                                                                             |
