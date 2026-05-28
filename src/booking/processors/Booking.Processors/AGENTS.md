# Booking Processors Project Notes

This file applies to `booking/processors/Booking.Processors`.

## Purpose

- This is the runnable processor host for the booking domain.
- Read the parent `booking/processors/AGENTS.md` for all rules governing this host.

## Agent Rule

- Keep `Program.cs` / `Extensions.cs` focused on host wiring.
- Do not add business logic directly here; delegate to `booking/shared/Booking.Shared`.
