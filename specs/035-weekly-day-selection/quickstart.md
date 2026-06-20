# Quickstart Validation: Weekly Price Day Selection

## Prerequisites

- Work from branch `035-weekly-day-selection`.
- Run `api-definitions/events/generate.sh`, `scripts/generate-graphql.sh`, and `src/web/apps/webapp/scripts/generate.sh` after contract changes.
- Use the established local Booking, Marketplace, Host, Spaces, and public-site test instructions.

## Validation Scenarios

1. In Skedular Host and Skedular Spaces, configure a weekly price with Monday through Friday available and an exact required count of two. Verify that invalid values are rejected and non-weekly prices cannot configure the weekly value.
2. In the marketplace customer purchase flow, purchase the configured weekly price. Verify that the customer sees Monday through Friday only, must select exactly two days, cannot select duplicates or a third day, and retains the selection through checkout.
3. Submit a direct invalid purchase request with missing, too few, too many, duplicate, or unavailable selected days. Verify no subscription or recurring schedule persists.
4. Purchase a Tuesday-and-Wednesday pattern where resources exist only on other available weekdays. Verify that the allocator does not substitute other days, creates visible resource-less booking shells on the selected dates, retains payment, and exposes their pending-resource status to both customer and operator.
5. Make a compatible resource available later and run reconciliation. Verify it attaches to the existing untouched shell on its original selected date. In Host, edit another individual shell and verify it becomes an override while the subscription’s selected-day pattern remains unchanged.
6. In Host, cancel an individual shell that cannot be fulfilled. Verify only that Booking is canceled, the existing refund lifecycle starts, Spaces shows the cancellation/refund status or next step, and the remaining subscription schedule remains active.
7. Enable auto-renewal, renew a selected-day subscription, and verify the renewal keeps the selection. Exercise a selected-day capacity failure and confirm it creates/repairs a shell rather than choosing another day.
8. Review the Host editor, Spaces customer/operator views, and public documentation. Confirm clear American-English copy distinguishes available days, weekly selection counts, customer-selected days, resource-less booking shells, individual overrides, and individual refund resolution. Confirm weekday matching uses UTC calendar dates.

## Focused Test Commands

- Run affected Marketplace and Booking unit/integration projects with `dotnet test` using their existing project paths.
- Run affected Host and Spaces web tests with their existing package test commands.
- Run `git diff --check`; inspect generated event, GraphQL schema, and Relay artifacts after regeneration.
