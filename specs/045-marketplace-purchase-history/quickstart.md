# Quickstart: Marketplace Purchase Lifecycle History

## Verify the contract

1. Create a subscription and a credit entitlement in test data.
2. Drive creation, activation/grant, payment, renewal/consumption, cancellation/expiration, and refund transitions.
3. Query the purchases list and confirm current snapshots remain present.
4. Query eligible detail history with a small page size; verify newest-first cursors.
5. Repeat the same transition delivery and confirm the event count is unchanged.
6. Refresh and deep-link to each eligible detail page; confirm events come from the response.
7. Open a one-time booking detail and confirm no history tab or history request exists.

## Verification commands after implementation

```bash
dotnet test src/booking/shared/Booking.Shared.UnitTests
dotnet test src/booking/apis/Booking.Api.UnitTests
dotnet test src/booking/domain/Booking.Domain.IntegrationTests
pnpm --dir src/web/apps/webapp test
scripts/generate-graphql.sh
pnpm --dir src/web relay
graphify update .
```

Migration verification must create a clean database, apply the migration, append events, rebuild a snapshot, and prove that no legacy-data backfill is required.
