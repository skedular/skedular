# Booking Jobs Project Notes

This file applies to `booking/jobs/Booking.Jobs`.

## Purpose

- This is the runnable job host for the booking domain.
- Read the parent `booking/jobs/AGENTS.md` for all rules governing this host.

## Agent Rule

- Keep `Program.cs` / `Extensions.cs` focused on host wiring.
- Do not add business logic directly here; delegate to `booking/shared/Booking.Shared`.
