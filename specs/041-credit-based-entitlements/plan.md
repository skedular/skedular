# Implementation Plan: Credit-Based Booking Entitlements

**Branch**: `041-credit-based-entitlements` | **Date**: 2026-08-11 | **Spec**: [spec.md](spec.md)

## Summary

Implement token-based product pricing as a second fulfillment path alongside reservation pricing. A token purchase follows the existing marketplace payment and renewal patterns, but creates no booking or resource reservation at purchase time. After confirmed payment, it grants a time-bounded entitlement. Customers and authorized Spaces/Host operators can later create, modify, and cancel ordinary bookings using that entitlement. Auto-renewal creates a new token cycle only after confirmed payment and re-evaluates current active pricing.

## Technical Context

**Language/Version**: C# .NET 10; TypeScript 6, React 19, Next.js 16, Astro
**Primary Dependencies**: EF Core/PostgreSQL, HotChocolate/Fusion GraphQL, Temporal, Stripe, Xero, Relay, MUI, `@skedular/ui`, `@skedular/shared`
**Storage**: Booking-owned PostgreSQL entitlement, purchase, ledger, renewal, and refund state through repositories and one EF migration set
**Testing**: .NET unit/integration tests, GraphQL schema tests, Vitest/React Testing Library, public-web tests
**Target Platform**: Booking services/jobs/processors, Skedular Spaces, Skedular Host, and public web
**Project Type**: Cross-domain backend and web application feature
**Performance Goals**: No new performance target; preserve existing marketplace booking/payment behavior
**Constraints**: Repository factory boundary; services return models; generated GraphQL/Relay outputs are regenerated;
Relay mutation payloads return rendered entitlement fields and stable IDs, while linked booking lists/counts use a
targeted refetch or connection update rather than `window.location.reload()`; purchase must not create
booking/resource/quota state; bank transfer is manually confirmed; Xero is an accounting projection
**Scale/Scope**: Existing marketplace customers, organizations, products, resources, payments, subscriptions, and entitlement surfaces in both Spaces and Host

## Constitution Check

- [x] **I. Contract-First** — GraphQL/OpenAPI/Relay changes use source definitions and required generators.
- [x] **II. Domain Boundaries** — Booking owns entitlement and booking/payment coordination; organization pricing remains accessed through existing public domain contracts and replicated state.
- [x] **III. Testing** — Unit tests cover rules first; integration tests cover persistence, migration, concurrency, Temporal, payment, and schema boundaries.
- [x] **IV. Frontend** — Relay artifacts are regenerated; mutation results update the Relay store directly and only
  linked booking lists/counts receive a targeted refetch; Spaces and Host receive equivalent customer/operator flows;
  public documentation is updated; existing typography wrappers are used.
- [x] **V. Pattern Consistency** — Token purchase and renewal reuse existing reservation marketplace payment/subscription patterns while separating purchase-time entitlement grant from later booking creation.
- [x] **VI. Logging** — Purchase, grant, renewal, payment, booking consumption, restoration, forfeiture, expiry, refund, operator action, and failure paths receive structured logs.

## Architecture and Data Flow

1. Product pricing declares `RESERVATION` or `ENTITLEMENT`, token quantity, validity, restrictions, refund policy, supported payment methods, and auto-renew.
2. Customer starts token purchase. Booking snapshots pricing and creates a pending purchase; no booking/resource/quota state is touched.
3. Stripe follows the existing automatic checkout/webhook path. Bank transfer follows invoice creation plus authorized manual confirmation. Xero remains accounting projection/manual settlement.
4. Confirmed payment grants exactly one entitlement cycle and ledger grant. Renewal uses the existing marketplace subscription workflow shape, current active pricing, and confirmed payment before granting the next cycle.
5. Later booking creation uses existing marketplace booking allocation with entitlement validation and atomic ledger consumption. Modification and cancellation reuse existing booking/resource and refund/cancellation rules.
6. Spaces and Host customer/operator screens use the same GraphQL contracts and authorization semantics; operator actions record acting operator and customer.

## Project Structure

```text
src/booking/shared/Booking.Shared/
├── Database/Entities/                 # purchase, entitlement, ledger, renewal persistence
├── Database/Migrations/                # one generated migration set
├── Repositories/                       # repository-factory persistence boundary
├── Models/Entitlements/                # service-facing models
├── Mappers/                            # injected entity/model and API mappings
├── Services/Entitlements/              # purchase, eligibility, grant, renewal, lifecycle
├── Workflows/                          # payment, renewal, expiry Temporal workflows
└── Activities/                         # Stripe, invoice, Xero integration activities
src/booking/apis/Booking.Api/
├── GraphQL/                            # purchase, entitlement, booking and admin contracts
├── Services/                           # authorization and orchestration
└── Mappers/                            # transport mappings
src/booking/processors/Booking.Processors/ # payment/webhook correlation
src/booking/domain/Booking.Domain.IntegrationTests/ # persistence/payment/schema tests
src/web/apps/webapp/                    # customer marketplace booking/entitlement UI
src/web/apps/webapp-host/               # Host customer and operator UI
src/web/apps/webapp-spaces/             # Spaces customer and operator UI
src/web/apps/public-web/                # public documentation
api-definitions/                        # GraphQL/OpenAPI/event source contracts
specs/041-credit-based-entitlements/    # design and validation artifacts
```

**Structure Decision**: Extend existing Booking ownership and marketplace surfaces. Do not create a parallel payment, booking, or customer-data subsystem.

## Delivery Phases

1. Foundation: model, repository, migration, pricing fields, mappers, status mappings.
2. Token purchase: Stripe/bank transfer/Xero projection, confirmation, grant, no-booking invariant.
3. Renewal: auto-renew cycle workflow, current-pricing re-match, failure/retry/idempotency.
4. Token booking lifecycle: customer and operator creation, resource allocation, modification, cancellation, restore/forfeit.
5. Read/admin/UI: balances, restrictions, payment actions, renewals, lifecycle, operator audit, Spaces/Host parity.
6. Generated contracts, public docs, integration validation, and graph refresh.

## Complexity Tracking

No constitution violations identified.
