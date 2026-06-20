# Quickstart: Validate Skedular Spaces Pricing Implementation

## Prerequisites

- Local backend and `src/web` dependencies are installed.
- Test infrastructure is available through the repo's existing Docker/Aspire setup.
- Source definitions are updated before generated files.

## Regenerate Contracts

Run GraphQL regeneration after backend GraphQL changes:

```bash
scripts/generate-graphql.sh
```

Run OpenAPI generation only if OpenAPI YAML was changed:

```bash
api-definitions/openapi/generate.sh
src/web/apps/webapp/scripts/generate.sh
```

Use `make generate` from the repo root when multiple generated surfaces are affected.

## Backend Validation

Run focused unit tests for shared catalog/offering behavior, Organization pricing, Booking quota enforcement, and rollover behavior:

```bash
dotnet test src/shared/Api.Shared.Services.UnitTests
dotnet test src/organization/apis/Organization.Api.UnitTests
dotnet test src/organization/shared/Organization.Shared.UnitTests
dotnet test src/booking/apis/Booking.Api.UnitTests
dotnet test src/booking/shared/Booking.Shared.UnitTests
```

Run integration tests for persistence, GraphQL contracts, Temporal rollover, booking creation paths, recurring generation, and cross-domain subscription projection:

```bash
dotnet test src/organization/domain/Organization.Domain.IntegrationTests
dotnet test src/booking/domain/Booking.Domain.IntegrationTests
```

Expected outcomes:

- Free allows 100 monthly booking instances and blocks the 101st.
- Growth allows 500 monthly booking instances and blocks the 501st.
- Rebooking/updating existing booking records does not increment usage.
- Multi-slot and recurring generation count each stored booking record.
- Failed booking creation after quota validation does not increment usage.
- Canceled booking instances remain counted for the period.
- Booking instances scheduled outside the current billing period are excluded from current-period usage.
- Parallel booking creation attempts may slightly exceed quota under concurrency; normal requests are blocked once the current persisted booking count reaches the quota.
- First-day-of-month Temporal rollover compatibility remains wired, but current usage is derived from the booking rows scheduled inside the active UTC billing period.
- Organizations without active Spaces subscription state are assigned Free during migration.
- Admin/custom capacity overrides are respected for Contact Us/Enterprise-style organizations.
- Generated Organization and composed GraphQL schemas expose both `TEAMS_V1` and `SPACES_V1` in `CatalogVersion`.

## Web Validation

Run Spaces web checks after adding pricing/quota status and upgrade/contact prompts:

```bash
CI=true pnpm --dir src/web --filter webapp-spaces lint
CI=true pnpm --dir src/web --filter webapp-spaces build
```

If package-level shared/UI code changes, also run the relevant package tests:

```bash
CI=true pnpm --dir src/web --filter @skedular/shared test
```

Expected outcomes:

- Spaces pricing and upgrade/contact prompts render from backend catalog data.
- Quota status reflects backend current usage, quota limit, and remaining quota.
- Quota-exceeded errors show backend-provided upgrade/contact options.
- No Spaces pricing values are hardcoded in frontend feature/page components.
- Typography wrappers come from `@skedular/ui`; shared runtime helpers come from `@skedular/shared`.
- User-facing copy uses American spelling.

## Operational Validation

Inspect structured logs for:

- Spaces pricing catalog retrieval and product filtering
- default Free assignment/migration decisions
- subscription update/admin override outcomes
- offering discount updates and renewal copy behavior
- quota allow/block decisions
- out-of-period booking instances excluded from current-period quota
- booking creation failure does not affect usage because failed attempts create no booking row
- recurring booking instance allow/block outcomes
- first-day-of-month rollover compatibility activity, if enabled
- catalog read p95 latency under 500 ms
- booking quota check p95 latency under 100 ms using current-period Booking row counts

Validate offering discount behavior with focused Organization tests:

- `DiscountPercentage` is never null and defaults to 0 for omitted admin/workaround input.
- Values outside 0 through 100 are rejected.
- Billing amount applies the discount to fixed-price and unit-price offerings.
- A 100% discount produces a zero charge without changing the offering plan, price, quota, or capacity.
- Renewed offerings copy the previous discount percentage until an admin resets it.

## Known Validation Gaps

- Catalog and quota p95 latency require production-like load testing infrastructure.
- Concurrent booking creation can exceed quota slightly because quota enforcement intentionally uses a simple count-before-create check instead of a transactional counter.

## Latest Validation Results

Last updated: 2026-06-17

- `pnpm --dir src/web --filter webapp-spaces relay` passed.
- `dotnet test src/shared/Api.Shared.Services.UnitTests/Api.Shared.Services.UnitTests.csproj --no-restore` passed: 20 tests.
- `dotnet test src/organization/apis/Organization.Api.UnitTests/Organization.Api.UnitTests.csproj --no-restore` passed: 68 tests.
- `dotnet test src/organization/shared/Organization.Shared.UnitTests/Organization.Shared.UnitTests.csproj --no-restore` passed: 29 tests.
- `dotnet test src/booking/apis/Booking.Api.UnitTests/Booking.Api.UnitTests.csproj --no-restore` passed: 101 tests.
- `dotnet test src/booking/shared/Booking.Shared.UnitTests/Booking.Shared.UnitTests.csproj --no-restore` passed: 296 tests.
- `dotnet test src/organization/domain/Organization.Domain.IntegrationTests/Organization.Domain.IntegrationTests.csproj --no-restore` passed: 12 tests.
- `dotnet test src/booking/domain/Booking.Domain.IntegrationTests/Booking.Domain.IntegrationTests.csproj --no-restore` passed: 27 tests.
- `pnpm --dir src/web --filter webapp-spaces test -- spaces-quota-upgrade-prompt.test.tsx` passed: 127 tests.
- `CI=true pnpm --dir src/web --filter webapp-spaces lint` passed.
- `CI=true pnpm --dir src/web --filter webapp-spaces build` passed.
- `graphify update .` was attempted but still failed locally with `[Errno 1] Operation not permitted`.
