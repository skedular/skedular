# Team API Agent Notes

This file covers `team/apis/`.

## Agent Rule

- Keep API surfaces thin.
- If the bug is about team data semantics, fix shared/domain logic instead of only patching transport.
- Do not reintroduce `hasFutureBooking` to team API surfaces.
- Do not reintroduce booking-derived read models into `team/apis/`; team booking questions belong to the booking domain.
