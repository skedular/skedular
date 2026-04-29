# Implementation Plan: Subscription Landing Page Filtering

**Branch**: `005-subscription-landing-page-filter` | **Date**: 2026-04-27 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `/specs/005-subscription-landing-page-filter/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/plan-template.md` for the execution workflow.

## Summary

Upgrade the coworking space owner Management Portal subscription list page from single-select, client-side status and payment status filters to multi-select, backend-driven filters. The available filter option values come from two new GraphQL queries in the booking domain API (`marketplaceBookingSubscriptionStatuses`, `marketplaceBookingPaymentStatuses`). The subscription list query is extended to accept multi-value `statuses` and `paymentStatuses` filters via `MarketplaceBookingSubscriptionWhereInput`, applied server-side in the repository. The UI re-issues a fresh Relay query on every filter selection change; selected filter values are reflected in the URL query string.

## Technical Context

**Language/Version**: C# .NET 10 (backend), TypeScript 6 / React 19 / Next.js 16 App Router (frontend)  
**Primary Dependencies**: HotChocolate (GraphQL), Entity Framework Core, Relay, MUI v9, `mui-rff` Autocomplete, `useSearchParams`/`useRouter` (Next.js)  
**Storage**: PostgreSQL — no new migrations; filtering via existing indexed `Status` and `MarketplaceBooking.PaymentStatus` columns  
**Testing**: xUnit + FakeItEasy (backend unit), Aspire integration tests (backend integration), Vitest + React Testing Library (frontend)  
**Target Platform**: Web (Next.js App Router), .NET 10 backend microservice  
**Project Type**: Full-stack feature (booking domain API + management portal webapp)  
**Performance Goals**: Backend query result within normal p95 for the subscription list; no client-side data loading for filtering  
**Constraints**: No new DB migrations; multi-select filter must reuse the existing `multiple-choices-*` component shape; Relay artefacts must be regenerated, not hand-edited  
**Scale/Scope**: Per-organisation subscription list, typically tens to hundreds of rows

## Constitution Check

_GATE: Must pass before Phase 0 research. Re-check after Phase 1 design._

- [x] **I. Contract-First** — This feature changes the GraphQL schema (new queries + extended `where` input). The correct generator is `scripts/generate-graphql.sh` (backend schema) and `web/apps/webapp/scripts/generate.sh` (Relay artefacts). Both must run before any downstream code is committed.
- [x] **II. Domain Boundaries** — All changes stay within the booking domain. The `MarketplaceBooking.PaymentStatus` join is within the same booking `DbContext`. No cross-domain DB access.
- [x] **III. Testing** — Unit tests required for repository filter logic and service criteria mapping. Integration tests required for paginated subscription queries with multi-value filters. Frontend Vitest tests required for filter components and page behaviour. All integration-test persistence assertions use the repository layer, not raw `DbContext`.
- [x] **IV. Frontend** — Relay fragments collocated with components; generated artefacts not hand-edited; typography uses `@skedular/ui` wrappers; British spelling in user-facing labels (e.g. "Renewal failed", "No payment required").
- [x] **V. Pattern Consistency** — Follows existing patterns: `multiple-choices-*.tsx` for multi-select combo boxes, backend-driven `*Details` types for filter option values, `*SearchCriteria` record extension for backend filtering. No new patterns introduced.
- [x] **VI. Logging** — LOG-001 through LOG-004 in the spec cover all required logging surfaces. Structured logging must be added to the resolver and service for filter input receipt, unrecognised values, and option-load failures.

_Post-Phase 1 re-check_: Constitution passes. No violations.

## Project Structure

### Documentation (this feature)

```text
specs/005-subscription-landing-page-filter/
├── plan.md              ← this file
├── research.md          ← Phase 0 output
├── data-model.md        ← Phase 1 output
├── quickstart.md        ← Phase 1 output
├── contracts/
│   └── graphql-filter-api.md  ← Phase 1 output
└── tasks.md             ← Phase 2 output (/speckit.tasks)
```

### Source Code (affected paths)

