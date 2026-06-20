# Implementation Plan: Skedular Spaces Free Trial

**Branch**: `030-spaces-free-trial` | **Date**: 2026-06-27 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/030-spaces-free-trial/spec.md`

## Summary

Convert the Skedular Spaces Free plan into a one-time 14-day trial while retaining its existing 100-booking-instance monthly quota. Organization owns an immutable trial-start anchor and computes full authenticated subscription status; Booking and Location receive trial dates through the existing Organization event projection and evaluate access locally at mutation time. Active/expiring trials remain subject to the Free quota, expired trials block new operations and bookings while retaining read/recovery/protective actions, and paid plans retain their existing quota behavior. The current explicit paid-upgrade flow provides immediate paid-plan access plus the accepted complimentary bridge to month-end, with the first full charge at the next first-of-month boundary. Operator UI lives in `webapp-spaces`, public customer booking availability lives in `webapp`, and pricing/marketing copy lives in `public-web`; Teams behavior remains untouched.

## Technical Context

**Language/Version**: C# .NET 10 backend; `Api.Shared.Services` remains `netstandard2.0`; TypeScript 6.0.3, React 19.2.6, Next.js 16.2.6 App Router; Astro public website  
**Primary Dependencies**: EF Core/PostgreSQL, HotChocolate/Fusion GraphQL, Kafka/protobuf Organization events, Temporal offering renewal/payment workflows, Stripe payment methods/payment intents, Relay 21, `react-relay`, MUI 9, `@skedular/ui`, `@skedular/shared`, Vitest, React Testing Library  
**Storage**: Organization-owned PostgreSQL adds durable `SpacesTrialStartedAt` on Organization and nullable `SpacesBillingStartsAt` on OrganizationOffering; Booking and Location retain trial/access metadata inside their existing replicated Organization/Offering JSON state; no booking usage table or new customer-data store  
**Testing**: .NET unit tests for access/status evaluation, subscription initialization/transitions, active-trial quota enforcement, expiry denial, billing bridge, logs, and Teams isolation; Organization/Booking/Location integration tests with repository-layer persistence assertions and event projection; Vitest/React Testing Library for operator and customer UI; public-web Vitest; generated schema/artifact checks
**Target Platform**: Existing Skedular Linux backend services and browser applications in the monorepo  
**Project Type**: Cross-domain web-service, background workflow, operator frontend, customer marketplace frontend, and static public website feature  
**Performance Goals**: Local access evaluation adds less than 5 ms p95 excluding existing repository/cache reads; active Free-trial booking checks do not execute booking-usage count queries; status/booking availability reflects time or offering changes within one minute; existing paid quota check and pricing-page targets do not regress. The evaluator target is verified with a focused performance test, and unit tests verify that active-trial decisions never invoke the booking-usage repository.
**Constraints**: Exactly 14 elapsed 24-hour periods from the authoritative one-time start; 3-day warning threshold; Free trial has no count-based limits; paid quotas remain unchanged; explicit upgrade and attached payment method are required for complimentary bridge; first full charge occurs next first day of month; no prorated bridge charge; protected cancellation/refund/closure remains available; full status is private while public availability is neutral; generated files are never hand-edited; American English copy; Teams/private organizations remain isolated  
**Scale/Scope**: All existing and new Marketplace/Spaces organizations, all one-off/recurring/admin/customer booking creation paths, operator organization routes, customer marketplace listing/booking surfaces, and public Spaces pricing content; no new checkout provider, email warnings, Teams trial, trial extensions, or abuse controls

## Constitution Check

_GATE: Passed before research and re-checked after design._

- [x] **I. Contract-First** — Organization event protobuf source changes are made under `api-definitions/events/skedular` and regenerated with `api-definitions/events/generate.sh` before consumer code changes. GraphQL server types/resolvers are changed first and exported/composed with `scripts/generate-graphql.sh`; Relay artifacts are then regenerated with `src/web/apps/webapp/scripts/generate.sh` before frontend consumers change. `make generate` remains the final drift check. Generated protobuf, schema, and Relay files are not edited directly.
- [x] **II. Domain Boundaries** — Organization owns trial/subscription/billing state. Booking and Location receive only authorization-relevant dates and access metadata through Organization events and evaluate locally. Booking owns public and private booking eligibility, quota behavior, and final booking enforcement. An API project MUST NOT synchronously call another domain's API; cross-domain propagation is allowed only through processor subscribers, Temporal workflows, or jobs. No domain reads another domain's database.
- [x] **III. Testing** — Unit tests cover every backend decision and transition. Persistence/event/workflow changes receive integration tests with repository queries rather than raw `DbContext`. Cross-domain system coverage is reserved for the end-to-end expiry/upgrade path. Web changes use Vitest/React Testing Library, with existing Playwright coverage extended only for the highest-value operator/customer flow.
- [x] **IV. Frontend** — Relay selections remain colocated, artifacts are regenerated, typography comes from `@skedular/ui`, runtime helpers come from `@skedular/shared`, responsive/keyboard states are tested, and all surfaced copy uses American spelling.
- [x] **V. Pattern Consistency** — The design extends existing Organization offering ownership, Kafka replication, local offering authorization, Booking quota boundaries, and Temporal calendar-month renewal. The only new shared abstraction is an action-aware Spaces access evaluator because the existing boolean/capacity evaluator cannot represent time-derived expiry plus protective exceptions; it remains portable and product-scoped.
- [x] **VI. Logging** — Structured logs are planned for trial initialization/creation-date fallback, every access branch, first warning/expired observations, event projection, booking allow/deny, upgrade/bridge scheduling, renewal/payment failure, retry, and Teams-isolation decisions. Tests assert level, event name, status/reason, organization correlation, and absence of sensitive data.

## Project Structure

### Documentation (this feature)

```text
specs/030-spaces-free-trial/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   ├── access-policy.md
│   ├── event-spaces-trial.md
│   ├── graphql-spaces-trial.md
│   └── billing-lifecycle.md
└── tasks.md                       # created by /speckit-tasks
```

### Source Code (repository root)

```text
src/shared/Api.Shared.Services/
├── Models/Offering.cs             # replicated trial/access dates
└── Offering/
    ├── SpacesAccessAction.cs
    ├── SpacesAccessDecision.cs
    ├── SpacesAccessReasonCode.cs
    ├── SpacesSubscriptionStatus.cs
    └── SpacesAccessEvaluator.cs   # portable evaluator; caller supplies now

