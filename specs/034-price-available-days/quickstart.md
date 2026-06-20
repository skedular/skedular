# Quickstart Validation: Product Price Available Days

## Prerequisites

- Work from branch `034-price-available-days`.
- Run generators after contract changes: `api-definitions/events/generate.sh`, `scripts/generate-graphql.sh`, and `src/web/apps/webapp/scripts/generate.sh`.
- Use the existing local service and web-app instructions for the affected test projects.

## Validation Scenarios

1. In Skedular Host, create or edit three prices: unrestricted, Saturday only, and Wednesday plus Thursday. Confirm all seven day choices are available and an empty selection says “Every day.”
2. In Skedular Spaces and the customer booking view, inspect a restricted price. Confirm the applicable days are visible and a disallowed date cannot be practically selected.
3. Submit a direct booking on a disallowed day. Confirm rejection occurs before resource allocation, no booking persists, and the message distinguishes price-day eligibility from resource availability.
4. Submit a booking on an allowed day with no compatible resource. Confirm existing availability behavior blocks it.
5. Purchase a six-month Saturday-only subscription. Run or await reconciliation and confirm every generated instance is Saturday in the location timezone.
6. Change the price rule during an active subscription period. Confirm current-period generation retains the purchased rule; renew and confirm the next period follows the edited rule.
7. Exercise a UTC-boundary location/time case and a part-day booking. Confirm local start-day evaluation is used and the selected time/duration is unchanged.
8. Review the Host, Spaces, and public-site documentation surfaces for “available days,” Sunday-through-Saturday equality, empty-selection behavior, resource availability, and subscription renewal behavior.

## Focused Test Commands

- Run the affected Marketplace and Booking unit/integration test projects with `dotnet test` using their existing project paths.
- Run the affected Host, Spaces, and customer web tests with their existing package test commands.
- Run `git diff --check` and inspect generated schemas and Relay artifacts after regeneration.