```text
# Backend — Booking domain
booking/apis/Booking.Api/GraphQL/MarketplaceBookingSubscription/
├── MarketplaceBookingPaymentStatusDetails.cs       ← new
├── MarketplaceBookingSubscriptionWhereInput.cs     ← extend (add statuses, paymentStatuses)
└── RootQuery.cs                                    ← extend (add 2 queries + wire new inputs)

booking/shared/Booking.Shared/Models/
└── MarketplaceBookingSubscriptionSearch.cs         ← extend SearchCriteria record

booking/shared/Booking.Shared/Repositories/
└── MarketplaceBookingSubscriptionRepository.cs     ← extend AddSearchCriteria

booking/apis/Booking.Api/schema.graphql             ← regenerated, not hand-edited
booking/domain/Booking.Domain.IntegrationTests/schema.graphql  ← regenerated

# Tests — Backend
booking/apis/Booking.Api.UnitTests/Services/MarketplaceBookingSubscriptionServiceTests/
└── (extend or add unit tests for new filter inputs)

booking/domain/Booking.Domain.IntegrationTests/
└── (add integration tests for multi-status + payment-status filtering)

# Frontend — Management portal webapp
web/apps/webapp/src/components/marketplaceProductSubscription/
├── marketplace-booking-subscription-status.ts       ← new type helper
└── marketplace-booking-payment-status.ts            ← new type helper

web/apps/webapp/src/components/organization/
├── multiple-choices-marketplace-booking-subscription-statuses.tsx  ← new
└── multiple-choices-marketplace-booking-payment-statuses.tsx       ← new

web/apps/webapp/src/rootPages/organizations/organization/subscriptions/
└── page.tsx                                          ← update filter state + query

web/apps/webapp/src/queries/__generated__/
└── (regenerated Relay artefacts — not hand-edited)
```

## Complexity Tracking

No constitution violations. No entries required.

---

## Phase 0: Research Findings

See [research.md](research.md) for full findings. Key resolved decisions:

| Unknown                               | Resolution                                                                                                                              |
| ------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------- |
| Which domain hosts subscription list? | Booking domain API                                                                                                                      |
| PaymentStatus location on entity      | `MarketplaceBooking.PaymentStatus` (string, indexed) — 1:1 nav from `MarketplaceBookingSubscription`                                    |
| Existing multi-select pattern         | `multiple-choices-product-pricing-billing-modes.tsx` — `mui-rff` `Autocomplete` with `multiple`, `disableCloseOnSelect`, Relay fragment |
| Backend-driven options pattern        | `marketplaceBookingSubscriptionCancellationModes()` → `Details` type with `type` + `name`                                               |
| Current filter implementation         | Already exists but single-select + fully client-side — must be replaced                                                                 |
| EF Core PaymentStatus join            | Navigation property predicate works without explicit Include; generates a JOIN                                                          |
| URL state                             | `useSearchParams` + `useRouter` (Next.js App Router) — already used in this codebase                                                    |

---

## Phase 1: Design

### 1a. Data Model

See [data-model.md](data-model.md). Summary:

- **No new DB entities or migrations** — filtering uses existing indexed columns
- Extend `MarketplaceBookingSubscriptionSearchCriteria` with `Statuses` and `PaymentStatuses` collections
- Extend `MarketplaceBookingSubscriptionWhereInput` with `statuses` and `paymentStatuses` GraphQL inputs
- Add `MarketplaceBookingPaymentStatusDetails` GraphQL type
- Add two new root queries for filter option values

### 1b. Interface Contracts

See [contracts/graphql-filter-api.md](contracts/graphql-filter-api.md). Summary:

- `marketplaceBookingSubscriptionStatuses: [MarketplaceBookingSubscriptionStatusDetails!]!` — returns all 5 subscription status options
- `marketplaceBookingPaymentStatuses: [MarketplaceBookingPaymentStatusDetails!]!` — returns operator-relevant payment status options
- `MarketplaceBookingSubscriptionWhereInput` extended with `statuses` and `paymentStatuses` array inputs
- Filtering semantics: empty array = no restriction; non-empty = IN clause; both non-empty = AND

