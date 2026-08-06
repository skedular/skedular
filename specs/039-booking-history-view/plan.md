# Implementation Plan: Unified Marketplace Booking History

**Branch**: `039-booking-history-view` | **Date**: 2026-08-02 | **Spec**: [spec.md](spec.md)

## Summary

Rename the existing operator-facing Subscriptions experience to **Marketplace purchases** in Spaces and Host while preserving `/subscriptions` URLs. Add one Booking-owned read contract that combines standalone marketplace bookings and subscription roots, all retained records first, newest activity first, with filters and keyset pagination. Subscription instances remain paginated/filterable in subscription details; booking cards/details link to their parent subscription.

No standalone booking becomes a subscription, and no renewal, allocation, cancellation, payment, or refund workflow changes.

## Technical Context

**Language/Version**: C# .NET 10; TypeScript 6.0.3, React 19.2.6, Next.js 16.2.6  
**Primary Dependencies**: EF Core/PostgreSQL, HotChocolate/Fusion GraphQL, Relay 21, MUI 9, `@skedular/ui`, `@skedular/shared`  
**Storage**: Booking-owned `MarketplacePurchaseHistory` durable read projection. It references the authoritative marketplace booking or subscription root and stores only the query/audit fields required by the operator feed; it is not a replacement source of truth.
**Testing**: xUnit/AutoFixture/FakeItEasy; Booking integration tests through repositories; Vitest/React Testing Library  
**Target Platform**: Booking API, Spaces app, Host app  
**Project Type**: Full-stack web service  
**Performance Goals**: First 50 authorized entries under 2 seconds at normal organization scale; stable cursor pagination on changing data  
**Constraints**: Preserve existing repository retention behavior without a new cutoff (indefinite when no policy exists); organization authorization; no parent/child duplicates; no raw EF in integration assertions; generated schemas/Relay artifacts are not edited; American English
**Scale/Scope**: All Spaces and Host organizations; unbounded history accessed page by page

## Constitution Check

_GATE: Passes before Phase 0 research and after Phase 1 design._

- [x] **I. Contract-First** — Update `src/booking/apis/Booking.Api/schema.graphqls` with the unified connection, filters/orders, choice details, subscription-instance connection, and parent link. Run `scripts/generate-graphql.sh`; run each operator app's `relay` script (`pnpm --dir src/web/apps/webapp-spaces relay` and `pnpm --dir src/web/apps/webapp-host relay`). Do not hand-edit outputs.
- [x] **II. Domain Boundaries** — The composite read model stays in Booking and uses Booking-owned sources and existing organization/team authorization services. Service contracts return shared models; GraphQL maps at the boundary. New enum-like values have explicit switch mappings.
- [x] **III. Testing** — Unit-test classification, authorization, filters, ordering, cursors, and links. Add focused repository integration tests for combined retained-history and subscription-instance pagination. Add Spaces/Host Vitest tests for filters, pagination, views, navigation, and states.
- [x] **IV. Frontend** — Update both apps' existing routes/link helpers/page queries; colocate Relay queries; use `@skedular/ui`; review/update Spaces, Host, and shared subscription documentation. Preserve the rule that one-time bookings do not create subscriptions.
- [x] **V. Pattern Consistency** — The Booking-owned composite read model is the smallest intentional extension: existing booking or subscription connections alone cannot represent the complete history. It reuses repository, connection, mapper, and grid/list patterns.
- [x] **VI. Logging** — Log authorized query scope, filters/order, source and total counts; warn for unresolved legacy relationships/classification gaps; include correlation context and omit PII/payment credentials.

## Project Structure

```text
specs/039-booking-history-view/
├── plan.md
├── research.md
├── data-model.md
├── contracts/graphql.md
└── quickstart.md

src/booking/
├── shared/Booking.Shared/
│   ├── Models/MarketplacePurchaseHistory*.cs
│   └── Repositories/MarketplacePurchaseHistoryRepository.cs
├── apis/Booking.Api/
│   ├── Services/MarketplacePurchaseHistoryService.cs
│   ├── GraphQL/MarketplacePurchaseHistory/
│   ├── GraphQL/MarketplaceBookingSubscription/
│   ├── Mappers/GraphQlMapper.cs
│   └── schema.graphqls
└── domain/Booking.Domain.IntegrationTests/

src/web/apps/
├── webapp-spaces/src/rootPages/organizations/organization/subscriptions/
├── webapp-host/src/rootPages/organizations/organization/subscriptions/
├── webapp-spaces/src/components/{links,booking}/
├── webapp-host/src/components/{links,booking}/
└── public-web/src/content/docs/
```

**Structure Decision**: Extend existing Booking shared-model → API-service → GraphQL mapping layers and matching Spaces/Host subscription routes. Preserve `/subscriptions` routes for existing links/bookmarks while changing visible terminology.

## Complexity Tracking

_No constitution violations requiring justification._
