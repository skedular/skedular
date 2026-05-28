# Location API Agent Notes

This file covers `location/apis/`.

## Agent Rule

- Keep API surfaces thin.
- If the issue is about availability or slot generation, the real fix is usually outside the controller layer.
- `location/apis/` should return precomputed analytics/state from local location storage.
- Do not reintroduce request-time location-to-booking calls for analytics or booking-derived state.
- Do not reintroduce `hasFutureBooking` to GraphQL or REST surfaces in this domain.
