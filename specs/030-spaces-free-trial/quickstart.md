# Quickstart: Validate Skedular Spaces Free Trial

## Prerequisites

- Repository dependencies are installed.
- Docker/Aspire dependencies required by Organization, Booking, and Location integration tests are available.
- Test clocks/fixtures can create organizations at fixed UTC instants.
- Source definitions are changed before generated outputs.

## Deterministic Clock and Fixture Conventions

- Use an injected or explicitly passed UTC `DateTimeOffset` in backend tests. Do not use wall-clock delays or derive expected values from `DateTimeOffset.UtcNow` inside assertions.
- Use `2026-07-01T10:00:00Z` as the default trial start and `2026-07-15T10:00:00Z` as its exact 14-day expiry unless a scenario specifically exercises a month, leap-day, or daylight-saving display boundary.
- Evaluate the active side at one tick before expiry and the blocked side at the exact expiry instant.
- Organization fixtures must declare product scope explicitly: Marketplace/Spaces at creation, pre-existing Private/Teams-only before first enablement, existing Free Spaces, existing paid Spaces, or legacy Spaces.
- Reuse one organization identifier across plan changes, cancellation, downgrade, and re-enable scenarios to prove `SpacesTrialStartedAt` is immutable.
- Booking and Location fixtures receive trial inputs only through their replicated Organization models or subscriber events. API tests must not call another domain API to arrange state.
- Frontend tests use fixed ISO timestamps in Relay fixtures and mock the application clock where relative-day rendering is involved.
- Integration persistence assertions go through repositories or query services, never `DbContext`.

## Regenerate Contracts

After changing the Organization event protobuf:

```bash
api-definitions/events/generate.sh
```

After changing Organization or Booking GraphQL server types/resolvers:

```bash
scripts/generate-graphql.sh
src/web/apps/webapp/scripts/generate.sh
```

Use the umbrella command when the final change spans multiple generated surfaces:

```bash
make generate
```

Expected generation outcome:

- Organization and Booking subgraph schemas expose the new status/access fields.
- The composed schema contains private subscription status, public availability, and Booking access-error contracts.
- `webapp-spaces` and `webapp` Relay artifacts match the composed schema.
- Event C# types are regenerated into build output; no generated protobuf classes are committed manually.

## Focused Backend Unit Tests

```bash
dotnet test src/shared/Api.Shared.Services.UnitTests/Api.Shared.Services.UnitTests.csproj
dotnet test src/organization/apis/Organization.Api.UnitTests/Organization.Api.UnitTests.csproj
dotnet test src/organization/shared/Organization.Shared.UnitTests/Organization.Shared.UnitTests.csproj
dotnet test src/booking/apis/Booking.Api.UnitTests/Booking.Api.UnitTests.csproj
dotnet test src/booking/shared/Booking.Shared.UnitTests/Booking.Shared.UnitTests.csproj
dotnet test src/booking/processors/Booking.Processors.UnitTests/Booking.Processors.UnitTests.csproj
dotnet test src/location/apis/Location.Api.UnitTests/Location.Api.UnitTests.csproj
dotnet test src/location/processors/Location.Processors.UnitTests/Location.Processors.UnitTests.csproj
```

Required unit scenarios:

- New Spaces organization starts at organization creation; older Teams-only organization starts at first Spaces enablement.
- Trial anchor cannot reset on disable/re-enable, upgrade, cancellation, renewal, or downgrade.
- Remaining days are 14 at start, 4 before warning, 3 in warning, 1 with less than 24 hours, and 0 at exact expiry.
- Active/expiring Free trials retain the existing 100-booking-instance monthly limit; expired trials deny before querying usage.
- Expired Free trial denies create/modify and all booking creation categories independent of usage.
- Expired Free trial allows read, export, account/upgrade, cancellation, refund, and closure actions.
- Protective actions cannot create replacement/renewal bookings.
- Growth, Business, Contact Us, and legacy quota decisions remain unchanged.
- Teams/private offerings never enter Spaces trial evaluation.
- Logs contain expected status/reason/action and no customer/payment payload.

## Persistence, Projection, and Workflow Integration Tests

```bash
dotnet test src/organization/domain/Organization.Domain.IntegrationTests/Organization.Domain.IntegrationTests.csproj
dotnet test src/booking/domain/Booking.Domain.IntegrationTests/Booking.Domain.IntegrationTests.csproj
dotnet test src/location/domain/Location.Domain.IntegrationTests/Location.Domain.IntegrationTests.csproj
```

Validate through repositories, never raw `DbContext`:

1. No customer-plan or trial-date backfill runs. Existing `SpacesFreeTierV1` organizations use `CreatedAt` as the effective anchor when the stored trial anchor is absent; Private/Teams-only organizations remain unaffected.
2. First Spaces enablement initializes an older Teams-only organization exactly once.
3. Organization outbox/event contains trial dates and Booking/Location projections retain them.
4. Projection consumers derive expiry using current time rather than an event-time status.
5. Private admin, marketplace customer, recurring, subscription-generated, multi-instance, and background booking paths allow during trial and block at exact expiry.
6. Existing bookings, recurring definitions, listings, products, customers, resources, and configuration remain queryable after expiry.
7. Location/resource/product operational mutations are denied after expiry while allowed exceptions continue.
8. Mid-month explicit upgrade requires a payment method, activates paid-plan access immediately, schedules first charge for next month start, and creates no partial-month charge.
9. The first boundary charge is linked to the upcoming full calendar-month offering and never to the complimentary bridge row.
10. Bridge cancellation stops the first charge, creates no retroactive invoice/payment intent, preserves trial anchor, and returns to expired Free access.
11. Existing paid Spaces offering periods, quotas, discounts, and workflow schedules remain unchanged.
12. Teams subscription and entitlement integration tests remain unchanged.