src/organization/shared/Organization.Shared/
├── Database/Entities/Organization.cs
├── Database/Entities/OrganizationOffering.cs
├── Database/Migrations/           # schema-only trial anchor and bridge billing start
├── Models/Organization.cs
├── Models/PricingCatalog/OrganizationSpacesSubscription.cs
├── Services/Pricing/              # catalog and transition/status mapping
├── Mappers/                       # entity/event projection
├── Activities/OrganizationOfferings.cs
└── Workflows/ScheduleRenewOrganizationOffering.cs

src/organization/apis/Organization.Api/
├── Services/Pricing/OrganizationSpacesSubscriptionService.cs
├── Services/OrganizationOfferingService.cs
└── GraphQL/Pricing/               # private member/support status and upgrade response

api-definitions/events/skedular/
└── organization_v1_value.proto    # time-stable trial/access inputs

src/booking/processors/Booking.Processors/
└── Subscribers/OrganizationSubscriber.cs

src/booking/shared/Booking.Shared/
├── Services/SpacesBookingQuotaService.cs
├── Services/PrivateBookingService.cs
├── Services/MarketplaceBookingService.cs
└── Activities/PrivateRecurringBookingIntegrations.cs

src/booking/apis/Booking.Api/
└── GraphQL/Booking/               # quota/status, public Organization availability extension, and access-error payload

src/location/
├── processors/                    # Organization projection mapping
├── shared/                        # replicated offering state
└── apis/Location.Api/Services/Authorization/
    └── OrganizationOfferingService.cs

src/web/apps/webapp-spaces/src/
├── components/rootShell/          # persistent status/warning/access context
├── components/organization/organizationAdmin/
│   └── subscription/trial/upgrade UI
└── components/booking/            # expired errors and disabled creation controls

src/web/apps/webapp/src/
├── components/organizationStoreFrontGuest/
├── components/marketplaceProductBooking/
└── components/marketplaceProductSubscription/

