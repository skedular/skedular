# Research: Skedular Spaces Free Trial

## Decision: Organization Owns the One-Time Spaces Trial Anchor

**Rationale**: Organization already owns Spaces offering assignment, subscription changes, payment methods, and offering renewal. Add a nullable, immutable `SpacesTrialStartedAt` to the Organization-owned organization record. Set it to organization creation time for newly created Spaces/marketplace organizations and once at first Spaces enablement for an older Teams-only organization. Existing `SpacesFreeTierV1` organizations use the already-durable organization creation timestamp as their effective anchor after the rollout date, without a customer data backfill. The 14-day end instant and remaining days are derived from this anchor.

The trial changes the behavior and copy of `SpacesFreeTierV1`; it does not create a V2 offering or migrate subscription codes.

An organization-level anchor survives monthly offering renewal, plan upgrades, cancellations, deleted offering rows, and later downgrades. It therefore enforces the one-trial-per-organization rule more reliably than an offering-period field.

**Alternatives considered**:

- Use `OrganizationOffering.Start`. Rejected because offering start/end are calendar-month billing periods and are rewritten during plan changes and renewal.
- Use offering `CreatedAt`. Rejected because paid changes can replace offering rows and migration-created Free offerings may be newer than the organization.
- Store both trial start and end. Rejected because the end is deterministically `start + 14 days`; storing both creates drift risk.

## Decision: Derive Spaces Subscription Status From Durable Dates and Current Offering

**Rationale**: Status must change at the exact trial boundary even if no event or background job runs at that instant. A shared, deterministic evaluator accepts current UTC time, organization type, current offering, `SpacesTrialStartedAt`, and the action category. It returns a `SpacesAccessDecision` containing the current `SpacesSubscriptionStatus`, access flags, trial timing, remaining days, and stable reason code.

The status set is Spaces-specific: `TRIAL_ACTIVE`, `TRIAL_EXPIRING`, `TRIAL_EXPIRED`, `COMPLIMENTARY_BRIDGE`, `PAID_ACTIVE`, `PAID_INACTIVE`, `LEGACY_ACTIVE`, and `MISSING_STATE`. The warning transition begins when 3 or fewer whole days remain. Remaining days are `ceil((trialEndsAt - now) / 24 hours)` while active and `0` at or after expiry.

**Alternatives considered**:

- Persist a mutable status and run a timer at day 11/day 14. Rejected because delayed jobs could leave authorization stale.
- Trust a status projected at the last Organization event. Rejected because time passage alone changes trial state.
- Reuse generic `OrganizationOfferingPlanStatus`. Rejected because it cannot distinguish active/expiring/expired trial or the complimentary bridge without changing Teams semantics.

## Decision: Add a Shared Spaces Access Policy With Explicit Action Categories

**Rationale**: Expiry blocks new operational work but must preserve read/export/account/billing/upgrade access and cancellation/refund/closure of existing commitments. A boolean interaction flag is insufficient. Define action categories such as `READ`, `CREATE_OR_MODIFY`, `CREATE_BOOKING_INSTANCE`, `PROTECT_EXISTING_COMMITMENT`, and `ACCOUNT_OR_UPGRADE`. The evaluator denies create/modify and booking creation for an expired trial, while allowing protective and recovery actions.

`Api.Shared.Services` owns only the portable decision model/evaluator and remains compatible with `netstandard2.0`; callers pass `now` explicitly rather than adding `TimeProvider` to the shared project. Organization uses the source record. Booking and Location use the replicated offering fields and their injected clocks. Domain services retain responsibility for authentication and authorization in addition to this commercial-access decision.

**Alternatives considered**:

- Add only a global `IsInteractionAllowed` flag. Rejected because it becomes stale without an event and cannot represent protective exceptions.
- Enforce only in the Spaces frontend. Rejected because direct API calls, stale clients, imports, and background workflows must be blocked.
- Add a gateway-only middleware. Rejected because Temporal/background paths bypass the gateway and mutation-specific exceptions need domain context.

## Decision: Project Trial Dates, Not a Time-Sensitive Status, Through Organization Events

