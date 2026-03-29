# Organization API Agent Notes

This file covers `organization/apis/`.

## Agent Rule

- Keep transport code thin.
- If the issue is about billing settings or bank-account behavior, the real fix is usually in shared logic or persistence.
- `organization/apis/` should serve precomputed analytics/state from local organization storage.
- Do not reintroduce request-time organization-to-booking calls for analytics or booking-derived state.
- Do not reintroduce `hasFutureBooking` to GraphQL or REST surfaces in this domain.