### 1c. Implementation Approach

#### Backend sequence

1. Add `MarketplaceBookingPaymentStatusDetails` C# class (mirrors `MarketplaceBookingSubscriptionStatusDetails`)
2. Add `ToMarketplaceBookingPaymentStatusName()` extension for display names
3. Add `marketplaceBookingSubscriptionStatuses` and `marketplaceBookingPaymentStatuses` to `RootQuery.cs`
4. Extend `MarketplaceBookingSubscriptionWhereInput` — add `Statuses` and `PaymentStatuses`
5. Extend `MarketplaceBookingSubscriptionSearchCriteria` — add both collections (empty by default)
6. Update `RootQuery.cs` `MarketplaceBookingSubscriptionsAsync` — pass new fields through to SearchCriteria
7. Update `AddSearchCriteria` in repository — add multi-value status `Contains` predicate and payment status `Contains` predicate via `item.MarketplaceBooking.PaymentStatus`
8. Add logging in resolver (LOG-001, LOG-002) and in any service layer that validates inputs
9. Run `scripts/generate-graphql.sh`

#### Frontend sequence

1. Add `marketplace-booking-subscription-status.ts` type helper (union type of valid status strings + `Details` type + guard)
2. Add `marketplace-booking-payment-status.ts` type helper (same pattern)
3. Add `multiple-choices-marketplace-booking-subscription-statuses.tsx` (Relay fragment + `Autocomplete multiple`)
4. Add `multiple-choices-marketplace-booking-payment-statuses.tsx` (same pattern)
5. Update `page.tsx` subscriptions root query:
   - Add `marketplaceBookingSubscriptionStatuses { type name }` fragment
   - Add `marketplaceBookingPaymentStatuses { type name }` fragment
   - Add `$statuses` and `$paymentStatuses` variables to the subscription list query
6. Replace `statusFilter`/`paymentFilter` string state with `selectedStatuses[]`/`selectedPaymentStatuses[]` arrays
7. Add URL read/write via `useSearchParams` and `useRouter`
8. Remove client-side `filteredSubscriptions` memo (status/payment portion)
9. On filter change: push URL update → `useQueryLoader` re-loads with new variables
10. Add skeleton loading indicator during query in-flight
11. Run `web/apps/webapp/scripts/generate.sh`

### 1d. Testing Plan

#### Backend unit tests

| Test class                | Method                                   | Scenario                                               |
| ------------------------- | ---------------------------------------- | ------------------------------------------------------ |
| `AddSearchCriteriaShould` | `FilterByStatuses`                       | Single status, multiple statuses, empty = all returned |
| `AddSearchCriteriaShould` | `FilterByPaymentStatuses`                | Single, multiple, empty = all returned                 |
| `AddSearchCriteriaShould` | `FilterByCombinedStatusAndPaymentStatus` | AND semantics verified                                 |

#### Backend integration tests

| Scenario                                                            | Assertion via                                    |
| ------------------------------------------------------------------- | ------------------------------------------------ |
| `marketplaceBookingSubscriptions` with `statuses: [ACTIVE]`         | Repository query — only active returned          |
| `marketplaceBookingSubscriptions` with `paymentStatuses: [PENDING]` | Repository query — only pending payment returned |
| `marketplaceBookingSubscriptions` with both filters                 | Repository query — AND semantics                 |
| `marketplaceBookingSubscriptionStatuses` query                      | Returns all 5 options                            |
| `marketplaceBookingPaymentStatuses` query                           | Returns expected options                         |

#### Frontend tests

| Test                                                 | Scope               |
| ---------------------------------------------------- | ------------------- |
| Filter combo renders options from backend data       | Unit (Vitest + RTL) |
| Selecting a status updates URL query param           | Unit                |
| Page re-queries with filter vars on selection change | Unit                |
| Skeleton shown during in-flight query                | Unit                |
| Pre-populated from URL on initial load               | Unit                |

---

## Quickstart

See [quickstart.md](quickstart.md) for validation steps.