**Rationale**: Extend the existing organization event offering payload with `spacesTrialStartedAt`, `spacesTrialEndsAt`, and billing/access metadata needed by consuming domains. Booking and Location persist these values in their existing replicated Organization/Offering JSON state and evaluate current status locally using authoritative time. Organization remains the source of truth; consumers do not query Organization synchronously on each mutation.

The event mapper also carries enough product scope to ensure Teams/private offerings never enter Spaces trial evaluation. Event source changes require `api-definitions/events/generate.sh`; generated protobuf classes remain build outputs.

**Alternatives considered**:

- Project only `TRIAL_ACTIVE` or `TRIAL_EXPIRED`. Rejected because projected state would go stale at expiry.
- Make synchronous Organization calls during every booking/location mutation. Rejected because it couples domain availability and adds latency to critical paths.
- Create a new trial event topic. Rejected because the existing Organization upsert/offering projection already carries commercial state needed for authorization.

## Decision: Remove Booking-Count Enforcement Only for the Free Trial

**Rationale**: Keep the existing `SpacesFreeTierV1` booking-instance quota and update the server-driven catalog to explain that it applies during the 14-day trial. During `TRIAL_ACTIVE` and `TRIAL_EXPIRING`, Booking first confirms time-based access and then performs the normal Free usage count. At `TRIAL_EXPIRED`, it blocks before querying usage. Growth, Business, Contact Us, and legacy behavior keep their existing quota/capacity enforcement so existing paid subscriptions are unaffected.

The existing quota status API remains backward-compatible for paid clients but is extended with subscription/access fields. Free-trial clients receive no meaningful monthly quota and the Spaces app removes the Free usage progress display.

**Alternatives considered**:

- Set a very large Free quota. Rejected because the trial must be independent of counts and a numeric limit still encodes the wrong entitlement.
- Delete all quota infrastructure. Rejected because paid Spaces plans still use booking-instance capacity.
- Continue counting Free usage but do not enforce it. Rejected because it adds unnecessary work and encourages clients to keep presenting a quota-based model.

## Decision: Reuse Existing Booking Enforcement Boundaries

**Rationale**: `PrivateBookingService`, `MarketplaceBookingService`, and private recurring-booking integrations already call `ISpacesBookingQuotaService`. Evolve that service into access-first, quota-second evaluation. Active Free trials continue through the current Free quota logic; expired trials return `TRIAL_EXPIRED` before usage counting; paid plans continue through current quota logic. This covers administrator, customer marketplace, multi-instance, recurring, subscription-generated, import/direct service, and background creation paths without duplicating date calculations.

Booking GraphQL payloads add a distinct access error so clients can distinguish expiry from paid quota exhaustion and missing state. Protective cancellation/refund/closure paths remain authorized and do not call the create/modify action policy.

**Alternatives considered**:

- Create a parallel trial-only booking service. Rejected because it would drift from existing one-off and recurring quota boundaries.
- Treat trial expiry as `FREE_TIER_LIMIT_EXCEEDED`. Rejected because the reason is not usage-based and clients require a clear subscription status.

## Decision: Reuse the Existing Calendar-Month Paid Upgrade Workflow

**Rationale**: `OrganizationOfferingService.UpdateOfferingAsync` already requires a payment method for chargeable offerings, creates a paid offering starting immediately, sets its end to the next first-of-month boundary, and schedules `ScheduleRenewOrganizationOffering`. Reuse that validation, timing, outbox, and payment-intent infrastructure, but mark the initial interval as a complimentary bridge. The current generic workflow charges the expiring offering before it creates the renewed offering; the bridge branch must instead charge and persist the upcoming full calendar-month offering so the bridge itself remains uncharged and billing records describe the correct covered period.

Consolidate the specialized `OrganizationSpacesSubscriptionService.UpdateAsync` path with the same transition logic so no mutation can bypass payment-method validation, workflow cancellation/restart, outbox publication, or bridge semantics. At the bridge boundary, a Spaces-specific workflow/activity branch uses an idempotent upcoming-offering/payment key, charges the full upcoming month, activates that offering only through the successful transition path, and then resumes normal renewal. Existing paid offerings continue using their current workflow unchanged. Cancellation during the bridge cancels the workflow, creates no retroactive charge, and returns the organization to its already-expired Free trial state.

