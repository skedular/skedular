# Research: Credit-Based Booking Entitlements

## Decision: Reuse reservation marketplace payment and renewal patterns

**Rationale**: Existing Booking services already coordinate Stripe, bank-transfer invoices, Xero projection, subscription cadence, current product-version pricing, `NextRenewalAt`, cancellation, and payment status. Token fulfillment should vary the post-payment grant and later booking claim, not create a second payment system.

**Alternatives considered**: A standalone payment workflow was rejected because it would duplicate payment correlation, retry, accounting, and renewal behavior.

## Decision: Separate purchase/entitlement cycle from booking

**Rationale**: Purchase-time state must not reserve dates, resources, or quota. The entitlement is later referenced by an ordinary marketplace booking when a token is consumed.

**Alternatives considered**: Creating a placeholder booking was rejected because it violates the customer flow and would reserve capacity before a date is selected.

## Decision: Renewal uses current active pricing

**Rationale**: Existing marketplace auto-renew reloads the current product version and re-matches pricing. Historical purchase and entitlement-cycle snapshots remain immutable for audit and refund calculation.

**Alternatives considered**: Reusing the original snapshot was rejected because it could renew discontinued pricing or stale payment rules.

## Decision: Failed renewal expires the current cycle

**Rationale**: The current token cycle ends at its configured boundary; payment retries do not extend entitlement validity. A new cycle is granted only after confirmed payment.

**Alternatives considered**: Temporary grace access and optimistic grants were rejected because they create unconfirmed entitlement value and complicate rollback.

## Decision: Existing organization roles authorize operator actions

**Rationale**: Spaces/Host owners and administrators already have organization-scoped authorization. Requiring a new approval protocol would diverge from existing operator booking workflows. Every action must audit operator and customer.

**Alternatives considered**: Per-booking customer approval was rejected as unnecessary for existing authorized support/front-desk roles.

## Decision: One migration set and repository-only persistence

**Rationale**: Booking persistence follows the repository factory and EF migration conventions. Services, workflows, processors, and GraphQL resolvers do not access EF directly.

**Alternatives considered**: Separate migrations per surface and direct service DbContext access were rejected by repository and migration conventions.

## Decision: Transport layers depend on services, never repositories

GraphQL resolvers and other API transport adapters must not inject `IRepositoryFactory` or repository implementations.
Entitlement and purchase reads are exposed through Booking shared services as model-returning methods, and GraphQL
maps those models to transport details. This keeps authorization and persistence coordination in the service layer and
prevents database entities from crossing the transport boundary.

## Decision: Generated contracts remain source-driven

**Rationale**: GraphQL/Relay/OpenAPI/event changes must update source definitions and run the repository generators. Event protobufs are compiled by consuming csproj files.

**Alternatives considered**: Hand-editing generated output was rejected.
