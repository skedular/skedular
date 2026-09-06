# Implementation Plan: Automatic Marketplace Renewal Charging

**Branch**: `049-marketplace-auto-charge` | **Date**: 2026-09-06 | **Spec**: [spec.md](spec.md)

## Summary

Implement a Booking-owned, purchase-specific Stripe renewal authorization and payment lifecycle for every card-paid marketplace purchase supporting AutoRenew. Initial/fallback purchase checkout captures explicit off-session authorization; a single idempotent PaymentIntent is then created per renewal cycle. The existing customer-profile payment method is never an eligibility or selection source. Products/Prices remain catalog references only when they exactly match a renewal; dynamic renewal values are charged directly without creating catalog objects per renewal.

## Technical Context

**Language/Version**: C# .NET 10; TypeScript 6.0.3; React 19.2.6
**Primary Dependencies**: Stripe .NET SDK, EF Core/PostgreSQL, Temporal, HotChocolate/Fusion GraphQL, Relay 21, Kafka/webhook processing
**Storage**: Booking-owned PostgreSQL entities/migrations and append-only MarketplacePurchaseHistory events; Stripe-side Checkout Sessions, PaymentIntents, Products/Prices, and purchase-scoped credential references
**Testing**: xUnit/AutoFakeItEasyData, unit tests, persistence/concurrency and Stripe/webhook integration tests, GraphQL contract tests, Relay/Vitest tests
**Target Platform**: Skedular backend and web applications
**Project Type**: Multi-domain service/web application
**Performance Goals**: 95% eligible renewals confirmed within 10 minutes; failure recovery visible within 5 minutes; maximum one successful charge per renewal cycle
**Constraints**: Explicit purchase-specific consent; never use customer-profile cards; three automatic attempts over three days; three-day fallback link; payment confirmation before access/credits; preserve Connect charge type; forward-only migrations
**Scale/Scope**: Reservation subscriptions, credit entitlements, and future card-paid AutoRenew purchase types

## Constitution Check

_Passed before Phase 0 and re-checked after Phase 1._

- [x] **I. Contract-First** — Changes to GraphQL/events originate in `api-definitions/`; run the repository generators, including Relay generation, rather than editing generated outputs.
- [x] **II. Domain Boundaries** — Booking owns renewal/payment state and services; APIs call services rather than repositories; Stripe and customer/organization collaborators cross public service/event boundaries. New enum mappings are explicit; `CancellationToken` remains final.
- [x] **III. Testing** — Unit tests first for lifecycle, calculation, idempotency, and retry behavior. Integration tests only cover migration, persistence/concurrency, Temporal, Stripe/webhook, and schema boundaries.
- [x] **IV. Frontend** — Relay payloads include stable IDs and rendered fields; connections update declaratively or by targeted refetch; no reload after mutation success; American English and shared typography wrappers apply; documentation is reviewed for customer/admin behavior.
- [x] **V. Pattern Consistency** — Purchase-specific authorization extends existing Booking payment/history patterns and does not repurpose Customer’s profile payment-method aggregate. Applied migrations remain immutable.
- [x] **VI. Logging** — Structured correlation and durable event writes cover renewal evaluation, attempt, outcome, fallback, webhook, retry, recovery, cancellation, and refund paths.

## Project Structure

```text
specs/049-marketplace-auto-charge/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   ├── marketplace-renewal-payment.graphql.md
│   ├── stripe-renewal-payment.md
│   └── marketplace-renewal-events.md
└── tasks.md                         # produced by /speckit-tasks

src/booking/shared/Booking.Shared/
├── Activities/                       # Stripe and Temporal activity seams
├── Database/Entities/                # persisted renewal/payment records
├── Database/Migrations/              # forward-only migrations
├── Services/                         # service-owned orchestration
└── Workflows/                        # renewal/retry/recovery workflows
src/booking/processors/Booking.Processors/Subscribers/
└── BookingInternalSubscriber.cs      # Stripe webhook reconciliation
src/booking/apis/Booking.Api/GraphQL/
└── ...                               # service-backed customer/admin API surface
src/web/apps/webapp-spaces/src/
└── ...                               # customer/admin Relay surfaces
```

**Structure Decision**: Extend Booking-owned marketplace lifecycle seams. Customer’s Stripe profile state remains separate; only Stripe identifiers technically required for the purchase authorization are referenced locally. No transport layer receives repository access.

## Phase Results

- [research.md](research.md): Stripe/payment/catalog and Connect decisions.
- [data-model.md](data-model.md): persisted entities, uniqueness, validation, and state transitions.
- [contracts/](contracts/): GraphQL, Stripe, and lifecycle event contracts.
- [quickstart.md](quickstart.md): focused validation guide.

## Complexity Tracking

No constitution exceptions are required.