## GraphQL Contract Validation

Execute existing Organization and Booking GraphQL integration suites after generation. Confirm:

- Authenticated `organizationSpacesSubscription` returns status, trial times, remaining days, flags, reason, bridge state, and next billing date.
- Status choice/detail fields expose stable enum names.
- Public availability returns only availability code/boolean/neutral message.
- Free active trial reports and enforces the existing 100-booking-instance monthly quota.
- Expired booking mutation returns access error, not quota error.
- Paid quota exhaustion still returns the existing quota error.
- Stale client mutation at/after expiry is rejected server-side.

## Operator Web Validation

```bash
CI=true pnpm --dir src/web --filter webapp-spaces test
CI=true pnpm --dir src/web --filter webapp-spaces lint
CI=true pnpm --dir src/web --filter webapp-spaces build
```

Expected outcomes:

- Normal active-trial status shows end date, remaining days, and the existing monthly booking quota progress.
- Three-day warning is prominent and keyboard-accessible.
- Exact-expiry state disables new operational actions but preserves navigation/read/export/upgrade and protective actions.
- Upgrade prompt requires payment-method readiness and explains complimentary month-end access plus next first-day billing.
- Successful upgrade changes to bridge/paid presentation without republishing data.
- Server access error is handled correctly when a page was opened before expiry.
- Typography imports use `@skedular/ui`; no generated Relay file is hand-edited.

## Customer Marketplace Web Validation

```bash
CI=true pnpm --dir src/web --filter webapp test
CI=true pnpm --dir src/web --filter webapp lint
CI=true pnpm --dir src/web --filter webapp build
```

Expected outcomes:

- Expired operator listings remain visible.
- Booking and subscription CTAs are disabled with neutral temporary-unavailability copy.
- Trial/billing/private plan details are not exposed.
- Stale booking/subscribe submissions handle authoritative denial.
- Paid/active-trial storefront behavior remains unchanged.

## Public Website Validation

```bash
CI=true pnpm --dir src/web --filter public-web test
CI=true pnpm --dir src/web --filter public-web lint
CI=true pnpm --dir src/web --filter public-web build
```

Review `/pricing/spaces`, Spaces product content, SEO metadata, `llms.txt`, and `llms-full.txt`:

- Free is consistently labeled as a 14-day trial, not a permanent free tier.
- Copy states that the existing 100-booking-instance monthly limit applies during the trial.
- Copy explains explicit upgrade, complimentary access through month-end after upgrade, and first full charge on the next first day.
- No Spaces content presents the 100-booking-instance allowance as a permanent free entitlement.
- Teams pricing names, prices, quotas, and copy remain byte-for-byte unchanged except tests may add negative assertions.

## End-to-End Boundary Scenarios

Use fixed timestamps rather than waiting in real time:

1. Create a Spaces organization at `2026-07-01T10:00:00Z`; verify expiry at `2026-07-15T10:00:00Z`.
2. At one tick before expiry, verify booking instance 100 succeeds and instance 101 is rejected by the existing monthly quota without changing the trial end.
3. At exact expiry, verify operator booking, customer booking, recurring generation, and resource/product mutations are blocked.
4. Verify reads and cancellation/refund/closure remain available and all persisted data is unchanged.
5. Add/verify a payment method and upgrade on July 15; access restores immediately, no July charge is created, and first full charge is scheduled for August 1.
6. Repeat with cancellation on July 31; verify no July/August charge, no trial reset, and expired Free status.
7. Run matching paid Spaces and Teams controls to prove no regression.

## Operational Validation

Inspect structured logs for:

- trial anchor initialized/derived-from-creation/already-present;
- status evaluated as active, expiring, expired, bridge, or paid;
- booking and operational action allowed/denied with stable reason;
- missing projection/state fail-closed recovery;
- Organization event publish and consumer projection;
- paid upgrade requested/completed/rejected;
- complimentary bridge start and scheduled first billing date;
- charge/renewal success, retry, exhaustion, and cancellation;
- public availability decision without commercial detail;
- Teams/private bypass.

Verify organization/request/workflow correlation fields are present and no card secrets, customer data, or booking payloads are logged.

## Final Consistency Checks

```bash
git diff --check
graphify update .
```

No OpenAPI generation is expected unless implementation adds or changes an OpenAPI source contract. If that occurs, update YAML first and run the repository's OpenAPI/web client generators.

## Validation Results (2026-06-27)

- `make generate`: passed after removing a stale Git index lock; GraphQL composition and all generated web clients completed.
- Relay compilation: passed for `webapp` and `webapp-spaces`.
- Backend unit suites: Api.Shared.Services 61/61, Booking.Shared 304/304, Organization.Api 86/86, Organization.Shared 40/40, Location.Api 47/47, Marketplace API 7/7, and Marketplace processor 4/4.
- Booking repository-backed trial/quota integration target compiled and reached test discovery after adding active-trial 100/101 and exact-expiry preservation scenarios, but the integration host did not emit an xUnit completion summary or terminate and was canceled; this matches the Booking test-host lifecycle issue below and is not recorded as a passing test run.
- Focused recurring bridge/activity tests: passed, including expired marketplace-subscription suppression and first-of-month full-period renewal.
- `webapp-spaces`: lint, 143/143 tests, and production build passed.
- customer `webapp`: lint, focused location/product tests, and production build passed.
- `public-web`: focused pricing/LLM tests 4/4 and production build/Astro check passed.
- Booking.Api test host compiled successfully but did not terminate after test discovery in two runs; the process was canceled and is recorded as a test-host issue rather than a passing suite.