**Alternatives considered**:

- Add prorated immediate charging. Rejected by the accepted business decision.
- Automatically convert at trial expiry. Rejected because upgrade must be explicit.
- Add a separate checkout or Stripe subscription model. Rejected because the existing offering/payment-intent/Temporal flow already implements calendar-month charging and the feature must reuse supported flows.

## Decision: Separate Full Admin Status From Public Booking Availability

**Rationale**: Extend `organizationSpacesSubscription` with the full status, trial dates, remaining days, access flags, and next billing date for authenticated operator/support clients. Expose a minimal public booking-availability field containing only `canAcceptBookings` and neutral availability copy. Customer-facing marketplace pages can keep listings visible and disable booking/subscribe actions without exposing plan, trial, or billing details.

Booking mutation errors remain authoritative for stale pages. The customer app handles public availability preflight and server rejection; the operator app renders detailed warnings and upgrade controls.

**Alternatives considered**:

- Expose the full subscription object publicly. Rejected because customer messaging must not disclose private billing state.
- Hide expired listings. Rejected by the clarified requirement to preserve visibility.
- Leave booking controls enabled until submit. Rejected because it creates avoidable customer failure while still requiring server enforcement.

## Decision: Use Product-Specific Frontend Boundaries

**Rationale**: `webapp-spaces` owns operator/admin status, route-level warning/block presentation, and upgrade prompts. The shared `webapp` owns customer marketplace/listing/booking flows and consumes only public availability. `public-web` owns marketing pricing and machine-readable content. Relay queries stay colocated with consuming components, generated artifacts are regenerated, typography uses `@skedular/ui`, shared runtime helpers use `@skedular/shared`, and copy follows the repository's American-English rule.

**Alternatives considered**:

- Put customer marketplace changes in `webapp-spaces`. Rejected because current customer-facing marketplace routes live in `webapp`.
- Put trial state in a cross-product frontend package. Rejected because the behavior is Spaces-specific and must not affect Teams.

## Decision: Migrate Existing Organizations Without Deleting or Rewriting Product Data

**Rationale**: The Organization migration adds only the nullable trial-anchor column. Do not backfill customer rows. Existing Free Marketplace/Spaces organizations derive the effective anchor from their original organization `CreatedAt` whenever the stored anchor is absent; normal subscription writes persist that same fallback before a plan transition. Private/Teams-only organizations remain null until first Spaces enablement. Existing Free Spaces organizations older than 14 days therefore evaluate as expired immediately; younger organizations retain only their remaining time.

The existing hosted assignment service initializes missing Free Spaces offerings and trial anchors transactionally, publishes Organization state through the outbox, and logs counts. No Booking rows, listings, products, customers, resources, or configuration are changed.

**Alternatives considered**:

- Give all existing Free organizations 14 new days at deployment. Rejected because the specification anchors existing Spaces trials to original creation.
- Backfill every organization. Rejected because old Teams-only organizations must start when Spaces is first enabled.
- Persist expiry by deleting/ending listings or bookings. Rejected because data and configuration must be preserved.

## Decision: Regenerate Contracts and Test Cross-Domain Time Boundaries

**Rationale**: GraphQL fields/types are server-owned and require `scripts/generate-graphql.sh`; Relay artifacts then require `src/web/apps/webapp/scripts/generate.sh` (or `make generate`). Organization event protobuf changes require `api-definitions/events/generate.sh`. No OpenAPI change is required unless implementation discovers a non-GraphQL consumer that cannot use the existing surface.

Unit tests use injected `TimeProvider` in domain services and explicit `now` in the portable evaluator. Organization and Booking integration tests assert persistence through repositories, validate event projection, and cover exact day-11/day-14/month-boundary behavior. Vitest/React Testing Library cover operator and customer states; public-web tests cover pricing and machine-readable copy. Structured logs cover trial initialization/evaluation, access decisions, event projection, upgrade/bridge transitions, payment/renewal failures, and Teams-isolation branches.

**Alternatives considered**:

- Hand-edit schemas or Relay/protobuf outputs. Rejected by repository generation rules.
- Depend only on end-to-end tests. Rejected because time-boundary and decision-table behavior is faster and more deterministic at unit/integration tiers.