src/web/apps/public-web/
├── src/data/pricing-catalog/pricing-catalog.ts
├── src/data/spaces-page.ts
├── src/pages/llms.txt.ts
├── src/pages/llms-full.txt.ts
└── tests/
```

**Structure Decision**: Keep the trial source and billing transition in Organization; extend the existing event projection so Booking and Location make local, time-correct access decisions; evolve the current Booking quota service instead of creating parallel trial enforcement. Organization exposes authenticated member/support subscription status. Booking exposes neutral public booking availability as a federated `Organization` field and remains the final booking authority. No API-to-API cross-domain call is introduced. Operator, customer, and marketing UI remain in their existing product owners.

## Implementation Strategy

### 1. Shared access contract

- Add Spaces-specific status, reason, action, and decision models to `Api.Shared.Services` with complete conversions/display names and unit tests.
- Implement a pure evaluator accepting `now`, trial anchor, current offering, and action. It returns trial dates, ceiling-rounded remaining days, status, booking/operation/protective access flags, next billing date, and upgrade requirement.
- Preserve `netstandard2.0` by passing time explicitly and using only compatible APIs.

### 2. Organization trial ownership and migration

- Add nullable `SpacesTrialStartedAt` through entity/model/mappers and a schema-only migration.
- Keep `SpacesFreeTierV1` as the Free offering identity. From the rollout date, derive the effective anchor for an existing Spaces Free organization from its original `CreatedAt` when `SpacesTrialStartedAt` is absent. Do not run a customer data backfill or migrate subscription codes. New Spaces organizations and first-time Spaces enablement persist the immutable anchor normally, and the existing Organization event projects the effective value to Booking and Location.
- Initialize at creation for Marketplace organizations and exactly once when Spaces is first enabled for an older Teams-only organization.
- Extend default-Free assignment to initialize/publish the anchor transactionally without touching organization data beyond subscription state.
- Change Free catalog/offering definitions to a 14-day trial while retaining the 100-booking-instance monthly limit and `Free` plan code for compatibility.

### 3. Subscription status and billing transitions

- Extend `OrganizationSpacesSubscriptionService.GetAsync` and GraphQL mapping with the evaluated status/access contract.
- Route every self-service Spaces paid transition through the existing `OrganizationOfferingService` validation/outbox behavior: require payment method, cancel the old workflow, activate paid-plan access immediately, persist the complimentary bridge marker/billing start, and schedule the first full-month transition.
- Add a Spaces bridge-boundary branch that idempotently charges and persists the upcoming full calendar-month offering rather than charging the expiring bridge row; after that first successful transition, resume the existing paid renewal workflow unchanged.
- Remove or delegate the specialized mutation's divergent direct-update path so it cannot skip payment/workflow/outbox behavior.
- Preserve Growth/Business/Contact Us prices, quotas, discounts, existing paid periods, and regular renewal semantics; the special branch applies only to a newly upgraded complimentary bridge.
- On bridge cancellation, stop the workflow, create no retroactive charge, retain the original trial anchor, and return to expired Free state.

### 4. Cross-domain projection and access enforcement

- Add trial anchor/end and product-scoped billing metadata to the Organization event offering payload, event mapper, and Booking/Location processor mappers.
- Persist projection values in existing Offering JSONB; no new consumer-domain table is required.
- Update Booking's quota service to evaluate access first: active/expiring Free trials continue through the existing 100-booking-instance monthly quota calculation; expired Free trials deny with a stable trial reason before querying usage; paid/legacy plans execute existing quota logic unchanged.
- Return separate trial/access and quota errors from Booking GraphQL.
- Apply the action-aware gate at Booking and Location mutation authorization boundaries and Organization-owned operational updates. Explicitly bypass only reads/exports, payment/upgrade/account operations, and cancellation/refund/closure of existing commitments.
- Retain server checks behind all disabled UI controls, including marketplace and recurring/background creation.

### 5. Operator and customer experiences

- Replace the Free quota progress surface in `webapp-spaces` with status, trial end, remaining days, 3-day warning, expired block, upgrade action, and complimentary-bridge/first-billing messaging.
- Load status in the organization root shell, provide it through a Spaces-local context, preserve navigation/read access, and disable create/edit controls while keeping permitted recovery/protective actions enabled.
- Update mutation error handling for stale-page expiry.
- Add a Booking-owned public GraphQL availability projection as an extension of the federated `Organization` type. Resolve it only from Booking's replicated Organization state; do not call Organization API at request time. Keep listings visible, disable booking/subscription CTAs, show neutral temporary unavailability, and handle authoritative mutation denial.
- Add accessible alert semantics, keyboard-reachable upgrade actions, and responsive tests.

### 6. Public website and generated surfaces

- Update all Spaces plan/pricing/FAQ/SEO/LLM content to describe a 14-day Free trial, the existing 100-booking monthly limit, explicit upgrade, complimentary remainder-of-month after upgrade, and first full charge on the next first day.
- Do not modify Teams catalog entries, copy, interfaces, or tests except negative regression assertions.
- Regenerate event outputs immediately after changing the protobuf source and before consumer implementation. Regenerate GraphQL schemas immediately after each server contract slice, then regenerate Relay outputs before changing the corresponding frontend consumers. Use `make generate` after implementation as a final drift check.

### 7. Verification and observability

- Unit-test exact start, day-11 warning, just-before expiry, exact expiry, leap/month boundaries, Teams-only first enablement, no reset, paid bridge, cancellation, payment failure, paid quota preservation, evaluator performance, and absence of booking-usage repository calls during a trial.
- Inventory and integration-test every booking-creation boundary, including administrator, customer, marketplace, recurring, import, automation/job, and direct service paths. Also cover Organization migration/persistence/outbox, Booking and Location projection, public availability, permitted read/export/account/billing/protective actions, support read-only visibility, paid upgrade retry, listing restoration, paid upgrade scheduling, and repository-visible preserved state.
- Add structured logs and tests asserting their level, event name, organization/product/correlation properties, success/failure/retry behavior, and absence of customer/payment-sensitive payloads. Do not label business denials as cache misses.
- Extend one high-value browser flow and add accessible alert semantics, keyboard-reachable upgrade actions, and responsive-state tests.
- Run focused suites, generation validation, builds/lints, then `graphify update .` after implementation.

## Complexity Tracking

| Design choice | Why needed | Simpler alternative rejected because |
|---|---|---|
| Action-aware shared Spaces access evaluator | Expiry blocks new work but permits reads, upgrades, and protective commitment handling across synchronous/background paths | A boolean interaction flag cannot represent exceptions and becomes stale when time changes without an event |
| Trial dates in replicated offering state | Booking and Location require local authorization without cross-domain runtime calls | Projected status alone becomes stale at the exact day-14 boundary |

## Phase 0: Research

See [research.md](./research.md). All technical-context decisions are resolved.

## Phase 1: Design and Contracts

- [data-model.md](./data-model.md)
- [contracts/access-policy.md](./contracts/access-policy.md)
- [contracts/event-spaces-trial.md](./contracts/event-spaces-trial.md)
- [contracts/graphql-spaces-trial.md](./contracts/graphql-spaces-trial.md)
- [contracts/billing-lifecycle.md](./contracts/billing-lifecycle.md)
- [quickstart.md](./quickstart.md)

## Post-Design Constitution Check

- [x] **I. Contract-First** — Contract documents identify source definitions and exact event/GraphQL/Relay generation order; no generated target is treated as editable source.
- [x] **II. Domain Boundaries** — Trial and billing ownership remains in Organization; dates flow over the existing event contract; Booking/Location use local projections; Booking owns public booking availability and final enforcement; frontend clients use GraphQL; APIs never synchronously call another domain API.
- [x] **III. Testing** — Data, workflow, projection, UI, and logging changes have proportionate unit/integration/web coverage with repository-only persistence assertions.
- [x] **IV. Frontend** — Operator/customer ownership, Relay colocation, design-system imports, responsive/accessibility validation, and American copy are explicit.
- [x] **V. Pattern Consistency** — New policy types extend the existing offering contract and solve an explicitly documented gap; no new service/domain/storage platform is introduced.
- [x] **VI. Logging** — Research, contracts, data model, strategy, and quickstart identify success, transition, denial, integration, failure, and retry signals with correlation context.
